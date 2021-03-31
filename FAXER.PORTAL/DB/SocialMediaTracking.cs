using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class SocialMediaTracking
    {
        public int Id { get; set; }
        public string Services { get; set; }
        public ApplicationType ApplicationType { get; set; }
        public string TrackingPage { get; set; }
        public string TrackingCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
    }

    public enum ApplicationType
    {
        [Display(Name = "Select Type")]
        Select,
        [Display(Name = "Web")]
        Web,
        [Display(Name = "Mobile App")]
        MobileApp
    }
}