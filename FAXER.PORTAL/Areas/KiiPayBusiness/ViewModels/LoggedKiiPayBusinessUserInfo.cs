using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class LoggedKiiPayBusinessUserInfo
    {
        public int KiiPayBusinessInformationId { get; set; }

        public string BusinessName { get; set; }

        public string FullName { get; set; }
        public string UserPhoto{ get; set; }


        public string BusinessEmailAddress { get; set; }
        
        public string BusinessMobileNo { get; set; }

        public string CountryCode { get; set; }

        public string CurrencySymbol { get; set; }


        public string KiiPayBusinessWalletUserName { get; set; }

        public decimal CurrentBalanceOnCard { get; set; }



    }
}