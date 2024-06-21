namespace Api.Models.IOptionModel
{
    public class EmailOption
    {
        public string From { get; set; }
        public string ApplicationName { get; set; }
        public string ConfirmEmailPath { get; set; }
        public string ResetPasswordPath { get; set; }
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string AppPassword { get; set; }
    }
}
