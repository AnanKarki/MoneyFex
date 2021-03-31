using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class DashboardServices
    {
        FAXEREntities dbContext = null;
        public DashboardServices()
        {
            dbContext = new FAXEREntities();
        }


        public bool updateKiiPayPersonalAccount(EditAccountProfileViewModel model)
        {
            if (model != null)
            {
                if (Common.CardUserSession.LoggedCardUserViewModel != null)
                {
                    if (Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                    {
                        var userData = dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault();
                        if (userData != null)
                        {
                            int kiiPayPersonalWalletId = userData.KiiPayPersonalWalletInformationId;
                            var walletData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == kiiPayPersonalWalletId).FirstOrDefault();
                            var userLogInData = dbContext.KiiPayPersonalUserLogin.Where(x => x.KiiPayPersonalUserInformationId == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault();
                            if (walletData != null && userLogInData != null)
                            {
                                walletData.Address1 = model.Address1;
                                walletData.Address2 = model.Address2;
                                walletData.CardUserCity = model.City;
                                walletData.MobileNo = model.MobileNo;
                                walletData.CardUserEmail = model.EmailAddress;
                                dbContext.Entry(walletData).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();

                                userLogInData.Email = model.EmailAddress;
                                dbContext.Entry(userLogInData).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();

                                userData.City = model.City;
                                userData.AddressLine1 = model.Address1;
                                userData.AddressLine2 = model.Address2;
                                userData.MobileNo = model.MobileNo;
                                userData.EmailAddress = model.EmailAddress;
                                userData.IdCardType = model.IDCardType;
                                userData.IdCardNumber = model.IDCardNumber;
                                userData.IdCardExpiringDate = new DateTime(model.IDCardExpiringYear, (int)model.IDCardExpiringMonth, model.IDCardExpiringDay);
                                userData.IssuingCountry = model.IDCardIssuingCountry;
                                dbContext.Entry(userData).State = System.Data.Entity.EntityState.Modified;
                                dbContext.SaveChanges();
                                return true;
                            }
                        }
                    }
                }

            }
            return false;
        }

        public EditAccountProfileViewModel getViewProfileDetails()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if (Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    var result = (from c in dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).ToList()

                                  select new EditAccountProfileViewModel()
                                  {
                                      KiiPayPersonalUserId = c.Id,
                                      PhotoUrl = c.KiiPayPersonalWalletInformation.UserPhoto,
                                      Name = c.FirstName + " " + c.MiddleName + " " + c.Lastname,
                                      DOB = c.DOB.ToString("dd/MM/yyyy"),
                                      FullAddress = c.AddressLine1 + " " + c.AddressLine2 + " " + c.City,
                                      Country = Common.Common.GetCountryName(c.Country),
                                      MobileNo = Common.Common.GetCountryPhoneCode(c.Country) + " " + c.MobileNo,
                                      EmailAddress = c.EmailAddress,
                                      IDCardType = c.IdCardType,
                                      IDCardNumber = c.IdCardNumber,
                                      IDCardExpiringDate = c.IdCardExpiringDate == null ? "" : c.IdCardExpiringDate.Value.ToString("dd/MM/yyyy"),
                                      IDCardIssuingCountry = Common.Common.GetCountryName(c.IssuingCountry)
                                  }).FirstOrDefault();
                    return result;
                }
            }
            return null;
        }

        public EditAccountProfileViewModel getEditProfileDetails()
        {

            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if (Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    var result = (from c in dbContext.KiiPayPersonalUserInformation.Where(x => x.Id == Common.CardUserSession.LoggedCardUserViewModel.id).ToList()

                                  select new EditAccountProfileViewModel()
                                  {
                                      KiiPayPersonalUserId = c.Id,
                                      City = c.City,
                                      Address1 = c.AddressLine1,
                                      Address2 = c.AddressLine2,
                                      CountryPhoneCode = Common.Common.GetCountryPhoneCode(c.Country),
                                      MobileNo = c.MobileNo,
                                      EmailAddress = c.EmailAddress,
                                      IDCardType = c.IdCardType,
                                      IDCardNumber = c.IdCardNumber,
                                      IDCardExpiringDay = c.IdCardExpiringDate == null ? 0 : c.IdCardExpiringDate.Value.Day,
                                      IDCardExpiringMonth = c.IdCardExpiringDate == null ? Month.Month : (Month)c.IdCardExpiringDate.Value.Month,
                                      IDCardExpiringYear = c.IdCardExpiringDate == null ? 0000 : c.IdCardExpiringDate.Value.Year,
                                      IDCardIssuingCountry = c.IssuingCountry
                                  }).FirstOrDefault();
                    return result;

                }
            }
            return null;
        }

        public bool checkIfMobileNumberAlreadyExists(string mobileNumber)
        {
            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if (Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    var data = dbContext.KiiPayPersonalUserInformation.Where(x => x.MobileNo.Trim() == mobileNumber.Trim() && x.Id != Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault();
                    if (data == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool checkIfEmailAlreadyExists(string emailAddress)
        {
            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if (Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    var data = dbContext.KiiPayPersonalUserInformation.Where(x => x.EmailAddress.ToLower().Trim() == emailAddress.ToLower().Trim() && x.Id != Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId).FirstOrDefault();
                    if (data == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public List<DropDownViewModel> GetIDCardTypes()
        {
            var result = (from c in dbContext.IdentityCardType
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Code = c.CardType,
                              Name = c.CardType
                          }).ToList();
            return result;

        }

        public WalletUsageControlViewModel getWalletUsageControlViewModel()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if (Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    var walletData = dbContext.KiiPayPersonalWalletInformation.Find(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId);
                    if (walletData != null)
                    {
                        var result = new WalletUsageControlViewModel()
                        {
                            Id = walletData.Id,
                            WithdrawalAmount = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + walletData.CashWithdrawalLimit.ToString(),
                            WithdrawalType = Enum.GetName(typeof(CardLimitType), walletData.CashLimitType),
                            PurchaseAmount = Common.CardUserSession.LoggedCardUserViewModel.CardUserCurrencySymbol + walletData.GoodsPurchaseLimit.ToString(),
                            PurchaseType = Enum.GetName(typeof(AutoPaymentFrequency), walletData.GoodsLimitType)
                        };
                        return result;
                    }

                }
            }
            return null;
        }

        public string getWithdrawalCode(int walletId)
        {
            if (walletId != 0)
            {
                var data = dbContext.KiiPayPersonalWalletWithdrawalCode.Where(x => x.KiiPayPersonalWalletId == walletId).FirstOrDefault();
                if (data != null)
                {
                    return data.AccessCode;
                }
                KiiPayPersonalWalletWithdrawalCode withdrawalCodeData = new KiiPayPersonalWalletWithdrawalCode()
                {
                    KiiPayPersonalWalletId = walletId,
                    AccessCode = Common.Common.GetNewAccessCodeForCardUser(),
                    CreatedDateTime = DateTime.Now,
                    IsExpired = false
                };
                data = dbContext.KiiPayPersonalWalletWithdrawalCode.Add(withdrawalCodeData);
                dbContext.SaveChanges();
                return data.AccessCode;
            }
            return "";
        }

    }
}