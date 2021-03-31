using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Models.SignUp;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace FAXER.PORTAL.Controllers
{
    public class SenderBusinessSignUpController : Controller
    {
        SFaxerSignUp _senderSignUpServices = null;
        CommonServices _CommonServices = null;
        SenderBusinessInformationServices _businessInfoServices = null;

        public SenderBusinessSignUpController()
        {
            _senderSignUpServices = new SFaxerSignUp();
            _CommonServices = new CommonServices();
            _businessInfoServices = new SenderBusinessInformationServices();
        }
        // GET: SenderBusinessSignUp
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BusinessLoginInfo(string Country = "")
        {
            var countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            SenderBusinessLoginInfoViewModel vm = new SenderBusinessLoginInfoViewModel();
            if (!string.IsNullOrEmpty(Country))
            {
               
                vm.Country = Country;
                vm.MobileCode = Common.Common.GetCountryPhoneCode(Country);

                return View(vm);
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult BusinessLoginInfo([Bind(Include = SenderBusinessLoginInfoViewModel.BindProperty)]SenderBusinessLoginInfoViewModel vm)
        {

            var countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            vm.MobileCode = Common.Common.GetCountryPhoneCode(vm.Country);
            vm.EmailAddress.Trim();
            if (ModelState.IsValid)
            {
                vm.MobileNo = Common.Common.IgnoreZero(vm.MobileNo);
                SmsApi smsApi = new SmsApi();
                bool validMobile = smsApi.IsValidMobileNo(Common.Common.GetCountryPhoneCode(vm.Country) + vm.MobileNo);
                if (!validMobile)
                {
                    ModelState.AddModelError("MobileNo", "Invalid mobile number ");
                    return View(vm);

                }
                bool IsMobileNoExist = Common.OtherUtlities.IsMobileNoExist(vm.MobileNo);
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
                _senderSignUpServices.SetBusinessPersonalLoginInfo(vm);
                return RedirectToAction("BusinessPersonalDetails", "SenderBusinessSignUp");
            }

            return View(vm);

        }
        public ActionResult BusinessPersonalDetails()
        {
            return View();

        }

        [HttpPost]
        public ActionResult BusinessPersonalDetails([Bind(Include = SenderPersonalDetailVM.BindProperty)]SenderPersonalDetailVM vm)
        {

            if (ModelState.IsValid)
            {
                var DOB = new DateTime().AddDays(vm.Day).AddMonths((int)vm.Month).AddYears(vm.Year);
                bool isValidAge = Common.DateUtilities.ValidateAge(DOB);

                if (isValidAge == false)
                {
                    ModelState.AddModelError("DOB", "You must be 18 years of age or above to sign up to our services");
                    return View(vm);
                }
                _senderSignUpServices.SetPersonalDetail(vm);

                return RedirectToAction("BusinessPersonalAddress", "SenderBusinessSignUp");
            }

            return View(vm);

        }

        public ActionResult BusinessPersonalAddress()
        {
            return View();

        }

        [HttpPost]
        public ActionResult BusinessPersonalAddress([Bind(Include = SenderPersonalAddressVM.BindProperty)]SenderPersonalAddressVM vm)
        {

            if (ModelState.IsValid)
            {
                _senderSignUpServices.SetPersonalAddress(vm);
                return RedirectToAction("BusinessDetails", "SenderBusinessSignUp");
            }

            return View(vm);

        }
        public ActionResult BusinessDetails()
        {
            return View();

        }
        [HttpPost]
        public ActionResult BusinessDetails([Bind(Include = SenderBusinessDetailsViewModel.BindProperty)]SenderBusinessDetailsViewModel vm)
        {
            if (ModelState.IsValid)
            {
                if (vm.BusinessType == 0)
                {
                    ModelState.AddModelError("BusinessType", "Select Business Type");
                    return View(vm);
                }
                _senderSignUpServices.SetSenderBusinessDetails(vm);
                return RedirectToAction("BusinessconfirmOperatingBusinessAddress", "SenderBusinessSignUp");
            }
            return View(vm);
        }
        public ActionResult BusinessconfirmRegisteredBusinessAddress()
        {


            var countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            return View();
        }
        [HttpPost]
        public ActionResult BusinessconfirmRegisteredBusinessAddress([Bind(Include = SenderBusinessRegisteredViewModel.BindProperty)]SenderBusinessRegisteredViewModel vm)
        {

            var countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            if (ModelState.IsValid)
            {
                _senderSignUpServices.SetSenderBusinessRegistered(vm);
                return RedirectToAction("BusinessconfirmOperatingBusinessAddress", "SenderBusinessSignUp");
            }

            return View(vm);

        }
        public ActionResult BusinessconfirmOperatingBusinessAddress(bool isSameAddress = false)
        {
            if (isSameAddress)
            {
                var savedPersonalAddressData = _senderSignUpServices.GetPersonalAddress();
                SenderBusinessOperatingViewModel vm = new SenderBusinessOperatingViewModel()
                {
                    OperatingAddressLine1 = savedPersonalAddressData.AddressLine1,
                    OperatingAddressLine2 = savedPersonalAddressData.AddressLine2,
                    OperatingCity = savedPersonalAddressData.City,
                    OperatingPostal = savedPersonalAddressData.PostCode,
                    IsSamePersonalAddress = true

                };
                return View(vm);
            }

            else
            {
                SenderBusinessOperatingViewModel vm = new SenderBusinessOperatingViewModel();
                return View(vm);
            }
            return View();


        }

        [HttpPost]
        public ActionResult BusinessconfirmOperatingBusinessAddress([Bind(Include = SenderBusinessOperatingViewModel.BindProperty)]SenderBusinessOperatingViewModel vm)
        {
            if (ModelState.IsValid)
            {
                _senderSignUpServices.SetSenderBusinessOperating(vm);
                _businessInfoServices.AccountSetUpCompleted();

                return RedirectToAction("Login", "faxeraccount", new { @IsBusiness = true });
                return RedirectToAction("Index", "SenderRegistrationCodeVerification");

            }
            return View(vm);
        }

    }
}