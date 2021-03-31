using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class EmailCredentialVm
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }

    public enum TransactionEmailType 
    { 
        CustomerSupport,
        Rates,
        WelcomeCustomer,
        TransactionCancelled,
        IDCheck,
        TransactionCompleted,
        TransactionInProgress,
        TransactionPending
    }
}