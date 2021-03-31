using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewMerchantToMerchantPaymentController : Controller
    {
        Services.CommonServices CommonService = null;
        Services.ViewMerchantToMerchantPaymentServices ViewMerchantToMerchantPaymentServices = null;
        public ViewMerchantToMerchantPaymentController()
        {
            ViewMerchantToMerchantPaymentServices = new Services.ViewMerchantToMerchantPaymentServices();
            CommonService = new Services.CommonServices();
        }
        // GET: Admin/ViewMerchantToMerchantPayment
        public ActionResult Index(string CountryCode , string City)
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var cities = SCity.GetCities(DB.Module.CardUser, CountryCode);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
            ViewBag.Country = CountryCode;
            ViewBag.City = City;
            var vm = ViewMerchantToMerchantPaymentServices.GetMerchantToMerchantPaymentDetails(CountryCode, City);

            return View(vm);
            
        }

        public void PrintReceipt(int TransactionId)
        {


            var transactionDetails = ViewMerchantToMerchantPaymentServices.GetMerchantToMerchantTransactionDetail(TransactionId);


            var senderDetails = ViewMerchantToMerchantPaymentServices.GetBusinessDetails(transactionDetails.PayedFromKiiPayBusinessWalletId);
            var receiverDetails = ViewMerchantToMerchantPaymentServices.GetBusinessDetails(transactionDetails.PayedToKiiPayBusinessWalletId);
            string SendingCurrency = Common.Common.GetCountryCurrency(senderDetails.Country);
            string ReceivingCurrency = Common.Common.GetCountryCurrency(receiverDetails.Country);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string URL = baseUrl + "/EmailTemplate/ConfirmationofServiceProviderPaymentReceipt?ReceiptNumber=" + transactionDetails.ReceiptNumber
                + "&Date=" + transactionDetails.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + transactionDetails.TransactionDate.ToString("HH:mm") + "&SenderName=" +
                senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.MiddleName
                + "&ServiceProvideName=" + receiverDetails.FirstName + " " + receiverDetails.MiddleName + " " + receiverDetails.LastName +
                "&BusinessMFCode=" + receiverDetails.KiiPayBusinessInformation.BusinessMobileNo +
                "&TopUpAmount=" + transactionDetails.FaxingAmount + "&Fees=" + transactionDetails.FaxingFee +
                "&ExchangeRate=" + transactionDetails.ExchangeRate + "&TotalAmount=" + transactionDetails.TotalAmount + "&AmountInLocalCurrency=" + transactionDetails.RecievingAmount
                + "&SendingCurrency=" + SendingCurrency + "&ReceivingCurrency=" + ReceivingCurrency;
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