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
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ReceiversDetails
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("FaxerInformation")]
        public int FaxerID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }

        public string FullName { get; set; }

        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }
       
        public virtual FaxerInformation FaxerInformation { get; set; }
        public virtual NonCardReceive NonCardReceive { get; set; }
    }
}
