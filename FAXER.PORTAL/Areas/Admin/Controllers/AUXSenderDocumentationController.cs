using Antlr.Runtime.Tree;
using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AUXSenderDocumentationController : Controller
    {

        AuxSenderDocumentationServices _auxDocumentationservices = null;
        CommonServices _CommonServices = null;

        public AUXSenderDocumentationController()
        {
            _auxDocumentationservices = new AuxSenderDocumentationServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/AUXSenderDocumentation
        public ActionResult Index(string Country = "", string City = "",
            string SenderName = "", string CustomerNo = "", string DocumentName = "", string Uploader = "",
            int Status = 3, string DateRange = "", int? page = null)
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
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.SenderName = SenderName;
            ViewBag.CustomerNo = CustomerNo;
            ViewBag.DocumentName = DocumentName;
            ViewBag.Uploader = Uploader;
            ViewBag.Status = Status;
            ViewBag.DateRange = DateRange;
            IPagedList<BusinessDocumentationViewModel> vm = _auxDocumentationservices.GetAuxSenderDocuments(Country, City, SenderName, CustomerNo, DocumentName, Uploader,
                Status, DateRange).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }


        public ActionResult UploadAuxSenderDocument(int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var IssuingCountries = _CommonServices.GetCountries();
            ViewBag.IssuingCountries = new SelectList(IssuingCountries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            var Business = _CommonServices.GetSenderRegisteredByAuxAgent();
            ViewBag.Business = new SelectList(Business, "senderId", "senderName");

            BusinessDocumentationViewModel vm = new BusinessDocumentationViewModel(); ;
            if (Id != 0)
            {
                vm = _auxDocumentationservices.GetUploadedDocumentInfo(Id);
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult UploadAuxSenderDocument([Bind(Include = BusinessDocumentationViewModel.BindProperty)]BusinessDocumentationViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var IssuingCountries = _CommonServices.GetCountries();
            ViewBag.IssuingCountries = new SelectList(IssuingCountries, "Code", "Name");

            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            var Business = _auxDocumentationservices.GetAuxSender();
            ViewBag.Business = new SelectList(Business, "Id", "Name");

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

                if (Request.Files.Count < 1)
                {
                    var identificationdoc = Request.Files["DocumentPhotoUrl"];

                }
                string identificationDocPath = "";
                var IdentificationDoc = Request.Files["DocumentPhotoUrl"];
                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    var allowedExtensions = new string[] { "gif", "jpg", "png", "jpeg", "pdf" };
                    var extension = IdentificationDoc.FileName.Split('.')[1];
                    extension = extension.ToLower();
                    identificationDocPath = Guid.NewGuid() + "." + IdentificationDoc.FileName.Split('.')[1];

                    if (allowedExtensions.Contains(extension))
                    {

                        try
                        {


                            IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + identificationDocPath);
                        }
                        catch (Exception ex)
                        {


                        }

                        vm.DocumentPhotoUrl = "/Documents/" + identificationDocPath;

                    }
                    else
                    {
                        ModelState.AddModelError("DocumentPhotoUrl", "File type not allowed to upload. ");
                        return View(vm);
                    }

                }
                if (vm.Id == 0)
                {
                    _auxDocumentationservices.UploadAuxSenderDocument(vm);
                }
                else
                {
                    _auxDocumentationservices.UpdateAuxSenderDocument(vm);
                }
                return RedirectToAction("Index", "AUXSenderDocumentation");
            }

            return View(vm);
        }
        [HttpGet]
        public JsonResult Delete(int id)
        {
            if (id > 0)
            {
                _auxDocumentationservices.Delete(id);
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