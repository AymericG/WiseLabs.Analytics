using System.Collections.Generic;

namespace WiseLabs.Analytics
{
    public interface IDataLayer
    {
        List<Event> GetEvents();
        List<Experiment> GetExperiments();
        List<ExperimentEvent> GetEventsForExperiments(); 
        void InsertEventOnce(string userId, string cohortName, string eventName);
        void CreateTablesIfNeeded();
        Experiment GetOrCreateExperiment(string experimentName);
        void CreateExperimentUserOnce(string userId, long experimentId, int variation);
    }
}