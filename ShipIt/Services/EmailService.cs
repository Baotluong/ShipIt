using System;
using ShipIt.ViewModels;
using System.Net.Mail;
using System.IO;
using System.Configuration;

namespace ShipIt.Services
{
    public interface IEmailService : IDisposable
    {
        void BetStatusFormatEmail(BetStatusEmailViewModel emailTemplateViewModel);
        void ConfirmAccountFormatEmail(string recepientEmail, string callbackUrl);
    }

    public class EmailService : IEmailService
    {
        public EmailService()
        {
        }

        public void Dispose()
        {
        }

        private string BetStatusPopulateBody(BetStatusEmailViewModel vm)
        {
            string body = string.Empty;
            //QQ: Is using the following MapPath an issue?
            using (StreamReader reader = new StreamReader(System.Web.Hosting.HostingEnvironment.MapPath("~/Views/Bets/EmailTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", vm.UserName.Replace(".", "<span>.</span>"));
            body = body.Replace("{Title}", vm.Title);
            body = body.Replace("{Url}", vm.Url);
            body = body.Replace("{Description}", vm.Description.Replace(".", "<span>.</span>"));
            body = body.Replace("{BetPremise}", vm.BetPremise.Replace(".", "<span>.</span>"));
            body = body.Replace("{User1}", vm.User1.Replace(".", "<span>.</span>"));
            body = body.Replace("{User1Condition}", vm.User1Condition.Replace(".", "<span>.</span>"));
            body = body.Replace("{User2}", vm.User2.Replace(".", "<span>.</span>"));
            body = body.Replace("{User2Condition}", vm.User2Condition.Replace(".", "<span>.</span>"));
            return body;
        }

        public void BetStatusFormatEmail(BetStatusEmailViewModel emailTemplateViewModel)
        {
            //QQ: why use this?
            string body = this.BetStatusPopulateBody(emailTemplateViewModel);
            SendHtmlFormattedEmail(emailTemplateViewModel.RecipientEmail, emailTemplateViewModel.Subject, body);
        }

        public void ConfirmAccountFormatEmail(string recepientEmail, string callbackUrl)
        {
            var body = "Please confirm your ShipIt account by clicking this link: " + callbackUrl;
            var subject = "ShipIt - Please confirm your account";

            SendHtmlFormattedEmail(recepientEmail, subject, body);
        }

        private void SendHtmlFormattedEmail(string recepientEmail, string subject, string body)
        {
            using (MailMessage mailMessage = new MailMessage())
            {
                mailMessage.From = new MailAddress(ConfigurationManager.AppSettings["EmailUserName"]);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.To.Add(new MailAddress(recepientEmail));
                SmtpClient smtp = new SmtpClient();
                smtp.Host = ConfigurationManager.AppSettings["Host"];
                smtp.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"]);
                System.Net.NetworkCredential NetworkCred = new System.Net.NetworkCredential();
                NetworkCred.UserName = ConfigurationManager.AppSettings["EmailUserName"];
                NetworkCred.Password = ConfigurationManager.AppSettings["EmailPassword"];
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = NetworkCred;
                smtp.Port = int.Parse(ConfigurationManager.AppSettings["Port"]);
                smtp.Send(mailMessage);
            }
        }
    }
}