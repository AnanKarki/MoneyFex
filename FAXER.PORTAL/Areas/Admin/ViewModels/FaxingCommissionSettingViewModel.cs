using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class FaxingCommissionSettingViewModel
    {
        public const string BindProperty = "Id , Code ,Continent , Commission";

        public int Id { get; set; }
        public string Code { get; set; }
        public string Continent { get; set; }
        public decimal? Commission { get; set; }
    }
}