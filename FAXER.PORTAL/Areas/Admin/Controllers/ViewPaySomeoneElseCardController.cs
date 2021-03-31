using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewPaySomeoneElseCardController : Controller
    {
        Services.ViewPaySomeoneElseCardServices ViewPaySomeoneElseCardServices = null;

        Services.CommonServices CommonService = null;

        DB.FAXEREntities dbContext = null;
        public ViewPaySomeoneElseCardController()
        {
            ViewPaySomeoneElseCardServices = new Services.ViewPaySomeoneElseCardServices();
            CommonService = new Services.CommonServices();
            dbContext = new DB.FAXEREntities();
        }
        // GET: Admin/ViewPaySomeoneElseCard
        public ActionResult Index(string CountryCode, string City)
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var cities = SCity.GetCities(DB.Module.Faxer, CountryCode);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
            ViewBag.Country = CountryCode;
            ViewBag.City = City;
            var vm = ViewPaySomeoneElseCardServices.GetTransactionDetails(CountryCode, City);

            return View(vm);
            
        }

        public void PrintReceipt(int TransactionId)
        {

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var transactionDetails = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.Id == TransactionId).FirstOrDefault();
            var SenderInformation = dbContext.FaxerInformation.Where(x => x.Id == transactionDetails.FaxerId).FirstOrDefault();
            string FaxerCurrency = Common.Common.GetCountryCurrency(SenderInformation.Country);
            string CardUserCurrency = Common.Common.GetCountryCurrency(transactionDetails.KiiPayPersonalWalletInformation.CardUserCountry);
            string ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardTopUpReceipt?ReceiptNumber=" + transactionDetails.ReceiptNumber + "&Date=" +
              transactionDetails.TransactionDate.ToString("dd/MM/yyyy") + "&Time=" + transactionDetails.TransactionDate.ToString("HH:mm")
              + "&FaxerFullName=" + SenderInformation.FirstName + " " + SenderInformation.MiddleName + " " + SenderInformation.LastName
              + "&MFTCCardNumber=" + transactionDetails.KiiPayPersonalWalletInformation.MobileNo.Decrypt()
              + "&CardUserFullName=" + transactionDetails.KiiPayPersonalWalletInformation.FirstName + " " + transactionDetails.KiiPayPersonalWalletInformation.MiddleName + " " + transactionDetails.KiiPayPersonalWalletInformation.LastName
              + "&AmountTopUp=" + transactionDetails.FaxingAmount + " " + FaxerCurrency + "&ExchangeRate=" + transactionDetails.ExchangeRate +
              "&AmountInLocalCurrency=" + transactionDetails.RecievingAmount + " " + CardUserCurrency + "&Fee=" + transactionDetails.FaxingFee
              + " " + FaxerCurrency + "&BalanceOnCard=" + transactionDetails.KiiPayPersonalWalletInformation.CurrentBalance + " " + CardUserCurrency + "&TopupReference=" + transactionDetails.TopUpReference;
            var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);

            byte[] bytes = ReceiptPDF.Save();
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