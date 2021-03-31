using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    /// <summary>
    /// KiiPay Personal Standing Order payment of KiiPay Businesss user
    /// </summary>
    public class KiiPayBusinessKiiPayPersonalStandingOrderInfo
    {

        public int Id { get; set; }
        /// <summary>
        /// KiiPay Business Info Id (sender )
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

        public virtual KiiPayBusinessInformation Sender { get; set; }
        public virtual KiiPayPersonalWalletInformation Receiver { get; set; }




    }
}