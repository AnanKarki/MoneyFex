using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class TopUpSomeoneElsePaymentDetialsViewModel
    {

        public int Id { get; set; }

        public int MFTCCardId { get; set; }

        public string CardUserName { get; set; }

        public string MFTCCardNumber { get; set; }

        public string CardUserCountry { get; set; }

        public string CardUserCity { get; set; }

        public string TopUpAmount { get; set; }

        public string TopUpReference { get; set; }

        public DateTime Date { get; set; }

        public string Time { get; set; }

        public string PaymentMethod { get; set; }


    }
}