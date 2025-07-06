using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace toanggg.Data;

public partial class GymType
{
    public int GymTypeId { get; set; }
    [Display(Name = "Tên phòng tập")]
    public string? Name { get; set; }
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    public virtual ICollection<MembershipType> MembershipTypes { get; set; } = new List<MembershipType>();

    public virtual ICollection<Gym> Gyms { get; set; } = new List<Gym>();
}
