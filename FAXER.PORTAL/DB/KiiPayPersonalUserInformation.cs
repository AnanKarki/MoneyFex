using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayPersonalUserInformation
    {
        public int Id { get; set; }

        public int KiiPayPersonalWalletInformationId { get; set; }

        #region Personal Details 
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        public string Lastname { get; set; }

        public DateTime DOB { get; set; }
        public Gender Gender { get; set; }

        public string EmailAddress { get; set; }

        #endregion

        #region Personal Address 

        public string Country { get; set; }
        public string City { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string PostCode { get; set; }
        public string MobileNo { get; set; }
        #endregion



        #region KiiPay Personal User Identification Details

        public string IdCardType { get; set; }
        public string IdCardNumber { get; set; }
        public DateTime? IdCardExpiringDate { get; set; }
        public string IssuingCountry { get; set; }
        #endregion




        public virtual KiiPayPersonalWalletInformation KiiPayPersonalWalletInformation { get; set; } 
    }
}