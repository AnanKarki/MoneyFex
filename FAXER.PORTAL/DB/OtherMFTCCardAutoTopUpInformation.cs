using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class OtherMFTCCardAutoTopUpInformation
    {

        public int Id { get; set; }

        public int FaxerId { get; set; }

        public int MFTCCardId { get; set; }
        public bool EnableAutoPayment { get; set; }

        public AutoPaymentFrequency AutoPaymentFrequency { get; set; }

        public string FrequencyDetails { get; set; }
        public decimal AutoPaymentAmount { get; set; }

        public string TopUpReference { get; set; }

        public virtual KiiPayPersonalWalletInformation MFTCCard { get; set; }

    }
}