using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class ThirdPartyMoneyTransferServices
    {
        DB.FAXEREntities dbContext = null;
        public ThirdPartyMoneyTransferServices()
        {
            dbContext = new DB.FAXEREntities();

        }
        public DB.ThirdPartyMoneyTransfer SaveThirdPartyMoneyTransfer(DB.ThirdPartyMoneyTransfer model) {

            dbContext.ThirdPartyMoneyTransfer.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        internal List<ThirdPartyMoneyTransfer> List()
        {
            return dbContext.ThirdPartyMoneyTransfer.ToList();
        }
        internal ThirdPartyMoneyTransfer Update(ThirdPartyMoneyTransfer model)
        {
            dbContext.Entry<ThirdPartyMoneyTransfer>(model).State = EntityState.Modified;

            dbContext.SaveChanges();
            return model;
        }
    }
}