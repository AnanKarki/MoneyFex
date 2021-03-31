using FAXER.PORTAL.Areas.Mobile.Models.Common;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex.SignUp;
using FAXER.PORTAL.Areas.Mobile.Services.Common;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Twilio.Rest.Trunking.V1;

namespace FAXER.PORTAL.Areas.Mobile.Services.MoneyFex
{
    public class MobileMoneyFexSignUpServices
    {
        DB.FAXEREntities dbContext = null;
        SFaxerSignUp _sFaxerSignUp = null;
        MobileCommonServices _mobileCommonServices = null;
        public MobileMoneyFexSignUpServices()
        {
            dbContext = new DB.FAXEREntities();
            _sFaxerSignUp = new SFaxerSignUp();
            _mobileCommonServices = new MobileCommonServices();
        }

        public ServiceResult<MobilePersonalSignUpModel> CompletePersonalSignUp(MobilePersonalSignUpModel model)
        {
            //Take Data From model
            try
            {
                string accountNo = "MF" + _sFaxerSignUp.GetNewAccount(7);
                DB.FaxerInformation faxerInformation = new DB.FaxerInformation()
                {
                    FirstName = model.LegalFirstName,
                    MiddleName = "",
                    LastName = model.LegalLastName,
                    //DateOfBirth = model.DateOfBirth.ToDateTime(),
                    DateOfBirth = model.DateOfBirth,
                    GGender = model.Gender,
                    Email = model.EmailAddress,
                    AccountNo = accountNo,
                    Address1 = model.AddressLineOne,
                    Address2 = model.AddressLineTwo,
                    City = model.City,
                    PostalCode = model.PostcodeOrZipCode,
                    Country = model.CountryCode,
                    PhoneNumber = model.MobileNumber,

                };

                //First Step:
                // Save To FaxerInfo According To  FaxerInformation Table
                //Note: FaxerInformation IsBusiness should be false

                var addFaxerResult = _sFaxerSignUp.AddFaxerInformation(faxerInformation);

                SCity.Save(faxerInformation.City, faxerInformation.Country, DB.Module.Faxer);


                //Second Step
                //Save To FaxerInfo According To FaxerLogin Table

                var guId = Guid.NewGuid().ToString();
                DB.FaxerLogin login = new DB.FaxerLogin()
                {
                    FaxerId = faxerInformation.Id,
                    UserName = faxerInformation.Email,
                    Password = model.Password.ToHash(),
                    ActivationCode = guId,
                    IsActive = true,
                    MobileNo = model.MobileNumber
                };
                _sFaxerSignUp.AddFaxerLogin(login);



                string VerficationCode = FAXER.PORTAL.Common.Common.GenerateVerificationCode(6);


                MobileRegistrationCodeVerificationViewModel RegistrationCodeVm = new MobileRegistrationCodeVerificationViewModel()
                {
                    Country = faxerInformation.Country,
                    UserId = faxerInformation.Id,
                    PhoneNo = faxerInformation.PhoneNumber,
                    RegistrationOf = RegistrationOf.Sender,
                    VerificationCode = VerficationCode,

                };
                MobileCommonServices _mobileCommonServices = new MobileCommonServices();
                _mobileCommonServices.AddRegistrationVerificationCode(RegistrationCodeVm);

                //_mobileCommonServices.SendVerificationCodeSMS(VerficationCode, RegistrationCodeVm.PhoneNo);

                model.CurrencyCode = model.CurrencyCode;
                model.CurrencySymbol = model.CurrencySymbol;
                model.SenderId = faxerInformation.Id;



                /// Author Umesh Timsina
                /// For Signup Problem
                try
                {
                    _sFaxerSignUp.SendRegistrationEmail(faxerInformation.FirstName, faxerInformation.Email, faxerInformation.PhoneNumber, faxerInformation.AccountNo, RegistrationCodeVm.VerificationCode);
                }
                catch (Exception ex)
                {

                }


                var context = UpdateSession(model.SenderId);
                return new ServiceResult<MobilePersonalSignUpModel>()
                {
                    Data = model,
                    Message = "AddedSuccess",
                    Status = ResultStatus.OK,
                    Token = context.Token
                };
            }
            catch (Exception ex)
            {
                return new ServiceResult<MobilePersonalSignUpModel>()
                {
                    Data = model,
                    Message = "Please try again",
                    Status = ResultStatus.Warning,
                };
            }


        }

     
        public ServiceResult<MobileBusinessSignUpModel> CompleteBusinessSignUp(MobileBusinessSignUpModel model)
        {

            bool IsMobileNoExist = _mobileCommonServices.getIsMobileExist(model.MobileNumber).Data;
            bool isEmailExist = _mobileCommonServices.getIsEmailExist(model.EmailAddress).Data;

            if (!IsMobileNoExist && !isEmailExist)
            {
                string accountNo = "MFB" + _sFaxerSignUp.GetNewAccount(7);
                DB.FaxerInformation faxerInformation = new DB.FaxerInformation()
                {
                    FirstName = model.LegalFirstName,
                    MiddleName = "",
                    LastName = model.LegalLastName,
                    DateOfBirth = model.DateOfBirth,
                    GGender = model.Gender.ToInt(),
                    Email = model.EmailAddress,
                    AccountNo = accountNo,
                    Address1 = model.PersonalAddressLineOne,
                    Address2 = model.PersonalAddressLineTwo,
                    City = model.PersonalCity,
                    PostalCode = model.PersonalPostcodeOrZipCode,
                    Country = model.CountryCode,
                    PhoneNumber = model.MobileNumber,
                    IsBusiness = true,
                };
                var addFaxerInfo = _sFaxerSignUp.AddFaxerInformation(faxerInformation);


                SCity.Save(faxerInformation.City, faxerInformation.Country, DB.Module.Faxer);

                var guId = Guid.NewGuid().ToString();
                DB.FaxerLogin login = new DB.FaxerLogin()
                {
                    FaxerId = faxerInformation.Id,
                    UserName = faxerInformation.Email,
                    Password = model.Password.ToHash(),
                    ActivationCode = guId,
                    IsActive = true,
                    MobileNo = model.MobileNumber
                };
                _sFaxerSignUp.AddFaxerLogin(login);

                DB.BusinessRelatedInformation businessRelatedInformation = new DB.BusinessRelatedInformation()
                {
                    Country = model.CountryCode,
                    Postal = model.PersonalPostcodeOrZipCode,
                    AddressLine1 = model.PersonalAddressLineOne,
                    AddressLine2 = model.PersonalAddressLineTwo,
                    BusinessName = model.BusinessLeagalName,
                    //BusinessType = model.BusinessType,
                    RegistrationNo = model.BusinessRegistrationNumber,
                    City = model.PersonalCity,
                    OperatingAddressLine1 = model.OperatingAddressLineOne,
                    OperatingAddressLine2 = model.OperatingAddressLineTwo,
                    OperatingCity = model.OperatingCity,
                    OperatingPostal = model.OperatingPostcodeOrZipCode,
                    FaxerId = addFaxerInfo.Id

                };
                SenderBusinessInformationServices _senderBusinessInformationServices = new SenderBusinessInformationServices();
                _senderBusinessInformationServices.AddBusinessRelatedInformation(businessRelatedInformation);
                // Generate verification code for sender registration 

                string VerficationCode = FAXER.PORTAL.Common.Common.GenerateVerificationCode(6);


                MobileRegistrationCodeVerificationViewModel RegistrationCodeVm = new MobileRegistrationCodeVerificationViewModel()
                {
                    Country = faxerInformation.Country,
                    UserId = faxerInformation.Id,
                    PhoneNo = faxerInformation.PhoneNumber,
                    RegistrationOf = RegistrationOf.Sender,
                    VerificationCode = VerficationCode,

                };
                MobileCommonServices _mobileCommonServices = new MobileCommonServices();
                _mobileCommonServices.AddRegistrationVerificationCode(RegistrationCodeVm);



                /// Author Umesh Timsina
                /// For Signup Problem
                try
                {
                    _sFaxerSignUp.SendRegistrationEmail(faxerInformation.FirstName, faxerInformation.Email, faxerInformation.PhoneNumber, faxerInformation.AccountNo, RegistrationCodeVm.VerificationCode);
                }
                catch (Exception ex)
                {

                }
                model.SenderId = faxerInformation.Id;
            }

            else
            {
                var data = dbContext.FaxerInformation.Where(x => x.Email == model.EmailAddress && x.PhoneNumber == model.MobileNumber).FirstOrDefault();
                model.CountryCode = model.CountryCode;
                model.CurrencyCode = model.CurrencyCode;
                model.CurrencySymbol = model.CurrencySymbol;
                model.SenderId = data.Id;

            }
            var context = UpdateSession(model.SenderId);

            return new ServiceResult<MobileBusinessSignUpModel>()
            {
                Data = model,
                Message = "AddedSuccess",
                Status = ResultStatus.OK,
                Token = context.Token
            };

        }

        private Context UpdateSession(int userId)
        {
            try
            {

                var newContext = new Context() { Id = 0, LastLogin = DateTime.Now, TimeOut = TimeSpan.FromMinutes(20), UserId = userId, Token = getToken() };
                dbContext.Context.Add(newContext);
                dbContext.SaveChanges();
                return newContext;
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, ErrorType.UnSpecified, "Token");

                return new Context();
            }
        }



        private string getToken()
        {
            return Guid.NewGuid().ToString();
        }



        ////Make Add For FaxerInformation


        ///Make Add for FaxerLogin



        //First Step:
        // Save To FaxerInfo According To  FaxerInformation Table
        //Note: FaxerInformation IsBusiness should be true



        //Second Step
        //Save To FaxerInfo According To FaxerLogin Table
    }
}