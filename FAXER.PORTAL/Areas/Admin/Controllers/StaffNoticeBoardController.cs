using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class StaffNoticeBoardController : Controller
    {
        StaffNoticeBoardServices Service = new StaffNoticeBoardServices();
        CommonServices CommonService = new CommonServices();
        // GET: Admin/StaffNoticeBoard
        public ActionResult Index(string CountryCode = "", string City = "", string message = "", int Year = 0, int Month = 0, string staffName = ""
            , string Title = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "success")
            {
                ViewBag.Message = "Notice Added Successfully !";
                message = "";
            }
            else if (message == "updatesuccess")
            {
                ViewBag.Message = "Notice Update Successfully !";
                message = "";
            }
            SetViewBagForCountries();
            SetViewBagForCities(CountryCode);
            ViewBag.Years = new SelectList(Enumerable.Range(2018, 10));
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.staffName = staffName;
            ViewBag.TitleName = Title;
            IPagedList<ViewStaffNoticeViewModel> vm = Service.GetStaffNotices(CountryCode, City, Year, Month, staffName, Title).ToPagedList(pageNumber, pageSize);


            return View(vm);
        }

        [HttpGet]
        public ActionResult AddStaffNotice(int id = 0, string country = "", string city = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForCities(country);
            SetViewBagForStaffs(country, city);
            AddStaffNoticeViewModel vm = new AddStaffNoticeViewModel();
            if (id > 0)
            {
                vm = Service.EditNoticeInfo(id);
                SetViewBagForStaffs(vm.Country, vm.City);
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddStaffNotice([Bind(Include = AddStaffNoticeViewModel.BindProperty)]AddStaffNoticeViewModel model)
        {
            if (model != null)
            {
                bool valid = true;

                if (string.IsNullOrEmpty(model.Headline))
                {
                    ModelState.AddModelError("Headline", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.FullNotice))
                {
                    ModelState.AddModelError("FullNotice", "This field can't be blank !");
                    valid = false;
                }
                if (valid == true)
                {
                    if (model.Id > 0)
                    {

                        bool result = Service.SaveEditNoticeInfo(model);
                        if (result)
                        {

                            return RedirectToAction("Index", new { @message = "updatesuccess" });
                        }
                    }
                    else
                    {
                        bool result = Service.SaveStaffNotices(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "success" });
                        }
                    }

                }
            }

            SetViewBagForCountries();
            SetViewBagForCities();
            SetViewBagForStaffs();
            return View(model);
        }

        [HttpGet]
        public ActionResult EditStaffNotice(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var vm = Service.EditNoticeInfo(id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditStaffNotice([Bind(Include = AddStaffNoticeViewModel.BindProperty)]AddStaffNoticeViewModel model)
        {
            if (model != null)
            {
                bool valid = true;
                if (string.IsNullOrEmpty(model.Headline))
                {
                    ModelState.AddModelError("Headline", "This field can't be blank !");
                    valid = false;
                }
                else if (string.IsNullOrEmpty(model.FullNotice))
                {
                    ModelState.AddModelError("FullNotice", "This field can't be blank !");
                    valid = false;
                }
                if (valid == true)
                {
                    bool result = Service.SaveEditNoticeInfo(model);
                    if (result)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }



        [HttpGet]
        public JsonResult DeleteNotice(int id)
        {
            if (id != 0)
            {
                bool result = Service.DeleteNotice(id);
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


        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            countries.Add(new DropDownViewModel()
            {
                Code = "All",
                Name = "All"
            });
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public void SetViewBagForCities(string Country = "")
        {
            var cities = Service.GetCities();
            if (!string.IsNullOrEmpty(Country))
            {
                cities = Service.GetCitiesFromCountry(Country);
            }
            cities.Add(new DropDownCityViewModel
            {
                City = "All",
                CountryCode = "All",
            });
            ViewBag.Cities = new SelectList(cities, "City", "City");
        }
        public void SetViewBagForStaffs(string Country = "", string City = "")
        {
            var staffs = Service.GetEmptyStaffList();
            if (!string.IsNullOrEmpty(Country) && !string.IsNullOrEmpty(City))
            {
                City = (City.Trim()).ToLower();
                staffs = Service.GetFilteredStaffList(Country, City);
            }
            staffs.Add(new DropDownStaffViewModel
            {
                staffId = 0,
                staffName = "All"
            });
            ViewBag.Staffs = new SelectList(staffs, "staffId", "staffName");
        }

    }
}