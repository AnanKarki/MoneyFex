using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class TransferOperationSummaryViewModel
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string SendingCountryFlag { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryFlag { get; set; }
        public string CountryCurrency { get; set; }
        public string Month { get; set; }
        public int NoOfTrans { get; set; }
        public decimal Amount { get; set; }
        public decimal Fees { get; set; }
        public decimal Spread { get; set; }
        public decimal Refund { get; set; }
        public decimal NoOfInProgress { get; set; }

        #region  Agent and partner
        public string Name { get; set; }
        public string  AccountNo{ get; set; }
        public decimal Commission { get; set; }
        #endregion

    };
}