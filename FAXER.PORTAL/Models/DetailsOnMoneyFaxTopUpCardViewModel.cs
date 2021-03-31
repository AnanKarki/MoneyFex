using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    /// <summary>
    /// Details on Virtual Account ViewModel
    /// </summary>
    public class DetailsOnMoneyFaxTopUpCardViewModel
    {
        public int Id { get; set; }
        public string CardNumber { get; set; }

        public string NameOnMoneyFaxTopUpCard { get; set; }
        public string CountryOnMoneyFaxTopUpCard { get; set; }
        public string CityOnMoneyFaxTopUpCard { get; set; }
        public string TelephoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public string StatusOfMoneyFaxTopUpCard { get; set; }
        public string AmountOnMoneyFaxTopUpCard { get; set; }
        public string CardPhoto { get; set; }
        

    }
}