using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    /// <summary>
    /// Agent Staff
    /// </summary>
    public class RegisteredWithdrawalStaffController : Controller
    {
        RegisteredWithdrawalStaffServices _services = null;
        public RegisteredWithdrawalStaffController()
        {
            _services = new RegisteredWithdrawalStaffServices();
        }
        // GET: Admin/RegisteredWithdrawalStaff
        public ActionResult Index(string Country = "", string City = "", string Search = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            List<AgentCashWithdrawlViewModel> vm = new List<AgentCashWithdrawlViewModel>();

            vm = _services.GetAgentCashWithdrawl(Country, City, Search);

            string[] alpha = vm.GroupBy(x => x.FirstLetter).
                Select(x => x.FirstOrDefault()).OrderBy(x => x.FirstLetter).Select(x => x.FirstLetter).ToArray();
            ViewBag.Alpha = alpha;
            return View(vm);
        }
        public ActionResult WithdrawalStaffDashboard()
        {
            return View();
        }
        public ActionResult GenerateWithdrawalCode()
        {
            return View();
        }
    }
}