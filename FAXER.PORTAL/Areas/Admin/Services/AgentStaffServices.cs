using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AgentStaffServices
    {
        FAXEREntities dbcontext = null;
        CommonServices commonServices = null;

        public AgentStaffServices()
        {
            dbcontext = new FAXEREntities();
            commonServices = new CommonServices();
        }

        public List<ViewAgentStaffsViewModel> GetAgentStaffsList(string Country="")
        {
            var data = dbcontext.AgentStaffInformation.Where(x => x.IsDeleted == false).ToList();
            if (!string.IsNullOrEmpty(Country)) {

                data = data.Where(x => x.Country.ToLower() == Country.ToLower()).ToList();
            }
            var result = (from c in data
                          join d in dbcontext.AgentStaffLogin on c.Id equals d.AgentStaffId
                          select new ViewAgentStaffsViewModel()
                          {
                              Id = c.Id,
                              AgentStaffId = c.Id,
                              StaffName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              DOB = c.DateOfBirth.ToString("yyyy-MM-dd"),
                              Gender = Enum.GetName(typeof(Gender), c.Gender),
                              FullAddress = c.Address1 + ", " + c.City + ", " + c.State + ", " + c.Country,
                              Email = c.EmailAddress,
                              Telephone = c.PhoneNumber,
                              StaffCode = d.StaffLoginCode,
                              Status = d.IsActive
                          }).ToList();
            return result;
        }

        public ViewAgentStaffMoreDetailsViewModel getAgentStaffInfo(int agentStaffId)
        {
            var result = (from c in dbcontext.AgentStaffInformation.Where(x => x.Id == agentStaffId).ToList()
                          join d in dbcontext.AgentStaffLogin on c.Id equals d.AgentStaffId
                          select new ViewAgentStaffMoreDetailsViewModel()
                          {
                              Id = c.Id,
                              AgentStaffLevel = Enum.GetName(typeof(StaffType), c.AgentStaffType),
                              IDType = c.IdCardType,
                              IDNumber = c.IdCardNumber,
                              ExpiryDate = c.IdCardExpiryDate.ToString("yyyy-MM-dd"),
                              IssuingCountry = commonServices.getCountryNameFromCode(c.IssuingCountry),
                              StartTime = d.StartTime,
                              EndTime = d.EndTime,
                              StartDay = Enum.GetName(typeof(DayOfWeek), d.StartDay),
                              EndDay = Enum.GetName(typeof(DayOfWeek), d.EndDay),
                              StaffName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              StaffIDUrl = c.IDDocPhoto,
                              PassportSide1Url = c.Passport1Photo,
                              PassportSide2Url = c.Passport2Photo
                          }).FirstOrDefault();
            return result;
        }

        public bool activateDeactivateAgentStaff(int agentStaffId)
        {
            var data = dbcontext.AgentStaffLogin.Where(x => x.AgentStaffId == agentStaffId).FirstOrDefault();
            if (data != null)
            {
                if (data.IsActive == true)
                {
                    data.IsActive = false;                   
                }
                else if (data.IsActive == false)
                {
                    data.IsActive = true;
                }
                dbcontext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbcontext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool deleteAgentStaff(int agentStaffId)
        {
            var data = dbcontext.AgentStaffInformation.Where(x => x.Id == agentStaffId).FirstOrDefault();
            if (data != null)
            {
                data.IsDeleted = true;
                dbcontext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbcontext.SaveChanges();
                return true;
            }
            return false;

        }


    }
}