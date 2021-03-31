using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class MFTCCardTransactionSummaryViewModel
    {
        public const string BindProperty = "CardUserId,CardUsername,MFTCCardNumber,ReceiveOption,FaxerName," +
            "FaxerPhoneNumber,FaxerEmail,CountryOfBirth,SavedCardId,CardNumber,CardExpriyDate,streetAddress,City ,"+
            "State,PostalCode,SentAmount,Fees,TotalAmount,TotalReceiveAmount";

        #region Card User Information 
        public int CardUserId { get; set; }
        public string CardUsername { get; set; }
        public string MFTCCardNumber { get; set; }

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

        #endregion
    }
}