using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewMFBCCardUsageController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        Services.ViewMFBCCardUsageServices Service = new Services.ViewMFBCCardUsageServices();
        // GET: Admin/ViewMFBCCardUsage
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            var vm = Service.getMFBCCardWithdrawalList(CountryCode, City);
            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(vm);
        }


        public void PrintMFBCCardUsageReceipt(int id)
        {
            var model = Service.MFBCWithdrawalReceiptData(id);

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var ReceiptUrl = baseUrl + "/EmailTemplate/MFBCCardUserWithdrawlReceipt?MFReceiptNumber=" + model.MFReceiptNumber +
                "&TransactionDate=" + model.TransactionDate + "&TransactionTime=" + model.TransactionTime + "&BusinessMerchantName=" + model.BusinessMerchantName +
                "&MFBCCardNumber=" + model.MFBCCardNumber + "&BusinessCardUserFullName=" + model.BusinessCardUserFullName + "&Telephone=" + model.Telephone +
                "&AgentName=" + model.AgentName + "&AgentCode=" + model.AgentCode + "&AmountRequested=" + model.AmountRequested + "&ExchangeRate=" + model.ExchangeRate + "&AmountWithdrawn=" + model.AmountWithdrawn + "&Currency=" + model.Currency;

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

        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.BusinessMerchant, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
    }
}