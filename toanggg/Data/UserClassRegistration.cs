using System;
using System.Collections.Generic;

namespace toanggg.Data;

public partial class UserClassRegistration
{
    public int RegistrationId { get; set; }

    public int? UserId { get; set; }

    public int? ClassScheduleId { get; set; }

    public DateOnly? RegistrationDate { get; set; }

    public string? Status { get; set; }

    public virtual ClassSchedule? ClassSchedule { get; set; }

    public virtual User? User { get; set; }
}
