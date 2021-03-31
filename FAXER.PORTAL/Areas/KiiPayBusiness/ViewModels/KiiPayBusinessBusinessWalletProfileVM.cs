using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessBusinessWalletProfileVM
    {
        public const string BindProperty = " Id ,NameofBusiness  ,BusinessLicense , Address,Country , TelephoneWalletNumber,Email , BillIssuingCompany, ContactPersonName, " +
            "ContactPersonAddress,ContactPersonCountry , MobileCode";
        public int Id { get; set; }
        public string NameofBusiness{ get; set; }
        public string BusinessLicense{ get; set; }
        [Required(ErrorMessage = "Please enter  Address ")]
        public string Address{ get; set; }
        public string Country{ get; set; }
        [Required(ErrorMessage = "Please enter  Telephone/WalletNumber ")]
        public string TelephoneWalletNumber{ get; set; }
        [Required(ErrorMessage = "Please enter  Email ")]
        public string Email{ get; set; }
        [Required(ErrorMessage = "Please enter  BillIssuingCompany ")]
        public bool BillIssuingCompany { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonAddress { get; set; }
        public string ContactPersonCountry { get; set; }
        [Required(ErrorMessage = "Please enter  MobileCode ")]
        public string MobileCode { get; set; }
    }
}