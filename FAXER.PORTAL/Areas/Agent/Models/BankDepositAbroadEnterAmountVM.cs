using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class BankDepositAbroadEnterAmountVM
    {

        public const string BindProperty = "Id , ImageUrl , ReceiverName, ReceiverId, SendingCurrencySymbol, SendingCurrencyCode, SendingAmount, ReceivingCurrencySymbol," +
                                           " ReceivingCurrencyCode ,ReceivingAmount ,Fee,TotalAmount" +
            ",ExchangeRate,IsConfirm,AgentCommission,ReceivingCountry,SendingCountry ";


        public int Id { get; set; }

        public string ImageUrl { get; set; }

        public string ReceiverName { get; set; }

        public int ReceiverId { get; set; }

        public string SendingCurrencySymbol { get; set; }

        
        public string SendingCurrency { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Enter  amount")]
        public decimal SendingAmount { get; set; }

        public string ReceivingCurrencySymbol { get; set; }

        public string ReceivingCurrency { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Enter  amount")]
        public decimal ReceivingAmount { get; set; }

        public decimal Fee { get; set; }

        public decimal TotalAmount { get; set; }

        public decimal ExchangeRate { get; set; }

        [Required(ErrorMessage = "Please Confirm")]
        public bool IsConfirm { get; set; }

        public decimal AgentCommission { get; set; }
        public string ReceivingCountry { get; set; }
        public string SendingCountry { get; set; }
    }
}