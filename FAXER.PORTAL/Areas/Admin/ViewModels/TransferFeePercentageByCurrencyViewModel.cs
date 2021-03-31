using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Http.Results;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class TransferFeePercentageByCurrencyViewModel
    {
        public const string BindProperty = "Id ,SendingCurrency ,ReceivingCurrency ,TransferType , TransferTypeName , TransferMethod, TransferMethodName,Range ,ToRange ,FromRange ,  OtherRange, FeeType" +
            ",Fee ,Percentage ,FlatFee ,CreatedDate ,AgentId,RangeList, SendingCountry, ReceivingCountry ";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Currency")]
        public string SendingCurrency { get; set; }
        [Required(ErrorMessage = "Select Currency")]
        public string ReceivingCurrency { get; set; }

        [Required(ErrorMessage = "Select Transfer Type")]
        public TransactionTransferType TransferType { get; set; }
        public string TransferTypeName { get; set; }
        [Required(ErrorMessage = "Select Transfer Method")]

        public TransactionTransferMethod TransferMethod { get; set; }
        public string TransferMethodName { get; set; }
        public string Range { get; set; }
        public decimal ToRange { get; set; }
        public decimal FromRange { get; set; }
        public string OtherRange { get; set; }
        [Required(ErrorMessage = "Select Fee Type")]
        public FeeType FeeType { get; set; }
        [Required(ErrorMessage = "Enter Fee")]
        public decimal Fee { get; set; }
        public decimal Percentage { get; set; }
        public decimal FlatFee { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? AgentId { get; set; }
        public List<string> RangeList { get; set; }

        public string SendingCountryFlag { get; set; }
        public string SendingCountry { get; set; }

        public string ReceivingCountryFlag { get; set; }
        public string ReceivingCountry { get; set; }
    }
}