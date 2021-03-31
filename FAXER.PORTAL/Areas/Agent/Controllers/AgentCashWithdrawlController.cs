using FAXER.PORTAL.Areas.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentCashWithdrawlController : Controller
    {
        CommonServices _commonservices = null;
        public AgentCashWithdrawlController()
        {
            _commonservices = new CommonServices();
        }

        // GET: Agent/AgentCashWithdrawl
        public ActionResult Index(DateTime year, int month = 0, int day = 0)
        {
            ViewBag.Countries = new SelectList(_commonservices.GetCountries().ToList(), "Code", "Name");
            ViewBag.Cities = new SelectList(_commonservices.GetCities().ToList(), "Id", "City");
            return View();
        }
    }
}