using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class EnableManualBankDepositController : Controller
    {
        CommonServices _CommonServices = null;
        EnableManualBankDepositServices _enableManualDeposit = null;
        public EnableManualBankDepositController()
        {
            _CommonServices = new CommonServices();
            _enableManualDeposit = new EnableManualBankDepositServices();
        }

        // GET: Admin/EnableManualBankDeposit
        public ActionResult Index(string PayingCountry = "", int AgentId = 0, string AccountNo = "", string MobileNo = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = _CommonServices.GetCountries();
            var agents = _CommonServices.GetAgent();
            ViewBag.PayingCountries = new SelectList(countries, "Code", "Name", PayingCountry);
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName", AgentId);
            ViewBag.AccountNo = AccountNo;
            ViewBag.MobileNo = MobileNo;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<EnableManualBankDepositViewModel> model = _enableManualDeposit.List(PayingCountry, AgentId).ToPagedList(pageNumber, pageSize);

            if (!string.IsNullOrEmpty(AccountNo))
            {
                AccountNo = AccountNo.Trim();
                model = model.Where(x => x.AgentAccountNo.ToLower().Contains(AccountNo.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(MobileNo))
            {
                MobileNo = MobileNo.Trim();
                model = model.Where(x => x.MobileNo.ToLower().Contains(MobileNo.ToLower())).ToPagedList(pageNumber, pageSize);
            }
            return View(model);
        }
        public ActionResult EnableManualDeposit(int id = 0, int AgentId = 0, string CountryCode = "")
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = _CommonServices.GetCountries();
            var agents = _CommonServices.GetAgent(CountryCode);

            ViewBag.PayingCountries = new SelectList(countries, "Code", "Name", CountryCode);
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            EnableManualBankDepositViewModel vm = new EnableManualBankDepositViewModel();

            vm.MobileCode = Common.Common.GetCountryPhoneCode(CountryCode);
            if (AgentId != 0)
            {
                var agentInfo = _enableManualDeposit.GetAgentInfo(AgentId);
                vm.AgentAccountNo = agentInfo.AccountNo;
                string agentCountry = Common.Common.GetCountryName(agentInfo.CountryCode);
                vm.AgentAddress = agentInfo.Address1 + " " + agentInfo.Address2 + " " + agentInfo.City + " " + agentCountry;
                ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName", AgentId);

            }
            if (id != 0)
            {
                vm = _enableManualDeposit.GetManualBankDepositData(id);
            }
            return View(vm);
        }
        [HttpPost]
        public ActionResult EnableManualDeposit([Bind(Include = EnableManualBankDepositViewModel.BindProperty)]EnableManualBankDepositViewModel vm)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = _CommonServices.GetCountries();
            var agents = _CommonServices.GetAgent(vm.PayingCountry);

            ViewBag.PayingCountries = new SelectList(countries, "Code", "Name");
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            vm.MobileCode = Common.Common.GetCountryPhoneCode(vm.PayingCountry);
            if (ModelState.IsValid)
            {
                int staffId = Common.StaffSession.LoggedStaff.StaffId;
                vm.CreatedById = staffId;

                if (vm.Id == null || vm.Id == 0)
                {
                    _enableManualDeposit.Add(vm);
                    return RedirectToAction("Index", "EnableManualBankDeposit");
                }
                else
                {
                    _enableManualDeposit.Update(vm);
                    return RedirectToAction("Index", "EnableManualBankDeposit");

                }

            }
            return View(vm);
        }


        [HttpGet]
        public JsonResult Delete(int id)
        {
            if (id > 0)
            {
                _enableManualDeposit.Delete(id);
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
        [HttpGet]
        public JsonResult GetAgentDetails(int AgentId = 0)
        {
            if (AgentId > 0)
            {
                var agentInfo = _enableManualDeposit.GetAgentInfo(AgentId);
                var agentAccountNo = agentInfo.AccountNo;
                string agentCountry = Common.Common.GetCountryName(agentInfo.CountryCode);
                var agentAddress = agentInfo.Address1 + " " + agentInfo.Address2 + " " + agentInfo.City + " " + agentCountry;
                return Json(new
                {
                    agentAccountNo = agentAccountNo,
                    agentAddress = agentAddress,
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    agentAccountNo = "",
                    agentAddress = "",
             
                }, JsonRequestBehavior.AllowGet);
            }
        }


    }
}