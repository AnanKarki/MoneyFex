using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class PayMonthlyBillViewModel
    {

        public const string BindProperty = "Country , SupplierId ,ReferenceNo";
        [Required(ErrorMessage ="Select country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Select Supplier")]
        public string SupplierId { get; set; }
        [Required(ErrorMessage = "Enter Reference Number")]
        public string ReferenceNo { get; set; }

    }
}