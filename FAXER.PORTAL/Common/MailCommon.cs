using FAXER.PORTAL.Models;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Hosting;

namespace FAXER.PORTAL.Common
{
    public class MailCommon
    {
        public MailMessage CreateMessage(MailAddress FromMailAddress, MailAddress ToMailAddress, string Subject, string Body, string AttachmentPath)
        {
            MailMessage msgMail = new MailMessage();

            msgMail.From = FromMailAddress;
            msgMail.To.Add(ToMailAddress);
            msgMail.Body = Body;
            msgMail.IsBodyHtml = true;
            msgMail.Attachments.Add(new Attachment(AttachmentPath));
            return msgMail;
        }
        public MailMessage CreateMessage(MailAddress FromMailAddress, MailAddress ToMailAddress, string Subject, string Body)
        {
            MailMessage msgMail = new MailMessage();
            
            msgMail.From = FromMailAddress;
            msgMail.To.Add(ToMailAddress);
            msgMail.Body = Body;
            msgMail.IsBodyHtml = true;
            return msgMail;
        }
        public MailMessage CreateMessage(MailAddress FromMailAddress, MailAddress ToMailAddress, string Subject, string Body, Attachment attachment)
        {
            MailMessage msgMail = new MailMessage();

            msgMail.From = FromMailAddress;
            msgMail.To.Add(ToMailAddress);
            msgMail.Body = Body;
            msgMail.IsBodyHtml = true;
            msgMail.Attachments.Add(attachment);
            return msgMail;
        }
        private bool SendEmail(MailMessage msgMail, NetworkCredential cred)
        {
            #region smtp
            SmtpClient mailClient;
            smtpHost host = smtpHost.gmail;
            
            string hostName = "";

            hostName = GetValueAsString(host);
            mailClient = new SmtpClient(hostName, 587);
            
            mailClient.UseDefaultCredentials = false;
            mailClient.EnableSsl = true;
            mailClient.Credentials = cred;
            #endregion

            try
            {
                mailClient.Send(msgMail);
            }
            catch (System.IO.IOException ex)
            {
                Log.Write(ex.Message);
                return false;
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
                return false;
            }
            finally
            {
                //do something if required
            }
            return true;

        }

        public EmailCredentialVm GetEmailCredential(TransactionEmailType transactionEmailType = TransactionEmailType.CustomerSupport) {


            string configEmailUserName = "CustomerSupport";
            string configEmailPassword = "CustomerSupportPassword";
            switch (transactionEmailType)
            {
                case TransactionEmailType.CustomerSupport:
                    configEmailUserName = "CustomerSupport";
                    break;
                case TransactionEmailType.Rates:
                    configEmailUserName = "Rates";
                    break;
                case TransactionEmailType.WelcomeCustomer:
                    configEmailUserName = "WelcomeCustomer";
                    break;
                case TransactionEmailType.TransactionCancelled:
                    configEmailUserName = "TransactionCancelled";
                    break;
                case TransactionEmailType.IDCheck:
                    configEmailUserName = "IDCheck";
                    break;
                case TransactionEmailType.TransactionCompleted:
                    configEmailUserName = "TransactionCompleted";
                    break;
                case TransactionEmailType.TransactionInProgress:
                    configEmailUserName = "TransactionInProgress";
                    break;
                case TransactionEmailType.TransactionPending:
                    configEmailUserName = "TransactionPending";
                    break;
                default:
                    configEmailUserName = "CustomerSupport";
                    break;
            }

            configEmailPassword = configEmailUserName + "Password";

            string UserName = configEmailUserName.GetAppSettingValue();

            string Password = configEmailPassword.GetAppSettingValue();
            return new EmailCredentialVm()
            {
                UserName = UserName,
                Password = Password
            };


        }
        public void SendMail(string receiver, string subject, string message , PdfDocument document = null)
        {

            //ehajiri@gmail.com
            try
            {



                //MailAddress from = new MailAddress(Common.GmailUserName.GetAppSettingValue());
                //NetworkCredential cred = new NetworkCredential(Common.GmailUserName.GetAppSettingValue(),
                //    Common.GmailPassword.GetAppSettingValue());
                //if (FaxerSession.TransactionEmailTypeSession != null) {

                //    var credential = GetEmailCredential(FaxerSession.TransactionEmailTypeSession);
                //    from = new MailAddress(credential.UserName);
                //    cred = new NetworkCredential(credential.UserName,
                //        credential.Password);
                //}


                //MailAddress to = new MailAddress(receiver);
                //var mailMsg = CreateMessage(from, to, subject, message);
                //if (document != null)
                //{
                //    var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
                //    document.Save(path);
                //    mailMsg.Attachments.Add(new Attachment(path));

                //}
                //mailMsg.Subject = string.Format(subject);
                //if( SendEmail(mailMsg, cred))
                //{
                //    Log.Write("mail sent to " + receiver);
                //}
                //mailMsg.Dispose();
                SendAwsEmail(subject, message, receiver, document);

                var Filepath = HostingEnvironment.MapPath("~/Documents") + "attachment.pdf";
                File.Delete(Filepath);
                
            }
            catch (Exception ex)
            {

                Log.Write(ex.Message);
            }

        }

      

