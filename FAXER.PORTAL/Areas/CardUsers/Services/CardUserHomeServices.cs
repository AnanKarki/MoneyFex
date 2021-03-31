using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class CardUserHomeServices
    {
        DB.FAXEREntities dbContext = null;
        public CardUserHomeServices()
        {
            dbContext = new DB.FAXEREntities();
        }

     
    }
}