using System.ComponentModel.DataAnnotations;

namespace MarriageApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="you must specify the password between 4 and 8")]
        public string Password { get; set; }
    }
}