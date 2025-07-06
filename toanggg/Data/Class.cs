using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace toanggg.Data;

public partial class Class
{
    public int ClassId { get; set; }
    [Display(Name = "Tên lớp")]
    public string? Name { get; set; }
    [Display(Name = "Giờ bắt đầu")]
    public TimeOnly? StartTime { get; set; }
    [Display(Name = "Giờ kết thúc")]
    public TimeOnly? EndTime { get; set; }
    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }
    [Display(Name = "Loại lớp")]
    public string? ClassType { get; set; }

    public int? GymId { get; set; }

    public int? TrainerId { get; set; }

    public virtual ICollection<ClassSchedule> ClassSchedules { get; set; } = new List<ClassSchedule>();

    public virtual Gym? Gym { get; set; }

    public virtual Trainer? Trainer { get; set; }

    public virtual ICollection<MembershipType> MembershipTypes { get; set; } = new List<MembershipType>();
}
