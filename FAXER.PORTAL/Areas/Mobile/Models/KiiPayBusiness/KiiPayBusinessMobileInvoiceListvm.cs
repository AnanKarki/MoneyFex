using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness
{
    public class KiiPayBusinessMobileInvoiceListvm 
    {
        //public ListGroupByVm ListGroupByVm { get; set; }

        public string InvoiceDate { get; set; }
        public List<InvoiceMasterListvm> PayingInvoiceListvm { get; set; }
    }
   
    public class ListGroupByVm
    {
        public string InvoiceDate { get; set; }
    }
}