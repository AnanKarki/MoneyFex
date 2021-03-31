using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewMerchantMFTCTopUpController : Controller
    {

        Services.CommonServices CommonService = null;
        Services.ViewMerchantMFTCTopUpServices ViewMerchantMFTCTopUpServices = null;
        public ViewMerchantMFTCTopUpController()
        {
            CommonService = new Services.CommonServices();
            ViewMerchantMFTCTopUpServices = new Services.ViewMerchantMFTCTopUpServices();
        }
        // GET: Admin/ViewMerchantMFTCTopUp
        public ActionResult Index(string CountryCode = "", string City = "")
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var cities = SCity.GetCities(DB.Module.BusinessMerchant, CountryCode);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
            ViewBag.Country = CountryCode;
            ViewBag.City = City;


            var vm = ViewMerchantMFTCTopUpServices.GetMerchantMFTCTopUpDetails(CountryCode, City);
            return View(vm);
        }

        public void PrintReceipt(int TransactionId) {


            var TransactionDetails = ViewMerchantMFTCTopUpServices.GetTopUpByMerchantDetails(TransactionId);

            var senderDetails = ViewMerchantMFTCTopUpServices.GetSenderDetails(TransactionDetails.KiiPayBusinessInformationId);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string URL = baseUrl + "/EmailTemplate/MFTCCardTopUpconfirmationPaymentReceipt?ReceiptNumber=" + TransactionDetails.ReceiptNumber
                + "&Date=" + TransactionDetails.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + TransactionDetails.TransactionDate.ToString("HH:mm") + "&SenderName=" +
                senderDetails.BusinessName
                + "&MFTCCardNo=" + TransactionDetails.KiiPayPersonalWalletInformation.MobileNo.Decrypt()
                + "&MFTCCardName=" + TransactionDetails.KiiPayPersonalWalletInformation.FirstName + " " + TransactionDetails.KiiPayPersonalWalletInformation.MiddleName + " " + TransactionDetails.KiiPayPersonalWalletInformation.LastName +
                "&TopUpAmount=" + TransactionDetails.PayingAmount + "&Fees=" + TransactionDetails.Fee +
                "&ExchangeRate=" + TransactionDetails.ExchangeRate + "&TotalAmount=" + TransactionDetails.TotalAmount + "&AmountInLocalCurrency=" + TransactionDetails.RecievingAmount;
            var Receipt = Common.Common.GetPdf(URL);

            byte[] bytes = Receipt.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
        }
    }
}