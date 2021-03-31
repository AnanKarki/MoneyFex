using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class UpdateProperyViewModel
    {
        public int TransactionId { get; set; }
        public string ReceiptNo { get; set; }
        [Required(ErrorMessage = "Enter Receiver Name")]
        public string ReceiverName { get; set; }
        public string BankCode { get; set; }
        public int WalletId { get; set; }
        public int BankId { get; set; }
        public int RecipientId { get; set; }
        [Required(ErrorMessage = "Enter Account No")]
        public string AccountNo { get; set; }
        public TransactionServiceType TransactionServiceType { get; set; }
        public string Country { get; set; }
        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public string AccountNumber { get; set; }
        public string PhoneNumber { get; set; }
        public string ReceiverStreet { get; set; }
        public string ReceiverPostalCode { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverEmail { get; set; }
        public int IdenityCardId { get; set; }
        public string IdentityCardNumber { get; set; }
        public bool IsEuropeTransfer { get; set; }
        public bool IsWestAfricaTransfer { get; set; }
        public bool IsSouthAfricaTransfer { get; set; }

    }
}