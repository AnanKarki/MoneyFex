using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AddPublicHolidayViewModel
    {
        public const string BindProperty = "Id ,Country ,City , HolidayName ,FromDate ,ToDate";
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string HolidayName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? FromDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? ToDate { get; set; }
    }
}