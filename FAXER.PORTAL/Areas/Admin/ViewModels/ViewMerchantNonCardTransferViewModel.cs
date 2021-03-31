using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMerchantNonCardTransferViewModel
    {

        #region Sender Information

        public string MerchantName { get; set; }
        public string MerchantAccountNo { get; set; }
        public string MerchantCountry { get; set; }
        public string MerchantCity { get; set; }

        #endregion

        #region Receiver Information 


        public string ReceiverName { get; set; }
        public string ReceiverCountry { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverTelephone { get; set; }
        public string ReceiverEmail { get; set; }

        #endregion

        #region Transaction Information

        public int TransactionId { get; set; }
        public decimal TransferedAmount { get; set; }
        public decimal Fee { get; set; }
        public string MFCN { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string TransactionStatus { get; set; }

        public DB.FaxingStatus FaxingStatus { get; set; }

        public string TransactionIsUpdated { get; set; }


        #endregion

    }
}