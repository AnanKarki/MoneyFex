using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderPayARequestViewModel
    {
        public int Id { get; set; }
        public int StatusList { get; set; }
        public List<SenderRequestsList> RequestsList { get; set; }
    }
    public class SenderRequestsList
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WalletNo { get; set; }
        public string Date { get; set; }
        public string Amount { get; set; }
        public PaymentType PaymentType { get; set; }
        public RequestPaymentStatus Status { get; set; }

    }
}