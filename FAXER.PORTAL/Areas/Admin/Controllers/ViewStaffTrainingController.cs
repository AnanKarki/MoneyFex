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
    public class ViewStaffTrainingController : Controller
    {
        ViewStaffTrainingServices Service = null;
        CommonServices CommonService = null;
        public ViewStaffTrainingController()
        {
            Service = new ViewStaffTrainingServices();
            CommonService = new CommonServices();
        }

        // GET: Admin/ViewStaffTraining
        public ActionResult Index(string CountryCode = "", string City = "", string message = "", int Year = 0, int Month = 0, int StaffId = 0,
          string staffName = "", string Title = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "success")
            {
                ViewBag.Message = "Training Added Successfully !";
                message = "";
            }
            else if (message == "updatesuccess")
            {
                ViewBag.Message = "Training Update Successfully !";
                message = "";
            }
            ViewBag.Years = new SelectList(Enumerable.Range(2018, 10));

            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.staffName = staffName;
            ViewBag.TitleName = Title;
            IPagedList<ViewStaffTrainingViewModel> vm = Service.getTrainingList(CountryCode, City, Year, Month, StaffId, staffName, Title).ToPagedList(pageNumber, pageSize);

            return View(vm);
        }

        [HttpGet]
        public ActionResult AddStaffTraining(int id = 0, string countryCode = "", string city = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (city != null)
            {
                city = city.Trim();
                city = city.ToLower();
            }
            SetViewBagForCountries();
            SetViewBagForCities(countryCode);
            SetViewBagForStaffs(countryCode, city);
            AddStaffTrainingViewModel vm = new AddStaffTrainingViewModel();
            if (id > 0)
            {
                vm = Service.EditStaffTrainingInfo(id);


                SetViewBagForStaffs(vm.Country, vm.City);
            }



            return View(vm);
        }

        [HttpPost]
        public ActionResult AddStaffTraining([Bind(Include = AddStaffTrainingViewModel.BindProperty)]AddStaffTrainingViewModel model)
        {
            if (model != null)
            {
                //validation
                bool valid = true;

                if (string.IsNullOrEmpty(model.Title))
                {
                    ModelState.AddModelError("Title", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.Link))
                {
                    ModelState.AddModelError("Link", "This field can't be blank !");
                    valid = false;
                }

                if (valid == true)
                {
                    if (model.Id > 0)
                    {

                        bool result = Service.SaveTrainingEditInfo(model);
                        if (result)
                        {

                            return RedirectToAction("Index", new { @message = "updatesuccess" });
                        }
                    }
                    else
                    {
                        bool result = Service.saveStaffTrainingInfo(model);
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

        public JsonResult deleteTraining(int id)
        {
            if (id != 0)
            {
                bool result = Service.deleteTraining(id);
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

        [HttpGet]
        public ActionResult EditTraining(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var vm = Service.EditStaffTrainingInfo(id);

            vm.Deadline = DateTime.Now;
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditTraining([Bind(Include = AddStaffTrainingViewModel.BindProperty)]AddStaffTrainingViewModel model)
        {
            if (model != null)
            {
                bool valid = true;
                if (string.IsNullOrEmpty(model.Title))
                {
                    ModelState.AddModelError("Title", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.Link))
                {
                    ModelState.AddModelError("Link", "This field can't be blank !");
                    valid = false;
                }
                if (model.Deadline == default(DateTime))
                {
                    ModelState.AddModelError("Deadline", "This field can't be blank !");
                    valid = false;
                }
                //if (string.IsNullOrEmpty(model.completeTraining))
                //{
                //    ModelState.AddModelError("completeTraining", "This field can't be blank !");
                //    valid = false;
                //}
                //if (string.IsNullOrEmpty(model.outstandingTraining))
                //{
                //    ModelState.AddModelError("outstandingTraining", "This field can't be blank !");
                //    valid = false;
                //}
                if (valid == true)
                {
                    bool result = Service.SaveTrainingEditInfo(model);
                    if (result)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
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
            if (Country != "")
            {
                cities = Service.GetCitiesFromCountry(Country);
            }
            cities.Add(new DropDownCityViewModel()
            {
                City = "All",
            });
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
        public JsonResult GetStaffAccToCountryAndCity(string countryCode = "", string city = "")
         {
            StaffNoticeBoardServices _service = new StaffNoticeBoardServices();
            var staffs = _service.GetFilteredStaffList(countryCode, city);
            return Json(new
            {
                Data = staffs
            }, JsonRequestBehavior.AllowGet);

        }
    }
}