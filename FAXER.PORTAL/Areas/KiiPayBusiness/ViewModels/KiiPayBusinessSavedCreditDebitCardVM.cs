using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessSavedCreditDebitCardListVM
    {
        public int CardId { get; set; }
        public string CardNo { get; set; }
        public string FullCardNo { get; set; }
        public string CardStatus { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string CVVCode { get; set; }

    }
   
   

}