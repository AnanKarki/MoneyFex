using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderTopUpAnAccountVM
    {
        public const string BindProperty = "Country,Supplier,WalletNo";
        [Required(ErrorMessage = "Select County")]
        public string Country { get; set; }

        public string Supplier{ get; set; }

     
        public string WalletNo { get; set; }
    }
}