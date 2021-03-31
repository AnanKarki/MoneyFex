using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class LogInViewModel
    {
        public const string BindProperty = " Id , UserName ,Password";

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}