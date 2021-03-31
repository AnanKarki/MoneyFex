using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class TrackFaxDetails
    {
        [Display(Name = "MoneyFax Control Number (MFCN)")]
        public string MFCNNumber { get; set; }

        [Display(Name ="Status of Fax")]
        public string StatusOfFax { get; set; }

        public string SenderSurName { get; set; }
    }
}