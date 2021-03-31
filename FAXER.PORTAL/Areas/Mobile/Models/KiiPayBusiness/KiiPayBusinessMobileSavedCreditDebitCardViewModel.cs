using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness
{
    public class KiiPayBusinessMobileSavedCreditDebitCardViewModel
    {
        public const string BindProperty = "CardId ,CardNo ,Address ,CardStatus , CVVCode , ExpMonth" +
            ", ExpDate , ExpYear ,KiiPayBusinessId ,CardType";

        public int CardId { get; set; }
        public string CardNo { get; set; }
        public string Address { get; set; }
        public string CardStatus { get; set; }
        public string CVVCode { get; set; }
        public string ExpMonth { get; set; }
        public string ExpDate { get; set; }
        public string ExpYear { get; set; }
        public int KiiPayBusinessId { get; set; }
        public CreditDebitCardType CardType { get; set; }
    }
}