using System.ComponentModel.DataAnnotations;

namespace Core.DTOS.UserDTOS
{
    public sealed class LoginDTO
    {
        [EmailAddress(ErrorMessage = "invalid Email Address")]
        [Required(ErrorMessage = " Email Address can't be blank")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = " Password  can't be blank")]
        [MinLength(5, ErrorMessage = "password Too Short")]
        public string Password { get; set; }
       
    }
}
