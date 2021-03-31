using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderPayingSupplierAbroadReferenceOneVM
    {
        public const string BindProperty = "ReceiverName,PhotoUrl,ReferenceNo,BillNo,Amount,SendingAmount,Fee,Total,Currency,ExchangeRate";
        public string ReceiverName { get; set; }
        public string PhotoUrl { get; set; }
        public string ReferenceNo { get; set; }
        public string BillNo { get; set; }
        [Range(1.0, double.MaxValue, ErrorMessage = "Enter amount")]
        public decimal Amount { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal SendingAmount { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal Fee { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal Total { get; set; }
        public string Currency { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal ExchangeRate { get; set; }

    }
}