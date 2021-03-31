using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewMFTCCardToppingController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        Services.ViewMFTCCardToppingServices Service = new Services.ViewMFTCCardToppingServices();
        // GET: Admin/ViewMFTCCardTopping
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            var vm = Service.getMFTCCardTopUpList(CountryCode, City);
            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(vm);
        }

        public void PrintPDFReceipt(int id)
        {
            var data = new ViewModels.MFTCCardToppingReceiptViewModel();
            data = Service.getReceiptInfo(id);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var ReceiptUrl = baseUrl + "/EmailTemplate/AdminMFTCCardTop?ReceiptNumber=" + data.ReceiptNumber + "&Date=" + data.Date + "&Time=" + data.Time + "&FaxerFullName=" + data.FaxerFullName +
                "&MFTCCardNumber=" + data.MFTCCardNumber + "&CardUserFullName=" + data.CardUserFullName + "&AmountTopUp=" + data.AmountTopUp + "&ExchangeRate=" + data.ExchangeRate + "&AmountInLocalCurrency=" + data.AmountInLocalCurrency +
                "&Fee=" + data.Fee + "&BalanceOnCard=" + data.BalanceOnCard + "&StaffName=" + data.StaffName + "&StaffCode=" + data.StaffCode + "&SendingCurrency=" + data.SendingCurrency + "&ReceivingCurrency=" + data.ReceivingCurrency +
                 "&DepositType=" + data.StaffLoginCode +
                         "&FaxerCountry=" + data.FaxerCountry
                          + "&CardUserCountry=" + data.CardUserCountry +
                          "&CardUserCity=" + data.CardUserCity;

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