using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class TransactionUpdateVm
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string MobileNo { get; set; }
        public string ReceiverName { get; set; }
    }
}