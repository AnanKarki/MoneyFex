using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentProfileController : Controller
    {
        CommonServices CommonService = null;
        AgentProfileServices _services = null;

        public AgentProfileController()
        {
            _services = new AgentProfileServices();
            CommonService = new CommonServices();
        }
        // GET: Agent/AgentProfile
        [HttpGet]
        public ActionResult Index()
        {
            SetViewBagForCountries();
            var vm = _services.getAgentStaffProfileDetails();

            ViewBag.IsPinCodeSend = 0;
            ViewBag.PinCode = "";
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = AgentProfileViewModel.BindProperty)] AgentProfileViewModel model)
        {
            SetViewBagForCountries();
            #region old design


            //bool isValid = true;
            //if (string.IsNullOrEmpty(Common.AgentSession.UserEnterAccountVerficationCode))
            //{
            //    ModelState.AddModelError("VerificationError", "Please enter your address !");
            //    TempData["InvalidInformation"] = true;
            //    return View(model);
            //}
            //if (Common.AgentSession.UserEnterAccountVerficationCode.Trim() != Common.AgentSession.VerificationCode.Trim())
            //{
            //    ModelState.AddModelError("VerificationError", "Please enter the verification code sent to your phone !");
            //    TempData["InvalidInformation"] = true;
            //    return View(model);
            //}
            //if (string.IsNullOrEmpty(model.Address1))
            //{
            //    ModelState.AddModelError("Address1", "Please enter your Address !");
            //    isValid = false;
            //}
            //if (string.IsNullOrEmpty(model.City))
            //{
            //    ModelState.AddModelError("City", "Please enter your City !");
            //    isValid = false;
            //}
            //if (string.IsNullOrEmpty(model.State))
            //{
            //    ModelState.AddModelError("State", "Plese enter your State !");
            //    isValid = false;
            //}
            //if (string.IsNullOrEmpty(model.PostalCode))
            //{
            //    ModelState.AddModelError("PostalCode", "Please enter your Postal Code !");
            //    isValid = false;
            //}
            //if (isValid)
            //{
            //    var data = _services.updateAgentStaffAddressDetails(model);
            //    if (data)
            //    {
            //        MailCommon mail = new MailCommon();
            //        var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            //        string body = "";
            //        mail.SendMail(model.Email, "Your address details have been updated !", body);
            //        return RedirectToAction("Index", "AgentProfile", new { @Area = "Agent" });
            //    }
            //}

            //TempData["InvalidInformation"] = true;
            #endregion

            model = _services.getAgentStaffProfileDetails();
            ViewBag.IsPinCodeSend = 1;
            ViewBag.PinCode = _services.GetMobilePin();
            return View(model);
        }


        public ActionResult UpdateAgentProfile(string PinCode = "")
        {
            ViewBag.IsPinCodeSend = 1;
            if (PinCode != _services.GetMobilePin())
            {
                return RedirectToAction("Index");

            }
            var vm = _services.getAgentStaffProfileDetails();
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateAgentProfile([Bind(Include = AgentProfileViewModel.BindProperty)]AgentProfileViewModel vm)
        {


            if (ModelState.IsValid)
            {
                var model = _services.getAgentStaffProfileDetails();
                if (model.PhoneNumber != vm.PhoneNumber)
                {
                    vm.PhoneNumber = Common.Common.IgnoreZero(vm.PhoneNumber);
                    bool IsMobileNoExist = Common.OtherUtlities.IsMobileNoExist(vm.PhoneNumber);
                    if (IsMobileNoExist == false)
                    {
                        ModelState.AddModelError("PhoneNumber", "Mobile number already exists");
                        return View(vm);
                    }
                }
                if (model.Email != vm.Email)
                {
                    vm.Email.Trim();
                    bool isEmailExist = Common.OtherUtlities.IsEmailExist(vm.Email);
                    if (isEmailExist == false)
                    {
                        ModelState.AddModelError("Email", "Email address already exists");
                        return View(vm);
                    }
                }
                _services.UpdateAgentProfile(vm);
                return RedirectToAction("Index", "AgentProfile");
            }
            return View(vm);
        }



        public JsonResult SendVerificationCode()
        {

            string VerificationCode = Common.Common.GenerateRandomDigit(8);
            Common.AgentSession.VerificationCode = VerificationCode;

            SmsApi smsApiServices = new SmsApi();
            var message = smsApiServices.GetAddressUpdateMessage(VerificationCode);
            string phoneNo = _services.getAgentStaffPhoneNumberWithCode();
            smsApiServices.SendSMS(phoneNo, message);


            TempData["AccountVerificationCodeSend"] = true;

            return Json(new
            {

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult VerifyAccountToEdit(string verificationCode)
        {


            bool AccountVerified = false;

            if (Common.AgentSession.VerificationCode.Trim() == verificationCode.Trim())
            {

                // when verification code entered by user is valid the temp value 1
                AccountVerified = true;
                Common.FaxerSession.UserEnterAccountVerficationCode = verificationCode;
            }

            return Json(new
            {
                AccountVerified = AccountVerified
            }, JsonRequestBehavior.AllowGet
            );
        }

        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public ActionResult UpdatePhoneNumber(int agentStaffId, string phoneNumber)
        {
            if (agentStaffId != 0 && !string.IsNullOrEmpty(phoneNumber))
            {
                bool updatePhone = _services.updatePhoneNumber(agentStaffId, phoneNumber);
            }
            return RedirectToAction("Index", "AgentProfile", new { @Area = "Agent" });
        }

        public ActionResult UpdateEmail(int agentStaffId, string email)
        {
            if (agentStaffId != 0 && !string.IsNullOrEmpty(email))
            {
                bool updateEmail = _services.updateEmail(agentStaffId, email);
            }
            return RedirectToAction("Index", "AgentProfile", new { @Area = "Agent" });
        }

        [HttpPost]
        public ActionResult UploadPhoto()
        {
            string photoUrl = "";
            string fileName = "";
            if (Request.Files.Count > 0)
            {
                var upload = Request.Files[0];
                string directory = Server.MapPath("/Documents");
                if (upload != null && upload.ContentLength > 0)
                {
                    fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1];
                    upload.SaveAs(Path.Combine(directory, fileName));
                    photoUrl = "/Documents/" + fileName;
                }
                if (fileName == "")
                {
                    TempData["ChooseCard"] = "Choose a file to upload";
                    return RedirectToAction("Index", "AgentProfile", new { @Area = "Agent" });
                }
                else
                {
                    bool savePhoto = _services.updatePhotoUrl(Common.AgentSession.AgentStaffLogin.AgentStaffId, photoUrl);
                }
            }
            return RedirectToAction("Index", "AgentProfile", new { @Area = "Agent" });
        }
    }



}