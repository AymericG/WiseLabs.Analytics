using System;

namespace WiseLabs.Analytics
{
    public class Event
    {
        public string CohortName { get; set; }
        public string EventName { get; set; }
        public int EventCount { get; set; }
    }
}