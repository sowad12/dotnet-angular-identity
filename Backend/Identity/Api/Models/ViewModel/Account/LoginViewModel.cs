using System.ComponentModel.DataAnnotations;

namespace Api.Models.ViewModel.Account
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
