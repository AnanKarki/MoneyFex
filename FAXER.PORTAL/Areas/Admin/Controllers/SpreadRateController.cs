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
    public class SpreadRateController : Controller
    {
        Services.CommonServices CommonService = null;
        SSpreadRateServices _spreadRate = null;
        public SpreadRateController()
        {
            CommonService = new Services.CommonServices();
            _spreadRate = new SSpreadRateServices();
        }
        // GET: Admin/SpreadRate
        public ActionResult Index(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int Agent = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var sendingcountries = _spreadRate.GetCountries(SendingCountry);
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _spreadRate.GetCountries(ReceivingCountry);
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");

            var agents = _spreadRate.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            ViewBag.TransferType = TransferType;

            int StaffId = Common.StaffSession.LoggedStaff.StaffId;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<SpreadRateViewModel> vm = _spreadRate.GetSpreadRate(StaffId, SendingCountry, ReceivingCountry, TransferType, Agent).Data.ToPagedList(pageNumber, pageSize);


            return View(vm);
        }


        public ActionResult SetSpreadRate()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _spreadRate.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _spreadRate.GetCountries();
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");

            var Agent = CommonService.GetAgent();
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");

            SpreadRateViewModel vm = new SpreadRateViewModel();

            return View(vm);

        }
        [HttpPost]
        public ActionResult SetSpreadRate([Bind(Include = SpreadRateViewModel.BindProperty)]SpreadRateViewModel model)
        {
            var sendingcountries = _spreadRate.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _spreadRate.GetCountries();
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");

            var Agent = CommonService.GetAgent(model.SendingCountry);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");

            if (ModelState.IsValid)
            {
                if (model.Rate == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Spread Rate");
                    return View(model);
                }
                if (model.TransferType == 0)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Type");
                    return View(model);

                }
                if (model.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", "Select Transfer Method");
                    return View(model);

                }
                _spreadRate.AddSpreadRate(model);
                _spreadRate.AddSpreadRateHistory(model);
                return RedirectToAction("Index", "SpreadRate");
            }
            return View(model);
        }

        public ActionResult UpdateSpreadRate(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            SpreadRateViewModel vm = _spreadRate.GetSpreadRateById(id);


            var Agent = CommonService.GetAgent(vm.SendingCountry);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");

            return View(vm);
        }

        [HttpPost]
        public ActionResult UpdateSpreadRate([Bind(Include = SpreadRateViewModel.BindProperty)]SpreadRateViewModel model)
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var Agent = CommonService.GetAgent(model.SendingCountry);
            ViewBag.Agent = new SelectList(Agent, "AgentId", "AgentName");
            if (ModelState.IsValid)
            {
                if (model.TransferType == 0)
                {
                    ModelState.AddModelError("TransferType", "Select Transfer Type");
                    return View(model);

                }
                if (model.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransferMethod", "Select Transfer Method");
                    return View(model);

                }

                if (model.Rate == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Spread Rate");
                    return View(model);
                }

                _spreadRate.UpdateSpreadRate(model);
                _spreadRate.AddSpreadRateHistory(model);
                return RedirectToAction("Index", "SpreadRate");
            }
            return View(model);
        }

        [HttpGet]
        public JsonResult DeleteSpreadRate(int Id)
        {
            if (Id > 0)
            {
                var data = _spreadRate.List().Data.Where(x => x.Id == Id).FirstOrDefault();
                var result = _spreadRate.Remove(data); ;
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

        public ActionResult SpreadRateHistory(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int Agent = 0, int Year = 0, int Month = 0, int Day = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var sendingcountries = _spreadRate.GetCountries(SendingCountry);
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var receivingCountries = _spreadRate.GetCountries(ReceivingCountry);
            ViewBag.ReceivingCountries = new SelectList(receivingCountries, "Code", "Name");

            var agents = _spreadRate.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");

            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.TransferType = TransferType;
            ViewBag.Month = Month;
            ViewBag.Day = Day;

            List<SpreadRateHistoryViewModel> vm = _spreadRate.GetSpreadRateHistory(SendingCountry, ReceivingCountry, TransferType, Agent, Year, Month, Day).Data;

            return View(vm);
        }

        public JsonResult GetAgentByCountry(string Country)
        {

            CommonServices _CommonServices = new CommonServices();
            var data = _CommonServices.GetAgents().Where(x => x.Country == Country).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSpreadRate(string sendingCountry = "", string receivingCounrty = "", int transferType = 0, int method = 0, int agent = 0)
        {
            var spreadRate = _spreadRate.GetRate(sendingCountry, receivingCounrty, transferType, method, agent);
            return Json(new
            {
                Rate = spreadRate
            }, JsonRequestBehavior.AllowGet);
        }

    }
}