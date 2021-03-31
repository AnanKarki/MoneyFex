using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Models;
using Microsoft.AspNet.Identity.Owin;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models.SignUp;
using FAXER.PORTAL.DB;
using System.Web.Mvc;

namespace FAXER.PORTAL.Services
{
    public class SFaxerSignUp
    {
        DB.FAXEREntities db = null;
        public SFaxerSignUp()
        {
            db = new DB.FAXEREntities();
        }

        public void SetPersonalLoginInfo(SenderPersonalLoginInformationVM vm)
        {


            Common.FaxerSession.SenderPersonalLoginInformationVM = vm;
        }

        public SenderPersonalLoginInformationVM GetPersonalLoginInfo()
        {

            SenderPersonalLoginInformationVM vm = new SenderPersonalLoginInformationVM();
            if (Common.FaxerSession.SenderPersonalLoginInformationVM != null)
            {

                vm = Common.FaxerSession.SenderPersonalLoginInformationVM;
            }
            return vm;
        }

        internal DB.SenderBusinessDocumentation GetIdentificationDetail(int id)
        {
            var result = db.SenderBusinessDocumentation.Where(x => x.SenderId == id).FirstOrDefault();
            return result;
        }

        internal string GetIdType(int identificationTypeId)
        {
            var result = db.IdentityCardType.Where(x => x.Id == identificationTypeId).FirstOrDefault();

            return result == null ? "0" : result.CardType;
        }

        public void SetBusinessPersonalLoginInfo(SenderBusinessLoginInfoViewModel vm)
        {


            Common.FaxerSession.SenderBusinessLoginInfoViewModel = vm;
        }

        public SenderBusinessLoginInfoViewModel GetBusinessPersonalLoginInfo()
        {

            SenderBusinessLoginInfoViewModel vm = new SenderBusinessLoginInfoViewModel();
            if (Common.FaxerSession.SenderBusinessLoginInfoViewModel != null)
            {

                vm = Common.FaxerSession.SenderBusinessLoginInfoViewModel;
            }
            return vm;
        }

        public void SetPersonalDetail(SenderPersonalDetailVM vm)
        {

            Common.FaxerSession.SenderPersonalDetialVm = vm;

        }

        public SenderPersonalDetailVM GetPersonalDetail()
        {

            SenderPersonalDetailVM vm = new SenderPersonalDetailVM();
            if (Common.FaxerSession.SenderPersonalDetialVm != null)
            {

                vm = Common.FaxerSession.SenderPersonalDetialVm;
            }
            return vm;
        }


        public void SetPersonalAddress(SenderPersonalAddressVM vm)
        {

            Common.FaxerSession.SenderPersonalAddressVM = vm;
        }


        public SenderPersonalAddressVM GetPersonalAddress()
        {

            SenderPersonalAddressVM vm = new SenderPersonalAddressVM();
            if (Common.FaxerSession.SenderPersonalAddressVM != null)
            {

                vm = Common.FaxerSession.SenderPersonalAddressVM;
            }
            return vm;

        }

        public void SetSenderBusinessDetails(SenderBusinessDetailsViewModel vm)
        {

            Common.FaxerSession.SenderBusinessDetailsViewModel = vm;
        }

        public SenderBusinessDetailsViewModel GetSenderBusinessDetails()
        {

            SenderBusinessDetailsViewModel vm = new SenderBusinessDetailsViewModel();
            if (Common.FaxerSession.SenderBusinessDetailsViewModel != null)
            {

                vm = Common.FaxerSession.SenderBusinessDetailsViewModel;
            }
            return vm;

        }
        public void SetSenderBusinessRegistered(SenderBusinessRegisteredViewModel vm)
        {

            Common.FaxerSession.SenderBusinessRegisteredViewModel = vm;
        }

        public SenderBusinessRegisteredViewModel GetSenderBusinessRegistered()
        {

            SenderBusinessRegisteredViewModel vm = new SenderBusinessRegisteredViewModel();
            if (Common.FaxerSession.SenderBusinessRegisteredViewModel != null)
            {

                vm = Common.FaxerSession.SenderBusinessRegisteredViewModel;
            }
            return vm;

        }
        public void SetSenderBusinessOperating(SenderBusinessOperatingViewModel vm)
        {

            Common.FaxerSession.SenderBusinessOperatingViewModel = vm;
        }

