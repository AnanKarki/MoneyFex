using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BusinessLimtiHistory
    {

        public int Id { get; set; }
        public int SenderId { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public AutoPaymentFrequency Frequency { get; set; }
        public string FrequencyAmount { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        public string AccountNumber { get; set; }
        public bool IsBusiness{ get; set; }

    }
}