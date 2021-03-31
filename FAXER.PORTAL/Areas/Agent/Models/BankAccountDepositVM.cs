using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class BankAccountDepositVM
    {
        [Required(ErrorMessage ="Enter First Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Select Date of Birth")]
        public DateTime DOB { get; set; }
        [Required(ErrorMessage = "Enter Id type")]
        public int IdType { get; set; }
        [Required(ErrorMessage = "Enter Id Number")]
        public int IdNumber { get; set; }
        public DateTime ExpiryDate { get; set; }
        [Required(ErrorMessage = "Select Issuing Country")]
        public string IssuingCountry{ get; set; }
        [Required(ErrorMessage = "Enter Address")]
        public string AddressLine1 { get; set; }
        public string AddressLine2  { get; set; }
        [Required(ErrorMessage = " Enter PostCode/ZipCode")]
        public string PostCode { get; set; }
        [Required(ErrorMessage = " Select Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = " Enter Mobile Number")]
        public int MobileNo { get; set; }
        [Required(ErrorMessage = " Enter Email Address")]
        public string EmailAddress { get; set; }
    }
}