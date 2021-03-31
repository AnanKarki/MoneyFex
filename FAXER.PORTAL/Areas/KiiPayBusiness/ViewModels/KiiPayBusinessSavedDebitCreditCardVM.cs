using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessSavedDebitCreditCardVM
    {
        public const string BindProperty = " CardId, Name ,CardNumber , IsChecked ";

        public int CardId { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }

        public bool IsChecked { get; set; }


    }
    public class KiiPayBusinessSavedDebitCreditCard {

        public const string BindProperty = " DepositingAmount,KiiPayBusinessSavedDebitCreditCardVM ";
        public decimal DepositingAmount { get; set; }
            
        public List<KiiPayBusinessSavedDebitCreditCardVM> KiiPayBusinessSavedDebitCreditCardVM { get; set; }

    }

}