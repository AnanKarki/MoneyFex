using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewAgentMFBCPaymentsServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public List<ViewAgentMFBCPaymentsViewModel> getAgentMFBCPaymentsList(string CountryCode = "", string City = "")
        {
            var data = new List<DB.KiiPayBusinessWalletWithdrawlFromAgent>();

            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.MFBCCardWithdrawls.Where(x=>x.KiiPayBusinessWalletInformation.Country == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.MFBCCardWithdrawls.Where(x => x.KiiPayBusinessWalletInformation.City.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.MFBCCardWithdrawls.Where(x => (x.KiiPayBusinessWalletInformation.City.ToLower() == City.ToLower()) && (x.KiiPayBusinessWalletInformation.Country == CountryCode)).ToList();
            }

            var result = (from c in data.OrderByDescending(x=>x.TransactionDate)
                          join d in dbContext.IdentityCardType on c.IdentificationTypeId equals d.Id into joinedT
                          from d in joinedT.DefaultIfEmpty()
                          select new ViewAgentMFBCPaymentsViewModel()
                          {
                              BusinessName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                              BusinessMobileNo = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              CardUserFullName = c.KiiPayBusinessWalletInformation.FirstName + " " + c.KiiPayBusinessWalletInformation.MiddleName + " " + c.KiiPayBusinessWalletInformation.LastName,
                              CardUserPhotoIDType = d.CardType,
                              CardUserPhotoIDNumber = c.IdNumber,
                              CardUserPhotoIDExpDate = c.IdCardExpiringDate.ToFormatedString(),
                              CardUserPhotoIDIssuingCountry = CommonService.getCountryNameFromCode(c.IssuingCountryCode),
                              MFBCNumber = c.KiiPayBusinessWalletInformation.MobileNo.Decrypt().FormatMFBCCard(),
                              TotalCreditOnMFBC = c.KiiPayBusinessWalletInformation.CurrentBalance,
                              MoneyWDAmount = c.TransactionAmount,
                              Currency = CommonService.getCurrencyCodeFromCountry(c.KiiPayBusinessWalletInformation.Country),
                              MoneyWDTime = c.TransactionDate.ToString("HH:mm"),
                              MoneyWDDate = c.TransactionDate.ToFormatedString(),
                              AgentVerifier = c.PayingAgentName,
                              AgentName = c.AgentInformation.Name,
                              AgentMFSCode = c.AgentInformation.AccountNo,
                              BalanceOnBusinessCard = c.KiiPayBusinessWalletInformation.CurrentBalance,
                              ReceiptNo = c.ReceiptNumber,
                              PhoneNo = c.KiiPayBusinessWalletInformation.PhoneNumber
                          }).ToList();
            return result;
        }
    }
}