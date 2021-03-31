using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SReceiverDetails
    {
        DB.FAXEREntities db = new DB.FAXEREntities();
        public DB.ReceiversDetails Add(DB.ReceiversDetails obj)
        {
            db.ReceiversDetails.Add(obj);
            db.SaveChanges();
            return obj;
        }
    }
}