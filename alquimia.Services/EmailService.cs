using alquimia.Services.Interfaces;
using alquimia.Services.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace alquimia.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _settings;

        public EmailService(IOptions<EmailSettings> options)
        {
            _settings = options.Value;
        }

        public async Task SendEmailAsync(string recipient, string _subject, string mensajeHtml)
        {
            using var smtpClient = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.User, _settings.Password),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.From, "Alquimia"),
                Subject = _subject,
                Body = mensajeHtml,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(recipient);
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task<bool> SendEmailWithAttachmentAsync(string recipient, string subject, string htmlMessage, byte[] attachmentData, string attachmentFileName)
        {
            try
            {
                using var smtpClient = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
                {
                    Credentials = new NetworkCredential(_settings.User, _settings.Password),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_settings.From, "Alquimia"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(recipient);

                using var attachmentStream = new MemoryStream(attachmentData);
                var attachment = new Attachment(attachmentStream, attachmentFileName, "application/pdf");
                mailMessage.Attachments.Add(attachment);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
