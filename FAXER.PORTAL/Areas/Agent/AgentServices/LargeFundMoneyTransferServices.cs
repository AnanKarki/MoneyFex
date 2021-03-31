using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class LargeFundMoneyTransferServices
    {
        DB.FAXEREntities dbContext = null;
        public LargeFundMoneyTransferServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public LargeFundMoneyTransferFormData Update(LargeFundMoneyTransferFormData model)
        {
            dbContext.Entry<LargeFundMoneyTransferFormData>(model).State = EntityState.Modified;

            dbContext.SaveChanges();
            return model;
        }
        public List<LargeFundMoneyTransferFormData> List()
        {

            return dbContext.LargeFundMoneyTransferFormData.ToList();
        }
        public DB.LargeFundMoneyTransferFormData CreateLargeFundMoneyTransfer(DB.LargeFundMoneyTransferFormData model) {

             
            dbContext.LargeFundMoneyTransferFormData.Add(model);
            dbContext.SaveChanges();
            return model;

        }
    }
}