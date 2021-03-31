using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SFaxerActivityLog
    {
        DB.FAXEREntities dbContext = null;
        public SFaxerActivityLog()
        {
            dbContext = new DB.FAXEREntities();
        }
        public void AddActivityLog(FaxerActivityLog faxerActivity ) {

            try
            {


                dbContext.FaxerActivityLog.Add(faxerActivity);
                dbContext.SaveChanges();
            }
            catch (Exception)
            {

            }
        }
    }
}