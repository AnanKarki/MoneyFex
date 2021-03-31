using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewMFTCCardWithdrawalViewModel
    {
        public int Id { get; set; }
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        public string FaxerLastName { get; set; }
        public string FaxerFullName { get; set; }
        public string CardUserFirstName { get; set; }
        public string CardUserMiddleName { get; set; }
        public string CardUserLastName { get; set; }
        public string CardUserFullName { get; set; }
        public string CardUserTelephone { get; set; }
        public string ReceiverIDType { get; set; }
        public string ReceiverIDNumber { get; set; }
        public string ReceiverIDExpDate { get; set; }
        public string ReceiverIDIssuingCountry { get; set; }
        public string MFTCCardNumber { get; set; }
        public decimal AmountOnMFTCCard { get; set; }
        public string Currency { get; set; }
        public decimal WithdrawalAmount { get; set; }
        public string WithdrawalTime { get; set; }
        public string WithdrawalDate { get; set; }
        public string AgentVerifier { get; set; }
        public string AgentName { get; set; }
        public string AgentMFSCode { get; set; }
        public string PaymentRejection { get; set; }
        public string ReceiptNumber { get; set; }

    }

    public class ViewMFTCCardPurchaseViewModel
    {
        public int Id { get; set; }

    }

    public class ViewMFTCCardUsageViewModel
    {
        public ViewMFTCCardUsageViewModel()
        {
            ViewMFTCCardPurchases = new List<ViewMFTCCardPurchaseViewModel>();
            ViewMFTCCardWithdrawals = new List<ViewMFTCCardWithdrawalViewModel>();
        }
        public List<ViewMFTCCardWithdrawalViewModel> ViewMFTCCardWithdrawals { get; set; }
        public List<ViewMFTCCardPurchaseViewModel> ViewMFTCCardPurchases { get; set; }
    }
}