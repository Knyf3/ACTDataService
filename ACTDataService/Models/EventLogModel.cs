namespace ACTDataService.Models
{
    public class EventLogModel
    {
        public long EventID { get; set; }
        public DateTime When { get; set; }
        public long TimeStamp { get; set; }
        public int Event { get; set; }
        public int Controller { get; set; }
        public int Door { get; set; }
        public long EventData { get; set; }
        public string OriginalForename { get; set; }
        public string OriginalSurname { get; set; }
    }
}
