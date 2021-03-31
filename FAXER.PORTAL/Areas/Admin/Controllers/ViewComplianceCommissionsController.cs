using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewComplianceCommissionsController : Controller
    {
        ViewComplianceCommissionsServices complianceCommissionsServices = null;
        public ViewComplianceCommissionsController()
        {
            complianceCommissionsServices = new ViewComplianceCommissionsServices();
        }
        // GET: Admin/ViewComplianceCommissions
        public ActionResult Index(int staffId=0,  int day=0, int month=0, int year=0)
        {
            ViewBag.Days = new SelectList(Enumerable.Range(1, 32));
            ViewBag.Years = new SelectList(Enumerable.Range(2018, 10));
            var agentStaffs = complianceCommissionsServices.getAgentStaffList();
            ViewBag.AgentStaffs = new SelectList(agentStaffs, "Id", "AgentStaffName");
            
            var vm = complianceCommissionsServices.getComplianceComissions();
            if (staffId != 0)
            {
                vm.complianceCommissionsList = vm.complianceCommissionsList.Where(x => x.AgentStaffId == staffId).ToList();
                vm.FilterStaffId = staffId; 
            }            
            if (day != 0)
            {
                vm.complianceCommissionsList = vm.complianceCommissionsList.Where(x => x.AppointmentDateTime.Day == day).ToList();
                vm.FilterDay = day;
            }
            if (month != 0)
            {
                vm.complianceCommissionsList = vm.complianceCommissionsList.Where(x => x.AppointmentDateTime.Month == month).ToList();
                vm.FilterMonth = (Month)month;
            }
            if (year != 0)
            {
                vm.complianceCommissionsList = vm.complianceCommissionsList.Where(x => x.AppointmentDateTime.Year == year).ToList();
                vm.FilterYear = year;
            }

            return View(vm);
        }

        public ActionResult DeActivateComplianceCommission(int id)
        {
            if (id != 0)
            {
                bool deactivate = complianceCommissionsServices.deactivateComplianceCommission(id);

            }
            return RedirectToAction("Index");
        }
    }
}