using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class NoCardPaymentTransactionSummaryViewModel
    {
        public const string BindProperty = " ReceiverName ,ReceiverId ,ReceiveOption , MerchantName,MerchantPhoneNumber , MerchantEmail , CountryOfBirth ,SenderMFTCCardNumber" +
            " , CardExpriyDate ,streetAddress , City, State,PostalCode ,SentAmount ,Fees ,   TotalAmount ,TotalReceiveAmount  , PaymentReference, SendSms ";

        #region Receiver Information 
        public string ReceiverName { get; set; }
        public string  ReceiverId{ get; set; }

        public string ReceiveOption { get; set; }
        #endregion

        #region Personal Information 

        
        public string MerchantName { get; set; }

        public string MerchantPhoneNumber { get; set; }

        public string MerchantEmail { get; set; }
        public string CountryOfBirth { get; set; }
        public string SenderMFTCCardNumber { get; set; }


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


        public bool SendSms { get; set; }
    }
}