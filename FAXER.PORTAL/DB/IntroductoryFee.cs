using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class IntroductoryFee
    {
        public int Id { get; set; }
        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public TranfserFeeRange Range { get; set; }
        public string OtherRange { get; set; }
        public FeeType FeeType { get; set; }
        public decimal Fee { get; set; }
        public DateTime CreatedDate { get; set; }
        public NumberOfTransaction NumberOfTransaction { get; set; }
        public int AgentId { get; set; }
        public int IntroductoryFeeByCurrencyId { get; set; }
    }

    public enum NumberOfTransaction
    {
        [Display(Name = "Select Number")]
        select,
        [Display(Name = "1")]
        One,
        [Display(Name = "2")]
        Two,
        [Display(Name = "3")]
        Three,
        [Display(Name = "4")]
        Four,
        [Display(Name = "5")]
        Five
    }
}