        public void SendAwsEmail(string subject , string body , string to, PdfDocument document = null) {


            string FROM = Common.GetAppSettingValue("AWS_EMAIL");
            string HOST = Common.GetAppSettingValue("AWS_HOST");
            int PORT = 587;
            int.TryParse(Common.GetAppSettingValue("AWS_PORT"), out PORT);
            string FROMNAME = Common.GetAppSettingValue("AWS_FROM_NAME");
            string SMTP_USERNAME = Common.GetAppSettingValue("AWS_USERNAME");
            string SMTP_PASSWORD = Common.GetAppSettingValue("AWS_PASSWORD");
            string BCC = Common.GetAppSettingValue("AWS_BCC");
            MailMessage message = new MailMessage();
            message.IsBodyHtml = true;
            message.From = new MailAddress(FROM, FROMNAME);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            MailAddress bcc = new MailAddress(BCC);
            message.Bcc.Add(bcc);
            if (document != null)
            {
                var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
                document.Save(path);
                message.Attachments.Add(new Attachment(path));

            }
            using (var client = new System.Net.Mail.SmtpClient(HOST, PORT))
            {
                // Pass SMTP credentials
                client.Credentials =
                    new NetworkCredential(SMTP_USERNAME, SMTP_PASSWORD);
                // Enable SSL encryption
                client.EnableSsl = true;
                // Try to send the message. Show status in console.
                try
                {
                    //Console.WriteLine("Attempting to send email...");
                    client.Send(message);
                    //Console.WriteLine("Email sent!");
                    Log.Write("mail sent to " + to);
                }
                catch (Exception ex)
                {
                    //Log.Write("mail sent to " + to);
                    Log.Write("The email was not sent." + to);
                    Log.Write("Error message: " + ex.Message);
                }
            }
        }
        public void SendMail(string receiver, string subject, string message, string sender, string password)
        {

            MailAddress from = new MailAddress(sender);
            NetworkCredential cred = new NetworkCredential(sender, password);
            MailAddress to = new MailAddress(receiver);
            var mailMsg = CreateMessage(from, to, subject, message);

            mailMsg.Subject = string.Format(subject);
            SendEmail(mailMsg, cred);
            mailMsg.Dispose();
        }

        public void SendMail_ApplyJob(string receiver, string subject, string message, string[] paths)
        {

            //ehajiri@gmail.com
            MailAddress from = new MailAddress(Common.GmailUserName.GetAppSettingValue());
            NetworkCredential cred = new NetworkCredential(Common.GmailUserName.GetAppSettingValue(), Common.GmailPassword.GetAppSettingValue());
           
            MailAddress to = new MailAddress(receiver);
            var mailMsg = CreateMessage(from, to, subject, message);
            foreach (var path in paths)
            {
                mailMsg.Attachments.Add(new Attachment(path));
            }

            mailMsg.Subject = string.Format(subject);
            SendEmail(mailMsg, cred);
            mailMsg.Dispose();

        }


        public static string GetValueAsString(smtpHost host)
        {
            // get the field 
            var field = host.GetType().GetField(host.ToString());
            var customAttributes = field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (customAttributes.Length > 0)
            {
                return (customAttributes[0] as DescriptionAttribute).Description;
            }
            else
            {
                return host.ToString();
            }
        }
    }
    public enum smtpHost
    {
        [Description("smtp.gmail.com")]
        gmail = 0,
        [Description("smtp.gmail.com")]
        live = 1,
        [Description("smtp.mail.yahoo.com")]
        yahoo = 2,
        [Description("smtp.aim.com")]
        aim = 3
    }
}