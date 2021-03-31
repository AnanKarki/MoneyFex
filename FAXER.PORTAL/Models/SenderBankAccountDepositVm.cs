using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderBankAccountDepositVm
    {

        public const string BindProperty = "Id ,walletId,CountryCode , RecentAccountNumber ,AccountOwnerName , " +
            "CountryPhoneCode , MobileNumber , AccountNumber ,BankId , BranchId , BranchCode , IsManualDeposit , IsBusiness , " +
            "ReasonForTransfer , IsEuropeTransfer , BankName , IsSouthAfricaTransfer  ,ReceiverStreet ,ReceiverPostalCode ,ReceiverEmail,ReceiverCity,SenderId," +
            "IdenityCardId,IdentityCardNumber ";


        [Range(0, int.MaxValue)]
        public int Id { get; set; }
        [Range(0, int.MaxValue)]
        public int walletId { get; set; }
        [Range(0, int.MaxValue)]
        public int ReceipientId { get; set; }

        [Required(ErrorMessage = "Select Country")]
        public string CountryCode { get; set; }

        [StringLength(20)]
        public string RecentAccountNumber { get; set; }

        [Required(ErrorMessage = "Enter owner name")]
        //[RegularExpression("^[a-zA-Z ]*$", ErrorMessage = "No numbers and special characters allowed")]
        public string AccountOwnerName { get; set; }
        [MaxLength(20)]
        public string CountryPhoneCode { get; set; }

        public string MobileNumber { get; set; }



        /// <summary>
        /// If Transfer To Europe used as IBAN Number
        /// </summary>
        [Required(ErrorMessage = "Enter account number")]
        public string AccountNumber { get; set; }


        //[Required(ErrorMessage = "Select Bank")]
        public int BankId { get; set; }


        public int? BranchId { get; set; }

        /// <summary>
        /// If Transfer To Europe used as BIC/Swift code
        /// </summary>
        /// 
        //[Required(ErrorMessage = "Enter code")]

        public string BranchCode { get; set; }

        public bool IsManualDeposit { get; set; }

        public bool IsBusiness { get; set; }
        public ReasonForTransfer ReasonForTransfer { get; set; }

        public bool IsEuropeTransfer { get; set; }



        /// <summary>
        /// Used for Europe bank transfer
        /// </summary>
        public string BankName { get; set; }
        public int TransactionSummaryId { get; set; }

        /// <summary>
        /// Used for South african countries bank transfer
        /// </summary>
        public bool IsSouthAfricaTransfer { get; set; }

        public bool IsWestAfricaTransfer { get; set; }
        public string ReceiverStreet { get; set; }
        public string ReceiverPostalCode { get; set; }
        public string ReceiverEmail { get; set; }
        public string ReceiverCity { get; set; }
        public int IdenityCardId { get; set; }
        public int SenderId { get; set; }
        public string IdentityCardNumber { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
    }
}