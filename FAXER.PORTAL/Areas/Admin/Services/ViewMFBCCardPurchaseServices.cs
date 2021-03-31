using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMFBCCardPurchaseServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public List<ViewMFBCCardPurchaseViewModel> getMFBCPurchaseList(string CountryCode = "", string City = "")
        {
            var nationalPayment = new List<DB.KiiPayBusinessLocalTransaction>();

            var InternationalPayment = new List<DB.KiiPayBusinessInternationalPaymentTransaction>();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                nationalPayment = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessWalletInformation.Country == CountryCode).ToList();

                InternationalPayment = (from c in dbContext.KiiPayBusinessInternationalPaymentTransaction
                                        join d in dbContext.KiiPayBusinessWalletInformation.Where(x => x.Country == CountryCode) on c.PayedFromKiiPayBusinessWalletId equals d.Id
                                        select c).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                nationalPayment = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessWalletInformation.City.ToLower() == City.ToLower()).ToList();

                InternationalPayment = (from c in dbContext.KiiPayBusinessInternationalPaymentTransaction
                                        join d in dbContext.KiiPayBusinessWalletInformation.Where(x => x.City.ToLower() == City.ToLower()) on c.PayedFromKiiPayBusinessWalletId equals d.Id
                                        select c).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                nationalPayment = dbContext.KiiPayBusinessLocalTransaction.Where(x => (x.PayedFromKiiPayBusinessWalletInformation.City.ToLower() == City.ToLower()) && (x.PayedFromKiiPayBusinessWalletInformation.Country == CountryCode)).ToList();

                InternationalPayment = (from c in dbContext.KiiPayBusinessInternationalPaymentTransaction
                                        join d in dbContext.KiiPayBusinessWalletInformation.Where(x => x.Country == CountryCode && x.City.ToLower() == City.ToLower()) on c.PayedFromKiiPayBusinessWalletId equals d.Id
                                        select c).ToList();
            }



            var MerchantLocalPayment = (from c in nationalPayment
                                        select new ViewMFBCCardPurchaseViewModel()
                                        {
                                            Id = c.Id,
                                            BusinessName = c.PayedFromKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                            CardUserFullName = c.PayedFromKiiPayBusinessWalletInformation.FirstName + " " + c.PayedFromKiiPayBusinessWalletInformation.MiddleName + " " + c.PayedFromKiiPayBusinessWalletInformation.LastName,
                                            PurchaseAmt = c.AmountSent,
                                            Currency = CommonService.getCurrencyCodeFromCountry(c.PayedFromKiiPayBusinessWalletInformation.Country),
                                            PurchaseTime = c.TransactionDate.ToString("HH:mm"),
                                            PurchaseDate = c.TransactionDate.ToFormatedString(),
                                            MerchantVerifier = "",
                                            MerchantName = c.PayedToKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                            MerchantBMFSCode = c.PayedToKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                                            TransactionDate = c.TransactionDate,
                                            PaymentType = "Local"
                                        }).ToList();

            var MerchantInternationalPayment = (from c in InternationalPayment
                                                join PayedFromMerch in dbContext.KiiPayBusinessWalletInformation on c.PayedFromKiiPayBusinessWalletId equals PayedFromMerch.Id
                                                select new ViewMFBCCardPurchaseViewModel()
                                                {

                                                    Id = c.Id,
                                                    BusinessName = PayedFromMerch.KiiPayBusinessInformation.BusinessName,
                                                    CardUserFullName = PayedFromMerch.FirstName + " " + PayedFromMerch.MiddleName + " " + PayedFromMerch.LastName,
                                                    PurchaseAmt = c.RecievingAmount,
                                                    Currency = CommonService.getCurrencyCodeFromCountry(PayedFromMerch.Country),
                                                    PurchaseTime = c.TransactionDate.ToString("HH:mm"),
                                                    PurchaseDate = c.TransactionDate.ToFormatedString(),
                                                    MerchantVerifier = "",
                                                    MerchantName = c.PayedToKiiPayBusinessWallet.KiiPayBusinessInformation.BusinessName,
                                                    MerchantBMFSCode = c.PayedToKiiPayBusinessWallet.KiiPayBusinessInformation.BusinessMobileNo,
                                                    TransactionDate = c.TransactionDate,
                                                    PaymentType = "International"
                                                }).ToList();

            var result = new List<ViewMFBCCardPurchaseViewModel>();

            result = MerchantLocalPayment.Concat(MerchantInternationalPayment).OrderByDescending(x => x.TransactionDate).ToList();
            return result;
        }

        //        public AdminPayGoodsAndServicesReceiptViewModel getReceiptInfo(int id)
        //        {
        //            var result = (from c in dbContext.MFBCCardTransactions.Where(x=>x.Id == id).ToList()
        //                          select new AdminPayGoodsAndServicesReceiptViewModel()
        //                          {
        //ReceiptNumber = c.re
        //                          }
        //                          )
        //        }
    }
}