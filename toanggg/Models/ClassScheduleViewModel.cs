namespace toanggg.Models
{
    public class ClassScheduleViewModel
    {
        public int ScheduleId { get; set; }
        public string Weekday { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public List<StudentViewModel> Students { get; set; }
    }
}
