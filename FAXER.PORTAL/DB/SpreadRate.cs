using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class SpreadRate
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public int? AgentId { get; set; }
        public string AgentName { get; set; }
        public decimal Rate { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }
    }

    public enum TransactionTransferType
    {
        [Display(Name = "Select Type")]
        [Description("Select Type")]
        Select = 0,
        [Display(Name = "All")]
        [Description("All")]
        All = 5,
        [Display(Name = "Online")]
        [Description("Online")]
        Online = 1,
        [Display(Name = "Agent")]
        [Description("All")]
        Agent = 2,
        [Display(Name = "Admin")]
        [Description("Admin")]
        Admin = 3,
        [Display(Name = "Aux Agent")]
        [Description("Aux Agent")]
        AuxAgent = 4,


    }
    public enum TransactionTransferMethod
    {
        [Display(Name = "Select Method")]
        Select = 0,
        All = 7,
        CashPickUp = 1,
        KiiPayWallet = 2,
        OtherWallet = 3,
        BankDeposit = 4,
        BillPayment = 5,
        ServicePayment = 6,


    }
}