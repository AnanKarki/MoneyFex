using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewFaxerMerchantBusinessPaymentsController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        ViewFaxerMerchantBusinessPaymentsServices Service = new ViewFaxerMerchantBusinessPaymentsServices();
        // GET: Admin/ViewFaxerMerchantBusinessPayments
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            var vm = Service.getDetails(CountryCode, City);
            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(vm);
        }

        public void PrintReceipt(int id)
        {
            var data = Service.getReceiptInfo(id);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptUrl = baseUrl + "/EmailTemplate/AdminPayGoodsAndServicesReceipt?ReceiptNumber=" + data.ReceiptNumber + "&Date=" + data.Date + "&Time=" + data.Time + "&FaxerFullName=" + data.FaxerFullName +
                "&BusinessMerchantName=" + data.BusinessMerchantName + "&BusinessMFCode=" + data.BusinessMFCode + "&AmountPaid=" + data.AmountPaid + "&ExchangeRate=" + data.ExchangeRate + "&AmountInLocalCurrency=" + data.AmountInLocalCurrency +
                "&Fee=" + data.Fee + "&StaffName=" + data.StaffName + "&StaffCode=" + data.StaffCode + "&SendingCurrency="
                + data.SendingCurrency + "&ReceivingCurrency=" + data.ReceivingCurrency +
                 "&Faxercountry=" + data.FaxerCountry
                 + "&BusinessCountry=" + data.BusinessCountry
                 + "&BusinessCity=" + data.BusinessCity + "&DepositType=" + data.StaffLoginCode ;

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
            var cities = SCity.GetCities(DB.Module.BusinessMerchant, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
    }
}