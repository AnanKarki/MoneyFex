using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SRegisterKiiPayWallet
    {

        DB.FAXEREntities dbContext = null;
        public SRegisterKiiPayWallet()
        {
            dbContext = new DB.FAXEREntities();
        }


        public bool RegisterFamilyAndFriendsKiiPayWallet(SenderKiiPayWalletUserRegistrationViewModel vm) {


            // Add to KiiPayWalletInfo table 
            DB.KiiPayPersonalWalletInformation model = new DB.KiiPayPersonalWalletInformation()
            {
            };

            // Add to senderKiipay Account table 
            DB.SenderKiiPayPersonalAccount senderKiiPayPersonalAccount = new DB.SenderKiiPayPersonalAccount()
            {

            };

            return true;
        }

        

    }
}