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
    public class AgentTransferLimitController : Controller
    {
        CommonServices _CommonServices = null;
        AgentTransferLimitServices _agentTransferLimitServices = null;
        public  AgentTransferLimitController()
        {
            _CommonServices = new CommonServices();
            _agentTransferLimitServices = new AgentTransferLimitServices();
        }
        // GET: Admin/AgentTransferLimit
        public ActionResult Index(string Country = "", int Services = 0, string city= "", int AgentId = 0,int? page=null)
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
            ViewBag.Services = Services;
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
           IPagedList<AgentTransferLimtViewModel> vm = _agentTransferLimitServices.GetAgentTransferLimit(Country, Services, city, AgentId).ToPagedList(pageNumber,pageSize);
            return View(vm);
        }

        [HttpPost]
        public ActionResult SetTransferLimit(AgentTransferLimtViewModel vm)
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

                if (vm.TransferMethod == 0)
                {
                    ModelState.AddModelError("TransactionTransferMethod", "Select Transfer Method");
                    return View(vm);

                }
                if (vm.Amount == 0)
                {
                    ModelState.AddModelError("Invalid", "Enter Amount");
                    return View(vm);
                }

     
                if (vm.Id > 0)
                {
                     _agentTransferLimitServices.UpdateAgentTransferLimit(vm);

                }
                else
                {

                    _agentTransferLimitServices.AddAgentTransferLimit(vm);

                }
                return RedirectToAction("Index");

            }
            return View(vm);
        }

        [HttpGet]
        public ActionResult SetTransferLimit(int Id = 0)
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

     
            var vm = GetTransferLimit(Id);

            return View(vm);
        }

        public ViewModels.AgentTransferLimtViewModel GetTransferLimit(int Id)
        {

            if (Id > 0)
            {
                var rate = (from c in _agentTransferLimitServices.List().Where(x => x.Id == Id).ToList()
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
                                 LimitType=c.LimitType,
                                 Frequency=c.Frequency
                            }).FirstOrDefault();
                return rate;
            }
            else
            {
                return null; 

            }


        }

        
        [HttpGet]
        public JsonResult DeleteTransferLimit(int Id)
        {
            if (Id > 0)
            {
                _agentTransferLimitServices.Delete(Id);
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
        public ActionResult LimitHistory(string Country = "", int Services = 0, string city = "", int AgentId = 0,int? page=null)
        {
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var City = _CommonServices.GetCities();
            ViewBag.Cities = new SelectList(City, "City", "City");
            var Agent = _CommonServices.GetAgent();
            ViewBag.Agents = new SelectList(Agent, "AgentId", "AgentName");
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<AgentTransferLimtViewModel> vm = _agentTransferLimitServices.GetAgentTransferLimitHistory(Country, Services, city, AgentId).ToPagedList(pageNumber, pageSize);
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