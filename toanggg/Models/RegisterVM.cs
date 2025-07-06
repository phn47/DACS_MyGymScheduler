using System.ComponentModel.DataAnnotations;

namespace toanggg.Models
{
    public class RegisterVM
    {
     
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string Password { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string Email { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string FullName { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string Gender { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public DateOnly?  Dob { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string Address { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string Avatar { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string Role { get; set; }

        [Display(Name = "Tên đăng nhập")]   
        public string Status { get; set; }

        public int? MembershipTypeId { get; set; }
        public string? Specialization { get; set; }

        public int? Experience { get; set; }

        public string? Description { get; set; }

        public decimal? PricePerSession { get; set; }

        public decimal? Rating { get; set; }


    }
}
