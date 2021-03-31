using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class BusinessAlertsServices
    {
        DB.FAXEREntities dbContext = null;

        public BusinessAlertsServices() {
            dbContext = new DB.FAXEREntities();
        }

        public List<ViewModels.BusinessAlertsViewModel> GetAlerts()
        {
            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;

            var BusinessInformation = dbContext.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessInformationId).FirstOrDefault();
            DateTime CurrentDate = DateTime.Now.Date;
            var result = (from c in dbContext.BusinessAlerts.Where(x => ((x.SenderId == KiiPayBusinessInformationId
                         || x.SenderId == 0) && (x.IsDeleted == false)) &&
                        (x.Country == BusinessInformation.BusinessOperationCountryCode || x.Country == null) &&
                        (x.City.ToLower() == BusinessInformation.BusinessOperationCity.ToLower() || x.City == null)
                        && (DbFunctions.TruncateTime(x.PublishedDateAndTime) <= CurrentDate) && DbFunctions.TruncateTime(x.EndDate) >=CurrentDate).ToList()
                          select new ViewModels.BusinessAlertsViewModel()
                          {
                              Id = c.Id,
                              AlertHeading = c.Heading,
                              AlertFullMessage = c.FullMessage,
                              Date = c.PublishedDateAndTime.ToString("dd/MM/yyy"),
                              Time = c.PublishedDateAndTime.ToString("HH:mm")
                          }).ToList();
            return result;

        }

        public DB.BusinessAlerts GetAlertsDetialsById(int id)
        {

            var result = dbContext.BusinessAlerts.Where(x => x.Id == id).FirstOrDefault();
            return result;
        }

    }
}