using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMFTCToMerchantPaymentViewModel
    {

        #region Sender Information 

        public string CardUserName { get; set; }
        public string CardUserMFTCCardNumber { get; set; }
        public string CardUserCountry { get; set; }
        public string CardUserCity { get; set; }

        #endregion

        #region Receiver Information 

        public string MerchantName { get; set; }
        public string MerchantAccountNo { get; set; }
        public string MerchantCountry { get; set; }
        public string MerchantCity { get; set; }
        public string MerchantEmail { get; set; }


        #endregion

        #region TransactionInformation


        public int TransactionID { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal Fee { get; set; }
        public string PaymentType { get; set; }

        public string TransationDate { get; set; }
        public string TransactionTime { get; set; }

        public DateTime TransactionDateTime { get; set; }
        
        #endregion
    }
}