        public SenderBusinessOperatingViewModel GetSenderBusinessOperating()
        {

            SenderBusinessOperatingViewModel vm = new SenderBusinessOperatingViewModel();
            if (Common.FaxerSession.SenderBusinessOperatingViewModel != null)
            {

                vm = Common.FaxerSession.SenderBusinessOperatingViewModel;
            }
            return vm;

        }



        public bool CompleteRegistration(SenderKiiPayWalletEnableVM vm)
        {

            var loginInfo = GetPersonalLoginInfo();
            var personalInfo = GetPersonalDetail();
            var addressInfo = GetPersonalAddress();

            DateTime DOB = new DateTime(personalInfo.Year, (int)personalInfo.Month, personalInfo.Day);
            string accountNo = "MF" + GetNewAccount(7);
            DB.FaxerInformation faxerInformation = new DB.FaxerInformation()
            {
                FirstName = personalInfo.FirstName,
                MiddleName = personalInfo.MiddleName,
                LastName = personalInfo.LastName,
                DateOfBirth = DOB,
                GGender = (int)personalInfo.Gender,
                Email = loginInfo.EmailAddress,
                AccountNo = accountNo,
                Address1 = addressInfo.AddressLine1,
                Address2 = addressInfo.AddressLine2,
                City = addressInfo.City,
                PostalCode = addressInfo.PostCode,
                Country = loginInfo.CountryCode,
                PhoneNumber = loginInfo.MobileNo
            };


            var addFaxerResult = AddFaxerInformation(faxerInformation);

            SCity.Save(faxerInformation.City, faxerInformation.Country, DB.Module.Faxer);

            var guId = Guid.NewGuid().ToString();
            DB.FaxerLogin login = new DB.FaxerLogin()
            {
                FaxerId = faxerInformation.Id,
                UserName = faxerInformation.Email,
                Password = loginInfo.Password.ToHash(),
                ActivationCode = guId,
                IsActive = true,
                MobileNo = loginInfo.MobileNo
            };
            AddFaxerLogin(login);


            #region Enable KiipayWallet 

            if (vm.EnableKiiPayWallet)
            {
                EnableKiiPayWallet(addFaxerResult.Id);
            }
            #endregion
            // Generate verification code for sender registration 

            string VerficationCode = Common.Common.GenerateVerificationCode(6);

            RegistrationCodeVerificationViewModel RegistrationCodeVerificationViewModel = new RegistrationCodeVerificationViewModel()
            {

                PhoneNo = Common.Common.GetCountryPhoneCode(faxerInformation.Country) + " " + faxerInformation.PhoneNumber,
                UserId = faxerInformation.Id,
                VerificationCode = VerficationCode,
                RegistrationOf = RegistrationOf.Sender,
                Country = faxerInformation.Country
            };

            Common.FaxerSession.RegistrationCodeVerificationViewModel = RegistrationCodeVerificationViewModel;
            SRegistrationVerificationCode registrationVerificationCodeServices = new SRegistrationVerificationCode();
            registrationVerificationCodeServices.Add(RegistrationCodeVerificationViewModel);



            //send activation link to the user
            //MailCommon mail = new MailCommon();
            //var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            //var link = string.Format("{0}/FaxerAccount/Activate/{1}", baseUrl, guId);

            //string body = "";
            //body = Common.Common.GetTemplate(baseUrl + "/emailtemplate/FaxerActivationEmail?guid=" + guId + "&faxername=" + faxerInformation.FirstName + " " + faxerInformation.LastName);

            //mail.SendMail(faxerInformation.Email, "Welcome to MoneyFex", body);

            //#region Registration Verification 

            //// Sms Function Executed Here 

            //Common.FaxerSession.RegistrationCodeVerificationViewModel = RegistrationCodeVerificationViewModel;

            //SmsApi smsApiService = new SmsApi();


            //string message = smsApiService.GetRegistrationMessage(RegistrationCodeVerificationViewModel.VerificationCode);
            //smsApiService.SendSMS(RegistrationCodeVerificationViewModel.PhoneNo, message);
            //// redirected to the verfication Code Screen
            //#endregion


            SendRegistrationEmail(faxerInformation.FirstName, faxerInformation.Email, faxerInformation.PhoneNumber, faxerInformation.AccountNo, RegistrationCodeVerificationViewModel.VerificationCode);

            return true;
        }



