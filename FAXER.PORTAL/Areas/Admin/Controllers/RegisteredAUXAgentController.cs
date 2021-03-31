using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class RegisteredAUXAgentController : Controller
    {
        // GET: Admin/RegisteredAUXAgent
        AUXAgentRecentTransactionServices _auxAgentRecentTransactionServices = null;
        SAgentInformation _sAgentInformation = null;
        CommonServices _commonServices = null;
        public RegisteredAUXAgentController()
        {
            _auxAgentRecentTransactionServices = new AUXAgentRecentTransactionServices();
            _commonServices = new CommonServices();
            _sAgentInformation = new SAgentInformation();

        }
        public ActionResult Index(string SendingCountry = "", string City = "", string Date = "",
            string AgentName = "", string AccountNo = "", string LoginCode = "",
            string Telephone = "", string Email = "",
            int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            ViewBag.AgentName = AgentName;
            ViewBag.LoginCode = LoginCode;
            ViewBag.AccountNo = AccountNo;
            ViewBag.Telephone = Telephone;
            ViewBag.Email = Email;
            var sendingcountries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var Cities = _commonServices.GetCities();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            #region Old
            //List<AgentInformation> AgentList = new List<AgentInformation>();
            //AgentList = _auxAgentRecentTransactionServices.GetAuxAgentInformation().Where(x => x.IsDeleted == false).ToList();
            //var AgentLoginList = _auxAgentRecentTransactionServices.GetAgentLogins();
            //IPagedList<RegisteredAUXAgentViewModel> vm = (from c in AgentList
            //                                              join d in AgentLoginList on c.Id equals d.AgentId
            //                                              select new RegisteredAUXAgentViewModel()
            //                                              {
            //                                                  Id = c.Id,
            //                                                  AccountNo = c.AccountNo,
            //                                                  Address = c.Address1,
            //                                                  AgentName = c.Name,
            //                                                  BusinessType = c.TypeOfBusiness == null ? "" : Common.Common.GetEnumDescription(c.TypeOfBusiness),
            //                                                  Country = _commonServices.getCountryNameFromCode(c.CountryCode),
            //                                                  CountryCode = c.CountryCode,
            //                                                  City = c.City,
            //                                                  Email = c.Email,
            //                                                  LoginCode = d.LoginCode,
            //                                                  Telephone = c.PhoneNumber,
            //                                                  StatusName = Enum.GetName(typeof(AgentStatus), c.AgentStatus),
            //                                                  Date = c.CreatedDate.ToFormatedString(),
            //                                                  CreationDate = c.CreatedDate

            //                                              }).OrderByDescending(x => x.CreationDate).ToPagedList(pageNumber, pageSize);

            //if (!string.IsNullOrEmpty(SendingCountry))
            //{
            //    vm = vm.Where(x => x.CountryCode == SendingCountry).ToPagedList(pageNumber, pageSize);
            //}
            //if (!string.IsNullOrEmpty(City))
            //{
            //    vm = vm.Where(x => x.City == City).ToPagedList(pageNumber, pageSize);
            //}
            //if (!string.IsNullOrEmpty(AgentName))
            //{
            //    AgentName = AgentName.Trim();
            //    vm = vm.Where(x => x.AgentName.ToLower().Contains(AgentName.ToLower())).ToPagedList(pageNumber, pageSize);

            //}
            //if (!string.IsNullOrEmpty(AccountNo))
            //{
            //    AccountNo = AccountNo.Trim();
            //    vm = vm.Where(x => x.AccountNo.ToLower().Contains(AccountNo.ToLower())).ToPagedList(pageNumber, pageSize);

            //}
            //if (!string.IsNullOrEmpty(LoginCode))
            //{
            //    LoginCode = LoginCode.Trim();
            //    vm = vm.Where(x => x.LoginCode.ToLower().Contains(LoginCode.ToLower())).ToPagedList(pageNumber, pageSize);

            //}
            //if (!string.IsNullOrEmpty(Telephone))
            //{
            //    Telephone = Telephone.Trim();
            //    vm = vm.Where(x => x.Telephone.ToLower().Contains(Telephone.ToLower())).ToPagedList(pageNumber, pageSize);

            //}
            //if (!string.IsNullOrEmpty(Email))
            //{
            //    Email = Email.Trim();
            //    vm = vm.Where(x => x.Email.ToLower().Contains(Email.ToLower())).ToPagedList(pageNumber, pageSize);

            //}
            //if (!string.IsNullOrEmpty(Date))
            //{
            //    string[] DateString = Date.Split('-');
            //    DateTime FromDate = Convert.ToDateTime(DateString[0]);
            //    DateTime ToDate = Convert.ToDateTime(DateString[1]);
            //    vm = vm.Where(x => x.CreationDate >= FromDate && x.CreationDate <= ToDate).ToPagedList(pageNumber, pageSize);
            //}
            #endregion

            IPagedList<RegisteredAUXAgentViewModel> vm = _auxAgentRecentTransactionServices.GetRegisteredAuxAgents(SendingCountry, City,
                Date, AgentName, AccountNo, LoginCode, Telephone, Email).ToPagedList(pageNumber, pageSize);
            return View(vm);

        }

        [HttpGet]
        public JsonResult DeleteAUXAgent(int id)
        {
            if (id > 0)
            {
                var Agent = _sAgentInformation.list().Data.Where(x => x.Id == id).FirstOrDefault();
                Agent.IsDeleted = true;
                _sAgentInformation.Update(Agent);
                return Json(new
                {
                    Data = true,
                    Message = "Deleted Sucessfully"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Data = false,
                    Message = "Something went wrong. Please try again!"
                }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult UpdateAUXAgent(int id)
        {
            RegisteredAUXAgentViewModel vm = _auxAgentRecentTransactionServices.GetRegisteredAUXAgentViewModel(id);
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);

            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateAUXAgent(RegisteredAUXAgentViewModel vm)
        {
            var Agent = _sAgentInformation.list().Data.Where(x => x.Id == vm.Id).FirstOrDefault();
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode);

            Agent.AgentStatus = vm.AgentStatus;
            var agentLogin = _sAgentInformation.AgentLoginList().Data.Where(x => x.AgentId == vm.Id).FirstOrDefault();
            var agentStaffInfo = _sAgentInformation.AgentStaffInfoList().Data.Where(x => x.AgentId == vm.Id).FirstOrDefault();
            var agentStaffLogin = _sAgentInformation.AgentStaffLoginList().Data.Where(x => x.AgentStaffId == agentStaffInfo.Id).FirstOrDefault();

            if (ModelState.IsValid)
            {
                vm.Email = vm.Email.Trim();
                vm.Telephone = vm.Telephone.Trim();
                if (Agent.Email != vm.Email)
                {
                    bool isEmailExist = Common.OtherUtlities.IsEmailExistInAgent(vm.Email);
                    if (isEmailExist == false)
                    {
                        ModelState.AddModelError("Email", "Email Already Exist");
                        return View(vm);
                    }
                }

                if (Agent.PhoneNumber != vm.Telephone)
                {
                    bool IsMobileNoExist = OtherUtlities.IsMobileNoExistInAgent(vm.Telephone);
                    if (IsMobileNoExist == false)
                    {
                        ModelState.AddModelError("Telephone", "Phone number Already Exist");
                        return View(vm);
                    }
                }
                var countryPhoneCode = Common.Common.GetCountryPhoneCode(vm.CountryCode).Split('+')[1];
                if (vm.Telephone.StartsWith(countryPhoneCode))
                {
                    vm.Telephone = vm.Telephone.Trim();
                    vm.Telephone = vm.Telephone.Substring(countryPhoneCode.Length, vm.Telephone.Length - countryPhoneCode.Length);
                }
                SmsApi smsApi = new SmsApi();
                bool validMobile = smsApi.IsValidMobileNo(Common.Common.GetCountryPhoneCode(vm.CountryCode) + vm.Telephone);
                if (!validMobile)
                {
                    ModelState.AddModelError("Telephone", "Invalid mobile number ");
                    return View(vm);
                }

                if (vm.TypeOfBusiness == BusinessType.Non)
                {
                    ModelState.AddModelError("TypeOfBusiness", "Select Business Type");
                    return View(vm);
                }
                Agent.PhoneNumber = vm.Telephone;
                Agent.Email = vm.Email;
                Agent.Name = vm.AgentName;
                Agent.TypeOfBusiness = vm.TypeOfBusiness;
                if (vm.AgentStatus == AgentStatus.Active)
                {
                    agentLogin.IsActive = true;
                    agentStaffLogin.IsActive = true;
                }
                else if (vm.AgentStatus == AgentStatus.Inactive)
                {
                    agentLogin.IsActive = false;
                    agentStaffLogin.IsActive = false;
                }
                _sAgentInformation.UpdateAgentStaffLogin(agentStaffLogin);
                _sAgentInformation.UpdateAgentLogin(agentLogin);
                _sAgentInformation.Update(Agent);
                return RedirectToAction("Index");
            }
            return View(vm);
        }

        public ActionResult AuxAgentContactPersonDetails(int agentId)
        {
            var vm = _auxAgentRecentTransactionServices.GetAuxAgentContactDetails(agentId);
            return View(vm);
        }
    }
}