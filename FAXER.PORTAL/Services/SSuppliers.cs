using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{


    public class SSuppliers
    {
        DB.FAXEREntities dbContext = null;
        public SSuppliers()
        {
            dbContext = new DB.FAXEREntities();
        }
        public ServiceResult<IQueryable<Suppliers>> List()
        {
            return new ServiceResult<IQueryable<Suppliers>>()
            {
                Data = dbContext.Suppliers,
                Status =ResultStatus.OK  
            };

        }
    }
}