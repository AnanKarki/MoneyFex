using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class VirtualAccountDepositServices
    {
        FAXEREntities dbContext = null;
        public VirtualAccountDepositServices()
        {
            dbContext = new FAXEREntities();
        }


        public List<DropDownViewModel> getRecentPhoneNumbers()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if (Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    var result = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderId == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId && x.PaymentType == PaymentType.Local).ToList()
                                  select new DropDownViewModel()
                                  {
                                      Id = c.Id,
                                      Code = c.ReceivingMobileNumber,
                                      Name = c.ReceivingMobileNumber
                                  }).ToList();
                    return result;
                }
            }
            return null;
        }

        public List<DropDownViewModel> getRecentPhoneNumbersInternational()
        {
            if(Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    var result = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.Where(x => x.SenderId == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId && x.PaymentType == PaymentType.International).ToList()

                                  select new DropDownViewModel()
                                  {
                                      Id = c.Id,
                                      Code = c.ReceivingMobileNumber,
                                      Name = c.ReceivingMobileNumber
                                  }).ToList();
                    return result;
                }
            }
            return null;
        }

        public string getCountryPhoneCode()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if (Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    var countryPhoneCode = Common.Common.GetCountryPhoneCode(Common.CardUserSession.LoggedCardUserViewModel.CountryCode);
                    return countryPhoneCode;
                }
            }
            return "";
        }

        public bool isPhoneNumberValid (string phoneNumber)
        {
            if(!string.IsNullOrEmpty(phoneNumber))
            {
                var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == phoneNumber.Trim() && x.CardUserCountry.Trim() == Common.CardUserSession.LoggedCardUserViewModel.CountryCode.Trim() ).FirstOrDefault();
                if(data != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool isphoneNumberValidForCountry(string phoneNumber, string countryCode)
        {
            if(!string.IsNullOrEmpty(phoneNumber) && !string.IsNullOrEmpty(countryCode))
            {
                var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == phoneNumber.Trim() && x.CardUserCountry.Trim() == countryCode.Trim()).FirstOrDefault();
                if(data != null)
                {
                    return true;
                }
            }
            return false;
        }

        
        public KiiPayEnterAmountViewModel getEnterAmountData()
        {
            
                var data = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo.Trim() == Common.CardUserSession.PayIntoAnotherWalletSession.ReceivingPhoneNumber.Trim()).FirstOrDefault();
                if(data != null)
                {
                    var vm = new KiiPayEnterAmountViewModel()
                    {
                        Id = data.Id,
                        Name = data.FirstName + " " + data.MiddleName + " " + data.Lastname,
                        CurrencySymbol = Common.Common.GetCurrencySymbol(data.Country),
                        Amount = 0,
                        CurrencyCode = Common.Common.GetCountryCurrency(data.Country),
                        SendSMSNotification = false,
                        PhotoUrl = data.KiiPayPersonalWalletInformation.MFTCardPhoto
                    };
                    return vm;                              
                }
            
            return null;
        }

        public string getPhotoUrl(string phoneNumber)
        {
            if(!string.IsNullOrEmpty(phoneNumber))
            {
                var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo.Trim() == phoneNumber.Trim()).FirstOrDefault();
                if(data != null)
                {
                    return data.UserPhoto;
                }
            }
            return "";
        }

        public bool checkIfAmountIsValid(decimal amount)
        {
            if(amount != 0)
            {
                var data = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault();
                if(data != null)
                {
                    decimal availableBalance = data.KiiPayPersonalWalletInformation.CurrentBalance;
                    if(availableBalance > amount)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}