using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels
{
    public class SearchParamVM
    {
        public string SendingCountry { get; set; }
        public string City { get; set; }
        public string Date { get; set; }
        public string AgentName { get; set; }
        public string SenderName { get; set; }
        public string AccountNo { get; set; }
        public string LoginCode { get; set; }
        public string Telephone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

    }

}