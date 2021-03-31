using FAXER.PORTAL.Areas.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AgentDashboardController : Controller
    {
        AgentDashboardServices _services = null;
        public AgentDashboardController()
        {
            _services = new AgentDashboardServices();
        }
        // GET: Admin/AgentDashboard
        public ActionResult Index(int Id = 0)
        {
            var vm = _services.AgentDetails(Id);
            return View(vm);
        }
    }
}