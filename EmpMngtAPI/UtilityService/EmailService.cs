using EmpMngtAPI.Model.RequestModel;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace EmpMngtAPI.UtilityService
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        public EmailService(IConfiguration configuration)
        {
            _config = configuration;
        }
        public void SendEmail(EmailModel emailModel)
        {
            var emailMessage = new MimeMessage();
            var from = _config["EmailSetting:From"];
            emailMessage.From.Add(new MailboxAddress("Rai Divyanshu", from));
            emailMessage.To.Add(new MailboxAddress(emailModel.To, emailModel.To));
            emailMessage.Subject = emailModel.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = string.Format(emailModel.Content)
            };

            using (var smtp = new SmtpClient())
            {
                try
                {
                    //client.Connect(_config["EmailSetting:SmtpServer"], 465, true);
                    //client.Authenticate(_config["EmailSetting:From"], _config["EmailSetting:Password"]);
                    //client.Send(emailMessage);
                    smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;
                    smtp.Connect("smtp.gmail.com", 465, SecureSocketOptions.StartTls);
                    smtp.Authenticate(_config["EmailSetting:From"], _config["EmailSetting:Password"]);
                    smtp.Send(emailMessage);
                    smtp.Disconnect(true);
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    smtp.Disconnect(true);
                    smtp.Dispose();
                }
            }
        }
    }
}
