using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class HomeViewModel
    {

        public List<ExchangeRateVm> ExchangeRates { get; set; }

        public List<CountryViewModel> Countries { get; set; } 
        public List<FeedBackViewModel> Feedbacks { get; set; }

        public List<string> PartnersLogo { get; set; }
        [Range(0.0, Double.MaxValue)]
        public decimal SendingAmount { get; set; }
        [Range(0.0, Double.MaxValue)]
        public decimal ReceivingAmount { get; set; }
        [Range(0.0, Double.MaxValue)]
        public decimal Fee { get; set; }
        [Range(0.0, Double.MaxValue)]
        public decimal TotalAmount { get; set; }
        [Range(0.0, Double.MaxValue)]
        public decimal ExchangeRate { get; set;  }

    }
    public class CountryViewModel
    {

        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Currency { get; set; }

        public string FlagCountryCode { get; set; }
        public string CountryWithCurrency { get; set; }


    }

    public class ExchangeRateVm {

        public string SendingCountryCurrency { get; set; }
        public string ReceivingCountryCurrency { get; set; }

        public string SendingFlagCode { get; set; }

        public string ReceivingFlagCode { get; set; }

        public string ExchangeRate { get; set; }

    }

    
}