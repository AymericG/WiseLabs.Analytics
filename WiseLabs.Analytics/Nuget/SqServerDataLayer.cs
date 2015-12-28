using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Transactions;

namespace WiseLabs.Analytics
{
    public class SqServerDataLayer : IDataLayer
    {
        private string ConnectionString
        {
            get
            {
                var connectionStringName = ConfigurationManager.AppSettings["WiseLabs.Analytics.ConnectionString.Name"];
                return ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
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

        private static Event ToEvent(SqlDataReader reader)
        {
            return new Event()
            {
                EventName = (string)reader["EventName"],
                CohortName = (string)reader["CohortName"],
                EventCount = (int)reader["EventCount"]
            };
        }

        public void TrackEvent(string userId, string cohortName, string eventName)
        {
            using (var transaction = new TransactionScope())
            {
                InsertEventOnce(userId, cohortName, eventName);
                transaction.Complete();
            }
        }

        
        private void InsertEventOnce(string userId, string cohortName, string eventName)
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

        public void CreateTableIfNeeded()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
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
        }
    }
}