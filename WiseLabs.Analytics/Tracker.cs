using System;
using System.Collections.Generic;
using System.Linq;

namespace WiseLabs.Analytics
{
    public class Experiment
    {
        public string Name { get; set; }
        public DateTime CreatedAt { get; set; }
        public long ExperimentId { get; set; }
    }
    public static class SplitTesting
    {
        static IDataLayer _dataLayer;
        private static IDataLayer DataLayer
        {
            get { return _dataLayer ?? (_dataLayer = new SqServerDataLayer()); }
            set { _dataLayer = value; }
        }

        static readonly List<Experiment> _cachedExperiments = new List<Experiment>(); 
        public static void Experiment(
            string userId, 
            string experimentName, 
            params Action[] variations)
        {
            var experiment = GetOrCreateExperiment(experimentName);
            var variation = PickVariation(userId, variations.Length);
            DataLayer.CreateExperimentUserOnce(userId, experiment.ExperimentId, variation);
            
            // execute variation
            variations[variation]();
        }

        private static int PickVariation(string userId, int variationCount)
        {
            return Math.Abs(userId.GetHashCode() % variationCount);
        }

        private static Experiment GetOrCreateExperiment(string experimentName)
        {
            // create experiment if does not exist
            var experiment = _cachedExperiments.SingleOrDefault(x => x.Name == experimentName);
            if (experiment != null)
            {
                return experiment;
            }

            experiment = DataLayer.GetOrCreateExperiment(experimentName);
            if (experiment == null)
            {
                return null;
            }

            _cachedExperiments.Add(experiment);
            return experiment;
        }
    }

    public static class Tracker
    {
        static IDataLayer _dataLayer;
        private static IDataLayer DataLayer
        {
            get { return _dataLayer ?? (_dataLayer = new SqServerDataLayer()); }
            set { _dataLayer = value; }
        }

        public static void SetUp()
        {
            DataLayer.CreateTablesIfNeeded();
        }

        public static List<Event> GetEvents()
        {
            return DataLayer.GetEvents();
        } 

        public static void Track(string userId, string cohortName, string eventName)
        {
            // We need userId to check whether this event has already been reported.
            DataLayer.InsertEventOnce(userId, cohortName, eventName);
        }
    }
}