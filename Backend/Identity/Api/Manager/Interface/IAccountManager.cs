using Api.Models.Entites;

namespace Api.Manager.Interface
{
    public interface IAccountManager
    {
        Task<bool> CheckEmailExistsAsync(string email);
        Task<bool> SendConfirmEMailAsync(User user);
        Task<bool> SendEmailForgotEmailPassword(User user);
    }
}
