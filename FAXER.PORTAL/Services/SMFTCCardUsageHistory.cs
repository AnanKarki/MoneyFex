using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SMFTCCardUsageHistory
    {
        DB.FAXEREntities dbContext = null;

        public SMFTCCardUsageHistory()
        {
            dbContext = new DB.FAXEREntities();
        }


        public List<Models.MFTCCardTopUpHistoryViewModel> GetTopUpDetails(int MFTCCardId)
        {


            var result = (from c in dbContext.FaxingCardTransaction.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                          select new Models.MFTCCardTopUpHistoryViewModel()
                          {
                              FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry),
                              ReceivingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              TopUpAmount = c.RecievingAmount,
                              TopUpDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TopUpTime = c.TransactionDate.ToString("HH:mm"),
                              TransationDate = c.TransactionDate,
                              TopUpReference = c.OperatingUserType == DB.OperatingUserType.Admin ? "Admin" : "Registrar"
                          }).ToList();

            var result2 = (from c in dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.KiiPayPersonalWalletId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                           select new Models.MFTCCardTopUpHistoryViewModel()
                           {
                               FaxerCurrency = Common.Common.GetCountryCurrency(dbContext.FaxerInformation.Where(x=> x.Id == c.FaxerId).Select(x => x.Country).FirstOrDefault()),
                               ReceivingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                               TopUpAmount = c.FaxingAmount,
                               TopUpDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                               TopUpTime = c.TransactionDate.ToString("HH:mm"),
                               TransationDate = c.TransactionDate,
                               TopUpReference = c.TopUpReference
                           }).ToList();

            var MFTCCardTopUpByCardUser = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.ReceiverWalletId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                                           select new Models.MFTCCardTopUpHistoryViewModel()
                                           {
                                               FaxerCurrency = Common.Common.GetCountryCurrency(dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == c.SenderWalletId).Select(x=> x.CardUserCountry).FirstOrDefault()),
                                               ReceivingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                               TopUpAmount = c.FaxingAmount,
                                               TopUpDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                               TopUpTime = c.TransactionDate.ToString("HH:mm"),
                                               TransationDate = c.TransactionDate,
                                               TopUpReference = c.PaymentReference
                                           }).ToList();

            var MFTCCardTopUpByBusinessMerchant = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                                                   select new Models.MFTCCardTopUpHistoryViewModel()
                                                   {
                                                       FaxerCurrency = Common.Common.GetCountryCurrency(c.KiiPayBusinessWalletInformation.Country),
                                                       ReceivingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                       TopUpAmount = c.PayingAmount,
                                                       TopUpDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                       TopUpTime = c.TransactionDate.ToString("HH:mm"),
                                                       TransationDate = c.TransactionDate,
                                                       TopUpReference = c.PaymentReference
                                                   }).ToList();

            //var MFTCCarTopUP
            var data = new List<Models.MFTCCardTopUpHistoryViewModel>();
            data = result.Concat(result2).Concat(MFTCCardTopUpByCardUser).Concat(MFTCCardTopUpByBusinessMerchant).OrderByDescending(x => x.TransationDate).ToList();
            return data;


        }
        public List<Models.MFTCCardTopUpHistoryViewModel> GetTopUpDetailsFilterByDate(int MFTCCardId, DateTime FromDate, DateTime ToDate)
        {


            var result = (from c in dbContext.FaxingCardTransaction.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId
                          && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate.Date
                          && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                          select new Models.MFTCCardTopUpHistoryViewModel()
                          {
                              FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.FaxerCountry),
                              ReceivingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              TopUpAmount = c.FaxingAmount,
                              TopUpDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              TopUpTime = c.TransactionDate.ToString("HH:mm"),
                              TopUpReference = c.OperatingUserType == DB.OperatingUserType.Admin ? "Admin" : "Registrar",
                              TransationDate = c.TransactionDate
                          }).ToList();

            var result2 = (from c in dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.KiiPayPersonalWalletId == MFTCCardId
                          && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate.Date
                          && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                           join d in dbContext.FaxerInformation on c.FaxerId equals d.Id
                           select new Models.MFTCCardTopUpHistoryViewModel()
                           {
                               FaxerCurrency = Common.Common.GetCountryCurrency(d.Country),
                               ReceivingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                               TopUpAmount = c.FaxingAmount,
                               TopUpDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                               TopUpTime = c.TransactionDate.ToString("HH:mm"),
                               TopUpReference = c.TopUpReference,
                               TransationDate = c.TransactionDate
                           }).ToList();



            var MFTCCardTopUpByCardUser = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.ReceiverWalletId == MFTCCardId
                                           && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate.Date
                                          && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                                           select new Models.MFTCCardTopUpHistoryViewModel()
                                           {
                                               FaxerCurrency = Common.Common.GetCountryCurrency(dbContext.KiiPayPersonalWalletInformation.Where(x=> x.Id == c.SenderWalletId).Select(x => x.CardUserCountry).FirstOrDefault()),
                                               ReceivingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                               TopUpAmount = c.FaxingAmount,
                                               TopUpDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                               TopUpTime = c.TransactionDate.ToString("HH:mm"),
                                               TransationDate = c.TransactionDate,
                                               TopUpReference = c.PaymentReference
                                           }).ToList();

            var MFTCCardTopUpByBusinessMerchant = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId
                                                   && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate.Date
                                                   && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                                                   select new Models.MFTCCardTopUpHistoryViewModel()
                                                   {
                                                       FaxerCurrency = Common.Common.GetCountryCurrency(c.KiiPayBusinessWalletInformation.Country),
                                                       ReceivingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                       TopUpAmount = c.PayingAmount,
                                                       TopUpDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                       TopUpTime = c.TransactionDate.ToString("HH:mm"),
                                                       TransationDate = c.TransactionDate,
                                                       TopUpReference = c.PaymentReference
                                                   }).ToList();

            var data = new List<Models.MFTCCardTopUpHistoryViewModel>();

            data = result.Concat(result2).Concat(MFTCCardTopUpByCardUser).Concat(MFTCCardTopUpByBusinessMerchant).OrderByDescending(x => x.TransationDate).ToList();


            return data;


        }



        public List<Models.MFTCCardCashwithDrawlHistoryViewModel> GetCashWithDrawlDetails(int MFTCardid)
        {

            var result = (from c in dbContext.UserCardWithdrawl.Where(x => x.KiiPayPersonalWalletInformationId == MFTCardid).OrderByDescending(x => x.TransactionDate).ToList()
                          select new Models.MFTCCardCashwithDrawlHistoryViewModel()
                          {
                              WithdrawlAmount = c.TransactionAmount,
                              withdrawlCurrency = Common.Common.GetCountryCurrency(c.AgentInformation.CountryCode),
                              AgentLocation = c.AgentInformation.Address1 + "," + c.AgentInformation.City + "," + Common.Common.GetCountryName(c.AgentInformation.CountryCode),
                              AgentName = c.AgentInformation.Name,
                              AgentMFCode = c.AgentInformation.AccountNo,
                              WithdrawlDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              WithdrawlTime = c.TransactionDate.ToString("HH:mm")
                          }).ToList();

            return result;
        }
        public List<Models.MFTCCardCashwithDrawlHistoryViewModel> GetCashWithDrawlDetailsFilterByDate(int MFTCardid, DateTime FromDate, DateTime ToDate)
        {

            var result = (from c in dbContext.UserCardWithdrawl.Where(x => x.KiiPayPersonalWalletInformationId == MFTCardid
                          && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate.Date
                          && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                          select new Models.MFTCCardCashwithDrawlHistoryViewModel()
                          {
                              WithdrawlAmount = c.TransactionAmount,
                              withdrawlCurrency = Common.Common.GetCountryCurrency(c.AgentInformation.CountryCode),
                              AgentLocation = c.AgentInformation.Address1 + "," + c.AgentInformation.City + "," + Common.Common.GetCountryName(c.AgentInformation.CountryCode),
                              AgentName = c.AgentInformation.Name,
                              AgentMFCode = c.AgentInformation.AccountNo,
                              WithdrawlDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                              WithdrawlTime = c.TransactionDate.ToString("HH:mm")
                          }).ToList();

            return result;
        }


        public List<Models.MFTCCardMerchantPaymentHistoryViewModel> GetMerchantPaymentDetials(int MFTCCardId)
        {


            var MerchantLocalPaymentByCarduser = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                                                  select new Models.MFTCCardMerchantPaymentHistoryViewModel()
                                                  {
                                                      FaxingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                      PaymentAmount = c.AmountSent,
                                                      BusinessLocation = c.KiiPayBusinessWalletInformation.AddressLine1 + "," + c.KiiPayBusinessWalletInformation.City + "," + Common.Common.GetCountryName(c.KiiPayBusinessWalletInformation.Country),
                                                      BusinessMerchantName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                                      BusinessMFCode = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                                                      PaymentDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                      PaymentTime = c.TransactionDate.ToString("HH:mm"),
                                                      TransactionDateTime = c.TransactionDate
                                                  }).ToList();

            var MerchantInternationalPaymentByCarduser = (from c in dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedFromKiiPayPersonalWalletId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                                                          select new Models.MFTCCardMerchantPaymentHistoryViewModel()
                                                          {
                                                              FaxingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                              PaymentAmount = c.TotalAmount,
                                                              BusinessLocation = c.KiiPayBusinessWalletInformation.AddressLine1 + "," + c.KiiPayBusinessWalletInformation.City + "," + Common.Common.GetCountryName(c.KiiPayBusinessWalletInformation.Country),
                                                              BusinessMerchantName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                                              BusinessMFCode = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                                                              PaymentDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                              PaymentTime = c.TransactionDate.ToString("HH:mm"),
                                                              TransactionDateTime = c.TransactionDate
                                                          }).ToList();

            var MFTCCardTopUpCardByCardUser = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderWalletId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                                               join d in dbContext.KiiPayPersonalWalletInformation on c.ReceiverWalletId equals d.Id
                                               select new Models.MFTCCardMerchantPaymentHistoryViewModel()
                                               {
                                                   FaxingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                   PaymentAmount = c.TotalAmount,
                                                   BusinessLocation = d.Address1 + "," + d.CardUserCity + "," + Common.Common.GetCountryName(d.CardUserCountry),
                                                   BusinessMerchantName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                                                   BusinessMFCode = d.MobileNo.Contains("MF") ? d.MobileNo : d.MobileNo.Decrypt(),
                                                   PaymentDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                   PaymentTime = c.TransactionDate.ToString("HH:mm"),
                                                   TransactionDateTime = c.TransactionDate
                                               }).ToList();
            var model = new List<Models.MFTCCardMerchantPaymentHistoryViewModel>();
            model = MerchantLocalPaymentByCarduser.Concat(MerchantInternationalPaymentByCarduser).Concat(MFTCCardTopUpCardByCardUser).OrderByDescending(x => x.TransactionDateTime).ToList();
            return model;
        }
        public List<Models.MFTCCardMerchantPaymentHistoryViewModel> GetMerchantPaymentDetialsFilterByDate(int MFTCCardId, DateTime FromDate, DateTime ToDate)
        {


            var MerchantLocalPaymentByCarduser = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId
                           && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate.Date
                          && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                                                  select new Models.MFTCCardMerchantPaymentHistoryViewModel()
                                                  {

                                                      FaxingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                      PaymentAmount = c.AmountSent,
                                                      BusinessLocation = c.KiiPayBusinessWalletInformation.AddressLine1 + "," + c.KiiPayBusinessWalletInformation.City + "," + Common.Common.GetCountryName(c.KiiPayBusinessWalletInformation.Country),
                                                      BusinessMerchantName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                                      BusinessMFCode = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                                                      PaymentDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                      PaymentTime = c.TransactionDate.ToString("HH:mm"),
                                                      TransactionDateTime = c.TransactionDate
                                                  }).ToList();

            var MerchantInternationalPaymentByCarduser = (from c in dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedFromKiiPayPersonalWalletId == MFTCCardId
                                                           && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate.Date
                                                           && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                                                          select new Models.MFTCCardMerchantPaymentHistoryViewModel()
                                                          {

                                                              FaxingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                              PaymentAmount = c.TotalAmount,
                                                              BusinessLocation = c.KiiPayBusinessWalletInformation.AddressLine1 + "," + c.KiiPayBusinessWalletInformation.City + "," + Common.Common.GetCountryName(c.KiiPayBusinessWalletInformation.Country),
                                                              BusinessMerchantName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                                              BusinessMFCode = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                                                              PaymentDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                              PaymentTime = c.TransactionDate.ToString("HH:mm"),
                                                              TransactionDateTime = c.TransactionDate
                                                          }).ToList();

            var MFTCCardTopUpCardByCardUser = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderWalletId == MFTCCardId
                                               && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate.Date
                                               && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                                               join d in dbContext.KiiPayPersonalWalletInformation on c.ReceiverWalletId equals d.Id
                                               select new Models.MFTCCardMerchantPaymentHistoryViewModel()
                                               {

                                                   FaxingCurrency = Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                   PaymentAmount = c.TotalAmount,
                                                   BusinessLocation = d.Address1 + "," + d.CardUserCity + "," + Common.Common.GetCountryName(d.CardUserCountry),
                                                   BusinessMerchantName = d.FirstName + " " + d.MiddleName + " " + d.LastName ,
                                                   BusinessMFCode = d.MobileNo.Decrypt(),
                                                   PaymentDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                   PaymentTime = c.TransactionDate.ToString("HH:mm"),
                                                   TransactionDateTime = c.TransactionDate
                                               }).ToList();
            var model = new List<Models.MFTCCardMerchantPaymentHistoryViewModel>();
            model = MerchantLocalPaymentByCarduser.Concat(MerchantInternationalPaymentByCarduser).Concat(MFTCCardTopUpCardByCardUser).OrderByDescending(x => x.TransactionDateTime).ToList();
            return model;
        }
    }
}