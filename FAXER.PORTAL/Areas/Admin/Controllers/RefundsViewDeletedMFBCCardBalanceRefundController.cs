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
    public class RefundsViewDeletedMFBCCardBalanceRefundController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        RefundsViewDeletedMFBCCardBalanceRefundServices Service = new RefundsViewDeletedMFBCCardBalanceRefundServices();
        // GET: Admin/RefundsViewDeletedMFBCCardBalanceRefund
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


        public void printReceipt(RefundBalanceOnDeletedMFBCViewModel model)
        {
            string receiptNo = Service.generateReceiptNo();
            bool insertReceiptNo = Service.insertReceiptNo(receiptNo, model.Id);
            if (insertReceiptNo == false)
            {
                receiptNo = Service.getReceiptNo(model.Id);
            }
            string MFBCardUserName = Service.getMFBCardUserName(model.Id);


            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptUrl = baseUrl + "/EmailTemplate/AdminMFBCCardRefundReciept?ReceiptNumber=" + receiptNo
                + "&MerchantAccNo=" + model.BusinessLicenseNo + "&Date=" + model.RefundDate + "&Time=" + model.RefundTime 
                + "&MerchantName=" + model.BusinessName + "&MFBCardNumber=" + model.MFBCNumber + "&MerchantCountry=" + model.Country 
                + "&MerchantCity=" + model.City + "&MFBCCardUserName=" + MFBCardUserName + "&RefundingStaffName=" 
                + model.AdminRefunder + "&RefundingStaffCode=" + model.AdminRefunderMFSCode + "&RefundedAmount=" + model.CreditOnMFBC.ToString() + model.Currency +
                "&RefundingType=" + model.StaffLoginCode; 

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