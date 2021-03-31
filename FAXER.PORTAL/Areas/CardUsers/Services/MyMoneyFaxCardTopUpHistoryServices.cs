using FAXER.PORTAL.Areas.CardUsers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class MyMoneyFaxCardTopUpHistoryServices
    {
        DB.FAXEREntities dbContext = null;

        public MyMoneyFaxCardTopUpHistoryServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.MyMoneyFaxCardTopUpHistoryViewModel> GetAllTopUpCardHistoryDetails()
        {

            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            var result = (from c in dbContext.FaxingCardTransaction.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                          join Faxer in dbContext.FaxerInformation on c.FaxerId equals Faxer.Id
                          select new ViewModels.MyMoneyFaxCardTopUpHistoryViewModel()
                          {
                              TransactionId = c.Id,
                              FaxerName = Faxer.FirstName + " " + Faxer.MiddleName + " " + Faxer.LastName,
                              FaxerCity = Faxer.City,
                              FaxerCountry =  Common.Common.GetCountryName(Faxer.Country),
                              TopUpAmount = c.RecievingAmount.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry) ,
                              Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                              Time = c.TransactionDate.ToString("HH:mm"),
                              TopUpReference = "Registrar",
                              TransactionDate = c.TransactionDate,
                              TopUpBy = TopUpBy.Registar
                          }).GroupBy(x => x.TransactionId).Select(x => x.FirstOrDefault()).ToList();
            var result2 = (from c in dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.KiiPayPersonalWalletId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                           join Faxer in dbContext.FaxerInformation on c.FaxerId equals Faxer.Id
                           select new ViewModels.MyMoneyFaxCardTopUpHistoryViewModel()
                           {
                               FaxerName = Faxer.FirstName + " " + Faxer.MiddleName + " " + Faxer.LastName,
                               FaxerCity = Faxer.City,
                               FaxerCountry = Common.Common.GetCountryName(Faxer.Country),
                               TopUpAmount = c.RecievingAmount.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                               Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                               Time = c.TransactionDate.ToString("HH:mm"),
                               TopUpReference = c.TopUpReference,
                               TransactionDate = c.TransactionDate,
                               TopUpBy = TopUpBy.Sender
                           }).ToList();


            var MFTCCardTopByCardUser = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.ReceiverWalletId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                                         join sender in dbContext.KiiPayPersonalWalletInformation on c.SenderWalletId equals sender.Id
                                         select new ViewModels.MyMoneyFaxCardTopUpHistoryViewModel()
                                         {
                                             FaxerName = sender.FirstName + " " + sender.MiddleName + " " + sender.LastName,
                                             FaxerCity = sender.CardUserCity,
                                             FaxerCountry = Common.Common.GetCountryName(sender.CardUserCountry),
                                             TopUpAmount = c.RecievingAmount.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                             Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                             Time = c.TransactionDate.ToString("HH:mm"),
                                             TopUpReference = c.PaymentReference,
                                             TransactionDate = c.TransactionDate,
                                             TopUpBy = TopUpBy.CardUser
                                         }).ToList();


            var MFTCCardTopUPByMerchant = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                                           join sender in dbContext.KiiPayBusinessWalletInformation on c.KiiPayBusinessWalletInformationId equals sender.Id
                                           select new ViewModels.MyMoneyFaxCardTopUpHistoryViewModel()
                                           {
                                               FaxerName = sender.KiiPayBusinessInformation.BusinessName,
                                               FaxerCity = sender.City,
                                               FaxerCountry = Common.Common.GetCountryName(sender.Country),
                                               TopUpAmount = c.RecievingAmount.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                               Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                               Time = c.TransactionDate.ToString("HH:mm"),
                                               TopUpReference = c.PaymentReference,
                                               TransactionDate = c.TransactionDate,
                                               TopUpBy = TopUpBy.Service_provider
                                           }).ToList();

            var model = new List<ViewModels.MyMoneyFaxCardTopUpHistoryViewModel>();
            model = result.Concat(result2).Concat(MFTCCardTopByCardUser).Concat(MFTCCardTopUPByMerchant).OrderByDescending(x => x.TransactionDate).ToList();
            return model;

        }
        public List<ViewModels.MyMoneyFaxCardTopUpHistoryViewModel> FilterTopUpCardHistoryDetails(DateTime fromDate, DateTime toDate)
        {

            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;
            var result = (from c in dbContext.FaxingCardTransaction.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId
                          && (DbFunctions.TruncateTime(x.TransactionDate) >= fromDate.Date && DbFunctions.TruncateTime(x.TransactionDate) <= toDate.Date)
                          ).OrderByDescending(x => x.TransactionDate).ToList()
                          join Faxer in dbContext.FaxerInformation on c.FaxerId equals Faxer.Id
                          select new ViewModels.MyMoneyFaxCardTopUpHistoryViewModel()
                          {
                              FaxerName = Faxer.FirstName + " " + Faxer.MiddleName + " " + Faxer.LastName,
                              FaxerCity = Faxer.City,
                              FaxerCountry = Common.Common.GetCountryName(Faxer.Country),
                              TopUpAmount = c.RecievingAmount.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                              Time = c.TransactionDate.ToString("HH:mm"),
                              TopUpReference = "Registrar",
                              TransactionDate = c.TransactionDate,
                              TopUpBy = TopUpBy.Registar
                          }).ToList();

            var result2 = (from c in dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.KiiPayPersonalWalletId == MFTCCardId
                          && (DbFunctions.TruncateTime(x.TransactionDate) >= fromDate.Date && DbFunctions.TruncateTime(x.TransactionDate) <= toDate.Date)
                          ).OrderByDescending(x => x.TransactionDate).ToList()
                           join Faxer in dbContext.FaxerInformation on c.FaxerId equals Faxer.Id
                           select new ViewModels.MyMoneyFaxCardTopUpHistoryViewModel()
                           {
                               FaxerName = Faxer.FirstName + " " + Faxer.MiddleName + " " + Faxer.LastName,
                               FaxerCity = Faxer.City,
                               FaxerCountry =  Common.Common.GetCountryName( Faxer.Country),
                               TopUpAmount = c.RecievingAmount.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                               Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                               Time = c.TransactionDate.ToString("HH:mm"),
                               TopUpReference = c.TopUpReference,
                               TransactionDate = c.TransactionDate,
                               TopUpBy = TopUpBy.Sender
                           }).ToList();


            var MFTCCardTopByCardUser = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.ReceiverWalletId == MFTCCardId
                                         && (DbFunctions.TruncateTime(x.TransactionDate) >= fromDate.Date && DbFunctions.TruncateTime(x.TransactionDate) <= toDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                                         join sender in dbContext.KiiPayPersonalWalletInformation on c.SenderWalletId equals sender.Id
                                         select new ViewModels.MyMoneyFaxCardTopUpHistoryViewModel()
                                         {
                                             FaxerName = sender.FirstName + " " + sender.MiddleName + " " + sender.LastName,
                                             FaxerCity = sender.CardUserCity,
                                             FaxerCountry = Common.Common.GetCountryName(sender.CardUserCountry),
                                             TopUpAmount = c.RecievingAmount.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                             Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                             Time = c.TransactionDate.ToString("HH:mm"),
                                             TopUpReference = c.PaymentReference,
                                             TransactionDate = c.TransactionDate,
                                             TopUpBy = TopUpBy.CardUser
                                         }).ToList();


            var MFTCCardTopUPByMerchant = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId
                                            && (DbFunctions.TruncateTime(x.TransactionDate) >= fromDate.Date && DbFunctions.TruncateTime(x.TransactionDate) <= toDate.Date)).OrderByDescending(x => x.TransactionDate).ToList()
                                           join sender in dbContext.KiiPayBusinessWalletInformation on c.KiiPayBusinessWalletInformationId equals sender.Id
                                           select new ViewModels.MyMoneyFaxCardTopUpHistoryViewModel()
                                           {
                                               FaxerName = sender.KiiPayBusinessInformation.BusinessName,
                                               FaxerCity = sender.City,
                                               FaxerCountry = Common.Common.GetCountryName(sender.Country),
                                               TopUpAmount = c.RecievingAmount.ToString() + " " + Common.Common.GetCountryCurrency(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                               Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                               Time = c.TransactionDate.ToString("HH:mm"),
                                               TopUpReference = c.PaymentReference,
                                               TransactionDate = c.TransactionDate,
                                               TopUpBy = TopUpBy.Service_provider
                                           }).ToList();
            var model = new List<MyMoneyFaxCardTopUpHistoryViewModel>();

            model = result.Concat(result2).Concat(MFTCCardTopByCardUser).Concat(MFTCCardTopUPByMerchant).OrderByDescending(x => x.TransactionDate).ToList();

            return model;

        }

    }
}