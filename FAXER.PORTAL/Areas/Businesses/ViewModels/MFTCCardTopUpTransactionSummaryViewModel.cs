using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MFTCCardTopUpTransactionSummaryViewModel
    {
        public const string BindProperty = "CardUserId , CardUsername , MFTCCardNumber, ReceiveOption, MerchantCardID, KiiPayBusinessInformationId, MerchantName, MerchantPhoneNumber," +
                                              " MerchantEmail ,CountryOfBirth ,MerchantMFBCCardNumber,SavedCardId,CardNumber,CardExpriyDate,streetAddress ,City , State," +
                                              " PostalCode ,SentAmount ,Fees ,TotalAmount ,TotalReceiveAmount ,SendSms";

        #region Card User Information 
        public int CardUserId { get; set; }
        public string CardUsername { get; set; }
        public string MFTCCardNumber { get; set; }

        public string ReceiveOption { get; set; }
        #endregion

        #region Personal Information 

        public int MerchantCardID { get; set; }

        public int KiiPayBusinessInformationId { get; set; }
        public string MerchantName { get; set; }

        public string MerchantPhoneNumber { get; set; }

        public string MerchantEmail { get; set; }
        public string CountryOfBirth { get; set; }

        public string MerchantMFBCCardNumber { get; set; }


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

        public bool SendSms { get; set; }
    }
}