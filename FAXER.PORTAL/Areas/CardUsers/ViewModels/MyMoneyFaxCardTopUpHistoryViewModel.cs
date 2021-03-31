using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.CardUsers.ViewModels
{
    public class MyMoneyFaxCardTopUpHistoryViewModel
    {

        public int TransactionId { get; set; }
        public string FaxerName { get; set; }   
        public string FaxerCountry { get; set; }
        public string FaxerCity { get; set; }

        public string TopUpAmount { get; set; }
        public string TopUpReference { get; set; }

        public string Date { get; set; }

        public string Time { get; set; }

        public TopUpBy TopUpBy { get; set; }
        public DateTime TransactionDate { get; set; }

    }

    public enum TopUpBy {

        Registar,
        Sender,
        CardUser,
        Service_provider,
    }
}