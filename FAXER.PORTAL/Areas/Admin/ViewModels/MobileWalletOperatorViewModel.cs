using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class MobileWalletOperatorViewModel
    {
        public const string BindProperty = "Id , Code,Name ,Country";

        public int? Id { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }

    }
}