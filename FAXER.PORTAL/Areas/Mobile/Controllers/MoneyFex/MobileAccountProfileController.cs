using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Mobile.Controllers.MoneyFex
{
    public class MobileAccountProfileController : Controller
    {
        // GET: Mobile/MobileAccountProfile
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public JsonResult GetAccountAndIdentityDetails(int senderId)
        {

            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                SFaxerSignUp service = new SFaxerSignUp();
                var model = service.GetInformation(senderId);

                var idInfo = service.GetIdentificationDetail(model.Id);
                string DateOfBirth = "";
                if (idInfo != null)
                {

                    model.IdCardNumber = idInfo.IdentityNumber;
                    model.IdCardExpiringDate = (DateTime)idInfo.ExpiryDate;
                    model.IssuingCountry = idInfo.IssuingCountry;
                    model.IdCardType = service.GetIdType(idInfo.IdentificationTypeId);
                }
                if (!string.IsNullOrEmpty(model.IdCardNumber))
                {
                    int IDCardNumberCount = model.IdCardNumber.Count();
                    try
                    {
                        model.IdCardNumber = "****" + model.IdCardNumber.Substring(IDCardNumberCount - 2, 2);
                    }
                    catch (Exception)
                    {


                    }


                    if (!string.IsNullOrEmpty(model.IdCardType))
                    {

                        model.IdCardType = "*****";

                    }
                }

                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {

                    model.PhoneNumber = "*****" + model.PhoneNumber.Substring(model.PhoneNumber.Length - 2, 2);

                }

                if (!string.IsNullOrEmpty(model.Address1))
                {

                    model.Address1 = model.Address1.Substring(0, 1) + "*****" + model.Address1.Right(1);
                }
                if (!string.IsNullOrEmpty(model.Address2))
                {

                    model.Address2 = model.Address2.Substring(0, 1) + "*****" + model.Address2.Right(1);
                }
                if (!string.IsNullOrEmpty(model.City))
                {

                    model.City = model.City.Substring(0, 1) + "*****" + model.City.Right(1);
                }
                if (!string.IsNullOrEmpty(model.PostalCode))
                {

                    model.PostalCode = model.PostalCode.Substring(0, 1) + "*****" + model.PostalCode.Right(1);
                }
                if (model.DateOfBirth.HasValue)
                {
                    string DOB = model.DateOfBirth.ToFormatedString();
                    DateOfBirth = "**/**/**" + DOB.Substring(DOB.Length - 2, 2);
                }
                model.Address1 = model.Address1 + " " + model.Address2 + " " + model.City + " " + model.PostalCode;
                model.IssuingCountry = "*****";
                if (!string.IsNullOrEmpty(model.Email))
                {
                    model.Email = model.Email.Split('@')[0].Substring(0, 1) + "*******" + model.Email.Split('@')[1];
                }
                model.Country = FAXER.PORTAL.Common.Common.GetCountryName(model.Country);

                //Common.FaxerSession.IDhasbeenExpired = false;
                //ViewBag.IsPinCodeSend = 0;
                //ViewBag.PinCode = "";

                //model.Country = cO

                //SCommon
                var result = new AccountProfileViewModel();
                result.AccountNumber = model.AccountNo;
                result.AdditionalAddress = model.Address2;
                result.Address = model.Address1;
                result.Country = model.Country;
                result.City = model.City;
                result.PostalCode = model.PostalCode;
                //result.DateOfBirth = Convert.ToString(model.DateOfBirth);
                result.DateOfBirth = DateOfBirth;
                result.Email = model.Email;
                result.Name = model.FirstName + " " + model.MiddleName + "" + model.LastName;
                result.TelephoneWalletNumber = model.PhoneNumber;
                result.FaxerInfoId = model.Id;
                if (idInfo != null)
                {
                    result.ExpiryDate = Convert.ToString(idInfo.ExpiryDate);
                    result.IdNumber = idInfo.IdentityNumber;
                    result.IdType = Enum.GetName(typeof(DocumentType), idInfo.DocumentType);
                    result.IsIdUploaded = idInfo.IsUploadedFromSenderPortal;
                    result.IssuingCountry = idInfo.IssuingCountry;
                }

                return Json(new ServiceResult<AccountProfileViewModel>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<AccountProfileViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetAccountDetails(int senderId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                SFaxerSignUp service = new SFaxerSignUp();
                var model = service.GetInformation(senderId);
                var result = new AccountProfileViewModel();
                result.FaxerInfoId = model.Id;
                result.AccountNumber = model.AccountNo;
                result.AdditionalAddress = model.Address2;
                result.Address = model.Address1;
                result.Country = model.Country;
                result.City = model.City;
                result.PostalCode = model.PostalCode;
                result.DateOfBirth = Convert.ToString(model.DateOfBirth);
                result.Email = model.Email;
                result.Name = model.FirstName + " " + model.MiddleName + "" + model.LastName;
                result.TelephoneWalletNumber = model.PhoneNumber;
                result.SenderId = model.Id;

                return Json(new ServiceResult<AccountProfileViewModel>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<AccountProfileViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public JsonResult AccountProfileUpdate([Bind(Include = AccountProfileViewModel.BindProperty)]AccountProfileViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                SSenderProfileService _senderProfile = new SSenderProfileService();
                SenderProfileEditViewModel vm = new SenderProfileEditViewModel()
                {
                    Address = model.Address,
                    AddressLine2 = model.AdditionalAddress,
                    City = model.City,
                    Country = model.Country,
                    DateOfBirth = model.DateOfBirth.ToDateTime(),
                    EmailAddress = model.Email,
                    MobileNumber = model.TelephoneWalletNumber,
                    Id = model.FaxerInfoId,
                    PostCode = model.PostalCode,




                };
                _senderProfile.UpdateSenderInformation(vm);

                return Json(new ServiceResult<AccountProfileViewModel>()
                {
                    Data = model,
                    Message = "AddedSuccess",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<AccountProfileViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }

        }


    }
    public class AccountProfileViewModel
    {
        public const string BindProperty = "FaxerInfoId,Name ,AccountNumber , DateOfBirth,Address , Country,AdditionalAddress ,TelephoneWalletNumber , Email, IdType" +
            ", IdNumber,ExpiryDate ,IssuingCountry ,IsIdUploaded ,City , PostalCode,SenderId ";
        public int  FaxerInfoId { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }
        public string DateOfBirth { get; set; }
        public string Address { get; set; }
        public string Country { get; set; }
        public string AdditionalAddress { get; set; }
        public string TelephoneWalletNumber { get; set; }
        public string Email { get; set; }
        public string IdType { get; set; }
        public string IdNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string IssuingCountry { get; set; }
        public bool IsIdUploaded { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public int SenderId { get; set; }
    }
}