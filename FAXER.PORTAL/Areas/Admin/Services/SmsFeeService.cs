using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class SmsFeeService
    {
        FAXEREntities db = null;

        public SmsFeeService()
        {
            db = new FAXEREntities();
        }

        public SmsCharge Post(SmsCharge smsCharge)
        {
             db.SmsFee.Add(smsCharge);
            db.SaveChanges();
            return smsCharge;
        }

        public List<SmsCharge> getFeeList()
        {
            var result = db.SmsFee.Where(x => x.IsDeleted == false).ToList();
            return result;
        }

        public bool Delete(SmsCharge model)
        {
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return true;
        }


        public bool Update(SmsCharge model)
        {
            db.Entry(model).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            return true;
        }


    }
}