using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderWithdrawMoneyFromWallet
    {
        DB.FAXEREntities dbContext = null;
        public SSenderWithdrawMoneyFromWallet()
        {
            dbContext = new DB.FAXEREntities();

        }
       

    }
}