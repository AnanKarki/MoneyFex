using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AgentCommissionSettingController : Controller
    {
        AgentCommissionSettingServices Service = new AgentCommissionSettingServices();
        CommonServices CommonService = new CommonServices();


        // GET: Admin/AgentCommissionSetting
        public ActionResult Index(string Country = "", string City = "", int Agent = 0, int? page = null, int TransferSevice = 0, int CommissionType = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = CommonService.GetCountries();
            var Cities = CommonService.GetCities();
            var Agents = CommonService.GetAgents();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            ViewBag.Cities = new SelectList(Cities, "City", "City");
            ViewBag.Agents = new SelectList(Agents, "AgentId", "AgentName");
            int pageSize = 10;
            int pageNumber = (page ?? 1);


            IPagedList<AgentCommissionSettingViewModel> vm = Service.getAgentCommissionList(Country, City, Agent).ToPagedList(pageNumber, pageSize);

            if (TransferSevice > 0)
            {
                vm = vm.Where(x => x.TransferSevice == (TransferService)TransferSevice).ToPagedList(pageNumber, pageSize);

            }
            if (CommissionType > 0)
            {
                vm = vm.Where(x => x.CommissionType == (CommissionType)CommissionType).ToPagedList(pageNumber, pageSize);
            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddAgentCommission(string Country = "", string City = "", int AgentId = 0, int TransferSevice = 0, int CommissionType = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetViewBagForCountries();

            ViewBag.Cities = new SelectList(CommonService.GetCities().ToList(), "City", "City");
            ViewBag.Agents = new SelectList(CommonService.GetAgents().ToList(), "AgentId", "AgentName");
            if (!string.IsNullOrEmpty(Country))
            {
                ViewBag.Cities = new SelectList(CommonService.GetCities(Country), "City", "City");
            }
            if (!string.IsNullOrEmpty(City))
            {
                ViewBag.Agents = new SelectList(CommonService.GetAgents(City).ToList(), "AgentId", "AgentName", AgentId);
            }
            if (AgentId != 0 && TransferSevice != 0 && CommissionType != 0)
            {
                AgentCommissionSettingViewModel model = new AgentCommissionSettingViewModel();
                var vm = Service.getEditInfo(AgentId, TransferSevice, CommissionType);
                if (vm != null)
                {
                    vm.AccountNo = CommonService.GetAgentAccNo(AgentId);
                    return View(vm);

                }
                model.AccountNo = CommonService.GetAgentAccNo(AgentId);
                return View(model);
            }
            if (TransferSevice != 0 && CommissionType != 0)
            {
                AgentCommissionSettingViewModel vm = new AgentCommissionSettingViewModel();

                vm.Country = Country;
                vm.City = City;
                vm.AgentId = AgentId;
                vm.TransferSevice = (TransferService)TransferSevice;
                vm.CommissionType = (CommissionType)CommissionType;

                vm.AccountNo = CommonService.GetAgentAccNo(AgentId);
                Service.getAgentInfo(TransferSevice, CommissionType, ref vm);
                return View(vm);
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddAgentCommission([Bind(Include = AgentCommissionSettingViewModel.BindProperty)]AgentCommissionSettingViewModel model)
        {
            ViewBag.Cities = new SelectList(CommonService.GetCities().ToList(), "City", "City");
            ViewBag.Agents = new SelectList(CommonService.GetAgents().ToList(), "AgentId", "AgentName");

            if (ModelState.IsValid)
            {

                bool ressult = Service.SaveAgentCommission(model);
                Service.SaveAgentCommissionHistory(model);
                if (ressult)
                {
                    return RedirectToAction("Index");
                }

            }
            SetViewBagForCountries();
            return View(model);

        }

        public ActionResult UpdateAgentCommission(int Id = 0, string Country = "", string City = "", int AgentId = 0, int TransferSevice = 0, int CommissionType = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetViewBagForCountries();
            ViewBag.Cities = new SelectList(CommonService.GetCities().ToList(), "City", "City");
            ViewBag.Agents = new SelectList(CommonService.GetAgents().ToList(), "AgentId", "AgentName");
            if (!string.IsNullOrEmpty(Country))
            {
                ViewBag.Cities = new SelectList(CommonService.GetCities(Country), "City", "City");
            }
            if (!string.IsNullOrEmpty(City))
            {
                ViewBag.Agents = new SelectList(CommonService.GetAgents(City).ToList(), "AgentId", "AgentName");
            }
            if (AgentId != 0 && TransferSevice != 0 && CommissionType != 0)
            {
                AgentCommissionSettingViewModel VM = new AgentCommissionSettingViewModel();
                var vm = Service.getEditInfo(AgentId, TransferSevice, CommissionType);
                if (vm != null)
                {
                    vm.AccountNo = CommonService.GetAgentAccNo(AgentId);
                    return View(vm);

                }
                VM.AccountNo = CommonService.GetAgentAccNo(AgentId);
                return View(VM);
            }
            if (TransferSevice != 0 && CommissionType != 0)
            {

                AgentCommissionSettingViewModel vm = new AgentCommissionSettingViewModel();

                vm.Country = Country;
                vm.City = City;
                vm.AgentId = AgentId;
                vm.TransferSevice = (TransferService)TransferSevice;
                vm.CommissionType = (CommissionType)CommissionType;

                vm.AccountNo = CommonService.GetAgentAccNo(AgentId);
                Service.getAgentInfo(TransferSevice, CommissionType, ref vm);
                return View(vm);
            }
            AgentCommissionSettingViewModel model = Service.GetAgentCommissionDataById(Id);
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateAgentCommission([Bind(Include = AgentCommissionSettingViewModel.BindProperty)]AgentCommissionSettingViewModel model)
        {
            if (ModelState.IsValid)
            {
                bool ressult = Service.SaveAgentCommission(model);
                Service.SaveAgentCommissionHistory(model);
                if (ressult)
                {
                    return RedirectToAction("Index");
                }
            }
            SetViewBagForCountries();
            return View(model);
        }



        [HttpGet]
        public JsonResult DeleteAgentCommission(int id)
        {
            if (id > 0)
            {

                bool result = Service.DeleteAgentCommission(id);
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
        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }


        public JsonResult GetAccountNo(int agentId)
        {
            var data = CommonService.GetAgentAccNo(agentId);
            decimal SendingCommissionRate = 0;
            decimal ReceivingCommissionRate = 0;
            var rate = Service.getAgentCommissionList().Where(x => x.AgentId == agentId).FirstOrDefault();
            if (rate != null)
            {
                SendingCommissionRate = (decimal)rate.SendingCommission;
                ReceivingCommissionRate = (decimal)rate.ReceivingCommission;
            }
            if (data != null)
            {
                return Json(new
                {
                    accountNo = data,
                    SendingCommissionRate = SendingCommissionRate,
                    ReceivingCommissionRate = ReceivingCommissionRate

                }, JsonRequestBehavior.AllowGet);
            }
            return null;
        }


        public ActionResult AgentCommissionHistoryOnTransferFee()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var vm = Service.getAgentCommissionHistoryOnTransferFeeList();

            return View(vm);


        }

        public ActionResult AgentCommissionHistoryOnAmount()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var vm = Service.getAgentCommissionHistoryOnAmount();

            return View(vm);

        }

        public ActionResult AgentCommissionHistoryOnFlatFee()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var vm = Service.getAgentCommissionHistoryOnFlatFee();

            return View(vm);

        }
        public ActionResult AgentCommissionHistory(string Country = "", string City = "", int TransferService = 0, int Agent = 0, int CommissionType = 0, string YearMonth = "", int? page = null)
        {
            var Countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name", Country);

            var Cities = CommonService.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City", City);

            var Agents = CommonService.GetAgent();
            ViewBag.Agents = new SelectList(Agents, "AgentId", "AgentName", Agent);

            ViewBag.DateRange = YearMonth;
            ViewBag.TransferService = TransferService;
            ViewBag.CommissionType = CommissionType;

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<AgentCommissionHisotryViewModel> vm = Service.getAgentCommissionHistory(Country, City, TransferService, Agent, CommissionType, YearMonth).ToPagedList(pageNumber, pageSize);
            return View(vm);

        }
    }
}