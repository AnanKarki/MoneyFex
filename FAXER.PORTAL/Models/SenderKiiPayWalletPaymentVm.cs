using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderKiiPayWalletPaymentVm
    {
        public int Id { get; set; }
        public string Currency { get; set; }
        [Range(1.0, double.MaxValue, ErrorMessage = "Enter amount")]
        public decimal Amount { get; set; }
        public decimal Cents { get; set; }
        public string CardNumber { get; set; }
        public decimal KiiPAyWalletBalance { get; set; }
        public bool SetStandingOrderPayment{ get; set; }
        public decimal EnterAmount{ get; set; }
        public int PaymentFrequency{ get; set; }
    }
}