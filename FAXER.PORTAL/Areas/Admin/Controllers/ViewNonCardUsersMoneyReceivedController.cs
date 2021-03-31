using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewNonCardUsersMoneyReceivedController : Controller
    {
        Services.ViewNonCardUsersMoneyReceivedServices Service = new Services.ViewNonCardUsersMoneyReceivedServices();
        // GET: Admin/ViewNonCardUsersMoneyReceived
        Services.CommonServices CommonService = new Services.CommonServices();
        public ActionResult Index(string CountryCode , string City)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            SetViewBagForSCities(CountryCode);
            var viewmodel = new ViewModels.ViewNonCardUsersMoneyReceivedViewModel();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                viewmodel = Service.getFilterNonCardUserMoneyTransactionList(CountryCode, City);
                ViewBag.Country = CountryCode;

            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                viewmodel = Service.getFilterNonCardUserMoneyTransactionList(CountryCode, City);
                ViewBag.Country = CountryCode;
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                viewmodel = Service.getFilterNonCardUserMoneyTransactionList(CountryCode, City);
                ViewBag.Country = CountryCode;
                ViewBag.City = City;


            }
            else
            {
                //viewmodel = Service.getNonCardUserMoneyTransactionList();
                ViewBag.Country = "";
            }
            
            return View(viewmodel);
        }

        public ActionResult ViewMoreInfo(int id)
        {
            var vm = Service.getMoreInfo(id);
            return View(vm);
        }

        public void PrintReceipt(ViewNonCardUsersDetailsViewModel model)
        {
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var ReceiptUrl = baseUrl + "/EmailTemplate/NonCardUserWithrawlReceipt?MFReceiptNumber=" + model.ReceiptNo + "&TransactionDate=" 
                + model.TransactionDate + "&TransactionTime=" + model.TransactionTime + "&FaxerFullName=" + model.FaxerFullName + "&MFCN=" + model.MFCN 
                + "&ReceiverFullName=" + model.ReceiverFullName + "&Telephone=" + model.ReceiversTelephone + "&AgentName=" + model.PayingAgentName + "&AgentCode=" 
                + model.PayingAgentMFSCode + "&AmountSent=" + model.AmountSent + "&ExchangeRate=" + model.ExchangeRate + "&Fee=" + model.Fee 
                + "&AmountReceived=" + model.AmountReceived + "&SendingCurrency=" + model.SendingCurrency + "&ReceivingCurrency=" + model.ReceivingCurrency;

            var ReceiptPDF = Common.Common.GetPdf(ReceiptUrl);
            byte[] bytes = ReceiptPDF.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
        }
        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Faxer, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
    }
}