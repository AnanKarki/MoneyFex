using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class MerchantNationalPaymentServices
    {
        DB.FAXEREntities dbContext = null;
        public MerchantNationalPaymentServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public DB.KiiPayBusinessInformation GetBusinessInformation(string AccountNo = "")
        {

            var result = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo == AccountNo).FirstOrDefault();
            return result;
        }

        public DB.KiiPayBusinessWalletInformation GetMFBCCardInformation(int KiiPayBusinessInformationId)
        {

            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();
            return result;
        }

        public DB.KiiPayBusinessWalletInformation GetSenderBusinessInformation() {

            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();
            return result;
        }

        public DB.KiiPayBusinessLocalTransaction SaveTransaction(DB.KiiPayBusinessLocalTransaction model) {

            dbContext.KiiPayBusinessLocalTransaction.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public List<PreviousPayeeDropDown> GetPreviousPayee()
        {

            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            string MerchantCountry = Common.BusinessSession.LoggedBusinessMerchant.CountryCode;
            var result = (from c in dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessInformationId == KiiPayBusinessInformationId && x.PayedToKiiPayBusinessWalletInformation.Country == MerchantCountry)
                          select new PreviousPayeeDropDown()
                          {

                              BusinessMFCode = c.PayedToKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              Name = c.PayedToKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName
                          }
                          ).Distinct().ToList();

            
            return result;
        }
    }
    public class PreviousPayeeDropDown
    {

        public string BusinessMFCode { get; set; }

        public string Name { get; set; }
    }


}