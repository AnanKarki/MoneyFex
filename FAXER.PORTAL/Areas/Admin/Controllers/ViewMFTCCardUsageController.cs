using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewMFTCCardUsageController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        Services.ViewMFTCCardUsageServices Service = new Services.ViewMFTCCardUsageServices();
        // GET: Admin/ViewMFTCCardUsage
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            var vm = Service.getMFTCCardUsageList(CountryCode, City);
            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(vm);
        }

        public void PrintReceipt(ViewMFTCCardWithdrawalViewModel model)
        {
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var ReceiptUrl = baseUrl + "/EmailTemplate/MFTCCardUserReceiverReceipt?MFReceiptNumber=" + model.ReceiptNumber + 
                "&TransactionDate=" + model.WithdrawalDate + "&TransactionTime=" + model.WithdrawalTime + "&FaxerFullName=" + model.FaxerFullName +
                "&MFTCCardNumber=" + model.MFTCCardNumber + "&CardUserFullName=" + model.CardUserFullName + "&Telephone=" + model.CardUserTelephone + "&AgentName=" 
                + model.AgentName + "&AgentCode=" + model.AgentMFSCode + "&AmountRequested=" + model.WithdrawalAmount + "&ExchangeRate=" + " " + "&Fee=" + "" + "&AmountWithdrawn=" + model.WithdrawalAmount + "&Currency=" + model.Currency;

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
            var cities = SCity.GetCities(DB.Module.CardUser, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
    }
}