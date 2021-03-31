using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AddNewNewsViewModel
    {
        public const string BindProperty = "Id , Headline ,FullNews , Image, OldImagePath";

        public int Id { get; set; }
        public string Headline { get; set; }
        [AllowHtml]
        public string FullNews { get; set; }
        public string Image { get; set; }
        public string OldImagePath { get; set; }
    }
}