using System.ComponentModel.DataAnnotations;

namespace Api.Models.ViewModel.Account
{
    public class ConfirmEmailViewModel
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }   
    }
}
