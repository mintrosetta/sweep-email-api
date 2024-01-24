using System.Net;
using System.Net.Mail;
using System.Text;
using Microsoft.Extensions.Options;
using sweep_email_api.Data.Configurations;
using sweep_email_api.Services.Interfaces;

namespace sweep_email_api.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpZimbra smtpZimbra;

        public EmailService(IOptions<SmtpZimbra> smtpZimbra)
        {
            this.smtpZimbra = smtpZimbra.Value;
        }

        public async Task SendByZimbraAsync(string receive, string subject, string body)
        {
            if (string.IsNullOrEmpty(receive)) throw new Exception("Receive is required");

            var msg = new MailMessage();
            msg.From = new MailAddress($"TQM New Core {smtpZimbra.Email}");
            msg.To.Add(receive);
            msg.Subject = subject;
            msg.Body = body;
            msg.BodyEncoding = Encoding.UTF8;
            msg.SubjectEncoding = Encoding.UTF8;

            using (var client = new SmtpClient())
            {
                client.Host = smtpZimbra.Host;
                client.Port = smtpZimbra.Port;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpZimbra.Email, smtpZimbra.Password);
                client.EnableSsl = true;
                
                await client.SendMailAsync(msg);
            }   
        }
    }
}