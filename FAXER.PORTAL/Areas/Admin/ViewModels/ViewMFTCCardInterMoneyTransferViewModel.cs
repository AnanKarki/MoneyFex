using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMFTCCardInterMoneyTransferViewModel
    {
        public int Id { get; set; }
        public int FaxerId { get; set; }
        public string FaxerName { get; set; }
        public string FaxerAddress { get; set; }
        public string FaxerCountry { get; set; }
        public string FaxerTelephone { get; set; }
        public string FaxerEmail { get; set; }
        public string FaxingMFTCNumber { get; set; }
        public string FaxingMFTCName { get; set; }
        public string FaxingMFTCCountry { get; set; }
        public decimal FaxingMFTCAmount { get; set; }
        public string ReceivingMFTCNumber { get; set; }
        public string ReceivingMFTCName { get; set; }
        public string ReceivingMFTCCountry { get; set; }
        public decimal ReceivingMFTCAmount { get; set; }
        public decimal AmountTransferred { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
    }
}