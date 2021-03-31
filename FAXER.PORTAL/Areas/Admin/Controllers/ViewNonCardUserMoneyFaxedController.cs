using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewNonCardUserMoneyFaxedController : Controller
    {
        Services.ViewNonCardUserMoneyFaxedServices Service = new Services.ViewNonCardUserMoneyFaxedServices();
        Services.CommonServices common = new Services.CommonServices();
        // GET: Admin/ViewNonCardUserMoneyFaxed
        [HttpGet]
        public ActionResult Index(string CountryCode = "", string City = "", string Message = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (Message == "cancelSuccess")
            {
                ViewBag.Message = "Transaction Cancelled Successfully !";
                ViewBag.ToastrVal = 4;
                Message = "";
            }
            else if (Message == "cancelFailure")
            {
                ViewBag.Message = "Something went wrong. Please contact Administrator !";
                ViewBag.ToastrVal = 0;
                Message = "";
            }

            var countries = common.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            SetViewBagForSCities(CountryCode);
            var viewmodel = new ViewModels.ViewNonCardUserMoneyFaxedViewModel();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                viewmodel = Service.getFilterNonCardMoneyFaxedList(CountryCode, City);
                ViewBag.Country = CountryCode;

            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                viewmodel = Service.getFilterNonCardMoneyFaxedList(CountryCode, City);
                ViewBag.Country = CountryCode;
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                viewmodel = Service.getFilterNonCardMoneyFaxedList(CountryCode, City);
                ViewBag.Country = CountryCode;
                ViewBag.City = City;


            }
            else
            {
                //viewmodel= Service.getNonCardUserMoneyFaxedList();
                ViewBag.Country = "";
            }

            return View(viewmodel);
        }

        public ActionResult cancelTransaction(int id)
        {
            if (id != 0)
            {
                var result = Service.cancelTransaction(id);
                if (result != null)
                {
                    return RedirectToAction("Index", "ViewNonCardUserMoneyFaxed", new { area = "Admin", CountryCode = result.NonCardReciever.FaxerInformation.Country, City = "", @Message = "cancelSuccess" });
                }
            }
            return RedirectToAction("Index", "ViewNonCardUserMoneyFaxed", new { area = "Admin", CountryCode = "", City = "", @Message = "cancelFailure" });
        }

        public JsonResult HoldTransaction(int transactionID, bool Hold)
        {

            var data = Service.GetTransactionDetails(transactionID);
            if (data != null)
            {


                if (data.FaxingStatus == FaxingStatus.NotReceived || data.FaxingStatus == FaxingStatus.Hold)
                {

                    FaxingStatus status = FaxingStatus.Hold;
                    if (Hold == false)
                    {

                        status = FaxingStatus.NotReceived;
                    }

                    data.FaxingStatus = status;
                    var result = Service.HoldUnholdNonCardTransaction(data);


                    //return RedirectToAction("Index", "ViewNonCardUserMoneyFaxed", new { area = "Admin", CountryCode = data.NonCardReciever.FaxerInformation.Country, City = "", @Message = message });

                    return Json(new
                    {

                        Success = true,
                    }, JsonRequestBehavior.AllowGet);
                }
            }

            //return RedirectToAction("Index", "ViewNonCardUserMoneyFaxed", new { area = "Admin", CountryCode = data.NonCardReciever.FaxerInformation.Country, City = "", @Message = "HoldFailure" });


            return Json(new
            {

                Success = false,
            }, JsonRequestBehavior.AllowGet);
        }


        public void PrintReceipt(int id)
        {
            var data = Service.getReceiptInfo(id);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var ReceiptUrl = baseUrl + "/EmailTemplate/AdminNonCardMoneyTransfer?MFReceiptNumber=" + data.MFReceiptNumber + "&TransactionDate=" + data.TransactionDate + "&TransactionTime=" + data.TransactionTime + "&FaxerFullName=" + data.FaxerFullName
                + "&MFCN=" + data.MFCN + "&ReceiverFullName=" + data.ReceiverFullName + "&Telephone=" + data.Telephone + "&StaffName=" + data.StaffName + "&StaffCode=" + data.StaffCode + "&AmountSent=" + data.AmountSent + "&ExchangeRate=" + data.ExchangeRate + "&Fee=" + data.Fee + "&AmountReceived=" + data.AmountReceived
                + "&TotalAmountSentAndFee=" + data.TotalAmountSentAndFee + "&SendingCurrency=" + data.SendingCurrency + "&ReceivingCurrency=" + data.ReceivingCurrency +
                  "&PaymentType=" + data.StaffLoginCode
                    + "&SenderPhoneNo=" + Common.Common.GetCountryPhoneCode(data.FaxerCountryCode) + " " + data.FaxerPhoneNo;

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
    }
}