using FAXER.PORTAL.Areas.CardUsers.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class LogInServices
    {
        FAXEREntities dbContext = null;
        public LogInServices()
        {
            dbContext = new FAXEREntities();
        }

        public List<KiiPayPersonalUserLogin> userLoginList()
        {
            return dbContext.KiiPayPersonalUserLogin.ToList();

        }

        public KiiPayPersonalUserLogin updateUserLogin(KiiPayPersonalUserLogin model)
        {
            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return model;
        }

        public void SetKiiPayPersonalUserLoginSession(KiiPayPersonalUserLogin model)
        {
            Common.CardUserSession.LoggedCardUserViewModel = new LoggedKiiPayUserViewModel()
            {
                id = model.KiiPayPersonalUserInformationId,
                FirstName = model.KiiPayPersonalUserInformation.FirstName,
                LastName = model.KiiPayPersonalUserInformation.Lastname,
                FullName = model.KiiPayPersonalUserInformation.FirstName + " " + model.KiiPayPersonalUserInformation.Lastname,
                Country = Common.Common.GetCountryName(model.KiiPayPersonalUserInformation.Country),
                Email = model.Email,
                KiiPayPersonalId = model.KiiPayPersonalUserInformationId,
                BalanceOnCard = model.KiiPayPersonalUserInformation.KiiPayPersonalWalletInformation.CurrentBalance,
                CardUserCurrency = Common.Common.GetCountryCurrency(model.KiiPayPersonalUserInformation.Country),
                CardUserCurrencySymbol = Common.Common.GetCurrencySymbol(model.KiiPayPersonalUserInformation.Country),
                MobileNumber = model.KiiPayPersonalUserInformation.MobileNo,
                CountryCode = model.KiiPayPersonalUserInformation.Country,
                KiiPayPersonalWalletId = model.KiiPayPersonalUserInformation.KiiPayPersonalWalletInformationId
            };

        }

        public bool changeKiiPayPersonalUserPassword(string password)
        {
            if (!string.IsNullOrEmpty(password))
            {
                if (!string.IsNullOrEmpty(Common.MiscSession.ForgotPasswordEmailAddress))
                {
                    var userloginData = dbContext.KiiPayPersonalUserLogin.Where(x => x.Email.ToLower().Trim() == Common.MiscSession.ForgotPasswordEmailAddress.ToLower().Trim()).FirstOrDefault();
                    if(userloginData != null)
                    {
                        userloginData.Password = password.Encrypt();
                        updateUserLogin(userloginData);
                        return true;
                    }
                }
            }
            return false;
        }



    }
}