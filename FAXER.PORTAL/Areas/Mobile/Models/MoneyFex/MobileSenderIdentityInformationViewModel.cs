using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.DB;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex
{
    public class MobileSenderIdentityInformationViewModel
    {

        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int SenderId { get; set; }
        public string AccountNo { get; set; }
        public DocumentType DocumentType { get; set; }
        public DocumentExpires DocumentExpires { get; set; }
        public int IdentificationTypeId { get; set; }
        public string IdentityNumber { get; set; }
        public string FormatedIdentityNumber { get; set; }
        public string IdentificationTypeName { get; set; }
        public string FormatedIdentityName { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string ExpiryYearMonth { get; set; }
        public string ExpiryMonth { get; set; }
        public string ExpiryYear { get; set; }
        public string ExpiryDay { get; set; }
        public string DocumentName { get; set; }
        public string DocumentPhotoUrl { get; set; }
        public string DocumentPhotoUrlTwo { get; set; }
        public string IssuingCountry { get; set; }
        public string IssuingCountryCode { get; set; }
        public DocumentApprovalStatus Status { get; set; }
        public bool IsUploadedFromSenderPortal { get; set; }
    }
}