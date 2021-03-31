using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class BankAccountCreditViewModel
    {
        public const string BindProperty = "AgentName , AgentAccountNumber ,AgentCountry,City , CurrentCustomerDeposit ,CurrentCustomerDepositFees,CurrenctBankDeposit , NewAccountDeposit ," +
         "NameOfUpdater,LatestTransactionDateTime, NameOfLatestUpdater , TotalAmountDeposited,Currency,CurrencySymbol ";

        public string AgentName { get; set; }
        public string AgentAccountNumber { get; set; }
        public string AgentCountry { get; set; }
        public string City { get; set; }
        public decimal CurrentCustomerDeposit { get; set; }

        public decimal CurrentCustomerDepositFees { get; set; }
        public decimal CurrenctBankDeposit { get; set; }
        public decimal NewAccountDeposit { get; set; }
        public string NameOfUpdater { get; set; }

        #region BankAccountCreditDetails

        public DateTime LatestTransactionDateTime { get; set; }
        public string NameOfLatestUpdater { get; set; }

        public string TotalAmountDeposited { get; set; }
        #endregion

        public string Currency { get; set; }

        public string CurrencySymbol { get; set; }
    }


}