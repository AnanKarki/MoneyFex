using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AddNewBankAccountViewModel
    {
        public const string BindProperty = "Id , Country ,AccountNo , LabelName, LabelValue, TransferType";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Enter Account no")]
        public string AccountNo { get; set; }
        public string LabelName { get; set; }
        public string LabelValue { get; set; }
        public TransferTypeForBankAccount TransferType { get; set; }
    }
}