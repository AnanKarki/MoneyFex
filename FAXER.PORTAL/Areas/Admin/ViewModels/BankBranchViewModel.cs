using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class BankBranchViewModel
    {
        public const string BindProperty = "Id,BankId , BranchName, BankName, Country, BranchCode ,BranchAddress ";

        public int Id { get; set; }
        [Required(ErrorMessage ="Select Bank")]
        public int BankId { get; set; }
        [Required(ErrorMessage = "Enter Branch Name")]
        public string BranchName { get; set; }
        public string BankName { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country{ get; set; }
        [Required(ErrorMessage = "Enter Bank Code")]
        public string BranchCode { get; set; }
        [Required(ErrorMessage = "Enter Bank Address")]
        public string BranchAddress { get; set; }

    }
}