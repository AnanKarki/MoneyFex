using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class BusinessLoginServices
    {
        DB.FAXEREntities dbContext = null;
        public BusinessLoginServices()
        {

            dbContext = new DB.FAXEREntities();
        }

        public DB.KiiPayBusinessLogin Login(ViewModels.BusinessLoginViewModel model)
        {

            var result = dbContext.KiiPayBusinessLogin.Where(x => x.Username == model.EmailAddress && x.IsDeleted == false).FirstOrDefault();

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
        public DB.KiiPayBusinessLogin PasswordReset(string email, ViewModels.ConfirmPasswordResetViewModel model)
        {
            var result = dbContext.KiiPayBusinessLogin.Where(x => x.Username == email).FirstOrDefault();
            result.Password = Common.Common.Encrypt(model.NewPassword);
            result.IsActive = true;
            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return result;


        }

        public void SendPasswordResetEmail( string UserName ) {

            MailCommon mail = new MailCommon();
            try
            {
                string mailMessage = string.Format("Contact Your Administration to activate your account. You Password Has been changed at", System.DateTime.Now);
                mail.SendMail(UserName, "Password Changed.", mailMessage);

            }
            catch (Exception)
            {
            }

        }
    }
}