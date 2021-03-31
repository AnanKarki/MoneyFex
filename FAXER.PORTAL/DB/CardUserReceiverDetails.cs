using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class CardUserReceiverDetails
    {
        [Key]
        public int Id { get; set; }
        
        public int MFTCCardInformationID { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public string EmailAddress { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public bool IsDeleted { get; set; }

        public virtual KiiPayPersonalWalletInformation MFTCCardInformation { get; set; }
    }
}