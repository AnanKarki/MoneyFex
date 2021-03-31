using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class UploadMFBCardPhotoViewModel
    {
        public const string BindProperty = "Id , Name ,CardNum ,BusinessName , PhotoURL";

        public int Id { get; set; }
        public string Name { get; set; }
        public string CardNum { get; set; }
        public string BusinessName { get; set; }
        public string PhotoURL { get; set; }
    }
}