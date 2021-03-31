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

    public class ViewRegisterBusinessController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        Services.ViewRegisterBusinessServices Service = new Services.ViewRegisterBusinessServices();



        // GET: Admin/ViewRegisterBusiness
        public ActionResult Index(string CountryCode = "", string City = "", string message = "",int? page=null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "mailSent")
            {
                ViewBag.Message = "Mail sent successfully !";
                message = "";
            }
            else if (message == "mailNotSent")
            {
                ViewBag.Message = "Mail not sent ! Please try again.";
                message = "";
            }
            else if (message == "merchantRegistered")
            {
                ViewBag.Message = "Merchant Initial Registration Successful !";
                message = "";
            }
            Common.MiscSession.CountryCode = CountryCode;
            Common.MiscSession.City = City;
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<ViewRegisterBusinessViewModel> registeredbusiness = Service.getBusinessList(CountryCode, City).ToPagedList(pageNumber,pageSize);
            if (registeredbusiness != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(registeredbusiness);
        }

        [HttpGet]
        public ActionResult RegisterAMerchant(string RegNumber = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            if (!(string.IsNullOrEmpty(RegNumber)))
            {
                var viewmodel = Service.GetBecomeMerchant(RegNumber);
                return View(viewmodel);
            }
            return View();
        }

        [HttpPost]
        public ActionResult RegisterAMerchant([Bind(Include = ViewRegisterBusinessViewModel.BindProperty)] ViewRegisterBusinessViewModel model)
         {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            if ((model.Id == 0) || (model.Id == null))
            {
                bool checkEmail = Service.CheckMerchantEmail(model.Email);
                if (checkEmail == true)
                {
                    ModelState.AddModelError("Email", "This email address is already taken !");
                    return View(model);
                }
            }

            if (ModelState.IsValid)
            {
                if (model.Confirm == false)
                {
                    ModelState.AddModelError("Confirm", "Please Confirm Before Continue.");
                    return View(model);
                }

                bool result = Service.AddMerchant(model);
                if (result)
                {
                    return RedirectToAction("Index", "ViewRegisterBusiness", new { @message = "merchantRegistered"});
                }


            }
            return View(model);
        }

        public ActionResult getCountryCode(string countryCode)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var code = "+" + CommonService.getPhoneCodeFromCountry(countryCode);
            return Json(new
            {
                CountryCode = code
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UpdateMerchant(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            if (id != 0)
            {
                var viewmodel = Service.GetBusinessInfo(id);
                return View(viewmodel);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult UpdateMerchant([Bind(Include = ViewRegisterBusinessViewModel.BindProperty)]ViewRegisterBusinessViewModel model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            var errors = ModelState.Values.SelectMany(v => v.Errors);

            if (ModelState.IsValid)
            {
                if (model.Confirm == false)
                {
                    ModelState.AddModelError("Confirm", "Please Confirm Before Continue.");
                }
                else
                {
                    bool result = Service.UpdateMerchant(model);
                    if (result)
                    {
                        return RedirectToAction("Index", "ViewRegisterBusiness");
                    }
                    return View();
                }
            }
            ViewBag.SuccessMessage = "The merchant account has been updated";
            return View(model);
        }

        public ActionResult DeleteMerchant(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (id != 0)
            {
                var result = Service.DeleteMerchant(id);
                if (result)
                {
                    return RedirectToAction("Index", "ViewRegisterBusiness");
                }

            }
            return RedirectToAction("Index", "ViewRegisterBusiness");

        }

        public ActionResult UpdateAccountStatus(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (id != 0)
            {
                var result = Service.UpdateAccountStatus(id);
                if (result)
                {
                    return RedirectToAction("Index", "ViewRegisterBusiness");
                }
                return RedirectToAction("Index", "ViewRegisterBusiness");
            }
            return RedirectToAction("Index", "ViewRegisterBusiness");
        }


        public ActionResult ViewRegisteredBusinessMore(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (id != 0)
            {
                var viewmodel = Service.GetBusinessInfo(id);
                var result = Service.GetBusinessNotes(id);
                viewmodel.BusinessNoteList = result;
                return View(viewmodel);
            }
            return View();
        }


        public ActionResult AddBusinessNote(string note, int KiiPayBusinessInformationId)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            BusinessNote businessNote = new BusinessNote()
            {

                KiiPayBusinessInformationId = KiiPayBusinessInformationId,
                CreatedByStaffId = Common.StaffSession.LoggedStaff.StaffId,
                CreatedByStaffName = Common.StaffSession.LoggedStaff.FirstName + " " + Common.StaffSession.LoggedStaff.MiddleName + " " + Common.StaffSession.LoggedStaff.LastName,
                CreatedDateTime = DateTime.Now,
                Note = note
            };

            var result = Service.AddNewBusinessNote(businessNote);
            return RedirectToAction("ViewRegisteredBusinessMore", new { id = KiiPayBusinessInformationId });
        }
        public void SetViewBagForSCities(string Country = "")
        {

            var cities = SCity.GetCities(DB.Module.BusinessMerchant, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public ActionResult sendEmails(string body = "", string subject = "", string emails = "")
        {
            if (!string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(subject) && !(string.IsNullOrEmpty(emails)))
            {
                bool result = Common.Common.sendBulkMail(emails, subject, body);
                if (result)
                {
                    return RedirectToAction("Index", new { @message = "mailSent" });
                }

            }
            return RedirectToAction("Index", new { @message = "mailNotSent" });
        }


    }

}

