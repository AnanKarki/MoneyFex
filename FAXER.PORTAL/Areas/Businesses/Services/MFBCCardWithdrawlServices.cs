using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class MFBCCardWithdrawlServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices CommonService = new CommonServices();
        public MFBCCardWithdrawlServices() {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.MFBCCardWithdrawlViewModel> GetMFBCCardWithdrawlDetails() {



            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;

            var MFBCCardId = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).Select(x => x.Id).FirstOrDefault();
            var result = (from c in dbContext.MFBCCardWithdrawls.Where(x => x.KiiPayBusinessWalletInformation.KiiPayBusinessInformationId == KiiPayBusinessInformationId).ToList().OrderByDescending(x => x.TransactionDate)
                         select new ViewModels.MFBCCardWithdrawlViewModel() {
                             PayinAgentName = c.AgentInformation.Name,
                             city = c.AgentInformation.City,
                             Date = Common.ConversionExtension.ToFormatedString(c.TransactionDate),
                             TIme = c.TransactionDate.ToString("HH:mm"),
                             withdrawlAmount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " "+ c.TransactionAmount.ToString() +" "+ CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),
                             AgentMFSCode = c.AgentInformation.AccountNo

                         }).ToList();
            return result;
        }
        public List<ViewModels.MFBCCardWithdrawlViewModel> GetMFBCCardWithdrawlDetailsByFilterDate(DateTime FromDate , DateTime ToDate)
        {



            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;

            var MFBCCardId = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).Select(x => x.Id).FirstOrDefault();
            var result = (from c in dbContext.MFBCCardWithdrawls.Where(x => x.KiiPayBusinessWalletInformation.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (DbFunctions.TruncateTime(x.TransactionDate) >= FromDate.Date) && DbFunctions.TruncateTime(x.TransactionDate) <= ToDate.Date).ToList().OrderByDescending(x => x.TransactionDate)
                          select new ViewModels.MFBCCardWithdrawlViewModel()
                          {
                              PayinAgentName = c.AgentInformation.Name,
                              city = c.AgentInformation.City,
                              Date = Common.ConversionExtension.ToFormatedString(c.TransactionDate),
                              TIme = c.TransactionDate.ToString("HH:mm"),
                              withdrawlAmount = CommonService.getBusinessCurrencySymbol(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId) + " " + c.TransactionAmount.ToString() + " "+ CommonService.getBusinessCurrency(Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId),
                              AgentMFSCode = c.AgentInformation.AccountNo

                          }).ToList();
            return result;
        }
    }
}