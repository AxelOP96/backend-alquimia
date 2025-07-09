namespace alquimia.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string recipient, string subject, string htmlMessage);
        Task<bool> SendEmailWithAttachmentAsync(string toEmail, string subject, string htmlMessage, byte[] attachmentData, string attachmentFileName);
    }
}
