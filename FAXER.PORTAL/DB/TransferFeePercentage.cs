using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class TransferFeePercentage
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
        public int AgetnId { get; set; }
        public int TransfeFeeByCurrencyId { get; set; }
        public string SendingCurrency { get; set; }
        public  string ReceivingCurrency { get; set; }
    }

    public enum FeeType
    {
        [Display(Name = "Select Fee Type")]
        Select,
        [Display(Name = "Percent")]
        [Description("Percent")]
        Percent,
        [Display(Name = "Flat Fee")]
        [Description("Flat Fee")]
        FlatFee,
    }

    public enum TranfserFeeRange
    {
        [Display(Name = "Select Range")]
        Select = 0,

        All = 9,
        [Display(Name = "1-100")]
        [Description("1-100")]
        OneToHundred = 1,

        [Display(Name = "101-500")]
        [Description("101-500")]
        HundredOnetoFiveHundred = 2,

        [Display(Name = "501-1000")]
        [Description("501-1000")]
        FivehundredOneToThousand = 3,

        [Display(Name = "1001-1500")]
        [Description("1001-1500")]
        ThousandOneToFifteenHundred = 4,

        [Display(Name = "1501-2000")]
        [Description("1501-2000")]
        FifteenHundredOneToTwoThousand = 5,

        [Display(Name = "2001-3000")]
        [Description("2001-3000")]
        TwothousandOneToThreeThousand = 6,

        [Display(Name = "3001-5000")]
        [Description("3001-5000")]
        ThreeTHousandOneToFiveThousand = 7,

        [Display(Name = "5001-10000")]
        [Description("5001-10000")]
        FivethousandOneToTenThousand = 8,

        [Display(Name = "11000+")]
        [Description("11000+")]
        AboveTenThousand = 10,



    }

}