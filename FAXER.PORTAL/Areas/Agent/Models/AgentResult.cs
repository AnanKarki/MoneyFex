using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentResult
    {
        public ResultStatus Status { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
    }
    //public enum ResultStatus
    //{
    //    Default,
    //    OK,
    //    Warning,
    //    Error,
    //    Info
    //}
}