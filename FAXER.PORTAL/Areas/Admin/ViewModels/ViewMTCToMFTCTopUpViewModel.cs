using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMFTCToMFTCTopUpViewModel
    {

        #region Sender Information

        public string SenderFullName { get; set; }
        public string SenderMFTCCardNumber { get; set; }
        public string SenderCountry { get; set; }
        public string SenderCity { get; set; }

        #endregion

        #region Receiver Information

        public string ReceiverFullName { get; set; }
        public string ReceiverMFTCCardNumber { get; set; }
        public string ReceiverCountry { get; set; }
        public string ReceiverCity { get; set; }

        public string ReceiverEmail { get; set; }

        #endregion

        #region Transaction Information 
        public decimal TopUpAmount { get; set; }
        public decimal TopUpFee { get; set; }
        public string TopUpType { get; set; }

        public DB.PaymentType TopUpTypeEnum { get; set; }

        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }
        public int TransactionId { get; set; }


        #endregion


    }
}