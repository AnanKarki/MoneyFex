using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class BankToBankTransferViewModel
    {

        public const string BindProperty = "SenderSurname,AccountNumber," +
            " LabelName , LabelValue ,SendingAmount , Address1,Address2," +
            "City,PostalCode,Country,PaymentReference,Confirm, Accept," +
            "SendingCurrency, SendingCurrencySymbol";
        [MaxLength(200)]
        public string SenderSurname { get; set; }
        [MaxLength(200)]
        public string AccountNumber { get; set; }
        [MaxLength(200)]
        public string LabelName { get; set; }

        [MaxLength(200)]
        public string LabelValue { get; set; }
        
        [Range(typeof(decimal), "0", "20")]
        public decimal SendingAmount { get; set; }
        [MaxLength(200)]
        public string Address1 { get; set; }
        [MaxLength(200)]
        public string Address2 { get; set; }
        [MaxLength(200)]
        public string City { get; set; }
        [MaxLength(200)]
        public string PostalCode { get; set; }
        [MaxLength(200)]
        public string Country { get; set; }
        [MaxLength(200)]
        public string PaymentReference { get; set; }
        
        public bool Confirm { get; set; }
        public bool Accept { get; set; }
        [MaxLength(200)]
        public string SendingCurrency { get; set; }
        [MaxLength(200)]
        public string SendingCurrencySymbol { get; set; }
    }
}