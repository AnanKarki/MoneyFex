using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class CardUser_MerchantInternationalPaymentTransactionSummaryViewModel
    {
        public const string BindProperty = " ReceiverBusinessname,ReceiverAccountNO , ReceiveOption, CardUserName  , CardUserPhoneNumber ,CardUserEmail ,CardUserCountry , SenderMFBCCardNumber " +
            ", CardExpriyDate, streetAddress ,City , State, PostalCode , SentAmount , Fees , TotalAmount , TotalReceiveAmount, PaymentReference";
        #region Receiver Information 
        public string ReceiverBusinessname { get; set; }
        public string ReceiverAccountNO { get; set; }
        public string ReceiveOption { get; set; }
        #endregion

        #region Personal Information 
        public string CardUserName { get; set; }
        public string CardUserPhoneNumber { get; set; }
        public string CardUserEmail { get; set; }
        public string CardUserCountry { get; set; }
        public string SenderMFBCCardNumber { get; set; }

        #endregion
        #region Payment Information 
        public string CardExpriyDate { get; set; }
        public string streetAddress { get; set; }
        public string City { get; set; }
        public string State { get; set; }

        public string PostalCode { get; set; }
        #endregion

        #region Transaction Details 

        public string SentAmount { get; set; }

        public string Fees { get; set; }

        public string TotalAmount { get; set; }

        public string TotalReceiveAmount { get; set; }

        public string PaymentReference { get; set; }

        #endregion
    }
}