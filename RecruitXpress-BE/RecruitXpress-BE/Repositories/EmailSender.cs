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
 

        public void Send(string to, string subject, string html, string from = null)
        {
            // create message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _emailConfig.From));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };

            // send email
            using var smtp = new SmtpClient();
            smtp.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_emailConfig.UserName, _emailConfig.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
        public void SendWithAttach(string to, string subject, string html, string filePath, string fileName, string from = null)
        {
            // Create the MIME message
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(from ?? _emailConfig.From));
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
            using (var smtp = new SmtpClient())
            {
                smtp.Connect(_emailConfig.SmtpServer, _emailConfig.Port, SecureSocketOptions.StartTls);
                smtp.Authenticate(_emailConfig.UserName, _emailConfig.Password);
                smtp.Send(email);
                smtp.Disconnect(true);
            }
        }
    }
}
