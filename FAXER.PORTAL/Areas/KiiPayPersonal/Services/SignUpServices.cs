
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Services
{
    public class SignUpServices
    {
        FAXEREntities dbContext = null;
        public SignUpServices()
        {
            dbContext = new FAXEREntities();
        }

        public bool checkIfEmailAddressAlreadyExists(string emailAddress)
        {
            if (!string.IsNullOrEmpty(emailAddress))
            {
                var data = dbContext.KiiPayPersonalUserInformation.Where(x => x.EmailAddress.ToLower().Trim() == emailAddress.ToLower().Trim()).FirstOrDefault();
                if (data != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsValidMobileNo(string mobileNo) {

            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == mobileNo).FirstOrDefault();
            return data == null ? true : false;
        }

        public int AddKiiPayPersonalUser()
        {
            if (Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel != null)
            {
                var sessionData = Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel;
                DateTime dob = new DateTime(sessionData.BirthdateYear, sessionData.BirthdateMonth, sessionData.BirthdateDay);

                DB.KiiPayPersonalWalletInformation walletData = new KiiPayPersonalWalletInformation()
                {
                    FirstName = sessionData.FirstName,
                    MiddleName = sessionData.MiddleName,
                    LastName = sessionData.LastName,
                    CardUserDOB = dob,
                    Gender = sessionData.Gender,
                    Address1 = sessionData.PersonalAddressAddress1,
                    Address2 = sessionData.PersonalAddressAddress2,
                    CardUserCountry = sessionData.PersonalAddressCountry,
                    CardUserCity = sessionData.PersonalAddressCity,
                    CardUserState = "",
                    CardUserPostalCode = sessionData.PersonalAddressPostalCode,
                    MobileNo = sessionData.PhoneNumber,
                    CurrentBalance = 0,
                    CashWithdrawalLimit = 0,
                    CashLimitType = CardLimitType.NoLimitSet,
                    GoodsPurchaseLimit = 0,
                    GoodsLimitType = AutoPaymentFrequency.NoLimitSet,
                    AutoTopUp = false,
                    CardUserTel = "",
                    CardUserEmail = sessionData.EmailAddress,
                    CardStatus = CardStatus.Active,
                    UserPhoto = sessionData.PhotoUrl,
                    MFTCardPhoto = "",
                    AutoTopUpAmount = 0,
                    walletIsOF = WalletIsOF.KiiPayIndividualUser
                };
                var saveWalletData = dbContext.KiiPayPersonalWalletInformation.Add(walletData);
                dbContext.SaveChanges();



                DB.KiiPayPersonalUserInformation userInfoData = new KiiPayPersonalUserInformation()
                {
                    KiiPayPersonalWalletInformationId = saveWalletData.Id,
                    FirstName = sessionData.FirstName,
                    MiddleName = sessionData.MiddleName,
                    Lastname = sessionData.LastName,
                    DOB = dob,
                    Gender = sessionData.Gender,
                    EmailAddress = sessionData.EmailAddress,
                    Country = sessionData.PersonalAddressCountry,
                    City = sessionData.PersonalAddressCity,
                    AddressLine1 = sessionData.PersonalAddressAddress1,
                    AddressLine2 = sessionData.PersonalAddressAddress2,
                    PostCode = sessionData.PersonalAddressPostalCode,
                    MobileNo = sessionData.PhoneNumber,
                    IdCardType = "",
                    IdCardNumber = "",
                    IdCardExpiringDate = null,
                    IssuingCountry = ""
                };
                var saveUserInfoData = dbContext.KiiPayPersonalUserInformation.Add(userInfoData);
                dbContext.SaveChanges();
                Common.CardUserSession.KiiPayPersonalSignUpSessionViewModel.UserId = saveUserInfoData.Id;

                DB.KiiPayPersonalUserLogin userLoginData = new KiiPayPersonalUserLogin()
                {
                    KiiPayPersonalUserInformationId = saveUserInfoData.Id,
                    Email = sessionData.EmailAddress,
                    Password = sessionData.Password.Encrypt(),
                    IsActive = true,
                    ActivationCode = "",
                    LoginFailedCount = 0
                };
                dbContext.KiiPayPersonalUserLogin.Add(userLoginData);
                dbContext.SaveChanges();

                return saveUserInfoData.Id;
            }
            return 0;
        }

    }
}