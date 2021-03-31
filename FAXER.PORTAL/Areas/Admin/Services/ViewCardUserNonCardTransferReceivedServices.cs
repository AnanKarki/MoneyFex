using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewCardUserNonCardTransferReceivedServices
    {
        DB.FAXEREntities dbContext = null;
        public ViewCardUserNonCardTransferReceivedServices()
        {
            dbContext = new DB.FAXEREntities();
        }


        public List<ViewModels.ViewCardUserNonCardTransferReceivedViewModel> GetTransactionDetails(string CountryCode, string City)
        {


            var data = dbContext.CardUserNonCardWithdrawal.ToList();

            if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {


                data = dbContext.CardUserNonCardWithdrawal.Where(x => x.ReceiversDetails.MFTCCardInformation.CardUserCountry.ToLower() == CountryCode.ToLower()
                                                                          && x.ReceiversDetails.MFTCCardInformation.CardUserCity.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode))
            {

                data = dbContext.CardUserNonCardWithdrawal.Where(x => x.ReceiversDetails.MFTCCardInformation.CardUserCountry.ToLower() == CountryCode.ToLower()).ToList();
            }

            var result = (from c in data.OrderByDescending(x => x.TransactionDate)
                          select new ViewModels.ViewCardUserNonCardTransferReceivedViewModel()
                          {
                              SenderName = c.ReceiversDetails.MFTCCardInformation.FirstName + " " + c.ReceiversDetails.MFTCCardInformation.MiddleName + " " + c.ReceiversDetails.MFTCCardInformation.LastName,
                              SenderAddress = c.ReceiversDetails.MFTCCardInformation.Address1,
                              SenderCity = c.ReceiversDetails.MFTCCardInformation.CardUserCity,
                              SenderCountry = Common.Common.GetCountryName(c.ReceiversDetails.MFTCCardInformation.CardUserCountry),
                              SenderEmail = c.ReceiversDetails.MFTCCardInformation.CardUserEmail,
                              SenderTel = c.ReceiversDetails.MFTCCardInformation.CardUserTel,
                              ReceiverName = c.ReceiversDetails.FirstName + " " + c.ReceiversDetails.MiddleName + " " + c.ReceiversDetails.LastName,
                              ReceiverTel = c.ReceiversDetails.PhoneNumber,
                              ReceiverCity = c.ReceiversDetails.City,
                              ReceiverCountry = Common.Common.GetCountryName(c.ReceiversDetails.Country),
                              ReceiverAddress = c.ReceiversDetails.City,
                              ReceiverEmail = c.ReceiversDetails.EmailAddress,
                              IDCardNO = c.IdNumber,
                              IDCardType = Common.Common.GetIDCardTypeName(c.IdentificationTypeId),
                              IDExpiryDate = c.IdCardExpiringDate.ToString("dd/MM/yyyy"),
                              IDIssuingCountry = Common.Common.GetCountryName(c.IssuingCountryCode),
                              AmountReceived = c.ReceivingAmount,
                              AmountSent = c.TransactionAmount,
                              MFCN = c.MFCN,
                              TransactionID = c.Id
                          }).ToList();
            return result;
        }

        public ViewModels.ViewCardUserNonCardTransferReceivedAdditonalInformatinViewModel GetAdditionalDetails(int TransactionId)
        {


            var data = dbContext.CardUserNonCardWithdrawal.Where(x => x.Id == TransactionId).FirstOrDefault();
            ViewModels.ViewCardUserNonCardTransferReceivedAdditonalInformatinViewModel vm = new ViewModels.ViewCardUserNonCardTransferReceivedAdditonalInformatinViewModel()
            {

                PayingAgent = data.PayingAgentName,
                NameOfAgency = data.Agent.Name,
                AgentMFCode = data.Agent.AccountNo,
                MFCN = data.MFCN,
                StatusOFTransaction = "Received",
                TransactionId = data.Id
            };

            return vm;
        }
    }
}