        public void SendRegistrationEmail(string firstName, string email, string mobileNo, string CustomerNo, string verificationCode)
        {

            Common.Common.SetTransactionEmailTypeSession(TransactionEmailType.WelcomeCustomer);

            //send activation link to the user
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

            string body = "";
            body = Common.Common.GetTemplate(baseUrl + "/emailtemplate/SenderSignUpEmailTemplate?" +
                "FirstName=" + firstName + "&Email=" + email + "&MobileNo=" + mobileNo + "&CustomerNo=" + CustomerNo + "&verificationCode=" + verificationCode);

            mail.SendMail(email, "Welcome to MoneyFex", body);


        }
        public void EnableKiiPayWallet(int SenderId)
        {

            var loginInfo = GetPersonalLoginInfo();
            var personalInfo = GetPersonalDetail();
            var addressInfo = GetPersonalAddress();


            DateTime DOB = new DateTime(personalInfo.Year, (int)personalInfo.Month, personalInfo.Day);

            KiiPayPersonalWalletInformation kiiPayPersonalWalletInformation = new KiiPayPersonalWalletInformation()
            {
                FirstName = personalInfo.FirstName,
                MiddleName = personalInfo.MiddleName,
                LastName = personalInfo.LastName,
                walletIsOF = WalletIsOF.Sender,
                UserPhoto = personalInfo.UserPhotoURL,
                MobileNo = loginInfo.MobileNo,
                IsDeleted = false,
                Address1 = addressInfo.AddressLine1,
                Address2 = addressInfo.AddressLine2,
                AutoTopUp = false,
                AutoTopUpAmount = 0,
                CardStatus = CardStatus.Active,
                CardUserCity = addressInfo.City,
                CardUserCountry = loginInfo.CountryCode,
                CardUserDOB = DOB,
                CardUserEmail = loginInfo.EmailAddress,
                CardUserPostalCode = addressInfo.PostCode,
                CardUserState = "",
                CardUserTel = loginInfo.MobileNo,
                Gender = Gender.Male,
                CashLimitType = CardLimitType.NoLimitSet,
                CashWithdrawalLimit = 0,
                CurrentBalance = 0,
                GoodsLimitType = AutoPaymentFrequency.NoLimitSet,
                GoodsPurchaseLimit = 0,
                MFTCardPhoto = ""
            };

            db.KiiPayPersonalWalletInformation.Add(kiiPayPersonalWalletInformation);
            //db.SaveChanges();

            SenderKiiPayPersonalAccount senderKiiPayPersonalAccount = new SenderKiiPayPersonalAccount()
            {
                KiiPayAccountIsOF = KiiPayAccountIsOF.Sender,
                KiiPayPersonalWalletId = kiiPayPersonalWalletInformation.Id,
                SenderId = SenderId
            };
            db.SenderKiiPayPersonalAccount.Add(senderKiiPayPersonalAccount);
            db.SaveChanges();

        }
        public DB.FaxerInformation AddFaxerInformation(DB.FaxerInformation faxer)
        {
            faxer.IdCardExpiringDate = DateTime.Now;
            db.FaxerInformation.Add(faxer);
            db.SaveChanges();
            return faxer;
        }
        public DB.FaxerLogin AddFaxerLogin(DB.FaxerLogin faxer)
        {
            db.FaxerLogin.Add(faxer);
            db.SaveChanges();
            return faxer;
        }
        public DB.FaxerLogin ActivateFaxerLogin(string id)
        {
            var faxer = db.FaxerLogin.Where(x => x.ActivationCode == id).FirstOrDefault();
            faxer.IsActive = true;
            db.Entry<DB.FaxerLogin>(faxer).State = EntityState.Modified;
            db.SaveChanges();
            return faxer;
        }
        public bool CheckEmailAvailableForLogin(string email)
        {
            return db.FaxerInformation.Where(x => x.Email == email).Count() == 0;
        }

