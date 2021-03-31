using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentSystemUserController : Controller
    {
        AgentStaffRegistrationServices agentStaffRegistrationServices = null;
        DB.FAXEREntities dbContext = null;
        AgentCommonServices agentCommonServices = null;
        ComplianceCommissionerServices complianceCommissionerServices = null;
        public AgentSystemUserController()
        {
            dbContext = new FAXEREntities();
            agentStaffRegistrationServices = new AgentStaffRegistrationServices();
            agentCommonServices = new AgentCommonServices();
            complianceCommissionerServices = new ComplianceCommissionerServices();



        }


        // GET: Agent/AgentSystemUser
        [HttpGet]
        public ActionResult Index()
        {

            if (Common.AgentSession.AgentStaffLogin == null)
            {
                return RedirectToAction("Login", "AgentLogin");
            }
            if (Common.AgentSession.AgentStaffLogin.AgentStaff.AgentStaffType == StaffType.Transaction)
            {
                return RedirectToAction("Index", "AgentDashboard");
            }
            var agentstaff = (from c in agentCommonServices.GetAgentStaffLoginInfo().ToList()
                              select new LoginTimeAndDayAccessVm()
                              {
                                  StaffName = c.AgentStaff.FirstName + " " + c.AgentStaff.MiddleName + " " + c.AgentStaff.LastName,
                                  EndTime = c.EndTime == null ? "00" : c.EndTime,
                                  EndDay = c.EndDay,
                                  Id = c.Id,
                                  StaaffType = c.AgentStaff.AgentStaffType,
                                  StartDay = c.StartDay,
                                  StartTime = c.StartTime == null ? "00" : c.StartTime,
                                  Status = c.IsActive == true ? "Active " : "DeActive",

                              }).ToList();
            return View(agentstaff);
        }

        [HttpPost]
        public ActionResult Index([Bind(Include = LoginTimeAndDayAccessVm.BindProperty)] LoginTimeAndDayAccessVm vm)
        {
            var agentStaffLogin = agentStaffRegistrationServices.GetAgentStaffLogin(vm.Id);
            agentStaffLogin.StartTime = vm.StartTime;
            agentStaffLogin.EndTime = vm.EndTime;
            agentStaffLogin.StartDay = vm.StartDay;
            agentStaffLogin.EndDay = vm.EndDay;
            agentStaffLogin.LoginFailedCount = 0;
            agentStaffLogin.UpdataedBy = Common.AgentSession.AgentStaffLogin.AgentStaff.Id;
            agentStaffLogin.UpdataedDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                var result = agentStaffRegistrationServices.UpdateAgentStaffLogin(agentStaffLogin);
                return RedirectToAction("Index");
            }

            return View(vm);

        }


        [HttpGet]
        public ActionResult AgentUserRegistration(int AgentStaffLoginId = 0)
        {

            if (Common.AgentSession.AgentStaffLogin == null)
            {
                return RedirectToAction("Login", "AgentLogin");
            }
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");
            AgentUserDetailsViewModel vm = new AgentUserDetailsViewModel();

            if (AgentStaffLoginId != 0)
            {
                var agentStaffLogin = agentStaffRegistrationServices.GetAgentStaffLogin(AgentStaffLoginId);
                vm = new AgentUserDetailsViewModel()
                {
                    CountryCode = agentStaffLogin.AgentStaff.BirthCountry,
                    Day = agentStaffLogin.AgentStaff.DateOfBirth.Day.ToString(),
                    FirstName = agentStaffLogin.AgentStaff.FirstName,
                    MiddleName = agentStaffLogin.AgentStaff.MiddleName,
                    LastName = agentStaffLogin.AgentStaff.LastName,
                    Gender = agentStaffLogin.AgentStaff.Gender,
                    Id = agentStaffLogin.AgentStaff.Id,
                    AgentStaffId = agentStaffLogin.AgentStaff.Id,
                    Month = (Months)agentStaffLogin.AgentStaff.DateOfBirth.Month,
                    Year = agentStaffLogin.AgentStaff.DateOfBirth.Year.ToString(),

                };
                Common.AgentSession.AgentUserInformation = vm;
                AgentUserContactDetailsViewModel agentUserContactDetals = new AgentUserContactDetailsViewModel()
                {
                    Address1 = agentStaffLogin.AgentStaff.Address1,
                    Address2 = agentStaffLogin.AgentStaff.Address2,
                    EmailAddress = agentStaffLogin.AgentStaff.EmailAddress,
                    City = agentStaffLogin.AgentStaff.City,
                    ConfirmPassword = "",
                    CountryCode = agentStaffLogin.AgentStaff.Country,
                    Id = agentStaffLogin.AgentStaff.Id,
                    Password = agentStaffLogin.Password,
                    PhoneNumber = agentStaffLogin.AgentStaff.PhoneNumber,
                    State = agentStaffLogin.AgentStaff.State,
                    ZipCode = agentStaffLogin.AgentStaff.PostalCode,
                    StaffType = agentStaffLogin.AgentStaff.AgentStaffType,
                    AgentStaffId = agentStaffLogin.AgentStaff.Id
                };
                Common.AgentSession.AgentUserContactDetails = agentUserContactDetals;


                StaffComplianceDocViewModel staffComplinceDoc = new StaffComplianceDocViewModel()
                {
                    ExpiryDay = agentStaffLogin.AgentStaff.IdCardExpiryDate.Day,
                    ExpiryMonth = (Month)agentStaffLogin.AgentStaff.IdCardExpiryDate.Month,
                    ExpiryYear = agentStaffLogin.AgentStaff.IdCardExpiryDate.Year,
                    HaveInternationalPassport = agentStaffLogin.AgentStaff.HasPassport,
                    Id = agentStaffLogin.AgentStaff.Id,
                    IdCaardType = agentStaffLogin.AgentStaff.IdCardType,
                    IdCardNumber = agentStaffLogin.AgentStaff.IdCardNumber,
                    IdentificationDoc = agentStaffLogin.AgentStaff.IDDocPhoto,
                    IssuingCountry = agentStaffLogin.AgentStaff.IssuingCountry,
                    PassportSide1 = agentStaffLogin.AgentStaff.Passport1Photo,
                    PassportSide2 = agentStaffLogin.AgentStaff.Passport2Photo,
                    AgentStaffId = agentStaffLogin.AgentStaff.Id
                };
                Common.AgentSession.StaffComplianceDocViewModel = staffComplinceDoc;

            }

            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName", vm.CountryCode);


            return View(vm);
        }

        [HttpPost]
        public ActionResult AgentUserRegistration([Bind(Include = AgentUserDetailsViewModel.BindProperty)] AgentUserDetailsViewModel vm)
        {
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");
            if (ModelState.IsValid)
            {
                var dateOfBirth = new DateTime();
                try
                {
                    dateOfBirth = new DateTime(int.Parse(vm.Year), (int)vm.Month, int.Parse(vm.Day));

                }
                catch (Exception)
                {

                    ModelState.AddModelError("Day", "Enter valid Date");
                    return View(vm);
                }
                bool isValidAge = Common.DateUtilities.ValidateAge(dateOfBirth);

                if (isValidAge == false)
                {
                    ModelState.AddModelError("DOB", "To be an agent staff, agent should be more than 18 years of age");
                    return View(vm);
                }
                Common.AgentSession.AgentUserInformation = vm;
                return RedirectToAction("AgentUserContactDetails");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AgentUserContactDetails()
        {
            if (Common.AgentSession.AgentStaffLogin == null)
            {
                return RedirectToAction("Login", "AgentLogin");
            }
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");
            AgentUserContactDetailsViewModel vm = new AgentUserContactDetailsViewModel();
            vm.CountryCode = Common.AgentSession.AgentInformation.CountryCode;
            vm.PhoneCode = Common.Common.GetCountryPhoneCode(Common.AgentSession.AgentInformation.CountryCode);
            if (Common.AgentSession.AgentUserContactDetails != null)
            {
                vm = Common.AgentSession.AgentUserContactDetails;
                vm.PhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);
                return View(vm);
            }


            return View(vm);
        }

        [HttpPost]
        public ActionResult AgentUserContactDetails([Bind(Include = AgentUserContactDetailsViewModel.BindProperty)] AgentUserContactDetailsViewModel vm)
        {

            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");
            if (ModelState.IsValid)
            {
                if (vm.AgentStaffId == 0)
                {
                    if (string.IsNullOrEmpty(vm.EmailAddress))
                    {

                        ModelState.AddModelError("EmailAddress", "Please enter the email address");
                        return View(vm);
                    }
                    if (string.IsNullOrEmpty(vm.Password))
                    {


                        ModelState.AddModelError("Password", "Please enter the email address");
                        return View(vm);
                    }
                    //if (string.IsNullOrEmpty(vm.ConfirmPassword))
                    //{

                    //    ModelState.AddModelError("ConfirmPassword", "Please enter the email address");
                    //    return View(vm);
                    //}

                    //if (vm.ConfirmPassword != vm.Password)
                    //{

                    //    ModelState.AddModelError("ConfirmPassword", "password did not match");
                    //    return View(vm);
                    //}
                    if (!string.IsNullOrEmpty(vm.Password) && !Common.Common.ValidatePassword(vm.Password))
                    {

                        ModelState.AddModelError("Password", "Password Should Contain Atleast 1 UpperCase, 1 number and 1 Special Character");
                        return View(vm);
                    }
                    var isEmailUnique = agentStaffRegistrationServices.checkUniqueEmail(vm.EmailAddress, Common.AgentSession.AgentUserInformation.AgentStaffId);
                    if (isEmailUnique == false)
                    {
                        ModelState.AddModelError("EmailAddress", "This email address is already used");
                        return View(vm);
                    }
                }




                Common.AgentSession.AgentUserContactDetails = vm;
                return RedirectToAction("AgentUserComplianceDocumentation");
            }
            return View(vm);
        }


        public JsonResult GeneratePassword()
        {

            string Password = Common.Common.GenerateRandomString(8);

            return Json(new
            {

                Password = Password
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult AgentUserComplianceDocumentation()
        {
            if (Common.AgentSession.AgentStaffLogin == null)
            {
                return RedirectToAction("Login", "AgentLogin");
            }

            var cardTypes = Common.Common.GetIdCardType();
            ViewBag.IdCardType = new SelectList(cardTypes, "Id", "Name");
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");

            StaffComplianceDocViewModel vm = new StaffComplianceDocViewModel();

            if (Common.AgentSession.StaffComplianceDocViewModel != null)
            {
                vm = Common.AgentSession.StaffComplianceDocViewModel;
                return View(vm);
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult AgentUserComplianceDocumentation([Bind(Include = StaffComplianceDocViewModel.BindProperty)] StaffComplianceDocViewModel vm)
        {
            var cardTypes = Common.Common.GetIdCardType();
            ViewBag.IdCardType = new SelectList(cardTypes, "Id", "Name");
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");
            DateTime ExpiryDate = new DateTime();
            if (Request.Files.Count < 1)
            {
                var identificationdoc = Request.Files["ResidentPermit"];

            }
            if (ModelState.IsValid)
            {
                try
                {
                    ExpiryDate = new DateTime(vm.ExpiryYear, (int)vm.ExpiryMonth, vm.ExpiryDay);

                }
                catch (Exception)
                {

                    ModelState.AddModelError("ExpiryDay", "Enter valid Date");
                    return View(vm);
                }
                string identificationDocPath = "";
                var IdentificationDoc = Request.Files["IdentificationDoc"];
                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    identificationDocPath = Guid.NewGuid() + "." + IdentificationDoc.FileName.Split('.')[1];
                    IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + identificationDocPath);

                }
                string Passport1path = "";
                var Passport1 = Request.Files["PassportSide1"];
                if (Passport1 != null && Passport1.ContentLength > 0)
                {
                    Passport1path = Guid.NewGuid() + "." + Passport1.FileName.Split('.')[1];
                    IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + Passport1path);

                }
                string Passport2path = "";
                var Passport2 = Request.Files["PassportSide1"];
                if (Passport2 != null && Passport2.ContentLength > 0)
                {
                    Passport2path = Guid.NewGuid() + "." + Passport2.FileName.Split('.')[1];
                    Passport2.SaveAs(Server.MapPath("~/Documents") + "\\" + Passport2path);

                }

                if (!string.IsNullOrEmpty(identificationDocPath))
                {

                    vm.IdentificationDoc = "/Documents/" + identificationDocPath;
                }
                if (!string.IsNullOrEmpty(Passport1path))
                {

                    vm.PassportSide1 = "/Documents/" + Passport1path;
                }
                if (!string.IsNullOrEmpty(Passport2path))
                {

                    vm.PassportSide2 = "/Documents/" + Passport2path;
                }

                Common.AgentSession.StaffComplianceDocViewModel = vm;

                var AgentUserDetails = Common.AgentSession.AgentUserInformation;

                //var AgentUserDetails = Common.AgentSession.AgentInformation;

                var AgentUserContactDetails = Common.AgentSession.AgentUserContactDetails;

                DateTime dateOfBirth = new DateTime();
                //dateOfBirth.AddYears(AgentUserDetails.Year);
                //dateOfBirth.AddMonths((int)AgentUserDetails.Month);
                //dateOfBirth.AddDays(double.Parse(AgentUserDetails.Day));
                try
                {
                    dateOfBirth = new DateTime(int.Parse(AgentUserDetails.Year), (int)AgentUserDetails.Month, int.Parse(AgentUserDetails.Day));

                }
                catch (Exception)
                {

                }
                //DateTime ExpiryDate = new DateTime();
                //ExpiryDate.AddYears(vm.ExpiryYear);
                //ExpiryDate.AddMonths((int)vm.ExpiryMonth);
                //ExpiryDate.AddDays(vm.ExpiryDay);


                DB.AgentStaffInformation agentStaffInformation = new DB.AgentStaffInformation()
                {
                    FirstName = AgentUserDetails.FirstName,
                    MiddleName = AgentUserDetails.MiddleName,
                    LastName = AgentUserDetails.LastName,
                    DateOfBirth = dateOfBirth,
                    BirthCountry = AgentUserDetails.CountryCode.ToString(),
                    Gender = AgentUserDetails.Gender,
                    Address1 = AgentUserContactDetails.Address1,
                    Address2 = AgentUserContactDetails.Address2,
                    City = AgentUserContactDetails.City,
                    Country = AgentUserContactDetails.CountryCode,
                    State = AgentUserContactDetails.State,
                    PostalCode = AgentUserContactDetails.ZipCode,
                    EmailAddress = AgentUserContactDetails.EmailAddress,
                    AgentId = Common.AgentSession.AgentInformation.Id,
                    PhoneNumber = AgentUserContactDetails.PhoneNumber,
                    HasPassport = vm.HaveInternationalPassport,
                    IdCardExpiryDate = ExpiryDate,
                    IdCardNumber = vm.IdCardNumber,
                    IssuingCountry = vm.IssuingCountry,
                    IdCardType = vm.IdCaardType,
                    IDDocPhoto = vm.IdentificationDoc,
                    Passport1Photo = vm.PassportSide1,
                    Passport2Photo = vm.PassportSide2,
                    AgentStaffType = AgentUserContactDetails.StaffType,
                    AgentMFSCode = agentCommonServices.AgentMFSCode(),
                    Id = vm.Id
                };
                
                if (agentStaffInformation.Id != 0)
                {

                    agentStaffInformation.UpdatedBy = Common.AgentSession.AgentStaffLogin.AgentStaffId;
                    agentStaffInformation.UpdatedDate = DateTime.Now;
                    var model = agentStaffRegistrationServices.UpdateAgentStaffInformation(agentStaffInformation);

                    var AgentStaffLogin = agentStaffRegistrationServices.GetAgentStaffLoginbYagentStaffId(model.Id);
                    AgentStaffLogin.Username = model.EmailAddress;

                    var staffLogin = agentStaffRegistrationServices.UpdateAgentStaffLogin(AgentStaffLogin);
                }
                else
                {

                    agentStaffInformation.CreatedBy = Common.AgentSession.AgentStaffLogin.AgentStaffId;
                    agentStaffInformation.CreatedDate = DateTime.Now;
                    var model = agentStaffRegistrationServices.CreateAgentStaff(agentStaffInformation);

                    AgentStaffLogin agentStaffLogin = new AgentStaffLogin()
                    {
                        ActivationCode = Guid.NewGuid().ToString(),
                        AgencyLoginCode = Common.AgentSession.AgentStaffLogin.AgencyLoginCode,
                        AgentStaffId = model.Id,
                        IsActive = false,
                        LoginFailedCount = 0,
                        Password = AgentUserContactDetails.Password.Encrypt(),
                        StaffLoginCode = agentCommonServices.GetStaffLoginCode(),
                        Username = agentStaffInformation.EmailAddress,
                        StartTime = "09:00",
                        EndTime = "17:00",
                        StartDay = DayOfWeek.Monday,
                        EndDay = DayOfWeek.Saturday,
                        IsFirstLogin = true
                    };

                    var staffLogin = agentStaffRegistrationServices.CreateAgentStaffLogin(agentStaffLogin);

                    MailCommon mail = new MailCommon();
                    //send mail to admin as activation code
                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);


                    //string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/AgentActivationEmail?NameOfContactPerson=" + model.FirstName
                    //                                   + " " + model.MiddleName + " " + model.LastName
                    //                                   + "&EmailAddress=" + agentStaffInformation.EmailAddress
                    //                                   + "&LoginCode=" + agentStaffLogin.StaffLoginCode + "&Link=" + "" + "&AgencyLoginCode=" + Common.AgentSession.AgentStaffLogin.AgencyLoginCode +
                    //                                   "&NameOfBusiness=" + agentStaffRegistrationServices.GetAgentInformation(agentStaffInformation.AgentId).Name + "&Password=" + AgentUserContactDetails.Password);

                    string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/AgentActivationEmail?NameOfContactPerson=" + model.FirstName
                                                       + " " + model.MiddleName + " " + model.LastName
                                                + "&EmailAddress=" + agentStaffInformation.EmailAddress
                                                + "&AgencyLoginCode=" + agentStaffLogin.AgencyLoginCode
                                                + "&StaffLoginCode=" + agentStaffLogin.StaffLoginCode
                                                + "&NameOfBusiness=" + " "
                                                + "&Password=" + " "
                                                + "&IsRegisteredByAdmin=" + false);


                    mail.SendMail(agentStaffInformation.EmailAddress, "Agent Registration Confirmation", body);
                    //end 



                }

               

                return RedirectToAction("LoginMessage");
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult LoginMessage()
        {
            if (Common.AgentSession.AgentStaffLogin == null)
            {
                return RedirectToAction("Login", "AgentLogin");
            }

            return View();
        }



        public ActionResult Activate(int AgentStaffLoginId)
        {
            var agentStaffLogin = agentStaffRegistrationServices.GetAgentStaffLogin(AgentStaffLoginId);
            agentStaffLogin.IsActive = true;
            agentStaffLogin.LoginFailedCount = 0;
            agentStaffLogin.UpdataedBy = Common.AgentSession.AgentStaffLogin.AgentStaff.Id;
            agentStaffLogin.UpdataedDate = DateTime.Now;
            var result = agentStaffRegistrationServices.UpdateAgentStaffLogin(agentStaffLogin);

            return RedirectToAction("Index");
        }

        public ActionResult AddLoginTime([Bind(Include = LoginTimeAndDayAccessVm.BindProperty)] LoginTimeAndDayAccessVm vm)
        {
            var agentStaffLogin = agentStaffRegistrationServices.GetAgentStaffLogin(vm.Id);
            agentStaffLogin.StartTime = vm.StartTime;
            agentStaffLogin.EndTime = vm.EndTime;
            agentStaffLogin.LoginFailedCount = 0;
            agentStaffLogin.UpdataedBy = Common.AgentSession.AgentStaffLogin.AgentStaff.Id;
            agentStaffLogin.UpdataedDate = DateTime.Now;
            var result = agentStaffRegistrationServices.UpdateAgentStaffLogin(agentStaffLogin);
            return RedirectToAction("Index");
        }



        public ActionResult DeActivate(int AgentStaffLoginId)
        {
            var agentStaffLogin = agentStaffRegistrationServices.GetAgentStaffLogin(AgentStaffLoginId);
            agentStaffLogin.IsActive = false;
            agentStaffLogin.UpdataedBy = Common.AgentSession.AgentStaffLogin.AgentStaff.Id;
            agentStaffLogin.UpdataedDate = DateTime.Now;
            var result = agentStaffRegistrationServices.UpdateAgentStaffLogin(agentStaffLogin);
            return RedirectToAction("Index");

        }

        public ActionResult ComplianceCommissioner(string message = "")
        {
            if (!string.IsNullOrEmpty(message))
            {
                if (message == "success")
                {
                    ViewBag.Message = "Added Successfully !";
                }
                else if (message == "failure")
                {
                    ViewBag.Message = "Something went wrong. Try Refreshing the page !";
                }
            }
            var agentStaffs = complianceCommissionerServices.getAgentStaffList();
            ViewBag.AgentStaffs = new SelectList(agentStaffs, "Id", "AgentStaffName");
            var vm = complianceCommissionerServices.getComplianceComissions();
            return View(vm);
        }

        public ActionResult AddComplianceCommission(int agentStaffId)
        {
            if (agentStaffId != 0)
            {
                bool save = complianceCommissionerServices.addComplianceCommission(agentStaffId);
                if (save)
                {
                    return RedirectToAction("ComplianceCommissioner", new { @message = "success" });
                }
            }
            return RedirectToAction("ComplianceCommissioner", new { @message = "failure" });
        }

        public ActionResult DeactivateComplianceCommission(int complianceCommissionId)
        {
            if (complianceCommissionId != 0)
            {
                bool save = complianceCommissionerServices.deactivateComplianceCommission(complianceCommissionId);
            }
            return RedirectToAction("ComplianceCommissioner");
        }


    }
}