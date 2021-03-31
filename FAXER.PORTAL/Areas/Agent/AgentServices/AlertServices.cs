using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class AlertServices
    {
        DB.FAXEREntities dbContext = null;
        AgentCommissionServices CommonServices = null;
        public AlertServices() {
            dbContext = new DB.FAXEREntities();
            CommonServices = new AgentCommissionServices();
        }
            public List<Models.AlertsViewModel> GetAlerts() {
                var agentInformation = Common.AgentSession.AgentInformation;

                DateTime CurrentDate = DateTime.Now.Date;
                var result = (from c in dbContext.AgentAlerts.Where(x => ((x.AgentId == agentInformation.Id 
                             || x.AgentId == 0) && (x.IsDeleted == false)) &&
                            (x.Country == agentInformation.CountryCode || x.Country == null) &&
                            (x.City.ToLower() == agentInformation.City.ToLower() || x.City == null)
                            && (DbFunctions.TruncateTime(x.PublishedDateAndTime) <= CurrentDate) && DbFunctions.TruncateTime(x.EndDate) >= CurrentDate).ToList()
                              select new Models.AlertsViewModel()
                              {
                                  Id = c.Id,
                                  AlertHeading = c.Heading,
                                  AlertFullMessage = c.FullMessage,
                                  Date = c.PublishedDateAndTime.ToString("dd/MM/yyy"),
                                  Time = c.PublishedDateAndTime.ToString("HH:mm"),
                                  
                              }).ToList();
                return result;

            }

            public DB.AgentAlerts GetAlertsDetialsById(int id) {

                var result = dbContext.AgentAlerts.Where(x => x.Id == id).FirstOrDefault();
                return result;
            }

    }
}