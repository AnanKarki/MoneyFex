using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessPayBillsController : Controller
    {
       

        // GET: KiiPayBusiness/KiiPayBusinessPayBills
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PayBills()
        {
            return View();
        }
        
    }
}