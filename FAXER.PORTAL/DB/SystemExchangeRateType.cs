using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class SystemExchangeRateType
    {
        public int Id { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public TransactionTransferMethod TransferMethod{ get; set; }
        public ExchangeRateType ExchangeRateType { get; set; }

    }
    public enum ExchangeRateType
    {

        [Description("Current ExchangeRate")]
        [Display(Name = "Current ExchangeRate")]
        CurrentExchangeRate,
        [Description("Transaction ExchangeRate")]
        [Display(Name = "Transaction ExchangeRate")]
        TransactionExchangeRate

    }
}