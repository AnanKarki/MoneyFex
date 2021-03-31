using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Controllers
{
    public class PersonalKiiPayBankAccountController : Controller
    {
        KiiPayPersonalCommonServices _commonServices = null;
        Admin.Services.CommonServices _adminCommonServices = null;
        PersonalKiiPayBankAccountServices _service = null;
        public PersonalKiiPayBankAccountController()
        {
            _commonServices = new KiiPayPersonalCommonServices();
            _adminCommonServices = new Admin.Services.CommonServices();
            _service = new PersonalKiiPayBankAccountServices();
        }
        // GET: KiiPayPersonal/PersonalKiiPayBankAccount
        public ActionResult Index()
        {
            var vm = _service.getSavedBanksList();
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddNewBankAccount()
        {
            var vm = new AddNewBankAccountViewModel();
            vm.Branch = "";
            SetViewBagForCountries();
            SetViewBagForAddresses();
            SetViewBagForBanks();
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddNewBankAccount([Bind(Include = AddNewBankAccountViewModel.BindProperty)]AddNewBankAccountViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool save = _service.addBankAccount(model);
                if (save == true)
                {
                    return RedirectToAction("Index");
                }
            }
            SetViewBagForCountries();
            SetViewBagForAddresses();
            SetViewBagForBanks();
            return View(model);
        }

        public ActionResult DeleteBank(int id)
        {
            if (id != 0)
            {
                _service.deleteBank(id);
            }
            return RedirectToAction("Index");
        }

        public JsonResult SendVerificationCode()
        {
            string verificationCode = Common.Common.GenerateVerificationCode(6);
            if (!string.IsNullOrEmpty(verificationCode))
            {
                SmsApi smaApiServices = new SmsApi();
                string message = "Your verification code is " + verificationCode;
                string phoneNumber = Common.Common.GetCountryPhoneCode(Common.CardUserSession.LoggedCardUserViewModel.CountryCode) + Common.CardUserSession.LoggedCardUserViewModel.MobileNumber;
                if (!string.IsNullOrEmpty(phoneNumber))
                {
                    smaApiServices.SendSMS(phoneNumber, message);
                    Common.MiscSession.PasswordSecurityCode = verificationCode;
                    return Json(new
                    {
                        result = true,
                        code = verificationCode
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
                result = false
            }, JsonRequestBehavior.AllowGet);
        }




        private void SetViewBagForCountries()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        private void SetViewBagForBanks()
        {
            var banks = _commonServices.GetBanks();
            ViewBag.Banks = new SelectList(banks, "Code", "Name");
        }
        private void SetViewBagForAddresses()
        {
            var addresses = _adminCommonServices.GetCities();
            ViewBag.Addresses = new SelectList(addresses, "Code", "Name");
        }
    }
}