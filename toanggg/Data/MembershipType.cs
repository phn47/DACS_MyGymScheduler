using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace toanggg.Data;

public partial class MembershipType
{
    public int MembershipTypeId { get; set; }
    [Display(Name = "Tên gói")]
    public string? Name { get; set; }
    [Display(Name = "Mô tả")]
    public string? Description { get; set; }

    public int? GymTypeAccess { get; set; }
    [Display(Name = "Giờ truy cập")]
    public string? AccessHours { get; set; }
    [Display(Name = "Giá")]
    public decimal? Price { get; set; }
    [Display(Name = "Trạng thái")]
    public string? Status { get; set; }

    public virtual GymType? GymTypeAccessNavigation { get; set; }

    public virtual ICollection<UserMembership> UserMemberships { get; set; } = new List<UserMembership>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();

    public virtual ICollection<Class> Classes { get; set; } = new List<Class>();
}
