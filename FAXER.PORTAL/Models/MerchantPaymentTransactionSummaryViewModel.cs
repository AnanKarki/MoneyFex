using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class MerchantPaymentTransactionSummaryViewModel
    {
        public const string BindProperty = "BusinessMerchantName,BusinessMFCode,ReceiveOption,FaxerName," +
        "FaxerPhoneNumber,FaxerEmail,CountryOfBirth,SavedCardId,CardNumber,CardExpriyDate,streetAddress,City ," +
        "State,PostalCode,SentAmount,Fees,TotalAmount,TotalReceiveAmount,PaymentReference";

        #region Business Merhcant Information 
        public string BusinessMerchantName { get; set; }

        public string BusinessMFCode { get; set; }
        public string ReceiveOption { get; set; }
        #endregion

        #region Personal Information 

        public string FaxerName { get; set; }

        public string FaxerPhoneNumber { get; set; }

        public string FaxerEmail { get; set; }
        public string CountryOfBirth { get; set; }

        #endregion
        #region Payment Information 
        public int SavedCardId { get; set; }
        public string CardNumber { get; set; }

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