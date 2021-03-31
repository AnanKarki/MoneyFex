using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels.SignUpViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessSignUpController : Controller
    {

        KiiPayBusinessSignUpServices _kiiPayBusinessSignUpServices = null;
        public KiiPayBusinessSignUpController()
        {
            _kiiPayBusinessSignUpServices = new KiiPayBusinessSignUpServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessSignUp
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult BusinessLoginInformaton() {

            ViewBag.Countries = new SelectList( Common.Common.GetCountriesForDropDown() , "CountryCode" , "CountryName" ) ;
            var vm = _kiiPayBusinessSignUpServices.GetKiiPayBusinessLoginInformation();
            return View(vm);
        }

        [HttpPost]
        public ActionResult BusinessLoginInformaton([Bind(Include = BusinessLoginInformationVM.BindProperty)]BusinessLoginInformationVM vm)
        {


            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");

            if (ModelState.IsValid) {

                if (IsMobileNoDuplicate(vm.MobileNo))
                {
                    ModelState.AddModelError("MobileNo", "The mobile no. has already been registered...");
                }
                else if (IsEmailDuplicate(vm.EmailAddress))
                {
                    ModelState.AddModelError("EmailAddress", "The email address has already been registered...");
                }
                else
                {
                    _kiiPayBusinessSignUpServices.SetKiiPayBusinessLoginInformation(vm);
                    
                    return RedirectToAction("PersonalInfo");

                }
            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult PersonalInfo() {

            var vm = _kiiPayBusinessSignUpServices.GetPersonalInfo();
            return View(vm);
        }
        [HttpPost]
        public ActionResult PersonalInfo([Bind(Include = KiiPayBusinessPersonalInfoVM.BindProperty)]KiiPayBusinessPersonalInfoVM vm)
        {
            if (ModelState.IsValid) {

                // Set Personal Info in session 

                _kiiPayBusinessSignUpServices.SetPersonalInfo(vm);
                return RedirectToAction("PersnalAddressInfo");
            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult PersnalAddressInfo() {

            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");

            var vm = _kiiPayBusinessSignUpServices.GetPersnalAddressInfo();
            return View(vm);
        }
        [HttpPost]
        public ActionResult PersnalAddressInfo([Bind(Include = AddressVM.BindProperty)]AddressVM vm)
        {

            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");
            if (ModelState.IsValid) {

                _kiiPayBusinessSignUpServices.SetPersnalAddressInfo(vm);
                return RedirectToAction("BusinessInfo");
            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult BusinessInfo() {

            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");
            var vm = _kiiPayBusinessSignUpServices.GetkiiPayBusinessInfo();
            return View(vm);
        }
        [HttpPost]
        public ActionResult BusinessInfo([Bind(Include = kiiPayBusinessInfoVM.BindProperty)]kiiPayBusinessInfoVM vm)
        {


            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");
            if (ModelState.IsValid) {

                _kiiPayBusinessSignUpServices.SetkiiPayBusinessInfo(vm);
                return RedirectToAction("BusinessAddressInfo");
            }
            return View(vm);
        }
        public ActionResult BusinessAddressInfo() {


            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");
            var vm = _kiiPayBusinessSignUpServices.GetBusinessAddressInfo();
            return View(vm);
        }
        [HttpPost]
        public ActionResult BusinessAddressInfo([Bind(Include = AddressVM.BindProperty)]AddressVM vm)
        {


            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");
            if (ModelState.IsValid) {
                // set 
                _kiiPayBusinessSignUpServices.SetBusinessAddressInfo(vm);
                return RedirectToAction("BusinessOpeationAddressInfo");
            }

            return View();
        }
        [HttpGet]
        public ActionResult BusinessOpeationAddressInfo() {


            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");
            var vm = _kiiPayBusinessSignUpServices.GetBusinessOpeationAddressInfo();
            return View(vm);
        
        }

        [HttpPost]
        public ActionResult BusinessOpeationAddressInfo([Bind(Include = AddressVM.BindProperty)]AddressVM vm) {


            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");
            if (ModelState.IsValid) {


                _kiiPayBusinessSignUpServices.SetBusinessOpeationAddressInfo(vm);
                return RedirectToAction("BillPaymentInfo");

            }
            return View(vm);

        }

        [HttpGet]
        public JsonResult GetAddressInfo() {

            var BusinessOpeationAddressInfo = _kiiPayBusinessSignUpServices.GetBusinessAddressInfo();
            return Json(new
            {
                AddressLine1 = BusinessOpeationAddressInfo.AddressLine1,
                AddressLine2 = BusinessOpeationAddressInfo.AddressLine2,
                City = BusinessOpeationAddressInfo.City,
                PostcodeORZipCode = BusinessOpeationAddressInfo.PostcodeORZipCode,
                Country = BusinessOpeationAddressInfo.Country,
            }, JsonRequestBehavior.AllowGet);

        }
        /// <summary>
        /// Does your company issue bills to customers?
        /// Yes or No(if yes then the business will be registered as supplier)
        /// </summary>
        [HttpGet]
        public ActionResult BillPaymentInfo() {

            return View();
        }

        public ActionResult BillPaymentInfo([Bind(Include = KiiPayBusinessBillPaymentInfoVM.BindProperty)]KiiPayBusinessBillPaymentInfoVM vm) {

            if (ModelState.IsValid) {

                _kiiPayBusinessSignUpServices.CompleteBuinessRegistration(vm);
                return RedirectToAction("Index" , "SenderRegistrationCodeVerification", new { Area=""});
            }
            return View(vm);
        }
        //public ActionResult RegistrationVerification() {

        //    return View();

        //}

        public JsonResult GetCountryPhoneCode(string CountryCode) {

            string PhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {

                PhoneCode = PhoneCode
            }, JsonRequestBehavior.AllowGet);
        }

        private bool IsMobileNoDuplicate(string MobileNo) {

            KiiPayBusinessSignUpServices kiiPayBusinessSignUpServices = new KiiPayBusinessSignUpServices();
            bool result = kiiPayBusinessSignUpServices.IsMobileNoDuplicate(MobileNo);
            return result;
        }

      
        private bool IsEmailDuplicate(string email) {



            KiiPayBusinessSignUpServices kiiPayBusinessSignUpServices = new KiiPayBusinessSignUpServices();
            bool result = kiiPayBusinessSignUpServices.IsEmailDuplicate(email);
            return result;
        }
    }
}