using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AgentComplianceReportViewModel
    {
        public const string BindProperty = "Id,AgentId,UploaderId,Country,AccountNo,ReportTitle,WriterName,SubmissionDate,DocumentPhotoUrl,PartnerId";

        //For Add and Update
        public int Id{ get; set; }
        [Required(ErrorMessage = "Select Agent")]
        public int AgentId{ get; set; }
        public int UploaderId{ get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country{ get; set; }
        [Required(ErrorMessage = "Select City")]
        public string City{ get; set; }
        public string AccountNo { get; set; }
        [Required(ErrorMessage = "Enter Report Title")]
        public string ReportTitle { get; set; }
        public string WriterName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? SubmissionDate { get; set; }
        [Required(ErrorMessage = "Choose a file")]
        public string DocumentPhotoUrl { get; set; }
        public int PartnerId { get; set; }

        //For Index View
        public string Name{ get; set; }
        public string CountryFlag{ get; set; }
        public string StaffName{ get; set; }
        public string DateTime{ get; set; }

    }
}