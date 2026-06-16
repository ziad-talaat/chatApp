using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOS
{
    public  sealed class AppUserDTO
    {
        [EmailAddress(ErrorMessage ="invalid Email Address")]
        [Required(ErrorMessage = " Email Address can't be blank")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = " Email Address can't be blank")]
        [MinLength(2,ErrorMessage ="Name is too short")]
        [MaxLength(150,ErrorMessage ="Name is too long")]
        public string DisplayName { get; set; } = string.Empty;
        [Required(ErrorMessage = " Password  can't be blank")]
        [MinLength(5,ErrorMessage ="password Too Short")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password  can't be blank")]
        [Compare(nameof(Password),ErrorMessage ="Confirm Password should match password")]
        public string ConfirmPassword { get; set; } 
    }
}
