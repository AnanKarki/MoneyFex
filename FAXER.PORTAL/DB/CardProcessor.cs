using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class CardProcessor
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public TransactionTransferType TransactionTransferType { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public CardProcessorApi CardProcessorApi { get; set; }
        public string CardProcessorName { get; set; }
        public string ContactName { get; set; }
        public string TelephoneNo { get; set; }
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }
    public enum CardProcessorApi
    {
        [Display(Name = "Select Card Procesor")]
        [Description("Select Card Procesor")]
        Select = 0,
        [Display(Name = "Trust Payment")]
        [Description("Trust Payment")]
        TrustPayment = 1,
        [Display(Name = "Transact365")]
        [Description("Transact365")]
        T365 = 2
    }
}