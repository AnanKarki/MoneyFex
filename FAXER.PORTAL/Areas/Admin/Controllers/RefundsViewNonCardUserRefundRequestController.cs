using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class RefundsViewNonCardUserRefundRequestController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        // GET: Admin/RefundsViewNonCardUserRefundRequest
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            Services.NonCardFaxingRefundRequestServices services = new Services.NonCardFaxingRefundRequestServices();
            var model = services.NonCardRefundDetails(CountryCode, City);
            if (model != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(model);
        }

        public void PrintMFSSentReceipt(NonCardFaxingRefundRequestViewModel model)
        {
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var ReceiptUrl = baseUrl + "/EmailTemplate/NonCardUserReceiver?MFReceiptNumber=" + model.FaxReceiptNumber + "&TransactionDate=" + model.TransactionDate.ToFormatedString()
                + "&TransactionTime=" + model.TransactionDate.ToString("HH:mm") + "&FaxerFullName=" + model.FaxerFirstName + "&MFCN=" + model.MFCNNumber + "&ReceiverFullName=" + model.ReceiverFirstName
                + "&Telephone=" + model.ReceiverTelephone + "&AgentName=" + model.NameOfAgency + "&AgentCode=" + model.AgencyMFSCode + "&AmountSent=" + model.FaxingAmount + "&ExchangeRate="
                + model.ExchangeRate + "&Fee=" + model.FaxingFee + "&AmountReceived=" + model.ReceivedAmount + "&SendingCurrency=" + model.FaxingCurrency + "&ReceivingCurrency=" + model.ReceivingCurrency;

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

        public void PrintRefundReceipt(NonCardFaxingRefundRequestViewModel model)
        {
            Services.NonCardFaxingRefundRequestServices services = new Services.NonCardFaxingRefundRequestServices();
            var RefundDetails = services.getAdminRefundReceiptDetails(model.MFCNNumber);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);


            var ReceiptUrl = baseUrl + "/EmailTemplate/AdminRefundReceipt?ReceiptNumber=" + model.ReceiptNumber
                + "&TransactionReceiptNumber=" + model.FaxReceiptNumber + "&Date=" + model.RefundedDate.ToFormatedString()
                + "&Time=" + model.RefendedTime + "&SenderFullName=" + model.FaxerFirstName + "&MFCN=" + model.MFCNNumber + "&ReceiverFullName="
                + model.ReceiverFirstName + "&Telephone=" + model.ReceiverTelephone + "&RefundingAdminName=" + model.NameOfRefunder + "&RefundingAdminCode="
                + model.AdminCode + "&OrignalAmountSent=" + model.FaxingAmount + "&RefundedAmount=" + model.FaxingAmount + "&SendingCurrency=" + model.FaxingCurrency + "&ReceivingCurrency=" + model.FaxingCurrency +
                "&ReceiverCountry=" + Common.Common.GetCountryName(model.ReceiverCountryCode) + "&ReceiverCity=" + model.ReceiverCity
                + "&RefundingType=" + RefundDetails.StaffLoginCode;
                



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
        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }
    }
}