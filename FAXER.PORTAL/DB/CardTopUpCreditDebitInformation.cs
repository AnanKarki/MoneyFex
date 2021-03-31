
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class CardTopUpCreditDebitInformation
    {
        [Key]
        public int Id { get; set; }
        public int? CardTransactionId { get; set; }
        public string NameOnCard { get; set; }

        public string CardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public bool IsSavedCard { get; set; }
        public bool AutoRecharged { get; set; }
        public int TransferType { get; set; }
        public DateTime CreatedDate { get; set; }


        public int? NonCardTransactionId { get; set; }
        public int? TopUpSomeoneElseTransactionId { get; set; }
        //public virtual SenderKiiPayPersonalWalletPayment CardTransaction { get; set; }
        //public virtual FaxingNonCardTransaction NonCardTransaction { get; set; }
        //public virtual TopUpSomeoneElseCardTransaction TopUpSomeoneElseTransaction { get; set; }
    }


}