using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class AddNewBankAccountViewModel
    {
        public const string BindProperty = "Id ,CountryCode ,NameOfAccountOwner , AccountNumber, BankId, Branchcode ,Branch  ,Address";

        public int Id { get; set; }
        public string CountryCode { get; set; }
        public string NameOfAccountOwner { get; set; }
        public string AccountNumber { get; set; }
        public int BankId { get; set; }
        public string Branchcode { get; set; }
        public string Branch { get; set; }
        public string Address { get; set; }
    }
}