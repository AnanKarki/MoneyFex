using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class VIewMerchantNonCardTransferReceivedServices
    {
        DB.FAXEREntities dbContext = null;
        public VIewMerchantNonCardTransferReceivedServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.VIewMerchantNonCardTransferReceivedViewModel> GetTransactionDetails(string CountryCode ,string City) {


            var data = dbContext.MerchantNonCardWithdrawal.ToList();

            if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                data = dbContext.MerchantNonCardWithdrawal.Where(x => x.ReceiversDetails.Business.BusinessOperationCountryCode.ToLower() == CountryCode.ToLower()
                                                                        && x.ReceiversDetails.Business.BusinessOperationCity.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode)) {


                data = dbContext.MerchantNonCardWithdrawal.Where(x => x.ReceiversDetails.Business.BusinessOperationCountryCode.ToLower() == CountryCode.ToLower()).ToList();

            }

            var result = (from c in data.OrderByDescending(x => x.TransactionDate)
                          select new ViewModels.VIewMerchantNonCardTransferReceivedViewModel()
                          {
                              SenderName = c.ReceiversDetails.Business.BusinessName,
                              SenderAddress = c.ReceiversDetails.Business.BusinessOperationAddress1,
                              SenderCity = c.ReceiversDetails.Business.BusinessOperationCity,
                              SenderCountry = Common.Common.GetCountryName(c.ReceiversDetails.Business.BusinessOperationCountryCode),
                              SenderEmail = c.ReceiversDetails.Business.Email,
                              SenderTel = Common.Common.GetCountryPhoneCode(c.ReceiversDetails.Business.BusinessOperationCountryCode) + " " + c.ReceiversDetails.PhoneNumber,
                              ReceiverName = c.ReceiversDetails.FirstName + " " + c.ReceiversDetails.MiddleName + " " + c.ReceiversDetails.LastName,
                              ReceiverAddress = c.ReceiversDetails.City,
                              ReceiverCity = c.ReceiversDetails.City,
                              ReceiverCountry = Common.Common.GetCountryName(c.ReceiversDetails.Country),
                              ReceiverEmail = c.ReceiversDetails.EmailAddress,
                              ReceiverTel = Common.Common.GetCountryPhoneCode(c.ReceiversDetails.Country) + " " + c.ReceiversDetails.PhoneNumber,
                              IDCardNO = c.IdNumber,
                              IDCardType = Common.Common.GetIDCardTypeName(c.IdentificationTypeId),
                              IDExpiryDate = c.IdCardExpiringDate.ToString("dd/MM/yyyy"),
                              IDIssuingCountry = Common.Common.GetCountryName(c.IssuingCountryCode),
                              MFCN = c.MFCN,
                              AmountSent = c.TransactionAmount,
                              AmountReceived = c.ReceivingAmount,
                              TransactionID = c.Id

                          }).ToList();
            return result;
        }


        public ViewModels.VIewMerchantNonCardTransferReceivedAdditonalInformatinViewModel GetAdditonalInformatinViewModel(int TransactionId) {


            var data = dbContext.MerchantNonCardWithdrawal.Where(x => x.Id == TransactionId).FirstOrDefault();
            ViewModels.VIewMerchantNonCardTransferReceivedAdditonalInformatinViewModel vm = new ViewModels.VIewMerchantNonCardTransferReceivedAdditonalInformatinViewModel() {

                PayingAgent = data.PayingAgentName,
                AgentMFCode = data.Agent.AccountNo,
                NameOfAgency = data.Agent.Name,
                StatusOFTransaction = "Received",
                MFCN = data.MFCN,
                TransactionId = data.Id
            };

            return vm;

        }
    }
    
}