using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderWithdrawMoneyFromWalletSummaryViewModel
    {

        public const string BindProperty = "SendToName,SendToAccount,SendingCurrencySymbol,SendingBalance,SendingCurrencyCode" +
            ",FeeBalance,ReceiveBalance,MobileCode,SendFromName,PinCode,UserEnterPinCode,FormattedAccNo";


        public string SendToName { get; set; }
        public string SendToAccount { get; set; }
         public string SendingCurrencySymbol { get; set; }
        public decimal SendingBalance { get; set; }

        public string SendingCurrencyCode { get; set; }
        public decimal FeeBalance { get; set; }
        public decimal ReceiveBalance { get; set; }
        [Required(ErrorMessage ="Enter Code")]
        public string MobileCode { get; set; }
        public string SendFromName { get; set; }
        public string PinCode { get; set; }
        public string UserEnterPinCode { get; set; }
        public string FormattedAccNo { get; set; }


    }
}