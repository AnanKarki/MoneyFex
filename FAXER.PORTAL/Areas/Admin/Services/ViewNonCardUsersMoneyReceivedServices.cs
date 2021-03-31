using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewNonCardUsersMoneyReceivedServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();
        public ViewNonCardUsersMoneyReceivedViewModel getNonCardUserMoneyTransactionList()
        {
            var result = new ViewNonCardUsersMoneyReceivedViewModel();
            result.viewNonCardUsersParties = (from c in dbContext.FaxingNonCardTransaction.ToList()
                                              select new ViewNonCardUsersPartiesViewModel()
                                              {
                                                  FaxerFirstName = c.NonCardReciever.FaxerInformation.FirstName,
                                                  FaxerMiddleName = c.NonCardReciever.FaxerInformation.MiddleName,
                                                  FaxerLastName = c.NonCardReciever.FaxerInformation.LastName,
                                                  FaxerAddress = c.NonCardReciever.FaxerInformation.Address1,
                                                  FaxerCountry = c.NonCardReciever.FaxerInformation.Country,
                                                  FaxerCity = c.NonCardReciever.FaxerInformation.City,
                                                  FaxerTelephone = c.NonCardReciever.FaxerInformation.PhoneNumber,
                                                  FaxerEmailAddress = c.NonCardReciever.FaxerInformation.Email,

                                                  AmountFaxed = c.FaxingAmount,
                                                  MFCN = c.MFCN,

                                                  ReceiverFirstName = c.NonCardReciever.FirstName,
                                                  ReceiverMiddleName = c.NonCardReciever.MiddleName,
                                                  ReceiverLastName = c.NonCardReciever.LastName,
                                                  //ReceiverAddress = c.NonCardReciever.a
                                                  ReceiverCountry = CommonService.getCountryNameFromCode(c.NonCardReciever.Country),
                                                  ReceiverCity = c.NonCardReciever.City,
                                                  ReceiverTelephone = c.NonCardReciever.PhoneNumber,
                                                  ReceiverEmailAddress = c.NonCardReciever.EmailAddress,
                                                  //ReceiverIDCardNumber = c.NonCardReciever.
                                                  //ReceiverIDCardType = c.NonCardReciever
                                                  //ReceiverIDCardExpDate = c.NonCardReciever
                                                  //ReceiverIDCardIssuingCountry = c.NonCardReciever
                                                  AmountReceived = c.ReceivingAmount
                                              }).ToList();
            result.viewNonCardUsersDetails = (from c in dbContext.ReceiverNonCardWithdrawl.ToList()
                                              join d in dbContext.FaxingNonCardTransaction on c.MFCN equals d.MFCN
                                              select new ViewNonCardUsersDetailsViewModel()
                                              {
                                                  PayingAgentVerifier = c.PayingAgentName,
                                                  PayingAgentName = c.Agent.Name,
                                                  PayingAgentMFSCode = c.Agent.AccountNo,
                                                  StatusOfTransaction = d.MFCN
                                                  //PaymentRejection = 
                                              }).ToList();

            return result;
        }
        // Filtered List by Country and City
        public ViewNonCardUsersMoneyReceivedViewModel getFilterNonCardUserMoneyTransactionList(string CountryCode, string City)
        {

            var data = new List<DB.ReceiverNonCardWithdrawl>();
            var data1 = new List<DB.FaxingNonCardTransaction>();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = (from c in dbContext.ReceiverNonCardWithdrawl
                        join d in dbContext.ReceiversDetails on c.ReceiverId equals d.Id
                        join e in dbContext.FaxerInformation on d.FaxerID equals e.Id
                        where e.Country == CountryCode
                        select c).ToList();
                data1 = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.Received)
                         join d in dbContext.ReceiversDetails on c.NonCardRecieverId equals d.Id
                         join e in dbContext.FaxerInformation on d.FaxerID equals e.Id
                         where e.Country == CountryCode
                         select c).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = (from c in dbContext.ReceiverNonCardWithdrawl
                        join d in dbContext.ReceiversDetails on c.ReceiverId equals d.Id
                        join e in dbContext.FaxerInformation on d.FaxerID equals e.Id
                        where e.City.ToLower() == City.ToLower()
                        select c).ToList();
                data1 = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.Received)
                         join d in dbContext.ReceiversDetails on c.NonCardRecieverId equals d.Id
                         join e in dbContext.FaxerInformation on d.FaxerID equals e.Id
                         where e.City.ToLower() == City.ToLower()
                         select c).ToList();
            }
            else
            {
                data = (from c in dbContext.ReceiverNonCardWithdrawl
                        join d in dbContext.ReceiversDetails on c.ReceiverId equals d.Id
                        join e in dbContext.FaxerInformation on d.FaxerID equals e.Id
                        where e.Country == CountryCode && e.City.ToLower() == City.ToLower()
                        select c).ToList();

                data1 = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.Received)
                         join d in dbContext.ReceiversDetails on c.NonCardRecieverId equals d.Id
                         join e in dbContext.FaxerInformation on d.FaxerID equals e.Id
                         where e.Country == CountryCode && e.City.ToLower() == City.ToLower()
                         select c).ToList();
            }

            var result = new ViewNonCardUsersMoneyReceivedViewModel();
            result.viewNonCardUsersParties = (from c in data1
                                              join d in dbContext.ReceiverNonCardWithdrawl on c.MFCN equals d.MFCN
                                              select new ViewNonCardUsersPartiesViewModel()
                                              {
                                                  Id = c.Id,
                                                  FaxerFirstName = c.NonCardReciever.FaxerInformation.FirstName,
                                                  FaxerMiddleName = c.NonCardReciever.FaxerInformation.MiddleName,
                                                  FaxerLastName = c.NonCardReciever.FaxerInformation.LastName,
                                                  FaxerAddress = c.NonCardReciever.FaxerInformation.Address1,
                                                  FaxerCountry = CommonService.getCountryNameFromCode(c.NonCardReciever.FaxerInformation.Country),
                                                  FaxerCity = c.NonCardReciever.FaxerInformation.City,
                                                  FaxerTelephone = CommonService.getPhoneCodeFromCountry(c.NonCardReciever.FaxerInformation.Country) + c.NonCardReciever.FaxerInformation.PhoneNumber,
                                                  FaxerEmailAddress = c.NonCardReciever.FaxerInformation.Email,
                                                  FaxerCurrency = CommonService.getCurrencyCodeFromCountry(c.NonCardReciever.FaxerInformation.Country),



                                                  AmountFaxed = c.FaxingAmount,
                                                  MFCN = c.MFCN,

                                                  ReceiverFirstName = c.NonCardReciever.FirstName,
                                                  ReceiverMiddleName = c.NonCardReciever.MiddleName,
                                                  ReceiverLastName = c.NonCardReciever.LastName,
                                                  ReceiverAddress = c.NonCardReciever.City +"," + CommonService.getCountryNameFromCode(c.NonCardReciever.Country),
                                                  ReceiverCountry = CommonService.getCountryNameFromCode(c.NonCardReciever.Country),
                                                  ReceiverCity = c.NonCardReciever.City,
                                                  ReceiverTelephone = CommonService.getPhoneCodeFromCountry(c.NonCardReciever.Country) + c.NonCardReciever.PhoneNumber,
                                                  ReceiverEmailAddress = c.NonCardReciever.EmailAddress,
                                                  ReceiverCurrency = CommonService.getCurrencyCodeFromCountry(c.NonCardReciever.Country),
                                                  ReceiverIDCardNumber = d.IdNumber,
                                                  ReceiverIDCardType = d.IdentificationTypeId == 1 ? "National ID" : d.IdentificationTypeId == 2 ? "Driving Licence" : d.IdentificationTypeId == 3 ? "Passport" : "",
                                                  ReceiverIDCardExpDate = d.IdCardExpiringDate.ToFormatedString(),
                                                  ReceiverIDCardIssuingCountry = CommonService.getCountryNameFromCode(d.IssuingCountryCode),
                                                  AmountReceived = c.ReceivingAmount,

                                                  //for receipt

                                              }).ToList();
            //result.viewNonCardUsersDetails = (from c in data
            //                                  join d in dbContext.FaxingNonCardTransaction on c.MFCN equals d.MFCN
            //                                  select new ViewNonCardUsersDetailsViewModel()
            //                                  {
            //                                      PayingAgentVerifier = c.PayingAgentName,
            //                                      PayingAgentName = c.Agent.Name,
            //                                      PayingAgentMFSCode = c.Agent.AccountNo,
            //                                      StatusOfTransaction = d.MFCN,
            //                                      //PaymentRejection = 
            //                                      ReceiptNo = d.ReceiptNumber,
            //                                      TransactionDate = d.TransactionDate.ToFormatedString(),
            //                                      TransactionTime = d.TransactionDate.ToString("HH:mm"),
            //                                      FaxerFullName = d.NonCardReciever.FaxerInformation.FirstName + d.NonCardReciever.FaxerInformation.MiddleName + d.NonCardReciever.FaxerInformation.LastName,
            //                                      ReceiverFullName = d.NonCardReciever.FirstName + d.NonCardReciever.MiddleName + d.NonCardReciever.LastName,
            //                                      ReceiversTelephone = d.NonCardReciever.PhoneNumber,
            //                                      ExchangeRate = d.ExchangeRate.ToString(),
            //                                      Fee = d.FaxingFee.ToString(),
            //                                      MFCN = d.MFCN,
            //                                      AmountSent = d.FaxingAmount.ToString(),
            //                                      AmountReceived = d.ReceivingAmount.ToString()
            //                                  }).ToList();

            return result;
        }

        public ViewNonCardUsersDetailsViewModel getMoreInfo(int id)
        {
            var result = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.Id == id).ToList()
                          join d in dbContext.ReceiverNonCardWithdrawl on c.MFCN equals d.MFCN
                          select new ViewNonCardUsersDetailsViewModel()
                          {
                              PayingAgentVerifier = d.PayingAgentName,
                              PayingAgentName = d.Agent.Name,
                              PayingAgentMFSCode = d.Agent.AccountNo,
                              StatusOfTransaction = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus),
                              ExchangeRate = c.ExchangeRate.ToString(),
                              Fee = c.FaxingFee.ToString(),
                              TransactionDate = d.TransactionDate.ToFormatedString(),
                              TransactionTime = d.TransactionDate.ToString("HH:mm"),
                              ReceiptNo = c.ReceiptNumber,
                              FaxerFullName = c.NonCardReciever.FaxerInformation.FirstName + c.NonCardReciever.FaxerInformation.MiddleName + c.NonCardReciever.FaxerInformation.LastName,
                              ReceiverFullName = c.NonCardReciever.FirstName + c.NonCardReciever.MiddleName + c.NonCardReciever.LastName,
                              MFCN = c.MFCN,
                              ReceiversTelephone = c.NonCardReciever.PhoneNumber,
                              AmountSent = c.FaxingAmount.ToString(),
                              AmountReceived = c.ReceivingAmount.ToString(),
                              SendingCurrency = CommonService.getFaxerCurrencyFromId(c.NonCardReciever.FaxerID),
                              ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(c.NonCardReciever.Country)
                          }).FirstOrDefault();
            return result;
        }
    }
}