using System;
using System.Collections.Generic;

namespace toanggg.Data;

public partial class User
{
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

    public string? Status { get; set; }

    public int? MembershipTypeId { get; set; }

    public string? Confirmemail { get; set; }

    public string? EmailconfirmationToken { get; set; }

    public bool? IsemailOnfirmed { get; set; }

    public string? FacebbookId { get; set; }

    public string? ProfilePictureUrl { get; set; }

    public DateOnly? CreatedAt { get; set; }

    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

    public virtual MembershipType? MembershipType { get; set; }

    public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();

    public virtual ICollection<UserClassRegistration> UserClassRegistrations { get; set; } = new List<UserClassRegistration>();

    public virtual ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();
}
