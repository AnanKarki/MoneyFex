using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessPayBillsReferenceOneVM
    {
        public const string BindProperty = " ReferanceNo0, ReferanceNo1 ,ReferanceNo2 ,BillNo ,Amount";

        public string ReferanceNo0 { get; set; }
        public string ReferanceNo1 { get; set; }
        public string ReferanceNo2 { get; set; }
        public string BillNo { get; set; }
        public decimal   Amount { get; set; }
    }

    public class KiiPayBusinessInternationalPayBillsReferenceOneVM : KiiPayBusinessPayBillsReferenceOneVM
    {
        public new const string BindProperty = " ReferanceNo0, ReferanceNo1 ,ReferanceNo2 ,BillNo ,Amount ,Fee  , TotalAmount";

        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
    }
}