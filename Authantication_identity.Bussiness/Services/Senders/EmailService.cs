using Authantication_identity.Bussiness.Enums;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Authantication_identity.Bussiness.Services.Senders
{
   public class EmailService:IMessageService
    {
        public string[] Cc { get; set; }
        public string[] Bcc { get; set; }
        public string FilePath { get; set; }
        private string _userId = HttpContext.Current.User.Identity.GetUserId();
        public MessageStates messageState { get; private set; }
        public string SenderMail { get; set; }
        public string Password { get; set; }
        public string Smtp { get; set; }
        public int SmtpPort { get; set; }

        public EmailService()
        {
            this.SenderMail = "akdenizerkin@hotmail.com";
            this.Password = "19959595";
            this.Smtp = "smtp.live.com";
            this.SmtpPort = 587;
        }

        public EmailService(string _senderMail, string _password, string _smtp, int _smtpPort)
        {
            this.SenderMail = _senderMail;
            this.Password = _password;
            this.Smtp = _smtp;
            this.SmtpPort = _smtpPort;
        }
        public MessageStates messageStates => throw new NotImplementedException();

        public void Send(IdentityMessage message, params string[] contacts)
        {
            Task.Run(async () => { await this.SendAsync(message, contacts); });
        }

        public async Task SendAsync(IdentityMessage message, params string[] contacts)
        {
            var userID = _userId??"system";
            try
            {
                var mail = new MailMessage { From = new MailAddress(this.SenderMail) };
                if (!string.IsNullOrEmpty(FilePath))
                {
                    mail.Attachments.Add(new Attachment(FilePath));
                }

                foreach (var con in contacts)
                {
                    mail.To.Add(con);
                }

                if (Cc != null && Cc.Length > 0)
                {
                    foreach (var cc in Cc)
                    {
                        mail.CC.Add(new MailAddress(cc));
                    }
                }

                if (Bcc != null && Bcc.Length > 0)
                {
                    foreach (var bcc in Bcc)
                    {
                        mail.Bcc.Add(new MailAddress(bcc));
                    }
                }

                mail.Subject = message.Subject;
                mail.Body = message.Body;
                mail.IsBodyHtml = true;
                mail.BodyEncoding = Encoding.UTF8;
                mail.SubjectEncoding = Encoding.UTF8;
                mail.HeadersEncoding = Encoding.UTF8;

                var smtpClient = new SmtpClient(this.Smtp, this.SmtpPort)
                {
                    Credentials = new NetworkCredential(this.SenderMail, this.Password),
                    EnableSsl = true
                };
                await smtpClient.SendMailAsync(mail);
                messageState = MessageStates.Delivered;
            }
            catch (Exception e)
            {
                messageState = MessageStates.NotDelivered;
            }
        }
    }
}
