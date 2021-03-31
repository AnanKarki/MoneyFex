using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class MyMoneyFaxCardPurchaseSheetServices
    {
        DB.FAXEREntities dbContext = null;
        public MyMoneyFaxCardPurchaseSheetServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.MyMoneyFaxCardPurchaseSheetViewModel> GetAllDetails()
        {

            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            var result = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                          select new ViewModels.MyMoneyFaxCardPurchaseSheetViewModel()
                          {
                              TransactionId = c.Id,
                              BusinessMerhantName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                              BusinessMobileNo = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              PaymentReference = c.PaymentReference,
                              AmoountSpent = c.AmountSent.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                              Time = c.TransactionDate.ToString("HH:mm"),
                              TransactionDate = c.TransactionDate
                          }).ToList();

            var MerchantInternationalPayment = (from c in dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedFromKiiPayPersonalWalletId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                                                select new ViewModels.MyMoneyFaxCardPurchaseSheetViewModel()
                                                {
                                                    TransactionId = c.Id,
                                                    BusinessMerhantName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                                    BusinessMobileNo = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                                                    PaymentReference = c.PaymentReference,
                                                    AmoountSpent = c.TotalAmount.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                    Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                    Time = c.TransactionDate.ToString("HH:mm"),
                                                    TransactionDate = c.TransactionDate
                                                }).ToList();
            //return result;

            var model = new List<ViewModels.MyMoneyFaxCardPurchaseSheetViewModel>();
            model = result.Concat(MerchantInternationalPayment).OrderByDescending(x => x.TransactionDate).ToList();
            return model;
        }
        public List<ViewModels.MyMoneyFaxCardPurchaseSheetViewModel> FilterAllDetails(DateTime fromDate, DateTime toDate)
        {

            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            var result = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId
                          && (DbFunctions.TruncateTime(x.TransactionDate) >= fromDate.Date
                          && DbFunctions.TruncateTime(x.TransactionDate) <= toDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                          join Currency in dbContext.Country on c.KiiPayPersonalWalletInformation.CardUserCountry equals Currency.CountryCode
                          select new ViewModels.MyMoneyFaxCardPurchaseSheetViewModel()
                          {
                              TransactionId = c.Id,
                              BusinessMerhantName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                              BusinessMobileNo = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              PaymentReference = c.PaymentReference,
                              AmoountSpent = c.AmountSent.ToString() + " " + Currency.Currency,
                              Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                              Time = c.TransactionDate.ToString("HH:mm")
                          }).ToList();

            var MerchantInternationalPayment = (from c in dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedFromKiiPayPersonalWalletId == MFTCCardId
                                                 && (DbFunctions.TruncateTime(x.TransactionDate) >= fromDate.Date
                                                && DbFunctions.TruncateTime(x.TransactionDate) <= toDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                                                select new ViewModels.MyMoneyFaxCardPurchaseSheetViewModel()
                                                {
                                                    TransactionId = c.Id,
                                                    BusinessMerhantName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                                    BusinessMobileNo = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                                                    PaymentReference = c.PaymentReference,
                                                    AmoountSpent = c.TotalAmount.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                    Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                    Time = c.TransactionDate.ToString("HH:mm")
                                                }).ToList();


            var model = new List<ViewModels.MyMoneyFaxCardPurchaseSheetViewModel>();
            model = result.Concat(MerchantInternationalPayment).OrderByDescending(x => x.TransactionDate).ToList();
            return model;
        }


        public List<ViewModels.MyMoneyFaxCardTopUpSheetViewModel> GetTopUpDetails()
        {

            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            var MFTCTopUpByCarduser = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderWalletId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                                       join d in dbContext.KiiPayPersonalWalletInformation on c.SenderWalletId equals d.Id
                                       select new ViewModels.MyMoneyFaxCardTopUpSheetViewModel()
                                       {
                                           TransactionId = c.Id,
                                           CarduserName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                           MFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Contains("MFS") ? d.MobileNo.GetVirtualAccountNo() : c.KiiPayPersonalWalletInformation.MobileNo.Decrypt().GetVirtualAccountNo(),
                                           PaymentReference = c.PaymentReference,
                                           AmoountSpent = c.TotalAmount.ToString() + " " + Common.Common.GetCountryCurrency(d.CardUserCountry),
                                           Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                           Time = c.TransactionDate.ToString("HH:mm"),
                                           TransactionDate = c.TransactionDate
                                       }).ToList();

            return MFTCTopUpByCarduser;
        }

        public List<ViewModels.MyMoneyFaxCardTopUpSheetViewModel> FilterGetTopUpDetails(DateTime fromDate, DateTime toDate)
        {
            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            var MFTCTopUpByCarduser = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderWalletId == MFTCCardId
                                        && (DbFunctions.TruncateTime(x.TransactionDate) >= fromDate.Date
                                        && DbFunctions.TruncateTime(x.TransactionDate) <= toDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                                       join d in dbContext.KiiPayPersonalWalletInformation on c.SenderWalletId equals d.Id
                                       select new ViewModels.MyMoneyFaxCardTopUpSheetViewModel()
                                       {
                                           TransactionId = c.Id,
                                           CarduserName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                           MFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Contains("MFS") ? c.KiiPayPersonalWalletInformation.MobileNo.GetVirtualAccountNo() : c.KiiPayPersonalWalletInformation.MobileNo.Decrypt().GetVirtualAccountNo(),
                                           PaymentReference = c.PaymentReference,
                                           AmoountSpent = c.TotalAmount.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                           Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                           Time = c.TransactionDate.ToString("HH:mm"),
                                           TransactionDate = c.TransactionDate
                                       }).ToList();
            return MFTCTopUpByCarduser;
        }
    }
}