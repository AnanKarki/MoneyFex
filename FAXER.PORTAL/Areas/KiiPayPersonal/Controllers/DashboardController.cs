using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Controllers
{
    public class DashboardController : Controller
    {
        KiiPayPersonalCommonServices _commonServices = null;
        DashboardServices _services = null;
        public DashboardController()
        {
            _commonServices = new KiiPayPersonalCommonServices();
            _services = new DashboardServices();
        }


        // GET: KiiPayPersonal/Dashboard

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Index()
        {
            if(Common.CardUserSession.PayIntoAnotherWalletSession != null)
            {
                Common.CardUserSession.PayIntoAnotherWalletSession = null;
            }

          
            return View();
        }

        public ActionResult PayBills()
        {
            return View();
        }

        public ActionResult AddAndWithdrawFromWallet()
        {
            return View();
        }

        public ActionResult WalletUsageControl()
        {
            var vm = _services.getWalletUsageControlViewModel();
            if(vm == null)
            {
                return RedirectToAction("Index");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AccountProfile()
        {
            
            var vm = _services.getViewProfileDetails();
            return View(vm);
        }
        

        [HttpGet]
        public ActionResult EditAccountProfile()
        {
            SetViewBagForCountries();
            SetViewBagForIDCardTypes();
            var vm = _services.getEditProfileDetails();
            return View(vm);
        }
        [HttpPost]
        public ActionResult EditAccountProfile([Bind(Include = EditAccountProfileViewModel.BindProperty)]EditAccountProfileViewModel model)
        {
            SetViewBagForCountries();
            SetViewBagForIDCardTypes();
            bool isValid = true;
            
            if (string.IsNullOrEmpty(model.City.Trim()))
            {
                ModelState.AddModelError("City", "This field can't be empty !");
                isValid = false;
            }
            if(string.IsNullOrWhiteSpace(model.City.Trim()))
            {
                ModelState.AddModelError("City", "This field can't be empty !");
                isValid = false;
            }
            if(string.IsNullOrEmpty(model.Address1.Trim()))
            {
                ModelState.AddModelError("Address1", "This field can't be empty !");
                isValid = false;
            }
            if(string.IsNullOrEmpty(model.MobileNo.Trim()))
            {
                ModelState.AddModelError("MobileNo", "This field can't be empty !");
                isValid = false;
            }
            else
            {
                bool checkIfPhoneNumberAlreadyExists = _services.checkIfMobileNumberAlreadyExists(model.MobileNo);
                if (checkIfPhoneNumberAlreadyExists == true)
                {
                    ModelState.AddModelError("MobileNo", "An user with this mobile number is already registered !");
                    isValid = false;
                }

            }
            if (string.IsNullOrEmpty(model.EmailAddress.Trim()))
            {
                ModelState.AddModelError("EmailAddress", "This field can't be empty !");
                isValid = false;

            }
            else {
                bool checkIfEmailAlreadyExists = _services.checkIfEmailAlreadyExists(model.EmailAddress);
                if(checkIfEmailAlreadyExists == true)
                {
                    ModelState.AddModelError("EmailAddress", "An user with this email address already exists !");
                    isValid = false;
                }
            }
            if(string.IsNullOrEmpty(model.IDCardType.Trim()))
            {
                ModelState.AddModelError("IDCardType", "Please choose IDCard Type !");
                isValid = false;
            }
            if(string.IsNullOrEmpty(model.IDCardNumber.Trim()))
            {
                ModelState.AddModelError("IDCardNumber", "This field can't be empty !");
                isValid = false;
            }
            if(model.IDCardExpiringDay == 0)
            {
                ModelState.AddModelError("IDCardExpiringDay", "Please choose a day !");
                isValid = false;
            }
            if(model.IDCardExpiringMonth == (Month)0)
            {
                ModelState.AddModelError("IDCardExpiringMonth", "Please choose a month !");
                isValid = false;
            }
            if(model.IDCardExpiringYear == 0)
            {
                ModelState.AddModelError("IDCardExpiringYear", "This field cant be blank !");
                isValid = false;
            }
            if(string.IsNullOrEmpty(model.IDCardIssuingCountry))
            {
                ModelState.AddModelError("IDCardIssuingCountry", "Please choose a country !");
                isValid = false;
            }
            if (isValid == true)
            {
                bool update = _services.updateKiiPayPersonalAccount(model);
                if (update == true)
                {
                    return RedirectToAction("AccountProfile");
                }
            }
            return View(model);
        }


        

        public JsonResult SendEditProfilePhoneCode()
        {
            if(Common.CardUserSession.LoggedCardUserViewModel != null)
            {
                if(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId != 0)
                {
                    string phoneNo = Common.Common.GetCountryPhoneCode(_commonServices.kiiPayPersonalUserInformation().Country) + _commonServices.kiiPayPersonalUserInformation().MobileNo;
                    string phoneCode = Common.Common.GenerateRandomDigit(8);
                    Common.MiscSession.EditProfielPhoneCode = phoneCode;
                    SmsApi smsApiServices = new SmsApi();
                    string message = smsApiServices.GetRegistrationMessage(phoneCode);
                    smsApiServices.SendSMS(phoneNo, message);
                    return Json(new
                    {
                        result = phoneCode
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new
            {
            }, JsonRequestBehavior.AllowGet);

            
        }

        public JsonResult GetWithdrawalCode()
        {
            var code = _services.getWithdrawalCode(Common.CardUserSession.LoggedCardUserViewModel == null ? 0 : Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalWalletId );
            if(!string.IsNullOrEmpty(code))
            {
                return Json(new
                {
                    Code = code
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }
        private void SetViewBagForCountries()
        {
            var countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        private void SetViewBagForIDCardTypes()
        {
            var idCardTypes = _services.GetIDCardTypes();
            ViewBag.IDCardTypes = new SelectList(idCardTypes, "Code", "Name");
        }
    }
}