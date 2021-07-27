using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketingSystemMVC.Models;
using static TicketingSystemMVC.Models.MailerModel;
using MimeKit;
using System.IO;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace TicketingSystemMVC.Mailer
{
    public class MailerService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IConfiguration _Configure;
       public MailerService(IOptions<MailSettings> mailSettings, IConfiguration configuration)
        {
            _Configure = configuration;
         //   _mailSettings = _Configure.GetValue<string>("Mail");
        }
        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_Configure.GetValue<string>("Mail"));
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_Configure.GetValue<string>("Host"), _Configure.GetValue<int>("Port"), SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
  }
}data:image/pjpeg;base64,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