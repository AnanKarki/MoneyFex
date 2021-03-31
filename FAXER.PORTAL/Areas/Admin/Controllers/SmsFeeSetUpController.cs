using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class SmsFeeSetUpController : Controller
    {
        Services.CommonServices commonServices = null;
        Services.SmsFeeService smsFeeServices = null;
        public SmsFeeSetUpController()
        {
            commonServices = new Services.CommonServices();
            smsFeeServices = new Services.SmsFeeService();
        }
        // GET: Admin/SmsFeeSetUp
        public ActionResult Index(string countryCode = "")
        {

            var model = new SmsFeeVm();
            var fees = smsFeeServices.getFeeList();
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            if (!string.IsNullOrEmpty(countryCode))
            {
                fees = fees.Where(x => x.CountryCode == countryCode).ToList();


            }
            model.SMSFeelistViewModels = (from c in fees
                                          select new SMSFeelistViewModel()
                                          {
                                              Id = c.Id,
                                              Country = c.CountryName,
                                              CountryCode = c.CountryCode,
                                              SmsFee = c.SmsFee
                                          }).ToList();
            return View(model);

        }

        public ActionResult SetNewFee()
        {
            ViewBag.Country = new SelectList(commonServices.GetCountries().ToList(), "Code", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult SetNewFee([Bind(Include = SMSFeelistViewModel.BindProperty)]SMSFeelistViewModel vm)
        {
            ViewBag.Country = new SelectList(commonServices.GetCountries().ToList(), "Code", "Name");

            if (vm.SmsFee <= 0) {

                ModelState.AddModelError("SmsFee", "Please enter the amount above 0");
                return View(vm);
            }

            if (ModelState.IsValid)
            {
                SmsCharge model = new SmsCharge()
                {
                    CountryCode = vm.CountryCode,
                    CountryName = Common.Common.GetCountryName(vm.CountryCode),
                    IsDeleted = false,
                    SmsFee = vm.SmsFee
                };
                var result = smsFeeServices.Post(model);
                return RedirectToAction("Index", "SmsFeeSetUp");
            }
            return View();
        }
        public ActionResult Delete(int id)
        {
            ViewBag.Country = new SelectList(commonServices.GetCountries().ToList(), "Code", "Name");
            var model = smsFeeServices.getFeeList().Where(x => x.Id == id).FirstOrDefault();
            model.IsDeleted = true;
            var result = smsFeeServices.Delete(model);
            if (result == true)
            {
                return RedirectToAction("Index", "SmsFeeSetUp");
            }
            else
            {
                ViewBag.DeleteError = "Could not delete this fee";
                return RedirectToAction("Index", "SmsFeeSetUp");
            }
        }


        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.Country = new SelectList(commonServices.GetCountries().ToList(), "Code", "Name");
            //  SetViewBagForCountries();
            var model = smsFeeServices.getFeeList().Where(x => x.Id == id).FirstOrDefault();
            SMSFeelistViewModel vm = new SMSFeelistViewModel()
            {
                Country = model.CountryCode,
                CountryName = model.CountryName,
                CountryCode = model.CountryCode,
                Id = model.Id,
                IsDeleted = model.IsDeleted,
                SmsFee = model.SmsFee
            };
            return View(vm);
        }

        [HttpPost]
        public ActionResult Edit([Bind(Include = SMSFeelistViewModel.BindProperty)]SMSFeelistViewModel vm)
        {
            ViewBag.Country = new SelectList(commonServices.GetCountries().ToList(), "Code", "Name");
            if (ModelState.IsValid)
            {
                SmsCharge model = new SmsCharge()
                {
                    Id = vm.Id,
                    CountryCode = vm.CountryCode,
                    CountryName = Common.Common.GetCountryName(vm.CountryCode),
                    IsDeleted = vm.IsDeleted,
                    SmsFee = vm.SmsFee
                };
                var result = smsFeeServices.Update(model);
                return RedirectToAction("Index", "SmsFeeSetUp");
            }

            return View(vm);

        }

        private void SetViewBagForCountries()
        {
            var countries = commonServices.GetCountries();
            ViewBag.Country = new SelectList(countries, "Code", "Name");
        }

        public JsonResult GetExistingDate(string countryCode)
        {
            var data = smsFeeServices.getFeeList().Where(x => x.CountryCode == countryCode && x.IsDeleted == false).FirstOrDefault();
            if (data != null)
            {
                if (data != null)
                {
                    return Json(new
                    {
                        SmsFee = data.SmsFee,
                        Id = data.Id
                    
                    }, JsonRequestBehavior.AllowGet);
                }              
            }
            return null;
        }
    }

}
