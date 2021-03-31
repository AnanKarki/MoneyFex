using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class PayGoodsAndServicesAbroadServices
    {
        DB.FAXEREntities dbContext = null;
        public PayGoodsAndServicesAbroadServices()
        {

            dbContext = new DB.FAXEREntities();
        }
        public string getMFTCCardCurrency(int id)
        {
            string countryCode = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == id).FirstOrDefault().CardUserCountry;
            return dbContext.Country.Where(x => x.CountryCode == countryCode).FirstOrDefault().Currency;
        }
        public string getMFTCCurrencySymbol(int id)
        {
            string countryCode = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == id).FirstOrDefault().CardUserCountry;
            return dbContext.Country.Where(x => x.CountryCode == countryCode).FirstOrDefault().CurrencySymbol;
        }

        public DB.KiiPayPersonalWalletInformation GetMFTCCardInformation()
        {

            int CardUserID = Common.CardUserSession.LoggedCardUserViewModel.id;
            int MFTCCardId = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == CardUserID).Select(x => x.KiiPayPersonalWalletInformationId).FirstOrDefault();

            var MFTCCardInformation = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
            

            return MFTCCardInformation;

        }

        public bool AddTransaction(DB.KiiPayPersonalNationalKiiPayBusinessPayment tran)
        {

            dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Add(tran);
            int result = dbContext.SaveChanges();
            if (result > 0)
            {

                var MFTCdata = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == tran.KiiPayPersonalWalletInformationId).FirstOrDefault();
                MFTCdata.CurrentBalance -= tran.AmountSent;
                dbContext.Entry(MFTCdata).State = System.Data.Entity.EntityState.Modified;
                var MFBCdata = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == tran.KiiPayBusinessWalletInformationId).FirstOrDefault();
                MFBCdata.CurrentBalance += tran.AmountSent;
                dbContext.Entry(MFBCdata).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;

            }
            return false;
        }
        public DB.KiiPayBusinessWalletInformation GetMFBCCardInformationByCardNumber(string CardNumber)
        {

            var result = dbContext.KiiPayBusinessWalletInformation.ToList();

            for (int i = 0; i < result.Count; i++)
            {

                string[] tokens = result[i].MobileNo.Decrypt().Split('-');
                if (tokens[1] == CardNumber)
                {

                    var MFBCCard = result[i].MobileNo;
                    var model = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == MFBCCard && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
                    return model;
                }

            }
            return null;
        }
        public DB.KiiPayBusinessWalletInformation GetMFBCCardInformation(string MFBCCard)
        {
            var MFBC = MFBCCard.Encrypt();
            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == MFBC && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();

            return result;

        }
        public string GetAmountOnCard()
        {

            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            var Carddetials = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();

            var AmountonCard = Carddetials.CurrentBalance;
            string CountryCurrency = Common.Common.GetCountryCurrency(Carddetials.CardUserCountry);

            return AmountonCard.ToString() + " " + CountryCurrency;

        }

        public string getCardPhoto()
        {
            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            string photoUrl = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault().UserPhoto;
            return photoUrl;
        }

        public DB.KiiPayBusinessInformation GetBusinessInformation(int MFBCCardid)
        {

            int KiiPayBusinessInformationId = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == MFBCCardid).Select(x => x.KiiPayBusinessInformationId).FirstOrDefault();
            var result = dbContext.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessInformationId).FirstOrDefault();
            return result;

        }
        public DB.FaxerInformation GetFaxerDetails(int FaxerId)
        {


            
            var result = dbContext.FaxerInformation.Where(x => x.Id == FaxerId).FirstOrDefault();
            return result;


        }

        public decimal TotalGoodsPurchaseAmount(DateTime StartTransactionDate, int CardId)
        {

            var TotaAmountPurchase = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId == CardId && x.TransactionDate >= StartTransactionDate).Sum(x => (Decimal?)x.AmountSent) ?? 0;


            var InternationalMerchantPayment = dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedFromKiiPayPersonalWalletId ==CardId && x.TransactionDate >= StartTransactionDate).Sum(x => (Decimal?)x.TotalAmount) ?? 0;

            return TotaAmountPurchase + InternationalMerchantPayment;

        }

    }
}