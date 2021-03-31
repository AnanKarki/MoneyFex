using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class CardProcessorSelectionViewModel
    {
        public const string BindProperty = " Id, SendingCurrency,ReceivingCurrency ,SendingCountry ,SendingCountryName ,ReceivingCountry ," +
            "ReceivingCountryName ,TransferType ,TransferTypeName ,TransferMethod , TransferMethodName, Range,RangeName , FromRange," +
            "ToRange ,CardProcessorId ,CardProcessorName , CreatedBy, CreatedDate";
        public int? Id { get; set; }
        [Required(ErrorMessage = "Select Currency")]
        public string SendingCurrency { get; set; }
        [Required(ErrorMessage = "Select Currency")]

        public string ReceivingCurrency { get; set; }

        [Required(ErrorMessage = "Select Country")]

        public string SendingCountry { get; set; }
        public string SendingCountryName { get; set; }

        [Required(ErrorMessage = "Select Country")]
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryName { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public string TransferTypeName { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public string TransferMethodName { get; set; }
        public TranfserFeeRange Range { get; set; }
        [Required(ErrorMessage = "Select Range")]
        public string RangeName { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }
        [Required(ErrorMessage = "Select Card Processor")]
        public int CardProcessorId { get; set; }
        public string CardProcessorName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}