namespace toanggg.Models
{
    public class lichtrinhVM
    {
        public int ScheduleId { get; set; }

        public int? ClassId { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Weekday { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public string? Room { get; set; }

        public string? Status { get; set; }
    }
}
