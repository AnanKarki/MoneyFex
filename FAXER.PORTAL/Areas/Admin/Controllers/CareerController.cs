using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using PagedList;
using RestSharp.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class CareerController : Controller
    {
        CareerServices Service = new CareerServices();
        CommonServices CommonService = new CommonServices();
       
        // GET: Admin/Career
        public ActionResult Index(string CountryCode = "", string City = "", string message="",string Date="",int? page=null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "success")
            {
                ViewBag.Message = "Job Posted Successfully !";
                message = "";
            }
            if (message == "successEdit")
            {
                ViewBag.Message = "Job update Successfully !";
                message = "";
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            if (CountryCode != "")
            {
                ViewBag.Country = CountryCode;
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<CareerIndexViewModel> vm = Service.getList(CountryCode, City).ToPagedList(pageNumber,pageSize);
            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                vm = vm.Where(x => Convert.ToDateTime(x.ClosingDate)>= FromDate && Convert.ToDateTime(x.ClosingDate)<= ToDate).ToPagedList(pageNumber, pageSize);
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddNewCareer(int id=0,string CountryCode = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetViewBagForCountries();
            SetViewBagForCities(CountryCode);
            var vm = new CareerViewModel();
            if (CountryCode != "")
            {
                vm.Country = CountryCode;
                vm.CountryCurrency = CommonService.getCurrencyCodeFromCountry(CountryCode);
            }
            if(id!=0)
            {
                 vm = Service.getJob(id);
                SetViewBagForCities(vm.Country);
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddNewCareer([Bind(Include = CareerViewModel.BindProperty)]CareerViewModel model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (model != null)
            {
                bool valid = true;
                if (string.IsNullOrEmpty(model.JobTitle))
                {
                    ModelState.AddModelError("JobTitle", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.Description))
                {
                    ModelState.AddModelError("Description", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.ContractType))
                {
                    ModelState.AddModelError("ContractType", "This field can't be blank !");
                    valid = false;
                }
                if (model.ClosingDate ==null)
                {
                    ModelState.AddModelError("ClosingDate", "This field can't be blank !");
                    valid = false;
                }
                if (model.ClosingDate <= DateTime.Now)
                {
                    ModelState.AddModelError("ClosingDate", "Closing Date should be greater than the current date !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.SalaryRange))
                {
                    ModelState.AddModelError("SalaryRange", "This field can't be blank !");
                    valid = false;
                }
                if (valid == true)
                {
                    if (Request.Files.Count > 0)
                    {
                        string fileName = "";
                        string directory = Server.MapPath("/Documents");
                        var newsImage = Request.Files["carrersImage"];

                        if (newsImage != null && newsImage.ContentLength > 0)
                        {
                            fileName = Guid.NewGuid() + "." + newsImage.FileName.Split('.')[1];
                            newsImage.SaveAs(Path.Combine(directory, fileName));
                            model.Image = "/Documents/" + fileName;
                        }

                    }
                    if (model.Id>0)
                    {
                        bool result = Service.saveEditedJob(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "successEdit" });
                        }
                    }
                    else
                    {
                        bool result = Service.saveCareer(model);
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

        public ActionResult deleteJob(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (id !=0)
            {
                bool result = Service.deleteJob(id);
                if (result)
                {
                    return RedirectToAction("Index");
                }
            }
            return null;
        }

        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Staff, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }


        public void SetViewBagForCities(string Country="")
        {
            var cities = Service.GetCitiesFromCountry(Country);
            ViewBag.Cities = new SelectList(cities, "City", "City");
        }
        public ActionResult showCareer(int id)
        {
            var data = Service.getJob(id);
            return Json(new
            {
                Title = data.JobTitle,
                FullBody = data.Description,
                ImageUrl = data.Image,
                Id = data.Id,
            }, JsonRequestBehavior.AllowGet);
        }


    }
}