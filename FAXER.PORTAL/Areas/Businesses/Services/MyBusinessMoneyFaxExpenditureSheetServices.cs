using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class MyBusinessMoneyFaxExpenditureSheetServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices CommonService = new CommonServices();

        string CardUserCountry = Common.BusinessSession.LoggedBusinessMerchant.CountryCode ?? "";

        public MyBusinessMoneyFaxExpenditureSheetServices()
        {

            dbContext = new DB.FAXEREntities();

        }

        public List<ViewModels.MyBusinessMoneyFaxPurchaseViewModel> GetMyBusinessMoneyFaxExpendituresDetials()
        {

            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var MerchantNationalPayment = (from c in dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList().OrderByDescending(x => x.TransactionDate)
                                           join d in dbContext.KiiPayBusinessWalletInformation on c.PayedToKiiPayBusinessWalletInformationId equals d.Id
                                           select new ViewModels.MyBusinessMoneyFaxPurchaseViewModel()
                                           {

                                               Id = c.Id,
                                               NameOfMerchant = d.KiiPayBusinessInformation.BusinessName,
                                               MerchantAccountNo = d.KiiPayBusinessInformation.BusinessMobileNo,
                                               AmountSpent = Common.Common.GetCurrencySymbol(c.PayedFromKiiPayBusinessWalletInformation.Country) + " " + c.AmountSent + " " + Common.Common.GetCountryCurrency(c.PayedFromKiiPayBusinessWalletInformation.Country),
                                               Date = Common.ConversionExtension.ToFormatedString(c.TransactionDate),
                                               Time = c.TransactionDate.ToString("HH:mm"),
                                               PaymentReference = c.PaymentReference,
                                               Country = Common.Common.GetCountryName(d.Country),
                                               City = d.City,
                                               TransactionDateTime = c.TransactionDate
                                           }).ToList();



            var MerchantInternationalPayment = (from c in dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.PayedFromKiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList().OrderByDescending(x => x.TransactionDate)
                                                join d in dbContext.KiiPayBusinessWalletInformation on c.PayedToKiiPayBusinessWalletId equals d.Id
                                                select new ViewModels.MyBusinessMoneyFaxPurchaseViewModel()
                                                {

                                                    Id = c.Id,
                                                    NameOfMerchant = d.KiiPayBusinessInformation.BusinessName,
                                                    MerchantAccountNo = d.KiiPayBusinessInformation.BusinessMobileNo,
                                                    AmountSpent = Common.Common.GetCurrencySymbol(CardUserCountry) + " " + c.TotalAmount + " " + Common.Common.GetCountryCurrency(CardUserCountry),
                                                    Date = Common.ConversionExtension.ToFormatedString(c.TransactionDate),
                                                    Time = c.TransactionDate.ToString("HH:mm"),
                                                    PaymentReference = c.PaymentReference,
                                                    Country = Common.Common.GetCountryName(d.Country),
                                                    City = d.City,
                                                    TransactionDateTime = c.TransactionDate
                                                }).ToList();

            var model = new List<ViewModels.MyBusinessMoneyFaxPurchaseViewModel>();
            model = MerchantNationalPayment.Concat(MerchantInternationalPayment).OrderByDescending(x => x.TransactionDateTime).ToList();
            return model;

        }
        public List<ViewModels.MyBusinessMoneyFaxPurchaseViewModel> FilterExpenditureByDate(DateTime FromDate, DateTime ToDate)
        {

            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var MerchantNationalPayment = (from c in dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessInformationId == KiiPayBusinessInformationId
                          && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate)).ToList().OrderByDescending(x => x.TransactionDate)
                                           join d in dbContext.KiiPayBusinessWalletInformation on c.PayedToKiiPayBusinessWalletInformationId equals d.Id
                                           select new ViewModels.MyBusinessMoneyFaxPurchaseViewModel()
                                           {

                                               Id = c.Id,
                                               NameOfMerchant = d.KiiPayBusinessInformation.BusinessName,
                                               MerchantAccountNo = d.KiiPayBusinessInformation.BusinessMobileNo,
                                               AmountSpent = Common.Common.GetCurrencySymbol(CardUserCountry) + " " + c.AmountSent + " " + Common.Common.GetCountryCurrency(CardUserCountry),
                                               Date = Common.ConversionExtension.ToFormatedString(c.TransactionDate),
                                               Time = c.TransactionDate.ToString("HH:mm"),
                                               PaymentReference = c.PaymentReference,
                                               Country = Common.Common.GetCountryName(d.Country),
                                               City = d.City
                                           }
                          ).ToList();

            var MerchantInternationalPayment = (from c in dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.PayedFromKiiPayBusinessInformationId == KiiPayBusinessInformationId
                                                 && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate)).ToList().OrderByDescending(x => x.TransactionDate)
                                                join d in dbContext.KiiPayBusinessWalletInformation on c.PayedToKiiPayBusinessWalletId equals d.Id
                                                select new ViewModels.MyBusinessMoneyFaxPurchaseViewModel()
                                                {

                                                    Id = c.Id,
                                                    NameOfMerchant = d.KiiPayBusinessInformation.BusinessName,
                                                    MerchantAccountNo = d.KiiPayBusinessInformation.BusinessMobileNo,
                                                    AmountSpent = Common.Common.GetCurrencySymbol(CardUserCountry) + " " + c.TotalAmount + " " + Common.Common.GetCountryCurrency(CardUserCountry),
                                                    Date = Common.ConversionExtension.ToFormatedString(c.TransactionDate),
                                                    Time = c.TransactionDate.ToString("HH:mm"),
                                                    PaymentReference = c.PaymentReference,
                                                    Country = Common.Common.GetCountryName(d.Country),
                                                    City = d.City,
                                                    TransactionDateTime = c.TransactionDate
                                                }
                     ).ToList();

            var model = new List<ViewModels.MyBusinessMoneyFaxPurchaseViewModel>();
            model = MerchantNationalPayment.Concat(MerchantInternationalPayment).OrderByDescending(x => x.TransactionDateTime).ToList();
            return model;

        }


        public List<ViewModels.MyBusinessMoneyFaxTopUpViewModel> GetTopUpDetails()
        {


            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var MFTCCardTopUPByMerchant = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList().OrderByDescending(x => x.TransactionDate)
                                           select new ViewModels.MyBusinessMoneyFaxTopUpViewModel()
                                           {

                                               Id = c.Id,
                                               CardUserName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + "" + c.KiiPayPersonalWalletInformation.LastName,
                                               MFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Contains("MFS") ? c.KiiPayPersonalWalletInformation.MobileNo.GetVirtualAccountNo() : c.KiiPayPersonalWalletInformation.MobileNo.Decrypt().GetVirtualAccountNo(),
                                               AmountSpent = Common.Common.GetCurrencySymbol(CardUserCountry) + " " + c.TotalAmount.ToString() + " " + Common.Common.GetCountryCurrency(CardUserCountry),
                                               Date = Common.ConversionExtension.ToFormatedString(c.TransactionDate),
                                               Time = c.TransactionDate.ToString("HH:mm"),
                                               PaymentReference = c.PaymentReference,
                                               Country = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                               City = c.KiiPayPersonalWalletInformation.CardUserCity
                                           }
                          ).ToList();

            return MFTCCardTopUPByMerchant;



        }
        public List<ViewModels.MyBusinessMoneyFaxTopUpViewModel> GetFilterTopUpDetails(DateTime FromDate, DateTime ToDate)
        {


            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var MFTCCardTopUPByMerchant = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate)).ToList().OrderByDescending(x => x.TransactionDate)
                                           select new ViewModels.MyBusinessMoneyFaxTopUpViewModel()
                                           {

                                               Id = c.Id,
                                               CardUserName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + "" + c.KiiPayPersonalWalletInformation.LastName,
                                               MFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Contains("MFS") ? c.KiiPayPersonalWalletInformation.MobileNo.GetVirtualAccountNo() : c.KiiPayPersonalWalletInformation.MobileNo.Decrypt().GetVirtualAccountNo(),
                                               AmountSpent = Common.Common.GetCurrencySymbol(CardUserCountry) + " " + c.TotalAmount.ToString() + " " + Common.Common.GetCountryCurrency(CardUserCountry),
                                               Date = Common.ConversionExtension.ToFormatedString(c.TransactionDate),
                                               Time = c.TransactionDate.ToString("HH:mm"),
                                               PaymentReference = c.PaymentReference,
                                               Country = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                               City = c.KiiPayPersonalWalletInformation.CardUserCity
                                           }
                          ).ToList();

            return MFTCCardTopUPByMerchant;



        }
    }
}