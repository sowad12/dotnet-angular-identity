using Api.Manager.Interface;
using Api.Models.Entites;
using Api.Models.IOptionModel;
using Api.Models.ViewModel.Account;
using Api.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Text;

namespace Api.Manager.Implementation
{
    public class AccountManager : IAccountManager
    {
        private readonly UserManager<User> _userManager;
        private readonly EmailOption _emailOption;
        private readonly MailJetOption _mailJetOption;
        private readonly JwtOption _jwtOption;
        private readonly EmailService _emailService;
        public AccountManager(UserManager<User> userManager,
                              IOptions<MailJetOption> mailJetOption,
                              IOptions<EmailOption> emailOption,
                              IOptions<JwtOption> jwtOption,
                              EmailService emailService)

        {

            _userManager = userManager;
            _mailJetOption = mailJetOption.Value;
            _emailOption = emailOption.Value;
            _jwtOption = jwtOption.Value;
            _emailService= emailService;

        }
        public async Task<bool> CheckEmailExistsAsync(string email)
        {
            return await _userManager.Users.AnyAsync(x => x.Email == email.ToLower());
        }

        public async Task<bool> SendConfirmEMailAsync(User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_jwtOption.ClientUrl}/{_emailOption.ConfirmEmailPath}?token={token}&email={user.Email}";

            var body = $"<p>Hello: {user.FirstName} {user.LastName}</p>" +
                "<p>Please confirm your email address by clicking on the following link.</p>" +
                $"<p><a href=\"{url}\">Click here</a></p>" +
                "<p>Thank you,</p>" +
                $"<br>{_emailOption.ApplicationName}";

            var emailSendViewModel = new EmailSendViewModel(user.Email, "confirm your email", body);
            //return await _emailService.SendDummyEmailAsync(emailSendViewModel);
            return await _emailService.SendMailkitEmailAsync(emailSendViewModel);
            //return await _emailService.SendDummyEmailAsync(emailSendViewModel);
     
        }

        public async Task<bool> SendEmailForgotEmailPassword(User user)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{_jwtOption.ClientUrl}/{_emailOption.ResetPasswordPath}?token={token}&email={user.Email}";

            var body = $"<p>Hello: {user.FirstName} {user.LastName}</p>" +
                $"<p>Username: {user.UserName}.</p>" +
               "<p>In order to reset your password, please click on the following link.</p>" +
               $"<p><a href=\"{url}\">Click here</a></p>" +
               "<p>Thank you,</p>" +
                $"<br>{_emailOption.ApplicationName}";

            var emailSendViewModel = new EmailSendViewModel(user.Email, "forgot email or password", body);
      
            return await _emailService.SendMailkitEmailAsync(emailSendViewModel);
        }
    }
}
