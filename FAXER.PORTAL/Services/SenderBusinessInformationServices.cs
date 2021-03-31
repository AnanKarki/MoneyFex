using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;

namespace FAXER.PORTAL.Services
{
    public class SenderBusinessInformationServices
    {
        DB.FAXEREntities db = null;
        SFaxerSignUp _senderSignUpServices = null;
        public SenderBusinessInformationServices()
        {
            db = new DB.FAXEREntities();
            _senderSignUpServices = new SFaxerSignUp();
        }

        public bool AccountSetUpCompleted()
        {
            var loginInfo = _senderSignUpServices.GetBusinessPersonalLoginInfo();
            var personalInfo = _senderSignUpServices.GetPersonalDetail();
            var personalAddress = _senderSignUpServices.GetPersonalAddress();
            var businessInfo = _senderSignUpServices.GetSenderBusinessDetails();
            var registeredAddressInfo = _senderSignUpServices.GetSenderBusinessRegistered();
            var operatingAddressInfo = _senderSignUpServices.GetSenderBusinessOperating();

            DateTime DOB = new DateTime(personalInfo.Year, (int)personalInfo.Month, personalInfo.Day);
            string accountNo = "MFB" + _senderSignUpServices.GetNewAccount(7);
            DB.FaxerInformation faxerInformation = new DB.FaxerInformation()
            {
                FirstName = personalInfo.FirstName,
                MiddleName = personalInfo.MiddleName,
                LastName = personalInfo.LastName,
                DateOfBirth = DOB,
                GGender = (int)personalInfo.Gender,
                Email = loginInfo.EmailAddress,
                AccountNo = accountNo,
                Address1 = personalAddress.AddressLine1,
                Address2 = personalAddress.AddressLine2,
                City = personalAddress.City,
                PostalCode = personalAddress.PostCode,
                Country = loginInfo.Country,
                PhoneNumber = loginInfo.MobileNo,
                IsBusiness = true,
            };
            var addFaxerInfo = _senderSignUpServices.AddFaxerInformation(faxerInformation);


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
            _senderSignUpServices.AddFaxerLogin(login);

            DB.BusinessRelatedInformation businessRelatedInformation = new DB.BusinessRelatedInformation()
            {
                Country = loginInfo.Country,
                Postal = personalAddress.PostCode,
                AddressLine1 = personalAddress.AddressLine1,
                AddressLine2 = personalAddress.AddressLine2,
                BusinessName = businessInfo.BusinessName,
                BusinessType = businessInfo.BusinessType,
                RegistrationNo = businessInfo.RegistrationNumber,
                City = personalAddress.City,
                OperatingAddressLine1 = operatingAddressInfo.OperatingAddressLine1,
                OperatingAddressLine2 = operatingAddressInfo.OperatingAddressLine2,
                OperatingCity = operatingAddressInfo.OperatingCity,
                OperatingPostal = operatingAddressInfo.OperatingPostal,
                FaxerId = addFaxerInfo.Id

            };
            AddBusinessRelatedInformation(businessRelatedInformation);
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


            //#region Registration Verification 

            //// Sms Function Executed Here 

            //Common.FaxerSession.RegistrationCodeVerificationViewModel = RegistrationCodeVerificationViewModel;

            //SmsApi smsApiService = new SmsApi();


            //string message = smsApiService.GetRegistrationMessage(RegistrationCodeVerificationViewModel.VerificationCode);
            //smsApiService.SendSMS(RegistrationCodeVerificationViewModel.PhoneNo, message);
            //// redirected to the verfication Code Screen
            //#endregion


            _senderSignUpServices.SendRegistrationEmail(faxerInformation.FirstName, faxerInformation.Email, faxerInformation.PhoneNumber, faxerInformation.AccountNo, RegistrationCodeVerificationViewModel.VerificationCode);

            return true;
        }


        public DB.BusinessRelatedInformation AddBusinessRelatedInformation(DB.BusinessRelatedInformation businessRelatedInformation)
        {   
            db.BusinessRelatedInformation.Add(businessRelatedInformation);
            db.SaveChanges();
            return businessRelatedInformation;
        }

    }
}