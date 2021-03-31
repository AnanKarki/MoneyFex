using FAXER.PORTAL.DB;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Common;
namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewMFTCCardUsageServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();


        public ViewModels.ViewMFTCCardUsageViewModel getMFTCCardUsageList(string CountryCode = "", string City = "")
        {
            var data = new List<DB.KiiPayPersonalWalletWithdrawalFromAgent>();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = dbContext.UserCardWithdrawl.Where(x => x.KiiPayPersonalWalletInformation.CardUserCountry == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.UserCardWithdrawl.Where(x => x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = dbContext.UserCardWithdrawl.Where(x => (x.KiiPayPersonalWalletInformation.CardUserCity.ToLower() == City.ToLower()) && (x.KiiPayPersonalWalletInformation.CardUserCountry == CountryCode)).ToList();
            }
           



            var result = new ViewMFTCCardUsageViewModel();

            result.ViewMFTCCardWithdrawals = (from c in data.OrderByDescending(x => x.TransactionDate)
                                              join d in dbContext.SenderKiiPayPersonalAccount on c.KiiPayPersonalWalletInformationId equals d.KiiPayPersonalWalletId into SenderKiiPay
                                              from d in SenderKiiPay.DefaultIfEmpty()
                                              select new ViewMFTCCardWithdrawalViewModel()
                                              {
                                                  FaxerFirstName = d == null ? "" : d.SenderInformation.FirstName,
                                                  FaxerMiddleName = d.SenderInformation.MiddleName,
                                                  FaxerLastName = d.SenderInformation.LastName,
                                                  FaxerFullName = d.SenderInformation.FirstName + d.SenderInformation.MiddleName + d.SenderInformation.LastName,

                                                  CardUserFirstName = c.KiiPayPersonalWalletInformation.FirstName,
                                                  CardUserMiddleName = c.KiiPayPersonalWalletInformation.MiddleName,
                                                  CardUserLastName = c.KiiPayPersonalWalletInformation.LastName,
                                                  CardUserFullName = c.KiiPayPersonalWalletInformation.FirstName + c.KiiPayPersonalWalletInformation.MiddleName + c.KiiPayPersonalWalletInformation.LastName,
                                                  CardUserTelephone = c.KiiPayPersonalWalletInformation.CardUserTel,


                                                  ReceiverIDType = c.IdentificationType,
                                                  ReceiverIDNumber = c.IdNumber,
                                                  ReceiverIDExpDate = c.IdCardExpiringDate.ToString("dd-MM-yyyy"),
                                                  ReceiverIDIssuingCountry = CommonService.getCountryNameFromCode(c.IssuingCountryCode),
                                                  Currency = CommonService.getCurrencyCodeFromCountry(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                  MFTCCardNumber = c.KiiPayPersonalWalletInformation.MobileNo.Decrypt().FormatMFTCCard(),
                                                  AmountOnMFTCCard = c.KiiPayPersonalWalletInformation.CurrentBalance,
                                                  WithdrawalAmount = c.TransactionAmount,
                                                  WithdrawalDate = c.TransactionDate.ToString("dd-MM-yyyy"),
                                                  WithdrawalTime = c.TransactionDate.ToString("00:00"),

                                                  AgentVerifier = c.PayingAgentName,
                                                  AgentName = c.AgentInformation.Name,
                                                  AgentMFSCode = c.AgentInformation.AccountNo,
                                                  ReceiptNumber = c.ReceiptNumber
                                                  //PaymentRejection = c.
                                              }
                                              ).ToList();

            return result;
        }



    }
}