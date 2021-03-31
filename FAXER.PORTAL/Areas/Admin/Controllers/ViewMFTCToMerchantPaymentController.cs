using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewMFTCToMerchantPaymentController : Controller
    {

        Services.CommonServices CommonService = null;
        Services.ViewMFTCToMerchantPaymentServices ViewMFTCToMerchantPaymentServices = null;
        public ViewMFTCToMerchantPaymentController()
        {
            CommonService = new Services.CommonServices();
            ViewMFTCToMerchantPaymentServices = new Services.ViewMFTCToMerchantPaymentServices();
        }
        // GET: Admin/ViewMFTCToMerchantPayment
        public ActionResult Index(string CountryCode="" , string City="")
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var cities = SCity.GetCities(DB.Module.CardUser, CountryCode);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
            ViewBag.Country = CountryCode;
            ViewBag.City = City;
            var vm = ViewMFTCToMerchantPaymentServices.GetMFTCToMerchantPaymentDetails(CountryCode, City);

            return View(vm);
            
        }

        public void PrintReceipt(int TransactionId) {


            var transactionDetails = ViewMFTCToMerchantPaymentServices.GetMerchantInternationalTransactionDetail(TransactionId);


            
            string SendingCurrency = Common.Common.GetCountryCurrency(transactionDetails.KiiPayPersonalWalletInformation.CardUserCountry);
            string ReceivingCurrency = Common.Common.GetCountryCurrency(transactionDetails.KiiPayBusinessWalletInformation.Country);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string URL = baseUrl + "/EmailTemplate/ConfirmationofServiceProviderPaymentReceipt?ReceiptNumber=" + transactionDetails.ReceiptNumber
                + "&Date=" + transactionDetails.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + transactionDetails.TransactionDate.ToString("HH:mm") + "&SenderName=" +
                transactionDetails.KiiPayPersonalWalletInformation.FirstName + " " + transactionDetails.KiiPayPersonalWalletInformation.MiddleName + " " + transactionDetails.KiiPayPersonalWalletInformation.LastName
                + "&ServiceProvideName=" + transactionDetails.KiiPayBusinessWalletInformation.FirstName + " " + transactionDetails.KiiPayBusinessWalletInformation.MiddleName + " " + transactionDetails.KiiPayBusinessWalletInformation.LastName +
                "&BusinessMFCode=" + transactionDetails.KiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo +
                "&TopUpAmount=" + transactionDetails.FaxingAmount + "&Fees=" + transactionDetails.FaxingFee +
                "&ExchangeRate=" + transactionDetails.ExchangeRate + "&TotalAmount=" + transactionDetails.TotalAmount + "&AmountInLocalCurrency=" + transactionDetails.ReceivingAmount
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