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
    
    public partial class AgentInformation
    {
        
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string RegistrationNumber { get; set; }
        public string LicenseNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ContactPerson { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        // City Added
        public string City { get; set; }

        public string CountryCode { get; set; }
        public string PhoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string Website { get; set; }
        // AgentStatus added        
        public AgentStatus AgentStatus { get; set; }
        public string AccountNo { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsAUXAgent { get; set; }
        public DateTime? CreatedDate { get; set; }

        //public BusinessType? BusinessType { get; set; }
        public BusinessType? TypeOfBusiness { get; set; }







    }
    public enum AgentStatus
    {
        Active = 0,
        Inactive = 1,
        Delayed = 2

    }
}
