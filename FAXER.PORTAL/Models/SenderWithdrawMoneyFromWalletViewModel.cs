using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderWithdrawMoneyFromWalletViewModel
    {
        public const string BindProperty = "AvailableCurrency,CardHolderName, AvailableBalance , Banks";
        [MaxLength(200)]
        public string AvailableCurrency { get; set; }
        [MaxLength(200)]
        public string CardHolderName { get; set; }
        [Range(0.0, double.MaxValue)]
        public decimal AvailableBalance { get; set; }
        public List<SavedBankDetailsViewModel> Banks { get; set; }
        
    }

    public class SavedBankDetailsViewModel
    {
        public int BankId { get; set; }
        public string BankName { get; set; }
        public string AccountNumber { get; set; }
        public string FormattedAccNo { get; set; }
        public bool IsChecked { get; set; }

    }
}