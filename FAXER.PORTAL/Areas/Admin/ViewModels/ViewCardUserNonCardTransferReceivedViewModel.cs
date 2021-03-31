using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewCardUserNonCardTransferReceivedViewModel
    {
        #region Sender Infomation 


        public string SenderName { get; set; }
        public string SenderAddress { get; set; }
        public string SenderCountry { get; set; }
        public string SenderCity { get; set; }
        public string SenderTel { get; set; }
        public string SenderEmail { get; set; }

        #endregion

        #region Receiver Information 


        public string ReceiverName { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverCountry { get; set; }
        public string ReceiverTel { get; set; }
        public string ReceiverEmail { get; set; }

        public string IDCardNO { get; set; }
        public string IDCardType { get; set; }
        public string IDExpiryDate { get; set; }
        public string IDIssuingCountry { get; set; }

        #endregion

        #region Transaction Information 


        public int TransactionID { get; set; }
        public decimal AmountReceived { get; set; }
        public decimal AmountSent { get; set; }

        public string MFCN { get; set; }

        #endregion
    }


    public class ViewCardUserNonCardTransferReceivedAdditonalInformatinViewModel
    {


        public int TransactionId { get; set; }

        public string PayingAgent { get; set; }
        public string NameOfAgency { get; set; }
        public string AgentMFCode { get; set; }
        public string MFCN { get; set; }

        public string StatusOFTransaction { get; set; }

    }
}