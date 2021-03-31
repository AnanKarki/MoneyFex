using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MobilePaymentSummary
{
    public class MobilePaymentPaymentMethodVm
    {


        public SenderPaymentMode PaymentMode { get; set; }

        public MobilePaymentCreditDebitCardDetailVm CreditDebitCardDetail { get; set; }

        public MobilePaymentMoneyFexBankAccountPaymentvm BankPayment { get; set; }


    }

    public class MobilePaymentCreditDebitCardDetailVm
    {

        public string CardHolderName { get; set; }
        public string CardNumber { get; set; }
        public string EndMonth { get; set; }
        public string EndYear { get; set; }
        public string SecurityCode { get; set; }
        public string Fee { get; set; }
        public bool SaveCard { get; set; }
    }


    public class MobilePaymentMoneyFexBankAccountPaymentvm {

        public string Amount { get; set; }
        public decimal BankFee { get; set; }
        public string Reference { get; set; }

    }

}