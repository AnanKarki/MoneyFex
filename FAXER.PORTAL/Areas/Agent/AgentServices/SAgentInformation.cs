using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{

    public class SAgentInformation
    {
        DB.FAXEREntities dbContext = null;
        public SAgentInformation()
        {

            dbContext = new DB.FAXEREntities();

        }

        public ServiceResult<IQueryable<AgentLogin>> AgentLoginList()
        {
            return new ServiceResult<IQueryable<AgentLogin>>()
            {
                Data = dbContext.AgentLogin,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<IQueryable<AgentStaffLogin>> AgentStaffLoginList()
        {
            return new ServiceResult<IQueryable<AgentStaffLogin>>()
            {
                Data = dbContext.AgentStaffLogin,
                Message = "",
                Status = ResultStatus.OK
            };
        } 
        public ServiceResult<IQueryable<AgentStaffInformation>> AgentStaffInfoList()
        {
            return new ServiceResult<IQueryable<AgentStaffInformation>>()
            {
                Data = dbContext.AgentStaffInformation,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<IQueryable<AgentInformation>> list()
        {
            return new ServiceResult<IQueryable<AgentInformation>>()
            {
                Data = dbContext.AgentInformation,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<AgentInformation> Update(AgentInformation model)
        {
            dbContext.Entry<AgentInformation>(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<AgentInformation>()
            {
                Data = model,
                Message = "",
                Status = ResultStatus.OK

            };
        }
        public ServiceResult<AgentLogin> UpdateAgentLogin(AgentLogin model)
        {
            dbContext.Entry<AgentLogin>(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<AgentLogin>()
            {
                Data = model,
                Message = "",
                Status = ResultStatus.OK

            };
        } 
        public ServiceResult<AgentStaffLogin> UpdateAgentStaffLogin(AgentStaffLogin model)
        {
            dbContext.Entry<AgentStaffLogin>(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<AgentStaffLogin>()
            {
                Data = model,
                Message = "",
                Status = ResultStatus.OK

            };
        }

        public ServiceResult<IQueryable<AgentStaffInformation>> GetAgentStaffInfo()
        {

            return new ServiceResult<IQueryable<AgentStaffInformation>>()
            {
                Data = dbContext.AgentStaffInformation,
                Message = "",
                Status = ResultStatus.OK
            };
        }
    }
}