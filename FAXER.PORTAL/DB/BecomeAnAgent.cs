//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FAXER.PORTAL.DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public partial class BecomeAnAgent
    {

        public const string BindProperty = "Id,AgentRegistrationCode,FirstName,LastName,CompanyBusinessName, BusinessType, BusinessLicenseRegistrationNumber, Address1, Address2, Street,City ,StateProvince ," +
                                           "PostZipCode , CountryCode,ContactName,BusinessEmailAddress, ContactPhone,FaxNo ,Website , AgentType,ActivationCode";


        public int Id { get; set; }
        public string AgentRegistrationCode { get; set; }
        [Required]
        [RegularExpression(@"^[a-z||A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Only Aphabetic Character is allowed"), DisplayName("First Name")]
        public string FirstName { get; set; }
        [Required]
        [RegularExpression(@"^[a-z||A-Z]+[a-zA-Z""'\s-]*$", ErrorMessage = "Only Aphabetic Character is allowed"), DisplayName("Last Name")]
        public string LastName { get; set; }
        [Required, DisplayName("Company Business Name")]
        public string CompanyBusinessName { get; set; }

        [DisplayName("Business Type")]
        public BusinessType BusinessType { get; set; }
        [Required, DisplayName("Business Licence Registration Number")]
        public string BusinessLicenseRegistrationNumber { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string Street { get; set; }
        [Required]
        public string City { get; set; }
        [Required, DisplayName("State/Province")]
        public string StateProvince { get; set; }
        [DisplayName("Post/Zip")]
        public string PostZipCode { get; set; }

        [Required, DisplayName("Country")]
        public string CountryCode { get; set; }
        [Required, DisplayName("Contact Name")]
        public string ContactName { get; set; }
        [Required]
        //[RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        [RegularExpression(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", ErrorMessage = "Please enter valid email")]
        public string BusinessEmailAddress { get; set; }
        [Required]
        //[RegularExpression(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}", ErrorMessage = "Please enter valid Phone Number")]
        public string ContactPhone { get; set; }
        public string FaxNo { get; set; }

        [RegularExpression(@"(http(s)?://)?([\w-]+\.)+[\w-]+(/[\w- ;,./?%&=]*)?", ErrorMessage = "Please enter valid URL")]
        public string Website { get; set; }

        public AgentTypePAndL AgentType { get; set; }
        public string ActivationCode { get; set; }


    }

    public enum BusinessType
    {
        [Display(Name = "Select Business Type")]
        [Description("Non")]
        Non,

        [Display(Name = "Insurance & Financial service")]
        [Description("Insurance & Financial service")]
        InsuranceFinancialService,

        [Display(Name = "Education")]
        [Description("Education")]
        Education,

        [Display(Name = "Retail")]
        [Description("Retail")]
        Retail,

        [Display(Name = "Health services")]
        [Description("Health services")]
        HealthServices,

        [Display(Name = "Legal")]
        [Description("Legal")]
        Legal,

        [Display(Name = "Energy supplier")]
        [Description("Energy supplier")]
        EnergySupplier,

        [Display(Name = "Entertainment")]
        [Description("Entertainment")]
        Entertainment,

        [Display(Name = "Hotel & Accommodation")]
        [Description("Hotel & Accommodation")]
        HotelAccommodation,

        [Display(Name = "IT & Telecommunication")]
        [Description("IT & Telecommunication")]
        ITTelecommunication,

        [Display(Name = "Others")]
        [Description("Others")]
        Others



    }

    public enum AgentTypePAndL
    {

        Non,
        Principal,
        Local
    }
}