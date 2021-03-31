using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
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
    public class StaffDocumentationController : Controller
    {
        StaffDocumentationServices _services = null;
        CommonServices _CommonServices = null;

        public StaffDocumentationController()
        {
            _services = new StaffDocumentationServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/StaffDocumentation
        public ActionResult Index(string Country = "", string City = "", int StaffId = 0, string StaffName = "", string DocumentName = "",
                          string createByName = "", string DateRange = "", int? page = null)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetViewBagForCountries();
            SetViewBagForSCities(Country);

            ViewBag.City = City;
            ViewBag.StaffName = StaffName;
            ViewBag.DocumentName = DocumentName;
            ViewBag.staff = createByName;
            int pageNumber = (page ?? 1);
            int pageSize = 10;
            IPagedList<BusinessDocumentationViewModel> vm = _services.GetStaffDocumentations(Country, City, StaffId, StaffName, DocumentName, createByName, DateRange).ToPagedList(pageNumber, pageSize);
           
            return View(vm);

        }
        public ActionResult StaffUploadDocument(int Id = 0, string country = "", string city = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForCities(country);
            SetViewBagForStaffs(country, city);
            BusinessDocumentationViewModel vm = new BusinessDocumentationViewModel();
            if (Id != 0)
            {
                vm = _services.GetStaffDocumentInfo(Id);
                SetViewBagForStaffs(vm.Country, vm.City);

            }
            if (country != "")
            {
                vm.Country = country;

            }
            if (city != "")
            {
                vm.City = city;
            }
            return View(vm);
        }


        [HttpPost]
        public ActionResult StaffUploadDocument([Bind(Include = BusinessDocumentationViewModel.BindProperty)]BusinessDocumentationViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForCities(vm.Country);
            SetViewBagForStaffs(vm.Country, vm.City);

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
                    _services.UploadDocument(vm);
                }
                else
                {
                    _services.UpdateDocument(vm);
                }
                return RedirectToAction("Index", "StaffDocumentation");
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
            return Json(new
            {
                Data = false,
                Message = "Something went wrong. Please try again!"
            }, JsonRequestBehavior.AllowGet);
        }


        private void SetViewBagForCountries()
        {
            var countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }




        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Staff, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
        public void SetViewBagForCities(string country = "")
        {
            StaffHolidaysServices Service = new StaffHolidaysServices();
            var cities = Service.GetCities();
            if (country != "")
            {
                cities = Service.GetCitiesFromCountry(country);
            }

            ViewBag.Cities = new SelectList(cities, "City", "City");
        }

        public void SetViewBagForStaffs(string country = "", string city = "")
        {
            StaffHolidaysServices Service = new StaffHolidaysServices();
            var staffs = Service.GetEmptyStaffList();
            if (country != "" && city != "")
            {
                city = (city.Trim()).ToLower();

                staffs = Service.GetFilteredStaffList(country, city);
            }
            ViewBag.Staffs = new SelectList(staffs, "staffId", "staffName");
        }

        public JsonResult GetStaffAccountNo(int StaffId)
        {
            var Accoutno = _CommonServices.getStaffMFSCode(StaffId);


            return Json(new
            {
                Data = Accoutno
            }, JsonRequestBehavior.AllowGet);
        }


    }
}