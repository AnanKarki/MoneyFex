using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class MFBCCardProfileServices
    {
        DB.FAXEREntities dbContext = null;
        Areas.Admin.Services.CommonServices CommonService = new Admin.Services.CommonServices();
        public MFBCCardProfileServices() {
            dbContext = new DB.FAXEREntities();
        }
        public ViewModels.MFBCCardProfileViewModel GetMFBCCardInformation(int KiiPayBusinessInformationId) {

            var result = (from c in dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).ToList()
                         join d in dbContext.Country on c.Country equals d.CountryCode
                         select new ViewModels.MFBCCardProfileViewModel()
                         {
                             CardId = c.Id,
                             Country = d.CountryName,
                             City = c.City,
                             MFBCCardNumber = c.MobileNo.Decrypt().FormatMFBCCard(),
                             FullName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                             EmailAddres = c.Email,
                             UserIdentificationURL = c.KiiPayUserPhoto,
                             PhoneNumber = c.PhoneNumber,
                             AmountOnCard = c.CurrentBalance,
                             CardStatus = Enum.GetName(typeof(DB.CardStatus) , c.CardStatus),
                             Currency = CommonService.getCurrencyCodeFromCountry(c.Country),
                             CurrencySymbol = CommonService.getCurrencySymbol(c.Country)

                         }).FirstOrDefault();

            return result;

        }
        public ViewModels.MFBCCardProfileViewModel GetMFBCCardProfileByCardId(int CardId) {

            var result = (from c in dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == CardId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).ToList()
                          join d in dbContext.Country on c.Country equals d.CountryCode
                          select new ViewModels.MFBCCardProfileViewModel()
                          {
                              CardId = c.Id,
                              Country = d.CountryName,
                              City = c.City,
                              //MFBCCardNumber = c.MFBCCardNumber.Decrypt().FormatMFBCCard(),
                              MFBCCardNumber = c.MobileNo.Decrypt().FormatMFBCCard(),
                              FullName = c.FirstName + " " +  c.MiddleName + " " + c.LastName,
                              EmailAddres = c.Email,
                              UserIdentificationURL = c.KiiPayUserPhoto,
                              PhoneNumber = c.PhoneNumber,
                              AmountOnCard = c.CurrentBalance,
                              CardStatus = Enum.GetName(typeof(DB.CardStatus), c.CardStatus)

                          }).FirstOrDefault();

            return result;


        }
        public bool UpdateCardUserCity(string city, int CardId) 
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == CardId).FirstOrDefault();
            data.City = city;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;

        }
        public bool UpdateCardUserEmail(string email, int CardId)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == CardId).FirstOrDefault();
            data.Email = email;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;

        }
        public bool UpdateCardUserPhoneNumber(string Telephone, int CardId)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == CardId).FirstOrDefault();
            data.PhoneNumber = Telephone;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;

        }
        public bool UpdateCardUserPhoto(string ImageUrl, int CardId)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == CardId).FirstOrDefault();
            data.KiiPayUserPhoto = ImageUrl;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;

        }
        public string GetImageURL(int CardId) {
            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == CardId).FirstOrDefault();
            string ImageUrl = result.KiiPayUserPhoto;
            string[] tokens = ImageUrl.Split('/');

            return tokens[2];
        }
    }
}