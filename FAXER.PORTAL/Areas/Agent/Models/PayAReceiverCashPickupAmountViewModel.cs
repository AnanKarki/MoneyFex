using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class PayAReceiverCashPickupAmountViewModel
    {

        public const string BindProperty = "Id , AmountSent ,ReceivingAmount ,IsConfirm , PickUpCurrency ,SendingCurrency ,ReceivingCurrency  , AgentCommission";

        public int Id { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Enter the amount")]
        public decimal AmountSent { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Enter the amount")]
        public decimal ReceivingAmount { get; set; }
        [Required(ErrorMessage = "Are you Confirm?")]

        public bool IsConfirm { get; set; }
        public string PickUpCurrency { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }

        public decimal AgentCommission { get; set; }
    }
}