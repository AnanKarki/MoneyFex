using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ReceiverDocumentationViewModel
    {
        public const string BindProperty = "Id, Country,CountryFlag ,City ,ReceiverId,ReceiverName ,ReceiverNumber ,DocumentType , DocumentTypeName, DocumentExpires," +
            " ExpiryDate, DocumentName, DocumentPhotoUrl,CreatedBy,CreatedDate,Status";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        public string CountryFlag { get; set; }
        [Required(ErrorMessage = "Select City")]
        public string City { get; set; }
        [Required(ErrorMessage = "Select Receiver")]
        public int ReceiverId { get; set; }
        public string ReceiverNumber { get; set; }
        public string ReceiverName { get; set; }
        [Required(ErrorMessage = "Select Document Type")]
        public DocumentType DocumentType { get; set; }
        public string DocumentTypeName { get; set; }
        public DocumentExpires DocumentExpires { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? ExpiryDate { get; set; }
        [Required(ErrorMessage = "Enter Document Name")]
        public string DocumentName { get; set; }
        [Required(ErrorMessage = "Choose a file")]
        public string DocumentPhotoUrl { get; set; }
        public string CreatedByName { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DocumentApprovalStatus Status { get; set; }
        public string ExpiryDateString { get; set; }

    }
}