using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Services.Common
{
    public class MobileRegistrationVerificationCodeService
    {

        DB.FAXEREntities dbContext = null;
        public MobileRegistrationVerificationCodeService()
        {
            dbContext = new DB.FAXEREntities();
        }

        public DB.RegistrationVerificationCode Add(DB.RegistrationVerificationCode model)
        {

            dbContext.RegistrationVerificationCode.Add(model);
            dbContext.SaveChanges();
            return model;

        }


        public bool Update(DB.RegistrationVerificationCode model)
        {
            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }


        public ServiceResult<IQueryable<RegistrationVerificationCode>> List()
        {
            return new ServiceResult<IQueryable<RegistrationVerificationCode>>
            {
                Data = dbContext.RegistrationVerificationCode,
                Status = ResultStatus.OK
            };
        }

    }
}