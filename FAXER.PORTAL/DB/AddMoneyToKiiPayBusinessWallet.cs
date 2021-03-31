using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AddMoneyToKiiPayBusinessWallet
    {
        public int Id { get; set; }
        public int KiipayBusinessWalletId { get; set; }
        public decimal Amount { get; set; }
        public string CardNum { get; set; }
        public string NameOnCard { get; set; }
        public string StripeTokenId { get; set; }
        public DateTime TransactionDate { get; set; }

    }
}