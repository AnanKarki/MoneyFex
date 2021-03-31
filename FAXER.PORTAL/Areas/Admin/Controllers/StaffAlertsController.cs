using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class StaffAlertsController : Controller
    {
        // GET: Admin/StaffAlerts
        ViewAlertsServices Service = new ViewAlertsServices();
        CommonServices CommonService = new CommonServices();

        public ActionResult Index(string CountryCode = "", string City = "", int staff = 0, string Date = "", string message = "",int? page=null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "success")
            {
                ViewBag.Message = "Alert added successfully !";
                message = "";

            }
            if (message == "updatesuccess")
            {
                ViewBag.Message = "Alert update successfully !";
                message = "";
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            SetViewBagForStaffs(CountryCode, City);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<MasterViewAlertsViewModel> vm = Service.getAlertsList(CountryCode, City,staff,Date,Module.Staff).ToPagedList(pageNumber,pageSize);
            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddNewAlert(int Id = 0, string countryCode = "", string city = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (string.IsNullOrEmpty(city) == false)
            {
                city = city.Trim();
                city = city.ToLower();
            }
            SetViewBagForCountries();
            SetViewBagForCities(countryCode);
            SetViewBagForStaffs(countryCode, city);
            var model = new AddNewAlertViewModel();
            model.Country = countryCode;
            model.PublishedDate = DateTime.Now.Date;

            if (Id > 0)
            {
                model = Service.getAlertEdit(Id, Module.Staff);
                SetViewBagForStaffs(model.Country, model.City);
            }
            if (countryCode != "")
            {
                model.Country = countryCode;
            }
            if (city != "")
            {
                model.City = city;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewAlert([Bind(Include = AddNewAlertViewModel.BindProperty)]AddNewAlertViewModel model)
        {
            if (model != null)
            {
                bool valid = true;
                if (string.IsNullOrEmpty(model.Heading))
                {
                    ModelState.AddModelError("Heading", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.FullMessage))
                {
                    ModelState.AddModelError("FullMessage", "This field can't be blank !");
                    valid = false;
                }
                else if (model.EndDate == default(DateTime))
                {
                    ModelState.AddModelError("EndDate", "This field can't be blank !");
                    valid = false;
                }
                else if (model.EndDate <= DateTime.Now)
                {
                    ModelState.AddModelError("EndDate", "This date must be greater than today's date !");
                    valid = false;
                }


                string fileNameUrl = "";
                if (Request.Files.Count > 0)
                {
                    var fileName = Request.Files["AlertPhoto"];
                    string directory = Server.MapPath("/Documents");

                    var upload = Request.Files[0];
                    if (upload != null && upload.ContentLength > 0)
                    {
                        fileNameUrl = Guid.NewGuid() + "." + fileName.FileName.Split('.')[1];
                        upload.SaveAs(Path.Combine(directory, fileNameUrl));
                    }

                }
                model.Photo = "/Documents/" + fileNameUrl;
                if (valid == true)
                {
                    model.Module = Module.Staff;
                    if (model.Id == 0)
                    {
                       
                        bool result = Service.saveNewAlert(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "success" });
                        }
                    }
                    else
                    {

                        bool result = Service.saveEditedAlert(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "updatesuccess" });
                        }
                    }

                }
            }
            SetViewBagForCountries();
            SetViewBagForCities(model.Country);
            SetViewBagForStaffs(model.Country, model.City);
            return View(model);
        }


        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public void SetViewBagForCities(string Country = "")
        {
            StaffHolidaysServices Service = new StaffHolidaysServices();
            var cities = Service.GetCities();
            if (Country != "")
            {
                cities = Service.GetCitiesFromCountry(Country);
            }
            ViewBag.Cities = new SelectList(cities, "City", "City");
        }

        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Staff, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }

        public void SetViewBagForStaffs(string Country = "", string City = "")
        {
            StaffNoticeBoardServices Service = new StaffNoticeBoardServices();
            var staffs = Service.GetEmptyStaffList();
            if (!string.IsNullOrEmpty(Country) && !string.IsNullOrEmpty(City))
            {
                City = (City.Trim()).ToLower();
                staffs = Service.GetFilteredStaffList(Country, City);
            }
            ViewBag.Staffs = new SelectList(staffs, "staffId", "staffName");
        }
     

        public JsonResult DeleteAlert(int id)
        {
            if (id != 0)
            {

                bool result = Service.DeleteAlert(id);
                if (result)
                {
                    return Json(new
                    {
                        Data = true,
                        Message = "Deleted Sucessfully"
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            return Json(new
            {
                Data = false,
                Message = "Something went wrong. Please try again!"
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetStaffAccountNo(int staffId)
        {
            var Accoutno = CommonService.getStaffMFSCode(staffId);


            return Json(new
            {
                Data = Accoutno
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPreView(int Id)
        {
            var data = Service.getAlertEdit(Id,Module.Staff);

            return Json(new
            {
                Photo = data.Photo,
                Heading = data.Heading,
                FullMessage = data.FullMessage,
                Id = data.Id,
            }, JsonRequestBehavior.AllowGet);
        }


    }
}