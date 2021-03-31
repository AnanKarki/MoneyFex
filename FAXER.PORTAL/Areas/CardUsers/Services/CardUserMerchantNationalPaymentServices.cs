using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class CardUserMerchantNationalPaymentServices
    {
        DB.FAXEREntities dbContext = null;
        public CardUserMerchantNationalPaymentServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public DB.KiiPayPersonalNationalKiiPayBusinessPayment SaveTransaction(DB.KiiPayPersonalNationalKiiPayBusinessPayment model) {

            dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Add(model);
            dbContext.SaveChanges();
            CardUserCommonServices cardUserCommonServices = new CardUserCommonServices();
            
            return model;

        }   
    }
}