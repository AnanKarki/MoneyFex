using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderTopUpSupplierAbroadAccountNmberVM
    {
        public const string BindProperty = "PhotoUrl,Name,AccountNo1,AccountNo2,AccountNo3,AccountNo4,AccountNo5,AccountNo6";

        public string PhotoUrl { get; set; }
        public string Name { get; set; }
        [Required(ErrorMessage ="Enter Account Number")]
        public string AccountNo1 { get; set; }

        public string AccountNo2 { get; set; }
        public string AccountNo3 { get; set; }
        public string AccountNo4 { get; set; }
        public string AccountNo5 { get; set; }
        public string AccountNo6{ get; set; }



    }
}