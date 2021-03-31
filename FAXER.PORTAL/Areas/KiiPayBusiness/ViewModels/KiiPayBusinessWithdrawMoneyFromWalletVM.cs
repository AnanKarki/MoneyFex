using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessWithdrawMoneyFromWalletVM
    {
    }
    public class KiiPayBusinessUserBanckAccountsVM
    {
        public  const string BindProperty= "BankId ,Name ,AccountNumber ,IsChecked";
        public int BankId { get; set; }
        public string Name { get; set; }
        public string AccountNumber { get; set; }

        public bool IsChecked { get; set; }


    }

    public class WithdrawMoneyFromWalletEnterAmountVM
    {
        public const string BindProperty = "Amount ,CurrencyCode";
        [Range(1, int.MaxValue, ErrorMessage = "Please enter the amount greater than {0}")]
        public decimal Amount { get; set; }
        public string CurrencyCode { get; set; }

    }

    public class WithdrawMoneyFromWalletSummaryVM
    {
        public const string BindProperty = "FromWallet ,MobileCode ,ToBankAccount , ToBankName, Amount, CurrencyCode, Fee , TotalReceiveAmount";
        public string FromWallet { get; set; }
        [Required(ErrorMessage = "Please enter the Mobile Code.")]
        public string MobileCode { get; set; }
        public string ToBankAccount { get; set; }
        public string ToBankName{ get; set; }
        public decimal Amount{ get; set; }
        public decimal CurrencyCode{ get; set; }
        public decimal Fee{ get; set; }
        public decimal TotalReceiveAmount { get; set; }
    }

    public class WithdrawMoneyFromWalletSuccessVM
    {
        public decimal Amount { get; set; }
        public string BankName { get; set; }

    }

    public class WithdrawMoneyFromWalletAddBankAccountVM
    {
        public const string BindProperty = "Country ,AccountUserName ,AccountNumber,BankName , BranchCode, BranchName, Address, MobileCode";

        [Required(ErrorMessage = "Please enter the Country.")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Please enter the AccountUserName.")]
        public string AccountUserName { get; set; }
        [Required(ErrorMessage = "Please enter the AccountNumber.")]
        public string AccountNumber { get; set; }
        // [Required(ErrorMessage = "Please enter the BankName.")]
        public string BankName { get; set; }
        public string BranchCode { get; set; }
        public string BranchName { get; set; }
        public string Address { get; set; }
        [Required(ErrorMessage = "Please enter the MobileCode.")]
        public string MobileCode { get; set; }
    }
}