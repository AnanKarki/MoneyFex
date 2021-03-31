using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessLoginServices
    {

        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessLoginServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public bool ValidateUserLoginCredentials(KiiPayBusinessLoginVM vm)
        {
            return true;
        }



      
        public DB.KiiPayBusinessLogin Login(ViewModels.KiiPayBusinessLoginVM model)
        {

            var result = dbContext.KiiPayBusinessLogin.Where(x => (x.Username == model.UserName|| x.MobileNo == model.UserName)  && x.IsDeleted == false).FirstOrDefault();

            return result;
        }
        public DB.KiiPayBusinessLogin UpdateLoginFailureCount(string email, int count, bool isActive)
        {

            var data = dbContext.KiiPayBusinessLogin.Where(x => x.Username == email).FirstOrDefault();

            data.LoginFailCount = count;
            data.IsActive = isActive;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }
      
        public bool IsDeleted(string Email)
        {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.CardUserEmail == Email).FirstOrDefault();
            if (result != null)
            {

                return result.IsDeleted;
            }
            return false;
        }

        public void SetLoggedKiiPayBusinessUserInfo(KiiPayBusinessInformation kiiPayBusinessInformation)
        {
            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            var kiiPayBusinessUserPersonalInfo = _kiiPayBusinessCommonServices.GetKiipayBusinessPersonalInfo(kiiPayBusinessInformation.Id);

            var KiiPayBusinessWalletInformation = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(kiiPayBusinessInformation.Id);


            ViewModels.LoggedKiiPayBusinessUserInfo loggedKiiPayBusinessUserInfo = new ViewModels.LoggedKiiPayBusinessUserInfo()
            {
                
                FullName = _kiiPayBusinessCommonServices.GetBusinessFullName(kiiPayBusinessInformation.Id),
                BusinessEmailAddress = kiiPayBusinessInformation.Email,
                BusinessMobileNo = kiiPayBusinessInformation.BusinessMobileNo,
                BusinessName = kiiPayBusinessInformation.BusinessName,
                CountryCode = kiiPayBusinessInformation.BusinessOperationCountryCode,
                KiiPayBusinessInformationId = kiiPayBusinessInformation.Id,
                CurrencySymbol = Common.Common.GetCurrencySymbol(kiiPayBusinessInformation.BusinessOperationCountryCode),
                //KiiPayBusinessWalletUserName= _kiiPayBusinessCommonServices.GetBusinessWalletFullName(KiiPayBusinessWalletInformation.Id)
            };

            Common.BusinessSession.LoggedKiiPayBusinessUserInfo = loggedKiiPayBusinessUserInfo;

            Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CurrentBalanceOnCard = _kiiPayBusinessCommonServices.GetAccountBalance();

        }

        public bool PasswordReset(ViewModels.KiiPayBusinessResetPasswordVM model)
        {

            string Email = Common.BusinessSession.EmailAddress;
            var result = dbContext.KiiPayBusinessLogin.Where(x => x.Username == Email).FirstOrDefault();
            result.Password = Common.Common.Encrypt(model.Password);
            result.IsActive = true;
            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            // Send Email And Sms 
            SendPasswordResetEmail(Email);
            return true;



        }

        public void SendPasswordResetEmail(string emailAddress) {

            MailCommon mail = new MailCommon();
            try
            {
                string mailMessage = string.Format("Contact Your Administration to activate your account. You Password Has been changed at", System.DateTime.Now);
                mail.SendMail(emailAddress, "Password Changed.", mailMessage);

            }
            catch (Exception)
            {
                
            }
        }
    }
}