using FAXER.PORTAL.Areas.Mobile.Common;
using FAXER.PORTAL.Areas.Mobile.Models.Common;
using FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness;
using FAXER.PORTAL.Areas.Mobile.Services.Common;
using FAXER.PORTAL.Areas.Mobile.Services.KiiPayBusiness;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Mobile.Controllers.KiiPayBusiness
{

    // GET: Mobile/KiiPayBusinessMobileSignUp
    public class KiiPayBusinessMobileSignUpController : Controller
    {
        MobileKiiPayBusinessInformationServices kiiPayBusinessInformationServices = new MobileKiiPayBusinessInformationServices();
        MobileKiiPayBusinessUserPersonalInfoService kiiPayBusinessUserPersonalInfoServices = new MobileKiiPayBusinessUserPersonalInfoService();
        MobileKiiPayBusinessLoginServices kiiPayBusinessLoginServices = new MobileKiiPayBusinessLoginServices();
        MobileKiiPayBusinessWalletInformationServices kiiPayBusinessWalletInformationServices = new MobileKiiPayBusinessWalletInformationServices();
        MobileRegistrationVerificationCodeService registrationVerificationCodeService = new MobileRegistrationVerificationCodeService();
        public KiiPayBusinessMobileSignUpController()
        {
            kiiPayBusinessInformationServices = new MobileKiiPayBusinessInformationServices();
            kiiPayBusinessUserPersonalInfoServices = new MobileKiiPayBusinessUserPersonalInfoService();
            kiiPayBusinessLoginServices = new MobileKiiPayBusinessLoginServices();
            kiiPayBusinessWalletInformationServices = new MobileKiiPayBusinessWalletInformationServices();
            registrationVerificationCodeService = new MobileRegistrationVerificationCodeService();
        }

       
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Post([Bind(Include = KiiPayBusinessMobileSignUpModel.BindProperty)]KiiPayBusinessMobileSignUpModel model)
        {

            //New KiiPayBusinessInformation
            KiiPayBusinessInformation kiiPayBusinessInformation = new KiiPayBusinessInformation
            {
                BusinessCountry = model.CountryCode,
                BusinessLicenseNumber = model.BusinessRegistrationNumber,
                BusinessName = model.BusinessName,
                BusinessType = model.BusinessType,
                Email = model.EmailAddress,
                BusinessMobileNo = model.MobileNumber,
                BusinessOperationAddress1 = model.BusinessAddress,
                BusinessOperationAddress2 = model.BusinessAddressOptional,
                BusinessOperationCity = model.BusinessCity,
                BusinessOperationPostalCode = model.BusinessPostZipCode,
                BusinessOperationCountryCode = model.CountryCode,
                BusinessOperationState = "",
                BusinessRegisteredAddress1 = model.BusinessAddress,
                BusinessRegisteredAddress2 = model.BusinessAddressOptional,
                BusinessRegisteredCity = model.BusinessCity,
                BusinessRegisteredCountry = model.CountryCode,
                BusinessRegisteredPostalCode = model.BusinessPostZipCode,
                BusinessRegisteredState = "",
                CountryOfIncorporation = model.CountryCode,
                ContactPerson = "",
                PhoneNumber = model.MobileNumber,
                FaxNumber = "",
                RegistrationNumBer = model.BusinessRegistrationNumber,
                Website = "",
                BillIsIssuedToCustomer = model.IsBillIssued,
            };
            var kiiPayBusinessInformation_result = kiiPayBusinessInformationServices.Add(kiiPayBusinessInformation);

            DB.KiiPayBusinessUserPersonalInfo kiiPayBusinessUserPersonalInfo = new DB.KiiPayBusinessUserPersonalInfo()
            {
                FirstName = model.PerosnalFirstName,
                MiddleName = "",
                LastName = model.PerosnalLastName,
                KiiPayBusinessInformationId = kiiPayBusinessInformation_result.Id,
                AddressLine1 = model.BusinessAddress,
                AddressLine2 = model.BusinessAddressOptional,
                City = model.BusinessCity,
                Country = model.CountryCode,
                BirthDate = (DateTime)model.DateOfBirth,
                PostCodeORZipCode = model.BusinessPostZipCode,
            };
            var kiiPayBusinessUserPersonalInfo_result = kiiPayBusinessUserPersonalInfoServices.Add(kiiPayBusinessUserPersonalInfo);

            DB.KiiPayBusinessLogin kiiPayBusinessLogin = new DB.KiiPayBusinessLogin()
            {

                Username = model.EmailAddress,
                MobileNo = model.MobileNumber,
                PinCode = model.PassCode.Encrypt(),
                LoginFailCount = 0,
                Password = "",
                IsActive = true,
                ActivationCode = "",
                IsDeleted = false,
                KiiPayBusinessInformationId = kiiPayBusinessInformation_result.Id,
                LoginCode = ""
            };

            var kiiPayBusinessLogin_result = kiiPayBusinessLoginServices.Add(kiiPayBusinessLogin);


            #region Business Wallet Registration 

            DB.KiiPayBusinessWalletInformation kiiPayBusinessWalletInformation = new DB.KiiPayBusinessWalletInformation()
            {
                FirstName = model.PerosnalFirstName,
                MiddleName = "",
                LastName = model.PerosnalLastName,
                AddressLine1 = model.BusinessAddress,
                AddressLine2 = model.BusinessAddressOptional,
                City = model.BusinessCity,
                Country = model.CountryCode,
                PostalCode = model.BusinessPostZipCode,
                State = "",
                AutoTopUp = false,
                CardStatus = CardStatus.Active,
                DOB = (DateTime)model.DateOfBirth,
                Email = model.EmailAddress,
                Gender = model.Gender,
                IdCardNumber = "",
                IdCardType = "",
                IdIssuingCountry = "",
                IdExpiryDate = DateTime.Now,
                MobileNo = model.MobileNumber,
                PhoneNumber = model.MobileNumber,
                KiiPayUserPhoto = "",
                MFBCardPhoto = "",
                KiiPayBusinessInformationId = kiiPayBusinessInformation_result.Id

            };
            var kiiPayBusinessWalletInformation_result = kiiPayBusinessWalletInformationServices.Add(kiiPayBusinessWalletInformation);

            ///Update RegistrationCode


            var RegistrationCodeInfo = registrationVerificationCodeService.List().Data.Where(x => x.VerificationCode == model.RegisterCode && x.IsExpired == false).FirstOrDefault();

            RegistrationCodeInfo.UserId = kiiPayBusinessInformation_result.Id;


            registrationVerificationCodeService.Update(RegistrationCodeInfo);

            #endregion

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
                var Save_SuppliersSucess = kiiPayBusinessWalletInformationServices.SaveSuppliers(Suppliers);
            }


            return Json(new ServiceResult<KiiPayBusinessMobileSignUpModel>()
            {
                Data = model,
                Message = "AddedSuccess",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);

        }
       
    }
}