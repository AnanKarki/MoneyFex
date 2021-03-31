using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class CalculateHowMuchViewModel
    {
        public const string BindProperty = " AgentCountryCode , ReceivingCountryCode ,AgentAmount,ReceivingAmount , AgentCurrencySymbol ,ReceivingCurrency,ReceivingCurrencySymbol , IsReceivingAmount ";

        public string AgentCountryCode { get; set; }
        public string AgentCurrency { get; set; }
        [Required,DisplayName("Receiving Country")]
        public string ReceivingCountryCode { get; set; }
        public decimal AgentAmount { get; set; }
        public string ReceivingAmount { get; set; }
        public string AgentCurrencySymbol { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public bool IsReceivingAmount { get; set; }
       

    }
}