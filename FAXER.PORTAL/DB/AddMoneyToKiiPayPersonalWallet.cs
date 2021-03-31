using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AddMoneyToKiiPayPersonalWallet
    {
        public int Id { get; set; }
        public int KiipayPersonalWalletId { get; set; }
        public decimal Amount { get; set; }
        public string CardNum { get; set; }
        public string NameOnCard { get; set; }
        public string StripeTokenId { get; set; }
        public DateTime TransactionDate { get; set; }
        public bool IsRefunded { get; set ; }
        public int? RefundId { get; set; }
    }
}