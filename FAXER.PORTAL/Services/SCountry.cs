using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SCountry
    {

        DB.FAXEREntities dbContext = null;
        public SCountry()
        {
            dbContext = new DB.FAXEREntities();
        }
        public SCountry(DB.FAXEREntities dbContext)
        {
            this.dbContext = dbContext;
        }

        public Country GetCountry() {

            return dbContext.Country.FirstOrDefault();
        }
    }
}