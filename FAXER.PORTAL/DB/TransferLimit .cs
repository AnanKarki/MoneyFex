using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class TransferLimit
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public decimal Amount { get; set; }
        public UserCategory UserCategory { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }
    public enum UserCategory
    {
        [Display(Name = "Verified")]
        [Description("Verified")]
        Verified,
        [Display(Name = "Non-Verifired")]
        [Description("Non-Verifired")]
        NonVerifired
    }
}