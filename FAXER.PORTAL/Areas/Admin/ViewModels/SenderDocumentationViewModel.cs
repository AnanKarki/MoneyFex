using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SenderDocumentationViewModel
    {
        public const string BindProperty = "Id,Country ,City ,SenderId , AccountNo, DocumentType, DocumentExpires, ExpiryDate,DocumentName , DocumentPhotoUrl, CreatedDate ," +
                                         "CreatedBy ,CreatedByStaffName ,SenderName ,StatusName,IssuingCountry ,Status ,IsUploadedFromAgentPortal,IsUploadedFromSenderPortal," +
            " AgentId,IdentityNumber,IdentificationTypeId, ReasonForDisApproval ,ReasonForDisApprovalByAdmin , ComplianceApprovalStatus";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }

        public string City { get; set; }
        [Required(ErrorMessage = "Select Sender")]
        public int SenderId { get; set; }
        [Required(ErrorMessage = "Select Sender")]
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
        public int CreatedBy { get; set; }
        public int NoteCount { get; set; }


        //[Required(ErrorMessage = "Select IssuingCountry")]
        public string IssuingCountry { get; set; }
        [Required(ErrorMessage = "Select Status")]


        private string _statusName;

        public DocumentApprovalStatus? Status { get; set; }

        public void SetStatusName()
        {

            switch (Status)
            {
                case DocumentApprovalStatus.Approved:
                    _statusName = "Approved";
                    break;
                case DocumentApprovalStatus.Disapproved:
                    _statusName = "Disapproved";
                    break;
                case DocumentApprovalStatus.InProgress:
                    _statusName = "InProgress";
                    break;
                default:
                    break;
            }
        }

        public string StatusName { get; set; }
        public bool IsUploadedFromSenderPortal { get; set; }
        public bool IsUploadedFromAgentPortal { get; set; }
        public int AgentId { get; set; }

        public string IdentityNumber { get; set; }
        public int IdentificationTypeId { get; set; }
        public string TelephoneNo { get; set; }
        public string SenderEmailAddress { get; set; }

        public ReasonForDisApproval ReasonForDisApproval { get; set; }
        public string ReasonForDisApprovalByAdmin { get; set; }
        public ComplianceApprovalStatus ComplianceApprovalStatus { get; set; }

        public string StaffFirstName { get; set; }
        public string StaffLastName { get; set; }
        public string StaffMiddleName { get; set; }

        public string SenderFirstName { get; set; }
        public string SenderLastName { get; set; }
        public string SenderMiddleName { get; set; }


        public string SenderName { get; set; }

        public string SenderFullName
        {
            get
            {
                return SenderFirstName + " " + (string.IsNullOrEmpty(SenderMiddleName) == true ? "" : SenderMiddleName + " ") + SenderLastName;

            }
        }


        public string CreatedByStaffName
        {
            get
            {
                return StaffFirstName + " " + (string.IsNullOrEmpty(StaffMiddleName) == true ? "" : StaffMiddleName + " ") + StaffLastName;
            }
        }

        public int OrderBy { get; set; }
        public int TotalCount { get; set; }
        public string CountryCode { get; set; }

    }
    public class SenderDocumentationAndSenderNote
    {
        public TransactionStatementNoteViewModel TransactionStatementNote { get; set; }
        public List<SenderDocumentationViewModel> SenderDocumentationViewModel { get; set; }
    }
}