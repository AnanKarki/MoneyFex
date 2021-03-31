using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class TopUpSomeoneElseCardViewModel
    {
        public const string BindProperty = "Id,NameOfCardUser,TopUpCardNumber,RegisteredCity,RegisteredCountry,RegisteredCountryCode,PhoneCode" +
            ",Telephone,Email,Website,Confirm";
        public int Id { get; set; }
        public string NameOfCardUser { get; set; }
        public string TopUpCardNumber { get; set; }
        public string RegisteredCity { get; set; }
        public string RegisteredCountry { get; set; }

        public string RegisteredCountryCode { get; set; }
        public string PhoneCode { get; set; }

        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }

        public bool Confirm { get; set; }
    }
}