using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Models.SignUp;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Controllers
{
    public class SenderSignUpController : Controller
    {
        SFaxerSignUp _senderSignUpServices = null;
        public SenderSignUpController()
        {

            _senderSignUpServices = new SFaxerSignUp();
        }
        // GET: SenderSignUp
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult PersonalLoginInfo()
        {


            GetCountries();

            var vm = _senderSignUpServices.GetPersonalLoginInfo();
            return View(vm);
        }

        [HttpPost]
        public ActionResult PersonalLoginInfo([Bind(Include = SenderPersonalLoginInformationVM.BindProperty)] SenderPersonalLoginInformationVM vm)
        {
            GetCountries();
            if (ModelState.IsValid)
            {

                vm.EmailAddress.Trim();
                vm.MobileNo = Common.Common.IgnoreZero(vm.MobileNo);
                var countryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode).Split('+')[1];
                if (vm.MobileNo.StartsWith(countryPhoneCode))
                {
                    vm.MobileNo = vm.MobileNo.Substring(countryPhoneCode.Length, vm.MobileNo.Length - countryPhoneCode.Length);
                }

                bool IsMobileNoExist = Common.OtherUtlities.IsMobileNoExist(vm.MobileNo);
                SmsApi smsApi = new SmsApi();
                bool validMobile = smsApi.IsValidMobileNo(Common.Common.GetCountryPhoneCode(vm.CountryCode) + vm.MobileNo);
                if (!validMobile)
                {
                    ModelState.AddModelError("MobileNo", "Invalid mobile number ");
                    return View(vm);

                }
                if (IsMobileNoExist == false)
                {
                    ModelState.AddModelError("MobileNo", "Mobile number already exists");
                    return View(vm);
                }
                bool isEmailExist = Common.OtherUtlities.IsEmailExist(vm.EmailAddress);
                if (isEmailExist == false)
                {
                    ModelState.AddModelError("EmailAddress", "Email address already exists");
                    return View(vm);
                }
                //Common.Common.IgnoreZero()
                _senderSignUpServices.SetPersonalLoginInfo(vm);

                return RedirectToAction("PersonalDetails");
            }
            return View(vm);
        }


        public JsonResult GetCountryPhoneCode(string CountryCode)
        {

            string PhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {

                PhoneCode = PhoneCode,


            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult PersonalDetails()
        {


            var vm = _senderSignUpServices.GetPersonalDetail();
            return View(vm);
        }
        [HttpPost]
        public ActionResult PersonalDetails([Bind(Include = SenderPersonalDetailVM.BindProperty)] SenderPersonalDetailVM vm)
        {

            if (ModelState.IsValid)
            {

                var senderName = vm.FirstName.Trim() + (!string.IsNullOrEmpty(vm.MiddleName) == true ? vm.MiddleName.Trim() : "") + vm.LastName.Trim();
                if (senderName.Length > 21)
                {
                    ModelState.AddModelError("InvalidName", "Name should contain 21 characters only");
                    return View(vm);
                }

                var DOB = Common.DateUtilities.GetDateByYMD(vm.Day, (int)vm.Month, vm.Year);
                if (DOB ==  null) {

                    ModelState.AddModelError("DOB", "Enter valid date");
                    return View(vm);
                } 
                bool isValidAge = Common.DateUtilities.ValidateAge(DOB);

                if (isValidAge == false)
                {
                    ModelState.AddModelError("DOB", "You must be 18 years of age or above to sign up to our services");
                    return View(vm);
                }
                _senderSignUpServices.SetPersonalDetail(vm);

                return RedirectToAction("PersonalAddress");
            }
            return View(vm);
        }

      
        [HttpGet]
        public ActionResult PersonalAddress()
        {

            var vm = _senderSignUpServices.GetPersonalAddress();
            return View(vm);
        }
        [HttpPost]
        public ActionResult PersonalAddress([Bind(Include = SenderPersonalAddressVM.BindProperty)] SenderPersonalAddressVM vm)
        {

            if (ModelState.IsValid)
            {

                _senderSignUpServices.SetPersonalAddress(vm);

                SenderKiiPayWalletEnableVM model = new SenderKiiPayWalletEnableVM();
                _senderSignUpServices.CompleteRegistration(model);

                return RedirectToAction("Login", "FaxerAccount");
                return RedirectToAction("Index", "SenderRegistrationCodeVerification");


            }
            return View(vm);
        }


        [HttpGet]
        public ActionResult kiipayWallet()
        {

            return View();
        }
        [HttpPost]
        //[PreventSpam]
        public ActionResult kiipayWallet([Bind(Include = SenderKiiPayWalletEnableVM.BindProperty)] SenderKiiPayWalletEnableVM vm)
        {

            if (ModelState.IsValid)
            {
                _senderSignUpServices.CompleteRegistration(vm);

                return RedirectToAction("Index", "SenderRegistrationCodeVerification");
            }
            return View(vm);
        }


        public void GetCountries()
        {

            var result = (from c in Common.Common.GetCountriesForDropDown()
                          select new Models.CountryViewModel()
                          {
                              CountryCode = c.CountryCode,
                              CountryName = c.CountryName,
                              FlagCountryCode = c.FlagCountryCode
                          }).ToList();
            ViewBag.Countries = new SelectList(result, "CountryCode", "CountryName");
        }
    }


}