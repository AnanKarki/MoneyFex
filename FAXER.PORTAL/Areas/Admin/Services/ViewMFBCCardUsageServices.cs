using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMFBCCardUsageServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public List<ViewMFBCCardUsageViewModel> getMFBCCardWithdrawalList(string CountryCode = "", string City = "")
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
                          select new ViewMFBCCardUsageViewModel()
                          {
                              Id = c.Id,
                              BusinessName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                              BusinessMobileNo = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              CardUserFullName = c.KiiPayBusinessWalletInformation.FirstName + " " + c.KiiPayBusinessWalletInformation.MiddleName + " " + c.KiiPayBusinessWalletInformation.LastName,
                              CardUserPhotoIDType = d.CardType,
                              CardUserIDNumber = c.IdNumber,
                              CardUserPhotoIDExpDate = c.IdCardExpiringDate.ToString("dd/MM/yyyy"),
                              CardUserPhotoIDIssuingCountry = CommonService.getCountryNameFromCode(c.IssuingCountryCode),
                              CardUserCountry = CommonService.getCountryNameFromCode(c.KiiPayBusinessWalletInformation.Country),
                              CardUserCity = c.KiiPayBusinessWalletInformation.City,
                              MFBCCardNumber =  c.KiiPayBusinessWalletInformation.MobileNo.Decrypt().FormatMFBCCard(),
                              CreditOnMFBCCard = c.KiiPayBusinessWalletInformation.CurrentBalance,
                              UserCurrency = CommonService.getCurrencyCodeFromCountry(c.KiiPayBusinessWalletInformation.Country),
                              MoneyWDAmount = c.TransactionAmount,
                              MoneyWDTime = c.TransactionDate.ToString("HH:mm"), //TimeOfDay.ToFormatedStringTime(), 
                              MoneyWDDate = c.TransactionDate.Date.ToFormatedString(), 
                              PayingAgentVerifier = c.PayingAgentName,
                              PayingAgentName = c.AgentInformation.Name,
                              PayingAgentMFSCode = c.AgentInformation.AccountNo,
                              PayingAgentCity = c.AgentInformation.City
                          }).ToList();
            return result;
        }

        public MFBCWithdrawalUsageReceiptViewModel MFBCWithdrawalReceiptData (int id)
        {
            var result = (from c in dbContext.MFBCCardWithdrawls.Where(x => x.Id == id).ToList()
                          select new MFBCWithdrawalUsageReceiptViewModel()
                          {
                              Id = c.Id,
                              MFReceiptNumber = c.ReceiptNumber,
                              TransactionDate = c.TransactionDate.ToFormatedString(),
                              TransactionTime = c.TransactionDate.ToString("HH:mm"),
                              BusinessMerchantName = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                              MFBCCardNumber = c.KiiPayBusinessWalletInformation.MobileNo.Decrypt().FormatMFBCCard(),
                              BusinessCardUserFullName = c.KiiPayBusinessWalletInformation.FirstName + c.KiiPayBusinessWalletInformation.MiddleName + c.KiiPayBusinessWalletInformation.LastName,
                              Telephone = c.KiiPayBusinessWalletInformation.PhoneNumber,
                              AgentName = c.AgentInformation.Name,
                              AgentCode = c.AgentInformation.AccountNo,
                              AmountRequested = Math.Round(c.TransactionAmount, 2).ToString(),
                              ExchangeRate = "1",
                              AmountWithdrawn = Math.Round(c.TransactionAmount, 2).ToString(),
                              Currency = CommonService.getCurrencyCodeFromCountry(c.KiiPayBusinessWalletInformation.Country),

                          }).FirstOrDefault();
            return result;
        }
    }
}