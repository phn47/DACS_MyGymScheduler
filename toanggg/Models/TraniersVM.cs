using toanggg.Data;

namespace toanggg.Models
{
    public class TraniersVM
    {
        public int ClassId { get; set; }

        public string? Name { get; set; }

        public TimeOnly? StartTime { get; set; }

        public TimeOnly? EndTime { get; set; }

        public string? Status { get; set; }

        public string? ClassType { get; set; }

        public int? GymId { get; set; }

        public int? TrainerId { get; set; }

        public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

        public virtual Gym? Gym { get; set; }

        public virtual Trainer? Trainer { get; set; }

        public virtual ICollection<MembershipType> MembershipTypes { get; set; } = new List<MembershipType>();

        public int UserId { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        public string? FullName { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Gender { get; set; }

        public DateOnly? Dob { get; set; }

        public string? Address { get; set; }

        public string? Avatar { get; set; }

        public string? Role { get; set; }

     /*   public string? Status { get; set; }
*/
        public int? MembershipTypeId { get; set; }

        public virtual MembershipType? MembershipType { get; set; }

        public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();

        public virtual ICollection<UserClassRegistration> UserClassRegistrations { get; set; } = new List<UserClassRegistration>();

        public virtual ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();
    }
}
