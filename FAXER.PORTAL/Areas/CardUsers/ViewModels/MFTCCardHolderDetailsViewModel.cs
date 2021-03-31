using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class MFTCCardHolderDetailsViewModel
    {
        public const string BindProperty = " MFTCCardId , CardUserName, CardUserMFTCCardNumber, CardUserCity ,CardUserCountry ,Confirm ,Address ,CountryPhoneCode , Phone ,Email";

        public int MFTCCardId { get; set; }

        public string CardUserName { get; set; }
        public string CardUserMFTCCardNumber { get; set; }
        public string CardUserCity { get; set; }
        public string CardUserCountry { get; set; }
        public bool Confirm { get; set; }
        public string Address { get; set; }
        public string CountryPhoneCode { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }

    }
}