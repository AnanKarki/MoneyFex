using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SystemExchangeRateViewModel
    {
        public const string BindProperty = "Id ,SendingCurrency ,ReceivingCurrency , TransferMethodName, TransferMethod,ExchangeRateType, IsCurrentExchangeRate,IsTransactionExchangeRate  ";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Currency")]
        public string SendingCurrency { get; set; }
        [Required(ErrorMessage = "Select Currency")]
        public string ReceivingCurrency { get; set; }
        public string TransferMethodName { get; set; }
        [Required(ErrorMessage = "Select Method")]
        public TransactionTransferMethod TransferMethod { get; set; }
        public ExchangeRateType ExchangeRateType { get; set; }
        public bool IsCurrentExchangeRate { get; set; }
        public bool IsTransactionExchangeRate { get; set; }

    }
}