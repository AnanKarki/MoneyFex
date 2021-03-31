using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class MoneyFaxCardAutoTopUpViewModel
    {
        public int Id { get; set; }
        public string MFTCCardNo { get; set; }
        public string NameonMFTCCard { get; set; }
        public decimal AutoAmount { get; set; }
        public string AutoTopUp { get; set; }
    }

    public class SomeoneElseMoneyFexCardAutoTopUpViewModel {
        public int Id { get; set; }
        public int MFTCCardId { get; set; }
        public string MFTCCardNo { get; set; }
        public string NameonMFTCCard { get; set; }
        public decimal AutoAmount { get; set; }
        public string AutoTopUp { get; set; }

        public string PaymentFrequency { get; set; }

        public DB.AutoPaymentFrequency Frequency { get; set; }
        public string FrequencyDetails { get; set; }

    }

    public class AutoTopUpMFTCCardList {

        public int Id { get; set; }

        public string FullName { get; set; }
    }
}