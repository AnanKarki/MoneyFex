using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace FAXER.PORTAL.Controllers
{
    public class TestApiController : ApiController
    {

        [HttpGet]
        public ServiceResult<string> GetSampleData() {

            return new ServiceResult<string>()
            {
                Data = "Data Successfully"
            };
        }
    }
}
