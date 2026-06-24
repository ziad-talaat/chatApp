using System;
using System.ComponentModel.DataAnnotations;
namespace Core.DTOS.UserDTOS
{
    public  sealed class RegisterDTo
    {
        [EmailAddress(ErrorMessage ="invalid Email Address")]
        [Required(ErrorMessage = " Email Address can't be blank")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = " Email Address can't be blank")]
        [MinLength(2,ErrorMessage ="Name is too short")]
        [MaxLength(150,ErrorMessage ="Name is too long")]
        public string UserName { get; set; } = string.Empty;

                //PhoneNumber = "01012345672",
                //DateOfBirth = new DateOnly(2003, 8, 28),
                //ImageUrl = "http://localhost:5247/images/batman.jpg",
                //Gender = "Male",
                //Description = "Junior .NET Developer",
                //City = "Fayoum",
                //Country = "Egypt"






        [Required(ErrorMessage = " Password  can't be blank")]

        [MinLength(5,ErrorMessage ="password Too Short")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password  can't be blank")]
        [Compare(nameof(Password),ErrorMessage ="Confirm Password should match password")]
        public string ConfirmPassword { get; set; } 
    }
}
