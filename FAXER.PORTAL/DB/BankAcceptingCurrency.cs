using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BankAcceptingCurrency
    {
        public int Id { get; set; }
        public int BankId { get; set; }
        public int ServiceSettingId { get; set; }
        public string Currency { get; set; }
        public DateTime CreateDate { get; set; }
    }
}