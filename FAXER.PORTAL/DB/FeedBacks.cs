using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class FeedBacks
    {
        public int Id { get; set; }
        public string Country { get; set; }
        public Platform Platform { get; set; }
        public CustomerType CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string FeedBack { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

    }
    public enum Platform
    {
        [Display(Name = "Select Platfrom")]
        Select,
        [Display(Name = "Web")]
        Website,
        Facebook,
        YouTube,
        Twitter,
        Linkedin,
        Instagram,

    }
    public enum CustomerType
    {
        [Display(Name = "Select Type")]
        [Description("Select Type")]
        Select,
        [Display(Name = "Money Sender")]
        [Description("Money Sender")]
        MoneySender,
        [Display(Name = "Money Receiver")]
        [Description("Money Receiver")]
        MoneyReceiver,
        [Display(Name = "Agent Partner")]
        [Description("Agent Partner")]
        AgentPartner,
        [Display(Name = "Business Owner")]
        [Description("Business Owner")]
        BusinessOwner
    }
}