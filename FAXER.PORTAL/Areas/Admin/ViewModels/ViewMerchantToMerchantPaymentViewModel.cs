using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMerchantToMerchantPaymentViewModel
    {

        #region Sender Information 

        public string SenderName { get; set; }
        public string SenderMFSCode { get; set; }
        public string SenderCountry { get; set; }
        public string SenderCity { get; set; }

        public string SenderCurrency { get; set; }
        public string SenderCurrencySymbol { get; set; }


        #endregion

        #region Receiver Information 

        public string ReceiverName { get; set; }
        public string ReceiverMFSCode { get; set; }
        public string ReceiverCountry { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverEmail { get; set; }
        public string ReceiverCurrency { get; set; }
        public string ReceiverCurrencySymbol { get; set; }



        #endregion

        #region Transaction Information 

        public int TransactionId { get; set; }
        public decimal TransactionAmount { get; set; }
        public decimal Fee { get; set; }
        public string PaymentType { get; set; }

        public DateTime TransactionDateTime { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }

        #endregion
    }
}