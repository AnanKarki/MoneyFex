using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class MerchantNonCardReceiverDetails
    {


        [Key]
        public int Id { get; set; }
        [ForeignKey("Business")]
        public int KiiPayBusinessInformationId { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual KiiPayBusinessInformation Business { get; set; }
    }
}