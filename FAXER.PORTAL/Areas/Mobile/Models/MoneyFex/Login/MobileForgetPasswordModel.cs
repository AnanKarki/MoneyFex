using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Models.MoneyFex.Login
{
    public class MobileForgetPasswordModel
    {
        public string OtpCode { get; set; }
        public int SenderId { get; set; }
    }
}