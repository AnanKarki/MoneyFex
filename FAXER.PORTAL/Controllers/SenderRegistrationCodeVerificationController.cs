using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class SenderRegistrationCodeVerificationController : Controller
    {

        SRegistrationVerificationCode registrationVerificationCodeServices = null;
        public SenderRegistrationCodeVerificationController()
        {
            registrationVerificationCodeServices = new SRegistrationVerificationCode();
        }
        // GET: RegistrationCodeVerification
        [HttpGet]
        public ActionResult Index()
        {

            RegistrationCodeVerificationViewModel vm = Common.FaxerSession.RegistrationCodeVerificationViewModel;

            //RegistrationCodeVerificationViewModel vm = new RegistrationCodeVerificationViewModel()
            //{
            //    Country = "UK",
            //    Message = "Hello",
            //    PhoneNo = "+447440395950",
            //    RegistrationOf = DB.RegistrationOf.Sender,
            //    UserId = 5023,
            //    VerificationCode = "289987"
            //};

            //RegistrationCodeVerificationViewModel vm = new RegistrationCodeVerificationViewModel();
            //string email = Common.FaxerSession.FaxerInformation.Email;
            //var faxerInfo = registrationVerificationCodeServices.GetFaxerInformation(email);
            //string VerficationCode = Common.Common.GenerateVerificationCode(6);

            //if (Common.FaxerSession.RegistrationCodeVerificationViewModel == null)
            //{
            //    vm.Country = faxerInfo.Country;
            //    vm.PhoneNo = faxerInfo.PhoneNumber;
            //    vm.RegistrationOf = DB.RegistrationOf.Sender;
            //    vm.VerificationCode = VerficationCode;
            //    vm.UserId = faxerInfo.Id;

            //    registrationVerificationCodeServices.Add(vm);
            //    SFaxerSignUp _sFaxerSignUp = new SFaxerSignUp();
            //    _sFaxerSignUp.SendRegistrationEmail(faxerInfo.FirstName, faxerInfo.Email, faxerInfo.PhoneNumber, faxerInfo.AccountNo, vm.VerificationCode);

            //}
            //else
            //{
            //     vm = Common.FaxerSession.RegistrationCodeVerificationViewModel;
            //}

            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Registration Confirmation");
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = RegistrationCodeVerificationViewModel.BindProperty)]RegistrationCodeVerificationViewModel vm)
        {

            var IsValid = registrationVerificationCodeServices.IsValidVerificationCode(vm);

            if (!IsValid)
            {

                ModelState.AddModelError("ErrorMessage", "Please enter a valid code");

                return View(vm);
            }


            switch (vm.RegistrationOf)
            {
                case DB.RegistrationOf.Sender:
                    return RedirectToAction("Login", "FaxerAccount");
                case DB.RegistrationOf.KiiPayPersonal:

                    break;
                case DB.RegistrationOf.KiiPayBusiness:
                    //return RedirectToAction("Login", "BusinessLogin" , new { area = "Businesses" });
                    return RedirectToAction("Login", "KiiPayBusinessLogin", new { area = "KiiPayBusiness" });
                case DB.RegistrationOf.Staff:
                    break;
                case DB.RegistrationOf.Agent:
                    break;
                default:

                    return RedirectToAction("Login", "FaxerAccount");
            }
            return View(vm);



        }
        public ActionResult ReSendVerficationCode(string PhoneNo)
        {


            PhoneNo = "+" + PhoneNo.Trim();
            SmsApi smsApiServices = new SmsApi();
            var VerificationCode = registrationVerificationCodeServices.GetVerificationCode(PhoneNo);
            string message = smsApiServices.GetRegistrationMessage(VerificationCode);
            smsApiServices.SendSMS(PhoneNo, message);
            return RedirectToAction("Index");
        }
    }
}