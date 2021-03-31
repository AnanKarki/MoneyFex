using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MFBCCardProfileViewModel
    {
        public int CardId { get; set; }
        public string MFBCCardNumber {   get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string CardStatus { get; set; }
        public decimal AmountOnCard { get; set; }
        public string EmailAddres { get; set; }
        public string UserIdentificationURL { get; set; }
        public string Currency { get; set; }
        public string CurrencySymbol { get; set; }
    }
}