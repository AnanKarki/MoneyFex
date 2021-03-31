using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class SupplierStandingOrderPayment
    {
        public int Id { get; set; }
        public int SupplierId { get; set; }
        public int PayerId { get; set; }
        public AutoPaymentFrequency PaymentFrequency { get; set; }
        public decimal Amount { get; set; }
        public string FrequenncyDetails{ get; set; }
    }
}