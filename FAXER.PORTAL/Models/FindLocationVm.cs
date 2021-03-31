using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class FindLocationVm
    {
        public const string BindProperty = "Country ,City,ServiceType,PostalCode, ServiceTypeDetails ";


        public string Country { get; set; }
        public string City { get; set; }
        public AgentType? ServiceType { get; set; }

        public string PostalCode { get; set; }

        public List<ServiceTypeDetailVm> ServiceTypeDetails { get; set; }


    }


    /// <summary>
    /// Agent And Business Merchant
    /// </summary>
    public class ServiceTypeDetailVm {



        // AgentName or BusinessName
        public string BusinessName { get; set; }

        public string BusinessType { get; set; }
        public string Address { get; set; }

        public string City { get; set; }
        public string PostalCode { get; set; }
        public string State { get; set; }

        public string Country { get; set; }

        public string EmailAddress { get; set; }
        public string PhoneCode { get; set; }
        
        public string Telephone { get; set; }
        public string Website { get; set; }

    }
}