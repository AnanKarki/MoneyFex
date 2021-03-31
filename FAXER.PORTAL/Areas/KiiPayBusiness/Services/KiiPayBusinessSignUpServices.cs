using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels.SignUpViewModel;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessSignUpServices
    {
        private DB.FAXEREntities dbContext = null;
        public KiiPayBusinessSignUpServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public bool IsMobileNoDuplicate(string MobileNo)
        {

            var result = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo == MobileNo).FirstOrDefault();
            if (result != null)
            {
                return true;
            }

            return false;


        }
        public bool IsEmailDuplicate(string email)
        {

            var result = dbContext.KiiPayBusinessInformation.Where(x => x.Email == email).FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public void SetKiiPayBusinessLoginInformation(BusinessLoginInformationVM vm)
        {

            Common.BusinessSession.KiiPayBusinessLoginInformation = vm;
        }
        public BusinessLoginInformationVM GetKiiPayBusinessLoginInformation()
        {
            BusinessLoginInformationVM vm = new BusinessLoginInformationVM();
            // if user try to go back get the session value of business login information  
            if (Common.BusinessSession.KiiPayBusinessLoginInformation != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessLoginInformation;
            }

            return vm;

        }

        public void SetPersonalInfo(KiiPayBusinessPersonalInfoVM vm)
        {
            Common.BusinessSession.KiiPayBusinessPersonalInfo = vm;
        }
        public KiiPayBusinessPersonalInfoVM GetPersonalInfo()
        {
            KiiPayBusinessPersonalInfoVM vm = new KiiPayBusinessPersonalInfoVM();
            // if user try to go back get the session value of business login information  
            if (Common.BusinessSession.KiiPayBusinessPersonalInfo != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessPersonalInfo;
            }

            return vm;

        }

        public void SetPersnalAddressInfo(AddressVM vm)
        {
            Common.BusinessSession.KiiPayBusinessPersnalAddressInfo = vm;
        }
        public AddressVM GetPersnalAddressInfo()
        {
            AddressVM vm = new AddressVM();
            vm.Country = GetKiiPayBusinessLoginInformation().Country;
            // if user try to go back get the session value of business login information  
            if (Common.BusinessSession.KiiPayBusinessPersnalAddressInfo != null)
            {
                vm = Common.BusinessSession.KiiPayBusinessPersnalAddressInfo;
            }

            return vm;

        }

        public void SetkiiPayBusinessInfo(kiiPayBusinessInfoVM vm)
        {
            Common.BusinessSession.kiiPayBusinessInfo = vm;
        }
        public kiiPayBusinessInfoVM GetkiiPayBusinessInfo()
        {
            kiiPayBusinessInfoVM vm = new kiiPayBusinessInfoVM();
            vm.CountryOfIncorporation = GetKiiPayBusinessLoginInformation().Country;
            // if user try to go back get the session value of business login information  
            if (Common.BusinessSession.kiiPayBusinessInfo != null)
            {
                vm = Common.BusinessSession.kiiPayBusinessInfo;
            }

            return vm;

        }

        public void SetBusinessAddressInfo(AddressVM vm)
        {
            Common.BusinessSession.BusinessAddressInfo = vm;

        }
        public AddressVM GetBusinessAddressInfo()
        {
            AddressVM vm = new AddressVM();
            vm.Country = GetKiiPayBusinessLoginInformation().Country;
            // if user try to go back get the session value of business login information  
            if (Common.BusinessSession.BusinessAddressInfo != null)
            {
                vm = Common.BusinessSession.BusinessAddressInfo;
            }

            return vm;

        }

        public void SetBusinessOpeationAddressInfo(AddressVM vm)
        {
            Common.BusinessSession.BusinessOpeationAddressInfo = vm;

        }
        public AddressVM GetBusinessOpeationAddressInfo()
        {
            AddressVM vm = new AddressVM();
            vm.Country = GetKiiPayBusinessLoginInformation().Country;
            // if user try to go back get the session value of business login information  
            if (Common.BusinessSession.BusinessOpeationAddressInfo != null)
            {
                vm = Common.BusinessSession.BusinessOpeationAddressInfo;
            }

            return vm;

        }

        public bool CompleteBuinessRegistration(KiiPayBusinessBillPaymentInfoVM vm)
        {


            var loginInfo = Common.BusinessSession.KiiPayBusinessLoginInformation;
            var personalInfo = Common.BusinessSession.KiiPayBusinessPersonalInfo;
            var KiiPayBusinessPersnalAddressInfo = Common.BusinessSession.KiiPayBusinessPersnalAddressInfo;
            var BusinessInfo = Common.BusinessSession.kiiPayBusinessInfo;
            var BusinessOpeationAddressInfo = Common.BusinessSession.BusinessOpeationAddressInfo;
            var BusinessAddressInfo = Common.BusinessSession.BusinessAddressInfo;



            DB.KiiPayBusinessInformation kiiPayBusinessInformation = new DB.KiiPayBusinessInformation()
            {
                BusinessCountry = BusinessInfo.CountryOfIncorporation,
                BusinessLicenseNumber = BusinessInfo.RegistrationNumber,
                BusinessName = BusinessInfo.BusinessName,
                BusinessType = BusinessInfo.BusinessType,
                Email = loginInfo.EmailAddress,
                BusinessMobileNo = loginInfo.MobileNo,
                BusinessOperationAddress1 = BusinessOpeationAddressInfo.AddressLine1,
                BusinessOperationAddress2 = BusinessOpeationAddressInfo.AddressLine2,
                BusinessOperationCity = BusinessOpeationAddressInfo.City,
                BusinessOperationPostalCode = BusinessOpeationAddressInfo.PostcodeORZipCode,
                BusinessOperationCountryCode = BusinessOpeationAddressInfo.Country,
                BusinessOperationState = "",
                BusinessRegisteredAddress1 = BusinessAddressInfo.AddressLine1,
                BusinessRegisteredAddress2 = BusinessAddressInfo.AddressLine2,
                BusinessRegisteredCity = BusinessAddressInfo.City,
                BusinessRegisteredCountry = BusinessAddressInfo.Country,
                BusinessRegisteredPostalCode = BusinessAddressInfo.PostcodeORZipCode,
                BusinessRegisteredState = "",
                CountryOfIncorporation = BusinessInfo.CountryOfIncorporation,
                ContactPerson = "",
                PhoneNumber  = loginInfo.MobileNo,
                FaxNumber = "",
                RegistrationNumBer = BusinessInfo.RegistrationNumber,
                Website = "",
                BillIsIssuedToCustomer = vm.DoesBillIsIssuedToCustomer

            };

            var kiiPayBusinessInformation_result = SaveKiiPayBusinessInformation(kiiPayBusinessInformation);
            DB.KiiPayBusinessUserPersonalInfo kiiPayBusinessUserPersonalInfo = new DB.KiiPayBusinessUserPersonalInfo()
            {
                FirstName = personalInfo.FirstName,
                MiddleName = personalInfo.MiddleName,
                LastName = personalInfo.LastName,
                KiiPayBusinessInformationId = kiiPayBusinessInformation_result.Id,
                AddressLine1 = KiiPayBusinessPersnalAddressInfo.AddressLine1,
                AddressLine2 = KiiPayBusinessPersnalAddressInfo.AddressLine2,
                City = KiiPayBusinessPersnalAddressInfo.City,
                Country = KiiPayBusinessPersnalAddressInfo.Country,
                BirthDate = (DateTime)personalInfo.BirthDate,
                PostCodeORZipCode= KiiPayBusinessPersnalAddressInfo.PostcodeORZipCode,
            };
            var kiiPayBusinessUserPersonalInfo_result = SaveKiiPayBusinessUserPersonalInfo(kiiPayBusinessUserPersonalInfo);

            DB.KiiPayBusinessLogin kiiPayBusinessLogin = new DB.KiiPayBusinessLogin() {

                Username = loginInfo.EmailAddress,
                MobileNo = loginInfo.MobileNo,
                Password = loginInfo.Password.Encrypt(),
                LoginFailCount = 0 ,
                IsActive = false,
                ActivationCode = "",
                IsDeleted = false,
                KiiPayBusinessInformationId = kiiPayBusinessInformation_result.Id,
                LoginCode= ""
            };

            var kiiPayBusinessLogin_result = SaveKiiPayBusinessLogin(kiiPayBusinessLogin);



            #region Business Wallet Registration 

            DB.KiiPayBusinessWalletInformation kiiPayBusinessWalletInformation = new DB.KiiPayBusinessWalletInformation()
            {
                FirstName = personalInfo.FirstName,
                MiddleName = personalInfo.MiddleName,
                LastName = personalInfo.LastName,
                AddressLine1 = BusinessOpeationAddressInfo.AddressLine1,
                AddressLine2 = BusinessOpeationAddressInfo.AddressLine2,
                City  = BusinessOpeationAddressInfo.City,
                Country = BusinessOpeationAddressInfo.Country,
                PostalCode = BusinessOpeationAddressInfo.PostcodeORZipCode,
                State = "",
                AutoTopUp = false,
                CardStatus = CardStatus.Active,
                DOB = (DateTime)personalInfo.BirthDate,
                Email = loginInfo.EmailAddress,
                Gender = personalInfo.Gender,
                IdCardNumber = "",
                IdCardType = "",
                IdIssuingCountry = "",
                IdExpiryDate = DateTime.Now,
                MobileNo = loginInfo.MobileNo,
                PhoneNumber = loginInfo.MobileNo,
                KiiPayUserPhoto  = "",
                MFBCardPhoto  = "",
                KiiPayBusinessInformationId = kiiPayBusinessInformation_result.Id

            };
            RegisterBusinessWallet(kiiPayBusinessWalletInformation);

            #endregion

            GenerateVerificationCode(kiiPayBusinessInformation_result.BusinessCountry, kiiPayBusinessInformation_result.Id, kiiPayBusinessInformation_result.BusinessMobileNo);
            if (kiiPayBusinessInformation.BillIsIssuedToCustomer == true)
            {
                DB.Suppliers Suppliers = new Suppliers()
                {
                    Country = kiiPayBusinessInformation_result.BusinessCountry,
                    CreationDate = DateTime.Now,
                    IsActive = true,
                    KiiPayBusinessInformationId = kiiPayBusinessInformation_result.Id,
                    RefCode = kiiPayBusinessInformation.BusinessCountry + kiiPayBusinessInformation_result.Id
                };
                var Save_SuppliersSucess = SaveSuppliers(Suppliers);
            }
            

            return true;


        }

        public DB.KiiPayBusinessInformation SaveKiiPayBusinessInformation(DB.KiiPayBusinessInformation model) {


            dbContext.KiiPayBusinessInformation.Add(model);
            dbContext.SaveChanges();
            return model;
            
        }

        public DB.KiiPayBusinessUserPersonalInfo SaveKiiPayBusinessUserPersonalInfo(DB.KiiPayBusinessUserPersonalInfo model) {


            dbContext.KiiPayBusinessUserPersonalInfo.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        public DB.Suppliers SaveSuppliers(DB.Suppliers model) {


            dbContext.Suppliers.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        public DB.KiiPayBusinessLogin SaveKiiPayBusinessLogin(DB.KiiPayBusinessLogin model) {


            dbContext.KiiPayBusinessLogin.Add(model);
            dbContext.SaveChanges();
            return model;

        }

        public void GenerateVerificationCode( string CountryCode , int KiiPayUserId , string PhoneNumber)
        {

            string verificationCode = Common.Common.GenerateVerificationCode(6);

            RegistrationCodeVerificationViewModel regverificationCodeVm = new RegistrationCodeVerificationViewModel()
            {
                Country = CountryCode,
                UserId = KiiPayUserId,
                //IsExpired = false,
                PhoneNo = Common.Common.GetCountryPhoneCode(CountryCode) + " " + PhoneNumber,
                RegistrationOf = RegistrationOf.KiiPayBusiness,
                VerificationCode = verificationCode,

            };

            Common.FaxerSession.RegistrationCodeVerificationViewModel = regverificationCodeVm;
            SRegistrationVerificationCode registrationVerificationCodeServices = new SRegistrationVerificationCode();
            registrationVerificationCodeServices.Add(regverificationCodeVm);


            // Send SMS 
            SendVerificationCodeSMS(verificationCode, regverificationCodeVm.PhoneNo);


        }

        public void SendVerificationCodeSMS(string verificationCode , string PhoneNo) {

            // Sms Function Execute Here 
            SmsApi smsApiServices = new SmsApi();
            string message = smsApiServices.GetRegistrationMessage(verificationCode);

            smsApiServices.SendSMS(PhoneNo, message);
            //Email 
        }


        public DB.KiiPayBusinessWalletInformation RegisterBusinessWallet(DB.KiiPayBusinessWalletInformation walletInfo)
        {

            dbContext.KiiPayBusinessWalletInformation.Add(walletInfo);
            dbContext.SaveChanges();
            return walletInfo;
          
        }

    }
}