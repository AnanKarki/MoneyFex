using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class PartnerPaymentCommissionController : Controller
    {
        ViewRegisteredPartnersServices Service = new ViewRegisteredPartnersServices();
        CommonServices Common = new CommonServices();
        // GET: Admin/PartnerPaymentCommission
        public ActionResult Index(string Country = "", string City = "", string Search = "", string Message = "")
        {
            SetViewBagForCountries();
            SetViewBagForSCities(Country);
            ViewBag.TotalRegisteredPartner = Service.List().Count;
            ViewBag.TotatActivePartner = Service.ListOfActivePartner().Count();
            ViewBag.TotalInActivePartner = Service.ListOfInActivePartner().Count;
            var vm = Service.getList(Country, City);
            if (!string.IsNullOrEmpty(Search))
            {
                vm = vm.Where(x => x.NameOfPartner.ToLower().Contains(Search.ToLower())).ToList();
            }

            string[] alpha = vm.GroupBy(x => x.FirstLetterOfPartner).Select(x => x.FirstOrDefault()).OrderBy(x => x.FirstLetterOfPartner).Select(x => x.FirstLetterOfPartner).ToArray();
            ViewBag.Alpha = alpha;
            ViewBag.Search = Search;
            return View(vm);
            
        }

        public ActionResult PartnerCommissionPaymentList(int Id=0,int transactionServiceType = 0, int year = 0, int month = 0, int Day = 0)
        {
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.Day = new SelectList(Enumerable.Range(1, 32));
            ViewBag.TransferMethod = transactionServiceType;
            ViewBag.Month = month;
           List<PartnerCommissionPaymentListViewModel> vm = new List<PartnerCommissionPaymentListViewModel>();


            return View(vm);
        }
        public ActionResult PartnerCommissionPaymentDetails()
        {
            PartnerCommissionPaymentDetailsViewModel vm = new PartnerCommissionPaymentDetailsViewModel();
            return View(vm);
        }
        private void SetViewBagForCountries()
        {
            var countries = Common.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Agent, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
    }
}