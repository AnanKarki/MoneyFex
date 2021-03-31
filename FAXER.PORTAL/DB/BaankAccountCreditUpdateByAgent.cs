using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class BaankAccountCreditUpdateByAgent
    {
        public int Id { get; set; }
        public int AgentId { get; set; }

        // Only Applicable for Customer deposit
        public int? NonCardTransactionId { get; set; }
        public string NameOfUpdater { get; set; }
        public decimal CustomerDeposit { get; set; }

        public decimal CustomerDepositFees { get; set; }

        public decimal BankDeposit { get; set; }

        public string ReceiptNo { get; set; }

        public DateTime CreatedDateTime { get; set; }


    }
}