using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AddNewBusinessAlertViewModel
    {
        public const string BindProperty = "Id , Heading ,FullMessage ,Photo ,Country , City , Business ,BusinessName , PublishedDate, EndDate,StartDate";

        public int Id { get; set; }
        public string Heading { get; set; }
        public string FullMessage { get; set; }
        public string Photo { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int Business { get; set; }
        public string BusinessName { get; set; }
        public DateTime PublishedDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? EndDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? StartDate { get; set; }
        public string BusinessAccountNo { get; set; }
    }
}