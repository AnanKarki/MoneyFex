using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class PayAReceiverKiiPayWalletViewModel
    {
        public const string BindProperty = "Id , FirstName ,MiddleName,LastName , WalletNo ,DOB,IdType , IdNumber ," +
         "ExpiryDate,CountryCode , IssuingCountry,AddressLine1,AddressLine2 , PostCode,Country ,MobileNo , PhoneCode ,Email," +
          "WalletStatus ,WalletStatusName ,CashLimitType , CashWithdrawalLimit, AgentId ,KiiPayWalletType";
        public int Id { get; set; }
        [Required(ErrorMessage = "Enter First Name")]
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        [Required(ErrorMessage = "Enter Middle Name")]
        public string LastName { get; set; }
        public string WalletNo { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DOB { get; set; }
        [Required(ErrorMessage = "Enter IdType")]
        public int IdType { get; set; }
        [Required(ErrorMessage = "Enter Id Number")]
        public string IdNumber { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpiryDate { get; set; }

        [Required(ErrorMessage = "Select Country")]
        public string CountryCode { get; set; }

        [Required(ErrorMessage = "Select Issuing Country")]
        public string IssuingCountry { get; set; }

        [Required(ErrorMessage = "Enter Address")]
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }

       
        public string PostCode { get; set; }

        public string Country { get; set; }

        [Required(ErrorMessage = "Enter Mobile Number")]
        public string MobileNo { get; set; }

        public string PhoneCode { get; set; }

        //[Required(ErrorMessage = "Enter Email")]
        //[DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }
               

        public CardStatus WalletStatus { get; set; }
        public string WalletStatusName { get; set; }
        public CardLimitType CashLimitType { get; set; }
        public decimal CashWithdrawalLimit { get; set; }
        public int AgentId { get; set; }

        public KiiPayWalletType KiiPayWalletType { get; set; }
    }

    public enum KiiPayWalletType
    {
        Personal,
        Business
    }

    public class IdTypeDropDownVm
    {
        public int Id    { get; set; }
        public string CardType { get; set; }

    }
}