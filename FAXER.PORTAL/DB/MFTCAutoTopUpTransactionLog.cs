using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class MFTCAutoTopUpTransactionLog
    {

        public int Id { get; set; }

        public int FaxerID { get; set; }

        public int MFTCCardID { get; set; }

        public decimal AutoTopUpAmount { get; set; }

        public MFTCAutoTopUpType MFTCAutoTopUpType { get; set; }

        public DateTime TransactionDate { get; set; }


    }
    public enum MFTCAutoTopUpType
    {
        MyCard,
        SomeoneElseCard,

    }
}