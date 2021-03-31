using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class KiiPayPersonalSavedDebitCreditCard
    {

        public const string BindProperty = "DepositingAmount , KiiPayPersonalSavedDebitCreditCardVM";
        public decimal DepositingAmount { get; set; }

        public List<KiiPayPersonalSavedDebitCreditCardVM> KiiPayPersonalSavedDebitCreditCardVM { get; set; }
    }

    public class KiiPayPersonalSavedDebitCreditCardVM
    {
        public int CardId { get; set; }
        public string Name { get; set; }
        public string CardNumber { get; set; }

        public bool IsChecked { get; set; }
    }
}