using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class SenderBusinessDocumentation
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        [Index(nameof(SenderId))]
        public int SenderId { get; set; }
        public string AccountNo { get; set; }
        public DocumentType DocumentType { get; set; }
        public DocumentExpires DocumentExpires { get; set; }
        public int IdentificationTypeId { get; set; }
        public string IdentityNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPhotoUrl { get; set; }
        public string DocumentPhotoUrlTwo { get; set; }
        public DateTime CreatedDate { get; set; }

        [Index(nameof(CreatedBy))]
        public int CreatedBy { get; set; }
        public string IssuingCountry { get; set; }
        public DocumentApprovalStatus Status { get; set; }
        public bool IsUploadedFromSenderPortal { get; set; }
        public bool IsUploadedFromAgentPortal { get; set; }
        public bool IsUploadedFromAuxAgentAdmin { get; set; }
        public int AgentId { get; set; }
        public ReasonForDisApproval ReasonForDisApproval { get; set; }
        public string ReasonForDisApprovalByAdmin { get; set; }
        public ComplianceApprovalStatus ComplianceApprovalStatus { get; set; }
    }

    public enum DocumentApprovalStatus
    {
        Select = 3,
        [Description("Approved")]
        Approved = 0,
        [Description("Disapproved")]
        Disapproved = 1,
        [Description("In progress")]
        InProgress = 2
    }
    public enum ReasonForDisApproval
    {
        [Description("Select Reason For DisApproval")]
        [Display(Name = "Select Reason For DisApproval")]
        Select,

        [Description("the name on ID does not match with registered name on our system")]
        [Display(Name = "the name on ID does not match with registered name on our system")]
        IdDoesNotMatch,

        [Description("the serial number is missing from the image sent")]
        [Display(Name = "the serial number is missing from the image sent")]
        SerialNumberMissing,

        [Description("it is not a full data page of the ID")]
        [Display(Name = "it is not a full data page of the ID")]
        NotFullData,

        [Description("you sent a photo of yourself instead")]
        [Display(Name = "you sent a photo of yourself instead")]
        OwnPhoto,

        [Description("the image sent cannot be read by our system")]
        [Display(Name = "the image sent cannot be read by our system")]
        ImageCannotBeRead,

        [Description("the expiry date is missing")]
        [Display(Name = "the expiry date is missing")]
        ExpiryDateMissing,

        [Description("the ID uploaded has expired")]
        [Display(Name = "the ID uploaded has expired")]
        IdExpired,

        [Description("it cannot be verified on our system")]
        [Display(Name = "it cannot be verified on our system")]
        CannotBeVerified,

        [Description("Others")]
        [Display(Name = "Others")]
        Others,

    }
    public enum ComplianceApprovalStatus
    {
        [Description("Compliance in progress")]
        [Display(Name = "Compliance in progress")]
        InProgress,
        [Description("Compliance Approved")]
        [Display(Name = "Compliance Approve")]
        Approved,
        [Description("Compliance Disapproved")]
        [Display(Name = "Compliance Disapprove")]
        Disapproved,

    }
}