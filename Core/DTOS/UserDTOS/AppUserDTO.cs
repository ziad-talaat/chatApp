using System;
using System.ComponentModel.DataAnnotations;
using Core.CustomeValidation;
using Core.Domain.Entities;
namespace Core.DTOS.UserDTOS
{
    public   class RegisterDTo
    {
        [EmailAddress(ErrorMessage ="invalid Email Address")]
        [Required(ErrorMessage = "Email Address can't be blank")]
        public string Email { get; set; } = string.Empty;
        [Required(ErrorMessage = "user name Address can't be blank")]
        [MinLength(2,ErrorMessage ="Name is too short")]
        [MaxLength(150,ErrorMessage ="Name is too long")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = " Password  can't be blank")]

        [MinLength(5,ErrorMessage ="password Too Short")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password  can't be blank")]
        [Compare(nameof(Password),ErrorMessage ="Confirm Password should match password")]
        public string ConfirmPassword { get; set; }
        public DateTime Created { get; set; }=DateTime.Now;

        [Required(ErrorMessage ="age cant be blank")]
        [ValidateDateOfBirth]
        public DateOnly DateOfBirth { get; set; }

        [Required(ErrorMessage = "age cant be blank")]
        [RegularExpression(@"^01[0125]\d{8}$",
            ErrorMessage = "Invalid Egyptian phone number.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage ="gender cant be blank")]
        [RegularExpression(@"^(?i:male|female)$",ErrorMessage ="only male or female")]
        public string Gender { get; set; }


        [Required(ErrorMessage ="City cant be blank")]
        [MinLength(2,ErrorMessage ="city name at least 2 chars" )]
        public string City { get; set; }
        [Required(ErrorMessage = "country cant be blank")]
        [MinLength(2, ErrorMessage = "country name at least 2 chars")]
        public string Country { get; set; }

    }
}
