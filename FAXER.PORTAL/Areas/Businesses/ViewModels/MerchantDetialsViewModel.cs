using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MerchantDetialsViewModel
    {
        public const string BindProperty = " KiiPayBusinessInformationId,MFBCCardID ,MerchantName ,MerchantAccountNo ,City ,Country , Phone,Email , Website ,confirm";
        public int KiiPayBusinessInformationId { get; set; }
        public int MFBCCardID { get; set; }
        public string MerchantName { get; set; }

        public string MerchantAccountNo { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }


        public bool confirm { get; set; }
    }

    public class PreviousPayeeViewModel {

        public const string BindProperty = "BusinessMFCode , Confirm";
        public string BusinessMFCode { get; set; }

        public bool Confirm { get; set; }
    }
}