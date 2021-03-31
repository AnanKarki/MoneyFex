using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderPayingSupplierAbroadReferenceVM
    {
        public const string BindProperty = "ReferenceNo,ReferenceNo1,ReferenceNo2,ReferenceNo3";
        [Required(ErrorMessage ="Enter Reference No") ]
        public string ReferenceNo { get; set; }
        public string ReferenceNo1 { get; set; }
        public string ReferenceNo2 { get; set; }
        public string ReferenceNo3 { get; set; }

    }
}