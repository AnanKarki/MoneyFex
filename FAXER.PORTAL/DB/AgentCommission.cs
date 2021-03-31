using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class AgentCommission
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public int AgentId { get; set; }
        public string City { get; set; }
        public Nullable<decimal> SendingRate { get; set; }
        public Nullable<decimal> ReceivingRate { get; set; }
        public TransferService TransferSevice { get; set; }
        public CommissionType CommissionType { get; set; }
        public CommissionDueDate CommissionDueDate { get; set; }
    }

    public enum TransferService
    {
        [Display(Name = "Select Services")]
        Select = 0 ,
        All = 6,
        [Display(Name = "KiiPay Wallet")]
        KiiPayWallet = 1,
        [Display(Name = "Bank Deposit")]
        BankDeposit = 2,
        [Display(Name = "Cash Pickup")]
        CahPickUp = 3,
        [Display(Name = "Other Wallet")]
        OtherWallet = 4,
        [Display(Name = "Pay Bills")]
        PayBills = 5,
        
    }
    public enum CommissionType
    {
        [Display(Name = "Select Type")]
        Select,
        [Display(Name = "Commission On Fee")]
        CommissionOnFee,
        [Display(Name = "Commission On Amount")]
        CommissionOnAmount,
        [Display(Name = "Flat Fee")]
        FlatFee
    }

    public enum CommissionDueDate
    {
        [Display(Name ="Select Date of the Month")]
        Select,

        [Display(Name = "Every 1st")]
        EveryFirst,

        [Display(Name = "Every 2nd")]
        EverySecond,

        [Display(Name = "Every 3rd")]
        EveryThird,

        [Display(Name = "Every 4th")]
        EveryFourth,

        [Display(Name = "Every 5th")]
        EveryFifth,

        [Display(Name = "Every 6th")]
        EverySixth,

        [Display(Name = "Every 7th")]
        EverySeventh,

        [Display(Name = "Every 8th")]
        EveryEighth,

        [Display(Name = "Every 9th")]
        EveryNinth,

        [Display(Name = "Every 10th")]
        EveryTenth,

        [Display(Name = "Every 11th")]
        EveryEleventh,

        [Display(Name = "Every 12th")]
        EveryTwelveth,

        [Display(Name = "Every 13th")]
        EveryThirteenth,

        [Display(Name = "Every 14th")]
        EveryFourteenth,

        [Display(Name = "Every 15th")]
        EveryFifteenth,

        [Display(Name = "Every 16th")]
        EverySixteenth,

        [Display(Name = "Every 17th")]
        EverySeventeenth,

        [Display(Name = "Every 18th")]
        EveryEighteenth,

        [Display(Name = "Every 19th")]
        Everynineteenth,

        [Display(Name = "Every 20th")]
        EveryTwentieth,

        [Display(Name = "Every 21st")]
        EveryTwentyfirst,

        [Display(Name = "Every 22nd")]
        EveryTwentySecond,

        [Display(Name = "Every 23rd")]
        EveryTwentyThird,

        [Display(Name = "Every 24th")]
        EveryTwentyFourth,

        [Display(Name = "Every 25th")]
        EveryTwentyFifth,

        [Display(Name = "Every 26th")]
        EveryTwentySixth,

        [Display(Name = "Every 27th")]
        EveryTwentySeventh,

        [Display(Name = "Every 28th")]
        EveryTwentyEighth,

        [Display(Name = "Every 29th")]
        EveryTwentyNinth,

        [Display(Name = "Every 30th")]
        EveryThirtieth,

        [Display(Name = "Every 31st")]
        EveryThirthyFirst
    }
}




