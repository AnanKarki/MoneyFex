using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Common;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AgentSubmittedFormController : Controller
    {
        // GET: Admin/AgentSubmittedForm

        CommonServices _commonServices = null;
        AgentStaffRegistrationServices _agentStaffInformation = null;
        AuxAgentComplianceFormServices _services = null;
        public AgentSubmittedFormController()
        {
            _commonServices = new CommonServices();
            _agentStaffInformation = new AgentStaffRegistrationServices();
            _services = new AuxAgentComplianceFormServices();
        }
        //Note: It is same page as AuxAgentComplianceForm
        public ActionResult Index(string SendingCountry = "", int AgentId = 0, string Date = "", int FormId = 0, string AgentStaffId = "", string staffId = ""
               , int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var agents = _commonServices.GetAuxAgents();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            ViewBag.AgentStaffId = AgentStaffId;
            ViewBag.staffId = staffId;


            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<AuxAgentComplianceFormViewModel> result = _services.GetAuxAgentFormsList(AgentId).ToPagedList(pageNumber, pageSize);

            if (!string.IsNullOrEmpty(SendingCountry))
            {
                result = result.Where(x => x.CountryCode == SendingCountry).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                result = result.Where(x => x.SubDate.ToDateTime() >= FromDate && x.SubDate.ToDateTime() <= ToDate).ToPagedList(pageNumber, pageSize);
            }
            if (AgentId != 0)
            {
                result = result.Where(x => x.AgentId == AgentId).ToPagedList(pageNumber, pageSize);
            }
            if (FormId != 0)
            {
                result = result.Where(x => x.FormId == FormId).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(AgentStaffId))
            {
                AgentStaffId = AgentStaffId.Trim();
                result = result.Where(x => x.AgentStaffId.ToLower().Contains(AgentStaffId.ToLower())).ToPagedList(pageNumber, pageSize);


            }
            if (!string.IsNullOrEmpty(staffId))
            {
                staffId = staffId.Trim();
                result = result.Where(x => x.StaffId.ToLower().Contains(staffId.ToLower())).ToPagedList(pageNumber, pageSize);

            }



            return View(result);
        }

    }
}