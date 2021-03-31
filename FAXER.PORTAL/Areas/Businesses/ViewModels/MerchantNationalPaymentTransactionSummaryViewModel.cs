using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class MerchantNationalPaymentTransactionSummaryViewModel
    {
        public const string BindProperty = "ReceiverCardId , ReceiverKiiPayBusinessInformationId,ReceiverName ,ReceiverAccountNo ,ReceiveOption ," +
            "MerchantCardID ,KiiPayBusinessInformationId ,MerchantName ,MerchantPhoneNumber , MerchantMFBCCardNumber,MerchantEmail ,CountryOfBirth ," +
            " CardExpriyDate , streetAddress,City , State, PostalCode, TotalAmount ,SendSms ";


        #region Card User Information 
        public int ReceiverCardId { get; set; }
        public int ReceiverKiiPayBusinessInformationId{ get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAccountNo { get; set; }

        public string ReceiveOption { get; set; }
        #endregion

        #region Personal Information 

        public int MerchantCardID { get; set; }

        public int KiiPayBusinessInformationId { get; set; }
        public string MerchantName { get; set; }

        public string MerchantPhoneNumber { get; set; }

        public string MerchantMFBCCardNumber { get; set; }
        public string MerchantEmail { get; set; }
        public string CountryOfBirth { get; set; }


        #endregion
        #region Payment Information 
        

        public string CardExpriyDate { get; set; }
        public string streetAddress { get; set; }
        public string City { get; set; }

        public string State { get; set; }

        public string PostalCode { get; set; }
        #endregion

        #region Transaction Details 
        
        public string TotalAmount { get; set; }


        #endregion

        public bool SendSms { get; set; }
    }
}