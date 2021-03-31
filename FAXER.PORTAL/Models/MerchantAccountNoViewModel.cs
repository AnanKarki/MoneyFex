using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class MerchantAccountNoViewModel
    {
        public int Id { get; set; }
        public string BusinessName { get; set; }
        public string BusinessMobileNo { get; set; }
 

    }

    public class PreviousPayeeViewModel {

        public const string BindProperty = "BusinessMFCode,Confirm";

        public string BusinessMFCode { get; set; }

        public bool Confirm { get; set; }


    }
}