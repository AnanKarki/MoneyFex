using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderAddNewBankVM
    {
        public const string BindProperty = "Id,CountryCode,OwnerName,AccountNumber,BankId,BranchCode,BranchId," +
            "Address,MobileCode,PinCode,UserEnterPinCode,BankAccountStatus";
        [Range(0 , int.MaxValue)]
        public int Id { get; set; }
        [MaxLength(200)]
        public string CountryCode { get; set; }
        [Required(ErrorMessage ="Enter Owner Name")]
        public string OwnerName { get; set; }
        [Required(ErrorMessage = "Enter Account Number")]
        public string AccountNumber { get; set; }
        [Range(0 , int.MaxValue)]
        public int BankId { get; set; }
        [Required(ErrorMessage = "Enter Branch Code")]
        public string BranchCode { get; set; }
        [Range(0, int.MaxValue)]
        public int BranchId { get; set; }
        [MaxLength(200)]
        public string Address { get; set; }
        [MaxLength(200)]
        public string MobileCode { get; set; }
        [MaxLength(200)]
        public string PinCode { get; set; }
        [MaxLength(200)]
        public string UserEnterPinCode { get; set; }
        [Range(0 , int.MaxValue)]
        public BankAccountStatus BankAccountStatus { get; set; }

    }

    public class BankVm
    {
        public string Code { get; set; }
        public string Name { get; set; }

    }

    public class BranchVm
    {
        public string Code { get; set; }
        public string Name { get; set; }

    }
    public enum BankAccountStatus
    {

        Active,
        InActive
    }
}