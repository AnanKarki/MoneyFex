using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class MobileMoneyTransferEnterAmountViewModel
    {
        public const string BindProperty = "Id , ImageUrl,SendingCountry,ReceivingCountry,ReceiverName,ReceiverId , SendingCurrencySymbol ,SendingCurrencyCode," +
            "SendingAmount , AgentCommission ,ReceivingCurrencySymbol,ReceivingCurrencyCode , ReceivingAmount ,Fee, TotalAmount ,ExchangeRate ,IsConfirm";
        public int Id { get; set; }

        public string ImageUrl { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }

        public string ReceiverName { get; set; }
        public int ReceiverId { get; set; }


        public string SendingCurrencySymbol { get; set; }


        public string SendingCurrencyCode { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Enter amount")]
        public decimal SendingAmount { get; set; }

        public decimal AgentCommission { get; set; }
        public string ReceivingCurrencySymbol { get; set; }

        public string ReceivingCurrencyCode { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Enter amount")]
        public decimal ReceivingAmount { get; set; }

        public decimal Fee { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal ExchangeRate { get; set; }
        [Required( ErrorMessage = "Please Confirm")]
        public bool IsConfirm { get; set; }

    }
}