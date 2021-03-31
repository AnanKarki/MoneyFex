using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessResult
    {

        public ResultStatus Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }

    }
}