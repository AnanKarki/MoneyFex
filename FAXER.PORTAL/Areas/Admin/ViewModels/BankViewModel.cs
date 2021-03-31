using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class BankViewModel
    {
        public const string BindProperty = "Id ,Name ,Code , Address,CountryCode,Country,IsChecked ";
        public int? Id { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Enter Code")]
        public string Code { get; set; }
        public string Address { get; set; }
        [Required(ErrorMessage = "Select County")]
        public string CountryCode { get; set; }
        public string Country{ get; set; }
        public bool IsChecked { get; set; }
    }
}