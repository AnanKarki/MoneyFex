using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffInformationViewModel
    {
        #region Staff General Details 
        public int StaffId { get; set; }
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string DOB { get; set; }
        #endregion
        #region CurrentAddress
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        public string Country { get; set; }
        public string CountryFlag { get; set; }
        public string TimeAtAddress { get; set; }
        #endregion
        #region staff Next of kin 
        public string KinFullName { get; set; }
        public string KinRelation { get; set; }
        
        public string KinAddress { get; set; }
        public string KinTelephone { get; set; }
        public string KinEmail { get; set; }

        #endregion

        #region Staff Identification
        public string IdCardTumber { get; set; }
        public DateTime IdCarype { get; set; }
        public string IdCardNdExpiryDate { get; set; }
        public string IdCardIssuingCountry { get; set; }
        public string IdCardIssuingCountryFlag { get; set; }

        #endregion

        #region Staff Compliance Document 
        public string ResidentPermit { get; set; }
        public string PassportSide1 { get; set; }
        public string PassportSide2 { get; set; }
        public string UtilityBill { get; set; }
        public string CiriculamVitae { get; set; }
        public string HighestQual { get; set; }
        #endregion

    }
}