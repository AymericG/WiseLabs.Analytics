using System.Collections.Generic;

namespace WiseLabs.Analytics
{
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
            DataLayer.CreateTableIfNeeded();
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