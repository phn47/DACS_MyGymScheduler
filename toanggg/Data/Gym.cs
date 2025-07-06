using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace toanggg.Data;

public partial class Gym
{
    public int GymId { get; set; }
    [Display(Name = "Tên phòng tập")]
    public string? Name { get; set; }
    [Display(Name = "Địa chỉ")]
    public string? Address { get; set; }
    [Display(Name = "SDT")]
    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public string? Description { get; set; }

    public string? Website { get; set; }
    [Display(Name = "Giờ mở cửa")]
    public TimeOnly? OpenHours { get; set; }
    [Display(Name = "Giờ đóng cửa")]
    public TimeOnly? CloseHours { get; set; }

    public string? PriceRange { get; set; }

    public string? Amenities { get; set; }

    public string? Images { get; set; }

    public decimal? Rating { get; set; }
    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();

    public virtual ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();

    public virtual ICollection<GymType> GymTypes { get; set; } = new List<GymType>();
}
