using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class MobileMoneyTransferResposeStatus
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public string Reason { get; set; }
        public string Code { get; set; }
        public int TransactionId { get; set; }
        public DateTime TransactionDateTime { get; set; }
        public string refId { get; set; }
        public virtual MobileMoneyTransfer Transaction { get; set; }

    }
    public class MobileMoneyTransferTransactionResponseResult
    {

        public int Id { get; set; }
        public int MobileMoneyTransferResposeStatusId { get; set; }
        public string PaymentAmount { get; set; }

        public string Currency { get; set; }
        public string externalId { get; set; }
        public string payerMessage { get; set; }
        public string payeeNote { get; set; }

          /// <summary>
        /// MSISDN (Mobile Station International Subscriber Directory Number)
        /// </summary>
        public string partyIdType { get; set; }

        /// <summary>
        /// Mobile Number
        /// </summary>
        public string partyId { get; set; }

        public virtual MobileMoneyTransferResposeStatus MobileMoneyTransferResposeStatus { get; set; }

    }
}