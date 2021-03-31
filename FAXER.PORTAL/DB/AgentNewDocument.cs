using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AgentNewDocument
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Document Type")]
        public DocumentType DocumentType { get; set; }

        [DataType(DataType.Date)]
        //[DisplayFormat(DataFormatString = "{0:dd MMM yyyy }")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? ExpiryDate { get; set; }

        [Required(ErrorMessage = "Select Document Title Name")]
        public string DocumentTitleName { get; set; }
        public DocumentExpires DocumentExpires { get; set; }
        public int AgentStaffId { get; set; }

        public DateTime UploadDateTime { get; set; }
        [Required(ErrorMessage = "Select Document File")]
        public string DocumentPhotoUrl { get; set; }
        public string AgentStaffName { get; set; }

        public string AgentStaffAccountNo { get; set; }
        public string IssuingCountry { get; set; }

        public DocumentApprovalStatus Status { get; set; }
        public int CreatedBy { get; set; }
        public int IdentificationTypeId { get; set; }
        public string IdentificationNumber { get; set; }
    }

    public enum DocumentType
    {
        [Display(Name = "Select type")]
        Select = 0,
        Compliance = 1,
        Staff = 2,
        Transaction = 3,
        Identification = 4
    }
    public enum DocumentExpires
    {
        Select = 0,
        Yes = 1,
        No = 2,

    }

}