namespace toanggg.Models
{
    public class ClassSchedulee
    {
        public int class_id { get; set; }
        public string name { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan end_time { get; set; }
        public string status { get; set; }
        public string class_type { get; set; }
        public int gym_id { get; set; }
        public int trainer_id { get; set; }
        public string weekday { get; set; }
    }
    public class Trainer2
    {
        public int TrainerId { get; set; }
        public string FullName { get; set; }
        // Thêm các thuộc tính khác của Trainer nếu cần thiết
    }
}
