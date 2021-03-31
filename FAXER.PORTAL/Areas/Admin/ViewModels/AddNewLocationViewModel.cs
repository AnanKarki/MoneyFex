using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AddNewLocationViewModel
    {
        public const string BindProperty = "Id , Country ,City ,AgentType , AgentId , Address ,ContactNo";

        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public AgentType AgentType { get; set; }
        public int AgentId { get; set; }
        public string Address { get; set; }
        public string ContactNo { get; set; }
    }

    public enum AgentType
    {
        [Display(Name ="Paying Agent")]
        Agent = 1, //1

        [Display(Name = "Business")]
        ServiceProvider = 2 //2
    }

    public enum RegisteredAgentType { 
    
        Agent,
        AuxAgent
    }
}