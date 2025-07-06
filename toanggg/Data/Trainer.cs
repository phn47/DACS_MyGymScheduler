using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace toanggg.Data;

public partial class Trainer
{
    public int TrainerId { get; set; }
    [Display(Name = "Họ tên")]
    public string? FullName { get; set; }
    [Display(Name = "SDT")]
    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }
    [Display(Name = "Chuyên môn")]
    public string? Expertise { get; set; }
    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    public int? GymId { get; set; }

    public int? UserId { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual Gym? Gym { get; set; }

    public virtual User? User { get; set; }
}
