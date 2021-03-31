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
    public class RefundsViewRefundBalanceOnDeletedMFTCCardController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        RefundsViewRefundBalanceOnDeletedMFTCCardServices Service = new RefundsViewRefundBalanceOnDeletedMFTCCardServices();
        // GET: Admin/RefundsViewRefundBalanceOnDeletedMFTCCard
        public ActionResult Index(string CountryCode = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            var vm = Service.getList(CountryCode, City);
            if (vm != null)
            {
                ViewBag.Country = CountryCode;
            }
            return View(vm);
        }

        public void printReceipt(RefundBalanceOnDeletedMFTCViewModel model)
        {
            string receiptNo = Service.generateReceiptNo();
            bool insertReceiptNo = Service.insertReceiptNo(receiptNo, model.Id);
            if (insertReceiptNo == false)
            {
                receiptNo = Service.getReceiptNo(model.Id);
            }
            string cardno = Service.getLastFourDigitsFromSavedCards(model.FaxerId);
            string cardFourNo = "";
            if (cardno != "")
            {
                cardFourNo = "xxxx-xxxx-xxxx-" + cardno;
            }
            else
                cardFourNo = "No Saved Card Available !";

            string cardUserName = Service.getCardUserName(model.Id);

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptUrl = baseUrl + "/EmailTemplate/AdminMFTCCardRefundRecieptController?ReceiptNumber=" + receiptNo
                + "&CardLastFourDigits=" + cardFourNo + "&Date=" + model.RefundDate + "&Time=" + model.RefundTime + "&MFTCardRegistrar="
                + model.FaxerName + "&MFTCardNumber=" + model.MFTCNumber + "&MFTCardUserName=" + cardUserName + "&MFTCardUserCountry=" + model.Country
                + "&MFTCardUserCity=" + model.City + "&RefundingStaffName=" + model.AdminRefunder + "&RefundingStaffCode=" + model.AdminRefunderMFSCode
                + "&RefundedAmount=" + model.AmountOnMFTC.ToString() + model.Currency +
                 "&BalanceOnCard=" + model.AmountOnMFTC + "&RefundingType=" + model.StaffLoginCode;
            ;

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