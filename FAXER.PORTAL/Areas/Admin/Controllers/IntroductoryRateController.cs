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
    public class IntroductoryRateController : Controller
    {
        IntroductoryRateServices _introductoryRateServices = null;
        CommonServices _CommonServices = null;
        public IntroductoryRateController()
        {
            _introductoryRateServices = new IntroductoryRateServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/IntroductoryRate
        public ActionResult Index(string SendingCountry = "", string ReceivingCountry = "", int TransferType = 0, int Agent = 0, int TransferMethod = 0, int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(countries, "Code", "Name");

            var agents = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            ViewBag.TransferType = TransferType;

            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<ViewModels.IntroductoryRateListVm> vm = _introductoryRateServices.GetIntroductoryRate(SendingCountry, ReceivingCountry, TransferType, Agent, TransferMethod).ToPagedList(pageNumber, pageSize);

            return View(vm);
        }
        [HttpGet]
        public ActionResult SetRate(int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }


            var countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(countries, "Code", "Name");
            var agents = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            var vm = GetRate(Id);

            return View(vm);
        }

        public ViewModels.IntroductoryRateVm GetRate(int Id)
        {

            if (Id > 0)
            {
                var rate = (from c in _introductoryRateServices.GetRates().Where(x => x.Id == Id).ToList()
                            select new ViewModels.IntroductoryRateVm()
                            {

                                Id = c.Id,
                                AgentId = c.AgentId,
                                CreatedBy = c.CreatedBy,
                                CreatedDate = c.CreatedDate,
                                FromRange = c.FromRange,
                                NoOfTransaction = c.NoOfTransaction,
                                Range = GetRange(c.FromRange, c.ToRange),
                                Rate = c.Rate,
                                SendingCountry = c.SendingCountry,
                                ReceivingCountry = c.ReceivingCountry,
                                ToRange = c.ToRange,
                                TransactionTransferMethod = c.TransactionTransferMethod,
                                TransactionTransferType = c.TransactionTransferType
                            }).FirstOrDefault();
                return rate;
            }
            return new ViewModels.IntroductoryRateVm()
            {
                Range = "0"
            };


        }

        private string GetRange(decimal fromRange, decimal toRange)
        {

            var from = (int)fromRange;
            var to = (int)toRange;
            var range = from + "-" + to;

            return range == null ? "0" : range;
        }

        [HttpPost]
        public ActionResult SetRate([Bind(Include = ViewModels.IntroductoryRateVm.BindProperty)]ViewModels.IntroductoryRateVm vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(countries, "Code", "Name");
            var agents = _CommonServices.GetAgent(vm.SendingCountry);
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            if (ModelState.IsValid)
            {
                if (vm.TransactionTransferType == 0)
                {
                    ModelState.AddModelError("TransactionTransferType", "Select Transfer Type");
                    return View(vm);

                }
                if (vm.TransactionTransferMethod == 0)
                {
                    ModelState.AddModelError("TransactionTransferMethod", "Select Transfer Method");
                    return View(vm);

                }
                if (vm.Rate == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Introductory Rate");
                    return View(vm);
                }
                if (string.IsNullOrEmpty(vm.Range) == true)
                {
                    ModelState.AddModelError("", "Select Range");
                    return View(vm);

                }
                if (vm.Id > 0)
                {
                    _introductoryRateServices.UpdateIntroductoryRate(vm);

                }
                else
                {
                    _introductoryRateServices.AddIntroductoryRate(vm);

                }
                return RedirectToAction("Index");

            }
            return View(vm);

        }
        [HttpGet]
        public JsonResult DeleteRate(int Id)
        {
            if (Id > 0)
            {
                _introductoryRateServices.Delete(Id);
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
        public JsonResult GetPreviousRate(int AgentId)
        {

            var data = _introductoryRateServices.GetRates().Where(x => x.AgentId == AgentId).FirstOrDefault();
            if (data != null)
            {
                return Json(new
                {
                    Id = data.Id,
                    Range = data.FromRange + "-" + data.ToRange,
                    Rate = data.Rate,
                    NoOfTransaction = data.NoOfTransaction
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {

                Id = 0,
                Range = "",
                Rate = 0,
                NoOfTransaction = ""
            }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult RateHistory(string sendingCountry = "", string receivingCountry = "", int transferType = 0, int agentId = 0, int TransferMethod = 0, int? page = null)
        {

            var countries = _CommonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(countries, "Code", "Name");
            ViewBag.ReceivingCountries = new SelectList(countries, "Code", "Name");

            var agents = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            ViewBag.TransferType = transferType;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<IntroductoryRateListVm> vm = _introductoryRateServices.GetIntroductoryRateHistory(sendingCountry, receivingCountry, transferType, agentId, TransferMethod).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        public JsonResult GetAgentByCountry(string Country)
        {

            var data = _CommonServices.GetAgents().Where(x => x.Country == Country).ToList();
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }
    }
}