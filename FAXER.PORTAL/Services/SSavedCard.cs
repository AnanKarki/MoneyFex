using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSavedCard
    {
        DB.FAXEREntities db = new DB.FAXEREntities();

        public DB.SavedCard Add(DB.SavedCard obj)
        {
            db.SavedCard.Add(obj);
            db.SaveChanges();
            return obj;
        }
        public DB.CardTopUpCreditDebitInformation Save(DB.CardTopUpCreditDebitInformation cardInformation)
        {
            db.CardTopUpCreditDebitInformation.Add(cardInformation);
            db.SaveChanges();
            return cardInformation;
        }
    }
}