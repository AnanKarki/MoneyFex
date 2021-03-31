using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class RegisteredWithdrawalStaffServices
    {
        FAXEREntities dbContext = null;
        public RegisteredWithdrawalStaffServices()
        {
            dbContext = new FAXEREntities();
        }
        public List<AgentCashWithdrawlViewModel> GetAgentCashWithdrawl(string Country = "", string City = "", string Search = "")
        {
            var result = (from c in dbContext.AgentStaffInformation.Where(x=> x.AgentStaffType == StaffType.Transaction)
                          select new AgentCashWithdrawlViewModel()
                          {
                              Id = c.Id,
                              AgentId = c.AgentId,
                              AgentName = c.Agent.Name,
                              AccountNo = c.AgentMFSCode,
                              StaffName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              FirstLetter = c.FirstName == null ? "" : c.FirstName.Substring(0, 1).ToLower(),


                          }).ToList();
            if (!string.IsNullOrEmpty(Search))
            {
                result = result.Where(x => x.AgentName.ToLower().Contains(Search.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(Country))
            {
                result = result.Where(x => x.Country == Country).ToList();
            }
            if (!string.IsNullOrEmpty(City))
            {
                result = result.Where(x => x.City == City).ToList();
            }

            return result;
        }
    }

}