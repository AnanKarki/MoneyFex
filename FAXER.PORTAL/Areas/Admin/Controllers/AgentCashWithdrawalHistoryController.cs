using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AgentCashWithdrawalHistoryController : Controller
    {
        // GET: Admin/AgentCashWithdrawalHistory
        ViewAgentCashWithdrawalServices service = null;
        CommonServices CommonService = null;
        public AgentCashWithdrawalHistoryController()
        {
            service = new ViewAgentCashWithdrawalServices();
            CommonService = new CommonServices();
        }
        public ActionResult Index(string message = "", string type = "", string Country = "", string City = "", string Date = "",int? page=null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForSCities(Country);

            if (!string.IsNullOrEmpty(message))
            {
                if (message == "confirmed")
                {
                    ViewBag.Message = "Withdrawal Confirmed !";
                }

                else if (message == "error")
                {
                    ViewBag.Message = "Something went wrong. Please contact Admin !";
                }

            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            ViewAgentCashWithdrawalViewModel vm = service.getCashWithdrawalList(pageNumber,pageSize);
            if (Country != "")
            {
                vm.AgentCashWithdrawalList = vm.AgentCashWithdrawalList.Where(x => x.Country == Country).ToList();
                ViewBag.Country = Country;
            }
            if (City != "")
            {
                vm.AgentCashWithdrawalList = vm.AgentCashWithdrawalList.Where(x => x.City == City).ToList();
                ViewBag.City = City;
            }
            if (type != "")
            {

                vm.AgentCashWithdrawalList = vm.AgentCashWithdrawalList.Where(x => x.WithdrawalType == type).ToList();
                ViewBag.Type = type;
            }
            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                vm.AgentCashWithdrawalList = vm.AgentCashWithdrawalList.Where(x => x.TransactionDate >= FromDate && x.TransactionDate <= ToDate).ToList();
                ViewBag.Date = Date;
            }
            return View(vm);
        }
        [HttpGet]
        public ActionResult CashWithDrawalDetails(int AgentId, int Id, int iscashWithdrawal)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            var data = service.getCashMoreDetails(AgentId);

            if (data.WithdrawalCode != null)
            {
                data.Id = Id;
                data.IsCashWithDrawal = iscashWithdrawal;
                return View(data);
            }
            else
            {
                return RedirectToAction("Index", new { @message = "error" });
            }
        }
        [HttpPost]
        public ActionResult CashWithDrawalDetails(MoreWithdrawalDetails model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            if (model.Id != 0)
            {
                bool confirmed = service.confirmWithdrawal(model.Id, model.IsCashWithDrawal);
                if (confirmed)
                {
                    return RedirectToAction("Index", new { @message = "confirmed" });
                }
            }
            return RedirectToAction("Index", new { @message = "error" });
        }


        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Agent, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
    }
}