using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class SetNewRateViewModel
    {
        public const string BindProperty = "Id , SourceCountry ,DestinationCountry, SourceCountryCode ,DestinationCountryCode, Rate ";

        public int Id { get; set; }
        public string SourceCountry { get; set; }
        public string DestinationCountry { get; set; }
        public string SourceCountryCode { get; set; }

        public string DestinationCountryCode { get; set; }

        public decimal Rate { get; set; }

    }

}