using System.Collections.Generic;

namespace WiseLabs.Analytics
{
    public interface IDataLayer
    {
        List<Event> GetEvents();
        void InsertEventOnce(string userId, string cohortName, string eventName);
        void CreateTableIfNeeded();
    }
}