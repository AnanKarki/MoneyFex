using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SpreadRateViewModel
    {

        public const string BindProperty = " Id, SendingCountry , ReceivingCountry,TransferType , TransferMethod, AgentId,AgentName ,Rate , CreatedDate, CreatedById, KiiPayWallet, " +
            "CashPickUp,ServicePayment , OtherWallet , BankDeposit ,BillPayment";
        public int Id { get; set; }

        [Required(ErrorMessage = "Select Country")]
        public string SendingCountry { get; set; }
        public string SendingCountryFlag { get; set; }

        [Required(ErrorMessage = "Select Country")]
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryFlag { get; set; }

        [Required(ErrorMessage = "Select Transfer Type")]
        public TransactionTransferType TransferType { get; set; }

        [Required(ErrorMessage = "Select Transfer Method Type")]
        public TransactionTransferMethod TransferMethod { get; set; }

        public int? AgentId { get; set; }

        public string AgentName { get; set; }

        [Required(ErrorMessage = "Enter Rate")]
        public decimal Rate { get; set; }

        public DateTime CreatedDate { get; set; }
        public int CreatedById { get; set; }

        public decimal KiiPayWallet { get; set; }
        public decimal CashPickUp { get; set; }
        public decimal ServicePayment { get; set; }
        public decimal OtherWallet { get; set; }
        public decimal BankDeposit { get; set; }
        public decimal BillPayment { get; set; }


    }
}