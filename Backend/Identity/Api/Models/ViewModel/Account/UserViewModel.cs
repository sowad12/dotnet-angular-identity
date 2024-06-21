using System.ComponentModel.DataAnnotations;

namespace Api.Models.ViewModel.Account
{
    public class UserViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public string JWT { get; set; }
    }
}
