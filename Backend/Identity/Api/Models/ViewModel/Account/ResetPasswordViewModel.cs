using System.ComponentModel.DataAnnotations;

namespace Api.Models.ViewModel.Account
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "New Password must be at least {2}, and maximum {1} characters")]
        public string Password { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "New Password must be at least {2}, and maximum {1} characters")]
        public string NewPassword { get; set; }
    }
}
