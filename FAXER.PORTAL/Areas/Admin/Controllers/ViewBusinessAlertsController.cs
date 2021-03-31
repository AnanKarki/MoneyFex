using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewBusinessAlertsController : Controller
    {
        ViewBusinessAlertsServices Service = new ViewBusinessAlertsServices();
        CommonServices CommonService = new CommonServices();

        // GET: Admin/ViewBusinessAlerts
        public ActionResult Index(string CountryCode = "", string City = "",int Business=0,string Date="", string message="",int? page=null)
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
            setViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            SetViewBagForBusiness(CountryCode, City);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<MasterViewBusinessMerchantViewModel> vm = Service.getAlertsList(CountryCode, City,Business,Date).ToPagedList(pageNumber,pageSize);
            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddNewAlert(int Id=0,string countryCode = "", string city = "")
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
            setViewBagForCountries();
            SetViewBagForCities(countryCode);
            SetViewBagForBusiness(countryCode, city);
            var model = new AddNewBusinessAlertViewModel();
            model.Country = countryCode;
            model.PublishedDate = DateTime.Now.Date;
            if(Id>0)
            {
                model= Service.getAlertEdit(Id);
                SetViewBagForBusiness(model.Country,model.City);
            }
            if(countryCode!="")
            {
                model.Country = countryCode;
            }
            if(city!="")
            {
                model.City = city;
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult AddNewAlert([Bind(Include = AddNewBusinessAlertViewModel.BindProperty)]AddNewBusinessAlertViewModel model)
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
                else if (model.StartDate == default(DateTime))
                {
                    ModelState.AddModelError("StartDate", "This field can't be blank !");
                    valid = false;
                }
                else if (model.EndDate <= DateTime.Now)
                {
                    ModelState.AddModelError("EndDate", "This date must be greater than today's date !");
                    valid = false;
                }
               

                string fileName = "";
                if (Request.Files.Count > 0)
                {
                    string directory = Server.MapPath("/Documents");
                    
                    var upload = Request.Files["AlertPhoto"];
                    if (upload != null && upload.ContentLength > 0)
                    {
                        fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1];
                        upload.SaveAs(Path.Combine(directory, fileName));
                    }
                }
                model.Photo = "/Documents/" + fileName;
                if (valid == true)
                {
                    if(model.Id>0)
                    {
                        bool result = Service.saveEditedAlert(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "updatesuccess" });
                        }
                    }
                    else
                    {
                        bool result = Service.saveNewAlert(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "success" });
                        }
                    }
                   
                }
            }
            setViewBagForCountries();
            SetViewBagForCities(model.Country);
            SetViewBagForBusiness(model.Country, model.City);
            return View(model);
        }

        [HttpGet]
        public ActionResult EditAlert(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var vm = Service.getAlertEdit(id);
            return View(vm);
        }

        [HttpPost]
        public ActionResult EditAlert ([Bind(Include = AddNewBusinessAlertViewModel.BindProperty)]AddNewBusinessAlertViewModel model)
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
                    ModelState.AddModelError("FullMessage", "This field can't be true !");
                    valid = false;
                }
                else if (model.PublishedDate == default(DateTime))
                {
                    ModelState.AddModelError("PublishedDate", "Please exit this page and try again !");
                    valid = false;
                }
                else if (model.EndDate == default(DateTime))
                {
                    ModelState.AddModelError("EndDate", "This field can't be true !");
                    valid = false;
                }
                else if (model.EndDate < DateTime.Now)
                {
                    ModelState.AddModelError("EndDate", "End Date can't be smaller than today's date !");
                    valid = false;
                }

                if (valid == true)
                {
                    string fileName = "";
                    if (Request.Files.Count > 0)
                    {
                        string directory = Server.MapPath("/Documents");
                        var upload = Request.Files[0];
                        if (upload != null && upload.ContentLength >0)
                        {
                            fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1];
                            upload.SaveAs(Path.Combine(directory, fileName));
                        }
                        model.Photo = "/Documents/" + fileName;
                    }
                    bool result = Service.saveEditedAlert(model);
                    if (result)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
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
        private void setViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public void SetViewBagForCities(string Country = "")
        {

            var cities = Service.GetCities();
            if (!(string.IsNullOrEmpty(Country)))
            {
                cities = Service.GetCitiesFromCountry(Country);
            }
            ViewBag.Cities = new SelectList(cities, "City", "City");
        }

        public void SetViewBagForBusiness(string countryCode = "", string city = "")
        {
            if (string.IsNullOrEmpty(city) == false)
            {
                city = city.Trim();
                city = city.ToLower();
            }
            var business = Service.GetEmptyBusinessList();
            if ((string.IsNullOrEmpty(countryCode) == false) && (string.IsNullOrEmpty(city) == false))
            {
                business = Service.GetBusinessFromCountryCity(countryCode, city);
            }
            ViewBag.Business = new SelectList(business, "KiiPayBusinessInformationId", "BusinessName");
        }
        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Faxer, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }

        public JsonResult GetBusinesAccounNo(int businessId)
        {
            var Accoutno = CommonService.GetSenderAccountNoBySenderId(businessId);

            return Json(new
            {
                Data = Accoutno
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPreView(int Id)
        {
            var data = Service.getAlertEdit(Id);

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