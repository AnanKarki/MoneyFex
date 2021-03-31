using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayECardUserDetailsVm
    {
        public const string BindProperty = " Wallet, WalletId ,Name ,DateOfBirth , Address, Country,PhoneNo ,EmailAddress ,IDCardType ,IDCardNumber" +
            " ,ExpiringDateDay ,ExpiringDateMonth ,ExpiringDateYear , IDIssuingCountry ,City ,MobileNo ,MobileCode";
        public string Wallet { get; set; }
        public int WalletId { get; set; }
        public string Name { get; set; }
        public string DateOfBirth { get; set; }
        [Required(ErrorMessage = "Please enter  Address ")]
        public string Address { get; set; }
        public string Country { get; set; }
        public string PhoneNo { get; set; }
        [Required(ErrorMessage = "Please enter  EmailAddress ")]
        public string EmailAddress { get; set; }
        public string IDCardType { get; set; }
        public string IDCardNumber { get; set; }
        public int ExpiringDateDay { get; set; }
        public string ExpiringDateMonth { get; set; }
        public int ExpiringDateYear { get; set; }
        public string IDIssuingCountry { get; set; }
        [Required(ErrorMessage = "Please enter  City ")]
        public string City { get; set; }
        [Required(ErrorMessage = "Please enter  MobileNo ")]
        public string MobileNo { get; set; }
        public string MobileCode { get; set; }

    }
    public class WalletDropDownVM {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}