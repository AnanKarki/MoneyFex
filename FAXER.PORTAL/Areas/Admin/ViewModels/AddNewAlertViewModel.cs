using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AddNewAlertViewModel
    {
        public const string BindProperty = "Id , Heading ,FullMessage ,Photo ,Country , City, Agent,AgentName , PublishedDate, EndDate,StartDate,Module,AccountNo";

        public int Id { get; set; }
        public string Heading { get; set; }
        public string FullMessage { get; set; }
        public string Photo { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public int Agent { get; set; }
        public string AgentName { get; set; }
        public DateTime PublishedDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? EndDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? StartDate { get; set; }

        public Module Module { get; set; }
        public string  AccountNo{ get; set; }
    }
}