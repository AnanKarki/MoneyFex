using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderAddNewCardVM
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        public decimal AvailableBalance { get; set; }
        public decimal AvailableBalanceCents { get; set; }
        public CreditDebitCardType CreditDebitCardType { get; set; }
        public string CardNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        public string SecurityCode{ get; set; }
        public int AddressId{ get; set; }
    }
}