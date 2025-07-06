using toanggg.Data;

namespace toanggg.Models
{
    public class ConflictViewModel
    {
        public string TrainerFullName { get; set; }
        public List<ClassSchedule> ConflictingSchedules { get; set; }
    }
}
