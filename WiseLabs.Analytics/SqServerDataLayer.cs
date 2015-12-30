using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace WiseLabs.Analytics
{
    public class SqServerDataLayer : IDataLayer
    {
        private string ConnectionString
        {
            get
            {
                var connectionStringName = ConfigurationManager.AppSettings["WiseLabs.Analytics.ConnectionString.Name"];
                if (connectionStringName != null)
                {
                    return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
                }
                return ConfigurationManager.AppSettings[ConfigurationManager.AppSettings["WiseLabs.Analytics.ConnectionString.AppSettings.Name"]];
            }
        }


        public List<Event> GetEvents()
        {
            const string QueryCommandText = @"
SELECT EventName, CohortName, COUNT(*) as EventCount 
    FROM Analytics_Event 
    GROUP BY CohortName, EventName";
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(QueryCommandText, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    var list = new List<Event>();
                    while (reader.Read())
                    {
                        list.Add(ToEvent(reader));
                    }
                    return list;
                }
            }
        }
        public List<Experiment> GetExperiments()
        {
            const string QueryCommandText = @"SELECT * FROM Analytics_Experiment";
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(QueryCommandText, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    var list = new List<Experiment>();
                    while (reader.Read())
                    {
                        list.Add(ToExperiment(reader));
                    }
                    return list;
                }
            }
        }

        public List<ExperimentEvent> GetEventsForExperiments()
        {
            const string QueryCommandText = @"
SELECT u.ExperimentId, u.Variation, e.EventName, COUNT(*) as EventCount 
    FROM Analytics_Event e
    JOIN Analytics_ExperimentUser u ON u.UserId = e.UserId
    WHERE e.CreatedAt > u.CreatedAt
    GROUP BY u.ExperimentId, u.Variation, e.EventName";
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(QueryCommandText, connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    var list = new List<ExperimentEvent>();
                    while (reader.Read())
                    {
                        list.Add(ToExperimentEvent(reader));
                    }
                    return list;
                }
            }
        }

        private ExperimentEvent ToExperimentEvent(SqlDataReader reader)
        {
            var experimentEvent = new ExperimentEvent();
            experimentEvent.EventName = (string)reader["EventName"];
            experimentEvent.ExperimentId = (long)reader["ExperimentId"];
            experimentEvent.Variation = (short)reader["Variation"];
            experimentEvent.EventCount = (int)reader["EventCount"];
            return experimentEvent;
        }

        private static Event ToEvent(SqlDataReader reader)
        {
            return new Event()
            {
                EventName = (string)reader["EventName"],
                CohortName = (string)reader["CohortName"],
                EventCount = (int)reader["EventCount"]
            };
        }

        public void InsertEventOnce(string userId, string cohortName, string eventName)
        {
            //if not exists (select * from Table with (updlock, rowlock, holdlock) where ...)
            const string InsertCommandText = @"
IF NOT EXISTS 
    (SELECT * 
        FROM Analytics_Event WITH (updlock, rowlock, holdlock) 
        WHERE UserId = @UserId 
        AND CohortName = @CohortName 
        AND EventName = @EventName)
BEGIN
    INSERT INTO Analytics_Event (UserId, CohortName, EventName) 
        VALUES (@UserId, @CohortName, @EventName)
END";

            // write your data layer like it is year 2004.
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(InsertCommandText, connection))
            {
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@CohortName", cohortName);
                command.Parameters.AddWithValue("@EventName", eventName);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        public void CreateTablesIfNeeded()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                CreateAnalyticsEventTable(connection);
                CreateAnalyticsExperimentTables(connection);
            }
        }

        public Experiment GetOrCreateExperiment(string experimentName)
        {
            //if not exists (select * from Table with (updlock, rowlock, holdlock) where ...)
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();

                var experiment = GetExperiment(connection, experimentName);
                if (experiment == null)
                {
                    experiment = InsertExperiment(connection, experimentName);
                }
                return experiment;
            }
        }

        public void CreateExperimentUserOnce(string userId, long experimentId, int variation)
        {
            //if not exists (select * from Table with (updlock, rowlock, holdlock) where ...)
            const string InsertCommandText = @"
IF NOT EXISTS 
    (SELECT * 
        FROM Analytics_ExperimentUser WITH (updlock, rowlock, holdlock) 
        WHERE UserId = @UserId 
        AND ExperimentId = @ExperimentId)
BEGIN
    INSERT INTO Analytics_ExperimentUser (UserId, ExperimentId, Variation) 
        VALUES (@UserId, @ExperimentId, @Variation)
END";

            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand(InsertCommandText, connection))
            {
                command.Parameters.AddWithValue("@ExperimentId", experimentId);
                command.Parameters.AddWithValue("@UserId", userId);
                command.Parameters.AddWithValue("@Variation", variation);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private Experiment GetExperiment(SqlConnection connection, string experimentName)
        {
            const string QueryCommandText = @"
SELECT ExperimentId, Name, CreatedAt 
    FROM Analytics_Experiment 
    WHERE Name = @ExperimentName";

            using (var command = new SqlCommand(QueryCommandText, connection))
            {
                command.Parameters.AddWithValue("@ExperimentName", experimentName);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        return ToExperiment(reader);
                    }
                    return null;
                }
            }
        }

        private Experiment InsertExperiment(SqlConnection connection, string experimentName)
        {
            const string InsertCommandText = @"INSERT INTO Analytics_Experiment (Name) output INSERTED.ExperimentId VALUES (@ExperimentName)";

            using (var command = new SqlCommand(InsertCommandText, connection))
            {
                command.Parameters.AddWithValue("@ExperimentName", experimentName);
                return new Experiment()
                {
                    CreatedAt = DateTime.UtcNow,
                    ExperimentId = (long) command.ExecuteScalar(),
                    Name = experimentName
                };
            }
        }

        private Experiment ToExperiment(SqlDataReader reader)
        {
            return new Experiment()
            {
                Name = reader["Name"].ToString(),
                CreatedAt = (DateTime)reader["CreatedAt"],
                ExperimentId = (long) reader["ExperimentId"]
            };
        }

        private static void CreateAnalyticsEventTable(SqlConnection connection)
        {
            try
            {
                using (var command = new SqlCommand("SELECT * FROM Analytics_Event WHERE 1 = 0", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message == "Invalid object name 'Analytics_Event'.")
                {
                    const string createTableCommand = @"CREATE TABLE [dbo].[Analytics_Event](
    [EventId] [bigint] IDENTITY(1,1) NOT NULL,
    [UserId] [nvarchar](max) NOT NULL,
    [CohortName] [nvarchar](max) NOT NULL,
    [EventName] [nvarchar](max) NOT NULL,
    [CreatedAt] datetime NOT NULL DEFAULT(GETUTCDATE()),
 CONSTRAINT [PK_Analytics_Event] PRIMARY KEY CLUSTERED 
(
    [EventId] ASC
))
";
                    using (var command = new SqlCommand(createTableCommand, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        private static void CreateAnalyticsExperimentTables(SqlConnection connection)
        {
            try
            {
                using (var command = new SqlCommand("SELECT * FROM Analytics_Experiment WHERE 1 = 0", connection))
                {
                    command.ExecuteNonQuery();
                }
            }
            catch (SqlException ex)
            {
                if (ex.Message == "Invalid object name 'Analytics_Experiment'.")
                {
                    const string createTableCommand = @"CREATE TABLE [dbo].[Analytics_Experiment](
    [ExperimentId] [bigint] IDENTITY(1,1) NOT NULL,
    [Name] [nvarchar](max) NOT NULL,
    [CreatedAt] [datetime] NOT NUlL DEFAULT (getutcdate()),
 CONSTRAINT [PK_Analytics_Experiment] PRIMARY KEY CLUSTERED 
(
    [ExperimentId] ASC
))

CREATE TABLE [dbo].[Analytics_ExperimentUser](
    [ExperimentUserId] [bigint] IDENTITY(1,1) NOT NULL,
    [ExperimentId] [bigint] NOT NULL,
    [UserId] [nvarchar](max) NOT NULL,
    [Variation] [smallint] NOT NULL,
    [CreatedAt] [datetime] NOT NULL DEFAULT (getutcdate()),
 CONSTRAINT [PK_Analytics_ExperimentUser] PRIMARY KEY CLUSTERED 
(
    [ExperimentUserId] ASC
))
";
                    using (var command = new SqlCommand(createTableCommand, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}