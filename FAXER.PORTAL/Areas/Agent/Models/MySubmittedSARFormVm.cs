using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{

    public class MySubmittedSARFormVm {

        public Month month { get; set; }
        public string Year { get; set; }
        public List<MySubmittedSARFormDetialsVm> MySubmittedSARFormDetials { get; set; }


    }
    public class MySubmittedSARFormDetialsVm
    {

        public int TransactionId { get; set; }

        public string Month { get; set; }
        public string Year { get; set; }
        public string AgentName { get; set; }
        public string AgentAccountNo { get; set; }
        /// <summary>
        /// Staff Login Code
        /// </summary>
        public string StaffId { get; set; }

        public string CustomerName { get; set; }

    }
}