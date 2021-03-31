using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class SetPublicHolidaysController : Controller
    {
        ViewPublicHolidaysServices Service = new ViewPublicHolidaysServices();
        CommonServices CommonService = new CommonServices();
        StaffHolidaysServices HolidayService = new StaffHolidaysServices();
        // GET: Admin/SetPublicHolidays
        public ActionResult Index(string CountryCode = "", string City = "", string message = "", int Year = 0, int Month = 0, string Name = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "success")
            {
                ViewBag.Message = "Holiday Calendar Added Successfully !";
                message = "";
            }
            else if (message == "updatesuccess")
            {
                ViewBag.Message = "Holiday Calendar Update Successfully !";
                message = "";
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            ViewBag.Years = new SelectList(Enumerable.Range(2018, 10));
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.Name = Name;
            IPagedList<ViewPublicHolidaysViewModel> vm = Service.GetPublicHolidaysList(CountryCode, City, Year, Month, Name).ToPagedList(pageNumber, pageSize);
            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }


            return View(vm);
        }

        [HttpGet]
        public ActionResult AddPublicHoliday(int id = 0, string country = "", string city = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForCities(country);
            AddPublicHolidayViewModel vm = new AddPublicHolidayViewModel();
            if (id > 0)
            {
                vm = Service.GetEditPublicHolidayInfo(id);

            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult AddPublicHoliday([Bind(Include = AddPublicHolidayViewModel.BindProperty)] AddPublicHolidayViewModel model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                ModelState.AddModelError("Country", "Sorry, No logged staff found !");
                SetViewBagForCountries();
                SetViewBagForCities(model.Country);

                return View(model);
            }
            if (model != null)
            {
                bool valid = true;
                var isValidDate = Service.ValidatePubilcHolidayDate(0, Convert.ToDateTime(model.FromDate), Convert.ToDateTime(model.ToDate));

                if (string.IsNullOrEmpty(model.HolidayName))
                {
                    ModelState.AddModelError("HolidayName", "This field can't be blank !");
                    valid = false;
                }
                else if (model.FromDate == default(DateTime))
                {
                    ModelState.AddModelError("FromDate", "This field can't be blank !");
                    valid = false;
                }
                else if (model.ToDate == default(DateTime))
                {
                    ModelState.AddModelError("ToDate", "This field can't be blank !");
                    valid = false;
                }
                else if (isValidDate == false)
                {
                    ModelState.AddModelError("ToDate", "There's already a holiday in this date !");
                    valid = false;
                }
                if (valid == true)
                {
                    if (model.Id > 0)
                    {
                        bool result = Service.SaveEditPublicHoliday(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "updatesuccess" });
                        }

                    }
                    else
                    {
                        bool result = Service.SavePublicHoliday(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "success" });
                        }
                    }

                }
            }
            SetViewBagForCountries();
            SetViewBagForCities(model.Country);

            return View(model);
        }

        [HttpGet]
        public ActionResult EditPublicHoliday(int Id)
        {

            var vm = Service.GetEditPublicHolidayInfo(Id);
            return View(vm);

        }

        [HttpPost]
        public ActionResult EditPublicHoliday([Bind(Include = AddPublicHolidayViewModel.BindProperty)]AddPublicHolidayViewModel model)
        {
            if (model != null)
            {
                var isValidDate = Service.ValidatePubilcHolidayDate(model.Id, Convert.ToDateTime(model.FromDate), Convert.ToDateTime(model.ToDate));
                if (string.IsNullOrEmpty(model.HolidayName))
                {
                    ModelState.AddModelError("HolidayName", "This field can't be blank !");
                    return View(model);
                }
                else if (model.FromDate == default(DateTime))
                {
                    ModelState.AddModelError("FromDate", "This field can't be blank !");
                    return View(model);
                }
                else if (model.ToDate == default(DateTime))
                {
                    ModelState.AddModelError("ToDate", "This field can't be blank !");
                    return View(model);
                }
                else if (isValidDate == false)
                {
                    ModelState.AddModelError("ToDate", "There's already a holiday in this date !");
                    return View(model);
                }
                bool result = Service.SaveEditPublicHoliday(model);
                if (result)
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }


        public JsonResult DeletePublicHoliday(int id)
        {
            if (id != 0)
            {
                bool result = Service.DeletePublicHoliday(id);
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

        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public void SetViewBagForCities(string country = "")
        {
            var cities = HolidayService.GetCities();
            if (country != "")
            {
                cities = HolidayService.GetCitiesFromCountry(country);
            }

            ViewBag.Cities = new SelectList(cities, "City", "City");
        }

        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Staff, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
    }
}