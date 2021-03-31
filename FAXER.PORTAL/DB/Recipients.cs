using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class Recipients
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public Service Service { get; set; }
        public string ReceiverName { get; set; }
        public string Country { get; set; }
        public string MobileNo { get; set; }
        public int BankId { get; set; }
        public string AccountNo { get; set; }
        public string BranchCode { get; set; }
        public ReasonForTransfer Reason { get; set; }
        public int MobileWalletProvider { get; set; }
        public bool IBusiness { get; set; }
        public bool IsBanned { get; set; }
        public string Email { get; set; }
        public string PostalCode { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public int IdentificationTypeId { get; set; }
        public string IdentificationNumber { get; set; }
    }
    public enum Service
    {
        [Description("Select")]
        Select = 4,
        [Description("Bank Account")]
        BankAccount = 0,
        [Description("Mobile Wallet")]
        MobileWallet = 1,
        [Description("Cash Pickup")]
        CashPickUP = 2,
        [Description("KiiPay Wallet")]
        KiiPayWallet = 3
    }
}