using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class PayingSupplierReferenceViewModel
    {

        public const string BindProperty = "ReceiverName , PhotoUrl ,ReferenceNo,BillNo , Amount ,Fee,Total , Currency ,ExchangeRate,AgentCommission , CurrencySymbol";
        public string ReceiverName { get; set; }
        public string PhotoUrl { get; set; }
        public string ReferenceNo { get; set; }

        public string BillNo { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Enter amount")]
        public decimal Amount { get; set; }

        public decimal Fee { get; set; }
        public decimal Total { get; set; }

        public string Currency { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal AgentCommission { get; set; }
        public string CurrencySymbol { get; set; }
    }
}