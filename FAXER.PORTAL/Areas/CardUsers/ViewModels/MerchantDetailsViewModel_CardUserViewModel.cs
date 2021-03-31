using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class MerchantDetailsViewModel_CardUserViewModel
    {
        public const string BindProperty = " KiiPayBusinessInformationId , MFBCCardID ,MerchantName ,MerchantAccountNo , City ,Country  ,confirm , Address, PhoneNo ,Email , Website, CountryPhoneCode ";
        public int KiiPayBusinessInformationId { get; set; }
        public int MFBCCardID { get; set; }
        public string MerchantName { get; set; }

        public string MerchantAccountNo { get; set; }
        public string City { get; set; }
        public string Country { get; set; }

        public bool confirm { get; set; }
        public string Address { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public string CountryPhoneCode { get; set; }
    }
}