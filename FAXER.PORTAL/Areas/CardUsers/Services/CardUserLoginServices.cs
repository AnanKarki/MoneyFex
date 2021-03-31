using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class CardUserLoginServices
    {
        DB.FAXEREntities dbContext = null;

        public CardUserLoginServices()
        {

            dbContext = new DB.FAXEREntities();
        }

        public DB.KiiPayPersonalUserLogin CardUserLogin(string Email)
        {

            var result = dbContext.KiiPayPersonalUserLogin.Where(x => x.Email == Email).FirstOrDefault();
            if (result != null)
            {
                var result2 = dbContext.KiiPayPersonalWalletInformation.Where(x => x.CardUserEmail == Email).FirstOrDefault();

                if (result2 != null)
                {

                    return result;
                }
            }
            return null;
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
        public DB.KiiPayPersonalUserLogin UpdateLoginFailureCount(string email, int count, bool isActive)
        {

            var data = dbContext.KiiPayPersonalUserLogin.Where(x => x.Email == email).FirstOrDefault();

            data.LoginFailedCount = count;
            data.IsActive = isActive;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return data;
        }

        public DB.KiiPayPersonalUserInformation GetCardUserInformation(int Id)
        {

            var result = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Id).FirstOrDefault();
            return result;
        }

        public bool ResetPassword(ViewModels.PasswordResetViewModel model)
        {

            string email = Common.CardUserSession.EmailAddress;
            var result = dbContext.KiiPayPersonalUserLogin.Where(x => x.Email == email).FirstOrDefault();
            result.Password = model.NewPassword.Encrypt();
            result.IsActive = false;
            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }

        public string GetMFTCCardNumber(int MFTCCardId) {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).Select(x => x.MobileNo).FirstOrDefault();

            return result.Decrypt();
        }
    }
}