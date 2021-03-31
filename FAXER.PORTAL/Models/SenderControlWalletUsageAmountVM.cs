using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderControlWalletUsageAmountVM
    {

        public const string BindProperty = "WalletId,PreviousAmount,CurrencySymbol," +
            "Amount,Cent,NewLimit,Currency,LimitFrequencyId,GoodFrequencyId," +
            "LimitTypeId,AvailableBal";

        [Range(0 , int.MaxValue)]
        public int WalletId { get; set; }


        [Range(0.0, double.MaxValue)]
        public decimal PreviousAmount { get; set; }
        [MaxLength(20)]
        public string CurrencySymbol { get; set; }


        [Range(0.0, double.MaxValue)]
        public decimal Amount { get; set; }


        [Range(0.0, double.MaxValue)]
        public decimal Cent { get; set; }
      
        [Range(0.0, double.MaxValue, ErrorMessage = "Enter  amount")]
        public decimal NewLimit { get; set; }
        [MaxLength(20)]
        public string Currency { get; set; }

        [Range(0, int.MaxValue)]
        public CardLimitType LimitFrequencyId { get; set; }

        [Range(0, int.MaxValue)]
        public AutoPaymentFrequency GoodFrequencyId { get; set; }
        [Range(0, int.MaxValue)]
        public int LimitTypeId { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal AvailableBal { get; set; }




    }
}