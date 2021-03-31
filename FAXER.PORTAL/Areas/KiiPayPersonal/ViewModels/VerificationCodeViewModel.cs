using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class VerificationCodeViewModel
    {
        public const string BindProperty = "Id ,Code1 ,Code2 , Code3 , Code4 ,Code5 ,Code6";
        public int Id { get; set; }
        public int Code1 { get; set; }
        public int Code2 { get; set; }
        public int Code3 { get; set; }
        public int Code4 { get; set; }
        public int Code5 { get; set; }
        public int Code6 { get; set; }
    }
}