using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{

    public class MFTCCardUsageHistoryViewModel
    {

        public MFTCCardUsageHistoryViewModel()
        {
            MFTCCardTopUpViewModel = new List<MFTCCardTopUpHistoryViewModel>();
            MFTCCardwithdrawlViewModel = new List<MFTCCardCashwithDrawlHistoryViewModel>();
            MFTCCardBusinessMerchantPaymentViewModel = new List<MFTCCardMerchantPaymentHistoryViewModel>();

        }
        public List<MFTCCardTopUpHistoryViewModel> MFTCCardTopUpViewModel { get; set; }
        public List<MFTCCardCashwithDrawlHistoryViewModel> MFTCCardwithdrawlViewModel { get; set; }

        public List<MFTCCardMerchantPaymentHistoryViewModel> MFTCCardBusinessMerchantPaymentViewModel { get; set; }
        public int MFTCCardId { get; set; }

        public string MFTCCardNumber { get; set; }

        public string MFTCCardCurrency { get; set; }
        public string MFTCCardCurrencySymbol { get; set; }

        public string CurrentBalance { get; set; }

        public CardUsageHistoryOption CardHistoryOption { get; set; }

    }

    public class MFTCCardTopUpHistoryViewModel
    {

        public List<MFTCCardCashwithDrawlHistoryViewModel> List { get; set; }
        public decimal TopUpAmount { get; set; }

        public string FaxerCurrency { get; set; }

        public string ReceivingCurrency { get; set; }


        public string TopUpDate { get; set; }

        public string TopUpTime { get; set; }
        public DateTime TransationDate { get; set; }

        public string TopUpReference { get; set; }
    }
    public class MFTCCardCashwithDrawlHistoryViewModel
    {

        public decimal WithdrawlAmount { get; set; }

        public string withdrawlCurrency { get; set; }
        public string WithdrawlDate { get; set; }

        public string WithdrawlTime { get; set; }
        public string AgentName { get; set; }

        public string AgentLocation { get; set; }
        public string AgentMFCode { get; set; }
    }

    public class MFTCCardMerchantPaymentHistoryViewModel
    {

        public string FaxingCurrency { get; set; }
        public decimal PaymentAmount { get; set; }

        public string BusinessLocation { get; set; }

        public string BusinessMerchantName { get; set; }

        public string BusinessMFCode { get; set; }

        public string PaymentDate { get; set; }
        public string PaymentTime { get; set; }

        public DateTime TransactionDateTime { get; set; }
    }

    public enum CardUsageHistoryOption
    {
        [Display(Name = "Select Virtual Account Usage History Option")]
        Select_Card_Usage_History_Option,
        [Display(Name = "Deposit History")]
        TopUp,
        [Display(Name = "Withdrawal History")]
        Card_withdrawl,
        [Display(Name = "Purchase and Payment History")]
        Card_Purchase,

    }


}