using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.MoblieTransferApi.Models
{
    public class MobileTransferApiConfigurationVm
    {

        public string apiUrl { get; set; }
        public string apirefId {
            get; set;

        }
        public string apiKey { get; set; }
        public string subscriptionKey { get; set; }
    }

    public class MobileTransferAccessTokeneResponse  : MobileTransferApiConfigurationVm
    {

        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }

    }


    public class BalanceCheckReponseVm
    {
        
        public string availableBalance { get; set; }
        public string currency { get; set; }
        public string code { get; set; }
        public string message { get; set; }

        

    }
}