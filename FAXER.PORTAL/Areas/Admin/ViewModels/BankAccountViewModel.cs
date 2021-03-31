using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class BankAccountViewModel
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public string AccountNo { get; set; }
        public string LabelName { get; set; }
        public string LabelValue { get; set; }
        public string CountryFlag { get; set; }
        public TransferTypeForBankAccount TransferType { get; set; }
        public string TransferTypeName { get; set; }
    }
}