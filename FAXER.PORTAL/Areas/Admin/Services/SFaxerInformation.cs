using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    
    public class SFaxerInformation
    {
        FAXEREntities dbContext = new FAXEREntities();
        public ServiceResult<IQueryable<FaxerInformation>> list()
        {
            return new ServiceResult<IQueryable<FaxerInformation>>()
            {
                Data = dbContext.FaxerInformation,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<FaxerInformation> Update(FaxerInformation model)
        {
            dbContext.Entry<FaxerInformation>(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<FaxerInformation>()
            {
                Data = model,
                Message = "",
                Status = ResultStatus.OK

            };
        }
    }
}