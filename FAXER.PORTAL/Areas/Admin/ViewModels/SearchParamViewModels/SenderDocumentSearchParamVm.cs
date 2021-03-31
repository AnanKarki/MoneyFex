using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels
{
    public class SenderDocumentSearchParamVm
    {

        public string SenderName { get; set; }
        public string AccountNo { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int Status { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }
        public string DocumentName { get; set; }
        public string Uploader { get; set; }
        public string DateRange { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}