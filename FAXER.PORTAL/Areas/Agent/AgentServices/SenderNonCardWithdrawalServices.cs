using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class SenderNonCardWithdrawalServices
    {

        DB.FAXEREntities dbContext = null;
        public SenderNonCardWithdrawalServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        

    }
}