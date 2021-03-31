using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class MerchantInternationalPaymentServices
    {
        DB.FAXEREntities dbContext = null;
        public MerchantInternationalPaymentServices()
        {
            dbContext = new DB.FAXEREntities();
        }


        public DB.KiiPayBusinessInternationalPaymentTransaction SaveTransaction(DB.KiiPayBusinessInternationalPaymentTransaction model) {

            dbContext.KiiPayBusinessInternationalPaymentTransaction.Add(model);
            dbContext.SaveChanges();
            return model;
        }


        public List<PreviousPayeeDropDown> GetPreviousPayee()
        {

            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            string MerchantCountry = Common.BusinessSession.LoggedBusinessMerchant.CountryCode;
            var result = (from c in dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessInformationId == KiiPayBusinessInformationId && x.PayedToKiiPayBusinessWalletInformation.Country != MerchantCountry)
                          select new PreviousPayeeDropDown()
                          {

                              BusinessMFCode = c.PayedToKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              Name = c.PayedToKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName
                          }
                          ).Distinct().ToList();
            return result;
        }
    }
}