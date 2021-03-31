using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class IntroductoryRate
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public TransactionTransferType TransactionTransferType { get; set; }
        public TransactionTransferMethod TransactionTransferMethod { get; set; }
        public int AgentId { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }
        public decimal Rate { get; set; }

        /// <summary>
        /// No of Transaction for the rate
        /// </summary>
        public int NoOfTransaction { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public int IntroductoryRateByCurrencyId { get; set; }
    }
    public class IntroductoryRateHistory
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public TransactionTransferType TransactionTransferType { get; set; }
        public TransactionTransferMethod TransactionTransferMethod { get; set; }
        public int AgentId { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }
        public decimal Rate { get; set; }
        /// <summary>
        /// No of Transaction for the rate
        /// </summary>
        public int NoOfTransaction { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

    }
}