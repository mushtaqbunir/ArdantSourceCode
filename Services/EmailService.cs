using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ArdantOffical.Services
{
    public class EmailService : IEmailSender
    {
        private List<string> attachments;

        public List<string> Attachments { get => attachments; set => attachments = value; }
        public EmailService()
        {

        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage, string filename)
        {

            string fromMail = "info@ardantadvantage.com";
            string fromPassword = "Nh264!qw";
            MailMessage message = new MailMessage
            {
                From = new MailAddress(fromMail),
                Subject = subject
            };
            message.To.Add(new MailAddress(email));
            message.Body = "<html><body> " + htmlMessage + " </body></html>";
            message.IsBodyHtml = true;
            if (!string.IsNullOrEmpty(filename))
            {
                message.Attachments.Add(new Attachment(filename));
            }
            SmtpClient smtpClient = new SmtpClient("webmail.ardantadvantage.com")
            {
                Port = 26,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = false
            };
            smtpClient.Send(message);
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string fromMail = "info@ardantadvantage.com";
            string fromPassword = "Nh264!qw";
            MailMessage message = new MailMessage
            {
                From = new MailAddress(fromMail),
                Subject = subject
            };
            message.To.Add(new MailAddress(email));
            message.Body = "<html><body> " + htmlMessage + " </body></html>";
            message.IsBodyHtml = true;            
            SmtpClient smtpClient = new SmtpClient("webmail.ardantadvantage.com")
            {
                Port = 26,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = false
            };
            smtpClient.Send(message);
        }
        public async Task SendEmailAsync(List<string> emails, string subject, string htmlMessage)
        {
            string fromMail = "info@ardantadvantage.com";
            string fromPassword = "Nh264!qw";
            MailMessage message = new MailMessage
            {
                From = new MailAddress(fromMail),
                Subject = subject
            };
            foreach (var email in emails)
            {
                message.To.Add(new MailAddress(email));
            }
            message.Body = "<html><body> " + htmlMessage + " </body></html>";
            message.IsBodyHtml = true;
            SmtpClient smtpClient = new SmtpClient("webmail.ardantadvantage.com")
            {
                Port = 26,
                Credentials = new NetworkCredential(fromMail, fromPassword),
                EnableSsl = false
            };
            smtpClient.Send(message);
        }
    }
}
