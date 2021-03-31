using FAXER.PORTAL.Areas.Mobile.Common;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Services.KiiPayBusiness
{
    public class MobileKiiPayBusinessInformationServices
    {

        DB.FAXEREntities dbContext = null;
        public MobileKiiPayBusinessInformationServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public DB.KiiPayBusinessInformation Add(DB.KiiPayBusinessInformation model)
        {

            dbContext.KiiPayBusinessInformation.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public ServiceResult<IQueryable<KiiPayBusinessInformation>> List()
        {
            return new ServiceResult<IQueryable<KiiPayBusinessInformation>>
            {
                Data = dbContext.KiiPayBusinessInformation,
                Status = ResultStatus.OK
            };
        }

        
    }
}