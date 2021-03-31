using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class ComplianceCommissionerServices
    {
        FAXEREntities dbContext = null;
        public ComplianceCommissionerServices()
        {
            dbContext = new FAXEREntities();


        }

        public List<ComplianceCommissionerViewModel> getComplianceComissions()
        {
            var result = (from c in dbContext.ComplianceCommission.ToList()
                          select new ComplianceCommissionerViewModel()
                          {
                              Id = c.Id,
                              StaffName = c.AgentStaff.FirstName + " " + c.AgentStaff.MiddleName + " " + c.AgentStaff.LastName,
                              StaffType = Enum.GetName(typeof(StaffType), c.AgentStaff.AgentStaffType),
                              AppointmentDate = c.AppointmentDateTime.ToString("dd-MM-yyyy"),
                              EndTime = c.AppointmentDateTime.ToString("HH:mm"),
                              Status = c.Status == false ? "Inactive" : "Active",
                              AppointmentDateTime = c.AppointmentDateTime
                          }).OrderByDescending(x=>x.AppointmentDateTime).ToList();
            return result;
        }

        public bool deactivateComplianceCommission(int id)
        {
            if (id != 0) {
                var data = dbContext.ComplianceCommission.Where(x => x.Id == id).FirstOrDefault();
                if (data != null) {
                    data.Status = false;
                    data.LastModifiedBy = Common.AgentSession.AgentStaffLogin.AgentStaffId;
                    data.LastModifiedDate = DateTime.Now;
                    data.ModifierStaffType = ModifierStaffType.AgentStaff;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true;
                }
            }
            return false;
        }

        public bool addComplianceCommission(int agentStaffId )
        {
            if (agentStaffId != 0) {
                DB.ComplianceCommission data = new DB.ComplianceCommission()
                {
                    AgentId = Common.AgentSession.AgentInformation.Id,
                    AgentStaffId = agentStaffId,
                    AppointmentDateTime = DateTime.Now,
                    Status = true,
                    CreatedBy = Common.AgentSession.AgentStaffLogin.AgentStaffId
                };
                dbContext.ComplianceCommission.Add(data);
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