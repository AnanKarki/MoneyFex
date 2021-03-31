using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class StaffComplianceDocumentationViewModel
    {
        public const string BindProperty = "Id , ResidencePermitIDCardUrl ,PassportSide1Url , PassportSide2Url , UtilityBillUrl ,CVUrl , HighestQualificationUrl";

        public int Id { get; set; }
        public string ResidencePermitIDCardUrl { get; set; }
        public string PassportSide1Url { get; set; }
        public string PassportSide2Url { get; set; }
        public string UtilityBillUrl { get; set; }
        public string CVUrl { get; set; }
        public string HighestQualificationUrl { get; set; }
    }

    
}