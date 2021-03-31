using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class BusinessDocumentationController : Controller
    {
        BusinessDocumentationServices _services = null;
        CommonServices _CommonServices = null;

        public BusinessDocumentationController()
        {
            _services = new BusinessDocumentationServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/BusinessDocumentation
        public ActionResult Index(string Country = "", string City = "", string SenderName = "", string CustomerNo = "",
            string DocumentName = "", string staffName = "", string DateRange = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            ViewBag.City = City;
            ViewBag.SenderName = SenderName;
            ViewBag.CustomerNo = CustomerNo;
            ViewBag.DocumentName = DocumentName;
            ViewBag.staff = staffName;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<BusinessDocumentationViewModel> vm = _services.GetBusinessDocuments(Country, City, SenderName, CustomerNo, DocumentName, staffName, DateRange).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }
        public ActionResult UploadDocument(int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            var Business = _services.GetBuisnessSender();
            ViewBag.Business = new SelectList(Business, "Id", "Name");

            BusinessDocumentationViewModel vm = new BusinessDocumentationViewModel();
            if (Id != 0)
            {
                vm = _services.GetUploadedDocumentInfo(Id);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult UploadDocument([Bind(Include = BusinessDocumentationViewModel.BindProperty)]BusinessDocumentationViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            var Business = _services.GetBuisnessSender();
            ViewBag.Business = new SelectList(Business, "Id", "Name");
           
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
                if (vm.Id != 0)
                {
                    if (vm.DocumentPhotoUrl == null)
                    {
                        vm.DocumentPhotoUrl = DocumentUrl;
                    }
                    if (vm.DocumentPhotoUrlTwo == null)
                    {
                        vm.DocumentPhotoUrlTwo = DocumentUrlTwo;
                    }
                }
                else
                {
                    if (vm.DocumentPhotoUrl == null)
                    {

                        ModelState.AddModelError("DocumentPhotoUrl", "Choose a file. ");
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


                if (vm.Id == 0)
                {
                    _services.UploadDocument(vm);
                }
                else
                {

                    _services.UpdateDocument(vm);
                }
                return RedirectToAction("Index", "BusinessDocumentation");
            }

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

        public JsonResult GetsenderByCountry(string Country = "", string City = "")
        {
            var data = _CommonServices.GetBusinessSenderList(City).Where(x => x.Country == Country).ToList();
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
    }
}