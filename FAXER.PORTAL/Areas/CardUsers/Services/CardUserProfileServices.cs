using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class CardUserProfileServices
    {
        DB.FAXEREntities dbContext = null;
        public CardUserProfileServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public ViewModels.CardUserInformationViewModel GETCardUserInformation()
        {

            int CardUserId = Common.CardUserSession.LoggedCardUserViewModel.id;
            var result = (from c in dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == CardUserId).ToList()
                          join Country in dbContext.Country on c.KiiPayPersonalWalletInformation.CardUserCountry equals Country.CountryCode
                          select new ViewModels.CardUserInformationViewModel()
                          {
                              Id = c.Id,
                              FullName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.LastName,

                              Address1 = c.KiiPayPersonalWalletInformation.Address1,
                              Address2 = c.KiiPayPersonalWalletInformation.Address2,
                              City = c.KiiPayPersonalWalletInformation.CardUserCity,
                              Country = c.KiiPayPersonalWalletInformation.CardUserCountry,
                              State = c.KiiPayPersonalWalletInformation.CardUserState,
                              ZipCode = c.KiiPayPersonalWalletInformation.CardUserPostalCode,
                              EmailAddress = c.EmailAddress,
                              PhoneNumber = c.KiiPayPersonalWalletInformation.CardUserTel
                          }).FirstOrDefault();
            return result;
        }

        internal IEnumerable getCountries()
        {
            var data = dbContext.Country.ToList();
            return data;
        }

        public DB.KiiPayPersonalWalletInformation GetCardInformation()
        {

            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();

            return result;
        }
        public bool UpdateAddress(DB.KiiPayPersonalWalletInformation model)
        {

            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            return true;
        }
        public bool UpdateTelephone(string PhoneNumber, int id)
        {
            var data = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == id).FirstOrDefault();

            if (data != null)
            {

                var data1 = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == data.KiiPayPersonalWalletInformationId).FirstOrDefault();
                data1.CardUserTel = PhoneNumber;
                dbContext.Entry(data1).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            return true;
        }
        public bool UpdateEmail(string email, int id)
        {

            var data = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == id).FirstOrDefault();
            data.EmailAddress = email;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            int result = dbContext.SaveChanges();
            if (result > 0)
            {


                var data1 = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == data.KiiPayPersonalWalletInformationId).FirstOrDefault();
                data1.CardUserEmail = email;
                dbContext.Entry(data1).State = System.Data.Entity.EntityState.Modified;
                var data2 = dbContext.KiiPayPersonalUserLogin.Where(x => x.KiiPayPersonalUserInformationId == data.Id).FirstOrDefault();
                data2.Email = email;
                dbContext.Entry(data2).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            return true;
        }

        public string GetFaxerEmailAddress(int MFTCCardId)
        {
            CardUserCommonServices cardUserCommonServices = new CardUserCommonServices();
            var result = cardUserCommonServices.GetSenderInformation(MFTCCardId);


            return result == null ? "" : result.Email;


        }
    }

}