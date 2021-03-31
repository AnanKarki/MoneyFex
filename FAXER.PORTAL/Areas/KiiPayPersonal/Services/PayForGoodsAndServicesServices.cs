using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class PayForGoodsAndServicesServices
    {
        FAXEREntities dbContext = null;
        public PayForGoodsAndServicesServices()
        {
            dbContext = new FAXEREntities();
        }


        public List<DropDownViewModel> GetRecentlyPaidBusinessPhoneNumbers()
        {
            var result = (from c in dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayPersonalWalletInformationId == Common.CardUserSession.LoggedCardUserViewModel.id).ToList()
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                              Code = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo
                          }).GroupBy(x=>x.Code).Select(x=>x.FirstOrDefault()).ToList();
            return result;
        }

        public List<DropDownViewModel> GetRecentPaidInternationalBusinessPhoneNumbers()
        {
            var kiiPayPersonalUserInfo = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault();
            if (kiiPayPersonalUserInfo != null)
            {
                int kiiPayPersonalWalletId = kiiPayPersonalUserInfo.KiiPayPersonalWalletInformationId;
                var result = (from c in dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedFromKiiPayPersonalWalletId == kiiPayPersonalWalletId).ToList()
                              select new DropDownViewModel()
                              {
                                  Id = c.Id,
                                  Name = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                                  Code = c.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo
                              }).GroupBy(x=>x.Code).Select(x=>x.FirstOrDefault()).ToList();
                return result;
            }
            return null;

        }

        public bool isPhoneNumberValid(string phoneNumber)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var data = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo.Trim() == phoneNumber.Trim() && x.BusinessCountry == Common.CardUserSession.LoggedCardUserViewModel.CountryCode).FirstOrDefault();
                if (data != null)
                {
                    var businessWalletData = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == data.Id && x.CardStatus == CardStatus.Active).FirstOrDefault();
                    if (businessWalletData != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool isInternationalPhoneValid(string phoneNumber, string countryCode)
        {
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                var data = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo.Trim() == phoneNumber.Trim() && x.BusinessCountry != Common.CardUserSession.LoggedCardUserViewModel.CountryCode).FirstOrDefault();
                if (data != null)
                {
                    if(data.BusinessCountry.Trim() == countryCode)
                    {
                        var businessWalletData = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == data.Id && x.CardStatus == CardStatus.Active).FirstOrDefault();
                        if (businessWalletData != null)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public string getBusinessReceiverName(string mobileNumber)
        {
            if (!string.IsNullOrEmpty(mobileNumber))
            {
                var data = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo.Trim() == mobileNumber.Trim()).FirstOrDefault();
                if (data != null)
                {
                    return data.BusinessName;
                }
            }
            return "";
        }

        public PayForServicesEnterAmountViewModel getPayForServicesEnterAmountViewModel()
        {
            var receiverData = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo.Trim() == Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingPhoneNumber.Trim()).FirstOrDefault();
            if (receiverData != null)
            {
                var result = new PayForServicesEnterAmountViewModel()
                {
                    PhotoUrl = "",
                    BusinessName = receiverData.BusinessName,
                    CurrencySymbol = Common.Common.GetCurrencySymbol(receiverData.BusinessCountry),
                    CurrencyCode = Common.Common.GetCountryCurrency(receiverData.BusinessCountry),
                    Amount = 0,
                    PaymentReference = "",
                    SendSMSNotification = false
                };
                return result;
            }
            return null;
        }

        public PayForServicesEnterAmountAbroadViewModel getEnterAmountAbroadViewModel()
        {
            var result = new PayForServicesEnterAmountAbroadViewModel()
            {
                Id = 0,
                PhotoUrl = "",
                ReceiversName = Common.CardUserSession.PayIntoAnotherWalletSession.ReceiversName,
                SendingAmount = 0,
                ReceivingAmount = 0,
                SendingCurrencyCode = Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencyCode,
                ReceivingCurrencyCode = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencyCode,
                SendingCurrencySymbol = Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencySymbol,
                ReceivingCurrencySymbol = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencySymbol,
                Fee = 0,
                PayingAmount = 0,
                ExchangeRate = Common.CardUserSession.PayIntoAnotherWalletSession.ExchangeRate,
                PaymentReference = Common.CardUserSession.PayIntoAnotherWalletSession.PaymentReference
            };
            return result;
        }

        public PayForServicesSummaryAbroadViewModel getAbroadPaymentSummaryViewModel()
        {
            var result = new PayForServicesSummaryAbroadViewModel()
            {
                Id = 0,
                Amount = Common.CardUserSession.PayIntoAnotherWalletSession.Amount,
                Fee = Common.CardUserSession.PayIntoAnotherWalletSession.Fee,
                PayingAmount = Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount,
                ReceivingAmount = Common.CardUserSession.PayIntoAnotherWalletSession.PayingAmount,
                PaymentReference = Common.CardUserSession.PayIntoAnotherWalletSession.PaymentReference,
                ReceiverName = Common.CardUserSession.PayIntoAnotherWalletSession.ReceiversName,
                SendersCurrencySymbol = Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencySymbol,
                ReceivingCurrencySymbol = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencySymbol,
                SenderCurrencyCode = Common.CardUserSession.PayIntoAnotherWalletSession.SendingCurrencyCode,
                ReceivingCurrencyCode = Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingCurrencyCode
            };
            return result;
        }

        public DB.KiiPayPersonalNationalKiiPayBusinessPayment SaveKiiPayPersonalNationalKiiPayBusinessPayment(DB.KiiPayPersonalNationalKiiPayBusinessPayment model)
        {
            dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Add(model);
            dbContext.SaveChanges();
            return model;


        }

        public DB.KiiPayPersonalInternationalKiiPayBusinessPayment SaveKiiPayPersonalInternationalKiiPayBusinessPayment(DB.KiiPayPersonalInternationalKiiPayBusinessPayment model)
        {
            dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Add(model);
            dbContext.SaveChanges();
            return model;


        }

        public DB.KiiPayPersonalWalletPaymentByKiiPayPersonal SaveKiiPayPersonalByKiiPayPersonalPayment(DB.KiiPayPersonalWalletPaymentByKiiPayPersonal model)
        {

            dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Add(model);
            dbContext.SaveChanges();
            return model;
        }



    }
}