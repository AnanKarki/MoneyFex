using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class TopUpAnAccountViewModel
    {

        public const string BindProperty = "Country , SupplierId ,AccountNo ";
        [Required(ErrorMessage ="Select County")]
        public string  Country { get; set; }
        [Required(ErrorMessage = "Select Supllier")]
        public string SupplierId { get; set; }

        [Required (ErrorMessage ="Enter Account Number")]
        public string AccountNo { get; set; }
    }
}