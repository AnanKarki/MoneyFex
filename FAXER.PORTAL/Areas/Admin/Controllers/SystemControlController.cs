using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.Rest.Trunking.V1;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class SystemControlController : Controller
    {
        // GET: Admin/SystemControl
        public ActionResult Agent()
        {
            return View();
        }

        public ActionResult Sender()
        {
            return View();
        }
        
        public ActionResult Business()
        {
            return View();
        }

        public ActionResult Admin()
        {
            return View();
        }
    }
}