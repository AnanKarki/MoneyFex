using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewComplianceCommissionsServices
    {
        FAXEREntities dbContext = null;
        public ViewComplianceCommissionsServices()
        {
            dbContext = new FAXEREntities();
        }

        public complianceCommissionVm getComplianceComissions()
        {
            complianceCommissionVm vm = new complianceCommissionVm();
            var result = (from c in dbContext.ComplianceCommission.ToList()
                          select new ComplianceCommissionerViewModel()
                          {
                              Id = c.Id,
                              AgentStaffId = c.AgentStaffId,
                              StaffName = c.AgentStaff.FirstName + " " + c.AgentStaff.MiddleName + " " + c.AgentStaff.LastName,
                              StaffType = Enum.GetName(typeof(StaffType), c.AgentStaff.AgentStaffType),
                              AppointmentDate = c.AppointmentDateTime.ToString("dd-MM-yyyy"),
                              EndTime = c.AppointmentDateTime.ToString("HH:mm"),
                              Status = c.Status == false ? "Inactive" : "Active",
                              AppointmentDateTime = c.AppointmentDateTime
                          }).OrderByDescending(x => x.AppointmentDateTime).ToList();
            vm.complianceCommissionsList = result;
            return vm;
        }

        public bool deactivateComplianceCommission(int id)
        {
            var data = dbContext.ComplianceCommission.Where(x => x.Id == id).FirstOrDefault();
            if (data != null)
            {
                data.Status = false;
                data.LastModifiedBy = Common.StaffSession.LoggedStaff.StaffId;
                data.LastModifiedDate = DateTime.Now;
                data.ModifierStaffType = ModifierStaffType.AdminStaff;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public List<AgentStaffListViewModel> getAgentStaffList()
        {
            var data = (from c in dbContext.AgentStaffInformation.Where(x => x.IsDeleted == false)
                        select new AgentStaffListViewModel()
                        {
                            Id = c.Id,
                            AgentStaffName = c.FirstName + " " + c.MiddleName + " " + c.LastName
                        }).ToList();
            return data;

        }

    }
}