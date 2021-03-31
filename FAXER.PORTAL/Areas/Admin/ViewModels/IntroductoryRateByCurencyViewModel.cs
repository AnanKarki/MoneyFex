using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class IntroductoryRateByCurencyViewModel
    {
        public const string BindProperty = "Id, SendingCurrency,ReceivingCurrency ,TransactionTransferType ,TransactionTransferTypeName,TransactionTransferMethod,TransactionTransferMethodName" +
            " , AgentId,AgentName, Range, FromRange,ToRange , Rate,NoOfTransaction , CreatedDate, CreatedBy , RangeList";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Curreny")]
        public string SendingCurrency { get; set; }

        [Required(ErrorMessage = "Select Curreny")]
        public string ReceivingCurrency { get; set; }
        public TransactionTransferType TransactionTransferType { get; set; }
        public string TransactionTransferTypeName { get; set; }
        public TransactionTransferMethod TransactionTransferMethod { get; set; }
        public string TransactionTransferMethodName { get; set; }
        public int? AgentId { get; set; }
        public string AgentName { get; set; }

        [Required(ErrorMessage = "Select Range")]
        public string Range { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }

        [Required(ErrorMessage = "Enter Rate")]
        public decimal Rate { get; set; }
        public int NoOfTransaction { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public List<string> RangeList { get; set; }

        public string SendingCountryFlag { get; set; }

        public string ReceivingCountryFlag { get; set; }
    }
}