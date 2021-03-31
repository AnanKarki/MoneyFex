using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.Services
{
    public class CardUserWithdrawalServices
    {

        DB.FAXEREntities dbContext = null;
        public CardUserWithdrawalServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public DB.KiiPayPersonalWalletWithdrawalCode GetExistingCardUserWithdrawalCode(int MFTCCardId)
        {


            var data = dbContext.KiiPayPersonalWalletWithdrawalCode.Where(x => x.KiiPayPersonalWalletId == MFTCCardId && x.IsExpired == false).FirstOrDefault();
            return data;

        }

        public DB.KiiPayPersonalWalletWithdrawalCode AddBNewCardWithdrawalCode(DB.KiiPayPersonalWalletWithdrawalCode model)
        {

            dbContext.KiiPayPersonalWalletWithdrawalCode.Add(model);
            dbContext.SaveChanges();
            return model;


        }
    }
}