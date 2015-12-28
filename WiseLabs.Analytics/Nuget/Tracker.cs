using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

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

        public static void Track(string cohortName, string eventName)
        {
            DataLayer.TrackEvent(cohortName, eventName);
        }
    }
}