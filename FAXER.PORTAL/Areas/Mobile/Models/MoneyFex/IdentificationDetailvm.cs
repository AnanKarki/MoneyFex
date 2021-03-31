using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex
{
    public class IdentificationDetailvm
    {
        public const string BindProperty = "Id,Country ,City , SenderId,AccountNo ,DocumentType ,DocumentExpires ,IdentificationTypeId ,IdentificationTypeName , IdentityNumber" +
            ", ExpiryDate, ExpiryDay, ExpiryMonth, ExpiryYear, DocumentName, DocumentPhotoUrl, DocumentPhotoUrlTwo, IssuingCountry,IssuingCountryCode , Status, IsUploadedFromSenderPortal ";
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int SenderId { get; set; }
        public string AccountNo { get; set; }
        public DocumentType DocumentType { get; set; }
        public DocumentExpires DocumentExpires { get; set; }
        public int IdentificationTypeId { get; set; }
        public string IdentificationTypeName { get; set; }
        public string IdentityNumber { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ExpiryDay { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPhotoUrl { get; set; }
        public string DocumentPhotoUrlTwo { get; set; }
        public string IssuingCountry { get; set; }
        public string IssuingCountryCode { get; set; }
        public DocumentApprovalStatus Status { get; set; }
        public bool IsUploadedFromSenderPortal { get; set; }
        public bool IsIdentityDocumentExist { get; set; }
        public bool IsIdentityDocumentExpired { get; set; }

    }
    public class IdentificationTypeDropDownVm
    {

        public int Id { get; set; }
        public string Name { get; set; }
    }

}