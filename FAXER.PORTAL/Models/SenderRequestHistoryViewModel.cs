using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderRequestHistoryViewModel
    {
        public int Id { get; set; }
        public int FilterKey { get; set; }
        public List<SenderRequestHistoryList> RequestHistoryList { get; set; }
    }

    public class SenderRequestHistoryList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WalletNumber { get; set; }
        public string Date { get; set; }
        public string Amount { get; set; }
        public RequestPaymentStatus Status { get; set; }
    }
}