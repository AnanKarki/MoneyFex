using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class AdminResult
    {
        
            public AdminResultStatus Status { get; set; }
            public string Message { get; set; }
            public object Data { get; set; }
        
       
    }
    public enum AdminResultStatus
    {
        Default,
        OK,
        Warning,
        Error,
        Info
    }
}