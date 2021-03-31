using FAXER.PORTAL.Areas.Mobile.Models.MobilePaymentSummary;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex.BankDeposit
{
    public class MobilePaymentBankDepositVm
    {
        public int SenderId { get; set; }
        public int TransactionId { get; set; }
        public string ReceiptNo { get; set; }
        public bool IsIdCheckIsProgress { get; set; }
        public MobilePaymentWalletReceiverVm MobileWallet { get; set; }
        public MobilePaymentBankDepositReceiverVm ReceiverDetail { get; set; }
        public MobilePaymentSummaryVm PaymentSummary { get; set; }
        public MobilePaymentPaymentMethodVm PaymentMethodDetail { get; set; }
    }


    public class MobilePaymentWalletReceiverVm {


        public string CountryCode { get; set; }
        public string ReceiverName { get; set; }
        public int WalletId { get; set; }

        public string WalletName { get; set; }
        public int IdentificationTypeId { get; set; }
        public string IdentificationNumber { get; set; }
        public string WalletNo { get; set; }
        public string ReasonForTransfer { get; set; }
        public ReasonForTransfer ReasonForTransferEnum { get; set; }

    }
}