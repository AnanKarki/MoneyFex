using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentRegistrationController : Controller
    {
        AgentStaffRegistrationServices agentStaffRegistrationServices = null;
        DB.FAXEREntities dbContext = null;
        AgentCommonServices agentCommonServices = null;
        public AgentRegistrationController()
        {
            agentStaffRegistrationServices = new AgentStaffRegistrationServices();
            agentCommonServices = new AgentCommonServices();
            dbContext = new FAXEREntities();
        }
        // GET: Agent/AgentRegistration
        [HttpGet]
        public ActionResult Index(int AgentType = 0)
        {
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");
            AgentInformtionViewModel vm = new AgentInformtionViewModel();
            if (Common.AgentSession.AgentInformtionViewModel != null)
            {

                vm = Common.AgentSession.AgentInformtionViewModel;
            }
            if (AgentType == 1)
            {
                Common.AgentSession.IsAuxAgent = false; 
            }
            ViewBag.AgentType = AgentType;
            return View(vm);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = AgentInformtionViewModel.BindProperty)] AgentInformtionViewModel vm)
        {
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");
            if (ModelState.IsValid)
            {

                Common.AgentSession.AgentInformtionViewModel = vm;
                return RedirectToAction("StaaffDetails");
            }


            return View(vm);
        }

        public ActionResult BecomeAuxAgent(int agentType = 0)
        {
            Common.AgentSession.IsAuxAgent = true;
            return RedirectToAction("Index", "AgentRegistration", new { @AgentType = agentType });
        }
        public JsonResult GetCountryPhoneCode(string CountryCode)
        {

            var PhoneCode = Common.Common.GetCountryPhoneCode(CountryCode);
            return Json(new
            {
                PhoneCode = PhoneCode
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult StaaffDetails()
        {

            AgentResult agentResult = new AgentResult();
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");
            StaffDetailsViewModel vm = new StaffDetailsViewModel();
            if (Common.AgentSession.StaffDetailsViewModel != null)
            {

                vm = Common.AgentSession.StaffDetailsViewModel;
            }
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }

        [HttpPost]
        public ActionResult StaaffDetails([Bind(Include = StaffDetailsViewModel.BindProperty)] StaffDetailsViewModel vm)
        {

            AgentResult agentResult = new AgentResult();
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");

            if (ModelState.IsValid)
            {
                DateTime dateOfBirth = new DateTime();

                try
                {

                    dateOfBirth = new DateTime(vm.Year, (int)vm.Month, int.Parse(vm.Day));

                }
                catch (Exception)
                {

                    ModelState.AddModelError("Day", "Enter valid Date");
                    if ((int)vm.Month == 0)
                    {
                        ModelState.AddModelError("Month", "Select Month");
                        ViewBag.AgentResult = agentResult;
                        return View(vm);
                    }
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }

                var currentYear = DateTime.Now.Year;
                int age = currentYear - vm.Year;
                if (age <= 19)
                {

                    ModelState.AddModelError("InvalidAge", "To be an agent, you should be more than 19 years of age");

                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }

                Common.AgentSession.StaffDetailsViewModel = vm;
                return RedirectToAction("StaffContactDetails");
            }
            ViewBag.AgentResult = agentResult;

            return View(vm);
        }

        [HttpGet]
        public ActionResult StaffContactDetails()
        {
            var country = Common.Common.GetCountries();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");
            StaffContaactDetailsViewModel vm = new StaffContaactDetailsViewModel();
            vm.Country = Common.AgentSession.AgentInformtionViewModel.CountryCode;
            //to check for anan later
            //vm.Country = Common.AgentSession.BecomeAnAgent.CountryCode;
            vm.PhoneCode = Common.Common.GetCountryPhoneCode(Common.AgentSession.AgentInformtionViewModel.CountryCode);
            if (Common.AgentSession.StaffContaactDetailsViewModel != null)
            {

                vm = Common.AgentSession.StaffContaactDetailsViewModel;

            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult StaffContactDetails([Bind(Include = StaffContaactDetailsViewModel.BindProperty)] StaffContaactDetailsViewModel vm)
        {

            var country = Common.Common.GetCountries();
            ViewBag.Countries = new SelectList(country, "CountryCode", "CountryName");
            if (ModelState.IsValid)
            {
                if (!Common.Common.ValidatePassword(vm.Password))
                {

                    ModelState.AddModelError("Password", "Password Should Contain Atleast 1 UpperCase, 1 number and 1 Special Character");
                    return View(vm);
                }

                var isEmailUnique = agentStaffRegistrationServices.checkUniqueEmail(vm.EmailAddress);
                if (isEmailUnique == false)
                {
                    ModelState.AddModelError("EmailAddress", "This email address is already used");
                    return View(vm);
                }
                Common.AgentSession.StaffContaactDetailsViewModel = vm;

                return RedirectToAction("StaffComplianceDocumentation");
            }
            vm.Country = Common.AgentSession.AgentInformtionViewModel.CountryCode;
            vm.PhoneCode = Common.Common.GetCountryPhoneCode(Common.AgentSession.AgentInformtionViewModel.CountryCode);
            return View(vm);
        }

        [HttpGet]
        public ActionResult StaffComplianceDocumentation()
        {
            var cardTypes = Common.Common.GetIdCardType();
            ViewBag.IdCardType = new SelectList(cardTypes, "Name", "Name");
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");
            StaffComplianceDocViewModel vm = new StaffComplianceDocViewModel();
            if (Common.AgentSession.StaffComplianceDocViewModel != null)
            {
                vm = Common.AgentSession.StaffComplianceDocViewModel;

            }

            return View();
        }

        [HttpPost]
        public ActionResult StaffComplianceDocumentation([Bind(Include = StaffComplianceDocViewModel.BindProperty)] StaffComplianceDocViewModel vm)
        {
            var cardTypes = Common.Common.GetIdCardType();
            ViewBag.IdCardType = new SelectList(cardTypes, "Name", "Name");
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");

            if (Request.Files.Count < 1)
            {
                var identificationdoc = Request.Files["IdentificationDoc"];

            }
            if (ModelState.IsValid)
            {

                if (vm.HaveInternationalPassport == true)
                {

                    if (string.IsNullOrEmpty(vm.PassportSide1) && string.IsNullOrEmpty(vm.PassportSide2))
                    {

                        ModelState.AddModelError("PassportSide2", "Please upload a copy of passport side 1 and side 2.");
                        return View(vm);
                    }
                }

                string identificationDocPath = "";
                var IdentificationDoc = Request.Files["IdentificationDoc"];
                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    identificationDocPath = Guid.NewGuid() + "." + IdentificationDoc.FileName.Split('.')[1];
                    IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + identificationDocPath);
                    vm.IdentificationDoc = "/Documents/" + identificationDocPath;

                }
                string Passport1path = "";
                var Passport1 = Request.Files["PassportSide1"];
                if (Passport1 != null && Passport1.ContentLength > 0)
                {
                    Passport1path = Guid.NewGuid() + "." + Passport1.FileName.Split('.')[1];
                    IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + Passport1path);
                    vm.PassportSide1 = "/Documents/" + Passport1path;
                }
                string Passport2path = "";
                var Passport2 = Request.Files["PassportSide2"];
                if (Passport2 != null && Passport2.ContentLength > 0)
                {
                    Passport2path = Guid.NewGuid() + "." + Passport2.FileName.Split('.')[1];
                    Passport2.SaveAs(Server.MapPath("~/Documents") + "\\" + Passport2path);
                    vm.PassportSide2 = "/Documents/" + Passport2path;
                }

                Common.AgentSession.StaffComplianceDocViewModel = vm;

                var AgentstaffDetails = Common.AgentSession.StaffDetailsViewModel;
                var AgentStaffContactDetails = Common.AgentSession.StaffContaactDetailsViewModel;

                bool modelIsValid = true;
                DateTime dateOfBirth = new DateTime();

                try
                {
                    dateOfBirth = new DateTime(AgentstaffDetails.Year, (int)AgentstaffDetails.Month, int.Parse(AgentstaffDetails.Day));

                }
                catch (Exception)
                {
                    modelIsValid = false;
                    ModelState.AddModelError("Day", "ID already expired");
                }

                DateTime ExpiryDate = new DateTime();
                try
                {
                    ExpiryDate = new DateTime(vm.ExpiryYear, (int)vm.ExpiryMonth, vm.ExpiryDay);

                    if (ExpiryDate <= DateTime.Now)
                    {


                        modelIsValid = false;
                        ModelState.AddModelError("ExpiryDay", "ID already expired");

                    }
                }
                catch (Exception)
                {

                    modelIsValid = false;
                    ModelState.AddModelError("ExpiryDay", "ID already expired");
                }


                if (modelIsValid == false)
                {

                    return View(vm);
                }

                var agentInformation = Common.AgentSession.AgentInformtionViewModel;
                var agentInformationResult = agentStaffRegistrationServices.CreateAgent(agentInformation);
                DB.AgentStaffInformation agentStaffInformation = new DB.AgentStaffInformation()
                {
                    FirstName = AgentstaffDetails.FirstName,
                    MiddleName = AgentstaffDetails.MiddleName,
                    LastName = AgentstaffDetails.LastName,
                    DateOfBirth = dateOfBirth,
                    BirthCountry = AgentstaffDetails.BirthCountry,
                    Gender = AgentstaffDetails.Gender,
                    Address1 = AgentStaffContactDetails.Address1,
                    Address2 = AgentStaffContactDetails.Address2,
                    City = AgentStaffContactDetails.City,
                    Country = AgentStaffContactDetails.Country,
                    State = AgentStaffContactDetails.State,
                    PostalCode = AgentStaffContactDetails.ZipCode,
                    EmailAddress = AgentStaffContactDetails.EmailAddress,
                    AgentId = agentInformationResult.Id,
                    PhoneNumber = AgentStaffContactDetails.PhoneNumber,
                    HasPassport = vm.HaveInternationalPassport,
                    IdCardExpiryDate = ExpiryDate,
                    IdCardNumber = vm.IdCardNumber,
                    IssuingCountry = vm.IssuingCountry,
                    IdCardType = vm.IdCaardType,
                    IDDocPhoto = vm.IdentificationDoc,
                    Passport1Photo = vm.PassportSide1,
                    Passport2Photo = vm.PassportSide2,
                    AgentStaffType = StaffType.Admin,
                    AgentMFSCode = agentInformationResult.AccountNo,
                    CreatedDate = DateTime.Now
                };
                var model = agentStaffRegistrationServices.CreateAgentStaff(agentStaffInformation);


                string activationCode = Guid.NewGuid().ToString();


                AgentLogin agentLogin = new AgentLogin
                {
                    ActivationCode = Guid.NewGuid().ToString(),
                    AgentId = model.AgentId,
                    IsActive = false,
                    LoginCode = agentCommonServices.GetAgentLoginCode(),
                    LoginFailedCount = 0,
                    Password = "",
                    Username = Common.AgentSession.StaffContaactDetailsViewModel.EmailAddress,

                };

                Common.AgentSession.AgentLogin = agentLogin;

                var agentLoginModel = agentStaffRegistrationServices.CreateAgentLogin(agentLogin);

                string loginCode = agentCommonServices.GetStaffLoginCode();

                if (loginCode == agentLogin.LoginCode)
                {
                    loginCode = agentCommonServices.GetStaffLoginCode();
                }
                DB.AgentStaffLogin agentStaffLogin = new DB.AgentStaffLogin()
                {
                    Username = model.EmailAddress,
                    Password = AgentStaffContactDetails.Password.Encrypt(),
                    AgentStaffId = model.Id,
                    ActivationCode = activationCode,
                    AgencyLoginCode = agentLogin.LoginCode,
                    IsActive = false,
                    LoginFailedCount = 0,
                    StaffLoginCode = loginCode,
                    StartTime = "9:00",
                    EndTime = "5:00",
                    StartDay = DayOfWeek.Monday,
                    EndDay = DayOfWeek.Saturday,


                };
                var loginModel = agentStaffRegistrationServices.CreateAgentStaffLogin(agentStaffLogin);

                agentStaffRegistrationServices.AddAgentDocument(vm, agentStaffLogin.AgentStaffId);
                MailCommon mail = new MailCommon();
                //send mail to admin as activation code
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);


                try
                {
                    string mailMessage = "";
                    string body = "";
                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/AgentActivationEmail?NameOfContactPerson=" + model.FirstName

                                                        + "&EmailAddress=" + agentStaffInformation.EmailAddress
                                                      + "&AgencyLoginCode=" + agentStaffLogin.AgencyLoginCode
                                                      + "&StaffLoginCode=" + loginCode
                                                      + "&NameOfBusiness=" + agentInformation.AgentName);

                    mail.SendMail(agentStaffInformation.EmailAddress, "Agent Registration Confirmation", body);
                    //end 

                }
                catch (Exception)
                {


                }
                #region SMS
                SmsApi smsSerives = new SmsApi();
                var sms = smsSerives.GetStaffLoginCodeMessage(model.FirstName, agentStaffLogin.StaffLoginCode);
                var phonenumber = Common.Common.GetCountryPhoneCode(model.Country) + model.PhoneNumber;
                smsSerives.SendSMS(phonenumber, sms);
                #endregion
                return RedirectToAction("LoginMessage");
            }
            return View(vm);

        }

        [HttpGet]
        public ActionResult LoginMessage()
        {
            AgentCommonServices cs = new AgentCommonServices();
            cs.ClearBecomeAAgent();
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }





    }
}