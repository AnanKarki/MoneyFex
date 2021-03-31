using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class RegisteredAUXAgentViewModel
    {
        public string Country { get; set; }
        public string CountryCode { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        public string AgentName { get; set; }
        public string AccountNo { get; set; }
        public string LoginCode { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        [Required(ErrorMessage = "Enter Telephone Number")]
        public string Telephone { get; set; }
        [Required(ErrorMessage = "Enter Email")]
        public string Email { get; set; }
        public string BusinessType { get; set; }
        public BusinessType TypeOfBusiness { get; set; }
        public string Date { get; set; }
        public string DOB { get; set; }
        public DateTime? CreationDate { get; set; }
        public string StatusName { get; set; }
        public int Id { get; set; }
        public AgentStatus AgentStatus { get; set; }
        public string Gender { get; set; }
    }
}