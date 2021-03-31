using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderAddMoneyToWalletSavedCardViewModel
    {
        public const string BindProperty = "AvailableBalance,SendingCurrencySymbol,Amount,CardDetails";

        public decimal AvailableBalance { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public decimal Amount { get; set; }


        //public string SavedCredit { get; set; }
        //public string Credit { get; set; }

        public List<SenderSavedDebitCreditCard> CardDetails { get; set; }
      
    }
    
    public class SenderSavedDebitCreditCard
    {
        
        [Range(0 , 2)]
        public CreditDebitCardType CreditDebitCardType { get; set; }
        [Range(0 , int.MaxValue)]
        public int CardId { get; set; }
        [StringLength(200)]
        public string CardHolderName { get; set; }
        [StringLength(16)]
        public string CardNumber { get; set; }
        [StringLength(16)]
        public string DecryptedCardNumber { get; set; }

        public bool IsChecked { get; set; }
        [StringLength(3)]
        public string SecurityCode { get; set; }
        [StringLength(2)]
        public string ExpiringDateMonth { get; set; }
        [StringLength(4)]
        public string ExpiringDateYear { get; set; }

    } public class SenderSavedDebitCreditCardViewModel
    {

        public const string BindProperty = "CardId ,CardNo ,Address ,CardStatus , CVVCode , ExpMonth" +
             ", ExpDate , ExpYear ,SenderId ,CardType,CurrencyCode";

        public int CardId { get; set; }
        public string CardNo { get; set; }
        public string Address { get; set; }
        public string CardStatus { get; set; }
        public string CVVCode { get; set; }
        public string ExpMonth { get; set; }
        public string ExpDate { get; set; }
        public string ExpYear { get; set; }
        public int SenderId { get; set; }
        public CreditDebitCardType CardType { get; set; }
        public string CurrencyCode { get;  set; }
    }


}