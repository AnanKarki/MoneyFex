using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class NonCardTransactionSummaryViewModel
    {

        public const string BindProperty = "ReceiverId,ReceiverName,ReceiveOption,FaxerName," +
          "FaxerPhoneNumber,FaxerEmail,CountryOfBirth,SavedCardId,CardNumber,CardExpriyDate,streetAddress,City ," +
          "State,PostalCode,SentAmount,Fees,TotalAmount,TotalReceiveAmount";


        #region Receiver Information 
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }

        public string ReceiveOption { get; set; }
        #endregion

        #region Personal Information 

        public string FaxerName { get; set; }

        public string FaxerPhoneNumber { get; set; }

        public string FaxerEmail { get; set; }
        public string CountryOfBirth { get; set; }

        #endregion
        #region Payment Information 
        public int SavedcardId { get; set; }
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