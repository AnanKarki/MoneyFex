using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class DailyRatesController : Controller
    {
        DailyRatesServices _services = null;
        public DailyRatesController()
        {
            _services = new DailyRatesServices();
        }

        // GET: DailyRates
        public ActionResult Index(string date = "")
        {
            List<DailyRatesViewModel> vm = _services.GetDailyRates(date);
            ViewBag.TodayDate = DateTime.Now.ToShortDateString();
            return View(vm);
        }
        public ActionResult DailyRateDetails(int id = 0)
        {
            DailyRatesViewModel vm = _services.GetDailyRates().Where(x => x.Id == id).FirstOrDefault();
            return View(vm);
        }
    }
}