using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderSavedCreditDebitCardViewModel
    {
        public int Id { get; set; }
        public CreditDebitCardType CardType { get; set; }

        public string CardNo { get; set; }
        public string CardNoWithoutFormat { get; set; }
        public string EndDate { get; set; }
        public string EndMonth { get; set; }
        public string EndYear { get; set; }
        public string SecurityCode { get; set; }
        public string SecurityCodeUnmask { get; set; }

        public string Status { get; set; }
    }
}