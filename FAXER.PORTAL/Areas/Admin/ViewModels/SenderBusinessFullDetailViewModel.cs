using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SenderBusinessFullDetailViewModel
    {
        public SenderBusinessprofileViewModel SenderDetails { get; set; }
        public List<SenderBusinessTransactionStatement> TransactionDetails { get; set; }
        public List<BusinessLimitViewModel> Limit { get; set; }
        public List<BusinessLimitViewModel> LimitHistory { get; set; }
        //public List<BusinessDocumentationViewModel> IdentificationDetails { get; set; }
    }
}