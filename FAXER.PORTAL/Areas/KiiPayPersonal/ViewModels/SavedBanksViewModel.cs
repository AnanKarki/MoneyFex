using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class SavedBanksViewModel
    {
        public int Id { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string Status { get; set; }
    }
}