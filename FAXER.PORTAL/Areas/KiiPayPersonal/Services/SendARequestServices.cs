using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class SendARequestServices
    {
        FAXEREntities dbContext = null;
        KiiPayPersonalCommonServices _commonServices = null;
        public SendARequestServices()
        {
            _commonServices = new KiiPayPersonalCommonServices();
            dbContext = new FAXEREntities();
        }

        public List<DropDownViewModel> getRecentPhoneNumbersLocal()
        {
            var kiiPayWalletId = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault().KiiPayPersonalWalletInformation.Id;
            var result = (from c in dbContext.KiiPayPersonalRequestForPayment.Where(x => x.RequestSenderId == kiiPayWalletId && x.RequestType == PaymentType.Local)
                          join d in dbContext.KiiPayPersonalWalletInformation on c.RequestReceiverId equals d.Id
                          select new DropDownViewModel()
                          {
                              Id = d.Id,
                              Code = d.MobileNo,
                              Name = d.MobileNo
                          }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault()).ToList();
            return result;
        }

        public List<DropDownViewModel> getRecentPhoneNumbersInternational()
        {
            var kiiPayWalletId = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault().KiiPayPersonalWalletInformation.Id;
            var result = (from c in dbContext.KiiPayPersonalRequestForPayment.Where(x => x.RequestSenderId == kiiPayWalletId && x.RequestType == PaymentType.International)
                          join d in dbContext.KiiPayPersonalWalletInformation on c.RequestReceiverId equals d.Id
                          select new DropDownViewModel()
                          {
                              Id = d.Id,
                              Code = d.MobileNo,
                              Name = d.MobileNo
                          }).GroupBy(x => x.Code).Select(x => x.FirstOrDefault()).ToList();
            return result;
        }


        public bool IsPhoneValidForLocalPaymentRequest(string mobileNumber)
        {
            if (!string.IsNullOrEmpty(mobileNumber))
            {
                if (mobileNumber.Trim() != Common.CardUserSession.LoggedCardUserViewModel.MobileNumber.Trim())
                {
                    var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == mobileNumber.Trim() && x.CardUserCountry.Trim() == Common.CardUserSession.LoggedCardUserViewModel.CountryCode.Trim() && x.CardStatus == CardStatus.Active).FirstOrDefault();
                    if (data != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool IsPhoneValidForInternationalPaymentRequest(string mobileNumber, string countryCode)
        {
            if (!string.IsNullOrEmpty(mobileNumber) && !string.IsNullOrEmpty(countryCode))
            {
                if (countryCode.Trim() != Common.CardUserSession.LoggedCardUserViewModel.CountryCode.Trim())
                {
                    var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == mobileNumber.Trim() && x.CardUserCountry.Trim() != Common.CardUserSession.LoggedCardUserViewModel.CountryCode.Trim() && x.CardStatus == CardStatus.Active).FirstOrDefault();
                    if (data != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }



        public SendRequestEnterAmountViewModel getRequestEnterAmountLocalVM()
        {
            var userInfo = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.RequestAPaymentSession.ReceivingMobileNumber.Trim()).FirstOrDefault();
            var result = new SendRequestEnterAmountViewModel()
            {
                Id = 0,
                PhotoUrl = userInfo.KiiPayPersonalWalletInformation.UserPhoto,
                ReceiverName = userInfo.FirstName + " " + userInfo.MiddleName + " " + userInfo.Lastname,
                CurrencySymbol = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol,
                CurrencyCode = Common.Common.GetCountryCurrency(userInfo.Country)
            };
            return result;
        }

        public bool requestForPayment(PaymentType requestType)
        {
            if (Common.CardUserSession.RequestAPaymentSession != null)
            {
                var senderWalletData = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault().KiiPayPersonalWalletInformation;
                var receiverWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.RequestAPaymentSession.ReceivingMobileNumber.Trim()).FirstOrDefault();
                if (senderWalletData != null && receiverWalletData != null)
                {
                    DB.KiiPayPersonalRequestForPayment data = new KiiPayPersonalRequestForPayment()
                    {
                        RequestSenderId = senderWalletData.Id,
                        RequestReceiverId = receiverWalletData.Id,
                        RequestSendingCountry = senderWalletData.CardUserCountry,
                        RequestReceivingCountry = receiverWalletData.CardUserCountry,
                        RequestSendingAmount = Common.CardUserSession.RequestAPaymentSession.SendingAmount,
                        RequestReceivingAmount = Common.CardUserSession.RequestAPaymentSession.ReceivingAmount,
                        ExchangeRate = Common.CardUserSession.RequestAPaymentSession.ExchangeRate,
                        Fee = Common.CardUserSession.RequestAPaymentSession.Fee,
                        TotalAmount = Common.CardUserSession.RequestAPaymentSession.PayingAmount,
                        RequestNote = Common.CardUserSession.RequestAPaymentSession.Note,
                        RequestType = requestType,
                        IsPaid = false,
                        Status = RequestPaymentStatus.UnPaid,
                        RequestedDate = DateTime.Now
                    };
                    dbContext.KiiPayPersonalRequestForPayment.Add(data);
                    dbContext.SaveChanges();
                    return true;

                }
            }
            return false;
        }

        public SendRequesEnterAmountAbroadViewModel getRequestEnterAmountInternationalVM()
        {
            if (Common.CardUserSession.RequestAPaymentSession != null)
            {
                var requestSessionData = Common.CardUserSession.RequestAPaymentSession;
                var requestedToWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == requestSessionData.ReceivingMobileNumber).FirstOrDefault();
                var requestedFromWalletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.LoggedCardUserViewModel.MobileNumber.Trim()).FirstOrDefault();
                if (requestedFromWalletData != null && requestedToWalletData != null)
                {


                    var result = new SendRequesEnterAmountAbroadViewModel()
                    {
                        PhotoUrl = requestedToWalletData.UserPhoto,
                        ReceiversName = requestedToWalletData.FirstName + " " + requestedToWalletData.MiddleName + " " + requestedToWalletData.LastName,
                        ExchangeRate = _commonServices.calculateExchangeRate(requestedToWalletData.CardUserCountry, Common.CardUserSession.LoggedCardUserViewModel.CountryCode),
                        LocalCurrency = Common.Common.GetCountryCurrency(Common.CardUserSession.LoggedCardUserViewModel.CountryCode),
                        ForeignCurrency = Common.Common.GetCountryCurrency(requestedToWalletData.CardUserCountry),
                        LocalCurrencySymbol = Common.Common.GetCurrencySymbol(Common.CardUserSession.LoggedCardUserViewModel.CountryCode),
                        ForeignCurrencySymbol = Common.Common.GetCurrencySymbol(requestedToWalletData.CardUserCountry),
                        Note = ""
                    };
                    return result;
                }
            }
            return null;
        }
    }
}