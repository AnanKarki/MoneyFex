using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AgentDashboardServices
    {
        DB.FAXEREntities dbContext = null;
        public AgentDashboardServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public AgentViewModels AgentDetails(int Id)
        {
            AgentViewModels vm = new AgentViewModels();
            var LoginInfo = dbContext.AgentLogin.Where(x => x.AgentId == Id).ToList();
            var AgentInfo = dbContext.AgentInformation.Where(x => x.Id == Id).ToList();

            var AgentLoginInfo = (from c in AgentInfo
                                  join d in LoginInfo on c.Id equals d.AgentId
                                  select new ViewRegisteredAgentsViewModel()
                                  {
                                      Id = c.Id,
                                      AccountNo = c.AccountNo,
                                      Logincode = d.LoginCode,
                                      Name = c.Name,
                                      City = c.City,
                                      CountryCode = c.CountryCode,
                                      AgentBusinessLicenseNumber = c.RegistrationNumber,
                                      PhoneNumber = Common.Common.GetCountryPhoneCode(c.CountryCode) + c.PhoneNumber,
                                      Address1 = c.Address1,
                                      Address2 = c.Address2,
                                      Country = Common.Common.GetCountryName(c.CountryCode),
                                      Email = c.Email,
                                      ContactPerson = c.ContactPerson,
                                      PostalCode = c.PostalCode,
                                      State = c.State,
                                      Website = c.Website,
                                      RegistrationNumber = c.RegistrationNumber,
                                      AgentStatus = d.IsActive,
                                  }).ToList();

            vm.LoginInformation = AgentLoginInfo;
            return vm;
        }
    }
}