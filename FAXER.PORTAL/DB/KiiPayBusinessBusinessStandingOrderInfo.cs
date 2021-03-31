using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    /// <summary>
    /// kiiPay Business Auto Payments Information 
    /// </summary>
    public class KiiPayBusinessBusinessStandingOrderInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// KiiPay Business Info Id (Sender )
        /// </summary>
        public int SenderId { get; set; }
        /// <summary>
        /// KiiPay Business Info Id (Receiver)
        /// </summary>
        public int ReceiverId { get; set; }
        public string ReceiverMobileNo { get; set; }
        public decimal Amount { get; set; }
        public AutoPaymentFrequency Frequency { get; set; }
        public string FrequencyDetail { get; set; }

        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }

        public DateTime CreationDate { get; set; }

        public KiiPayBusinessInformation Receiver { get; set; }

    }
}