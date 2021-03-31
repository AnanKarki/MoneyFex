using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class IdentificationDetailModel
    {

        public const string BindProperty = "IdentificationTypeId," +
            " IdentificationType ,Day , Month, Year,IdentityNumber,IssuingCountry," +
            "SenderBusinessDocumentationId,DocumentUrl,DocumentName,ExpiryDate";
        [Range(1, int.MaxValue, ErrorMessage = "Select Id Type")]
        public int IdentificationTypeId { get; set; }
        public string IdentificationType { get; set; }
        [Range(1, 32, ErrorMessage = "Enter a valid day")]
        [Required(ErrorMessage = "Enter day")]
        public int Day { get; set; }

        [Range(1, 32, ErrorMessage = "Select Month")]
        public Month Month { get; set; }

        [Range(1000, int.MaxValue, ErrorMessage = "Enter a valid year")]
        [Required(ErrorMessage = "Enter year")]
        public int Year { get; set; }
        public DateTime? ExpiryDate { get; set; }

        [Required(ErrorMessage = "Enter Identity Number")]
        public string IdentityNumber { get; set; }
        [Required(ErrorMessage = "Select Issuing Country")]
        public string IssuingCountry { get; set; }
        [Range(0, int.MaxValue)]
        public int SenderBusinessDocumentationId { get; set; }
        public DocumentApprovalStatus Status { get; set; }
        public string StatusName { get; set; }
        public string DocumentUrl { get; set; }
        public string DocumentUrlTwo { get; set; }
        public string DocumentName { get; set; }

    }

    public class IdentificationTypeModel
    {

        public int IdentificationTypeId { get; set; }
        public string Name { get; set; }

    }

    public class IssuingCountryModel
    {

        public string IssuingCountry { get; set; }
        public string IssuingCountryName { get; set; }


    }
}