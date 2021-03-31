using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class NonCardPaymentByMerchantServices
    {
        DB.FAXEREntities dbContext = null;
        int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant == null ? 0: Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId ;

        public NonCardPaymentByMerchantServices()
        {
            dbContext = new DB.FAXEREntities();

        }

        public DB.KiiPayBusinessWalletInformation getSenderDetails()
        {


            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();

            return result;
        }

        public DB.MerchantNonCardTransaction saveTransaction(DB.MerchantNonCardTransaction model)
        {

            dbContext.MerchantNonCardTransaction.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        public List<DropDownviewModel> getCountries()
        {

            var result = (from c in dbContext.Country
                          select new DropDownviewModel()
                          {
                              Code = c.CountryCode,
                              Name = c.CountryName
                          }).ToList();
            return result;
        }

        public DB.MerchantNonCardReceiverDetails SaveReceiverDetails(DB.MerchantNonCardReceiverDetails model)
        {
            dbContext.MerchantNonCardReceiverDetail.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        
        internal string getMFCN()
        {

            // 34 is code for cash to cash payment made by Business Merchant
            //this code should be unique and random with 8 digit length
            var val = Common.Common.GenerateRandomDigit(8)  + "-34";

            while (dbContext.MerchantNonCardTransaction.Where(x => x.MFCN == val).Count() > 0)
            {
                val = Common.Common.GenerateRandomDigit(8) + "-34";
            }
            return val;
        }
    }


    public class DropDownviewModel
    {

        public string Code { get; set; }

        public string Name { get; set; }
    }
}