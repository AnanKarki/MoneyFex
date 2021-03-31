using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewRegisteredMFTCViewModel
    {
        public int Id { get; set; }
        public int FaxerId { get; set; }
        public string MFTCCardNumber { get; set; }

        public string CardUserFirstName { get; set; }
        public string CardUserMiddleName { get; set; }
        public string CardUserLastName { get; set; }
        public DateTime CardUserDOB { get; set; }
        public string CardUserAddress1 { get; set; }
        public string CardUserAddress2 { get; set; }
        public string CardUserCountry { get; set; }
        public string CardUserCity { get; set; }
        public string CardUserTelephone { get; set; }
        public string CardUserEmail { get; set; }
        public string CardUserPhoto { get; set; }
        public string CardPhoto { get; set; }
        public string CardUsageStatus { get; set; }

        public bool TempSMS { get; set; }
        public decimal AmountOnCard { get; set; }
        public string Currency { get; set; }
        public bool AutoTopUp { get; set; }
        public decimal CashWithDrawalLimit { get; set; }
        public string CashWithDrawalLimitType { get; set; }
        public decimal GoodsPurchaseLimit { get; set; }
        public string GoodsPurchaseLimitType { get; set; }

    }
}