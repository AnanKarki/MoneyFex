using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class CalculateFaxingFeeViewModel
    {       

        [Required]
        [Display(Name = "Enter Top-up Amount")]
        public double TopUpAmount { get; set; }


        [Required]
        [Display(Name = "Top-up Fees")]
        public double TopUpFees { get; set; }

        [Required]
        [Display(Name = "Total Amount Including Fees")]
        public double AmountWithFee { get; set; }

        [Required]
        [Display(Name = "Current Exchange Rate")]
        public double ExchangeRate { get; set; }

        [Required]
        [Display(Name = "Amount To Be Avaialbe On Money Fax Top-up Card")]
        public double TopUpCardMoney { get; set; }
    }
}