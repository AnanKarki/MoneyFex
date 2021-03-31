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
    public class ViewAgentMFBCPaymentsController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        ViewAgentMFBCPaymentsServices Service = new ViewAgentMFBCPaymentsServices();
        // GET: Admin/ViewAgentMFBCPayments
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            var vm = Service.getAgentMFBCPaymentsList(CountryCode, City);
            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(vm);
        }

        public void PrintReceiptPDF(ViewAgentMFBCPaymentsViewModel model)
        {
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var ReceiptUrl = baseUrl + "/EmailTemplate/MFBCCardUserWithdrawlReceipt?MFReceiptNumber=" + model.ReceiptNo + "&TransactionDate=" + model.MoneyWDDate + "&TransactionTime=" + model.MoneyWDTime + "&BusinessMerchantName=" + model.BusinessName + "&MFBCCardNumber=" + model.MFBCNumber + "&BusinessCardUserFullName="
                + model.CardUserFullName + "&Telephone=" + model.PhoneNo + "&AgentName=" + model.AgentName + "&AgentCode=" + model.AgentMFSCode + "&AmountRequested=" + model.MoneyWDAmount.ToString() + "&ExchangeRate=" + "" + "&AmountWithdrawn=" + model.MoneyWDAmount.ToString() + "&Currency=" + model.Currency;

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