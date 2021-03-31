using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Mobile.Controllers.MoneyFex;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class SenderDocumentationController : Controller
    {
        SenderDocumentationServices _services = null;
        CommonServices _CommonServices = null;
        public SenderDocumentationController()
        {
            _services = new SenderDocumentationServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/SenderDocumentation
        public ActionResult Index(string Country = "", string City = "",
            string SenderName = "", string AccountNo = "", string Telephone = "",
            int Status = 3, string email = "", int? page = null, int PageSize = 10, int CurrentpageCount = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCities();
            ViewBag.Cities = new SelectList(Cities, "City", "City");
            ViewBag.SenderName = SenderName;
            ViewBag.AccountNo = AccountNo;
            ViewBag.Status = Status;
            ViewBag.Telephone = Telephone;
            ViewBag.City = City;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            var senderDocumentations = _services.GetSenderDocumentList(Country, City,
                SenderName, AccountNo, Status, Telephone, email, pageNumber, pageSize);
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = page ?? 1;
            ViewBag.NumberOfPage = 0;
            ViewBag.ButtonCount = 0;
            ViewBag.CurrentpageCount = CurrentpageCount;
            if (senderDocumentations.Count != 0)
            {
                var TotalCount = senderDocumentations.FirstOrDefault().TotalCount;
                int NumberOfPage = Common.Common.GetNumberOfPage(TotalCount, PageSize);
                ViewBag.NumberOfPage = NumberOfPage;
                var numberofbuttonshown = NumberOfPage - CurrentpageCount;
                ViewBag.ButtonCount = numberofbuttonshown;
            }
            SenderDocumentationAndSenderNote vm = new SenderDocumentationAndSenderNote();
            vm.SenderDocumentationViewModel = senderDocumentations;
            vm.TransactionStatementNote = new TransactionStatementNoteViewModel();
            Log.Write(DateTime.Now.ToString(), ErrorType.UnSpecified, "Sender Document Time");
            return View(vm);

        }
        public JsonResult Delete(int id)
        {


            if (id != 0)
            {
                _services.Delete(id);
                return Json(new
                {
                    Data = true,
                    Message = "Deleted Sucessfully"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Data = false,
                    Message = "Something went wrong. Please try again!"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UploadSenderDocumentation(int Id = 0, string Country = "", string city = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name", Country);


            var Cities = _CommonServices.GetCities();

            var sender = new List<SenderListDropDown>();

            SenderDocumentationViewModel vm = new SenderDocumentationViewModel();
            if (Id != 0)
            {
                vm = _services.GetUploadedDocumentInfo(Id);
                ViewBag.SenderId = vm.SenderId;
                Cities = _CommonServices.GetCities(vm.Country);
                sender = _CommonServices.GetSenderList().Where(x => x.Country == vm.Country).ToList();
            }
            ViewBag.Cities = new SelectList(Cities, "City", "City", vm.City);
            ViewBag.Sender = new SelectList(sender, "SenderId", "SenderName", vm.SenderId);
            //if (!string.IsNullOrEmpty(Country))
            //{
            //    var data = _CommonServices.GetSenderList(city).Where(x => x.Country == Country).ToList();
            //    ViewBag.Sender = new SelectList(data, "SenderId", "SenderName",);
            //}
            var IssuingCountries = _CommonServices.GetCountries();
            ViewBag.IssuingCountries = new SelectList(IssuingCountries, "Code", "Name", vm.IssuingCountry);
            return View(vm);
        }
        [HttpPost]
        public ActionResult UploadSenderDocumentation([Bind(Include = SenderDocumentationViewModel.BindProperty)] SenderDocumentationViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            ViewBag.IssuingCountries = new SelectList(Countries, "Code", "Name", vm.IssuingCountry);

            var Cities = _CommonServices.GetCities();
            ViewBag.Cities = new SelectList(Cities, "City", "City", vm.City);

            //var sender = _services.GetSender(vm.Country, vm.City);

            var sender = _CommonServices.GetSenderList(vm.City).Where(x => x.Country == vm.Country).ToList();

            ViewBag.Sender = new SelectList(sender, "SenderId", "SenderName", vm.SenderId);


            //if (!string.IsNullOrEmpty(Country))
            //{
            //    var data = _CommonServices.GetSenderList(city).Where(x => x.Country == Country).ToList();
            //    ViewBag.Sender = new SelectList(data, "Id", "Name");
            //}
            string DocumentUrl = null;
            string DocumentUrlTwo = null;
            var senderBusinessDocumentation = _services.GetDocumentDetails(vm.Id);
            if (senderBusinessDocumentation != null)
            {
                DocumentUrl = senderBusinessDocumentation.DocumentPhotoUrl;
                DocumentUrlTwo = senderBusinessDocumentation.DocumentPhotoUrlTwo;
            }

            if (ModelState.IsValid)
            {

                if (vm.Id == 0)
                {
                    var CurrentDate = DateTime.Now;
                    if (vm.DocumentType == DocumentType.Select)
                    {
                        ModelState.AddModelError("DocumentType", "Select Document Type.");
                        return View(vm);
                    }
                    if (vm.DocumentExpires == DocumentExpires.Select)
                    {
                        ModelState.AddModelError("DocumentExpires", "Select Document Expires.");
                        return View(vm);
                    }
                    if (vm.DocumentExpires == DocumentExpires.Yes && vm.ExpiryDate == null)
                    {
                        ModelState.AddModelError("ExpiryDate", "Enter Expiry Date. ");
                        return View(vm);
                    }

                    if (vm.ExpiryDate < CurrentDate)
                    {
                        ModelState.AddModelError("ExpiryDate", "Document has expired. ");
                        return View(vm);
                    }
                    if (vm.DocumentPhotoUrl == null)
                    {
                        ModelState.AddModelError("DocumentPhotoUrl", "Choose A File. ");
                        return View(vm);
                    }
                }

                if (vm.Status == null)
                {
                    ModelState.AddModelError("Status", "Select Status.");
                    return View(vm);
                }
                if (vm.Status == DocumentApprovalStatus.Disapproved)
                {
                    if (vm.ReasonForDisApproval == ReasonForDisApproval.Select)
                    {
                        ModelState.AddModelError("ReasonForDisApproval", "Select reason for disapproval.");
                        return View(vm);
                    }
                    if (vm.ReasonForDisApproval == ReasonForDisApproval.Others)
                    {
                        ModelState.AddModelError("ReasonForDisApprovalByAdmin", "Enter reason for disapproval.");
                        return View(vm);

                    }

                }

                if (Request.Files.Count < 2)
                {
                    var identificationdoc = Request.Files["DocumentPhotoUrl"];
                    var identificationdoc2 = Request.Files["DocumentPhotoUrlTwo"];

                }
                var IdentificationDoc = Request.Files["DocumentPhotoUrl"];
                var Identificationdoc2 = Request.Files["DocumentPhotoUrlTwo"];
                var DocumentData = new ServiceResult<string>();
                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    DocumentData = Common.Common.GetDocumentPath(IdentificationDoc);
                    if (DocumentData.Status == ResultStatus.OK)
                    {
                        vm.DocumentPhotoUrl = DocumentData.Data;
                    }
                    else
                    {
                        ModelState.AddModelError("DocumentPhotoUrl", DocumentData.Message);
                        return View(vm);
                    }

                }
                if (Identificationdoc2 != null && Identificationdoc2.ContentLength > 0)
                {
                    DocumentData = Common.Common.GetDocumentPath(Identificationdoc2);
                    if (DocumentData.Status == ResultStatus.OK)
                    {
                        vm.DocumentPhotoUrlTwo = DocumentData.Data;
                    }
                    else
                    {
                        ModelState.AddModelError("DocumentPhotoUrlTwo", DocumentData.Message);
                        return View(vm);
                    }
                }
                var existingData = _services.GetUploadedDocumentInfo(vm.Id);
                if (vm.Id == 0)
                {
                    vm.IsUploadedFromSenderPortal = false;
                    _services.UploadDocument(vm);
                }
                else
                {
                    if (vm.DocumentPhotoUrl == null)
                    {
                        vm.DocumentPhotoUrl = DocumentUrl;
                    }
                    if (vm.DocumentPhotoUrlTwo == null)
                    {
                        vm.DocumentPhotoUrlTwo = DocumentUrlTwo;
                    }
                    _services.UpdateDocument(vm);
                }
                #region send email

                if (vm.Status == DocumentApprovalStatus.Approved && (existingData != null && existingData.Status != DocumentApprovalStatus.Approved))
                {
                    _services.SendIdentiVerificationCompletedEmail(vm.SenderId);
                }


                if (vm.Status == DocumentApprovalStatus.InProgress)
                {
                    _services.SendIdentiVerificationInProgressEmail(vm.SenderId);
                }
                if (vm.Status == DocumentApprovalStatus.Disapproved)
                {
                    string Reason = "";
                    if (vm.ReasonForDisApproval != ReasonForDisApproval.Others)
                    {
                        Reason = Common.Common.GetEnumDescription(vm.ReasonForDisApproval);

                    }
                    else
                    {
                        Reason = vm.ReasonForDisApprovalByAdmin;
                    }
                    _services.SendIdentiVerificationFailedEmail(vm.SenderId, Reason);
                }

                #endregion
                return RedirectToAction("Index", "SenderDocumentation");
            }

            return View(vm);
        }
        public JsonResult GetSenderNote(int SenderId)
        {

            var result = _services.TransactionStatementNote(SenderId);
            _services.UpdateTransactionStatementNote(SenderId);
            return Json(new
            {
                result
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult SaveNote(TransactionStatementNoteViewModel vm)
        {

            vm.NoteType = NoteType.SenderDocumentation;
            _services.SaveDocumentationNote(vm);
            return Json(new
            {
                Data = true
            }, JsonRequestBehavior.AllowGet);
            //return RedirectToAction("Index", "SenderDocumentation");
        }
        public JsonResult GetsenderByCountry(string Country = "", string City = "")
        {
            CommonServices commonServices = new CommonServices();
            var data = commonServices.GetSenderList(City).Where(x => x.Country == Country).ToList();

            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAccountNumber(int SenderId = 0)
        {
            string Accountnumber = _CommonServices.GetSenderAccountNoBySenderId(SenderId);
            return Json(new
            {
                Data = Accountnumber
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDoumentImageUrl(int Id)
        {

            var senderDocumentURL = _services.GetUploadedDocumentInfo(Id).DocumentPhotoUrl ?? "";
            return Json(new
            {

                URL = senderDocumentURL
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetSenders(SelectSearchParam param)
        {
            var senderInformations = _CommonServices.GetSenderList().ToList();
            if (param.country != "All" && !string.IsNullOrEmpty(param.country))
            {
                senderInformations = senderInformations.Where(x => x.Country == param.country.Trim()).ToList();
            }
            if (param.query == null) { param.query = ""; }
            var senders = (from c in senderInformations.Where(x => x.senderName.ToLower().Contains(param.query.ToLower()))
                           select new SelectDropDownVm()
                           {
                               Id = c.senderId,
                               text = c.senderName

                           }).ToList();

            return Json(new
            {
                senders
            }, JsonRequestBehavior.AllowGet);

        }


    }
}