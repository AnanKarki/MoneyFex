using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class CashPickupInformationViewModel
    {
        public const string BindProperty = "Id , FirstName , MiddleName, LastName, Search, DOB, IdType, IdNumber," +
                                           " ExpiryDate ,CountryCode,IssuingCountry,SenderCountryCode,AddressLine1,AddressLine2,City ," +
                                           " PostCode ,Country,MobileNo,MobleCode,Email,Searched,Day,Month ,Year ,Gender,SenderFullName";

        public int Id { get; set; }
        [Required(ErrorMessage = "Enter First Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Enter Last Name")]
        public string LastName { get; set; }
        public string Search { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public System.DateTime? DOB { get; set; }
        [Required(ErrorMessage = "Select ID Type")]
        public int IdType { get; set; }
        [Required(ErrorMessage = "Enter ID Number")]
        public string IdNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "Enter Expiry Date")]
        public DateTime ExpiryDate { get; set; }
        public string CountryCode { get; set; }
        [Required(ErrorMessage = "Select Issuing Country")]
        public string IssuingCountry { get; set; }
        public string SenderCountryCode { get; set; }
        [Required(ErrorMessage = "Enter Address")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string PostCode { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Enter Mobile  Number")]
        public string MobileNo { get; set; }
        public string MobleCode { get; set; }

        public string Email { get; set; }

        public bool Searched { get; set; }

        public int Day { get; set; }
        public Month Month { get; set; }
        public int Year { get; set; }
        public Gender Gender { get; set; }
        public string SenderFullName{ get; set; }


    }
}