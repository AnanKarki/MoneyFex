using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class TrackFax
    {
        public const string BindProperty = "FaxerSurNam,MoneyFaxControlNumber,FaxingStatus";
        [Required (ErrorMessage ="Enter Faxer Last Name")]
        public string FaxerSurNam { get; set; }
        [Required (ErrorMessage = "Enter MFCN Number ")]
        public string MoneyFaxControlNumber { get; set; }

        public string FaxingStatus { get; set; }

    }
}