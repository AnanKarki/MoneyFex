using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class CardUser_MerchantNationalPaymentTransactionSummaryViewModel
    {

        public const string BindProperty = " ReceiverCardId,ReceiverKiiPayBusinessInformationId , ReceiverName,ReceiverAccountNo, ReceiveOption, CardUserName ,CardUserEmail ,CardUserPhoneNumber" +
            " ,CardUserCountry ,SenderMFTCCardNumber ,CardExpriyDate ,streetAddress , City,State ,PostalCode ,TotalAmount ,SendSms";
        #region Card User Information 
        public int ReceiverCardId { get; set; }
        public int ReceiverKiiPayBusinessInformationId { get; set; }
        public string ReceiverName { get; set; }
        public string ReceiverAccountNo { get; set; }

        public string ReceiveOption { get; set; }
        #endregion

        #region Personal Information 
        public string CardUserName { get; set; }
        public string CardUserEmail { get; set; }
        public string CardUserPhoneNumber { get; set; }
        public string CardUserCountry { get; set; }

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

        public string TotalAmount { get; set; }


        #endregion


        public bool SendSms { get; set; }
    }
}