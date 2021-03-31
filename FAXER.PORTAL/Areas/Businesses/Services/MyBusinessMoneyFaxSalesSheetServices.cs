using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class MyBusinessMoneyFaxSalesSheetServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices CommonService = new CommonServices();

        public MyBusinessMoneyFaxSalesSheetServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.MyBusinessMoneyFaxSalesSheetViewModel> GetSalesSheetDetials()
        {

            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var FaxerMerchantTransaction = (from c in dbContext.FaxerMerchantPaymentInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList()
                                            join d in dbContext.FaxerMerchantPaymentTransaction.OrderByDescending(x => x.PaymentDate) on c.Id equals d.SenderKiiPayBusinessPaymentInformationId
                                            select new ViewModels.MyBusinessMoneyFaxSalesSheetViewModel()
                                            {

                                                NameOfBuyer = c.SenderInformation.FirstName + " " + c.SenderInformation.LastName,
                                                PaymentReference = d.PaymentReference,
                                                //Amount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " "+ d.PaymentAmount.ToString() + " " + CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),
                                                Amount = Common.Common.GetCurrencySymbol(c.KiiPayBusinessInformation.BusinessOperationCountryCode) + " " + d.ReceivingAmount + " " + Common.Common.GetCountryCurrency(c.KiiPayBusinessInformation.BusinessOperationCountryCode),
                                                Date = d.PaymentDate,
                                                Time = d.PaymentDate.ToString("HH:mm"),
                                                Country = Common.Common.GetCountryName(c.SenderInformation.Country),
                                                City = c.SenderInformation.City



                                            }).ToList();
            var MFBCCardId = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded) ).Select(x => x.Id).FirstOrDefault();
            var MerchantLocalTransaction = (from c in dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList().OrderByDescending(x => x.TransactionDate)
                                            select new ViewModels.MyBusinessMoneyFaxSalesSheetViewModel()
                                            {
                                                NameOfBuyer = c.PayedFromKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                                PaymentReference = c.PaymentReference,
                                                //Amount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " " + c.AmountSent.ToString() + " " + CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),
                                                Amount = Common.Common.GetCurrencySymbol(c.PayedFromKiiPayBusinessWalletInformation.Country) + " " + c.AmountSent + " " + Common.Common.GetCountryCurrency(c.PayedFromKiiPayBusinessWalletInformation.Country),
                                                Date = c.TransactionDate,
                                                Time = c.TransactionDate.ToString("HH:mm"),
                                                Country = Common.Common.GetCountryName(c.PayedFromKiiPayBusinessWalletInformation.Country),
                                                City = c.PayedFromKiiPayBusinessWalletInformation.City


                                            }
                           ).ToList();

            var MerchantInternationalTransaction = (from c in dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList().OrderByDescending(x => x.TransactionDate)
                                                    join PayedFromMFBCCardInformation in dbContext.KiiPayBusinessWalletInformation on c.PayedFromKiiPayBusinessWalletId equals PayedFromMFBCCardInformation.Id
                                                    select new ViewModels.MyBusinessMoneyFaxSalesSheetViewModel()
                                                    {
                                                        NameOfBuyer = PayedFromMFBCCardInformation.KiiPayBusinessInformation.BusinessName,
                                                        PaymentReference = c.PaymentReference,
                                                        //Amount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " " + c.AmountSent.ToString() + " " + CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),
                                                        Amount = Common.Common.GetCurrencySymbol(c.PayedToKiiPayBusinessWallet.Country) + " " + c.RecievingAmount + " " + Common.Common.GetCountryCurrency(c.PayedToKiiPayBusinessWallet.Country),
                                                        Date = c.TransactionDate,
                                                        Time = c.TransactionDate.ToString("HH:mm"),
                                                        Country = Common.Common.GetCountryName(PayedFromMFBCCardInformation.Country),
                                                        City = PayedFromMFBCCardInformation.City


                                                    }).ToList();


            var CardUserLocalPurchase = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayBusinessWalletInformation.KiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList()
                                         select new ViewModels.MyBusinessMoneyFaxSalesSheetViewModel()
                                         {
                                             NameOfBuyer = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                             //Amount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " " + c.AmountSent.ToString() + " " + CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),,
                                             Amount = Common.Common.GetCurrencySymbol(c.KiiPayBusinessWalletInformation.Country) + " " + c.AmountSent + " " + Common.Common.GetCountryCurrency(c.KiiPayBusinessWalletInformation.Country),
                                             PaymentReference = c.PaymentReference,
                                             Date = c.TransactionDate,
                                             Time = c.TransactionDate.ToString("HH:mm"),
                                             Country = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                             City = c.KiiPayPersonalWalletInformation.CardUserCity
                                         }).ToList();




            var CardUserInterNationalPurchase = (from c in dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList()
                                                 select new ViewModels.MyBusinessMoneyFaxSalesSheetViewModel()
                                                 {
                                                     NameOfBuyer = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                                     //Amount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " " + c.AmountSent.ToString() + " " + CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),
                                                     Amount = Common.Common.GetCurrencySymbol(c.KiiPayBusinessWalletInformation.Country) + " " + c.ReceivingAmount + " " + Common.Common.GetCountryCurrency(c.KiiPayBusinessWalletInformation.Country),
                                                     PaymentReference = c.PaymentReference,
                                                     Date = c.TransactionDate,
                                                     Time = c.TransactionDate.ToString("HH:mm"),
                                                     Country = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                     City = c.KiiPayPersonalWalletInformation.CardUserCity

                                                 }).ToList();
            var model = new List<ViewModels.MyBusinessMoneyFaxSalesSheetViewModel>();
            model = FaxerMerchantTransaction.Concat(MerchantLocalTransaction).Concat(CardUserLocalPurchase).Concat(CardUserInterNationalPurchase).ToList();
            var vm = model.OrderByDescending(x => x.Date).ToList();

            return vm;

        }
        public List<ViewModels.MyBusinessMoneyFaxSalesSheetViewModel> GetSalesSheetDetialsByFilterDate(DateTime FromDate, DateTime ToDate)
        {

            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var FaxerMerchantTransaction = (from c in dbContext.FaxerMerchantPaymentInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList()
                          join d in dbContext.FaxerMerchantPaymentTransaction.Where(x => DbFunctions.TruncateTime(x.PaymentDate) >= FromDate && DbFunctions.TruncateTime(x.PaymentDate) <= ToDate) on c.Id equals d.SenderKiiPayBusinessPaymentInformationId
                          select new ViewModels.MyBusinessMoneyFaxSalesSheetViewModel()
                          {

                              NameOfBuyer = c.SenderInformation.FirstName + " " + c.SenderInformation.LastName,
                              PaymentReference = d.PaymentReference,
                              //Amount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " " + c.PaymentAmount.ToString() + " " + CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),,
                              Amount = Common.Common.GetCurrencySymbol(c.KiiPayBusinessInformation.BusinessOperationCountryCode) + " " + d.ReceivingAmount + " " + Common.Common.GetCountryCurrency(c.KiiPayBusinessInformation.BusinessOperationCountryCode),
                              Date = d.PaymentDate,
                              Time = d.PaymentDate.ToString("HH:mm"),
                              Country = Common.Common.GetCountryName(c.SenderInformation.Country),
                              City = c.SenderInformation.City


                          }).ToList();
            int MFBCCardId = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.CardStatus != DB.CardStatus.IsDeleted && x.CardStatus != DB.CardStatus.IsRefunded)).Select(x => x.Id).FirstOrDefault();
            var MerchantLocalTransaction = (from c in dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId
                           && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate)).ToList()
                           select new ViewModels.MyBusinessMoneyFaxSalesSheetViewModel()
                           {
                               NameOfBuyer = c.PayedFromKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                               PaymentReference = c.PaymentReference,
                               //Amount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " " + c.AmountSent.ToString() + " " + CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),,
                               Amount = Common.Common.GetCurrencySymbol(c.PayedToKiiPayBusinessWalletInformation.Country) + " " + c.AmountSent + " " + Common.Common.GetCountryCurrency(c.PayedToKiiPayBusinessWalletInformation.Country),
                               Date = c.TransactionDate,
                               Time = c.TransactionDate.ToString("HH:mm"),
                               Country = Common.Common.GetCountryName(c.PayedFromKiiPayBusinessWalletInformation.Country),
                               City = c.PayedFromKiiPayBusinessWalletInformation.City
                           }
                           ).ToList();

            var MerchantInternationalTransaction = (from c in dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId
                                                     && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate)).ToList().OrderByDescending(x => x.TransactionDate)
                                                    join PayedFromMFBCCardInformation in dbContext.KiiPayBusinessWalletInformation on c.PayedFromKiiPayBusinessWalletId equals PayedFromMFBCCardInformation.Id
                                                    select new ViewModels.MyBusinessMoneyFaxSalesSheetViewModel()
                                                    {

                                                        NameOfBuyer = PayedFromMFBCCardInformation.KiiPayBusinessInformation.BusinessName,
                                                        PaymentReference = c.PaymentReference,
                                                        //Amount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " " + c.AmountSent.ToString() + " " + CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),
                                                        Amount = Common.Common.GetCurrencySymbol(c.PayedToKiiPayBusinessWallet.Country) + " " + c.TotalAmount + " " + Common.Common.GetCountryCurrency(c.PayedToKiiPayBusinessWallet.Country),
                                                        Date = c.TransactionDate,
                                                        Time = c.TransactionDate.ToString("HH:mm"),
                                                        Country = Common.Common.GetCountryName(PayedFromMFBCCardInformation.Country),
                                                        City = PayedFromMFBCCardInformation.City


                                                    }).ToList();


            var CardUserLocalPurchase = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayBusinessWalletInformation.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate)).ToList()
                                    select new ViewModels.MyBusinessMoneyFaxSalesSheetViewModel()
                                    {
                                        NameOfBuyer = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                        //Amount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " " + c.AmountSent.ToString() + " " + CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),
                                        Amount = Common.Common.GetCurrencySymbol(c.KiiPayBusinessWalletInformation.Country) + " " + c.AmountSent + " " + Common.Common.GetCountryCurrency(c.KiiPayBusinessWalletInformation.Country),
                                        PaymentReference = c.PaymentReference,
                                        Date = c.TransactionDate,
                                        Time = c.TransactionDate.ToString("HH:mm"),
                                        Country = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                        City = c.KiiPayPersonalWalletInformation.CardUserCity

                                    }).ToList();

            var CardUserInterNationalPurchase = (from c in dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate)).ToList()
                                                 select new ViewModels.MyBusinessMoneyFaxSalesSheetViewModel()
                                                 {
                                                     NameOfBuyer = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                                     //Amount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " " + c.AmountSent.ToString() + " " + CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),
                                                     Amount = Common.Common.GetCurrencySymbol(c.KiiPayBusinessWalletInformation.Country) + " " + c.ReceivingAmount + " " + Common.Common.GetCountryCurrency(c.KiiPayBusinessWalletInformation.Country),
                                                     PaymentReference = c.PaymentReference,
                                                     Date = c.TransactionDate,
                                                     Time = c.TransactionDate.ToString("HH:mm"),
                                                     Country = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                     City = c.KiiPayPersonalWalletInformation.CardUserCity

                                                 }).ToList();
            var model = new List<ViewModels.MyBusinessMoneyFaxSalesSheetViewModel>();
            model = FaxerMerchantTransaction.Concat(MerchantLocalTransaction).Concat(CardUserLocalPurchase).Concat(CardUserInterNationalPurchase).ToList();
            var vm = model.OrderByDescending(x => x.Date).ToList();
            return vm;

        }
    }
}