namespace toanggg.Models
{
    public class ClassScheduleVM
    {
        public int Id { get; set; }
        public string Weekday { get; set; }
        public TimeOnly StartTime { get; set; }
        public int ClassId { get; set; }
    }
}
