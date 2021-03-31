using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMFTCNonCardTransferViewModel
    {

        #region Sender Information


        public string CardUserName { get; set; }
        public string MFTCCardNumber { get; set; }
        public string CardUserCountry { get; set; }
        public string CardUserCity { get; set; }


        #endregion

        #region Receiver Details 

        public string ReceiverName { get; set; }

        public string ReceiverCountry { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverTelephone { get; set; }
        public string ReceiverEmail { get; set; }


        #endregion


        #region Transaction Details

        public int TransactionId { get; set; }

        public string MFCN { get; set; }
        public decimal TrasactionAmount { get; set; }
        public decimal Fee { get; set; }
        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public string Status { get; set; }

        public DB.FaxingStatus FaxingStatus { get; set; }
        
        public string TransactionIsUpdated { get; set; }

        #endregion
    }
}