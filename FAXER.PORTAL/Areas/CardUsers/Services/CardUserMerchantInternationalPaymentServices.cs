using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class CardUserMerchantInternationalPaymentServices
    {
        DB.FAXEREntities dbContext = null;
        public CardUserMerchantInternationalPaymentServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public DB.KiiPayPersonalInternationalKiiPayBusinessPayment SaveTransaction(DB.KiiPayPersonalInternationalKiiPayBusinessPayment model) {


            dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Add(model);
            dbContext.SaveChanges();
            return model;
        }

    }
}