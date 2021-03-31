using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewPaySomeoneElseCardViewModel
    {
        #region Sender Information 

        public string SenderName { get; set; }
        public string SenderAccountNo { get; set; }
        public string SenderCountry { get; set; }
        public string SenderCity { get; set; }

        #endregion

        #region MFTC Card Information (Receiver)


        public string CardUserName { get; set; }
        public string CardUserMFTCCardNO { get; set; }
        public string CardUserCountry { get; set; }
        public string CardUserCity { get; set; }
        public string CardUserEmail  { get; set; }



        #endregion

        #region Transaction Information 

        public int TransactionId { get; set; }
        public decimal  TopUpAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal TopUpFee { get; set; }
        public string TopUpReference { get; set; }

        public string TransactionDate { get; set; }
        public string TransactionTime { get; set; }

        public string TopUpBy { get; set; }

        #endregion
    }
}