using System.ComponentModel.DataAnnotations;

namespace toanggg.Models
{
    public class LoginVM
    {
        [Display(Name = "Tên đăng nhập")]
        public string Username { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string Password { get; set; }
    }
}
