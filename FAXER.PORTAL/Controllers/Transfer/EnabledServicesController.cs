using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Transfer
{
    public class EnabledServicesController : Controller
    {
        // GET: EnabledServices
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetTransferServices(string sendingCountry, string receivingCountry, string receivingCurrency)
        {
            var services = Common.Common.GetTransferServicesByCurrency(sendingCountry, receivingCountry, receivingCurrency);
            //var services = 
            if (services != null)
            {
                var enabledServices = services.Select(x => x.ServiceType).ToArray();
                var result = string.Join(",", enabledServices);

                return Json(new
                {
                    Data = result
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(new
            {
                Data = ""
            }, JsonRequestBehavior.AllowGet);

        }

    }
}