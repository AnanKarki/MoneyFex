using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class CommonServices
    {
        DB.FAXEREntities dbContext = null;


        public CommonServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public string getBusinessCurrency(int id)
        {
            string countryCode = dbContext.KiiPayBusinessInformation.Where(x => x.Id == id).FirstOrDefault().BusinessOperationCountryCode;
            return dbContext.Country.Where(x => x.CountryCode == countryCode).FirstOrDefault().Currency;
        }
        public string getBusinessCurrencySymbol(int id)
        {
            string countryCode = dbContext.KiiPayBusinessInformation.Where(x => x.Id == id).FirstOrDefault().BusinessOperationCountryCode;
            return dbContext.Country.Where(x => x.CountryCode == countryCode).FirstOrDefault().CurrencySymbol;
        }
        public List<CountryDropDownViewModel> GetCountries()
        {
            var result = (from c in dbContext.Country
                          select new CountryDropDownViewModel()
                          {
                              Id = c.Id,
                              Code = c.CountryCode,
                              Name = c.CountryName
                          }
                      ).ToList();

            return result;
        }

        public bool DeductTheCreditOnCard(int CardID, decimal Amount)
        {
            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == CardID).FirstOrDefault();
            result.CurrentBalance = result.CurrentBalance - Amount;
            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            return true;
        }

        public bool IncreaseTheCreditBalanceonMFTC(int MFTCCardId, decimal RecevingAmount)
        {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
            result.CurrentBalance += RecevingAmount;
            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }
        public bool IncreaseTheCreditBalanceonMFBCCard(int MFBCCardId, decimal RecevingAmount)
        {

            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == MFBCCardId).FirstOrDefault();
            result.CurrentBalance += RecevingAmount;
            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }

        public decimal GetCurrentBalanceOnCard()
        {
            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();


            return result.CurrentBalance;
        }

        public bool CheckBalanceForMessage(decimal transactionAmount)
        {
            Services.MerchantNationalPaymentServices _merchantNationalPaymentServices = new MerchantNationalPaymentServices();
            var senderDetails = _merchantNationalPaymentServices.GetSenderBusinessInformation();
            decimal smsFee = dbContext.SmsFee.Where(x => x.CountryCode == senderDetails.Country).Select(x => x.SmsFee).FirstOrDefault();
            decimal accountbalance = senderDetails.CurrentBalance;
            if (accountbalance < smsFee + transactionAmount)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public int getMFTCCardID()
        {

            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();


            return result.Id;
        }
        internal string GetNonCardPaymentReceiptNumberToSave()
        {

            //this code should be unique and random with 8 digit length
            var val = "Os-Np-MF" + Common.Common.GenerateRandomDigit(5);
            while (dbContext.MerchantNonCardTransaction.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Os-Np-MF" + Common.Common.GenerateRandomDigit(5);
            }

            return val;

        }
        internal string ReceiptNoForMerchantInternationalPayment()
        {
            //this code should be unique and random with 8 digit length
            var val = "Os-MP-MF-" + Common.Common.GenerateRandomDigit(5);
            while (dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Os-Mp-MF-" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }
        //internal string ReceiptNoForMerchantNationalPayment()
        //{
        //    //this code should be unique and random with 8 digit length
        //    var val = "Os-MP-MF-" + Common.Common.GenerateRandomDigit(5);
        //    while (dbContext.MFTCCardToMFBCCardTransaction.Where(x => x. == val).Count() > 0)
        //    {
        //        val = "Os-Mp-MF-" + Common.Common.GenerateRandomDigit(5);
        //    }
        //    return val;
        //}
        internal string ReceiptNoForMFTCPayment()
        {
            //this code should be unique and random with 8 digit length
            var val = "Os-Ctu-MF-" + Common.Common.GenerateRandomDigit(5);
            while (dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Os-Ctu-MF-" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }

        internal DB.MerchantNonCardReceiverDetails GetNonCardReceiverDetails(int nonCardRecieverId)
        {
            var result = dbContext.MerchantNonCardReceiverDetail.Where(x => x.Id == nonCardRecieverId).FirstOrDefault();

            return result;
        }

        internal DB.KiiPayBusinessWalletInformation GetMFBCCardInformationByKiiPayBusinessInformationId(int KiiPayBusinessInformationId)
        {

            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).FirstOrDefault();
            return result;

        }
        internal DB.KiiPayBusinessWalletInformation GetMFBCCardInformationByMFBCCardID(int CardId)
        {


            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == CardId).FirstOrDefault();
            return result;
        }

        internal DB.KiiPayPersonalWalletInformation GetMFTCCardUserInformation(int mFTCCardInformationId)
        {

            var result = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == mFTCCardInformationId).FirstOrDefault();

            return result;
        }
        internal string GetNewAccessCodeForBusinessMerchant()
        {
            //this code should be unique and random with8 digit length
            var val = Common.Common.GenerateRandomDigit(8);

            while (dbContext.KiiPayBusinessWalletWithdrawalCode.Where(x => x.AccessCode == val).Count() > 0)
            {
                val = Common.Common.GenerateRandomDigit(8);
            }
            return val;
        }
        
    }

    public class CountryDropDownViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }
}