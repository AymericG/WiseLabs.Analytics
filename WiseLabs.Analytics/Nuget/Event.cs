using System;

namespace WiseLabs.Analytics
{
    public class Event
    {
        public string CohortName { get; set; }
        public string EventName { get; set; }
        public int EventCount { get; set; }
        public long EventId { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}