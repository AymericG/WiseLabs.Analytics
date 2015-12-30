namespace WiseLabs.Analytics
{
    public class ExperimentEvent
    {
        public string EventName { get; set; }
        public int EventCount { get; set; }
        public short Variation { get; set; }
        public long ExperimentId { get; set; }
    }
}