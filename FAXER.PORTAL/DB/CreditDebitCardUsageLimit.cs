using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class CreditDebitCardUsageLimit
    {

        [Key]
        public int Id { get; set; }

        public CrediDebitCardUsageFrequency Frequency { get; set; }
        public int frequencydetials { get; set; }
        /// <summary>
        /// Count 
        /// </summary>
        public int UsageLimit { get; set; }


    }
    public enum CrediDebitCardUsageFrequency {

        Daily,
        Weekly,
        Monthly,
        Yearly
    }

    public class CreditCardUsageLog {

        public int Id { get; set; }
        public int SenderId { get; set; }
        public string CardNum { get; set; }
        public Module Module { get; set; }
        public int Count { get; set; }
        public DateTime UpdatedDateTime { get; set; }
    }

    public class CreditCardAttemptLimit {

        [Key]
        public int Id { get; set; }
        public CrediDebitCardUsageFrequency Frequency { get; set; }
        /// <summary>
        /// Count 
        /// </summary>
        public int AttemptLimit { get; set; }
        public string  SendingCountry { get; set; }
        public string  ReceivingCountry { get; set; }
        public int?  SenderId{ get; set; }
    }

    public class CreditCardAttemptLog
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public string CardNum { get; set; }
        public Module Module { get; set; }
        public string Message { get; set; }
        public CreditCardAttemptStatus CreditCardAttemptStatus { get; set; }
        public DateTime AttemptedDateTime { get; set; }

    }
    public enum CreditCardAttemptStatus {

        Success,
        Failure
    }
}