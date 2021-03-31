using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.ViewModels
{
    public class BusinessOwnerInformationVm
    {
        public const string BindProperty = "Id , FirstName , MiddleName, LastName, IsAddressSame, AddressLine1 , AddressLine2, ZipCode," +
                                               " City ,State ,CountryCode ,ContactNo ,EmailAddress";


        public int Id { get; set; }

        [Required, DisplayName("First Name")]
        public string FirstName { get; set; }

        public string MiddleName { get; set; }

        [Required, DisplayName("Last Name")]
        public string LastName { get; set; }

        public bool IsAddressSame { get; set; }

        [Required, DisplayName("Address Line1")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }

        [Required, DisplayName("Zip Code")]
        public string ZipCode { get; set; }

        [Required, DisplayName("City")]
        public string City { get; set; }

        [Required, DisplayName("State")]
        public string State { get; set; }

        [Required, DisplayName("Country")]
        public string CountryCode{ get; set; }

        [Required, DisplayName("Contact Number")]
        public string ContactNo { get; set; }

        [Required, DisplayName("Email Address"),DataType(DataType.EmailAddress)]
        public string EmailAddress { get; set; }
    }
}