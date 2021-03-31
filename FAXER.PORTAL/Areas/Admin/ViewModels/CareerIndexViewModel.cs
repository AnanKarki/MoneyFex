using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class CareerIndexViewModel
    {
        public int Id { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ContractType { get; set; }
        public string CountrySymbol { get; set; }
        public string SalaryRange { get; set; }

        public string ClosingDate { get; set; }
        public string PublishDate { get; set; }
    }
}