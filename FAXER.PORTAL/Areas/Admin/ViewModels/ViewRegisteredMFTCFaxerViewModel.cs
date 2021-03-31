using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ViewRegisteredMFTCFaxerViewModel
    {
        public int Id { get; set; }
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        public string FaxerLastName { get; set; }
        public string FaxerAddress { get; set; }
        public string FaxerCountry { get; set; }
        public string FaxerCity { get; set; }
        public string FaxerTelephone { get; set; }
        public string FaxerEmail { get; set; }
        

    }
}