using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class ReceiverLimit
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int ReceiverId { get; set; }
   
        public TransactionTransferMethod TransferMethod { get; set; }
        public AutoPaymentFrequency Frequency { get; set; }
        public decimal Amount { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

    }
}