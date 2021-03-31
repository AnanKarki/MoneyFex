using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class TransferExchangeRateByCurrencyViewModel
    {
        public const string BindProperty = "Id , SendingCurrency,ReceivingCurrency ,CreatedDate , CreatedBy ,TransferType ,TransferMethod, TransferMethodName, AgentId" +
            ", Rate,KiiPayWallet ,CashPickUp , ServicePayment, OtherWallet, BankDeposit,BillPayment , AgentName, Range, SendingCountry ,ReceivingCountry ";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Currency")]
        public string SendingCurrency { get; set; }
        [Required(ErrorMessage = "Select Currency")]
        public string ReceivingCurrency { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public string TransferMethodName { get; set; }
        public int? AgentId { get; set; }
        [Required(ErrorMessage = "Enter Rate")]
        public decimal Rate { get; set; }
        public decimal KiiPayWallet { get; set; }
        public decimal CashPickUp { get; set; }
        public decimal ServicePayment { get; set; }
        public decimal OtherWallet { get; set; }
        public decimal BankDeposit { get; set; }
        public decimal BillPayment { get; set; }
        public string AgentName { get; set; }
        public string Range { get; set; }
        public string SendingCountry{ get; set; }
        public string SendingCountryFlag { get; set; }
        public string ReceivingCountry{ get; set; }
        public string ReceivingCountryFlag { get; set; }
        public decimal FromRange{ get; set; }
        public decimal ToRange{ get; set; }
    }
}