using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class MyMoneyFaxCardWithdrawlSheetServices
    {
        DB.FAXEREntities dbContext = null;
        public MyMoneyFaxCardWithdrawlSheetServices()
        {

            dbContext = new DB.FAXEREntities();
        }
        public List<ViewModels.MyMoneyFaxCardWithdrawlSheetViewModel> GetAllDetails()
        {

            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;

            var result = (from c in dbContext.UserCardWithdrawl.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId).OrderByDescending(x => x.TransactionDate).ToList()
                          join Currency in dbContext.Country on c.KiiPayPersonalWalletInformation.CardUserCountry equals Currency.CountryCode
                          select new ViewModels.MyMoneyFaxCardWithdrawlSheetViewModel()
                          {
                              TransactionId = c.Id,
                              NameOfAgency = c.AgentInformation.Name,
                              AgencyMFSCode = c.AgentInformation.AccountNo,
                              WithdrwalAmount = c.TransactionAmount.ToString() + " " + Currency.Currency,
                              Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                              Time = c.TransactionDate.ToString("HH:mm")
                          }).ToList();

            return result;
        }
        public List<ViewModels.MyMoneyFaxCardWithdrawlSheetViewModel> FilterAllDetails(DateTime fromDate, DateTime toDate)
        {

            int MFTCCardId = Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId;

            var result = (from c in dbContext.UserCardWithdrawl.Where(x => x.KiiPayPersonalWalletInformationId == MFTCCardId
                          && (DbFunctions.TruncateTime(x.TransactionDate) >= fromDate.Date
                          && DbFunctions.TruncateTime(x.TransactionDate) <= toDate.Date)
                          ).OrderByDescending(x => x.TransactionDate).ToList()
                          join Currency in dbContext.Country on c.KiiPayPersonalWalletInformation.CardUserCountry equals Currency.CountryCode
                          select new ViewModels.MyMoneyFaxCardWithdrawlSheetViewModel()
                          {
                              TransactionId = c.Id,
                              NameOfAgency = c.AgentInformation.Name,
                              AgencyMFSCode = c.AgentInformation.AccountNo,
                              WithdrwalAmount = c.TransactionAmount.ToString() + " " + Currency.Currency,
                              Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                              Time = c.TransactionDate.ToString("HH:mm")
                          }).ToList();

            return result;
        }

    }
}