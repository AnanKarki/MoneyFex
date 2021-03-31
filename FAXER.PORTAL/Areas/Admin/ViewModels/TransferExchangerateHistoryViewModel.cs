using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class TransferExchangerateHistoryViewModel
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCountryFlag { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCountryFlag { get; set; }
        public string CreatedBy { get; set; }
        public string CreatedDate { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public string TransferMethodName { get; set; }
        public int? AgentId { get; set; }
        public decimal Rate { get; set; }
        public decimal KiiPayWallet { get; set; }
        public decimal CashPickUp { get; set; }
        public decimal ServicePayment { get; set; }
        public decimal OtherWallet { get; set; }
        public decimal BankDeposit { get; set; }
        public decimal BillPayment { get; set; }
        public string AgentName { get; set; }
        public string Range { get; set; }

    }
}