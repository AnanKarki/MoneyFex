using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Services.KiiPayBusiness
{
    public class MobileKiiPayBusinessLoginServices
    {

        DB.FAXEREntities dbContext = null;
        public MobileKiiPayBusinessLoginServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        

        public DB.KiiPayBusinessLogin Add(DB.KiiPayBusinessLogin model)
        {
            dbContext.KiiPayBusinessLogin.Add(model);
            dbContext.SaveChanges();
            return model;

        }

        public ServiceResult<IQueryable<KiiPayBusinessLogin>> List()
        {
            return new ServiceResult<IQueryable<KiiPayBusinessLogin>>
            {
                Data = dbContext.KiiPayBusinessLogin,
                Status = ResultStatus.OK
            };
        }

        public bool Update(DB.KiiPayBusinessLogin model)
        {
            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;

        }


       
    }
}