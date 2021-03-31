using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SFaxingNonCardTransaction
    {
        DB.FAXEREntities db = new DB.FAXEREntities();
        public DB.FaxingNonCardTransaction SaveTransaction(DB.FaxingNonCardTransaction obj)
        {
            db.FaxingNonCardTransaction.Add(obj);
            db.SaveChanges();
            return obj;
        }

        internal string GetNewMFCNToSave()
        {
            //this code should be unique and random with 8 digit length
            var val = Common.Common.GenerateRandomDigit(6);

            while (db.FaxingNonCardTransaction.Where(x => x.MFCN == val).Count() > 0)
            {
                val = Common.Common.GenerateRandomDigit(6);
            }
            return val;
        }
        internal string GetNewReceiptNumberToSave()
        {
            //this code should be unique and random with 8 digit length
            var val = "Os-Np-MF" + Common.Common.GenerateRandomDigit(5);
            while (db.FaxingNonCardTransaction.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Os-Np-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }

        internal string GetNewMFTCCardNumber()
        {
            //this code should be unique and random with 8 digit length
            //var val = Common.Common.GenerateRandomDigit(8);

            //MFS-123-123-1234-Firstname
            string val = String.Format("{0:d10}", (DateTime.Now.Ticks / 10) % 1000000000);

            while (db.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == val).Count() > 0)
            {
                val = String.Format("{0:d10}", (DateTime.Now.Ticks / 10) % 1000000000);
            }
            return "MFS-"+ val;
        }

        internal void UpdateTransaction(FaxingNonCardTransaction cashPickUpDetails)
        {
            db.Entry<FaxingNonCardTransaction>(cashPickUpDetails).State = EntityState.Modified;
            //db.FaxingNonCardTransaction.AddOrUpdate
            db.SaveChanges();
            
        }
    }
}