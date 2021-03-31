using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SocialMediaTrackingViewModel
    {
        public const string BindProperty = " Id,Services ,ApplicationType ,TrackingPage ,TrackingCode , CreatedDate,CreatedBy ";
        public int Id { get; set; }
        [Required(ErrorMessage ="select Services")]
        public string Services { get; set; }
        [Required(ErrorMessage = "select Application")]
        public ApplicationType ApplicationType { get; set; }
        [Required(ErrorMessage = "select Tracking Page")]
        public string TrackingPage { get; set; }
        [AllowHtml]
        [Required(ErrorMessage = "Enter Tracking Code")]
        public string TrackingCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }

    }
}