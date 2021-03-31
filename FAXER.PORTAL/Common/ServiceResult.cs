using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public class ServiceResult<T>
    {
        public ResultStatus Status {  get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public bool IsCardUsageMsg { get; set; }
        public string CardUsageMessage { get; set; }
        public bool IsLimitMsg { get; set; }
        public string Token { get; set; }

        public bool IsGetType3dAuth { get; set; }



    }

}