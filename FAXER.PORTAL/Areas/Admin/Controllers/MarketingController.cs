using FAXER.PORTAL.Areas.Admin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class MarketingController : Controller
    {
        CommonServices _CommonServices = null;

        public MarketingController()
        {
            _CommonServices = new CommonServices();

        }
        // GET: Admin/Marketing
        public ActionResult CurrentPormotion()
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetCountryViewBag();
            return View();
        }
        public ActionResult SetPromotion(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetCountryViewBag();
            return View();
        }
        public ActionResult DeletePromotiom(int id)
        {

            return View();
        }

        public ActionResult CurrentReferral()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            SetCountryViewBag();
            return View();
        }
        public ActionResult SetReferral(int id = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetCountryViewBag();
            return View();
        }
        public ActionResult DeleteReferral(int id)
        {
            return View();
        }
        public void SetCountryViewBag()
        {
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
        }
    }
}