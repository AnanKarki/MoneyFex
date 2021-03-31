using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class EstimateFeeController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();

        Repos.CommonRepo repo = new Repos.CommonRepo();
        // GET: EstimateFee
        [HandleError]
        public ActionResult Index()
        {
            try
            {
                var countries = dbContext.Country.OrderBy(x => x.CountryName).Select(c => new SelectListItem()
                {
                    Text = c.CountryName,
                    Value = c.CountryCode
                }).ToList();
                ViewBag.Faxing = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
                ViewBag.Receiving = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            }
            catch (Exception ex)
            {


            }

            Models.EstimateFaxingFee model = new Models.EstimateFaxingFee();

            if (Common.FaxerSession.LoggedUser != null)
            {
                model.Faxing = Common.FaxerSession.LoggedUser.CountryCode;
            }

            if (Common.FaxerSession.ReceivingCountry != null) {

                model.Receiving = Common.FaxerSession.ReceivingCountry;

            }
            return View(model);
        }
        [HttpPost]
        [HandleError]
        public ActionResult Index([Bind(Include = EstimateFaxingFee.BindProperty)]EstimateFaxingFee model)
        {
            try
            {

                if (ModelState.IsValid)
                {
                    Common.FaxerSession.FaxingCountry = model.Faxing;
                    Common.FaxerSession.ReceivingCountry = model.Receiving;

                    return RedirectToAction("Index", "EstimateFaxingAmount");
                }
                ViewBag.Faxing = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
                ViewBag.Receiving = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            }
            catch (Exception)
            {


            }
            return View();
        }
    }
}