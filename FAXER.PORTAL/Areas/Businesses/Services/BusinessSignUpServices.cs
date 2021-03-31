using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class BusinessSignUpServices
    {

        DB.FAXEREntities dbContext = null;
        public BusinessSignUpServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public DB.BecomeAMerchant GetBecomeAMerchantInformation(string RegistrationNumber)
        {

            var result = dbContext.BecomeAMerchant.Where(x => x.RegistrationNumber == RegistrationNumber).FirstOrDefault();
            return result;
        }

        public bool MerchantHasMFBCCard(int KiiPayBusinessInformationId) {


            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && x.CardStatus == DB.CardStatus.Active || x.CardStatus == DB.CardStatus.InActive).FirstOrDefault();

            return result == null ? false : true;
        }

        public DB.KiiPayBusinessInformation GetBusinessInformation(string email)
        {
            var result = dbContext.KiiPayBusinessInformation.Where(x => x.Email == email).FirstOrDefault();

            return result;
        }
        public DB.KiiPayBusinessLogin GetBusinessLoginDetails(string ActivationCode)
        {

            var data = dbContext.KiiPayBusinessLogin.Where(x => x.ActivationCode == ActivationCode).FirstOrDefault();

            return data;
        }
        public DB.KiiPayBusinessInformation SaveBusinessInformation(DB.KiiPayBusinessInformation BusinessInformation)
        {

            dbContext.KiiPayBusinessInformation.Add(BusinessInformation);
            dbContext.SaveChanges();
            return BusinessInformation;
        }

        public DB.KiiPayBusinessLogin SaveBusinessLogin(DB.KiiPayBusinessLogin businessLogin)
        {

            dbContext.KiiPayBusinessLogin.Add(businessLogin);
            dbContext.SaveChanges();
            return businessLogin;
        }
        public bool UpdateBusinessLogin(ViewModels.BusinessLoginViewModel businessLogin)
        {
            var data = dbContext.KiiPayBusinessLogin.Where(x => x.Username == businessLogin.EmailAddress && x.LoginCode == businessLogin.LoginCode).FirstOrDefault();
            if (data != null)
            {
                data.Password = Common.Common.Encrypt(businessLogin.password);
                data.IsActive = true;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;

            }
            return false;
        }

        internal string getBecomeaMerchantInformation(string registrationNumBer)
        {
            var data = dbContext.BecomeAMerchant.Where(x => x.RegistrationNumber == registrationNumBer).FirstOrDefault();
            if (data != null) {

                return data.FirstName + " " + data.LastName;
            }

            return "";
        }

        public DB.KiiPayBusinessLogin IsExist(string email)
        {
            var data = dbContext.KiiPayBusinessLogin.Where(x => x.Username == email).FirstOrDefault();
            return data;
        }
        public string GetResgistrationNumber(int length)
        {

            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            //return s;
            while (dbContext.KiiPayBusinessInformation.Where(x => x.RegistrationNumBer == s).Count() > 0)
            {
                for (int i = 0; i < length; i++)
                    s = String.Concat(s, random.Next(10).ToString());
            }
            return s;

        }
        public string GetMFSCode(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            //return s;
            while (dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo == s).Count() > 0)
            {
                for (int i = 0; i < length; i++)
                    s = String.Concat(s, random.Next(10).ToString());
            }
            return s;
        }
        public string GetNewLoginCode(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(5).ToString());
            //return s;
            while (dbContext.KiiPayBusinessLogin.Where(x => x.LoginCode == s).Count() > 0)
            {
                for (int i = 0; i < length; i++)
                    s = String.Concat(s, random.Next(5).ToString());
            }
            return s;
        }
    }
}
