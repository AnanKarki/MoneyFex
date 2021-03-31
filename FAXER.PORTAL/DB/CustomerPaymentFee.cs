using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class CustomerPaymentFee
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public decimal BankTransfer { get; set; }
        public decimal DebitCard { get; set; }
        public decimal CreditCard{ get; set; }
        public decimal KiiPayWallet{ get; set; }
    }
}