using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class MerchantDetailsViewModel
    {
        public const string BindProperty = "Id,BusinessName,PhoneCode,Telephone" +
        ", Email, Website, NameOfMerchant, MoneyFaxAccountNumber, RegisteredCity, RegisteredCountry, Confirm";
        public int Id { get; set; }
        public string BusinessName { get; set; }

        public string PhoneCode { get; set; }

        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }

        public string NameOfMerchant { get; set; }
        public string MoneyFaxAccountNumber { get; set; }
        public string RegisteredCity { get; set; }
        public string RegisteredCountry { get; set; }
        public bool Confirm { get; set; }
    }
}