using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class TrackAFaxViewModel
    {
        public int Id { get; set; }
        
        public string ReceiverName { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverCountryCode { get; set; }
        public string ReceiverCountry { get; set; }
        public string FaxedAmount { get; set; }
        public string FaxedDate { get; set; }
        public string FaxedTime { get; set; }
        public string MoneyFaxControlNumber { get; set; }
        

        public DB.OperatingUserType OperatingUserType { get; set; }
        public FaxingStatus StatusOfFax { get; set; }

        public string Faxingstatus { get; set; }
    }

    public class SenderCashPickUpDetialVM {

        public int Id { get; set; }

        public string ReceiverName { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverCountryCode { get; set; }
        public string ReceiverCountry { get; set; }

        public string ReceiverCurrencyCode { get; set; }
        public string ReceiverCurrencySymbol { get; set; }

        public string SendingCurrencyCode { get; set; }
        public string SendingCurrencySymbol { get; set; }

        public string SentAmount { get; set; }

        public string AmountPaid { get; set; }
        public string Fee { get; set; }
        public string ReceivingAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public string PaymentMethod { get; set; }



        public string SentDate { get; set; }
        public string SentTime { get; set; }
        public string MFCN { get; set; }

        public DB.OperatingUserType OperatingUserType { get; set; }
        public FaxingStatus StatusOfFax { get; set; }

        public string Faxingstatus { get; set; }
    }


}
