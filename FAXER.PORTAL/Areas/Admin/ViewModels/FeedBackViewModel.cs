using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class FeedBackViewModel
    {
        public const string BindProperty = " Id, Country,Platform , PlatformName, CustomerType,CustomerTypeName , CustomerName ,FeedBack ,CreatedDate, CreatedBy,CustomerFirstName";
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        public string CountryFlag { get; set; }
        [Required(ErrorMessage = "Select Platform")]
        public Platform Platform { get; set; }
        public string PlatformName { get; set; }
        [Required(ErrorMessage = "select Customer Type")]
        public CustomerType CustomerType { get; set; }
        public string CustomerTypeName { get; set; }
        [Required(ErrorMessage = "Enter Customer name")]
        public string CustomerName { get; set; }
        [Required(ErrorMessage = "Enter Feedback")]
        public string FeedBack { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string CustomerFirstName{ get; set; }
    }
}