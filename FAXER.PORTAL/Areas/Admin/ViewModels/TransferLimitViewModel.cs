using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class TransferLimitViewModel
    {
        public const string BindProperty = "Id , Country,Amount ,UserCategory,UserCategoryName";
        public int? Id { get; set; }
        [Required(ErrorMessage = "Select County")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Enter Amount")]
        public decimal Amount { get; set; }
        public UserCategory UserCategory { get; set; }
        public string UserCategoryName{ get; set; }
    }
}