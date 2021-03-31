using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class CreditDebitCardUsageServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices _CommonServices = null;
        public CreditDebitCardUsageServices()
        {
            dbContext = new DB.FAXEREntities();
            _CommonServices = new CommonServices();
        }

        public List<CreditDebitCardUsageViewModel> GetCreditDebitCardUsageLogList(string country)
        {
            List<CreditDebitCardUsageViewModel> vm = new List<CreditDebitCardUsageViewModel>();
            IQueryable<SecureTradingApiResponseTransactionLog> data = dbContext.SecureTradingApiResponseTransactionLog;
            IQueryable<Transact365ApiResponseTransationLog> T365data = dbContext.Transact365ApiResponseTransationLog;

            if (!string.IsNullOrEmpty(country))
            {
                data = data.Where(x => x.issuercountryiso2a == country);
                T365data = T365data.Where(x => x.BillingAddressCountry == country);
            }
            var SecureTradingresult = data.ToList().GroupBy(x => new
            {
                x.SenderId,
                x.maskedpan,
            }).Select(d => new CreditDebitCardUsageViewModel()
            {
                Id = d.FirstOrDefault().Id,
                CardNumber = d.FirstOrDefault().maskedpan,
                Amount = d.FirstOrDefault().baseamount,
                AuthCount = "",
                CardIssuingCountry = d.FirstOrDefault().issuercountryiso2a,
                CardIssuingCountryFlag = !string.IsNullOrEmpty(d.FirstOrDefault().issuercountryiso2a) == true ? d.FirstOrDefault().issuercountryiso2a.ToLower() : "",
                SenderId = d.FirstOrDefault().SenderId,
                OpStatus = d.FirstOrDefault().errormessage,
                DateTime = d.FirstOrDefault().transactionstartedtimestamp,
                SenderName = _CommonServices.GetSenderName(d.FirstOrDefault().SenderId),
            }).ToList();
            var T365result = T365data.ToList().GroupBy(x => new
            {
                x.SenderId,
                x.Last4DigitCardNum
            }).Select(d => new CreditDebitCardUsageViewModel()
            {
                Id = d.FirstOrDefault().Id,
                CardNumber = d.FirstOrDefault().First1DigitCardNum + "*****" + d.FirstOrDefault().Last4DigitCardNum,
                Amount = d.FirstOrDefault().Amount.ToString(),
                AuthCount = "",
                CardIssuingCountry = d.FirstOrDefault().IssuerCountry,
                CardIssuingCountryFlag = !string.IsNullOrEmpty(d.FirstOrDefault().IssuerCountry) == true ? d.FirstOrDefault().IssuerCountry.ToLower() : "",
                SenderId = d.FirstOrDefault().SenderId,
                OpStatus = d.FirstOrDefault().ThreeDMessage,
                DateTime = d.FirstOrDefault().CreatedAt,
                SenderName = _CommonServices.GetSenderName(d.FirstOrDefault().SenderId),
            }).ToList();
            var result = SecureTradingresult.Concat(T365result).ToList();
            return result;

        }

        public void BlockSender(int id)
        {
            var data = dbContext.FaxerLogin.Where(x => x.FaxerId == id).FirstOrDefault();
            data.IsActive = false;
            dbContext.Entry<FaxerLogin>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }
    }
}