        public bool? IsActive([Bind(Include = LoginViewModel.BindProperty)]LoginViewModel model)
        {

            model.Email.IgnoreZero();
            var passHash = model.Password.ToHash();

            var count = db.FaxerLogin.Where(x => (x.UserName == model.Email || x.MobileNo == model.Email) && x.Password == passHash).FirstOrDefault();

            if (count != null)
            {
                return count.IsActive;
            }

            return null;

        }
        internal SignInStatus Login(LoginViewModel model)
        {
            var passHash = model.Password.ToHash();
            var count = db.FaxerLogin.Where(x => (x.UserName == model.Email || x.MobileNo == model.Email)
            && x.Password == passHash).FirstOrDefault();
            if ((count != null) && count.IsActive == true)
            {
                var faxerInfo = GetInformation(count.UserName);
                if (faxerInfo.IsBusiness != model.isBusiness)
                {
                    return SignInStatus.Failure;
                }

                var faxer = count.Faxer;
                string FaxerMFCode = db.FaxerInformation.Where(x => x.Id == count.FaxerId).Select(x => x.AccountNo).FirstOrDefault();
                LoggedUser user = new LoggedUser()
                {
                    UserName = count.UserName,
                    FullName = faxer.FirstName + " " + faxer.MiddleName + " " + faxer.LastName,
                    Id = faxer.Id,
                    CountryCode = faxer.Country,
                    FaxerMFCode = FaxerMFCode,
                    PhoneNo = faxer.PhoneNumber,
                    CountryPhoneCode = Common.Common.GetCountryPhoneCode(faxer.Country),
                    IsBusiness = faxer.IsBusiness,
                    FirstName = faxer.FirstName,
                    HouseNo = faxer.Address1,
                    PostCode = faxer.PostalCode
                };

                count.LoginFailedCount = 0;
                count.IsActive = true;
                db.Entry(count).State = EntityState.Modified;
                db.SaveChanges();
                FaxerSession.LoggedUser = user;
                FaxerSession.LoggedUserName = faxer.Email;
                FaxerSession.FaxerCountry = faxer.Country;
                FaxerSession.MobileNo = faxer.PhoneNumber;
                FaxerSession.SenderId = faxer.Id;

                return SignInStatus.Success;
            }
            else
            {

                var data = db.FaxerLogin.Where(x => (x.UserName == model.Email || x.MobileNo == model.Email)).FirstOrDefault();
                //if ((data != null) && data.IsActive == true)
                //{

                //    data.LoginFailedCount = data.LoginFailedCount + 1;
                //    db.Entry(data).State = EntityState.Modified;
                //    db.SaveChanges();


                //    if (data.LoginFailedCount == 3)
                //    {

                //        data.IsActive = false;
                //        db.Entry(data).State = EntityState.Modified;
                //        db.SaveChanges();

                //    }
                //}


            }
            return SignInStatus.Failure;
        }

        internal int? LoginFailureCount(string Username)
        {


            var count = db.FaxerLogin.Where(x => x.UserName == Username).FirstOrDefault();

            if (count != null)
            {
                return count.LoginFailedCount;
            }
            return null;

        }


        internal FaxerLogin SenderInfo(string Username)
        {



            var result = db.FaxerLogin.Where(x => (x.UserName == Username || x.MobileNo == Username)).FirstOrDefault();

            return result;

        }

        internal bool ResetPassword(FaxerPasswordResetViewModel model)
        {
            var faxerLogin = db.FaxerLogin.Where(x => x.UserName == model.Email).FirstOrDefault();
            if (faxerLogin != null)
            {
                faxerLogin.Password = model.Password.ToHash();
                db.Entry<DB.FaxerLogin>(faxerLogin).State = EntityState.Modified;
                try
                {
                    db.SaveChanges();
                }
                catch
                {

                    return false;
                }
                return true;
            }
            else
            {
                return false;
            }

        }

        public DB.FaxerInformation GetInformation(string email)
        {
            var info = db.FaxerInformation.Where(x => x.Email == email || x.PhoneNumber == email).FirstOrDefault();

            Common.FaxerSession.LoggedUserName = info.Email;
            return info;
        }
        public DB.FaxerInformation GetInformation(int senderId)
        {
            var info = db.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();
            return info;
        }


        public string GetNewAccount(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            //return s;
            while (db.FaxerInformation.Where(x => x.AccountNo == s).Count() > 0)
            {
                for (int i = 0; i < length; i++)
                    s = String.Concat(s, random.Next(10).ToString());
            }
            return s;
        }

        public bool EmailExist(string email)
        {

            var data = db.FaxerInformation.Where(x => x.Email == email).FirstOrDefault() == null ? false : true;

            return data;

        }



        public string GetFaxerNameByEmail(string email)
        {

            var data = db.FaxerInformation.Where(x => x.Email == email).FirstOrDefault();

            return data.FirstName + " " + data.MiddleName + " " + data.LastName;

        }




    }
}