using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public class EmailManager
    {
        public static void AppSettings(out string UserID, out string Password, out string SMTPPort, out string Host)
        {
            UserID = ConfigurationManager.AppSettings.Get("GmailUserName");
            Password = ConfigurationManager.AppSettings.Get("GmailPassword");
            SMTPPort = ConfigurationManager.AppSettings.Get("GmailPort");
            Host = ConfigurationManager.AppSettings.Get("GmailHost");
                 
        }

        public static string UserId { get; set; }
        public static string Password { get; set; }
        public static string SMTPPort { get; set; }
        public static string Host { get; set; }
        public static bool EnableSSL { get; set; }
        public static void AppSettings()
        {
            UserId = ConfigurationManager.AppSettings.Get("GmailUserName");
            Password = ConfigurationManager.AppSettings.Get("GmailPassword");
            SMTPPort = ConfigurationManager.AppSettings.Get("GmailPort");
            Host = ConfigurationManager.AppSettings.Get("GmailHost");
            EnableSSL = bool.Parse(ConfigurationManager.AppSettings.Get("GmailSsl"));
        }

        public static bool SendEmail(string From, string Subject, string Body, string To, string UserID, string Password, string SMTPPort, string Host, string cc = "")
        {
            bool isSent = false;
            try
            {
                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    if (!string.IsNullOrEmpty(cc))
                        mail.CC.Add(cc);
                    mail.To.Add(To);
                    mail.From = new MailAddress(From);
                    mail.Subject = Subject;
                    mail.Body = Body;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = Host;
                    smtp.Port = Convert.ToInt16(SMTPPort);
                    smtp.Credentials = new NetworkCredential(UserID, Password);
                    smtp.EnableSsl = EnableSSL;
                    smtp.Send(mail);
                    isSent = true;
                    return isSent;
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                isSent = false;
                return isSent;
            }
        }
        public static bool SendHTMLEmail(string From, string Subject, string Body, string To, string UserID, string Password, string SMTPPort, string Host, string cc = "")
        {
            bool isSent = false;
            try
            {
                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    if (!string.IsNullOrEmpty(cc))
                        mail.CC.Add(cc);
                    mail.To.Add(To);
                    mail.From = new MailAddress(From);
                    mail.Subject = Subject;
                    mail.IsBodyHtml = true;
                    mail.Body = Body;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = Host;
                    smtp.Port = Convert.ToInt16(SMTPPort);
                    smtp.Credentials = new NetworkCredential(UserID, Password);
                    smtp.EnableSsl = EnableSSL;
                    smtp.Send(mail);
                    isSent = true;
                    return isSent;
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                isSent = false;
                return isSent;
            }
        }

        public static bool SendEmailAttachment(string From, string Subject, string Body, string To, string[] attchFile, string cc = "")
        {
            AppSettings();
            bool isSent = false;
            try
            {
                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    foreach (string fileName in attchFile)
                    {
                        mail.Attachments.Add(new Attachment(fileName));
                    }

                    mail.To.Add(To);
                    if (!string.IsNullOrEmpty(cc))
                        mail.CC.Add(cc);
                    mail.From = new MailAddress(From);
                    mail.Subject = Subject;
                    mail.Body = Body;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = Host;
                    smtp.Port = Convert.ToInt16(SMTPPort);
                    smtp.Credentials = new NetworkCredential(UserId, Password);
                    smtp.EnableSsl = EnableSSL;
                    smtp.Send(mail);
                    isSent = true;
                    return isSent;
                }
            }
            catch (SmtpFailedRecipientException ex)
            {
                isSent = false;
                return isSent;
            }
        }
        public static bool SendHTMLEmailAttachment(string From, string Subject, string Body, string To, string[] attchFile, string cc = "")
        {
            AppSettings();
            bool isSent = false;
            try
            {
                using (System.Net.Mail.MailMessage mail = new System.Net.Mail.MailMessage())
                {
                    foreach (string fileName in attchFile)
                    {
                        mail.Attachments.Add(new Attachment(fileName));
                    }

                    mail.To.Add(To);
                    if (!string.IsNullOrEmpty(cc))
                        mail.CC.Add(cc);
                    mail.From = new MailAddress(From);
                    mail.Subject = Subject;
                    mail.IsBodyHtml = true;
                    mail.Body = Body;
                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = Host;
                    smtp.Port = Convert.ToInt16(SMTPPort);
                    smtp.Credentials = new NetworkCredential(UserId, Password);
                    smtp.EnableSsl = EnableSSL;
                    smtp.Send(mail);
                    isSent = true;
                    return isSent;
                }
            }
            catch (SmtpFailedRecipientException ex)
            {                
                isSent = false;
                return isSent;
            }
        }
    }
}