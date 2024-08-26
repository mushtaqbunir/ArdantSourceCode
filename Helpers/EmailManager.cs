using Microsoft.Extensions.Configuration;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ArdantOffical.Helpers
{
    public class EmailManager
    {
        public EmailManager(IConfiguration configuration)
        {
            try
            {
                var emailCred = configuration.GetSection("EmailCred");
                UserName = emailCred["UserName"];
                Password = emailCred["Password"];
                SmtpHost = emailCred["SmtpHost"];


            }
            catch { }
        }
        public string SmtpHost { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public async Task SendEmail(string mailTo, string emailBody)
        {

            try
            {

                var client = new SmtpClient
                {
                    Host = SmtpHost,
                    // Port = 465,
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(UserName, Password)
                };
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(UserName),
                    Subject = "Confirm Email",
                    IsBodyHtml = true,
                    Body = emailBody
                };

                mail.To.Add(mailTo);

                await client.SendMailAsync(mail);
                //logger.LogInformation("mail Sent");
            }
            catch (Exception ex)
            {
                //logger.LogInformation(ex.Message);
                throw new BadRequestException(ex.Message);
            }
        }


        //public async Task SendFormattedEmail(string mailTo, IWebHostEnvironment env, string role, string emailConfirmationLink, string userId)
        //{
        //    //if (role == Shared.Constants.Roles.BANK_BRANCH || role == Shared.Constants.Roles.BANK_CPC)
        //    //{
        //    //    role = " as <strong style='color:#1ea653;'>Initiator</strong>";
        //    //}
        //    //else if (role == Shared.Constants.Roles.BANK_BRANCH_MANAGER || role == Shared.Constants.Roles.BANK_CPC_MANAGER)
        //    //{
        //    //    role = "as <strong style='color:#1ea653;'>Supervisor</strong>";
        //    //}
        //    //else if (role == Shared.Constants.Roles.BANK_HYBRID)
        //    //{
        //    //    role = "";
        //    //}



        //    //<p> <img width='86' height='86' src=\"cid:SOSLogo\" align='left' hspace='12' alt='photograph1' /></p>
        //    var cid = Guid.NewGuid().ToString(); ;
        //    string htmlBody = $"<p align='center'> <strong style='color:red;'>Welcome to SOS Digital</strong></p><p align='center'> Where excellence is not an exception</p><p> Dear Valued Customer</p><p> Thank you for choosing SOS Pakistan. You can now enjoy expanded access to Cash-in-Transit Services.</p><p> <a href= '{emailConfirmationLink}' target='_blank' >Click here</a> to verify your email\\Username and create your password {role}.<br>Your username is <strong style='color:#1ea653;'>{userId}</strong> for your record.<br><br>The portal link is <a href='https://sosdigital.pk'>https://sosdigital.pk</a>. </p><p> Please reach out to our Support Team if you have any questions at ​</p><p> <a href='mailto:digital​support@sospakistan.net' target='_blank'> <strong>digital​support@sospakistan.net</strong> </a> <strong> </strong> or call our <strong>Help Center</strong> <strong> +923000 341 900 </strong> for more information.</p><p><br><img height='100' src=\"cid:{cid}\"> </p> <p> Thank you! <br><strong style='color:red;'>SOS Digital Team</strong></p><p><br><br><br> Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox <br/> For more information visit our help page or contact us here</p>";
        //    AlternateView avHtml = AlternateView.CreateAlternateViewFromString
        //       (htmlBody, null, MediaTypeNames.Text.Html);

        //    LinkedResource inline = new LinkedResource("wwwroot/images/SOSLogo.png", MediaTypeNames.Image.Jpeg);
        //    inline.ContentId = cid;
        //    avHtml.LinkedResources.Add(inline);

        //    MailMessage mail = new MailMessage();
        //    mail.AlternateViews.Add(avHtml);

        //    var filePath = env.WebRootFileProvider.GetFileInfo("Images/SOSLogo.png")?.PhysicalPath;

        //    Attachment att = new Attachment(filePath);
        //    att.ContentDisposition.Inline = true;

        //    mail.From = new MailAddress(UserName);
        //    mail.To.Add(mailTo);
        //    mail.Subject = "Welcome to SOS Digital";
        //    //mail.Body = $"<p> <img width='86' height='86' src=\"cid:{att.ContentId}\" align='left' hspace='12' alt='photograph2' /></p><p align='center'> <strong></strong></p><p align='center'> <strong style='color:red;'>Welcome to SOS Digital</strong></p><p align='center'> Where excellence is not an exception</p><p> Dear Valued Customer</p><p> Thank you for choosing SOS Pakistan. You can now enjoy expanded access to Cash-in-Transit Services.</p><p> <a href= '{emailConfirmationLink}' target='_blank' >Click here</a> verify your email\\Username and create your password as  <strong style='color:#1ea653;'>{role}</strong>. You username is  <strong style='color:#1ea653;'>{userId}</strong> for your record. <a href='https://sosdigital.pk'>https://sosdigital.pk</a></p><p> Please reach out to our Support Team if you have any questions at ​</p><p><a href='mailto:digital​support@sospakistan.net' target='_blank'> <strong>digital​support@sospakistan.net</strong> </a> <strong> </strong> or call our <strong>Help Center</strong> <strong> +923000 341 900 </strong> for more information.</p><p> Thank you!</p><p> <strong style='color:red;'>SOS Digital Team</strong></p><p><br><br><br> Please do not reply to this email. Replies to this message are routed to an unmonitored mailbox <br/> For more information visit our help page or contact us here</p>";

        //    mail.IsBodyHtml = true;
        //    mail.Attachments.Add(att);

        //    var client = new SmtpClient
        //    {
        //        Host = SmtpHost,
        //        // Port = 465,
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(UserName, Password)
        //    };
        //    await client.SendMailAsync(mail);
        //}
    }
}
