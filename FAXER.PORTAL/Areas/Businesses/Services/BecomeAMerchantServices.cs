using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class BecomeAMerchantServices
    {
        DB.FAXEREntities dbContext = null;

        public BecomeAMerchantServices() {

            dbContext = new DB.FAXEREntities();
        }

        public DB.BecomeAMerchant GetBecomeAMerchantDetails(string ActivationCode) {

            var result = dbContext.BecomeAMerchant.Where(x => x.ActivationCode == ActivationCode).FirstOrDefault();
            return result;
        }
        public bool EmailExist(string email) {

            var result = dbContext.BecomeAMerchant.Where(x => x.BusinessEmailAddress == email).FirstOrDefault();
            if (result != null) {
                return true;
            }
            return false;
        }
        public DB.BecomeAMerchant AddBusinessMerchant(DB.BecomeAMerchant businessMerchant) {

            dbContext.BecomeAMerchant.Add(businessMerchant);
            dbContext.SaveChanges();
            return businessMerchant;

        }
    }
}