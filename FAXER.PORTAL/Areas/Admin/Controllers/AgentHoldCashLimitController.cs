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
    public class AgentHoldCashLimitController : Controller
    {
        // GET: Admin/AgentHoldCashLimit
        CommonServices _CommonServices = null;
        AgentHoldCashLimitServices _agentHoldCashLimitServices = null;
        public AgentHoldCashLimitController()
        {
            _CommonServices = new CommonServices();
            _agentHoldCashLimitServices = new AgentHoldCashLimitServices();
        }
        public ActionResult Index(string Country = "", string city = "", int AgentId = 0, string AccountNo = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var City = _CommonServices.GetCities();
            ViewBag.Cities = new SelectList(City, "City", "City");
            var Agent = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(Agent, "AgentId", "AgentName");
            ViewBag.AccountNo = AccountNo;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            IPagedList<AgentTransferLimtViewModel> vm = _agentHoldCashLimitServices.GetAgentHoldCashimit(Country, city, AgentId, AccountNo).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        [HttpPost]
        public ActionResult SetHoldCashLimit(AgentTransferLimtViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var City = _CommonServices.GetCities();
            ViewBag.Cities = new SelectList(City, "City", "City");
            var Agent = _CommonServices.GetAgent(vm.Country);
            ViewBag.Agents = new SelectList(Agent, "AgentId", "AgentName");
            if (ModelState.IsValid)
            {


                if (vm.Amount == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Amount");
                    return View(vm);
                }


                if (vm.Id > 0)
                {
                    _agentHoldCashLimitServices.UpdateAgentHoldCashLimit(vm);

                }
                else
                {

                    _agentHoldCashLimitServices.AddAgentHoldCashLimit(vm);

                }
                return RedirectToAction("Index");

            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult SetHoldCashLimit(int Id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var City = _CommonServices.GetCities();
            ViewBag.Cities = new SelectList(City, "City", "City");
            var Agent = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(Agent, "AgentId", "AgentName");


            var vm = GetHoldCashLimit(Id);

            return View(vm);
        }

        public ViewModels.AgentTransferLimtViewModel GetHoldCashLimit(int Id)
        {

            if (Id > 0)
            {
                var rate = (from c in _agentHoldCashLimitServices.List().Where(x => x.Id == Id).ToList()
                            select new ViewModels.AgentTransferLimtViewModel()
                            {

                                Id = c.Id,
                                AgentId = c.AgentId,
                                CreatedBy = c.CreatedBy,
                                CreatedDate = c.CreatedDate,
                                Amount = c.Amount,
                                AccountNo = c.AccountNo,
                                City = c.City,
                                Country = c.Country,
                                TransferMethod = c.TransferMethod,
                                LimitType = c.LimitType,
                                Frequency = c.Frequency
                            }).FirstOrDefault();
                return rate;
            }
            else
            {
                return null;

            }


        }



        [HttpGet]
        public JsonResult DeleteHoldCashLimit(int Id)
        {
            if (Id > 0)
            {
                _agentHoldCashLimitServices.Delete(Id);
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
        public ActionResult HoldCashLimitHistory(string Country = "", string city = "", int AgentId = 0, string AccountNo = "", int? page = null)
        {
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var City = _CommonServices.GetCities();
            ViewBag.Cities = new SelectList(City, "City", "City");
            var Agent = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(Agent, "AgentId", "AgentName");
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewBag.AccountNo = AccountNo;
            IPagedList<AgentTransferLimtViewModel> vm = _agentHoldCashLimitServices.GetAgentHoldCashLimitHistory(Country, city, AgentId, AccountNo).ToPagedList(pageNumber, pageSize);
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