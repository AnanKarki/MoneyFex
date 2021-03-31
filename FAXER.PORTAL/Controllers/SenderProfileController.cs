using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Controllers
{
    public class SenderProfileController : Controller
    {
        SSenderProfileService _senderProfileService = null;
        public SenderProfileController()
        {
            _senderProfileService = new SSenderProfileService();
        }

        // GET: SenderProfile

        [HttpGet]
        public ActionResult Index()
        {
            Services.SFaxerSignUp service = new Services.SFaxerSignUp();
            var model = service.GetInformation(Common.FaxerSession.LoggedUserName);

            var idInfo = service.GetIdentificationDetail(model.Id);

            //if (idInfo != null)
            //{

            //    model.IdCardNumber = idInfo.IdentityNumber;
            //    model.IdCardExpiringDate = (DateTime)idInfo.ExpiryDate;
            //    model.IssuingCountry = idInfo.IssuingCountry;
            //    model.IdCardType = service.GetIdType(idInfo.IdentificationTypeId);
            //}
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
                string CountryPhoneCode = Common.Common.GetCountryPhoneCode(model.Country);

                model.PhoneNumber = "*****" + model.PhoneNumber.Substring(model.PhoneNumber.Length - 2, 2);
                model.PhoneNumber = CountryPhoneCode + " " + model.PhoneNumber;
            }

            if (model.DateOfBirth.HasValue)
            {
                string DOB = model.DateOfBirth.ToFormatedString();
                ViewBag.DOB = "**/**/**" + DOB.Substring(DOB.Length - 2, 2);


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
            model.Address1 = model.Address1 + " " + model.Address2 + " " + model.City + " " + model.PostalCode;
            model.IssuingCountry = "*****";
            if (!string.IsNullOrEmpty(model.Email))
            {
                model.Email = model.Email.Split('@')[0].Substring(0, 1) + "*******" + model.Email.Split('@')[1];
            }
            model.Country = Common.Common.GetCountryName(model.Country);

            Common.FaxerSession.IDhasbeenExpired = false;
            ViewBag.IsPinCodeSend = 0;
            ViewBag.PinCode = "";
            return View(model);

        }

        [HttpPost]
        public ActionResult Index(FaxerInformation model)
        {
            if (ModelState.IsValid)
            {
                if (model.DateOfBirth.HasValue)
                {
                    string DOB = model.DateOfBirth.ToFormatedString();
                    ViewBag.DOB = "**/**/**" + DOB.Substring(DOB.Length - 2, 2);


                }
                ViewBag.IsPinCodeSend = 1;
                //ViewBag.UserEnterPinCode = GetMobilePin();
                ViewBag.PinCode = GetMobilePin();
                return View(model);
                return RedirectToAction("SenderProfileEdit");
            }


            return View(model);
        }

        [HttpGet]
        public ActionResult SenderProfileEdit(string PinCode)
        {

            ViewBag.IsPinCodeSend = 1;
            if (PinCode != GetMobilePin())
            {
                return RedirectToAction("Index");
            }
            ViewBag.Countries = new SelectList(Common.Common.GetCountries(), "CountryCode", "CountryName ");
            ViewBag.IdCardTypes = new SelectList(Common.Common.GetIdTypes(), "Id", "CardType");
            var vm = _senderProfileService.GetSenderProfileEdit();
            vm.UserEnterPinCode = PinCode;
            return View(vm);
        }
        [HttpPost]
        public ActionResult SenderProfileEdit([Bind(Include = SenderProfileEditViewModel.BindProperty)]SenderProfileEditViewModel model)
        {
            ViewBag.Countries = new SelectList(Common.Common.GetCountries(), "CountryCode", "CountryName ");
            ViewBag.IdCardTypes = new SelectList(Common.Common.GetIdTypes(), "Id", "CardType");

            ViewBag.IsPinCodeSend = 0;
            if (ModelState.IsValid)
            {

                //var IDCardExpiryDate = new DateTime();
                //try
                //{

                //    IDCardExpiryDate = new DateTime().AddDays(model.Day).AddMonths((int)model.Month).AddYears(model.Year);
                //}
                //catch (Exception)
                //{

                //    ModelState.AddModelError("IdCardNumber", "Enter a valid date");
                //    return View(model);
                //}

                //if (IDCardExpiryDate <= DateTime.Now)
                //{


                //    ModelState.AddModelError("IdCardNumber", "Your Identification has been expired!");
                //    return View(model);
                //}


                var validateage = Common.DateUtilities.ValidateAge(model.DateOfBirth);
                SmsApi smsApi = new SmsApi();
                bool validMobile = smsApi.IsValidMobileNo(Common.Common.GetCountryPhoneCode(model.Country) + model.MobileNumber);

                if (!validateage)
                {
                    ModelState.AddModelError("DateOfBirth", "You must be 18 years of age or above to sign up to our services!");
                    return View(model);
                }
                if (!validMobile)
                {
                    ModelState.AddModelError("MobileNumber", "Mobile number is invalid");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.UserEnterPinCode))
                {

                    model.PinCode = GetMobilePin();
                    ViewBag.IsPinCodeSend = 1;

                    return View(model);
                }
                else
                {
                    //  check if the pincode in session and model.enerpincode are equal if not then show error message in popup
                    string sentPinCode = _senderProfileService.GetMobilePinCode();
                    if (model.UserEnterPinCode != sentPinCode)
                    {
                        ViewBag.IsPinCodeSend = 1;
                        ModelState.AddModelError("UserEnterPinCode", " Invalid Pincode");
                        return View(model);
                    }
                    //model.IdExpiringDate = IDCardExpiryDate;
                    _senderProfileService.UpdateSenderInformation(model);
                    Common.FaxerSession.SentMobilePinCode = "";
                    return RedirectToAction("Index", "SenderProfile");
                }
            }
            return View(model);
        }

        public ActionResult IdentityVerificationTable()
        {
            int senderId = Common.FaxerSession.SenderId;
            List<IdentificationDetailModel> vm = _senderProfileService.GetIdentitificationDetailsOfSender(senderId);

            ViewBag.IdCount = vm.Count();
            ViewBag.IsPinCodeSend = 0;
            ViewBag.PinCode = "";

            return View(vm);
        }
        [HttpPost]
        public ActionResult IdentityVerificationTable(int documentationId = 0)
        {
            int senderId = Common.FaxerSession.SenderId;
            List<IdentificationDetailModel> vm = _senderProfileService.GetIdentitificationDetailsOfSender(senderId);
            ViewBag.IdCount = vm.Count();
            ViewBag.IsPinCodeSend = 1;
            //ViewBag.UserEnterPinCode = GetMobilePin();
            ViewBag.PinCode = GetMobilePin();
            ViewBag.SenderBusinessDocumentationId = documentationId;

            return View(vm);
        }
        public ActionResult IdentityVerification()
        {
            GetCountries();
            GetIdTypes();
            int senderId = Common.FaxerSession.LoggedUser.Id;
            if (_senderProfileService.GetIdentitificationDetailsOfSender(senderId).Count() > 0)
            {
                return RedirectToAction("IdentityVerificationTable");
            }
            return View();
        }

        public void GetCountries()
        {

            var result = (from c in Common.Common.GetCountriesForDropDown()
                          select new IssuingCountryModel()
                          {
                              IssuingCountry = c.CountryCode,
                              IssuingCountryName = c.CountryName,
                          }).ToList();
            ViewBag.Countries = new SelectList(result, "IssuingCountry", "IssuingCountryName");
        }
        public void GetIdTypes()
        {
            var IdenticationTypes = (from c in Common.Common.GetIdTypes()
                                     select new IdentificationTypeModel()
                                     {

                                         IdentificationTypeId = c.Id,
                                         Name = c.CardType
                                     }).ToList();
            ViewBag.IdenticationTypes = new SelectList(IdenticationTypes.ToList(), "IdentificationTypeId", "Name");
        }
        [HttpPost]
        public ActionResult IdentityVerification([Bind(Include = IdentificationDetailModel.BindProperty)]IdentificationDetailModel model)
        {

            GetCountries();
            GetIdTypes();
            if (ModelState.IsValid)
            {
                var ExpiryDate = new DateTime(model.Year, (int)model.Month, model.Day);
                if (ExpiryDate < DateTime.Now)
                {
                    ModelState.AddModelError("ExpiryDate", "ID has been expired");
                    return View(model);
                }
                if (Request.Files.Count == 0)
                {
                    ModelState.AddModelError("document", "Choose file to upload");
                    return View(model);
                }
                SenderDocumentationServices _senderDocumentationServices = new SenderDocumentationServices();
                if (Request.Files.Count < 1)
                {
                    var identificationdoc = Request.Files["Document"];
                }
                string identificationDocPath = "";
                string DocumentPhotoUrl = "";
                var IdentificationDoc = Request.Files["Document"];

                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    var allowedExtensions = new string[] { "gif", "jpg", "png", "jpeg", "pdf" };
                    int fileLength = IdentificationDoc.FileName.Split('.').Length;
                    var extension = IdentificationDoc.FileName.Split('.')[fileLength - 1];
                    extension = extension.ToLower();
                    identificationDocPath = Guid.NewGuid() + "." + IdentificationDoc.FileName.Split('.')[fileLength - 1];

                    if (allowedExtensions.Contains(extension))
                    {
                        try
                        {
                            IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + identificationDocPath);
                        }
                        catch (Exception ex)
                        {

                        }
                        DocumentPhotoUrl = "/Documents/" + identificationDocPath;
                    }
                    else
                    {
                        ModelState.AddModelError("document", "File type not allowed to upload. ");
                        return View(model);
                    }

                }
                else
                {


                    ModelState.AddModelError("document", "Upload Id");
                    return View(model);
                }
                int senderId = Common.FaxerSession.LoggedUser.Id;
                string DocumentName = IdentificationDoc.FileName.Split('.')[0];
                CommonServices _CommonServices = new CommonServices();
                var senderInfo = _CommonServices.GetSenderInfo(senderId);

                SenderDocumentationViewModel vm = new SenderDocumentationViewModel()
                {
                    SenderId = senderId,
                    AccountNo = senderInfo.AccountNo,
                    City = senderInfo.City,
                    Country = senderInfo.Country,
                    CreatedDate = DateTime.Now,
                    DocumentName = model.IdentityNumber,
                    DocumentExpires = DocumentExpires.Yes,
                    DocumentType = DocumentType.Compliance,
                    DocumentPhotoUrl = DocumentPhotoUrl,
                    SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName,
                    Status = DocumentApprovalStatus.InProgress,
                    IsUploadedFromSenderPortal = true,
                    IssuingCountry = model.IssuingCountry,
                    IdentificationTypeId = model.IdentificationTypeId,
                    IdentityNumber = model.IdentityNumber,
                    ExpiryDate = ExpiryDate

                };
                _senderDocumentationServices.UploadDocument(vm);

                _senderDocumentationServices.SendIdentiVerificationInProgressEmail(senderId);


                return RedirectToAction("IdentityVerificationProgress");
            }

            return View(model);
        }

        public ActionResult IdentityVerificationUpdate(string PinCode = "", int documentId = 0)
        {

            GetCountries();
            GetIdTypes();
            ViewBag.IsPinCodeSend = 1;
            if (PinCode != GetMobilePin())
            {
                return RedirectToAction("IdentityVerificationTable");
            }
            int senderId = Common.FaxerSession.SenderId;

            IdentificationDetailModel vm = _senderProfileService.GetIdentitificationDetailsOfSender(senderId).
                Where(x => x.SenderBusinessDocumentationId == documentId).FirstOrDefault();

            return View(vm);
        }
        [HttpPost]
        public ActionResult IdentityVerificationUpdate([Bind(Include = IdentificationDetailModel.BindProperty)]IdentificationDetailModel model)
        {
            GetCountries();
            GetIdTypes();

            if (ModelState.IsValid)
            {
                var ExpiryDate = new DateTime(model.Year, (int)model.Month, model.Day);
                if (ExpiryDate < DateTime.Now)
                {
                    ModelState.AddModelError("ExpiryDate", "ID has been expired");
                    return View(model);
                }
                if (Request.Files.Count == 0)
                {
                    ModelState.AddModelError("document", "Choose file to upload");
                    return View(model);
                }
                SenderDocumentationServices _senderDocumentationServices = new SenderDocumentationServices();

                string DocumentPhotoUrl = "";
                string DocumentPhotoUrlTwo = "";
                var senderBusinessDocumentation = _senderDocumentationServices.GetDocumentDetails(model.SenderBusinessDocumentationId);
                if (senderBusinessDocumentation != null)
                {
                    DocumentPhotoUrl = senderBusinessDocumentation.DocumentPhotoUrl;
                    DocumentPhotoUrlTwo = senderBusinessDocumentation.DocumentPhotoUrlTwo;
                }
                if (Request.Files.Count < 1)
                {
                    var identificationdoc = Request.Files["Document"];
                }
                var IdentificationDoc = Request.Files["Document"];
                var IdentificationDocTwo = Request.Files["Document2"];

                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    var DocumentData = Common.Common.GetDocumentPath(IdentificationDoc);
                    if (DocumentData.Status == ResultStatus.OK)
                    {
                        DocumentPhotoUrl = DocumentData.Data;
                    }
                    else
                    {
                        ModelState.AddModelError("document", DocumentData.Message);
                        return View(model);
                    }
                }
                else
                {
                    ModelState.AddModelError("document", "Upload Id");
                    return View(model);
                }

                if (IdentificationDocTwo != null && IdentificationDocTwo.ContentLength > 0)
                {
                    var DocumentData = Common.Common.GetDocumentPath(IdentificationDocTwo);
                    if (DocumentData.Status == ResultStatus.OK)
                    {
                        DocumentPhotoUrlTwo = DocumentData.Data;
                    }
                    else
                    {
                        ModelState.AddModelError("document2", DocumentData.Message);
                        return View(model);
                    }
                }


                int senderId = Common.FaxerSession.LoggedUser.Id;
                string DocumentName = IdentificationDoc.FileName.Split('.')[0];
                CommonServices _CommonServices = new CommonServices();
                var senderInfo = _CommonServices.GetSenderInfo(senderId);

                SenderDocumentationViewModel vm = new SenderDocumentationViewModel()
                {
                    SenderId = senderId,
                    AccountNo = senderInfo.AccountNo,
                    City = senderInfo.City,
                    Country = senderInfo.Country,
                    CreatedDate = DateTime.Now,
                    DocumentName = model.IdentityNumber,
                    DocumentExpires = DocumentExpires.Yes,
                    DocumentType = DocumentType.Compliance,
                    DocumentPhotoUrl = DocumentPhotoUrl,
                    DocumentPhotoUrlTwo = DocumentPhotoUrlTwo,
                    SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName,
                    Status = DocumentApprovalStatus.InProgress,
                    IsUploadedFromSenderPortal = true,
                    IssuingCountry = model.IssuingCountry,
                    IdentificationTypeId = model.IdentificationTypeId,
                    IdentityNumber = model.IdentityNumber,
                    ExpiryDate = ExpiryDate,
                    Id = model.SenderBusinessDocumentationId

                };
                _senderDocumentationServices.UpdateDocument(vm);


                return RedirectToAction("IdentityVerificationProgress");
            }
            return View(model);
        }
        public ActionResult IdentityVerificationProgress()
        {
            return View();
        }

        public ActionResult DeleteIdentityVerification(int documentationId)
        {
            _senderProfileService.DeleteIdentityVerification(documentationId);
            return RedirectToAction("IdentityVerificationTable", "SenderProfile");
        }

        public string GetMobilePin(string PhoneNo = "")
        {
            //if session null generate code and return  else return value in session
            string code = "";
            if (Common.FaxerSession.SentMobilePinCode == null || Common.FaxerSession.SentMobilePinCode == "")
            {
                code = Common.Common.GenerateRandomDigit(6);
                _senderProfileService.SetMobilePinCode(code);

                SmsApi smsApi = new SmsApi();
                var msg = smsApi.GetAddressUpdateMessage(code);
                string UserPhoneNo = Common.FaxerSession.LoggedUser.CountryPhoneCode + Common.FaxerSession.LoggedUser.PhoneNo;
                smsApi.SendSMS(UserPhoneNo, msg);
            }
            else
            {
                code = Common.FaxerSession.SentMobilePinCode;
            }
            string mobilePinCode = code;
            return mobilePinCode;
        }

        public JsonResult GetCountryPhoneCode(string CountryCode)
        {

            string PhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {

                PhoneCode = PhoneCode,


            }, JsonRequestBehavior.AllowGet);
        }

    }
}