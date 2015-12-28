using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Transactions;

namespace WiseLabs.Analytics
{
    public class Event
    {
        public string CohortName { get; set; }
        public string EventName { get; set; }
        public int EventCount { get; set; }
    }

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
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand("SELECT * From Analytics_Event", connection))
            {
                connection.Open();

                using (var reader = command.ExecuteReader())
                {
                    var list = new List<Event>();
                    while (reader.Read())
                    {
                        list.Add(new Event()
                        {
                            EventCount = (int)reader["EventCount"],
                            EventName = (string)reader["EventName"],
                            CohortName = (string)reader["CohortName"]
                        });
                    }
                    return list;
                }
            }
        } 

        public void TrackEvent(string cohortName, string eventName)
        {
            using (var transaction = new TransactionScope())
            {
                // get cohort pair
                int? currentEventCount = null;
                using (var connection = new SqlConnection(ConnectionString))
                using (var command = new SqlCommand("SELECT EventCount From Analytics_Event where CohortName = @CohortName and EventName = @EventName", connection))
                {
                    command.Parameters.AddWithValue("@CohortName", cohortName);
                    command.Parameters.AddWithValue("@EventName", eventName);

                    connection.Open();
                    currentEventCount = (int?) command.ExecuteScalar();
                }

                if (currentEventCount.HasValue)
                {
                    // update count
                    UpdateCohortEventPair(cohortName, eventName);
                }
                else
                {
                    // if not exist create
                    InsertNewCohortEventPair(cohortName, eventName);
                }
                transaction.Complete();
            }
        }

        private void InsertNewCohortEventPair(string cohortName, string eventName)
        {
            // write your data layer like it is year 2004.
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand("INSERT INTO Analytics_Event (CohortName, EventName, EventCount) VALUES (@CohortName, @EventName, 1)", connection))
            {
                command.Parameters.AddWithValue("@CohortName", cohortName);
                command.Parameters.AddWithValue("@EventName", eventName);
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private void UpdateCohortEventPair(string cohortName, string eventName)
        {
            // write your data layer like it is year 2004.
            using (var connection = new SqlConnection(ConnectionString))
            using (var command = new SqlCommand("UPDATE Analytics_Event SET EventCount = EventCount + 1 WHERE CohortName = @CohortName AND EventName = @EventName", connection))
            {
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
    [CohortName] [nvarchar](max) NOT NULL,
    [EventName] [nvarchar](max) NOT NULL,
    [EventCount] int NOT NULL,
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