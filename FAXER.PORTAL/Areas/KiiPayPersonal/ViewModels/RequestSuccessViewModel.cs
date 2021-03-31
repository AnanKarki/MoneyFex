using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class RequestSuccessViewModel
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string ReceiverName { get; set; }
     
    }
}