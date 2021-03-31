using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class IntroductoryFeeByCurrencyViewModel
    {
        public const string BindProperty = "Id , SendingCurrency ,ReceivingCurrency ,TransferType ,TransferMethod , " +
            "FromRange,ToRange ,Range ,OtherRange ,FeeType , Fee, CreatedDate ,NumberOfTransaction , AgentId, RangeList ,Percentage , FlatFee";


        public int Id { get; set; }
        [Required(ErrorMessage ="Select Currency")]
        public string SendingCurrency { get; set; }

        [Required(ErrorMessage = "Select Currency")]
        public string ReceivingCurrency { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }

        [Required(ErrorMessage = "Select Range")]
        public string Range { get; set; }
        public string OtherRange { get; set; }
        public FeeType FeeType { get; set; }
        [Required(ErrorMessage = "Enter Fee")]
        public decimal Fee { get; set; }
        public DateTime CreatedDate { get; set; }
        public NumberOfTransaction NumberOfTransaction { get; set; }
        public int? AgentId { get; set; }
        public List<string> RangeList { get; set; }

        public decimal Percentage { get; set; }
        public decimal FlatFee { get; set; }
        public string SendingCountryFlag { get;  set; }
        public string ReceivingCountryFlag { get;  set; }
    }
}