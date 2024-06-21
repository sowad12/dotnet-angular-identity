using System;

using System.Threading.Tasks;
using Api.Models.IOptionModel;
using Api.Models.ViewModel.Account;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Api.Service
{
    public class EmailService
    {
        private readonly EmailOption _emailOption;
        private readonly MailJetOption _mailJetOption;

        public EmailService(IOptions<EmailOption> emailOption, IOptions<MailJetOption> mailJetOption)
        {
            _emailOption = emailOption.Value;
            _mailJetOption = mailJetOption.Value;
        }

        public async Task<bool> SendEmailAsync(EmailSendViewModel emailSend)
        {
            try
            {
                var client = new MailjetClient(_mailJetOption.ApiKey, _mailJetOption.SecretKey);

                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource
                };

                var email = new TransactionalEmailBuilder()
                    .WithFrom(new SendContact(_emailOption.From, _emailOption.ApplicationName))
                    .WithSubject(emailSend.Subject)
                    .WithHtmlPart(emailSend.Body)
                    .WithTo(new SendContact(emailSend.To))
                    .Build();

                var response = await client.SendTransactionalEmailAsync(email);

                // Log the full response for debugging
                Console.WriteLine(JsonConvert.SerializeObject(response));

                if (response.Messages != null)
                {
                    if (response.Messages[0].Status == "success")
                    {
                        return true;
                    }
                    else
                    {
                        // Log the status message if not successful
                        Console.WriteLine($"Email sending failed with status: {response.Messages[0].Status}");
                    }
                }
                else
                {
                    // Log if Messages is null or empty
                    Console.WriteLine("Response.Messages is null or empty");
                }
            }
            catch (Exception ex)
            {
                // Log the exception details
                Console.WriteLine($"Exception occurred: {ex.Message}");
            }

            return false;
        }

        public async Task<bool> SendDummyEmailAsync(EmailSendViewModel emailSend)
        {
            try
            {
                MailjetClient client = new MailjetClient(_mailJetOption.ApiKey, _mailJetOption.SecretKey);
                MailjetRequest request = new MailjetRequest
                {
                    Resource = Send.Resource,
                }
                   .Property(Send.FromEmail, _emailOption.From)
                   .Property(Send.FromName, "Mailjet Pilot")
                   .Property(Send.Subject, emailSend.Subject)
                   .Property(Send.TextPart, "Dear passenger, welcome to Mailjet! May the delivery force be with you!")
                   .Property(Send.HtmlPart, emailSend.Body)
                   .Property(Send.Vars, new JArray
{
    new JObject
    {
        { "name", "Name of the customer" },
        { "org", "Organization of the customer" }
    }
})
                   .Property(Send.Recipients, new JArray {
                new JObject {
                 {"Email",emailSend.To}
                 }
                       });
                MailjetResponse response = await client.PostAsync(request);
                if (response.StatusCode == 200)
                {
                    //if (response.Messages[0].Status == "success")
                    //{
                    //    return true;
                    //}
                    return true;
                }


            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");

            }
            return false;

        }

        public async Task<bool> SendMailkitEmailAsync(EmailSendViewModel emailSend)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_emailOption.From));
                email.To.Add(MailboxAddress.Parse(emailSend.To));
                email.Subject = emailSend.Subject;
                email.Body = new TextPart(TextFormat.Html) { Text = emailSend.Body };
                using var smtp = new SmtpClient();
                smtp.Connect(_emailOption.SmtpServer, _emailOption.Port, SecureSocketOptions.SslOnConnect);
                smtp.Authenticate(_emailOption.From, _emailOption.AppPassword);
                var res = smtp.Send(email);
                smtp.Disconnect(true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

       
        }
    }
}
