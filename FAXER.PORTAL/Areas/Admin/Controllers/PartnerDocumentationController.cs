using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class PartnerDocumentationController : Controller
    {
        CommonServices _CommonServices = new CommonServices();
        // GET: Admin/PartnerDocumentation
        public ActionResult Index(string CountryCode = "", int PartnerId = 0, int type = 0, string PartnerAccountNo = "",
            string DocumentName = "", string Staff = "", string DateRange = "")
        {
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            ViewBag.Partner = new SelectList(Countries, "Code", "Name");

            List<BusinessDocumentationViewModel> vm = new List<BusinessDocumentationViewModel>();

            ViewBag.PartnerAccountNo = PartnerAccountNo;
            ViewBag.DocumentName = DocumentName;
            ViewBag.Staff = Staff;

            if (!string.IsNullOrEmpty(PartnerAccountNo))
            {
                PartnerAccountNo = PartnerAccountNo.Trim();
            }
            if (!string.IsNullOrEmpty(DocumentName))
            {
                DocumentName = DocumentName.Trim();
            }
            if (!string.IsNullOrEmpty(Staff))
            {
                Staff = Staff.Trim();
            }
            if (!string.IsNullOrEmpty(DateRange))
            {

                var Date = DateRange.Split('-');
                string[] startDate = Date[0].Split('/');
                string[] endDate = Date[1].Split('/');
                var FromDate = new DateTime(int.Parse(startDate[2]), int.Parse(startDate[0]), int.Parse(startDate[1]));
                var ToDate = new DateTime(int.Parse(endDate[2]), int.Parse(endDate[0]), int.Parse(endDate[1]));// Convert.ToDateTime(Date[1]);

            }


            return View(vm);
        }

        public ActionResult UploadDocument()
        {
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");
            //assume as partner
            BusinessDocumentationServices _services = new BusinessDocumentationServices();
            var Business = _services.GetBuisnessSender();
            ViewBag.Business = new SelectList(Business, "Id", "Name");
            // end

            BusinessDocumentationViewModel vm = new BusinessDocumentationViewModel();
            return View(vm);
        }
    }
}