using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class LogoAssignViewModel
    {
        public const string BindProperty = "Master , Details ";

        public LogoAssignMasterViewModel Master { get; set; }
        public List<LogoAssignDetailsViewModel> Details { get; set; }

    }

    public class LogoAssignMasterViewModel
    {
        public const string BindProperty = "Id , SendingCountry ,ReceivingCountry , Services,Label ,CreatedDate , CreatedBy ";

        public int Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string SendingCountry { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string ReceivingCountry { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public TransactionTransferMethod Services { get; set; }
        public string Label { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }
    public class LogoAssignDetailsViewModel
    {
        public const string BindProperty = "Id , LogoAssignId ,ImageUrl , LogoUpload,LogoUploadId ,IsChecked";

        public int Id { get; set; }
        public int LogoAssignId { get; set; }
        public string ImageUrl { get; set; }
        public string LogoUpload { get; set; }
        public int LogoUploadId { get; set; }
        public bool IsChecked { get; set; }

    }
}