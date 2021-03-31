using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class TopUpAmountViewModel
    {
        public const string BindProperty = " TopUpAmount ,ReceivingAmount , IncludingFee ,PaymentReference"; 
        public decimal TopUpAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public bool IncludingFee { get; set; }

        public string PaymentReference { get; set; }
        
    }

    public class NonCardPaymentViewModel {

        public const string BindProperty = " ReceivingCountry ,TopUpAmount , ReceivingAmount ,IncludingFee";
        public string ReceivingCountry { get; set; }
        public decimal TopUpAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public bool IncludingFee { get; set; }


    }


}