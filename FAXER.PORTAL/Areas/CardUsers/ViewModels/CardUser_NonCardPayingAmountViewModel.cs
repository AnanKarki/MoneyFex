using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class CardUser_NonCardPayingAmountViewModel
    {
        public const string BindProperty = " ReceivingCountry,TopUpAmount , ReceivingAmount, IncludingFee";

        public string ReceivingCountry { get; set; }
        public decimal TopUpAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public bool IncludingFee { get; set; }

    }
}