using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BusinessRelatedInformation
    {
        public int Id { get; set; }
        public int FaxerId { get; set; }
        public string BusinessName { get; set; }
        public string RegistrationNo { get; set; }
        public SenderBusinessType BusinessType { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Postal { get; set; }
        public string OperatingPostal { get; set; }
        public string OperatingAddressLine1 { get; set; }
        public string OperatingAddressLine2 { get; set; }
        public string OperatingCity { get; set; }
        
    }

    public enum SenderBusinessType
    {
        [Display(Name = "Select Type")]
        Select,
        [Display(Name = "Sole Trader")]
        SoleTrader,
        [Display(Name = "Limited Company")]
        LimitedCompany,
        [Display(Name = "Trust")]
        Trust,
        [Display(Name = "Charity")]
        Charity,
        [Display(Name = "Partner Company")]
        PartnerCompany,
        [Display(Name = "Company Limited by Guarantee")]
        CompanyLimitedbyGuarantee,
        Other
    }
}