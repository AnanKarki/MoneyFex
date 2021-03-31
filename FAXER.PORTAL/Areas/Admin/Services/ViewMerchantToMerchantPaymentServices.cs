using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMerchantToMerchantPaymentServices
    {

        DB.FAXEREntities dbContext = null;
        public ViewMerchantToMerchantPaymentServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewMerchantToMerchantPaymentViewModel> GetMerchantToMerchantPaymentDetails(string CountryCode, string City)
        {


            var MerchantInternationalTransaction = dbContext.KiiPayBusinessInternationalPaymentTransaction.ToList();

            var MerchantLocalTransaction = dbContext.KiiPayBusinessLocalTransaction.ToList();

            if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                MerchantInternationalTransaction = (from c in dbContext.KiiPayBusinessInternationalPaymentTransaction
                                                    join d in dbContext.KiiPayBusinessInformation.Where(x => x.BusinessOperationCountryCode.ToLower() == CountryCode.ToLower()
                                                                        && x.BusinessOperationCity.ToLower() == City.ToLower()) on c.PayedFromKiiPayBusinessInformationId equals d.Id
                                                    select c).ToList();

                MerchantLocalTransaction = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessWalletInformation.Country.ToLower() == CountryCode.ToLower()
                                                                                   && x.PayedFromKiiPayBusinessWalletInformation.City.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode))
            {

                MerchantInternationalTransaction = (from c in dbContext.KiiPayBusinessInternationalPaymentTransaction
                                                    join d in dbContext.KiiPayBusinessInformation.Where(x => x.BusinessOperationCountryCode.ToLower() == CountryCode.ToLower())
                                                    on c.PayedFromKiiPayBusinessInformationId equals d.Id
                                                    select c).ToList();

                MerchantLocalTransaction = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessWalletInformation.Country.ToLower() == CountryCode.ToLower()).ToList();
            }

            var result_MerchantInternationalTransaction = (from c in MerchantInternationalTransaction
                                                           join sender in dbContext.KiiPayBusinessInformation on c.PayedFromKiiPayBusinessInformationId equals sender.Id
                                                           join receiver in dbContext.KiiPayBusinessInformation on c.PayedToKiiPayBusinessInformationId equals receiver.Id
                                                           select new ViewMerchantToMerchantPaymentViewModel()
                                                           {
                                                               SenderName = sender.BusinessName,
                                                               SenderCity = sender.BusinessOperationCity,
                                                               SenderCountry = Common.Common.GetCountryName(sender.BusinessOperationCountryCode),
                                                               SenderMFSCode = sender.BusinessMobileNo,
                                                               ReceiverName = receiver.BusinessName,
                                                               ReceiverCity = receiver.BusinessOperationCity,
                                                               ReceiverCountry = Common.Common.GetCountryName(receiver.BusinessOperationCountryCode),
                                                               ReceiverEmail = receiver.Email,
                                                               ReceiverMFSCode = receiver.BusinessMobileNo,
                                                               Fee = c.FaxingFee,
                                                               PaymentType = "International",
                                                               TransactionAmount = c.FaxingAmount,
                                                               TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                               TransactionId = c.Id,
                                                               TransactionTime = c.TransactionDate.ToString("HH:mm"),
                                                               TransactionDateTime = c.TransactionDate,
                                                               SenderCurrency = Common.Common.GetCountryCurrency(sender.BusinessOperationCountryCode),
                                                               SenderCurrencySymbol = Common.Common.GetCurrencySymbol(sender.BusinessOperationCountryCode),
                                                               ReceiverCurrency = Common.Common.GetCountryCurrency(receiver.BusinessOperationCountryCode),
                                                               ReceiverCurrencySymbol = Common.Common.GetCurrencySymbol(receiver.BusinessOperationCountryCode)
                                                           }).ToList();


            var result_MerchantLocalInternational = (from c in MerchantLocalTransaction
                                                     join sender in dbContext.KiiPayBusinessInformation on c.PayedFromKiiPayBusinessInformationId equals sender.Id
                                                     join receiver in dbContext.KiiPayBusinessInformation on c.PayedToKiiPayBusinessInformationId equals receiver.Id
                                                     select new ViewMerchantToMerchantPaymentViewModel()
                                                     {
                                                         SenderName = sender.BusinessName,
                                                         SenderCity = sender.BusinessOperationCity,
                                                         SenderCountry = Common.Common.GetCountryName(sender.BusinessOperationCountryCode),
                                                         SenderMFSCode = sender.BusinessMobileNo,
                                                         ReceiverName = receiver.BusinessName,
                                                         ReceiverCity = receiver.BusinessOperationCity,
                                                         ReceiverCountry = Common.Common.GetCountryName(receiver.BusinessOperationCountryCode),
                                                         ReceiverEmail = receiver.Email,
                                                         ReceiverMFSCode = receiver.BusinessMobileNo,
                                                         Fee = 0,
                                                         PaymentType = "Local",
                                                         TransactionAmount = c.AmountSent,
                                                         TransactionDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                         TransactionId = c.Id,
                                                         TransactionTime = c.TransactionDate.ToString("HH:mm"),
                                                         TransactionDateTime =c.TransactionDate,
                                                         SenderCurrency = Common.Common.GetCountryCurrency(sender.BusinessOperationCountryCode),
                                                         SenderCurrencySymbol = Common.Common.GetCurrencySymbol(sender.BusinessOperationCountryCode),
                                                         ReceiverCurrency = Common.Common.GetCountryCurrency(receiver.BusinessOperationCountryCode),
                                                         ReceiverCurrencySymbol = Common.Common.GetCurrencySymbol(receiver.BusinessOperationCountryCode)
                                                     }).ToList();

            var result = new List<ViewMerchantToMerchantPaymentViewModel>();

            result = result_MerchantInternationalTransaction.Concat(result_MerchantLocalInternational).OrderByDescending(x => x.TransactionDate).ToList();

            return result;
        }

        internal DB.KiiPayBusinessWalletInformation GetBusinessDetails(int ID)
        {

            var result = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == ID).FirstOrDefault();
            return result;
        }

        internal DB.KiiPayBusinessInternationalPaymentTransaction GetMerchantToMerchantTransactionDetail(int transactionId)
        {
            var result = dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.Id == transactionId).FirstOrDefault();
            return result;
        }
    }
}