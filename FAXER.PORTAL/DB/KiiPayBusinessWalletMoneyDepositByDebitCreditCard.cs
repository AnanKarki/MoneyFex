using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayBusinessWalletMoneyDepositByDebitCreditCard
    {

        public int Id { get; set; }
        
        public string KiiPayWalletId { get; set; }

        public string CardNumber { get; set; }

        public string Amount { get; set; }

        public string Country { get; set; }
        public DateTime TransactionDate { get; set; }



    }
}