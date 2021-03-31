using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPersonalKiiPayPersonalStandingOrderInfo
    {
        public int Id { get; set; }
        /// <summary>
        /// KiiPay personal Info Id (sender )
        /// </summary>
        public int SenderId { get; set; }

        /// <summary>
        /// Kiipay personal wallet Id (receiver)
        /// </summary>
        public int ReceiverId { get; set; }

        public string ReceiverMobileNo { get; set; }
        public string SenderCountry { get; set; }
        public string ReceiverCountry { get; set; }
        public decimal Amount { get; set; }
        public AutoPaymentFrequency PaymentFrequency { get; set; }
        public string FrequencyDetials { get; set; }
        public DateTime CreationDate { get; set; }

        public virtual KiiPayPersonalWalletInformation Receiver { get; set; }
    }
}