using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class LoggedKiiPayUserViewModel
    {

        public int id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string FullName { get; set ; }
        public string Country { get; set; }

        public string Email { get; set; }

        public int KiiPayPersonalId { get; set; }
        public int KiiPayPersonalWalletId { get; set; }

        public decimal BalanceOnCard { get; set; }

        public string CardUserCurrency { get; set; }
        public string CardUserCurrencySymbol { get; set; }
        public string MobileNumber { get; set; }
        public string CountryCode { get; set; }


    }
}