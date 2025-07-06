using System;
using System.Collections.Generic;

namespace toanggg.Data;

public partial class UserMembership
{
    public int UserMembershipId { get; set; }

    public int? UserId { get; set; }

    public int? MembershipTypeId { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public string? Status { get; set; }

    public virtual MembershipType? MembershipType { get; set; }

    public virtual User? User { get; set; }
}
