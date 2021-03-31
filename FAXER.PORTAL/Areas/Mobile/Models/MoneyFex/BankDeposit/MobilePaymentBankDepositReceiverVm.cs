using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex.BankDeposit
{

    public class MobilePaymentBankDepositReceiverVm
    {
        public int ReceiverId { get; set; }
        public string ReceiverName { get; set; }
        public string MobileNo { get; set; }
        public string ReceiverAccountNo { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }

        /// <summary>
        /// 
        /// Bank Code  // SwiftCode
        /// </summary>
        public string BranchORBankORSwiftCode { get; set; }

        public bool IsBusiness { get; set; }

        public ReasonForTransfer ReasonForTransfer { get; set; }
        public string ReceiverAddress { get; set; }
        public string ReceiverPostCode { get; set; }
        public string ReceiverCity { get; set; }
        public string ReceiverEmailAddress { get; set; }

        public int RecipientIdentityCardId { get; set; }
        public string RecipientIdenityCardNumber { get; set; }
        public string BICSwift { get; set; }
        public string IBAN { get; set; }


    }
}