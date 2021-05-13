using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TaskRecorder.Areas.Home.Models
{
    public class ResetPasswordViewModel
    {
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2}  characters long.", MinimumLength = 6)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "New Password and ConfirmPassword Do not Match")]
        public string ConfirmNewpassword { get; set; }
    }
}
