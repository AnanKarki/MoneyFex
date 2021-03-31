using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPersonalWalletInterMoneyTransfered
    {

        public int Id { get; set; }
        public int SenderId { get; set; }

        public int SendingKiiPayWalletId { get; set; }

        public int ReceivingKiiPayWalletId { get; set; }

        public decimal AmountTransfered { get; set; }

        public DateTime TransactionDate { get; set; }

        public virtual FaxerInformation Sender { get; set; }
    }
}