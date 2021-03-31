using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayBusinessECardInfo
    {

        
            public int Id { get; set; }
            [ForeignKey("KiiPayBusinessInformation")]
            public int KiiPayBusinessInformationId { get; set; }

            #region Personal Information 
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastName { get; set; }
            public System.DateTime DOB { get; set; }
            public Gender Gender { get; set; }
            #endregion

            #region Address


            public string AddressLine1 { get; set; }
            public string AddressLine2 { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            #endregion

            #region KiiPay Account Info

            /// <summary>
            /// Mobile No will be used as their card no
            /// </summary>
            public string MobileNo { get; set; }

            public decimal CurrentBalance { get; set; }
            public decimal CashWithdrawalLimit { get; set; }
            public string CashLimitType { get; set; }
            public decimal GoodsPurchaseLimit { get; set; }
            public string GoodsLimitType { get; set; }
            public bool TempSMS { get; set; }
            public bool AutoTopUp { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
            public CardStatus CardStatus { get; set; }

            public string MFBCardPhoto { get; set; }
            #endregion


            #region KiiPay User Identification Details

            public string IdCardType { get; set; }
            public string IdCardNumber { get; set; }
            public DateTime IdExpiryDate { get; set; }
            public string IdIssuingCountry { get; set; }

            public string KiiPayUserPhoto { get; set; }
        #endregion

        public virtual KiiPayBusinessInformation KiiPayBusinessInformation { get; set; }


    }
}