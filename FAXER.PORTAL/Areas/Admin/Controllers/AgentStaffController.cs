using FAXER.PORTAL.Areas.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AgentStaffController : Controller
    {
        AgentStaffServices agentStaffServices = null;
        CommonServices commonServices = null;

        public AgentStaffController()
        {
            agentStaffServices = new AgentStaffServices();
            commonServices = new CommonServices();
        }
        // GET: Admin/AgentStaff
        [HttpGet]
        public ActionResult Index(string message = "" , string Country="")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            if (!string.IsNullOrEmpty(message) )
            {
                ViewBag.Message = message;
            }
            CommonServices commonServices = new CommonServices();
            var Countries = commonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name", Country);
            var vm = agentStaffServices.GetAgentStaffsList(Country);
            return View(vm);
        }

        public ActionResult AgentStaffMoreDetails(int agentStaffId)
        {
            if (agentStaffId != 0)
            {
                var vm = agentStaffServices.getAgentStaffInfo(agentStaffId);
                return View(vm);
            }
            return RedirectToAction("Index", new { @message="failure"});
        }
             

        public ActionResult ActivateDeactivateAgentStaff(int agentStaffId)
        {
            if (agentStaffId != 0)
            {
                bool activateDeactivate = agentStaffServices.activateDeactivateAgentStaff(agentStaffId);
                if (activateDeactivate)
                {
                    return RedirectToAction("Index", new { @message="Activation/Deactivation Successful !" });
                }
            }
            return RedirectToAction("Index", new { @message = "failure" });
        }

        public ActionResult DeleteAgentStaff(int agentStaffId)
        {
            if (agentStaffId != 0)
            {
                bool delete = agentStaffServices.deleteAgentStaff(agentStaffId);
                if (delete)
                {
                    return RedirectToAction("Index", new { @message = "Deletion Successful !" });
                }
            }
            return RedirectToAction("Index", new { @message = "failure" });
        }
    }
}