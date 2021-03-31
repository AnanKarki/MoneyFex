using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class TransferFeePercentageViewModel
    {
        public const string BindProperty = "Id , SendingCountry ,SendingCountryName,SendingCurrency,ReceivingCountry ," +
            "ReceivingCountryName,ReceivingCurrency, TransferType,TransferMethod,CreatedDate , Range,RangeName , OtherRange,FeeType , " +
            "Fee,Percentage ,FlatFee, AgentId ,RangeList , AgentAccountNo, TransfeFeeByCurrencyId ";

        public int Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string SendingCountry { get; set; }
        public string SendingCountryName { get; set; }
        public string SendingCurrency { get; set; }
        public string SendingCountryFlag { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryName { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCountryFlag { get; set; }
        [Required(ErrorMessage = "Select Transfer Type")]
        public TransactionTransferType TransferType { get; set; }
        [Required(ErrorMessage = "Select Transfer Method")]
        public TransactionTransferMethod TransferMethod { get; set; }

        [Required(ErrorMessage = "Select Range")]
        public TranfserFeeRange Range { get; set; }
        public string RangeName { get; set; }
        public List<string> RangeList { get; set; }
        public int? AgentId { get; set; }
        public string AgentName { get; set; }
        public string OtherRange { get; set; }

        [Required(ErrorMessage = "Select Fee Type")]
        public FeeType FeeType { get; set; }
        public string FeeTypeName { get; set; }

        [Required(ErrorMessage = "Enter Fee")]
        public decimal Fee { get; set; }
        public decimal Percentage { get; set; }
        public decimal FlatFee { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }
        public string TransferMethodName { get; set; }
        public string AgentAccountNo { get; set; }
        public int TransfeFeeByCurrencyId { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}