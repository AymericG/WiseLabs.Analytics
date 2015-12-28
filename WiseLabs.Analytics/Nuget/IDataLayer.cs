using System.Collections.Generic;

namespace WiseLabs.Analytics
{
    public interface IDataLayer
    {
        List<Event> GetEvents();
        void TrackEvent(string userId, string cohortName, string eventName);
        void CreateTableIfNeeded();
    }
}