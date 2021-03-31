using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class FaxerIdentificationController : Controller
    {
        // GET: FaxerIdentification
        FAXER.PORTAL.DB.FAXEREntities dbContext = new DB.FAXEREntities();
        public ActionResult Index()
        {
            ViewBag.IdCardType = new SelectList(dbContext.IdentityCardType.OrderBy(x => x.CardType), "CardType", "CardType");
            ViewBag.IssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            return View();
        }
        [HttpPost]
        [HandleError]
        public ActionResult Index([Bind(Include = FaxerIdentification.BindProperty)]FaxerIdentification model, HttpPostedFileBase upload, FormCollection val)
        {

            if (model.CheckAmount==true && upload==null)
            {
                ModelState.AddModelError("File", "Please Upload Identity Document");
            }
            var fileName = "";
            if (upload!=null)
            {
                string directory = Server.MapPath("/Documents");
                 fileName = "";
                if (upload != null && upload.ContentLength > 0)
                {
                    fileName = Path.GetFileName(upload.FileName);
                    upload.SaveAs(Path.Combine(directory, fileName));

                }
            }
            bool isValidExpityDate = Common.DateUtilities.DateGreaterThanToday(model.IdCardExpiringDate);
            if (!isValidExpityDate)
            {
                ModelState.AddModelError("IdCardExpiringDate", "Date Must Be Greater Than Today");
            }


            if (ModelState.IsValid)
            {
                
                Session["IdCardType"] = model.IdCardType;
                Session["IdCardNumber"] = model.IdCardNumber;
                Session["IdCardExpDate"] = model.IdCardExpiringDate;
                Session["IdCardIssuingCountry"] = model.IssuingCountry;
                Session["CardUrl"] =fileName;

                return RedirectToAction("Index", "FaxerContactDetails");
            }
            ViewBag.IdCardType = new SelectList(dbContext.IdentityCardType.OrderBy(x => x.CardType), "CardType", "CardType");
            ViewBag.IssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            return View();
        }        
    }
}