using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class SavedCreditDebitCardViewModel
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string Status { get; set; }
    }
}