using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class BlackListedReceiverViewModel
    {
        public const string BindProperty = " Id,ReceiverAccountNo ,ReceiverName ,ReceiverCountry ,CareatedDate ,CreatedByUserId , IsBlocked" +
            ",TransferMethodName , TransferMethod,ReceiverTelephone , BankCode, BankNameOrMobileWalletProvider ,IsDeleted";
        public int Id { get; set; }
        [Required]
        public string ReceiverAccountNo { get; set; }
        [Required]
        public string ReceiverName { get; set; }
        [Required]
        public string ReceiverCountry { get; set; }
        public DateTime CareatedDate { get; set; }
        public int CreatedByUserId { get; set; }
        public bool IsBlocked { get; set; }
        public string  TransferMethodName{ get; set; }
        [Required]
        public TransactionTransferMethod TransferMethod { get; set; }
        [Required]
        public string ReceiverTelephone { get; set; }
        public string BankCode { get; set; }
        public string BankNameOrMobileWalletProvider { get; set; }
        public bool IsDeleted { get; set; }
    }
}