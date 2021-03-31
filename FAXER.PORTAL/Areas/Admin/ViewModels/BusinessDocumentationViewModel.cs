using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class BusinessDocumentationViewModel
    {
        public const string BindProperty = "Id,Country ,IssuingCountry,CountryFlag,City ,SenderId , AccountNo, DocumentType, DocumentExpires, ExpiryDate,DocumentName , DocumentPhotoUrl, CreatedDate ," +
                                           "CreatedBy ,CreatedByStaffName ,SenderName,StatusName,Status,AgentId,StaffId, ReasonForDisApproval,ReasonForDisApprovalByAdmin ";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        public string IssuingCountry { get; set; }
        public string CountryFlag { get; set; }
        [Required(ErrorMessage = "Select City")]
        public string City { get; set; }
        [Required(ErrorMessage = "Select Business")]
        public int SenderId { get; set; }
        [Required(ErrorMessage = "Select Business")]
        public string AccountNo { get; set; }
        [Required(ErrorMessage = "Select Document Type")]
        public DocumentType DocumentType { get; set; }
        public DocumentExpires DocumentExpires { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? ExpiryDate { get; set; }

        [Required(ErrorMessage = "Enter Document Name")]
        public string DocumentName { get; set; }
        public string DocumentPhotoUrl { get; set; }
        public string DocumentPhotoUrlTwo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreationDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByStaffName { get; set; }
        public string SenderName { get; set; }
        public string StatusName { get; set; }
        public DocumentApprovalStatus Status { get; set; }
        [Required(ErrorMessage = "Select Agent")]
        public int AgentId { get; set; }

        //for staff documentation
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        //


        public string ExpiryDateString { get; set; }
        public string CountryCode { get; set; }

        public ReasonForDisApproval ReasonForDisApproval { get; set; }
        public string ReasonForDisApprovalByAdmin { get; set; }
    }

}
