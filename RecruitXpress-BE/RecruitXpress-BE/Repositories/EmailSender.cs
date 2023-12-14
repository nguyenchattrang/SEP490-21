using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using RecruitXpress_BE.DTO;
using RecruitXpress_BE.IRepositories;
using System.Net.Mime;
using ContentDisposition = MimeKit.ContentDisposition;

namespace RecruitXpress_BE.Repositories
{
    public class EmailSender :IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
 

        public async Task Send(string to, string subject, string html, string from = "RecruitXpress")
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailConfig.From, _emailConfig.UserName));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            await Task.Run(() => SendEmailAsync(email));
        }
        public async Task SendWithAttach(string to, string subject, string html, string filePath, string fileName, string from = null)
        {
            // Create the MIME message
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_emailConfig.From, _emailConfig.UserName));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            // Create the HTML body part
            var body = new TextPart(TextFormat.Html) { Text = html };

            // Create the multipart message and add the HTML body
            var multipart = new Multipart("mixed");
            multipart.Add(body);

            // Add the attachment
            var attachment = new MimePart(MediaTypeNames.Application.Pdf)
            {
                Content = new MimeContent(File.OpenRead(filePath), ContentEncoding.Default),
                ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                ContentTransferEncoding = ContentEncoding.Base64,
                FileName = fileName
            };

            multipart.Add(attachment);

            // Set the message body to the multipart content
            email.Body = multipart;

            // Send email
            await Task.Run(() => SendEmailAsync(email));
        }
        
        private async Task SendEmailAsync(MimeMessage email)
        {
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailConfig.UserName, _emailConfig.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }
}
