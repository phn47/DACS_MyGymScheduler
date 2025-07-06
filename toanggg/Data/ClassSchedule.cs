using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace toanggg.Data;

public partial class ClassSchedule
{
    public int ScheduleId { get; set; }

    public int? ClassId { get; set; }
    [Display(Name = "Ngày bắt đầu")]
    public DateOnly? StartDate { get; set; }

    public int? UserId { get; set; }
    [Display(Name = "Ngày kết thúc")]
    public DateOnly? EndDate { get; set; }
    [Display(Name = "Thứ")]
    public string? Weekday { get; set; }
    [Display(Name = "Giờ bắt đầu")]
    public TimeOnly? StartTime { get; set; }
    [Display(Name = "Giờ kết thúc")]
    public TimeOnly? EndTime { get; set; }
    [Display(Name = "Phòng")]
    public string? Room { get; set; }
    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    public bool? IsemailOnfirmed { get; set; }

    public string? EmailconfirmationToken { get; set; }
    [Display(Name = "Loại lớp")]
    public virtual Class? Class { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<UserClassRegistration> UserClassRegistrations { get; set; } = new List<UserClassRegistration>();
}
