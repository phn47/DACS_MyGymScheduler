namespace toanggg.Models
{
    public class ClassVM
    {
        public int ClassId { get; set; }

        public string? Name { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public string? Status { get; set; }

        public string? ClassType { get; set; }

        public int? GymId { get; set; }

        public int? TrainerId { get; set; }


    }
}
