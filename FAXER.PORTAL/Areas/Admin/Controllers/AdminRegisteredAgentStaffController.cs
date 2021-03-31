using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AdminRegisteredAgentStaffController : Controller
    {
        AdminRegisteredAgentStaffServices _services = null;
        CommonServices _commonServices = null;
        public AdminRegisteredAgentStaffController()
        {
            _services = new AdminRegisteredAgentStaffServices();
            _commonServices = new CommonServices();
        }
        // GET: Admin/AdminRegisteredAgentStaff
        public ActionResult Index(string Country = "", string agentName = "", string staffName = "", string agentCode = "",
            string staffCode = "", string status = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name", Country);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<AdminRegisteredAgentStaffInfoViewModel> model = _services.GetAgentRegistered(Country).ToPagedList(pageNumber, pageSize);
            Common.AdminSession.AgentStaffInfoAndLoginViewModel = null;
            if (!string.IsNullOrEmpty(agentName))
            {
                agentName = agentName.Trim();
                model = model.Where(x => x.AgentName.ToLower().Contains(agentName.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(staffName))
            {
                staffName = staffName.Trim();
                model = model.Where(x => x.StaffName.ToLower().Contains(staffName.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(agentCode))
            {
                agentCode = agentCode.Trim();
                model = model.Where(x => x.AgentCode.ToLower().Contains(agentCode.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(staffCode))
            {
                staffCode = staffCode.Trim();
                model = model.Where(x => x.StaffCode.ToLower().Contains(staffCode.ToLower())).ToPagedList(pageNumber, pageSize);

            }   if (!string.IsNullOrEmpty(status))
            {
                status = status.Trim();
                model = model.Where(x => x.Status.ToLower().Contains(status.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            return View(model);
        }
        public ActionResult ActivateDeactivate(string AccountNumber)
        {
            if (!string.IsNullOrEmpty(AccountNumber))
            {
                _services.ActivateDeactivate(AccountNumber);
            }

            return RedirectToAction("Index", "AdminRegisteredAgentStaff");
        }

        public ActionResult AgentStaff()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            List<AdminRegisteredAgentStaffViewModel> model = _services.GetAgentAddedAgentStaffByAdmin();
            return View(model);
        }

        public ActionResult AddAgentStaff()
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            return View();
        }
        [HttpPost]
        public ActionResult AddAgentStaff([Bind(Include = AdminRegisteredAgentStaffViewModel.BindProperty)]AdminRegisteredAgentStaffViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            if (ModelState.IsValid)
            {

                //if (!string.IsNullOrEmpty(vm.Password) && !Common.Common.ValidatePassword(vm.Password))
                //{
                //    ModelState.AddModelError("Password", "Password Should Contain Atleast 1 UpperCase, 1 number and 1 Special Character");
                //    return View(vm);
                //}
                AgentStaffRegistrationServices agentStaffRegistrationServices = new AgentStaffRegistrationServices();
                var isEmailUnique = agentStaffRegistrationServices.checkUniqueEmail(vm.EmailAddress);
                if (isEmailUnique == false)
                {
                    ModelState.AddModelError("EmailAddress", "This email address is already used");
                    return View(vm);
                }

                _services.Add(vm);

                return RedirectToAction("Index", "AgentStaff");
            }

            return View(vm);
        }


        public JsonResult GetAgentInfo(int AgentId)
        {


            var agentInfo = _commonServices.GetAgentInformation(AgentId);
            return Json(new
            {

                AccountNo = agentInfo.AccountNo,
                Address = agentInfo.City + " " + agentInfo.Address1 + " " + agentInfo.PostalCode
            }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult AddAgentStaffInfoAndLogin()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Agent = _commonServices.GetAgent();
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");

            AgentStaffInfoAndLoginViewModel vm = new AgentStaffInfoAndLoginViewModel();
            if (Common.AdminSession.AgentStaffInfoAndLoginViewModel != null)
            {

                vm = Common.AdminSession.AgentStaffInfoAndLoginViewModel;
                return View(vm);
            }

            return View();
        }
        [HttpPost]
        public ActionResult AddAgentStaffInfoAndLogin([Bind(Include = AgentStaffInfoAndLoginViewModel.BindProperty)]AgentStaffInfoAndLoginViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Countries = _commonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Agent = _commonServices.GetAgent(vm.Country);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName", vm.AgentId);
            if (ModelState.IsValid)
            {

                //var IsValidAccountno = _services.CheckIfAccountNoIsvalid(vm.AgenAccountNo);

                //if (!IsValidAccountno)
                //{
                //    ModelState.AddModelError("AgenAccountNo", "Account Number is Invlaid");
                //    return View(vm);
                //}

                //bool IsAgentStaffCreadted = _services.CheckIfAgentStaffIsCreated(vm.AgenAccountNo);
                //if (IsAgentStaffCreadted)
                //{

                //    ModelState.AddModelError("", "Agent Staff has been already created");
                //    return View(vm);
                //}

                Common.AdminSession.AgentStaffInfoAndLoginViewModel = vm;
                return RedirectToAction("AddAgentStaff");


            }
            return View(vm);
        }
        public ActionResult DeleteAgentStaff(int AgentStaffId = 0, string AccountNumber = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            _services.DeleteAgentStaff(AgentStaffId);
            return RedirectToAction("MoreDetails", "AdminRegisteredAgentStaff", new { @AccountNumber = AccountNumber });


        }
        public ActionResult MoreDetails(string AccountNumber = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            AdminRegisteredAgentStaffInfoViewModel vm = _services.GetAgentDetails(AccountNumber);
            return View(vm);
        }

        public ActionResult Update(string AccountNumber = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _commonServices.GetCountries();
            var identifyCardType = Common.Common.GetIdCardType();

            ViewBag.IdIssuingCountry = new SelectList(Countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(identifyCardType, "Name", "Name");

            AdminRegisteredAgentStaffInfoViewModel vm = _services.GetAgentDetails(AccountNumber);
            return View(vm);
        }
        [HttpPost]
        public ActionResult Update([Bind(Include = AdminRegisteredAgentStaffInfoViewModel.BindProperty)]AdminRegisteredAgentStaffInfoViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _commonServices.GetCountries();

            var identifyCardType = Common.Common.GetIdCardType();


            ViewBag.IdIssuingCountry = new SelectList(Countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(identifyCardType, "Name", "Name");

            if (ModelState.IsValid)
            {

                DateTime ExpiryDate = new DateTime(vm.Year, (int)vm.Month, vm.Day);

                //ExpiryDate.AddYears(vm.Year);
                //ExpiryDate.AddMonths((int)vm.Month);
                //ExpiryDate.AddDays(vm.Day);

                DateTime CurrentDate = DateTime.Now;
                if (ExpiryDate <= CurrentDate)
                {
                    ModelState.AddModelError("Day", "Id has Expired");
                    return View(vm);
                }
                vm.IdExpiryDate = ExpiryDate;
                var DOBYear = vm.DateOfBirth.Year;
                var currentYEar = CurrentDate.Year;
                int Age = currentYEar - DOBYear;
                if (Age <= 18)
                {
                    ModelState.AddModelError("DateOfBirth", "Agent should be aleast 18 years");
                    return View(vm);
                }
                var agentStaffInfo = _services.AgentStaffInfo(vm.AgentStaffId);
                if (agentStaffInfo.Agent.PhoneNumber != vm.PhoneNumber)
                {
                    vm.PhoneNumber = Common.Common.IgnoreZero(vm.PhoneNumber);
                    bool IsMobileNoExist = Common.OtherUtlities.IsMobileNoExist(vm.PhoneNumber);
                    if (IsMobileNoExist)
                    {
                        ModelState.AddModelError("MobileNo", "Mobile number already exists");
                        return View(vm);
                    }

                }
                if (agentStaffInfo.EmailAddress != vm.EmailAddress)
                {
                    bool isEmailExist = _services.CheckIfAgentStaffIsCreated(vm.EmailAddress);
                    if (isEmailExist)
                    {
                        ModelState.AddModelError("EmailAddress", "Email address already exists");
                        return View(vm);
                    }
                }
                _services.UpdateAgentStaffInfo(vm);
                return RedirectToAction("MoreDetails", "AdminRegisteredAgentStaff", new { @AccountNumber = vm.AccountNumber });
            }
            return View(vm);
        }



        public JsonResult GetAgentByCountry(string Country)
        {
            var data = _commonServices.GetAgents().Where(x => x.Country == Country).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
    }
}