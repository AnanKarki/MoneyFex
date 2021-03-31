using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Services.KiiPayBusiness
{
    public class MobileKiiPayBusinessUserPersonalInfoService
    {


        DB.FAXEREntities dbContext = null;
        public MobileKiiPayBusinessUserPersonalInfoService()
        {
            dbContext = new DB.FAXEREntities();
        }

        public DB.KiiPayBusinessUserPersonalInfo Add(DB.KiiPayBusinessUserPersonalInfo model)
        {
            dbContext.KiiPayBusinessUserPersonalInfo.Add(model);
            dbContext.SaveChanges();
            return model;

        }
    }
}