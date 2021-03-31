using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class IntroductoryFeeHistoryViewModel
    {

        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string SendingCountryFlag { get; set; }
        public string ReceivingCountry { get; set; }
        public string ReceivingCountryFlag { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public TranfserFeeRange Range { get; set; }
        public string OtherRange { get; set; }
        public FeeType FeeType { get; set; }
        public decimal Fee { get; set; }
        public string CreatedDate { get; set; }
        public string RangeName { get; set; }
        public decimal FlatFee { get; set; }
        public decimal Percentage { get; set; }
        public NumberOfTransaction NumberOfTransaction { get; set; }

    }
}