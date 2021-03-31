using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class VirtualAccountDepositViewModel
    {
        public const string BindProperty = "Id ,CountryPhoneCode ,PhoneNumber ,DropdownPhoneNumber";

        public int Id { get; set; }
        public string CountryPhoneCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        public string DropdownPhoneNumber { get; set; }
    }
}