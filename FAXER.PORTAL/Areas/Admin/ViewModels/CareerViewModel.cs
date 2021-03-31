using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class CareerViewModel
    {
        public const string BindProperty = "Id , JobTitle ,Image,Description ,Country , City, ContractType, SalaryRange,CountryCurrency ,ClosingDate ,ClosingTime";

        public int Id { get; set; }
        public string JobTitle { get; set; }

        public string Image { get; set; }
        public string Description { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ContractType { get; set; }
        public string SalaryRange { get; set; }
        public string CountryCurrency { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? ClosingDate { get; set; }


    }
}