using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Services;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AUXAgentFundAccountController : Controller
    {
        FundAccountServices _services = null;
        CommonServices _CommonServices = null;
        public AUXAgentFundAccountController()
        {
            _services = new FundAccountServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/AUXAgentFundAccount
        public ActionResult Index(string DateRange = "", string Country = "", int Agent = 0, string AuXAgentCode = "",
            string Reference = "", string ResponsiblePerson = "", string Status = "", string Details = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Country = new SelectList(Countries, "Code", "Name", Country);

            var Agents = _CommonServices.GetAgent();
            ViewBag.Agent = new SelectList(Agents, "AgentId", "AgentName", Agent);
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.AuXAgentCode = AuXAgentCode;
            ViewBag.Reference = Reference;
            ViewBag.ResponsiblePerson = ResponsiblePerson;
            ViewBag.Status = Status;
            ViewBag.Details = Details;
            IPagedList<AgentFundAccountViewModel> vm = _services.GetAgentFundAccountList(Agent, DateRange, Country, AuXAgentCode,
                Reference, ResponsiblePerson, Status, Details).ToPagedList(pageNumber, pageSize);
            ViewBag.DateRange = DateRange;
            return View(vm);
        }
        public ActionResult SetFundAccount(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Country = new SelectList(Countries, "Code", "Name");

            var City = _CommonServices.GetCitiesByName();
            ViewBag.City = new SelectList(City, "City", "City");



            AgentFundAccountViewModel vm = new AgentFundAccountViewModel();

            if (id != 0)
            {
                vm = _services.GetFundAccountDetails(id);
                ViewBag.selectedCity = vm.City;
            }
            var Agents = _CommonServices.GetAgent(vm.AgentCountry, true);
            ViewBag.Agent = new SelectList(Agents, "AgentId", "AgentName");
            return View(vm);
        }
        [HttpPost]
        public ActionResult SetFundAccount([Bind(Include = AgentFundAccountViewModel.BindProperty)] AgentFundAccountViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Country = new SelectList(Countries, "Code", "Name");

            var City = _CommonServices.GetCitiesByName();
            ViewBag.City = new SelectList(City, "City", "City");

            var Agents = _CommonServices.GetAgent();
            ViewBag.Agent = new SelectList(Agents, "AgentId", "AgentName");

            if (ModelState.IsValid)
            {

                var ReceiptNo = Common.Common.GenerateFundAccountReceiptNo(6);
                int StaffId = Common.StaffSession.LoggedStaff.StaffId;
                vm.ApprovedBy = StaffId;
                vm.BankAccountNo = _services.GetMFBankDetails(vm.AgentCountry).AccountNo;
                vm.BankSortCode = _services.GetMFBankDetails(vm.AgentCountry).LabelValue;
                vm.PaymentReference = Common.Common.GenerateFundAccountPaymentRefrence();
                vm.Receipt = ReceiptNo;
                if (vm.Id > 0)
                {
                    _services.updateFundAccount(vm);
                }
                else
                {
                    _services.AddFundInAccount(vm);
                }
                return RedirectToAction("Index", "AUXAgentFundAccount");
            }
            return View(vm);
        }
        public ActionResult Approved(int Id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            _services.Approved(Id, StaffId);
            return RedirectToAction("Index", "AUXAgentFundAccount");
        }
        public ActionResult FundDetails(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            AgentFundAccountViewModel vm = _services.GetAgentFundAccountList().Where(x => x.Id == id).FirstOrDefault();
            return View(vm);
        }

        public JsonResult GetCitiesByCountry(string Country)
        {
            var Cities = _CommonServices.GetCitiesByName(Country);
            return Json(new
            {
                Cities = Cities

            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAgentByCountry(string City)
        {
            //    string Cityname = _CommonServices.GetCityName(City);
            var Agents = _CommonServices.GetAgents(City);
            return Json(new
            {
                Agents = Agents,

            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAgentAccount(int AgentId)
        {
            var AgentAccountNo = _CommonServices.GetAgentAccNo(AgentId);
            return Json(new
            {
                AgentAccountNo = AgentAccountNo,

            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CheckPaymentStatus(string refno)
        {
            PGTransactionResultVm result = _services.CheckPaymentStatus(refno);
            return Json(new
            {
                Data = result
            }, JsonRequestBehavior.AllowGet);
        }



    }
}