using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class UserBankAccount
    {

        public int Id { get; set; }
        public int UserId { get; set; }
        public int BankId { get; set; }
        public int AddressId { get; set; }
        public int BankBranchId { get; set; }
        public string CountryCode { get; set; }
        public string BranchCode { get; set; }
        public Module BankAccountOf { get; set; }
        public BankAccountStatus BankAccountStatus { get; set; }
        public string  AccountOwnerName { get; set; }
        public string AccountNo { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    public enum BankAccountStatus {

        Active,
        InActive
    }
}