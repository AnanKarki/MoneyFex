using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
     


namespace FAXER.PORTAL.Common
{
    public class CommonAllServices
    {
        FAXEREntities dbContext = null;
        public CommonAllServices()
        {
            dbContext = new FAXEREntities();
        }
        public List< DB.MoneyFexBankAccountLog> GetMoneyFexBankAccount()
        {
            var result = dbContext.MoneyFexBankAccountLog.Where(x => x.IsConfirmed == false).ToList();
            return result;

        }


        public void SendPasswordResetCodeEmail(string receiverEmailAddress, string userName)
        {
            receiverEmailAddress = receiverEmailAddress.Trim();
            
            MailCommon mail = new MailCommon();
            string passwordSecurityCode = Common.GenerateRandomDigit(8);
            MiscSession.PasswordSecurityCode = passwordSecurityCode;
            MiscSession.ForgotPasswordEmailAddress = receiverEmailAddress;
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = Common.GetTemplate(baseUrl + "/EmailTemplate/PasswordSecirityCodeEmail?UserName=" + userName   + "&SecurityCode=" + passwordSecurityCode);
            mail.SendMail(receiverEmailAddress, "MoneyFex Password Reset", body);
        }

        public DB.SenderKiiPayPersonalAccount GetKiipayPersonalWalletInfoByKiiPayPersonalId(int Id)
        {
            var data = dbContext.SenderKiiPayPersonalAccount.Where(x => x.KiiPayPersonalWalletId == Id).FirstOrDefault();
            return data == null ? null : data;

        }

        public DB.KiiPayPersonalWalletInformation GetSenderWalletInfo(int Id)
        {
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == Id).FirstOrDefault();
            return data;

        }

        public void BalanceIn(int SenderWalletId, decimal Amount)
        {
            var data = GetSenderWalletInfo(SenderWalletId);
            data.CurrentBalance += Amount;
            dbContext.Entry<DB.KiiPayPersonalWalletInformation>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

    }
}