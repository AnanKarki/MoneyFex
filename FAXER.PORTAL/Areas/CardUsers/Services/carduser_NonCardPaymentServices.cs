using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class CardUser_NonCardPaymentServices
    {
        DB.FAXEREntities dbContext = null;
        public CardUser_NonCardPaymentServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public DB.CardUserNonCardTransaction SaveTransaction(DB.CardUserNonCardTransaction model) {

            dbContext.CardUserNonCardTransaction.Add(model);
            dbContext.SaveChanges();
            return model;

        }

        public DB.CardUserReceiverDetails SaveReceiverDetails(DB.CardUserReceiverDetails model) {

            dbContext.CardUserReceiverDetails.Add(model);
            dbContext.SaveChanges();
            return model; 
        }

      
    }
}