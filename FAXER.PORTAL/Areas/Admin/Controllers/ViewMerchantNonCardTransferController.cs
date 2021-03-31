using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewMerchantNonCardTransferController : Controller
    {

        Services.CommonServices CommonService = null;
        Services.ViewMerchantNonCardTransferServices ViewMerchantNonCardTransferServices = null;
        public ViewMerchantNonCardTransferController()
        {
            CommonService = new Services.CommonServices();
            ViewMerchantNonCardTransferServices = new Services.ViewMerchantNonCardTransferServices();

        }
        // GET: Admin/ViewMerchantNonCardTransfer
        public ActionResult Index(string CountryCode="" , string City="")
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var cities = SCity.GetCities(DB.Module.CardUser, CountryCode);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
            ViewBag.Country = CountryCode;
            ViewBag.City = City;
            var vm = ViewMerchantNonCardTransferServices.GetMerchantNonCardTransferDetails(CountryCode, City);

            return View(vm);
        }

        public ActionResult HoldOrUnhold(int TransactionId, bool hold)
        {

            AdminResult adminResult = new AdminResult();
            if (TransactionId > 0)
            {

                var data = ViewMerchantNonCardTransferServices.GetTransactionDetail(TransactionId);

                if (data.FaxingStatus == DB.FaxingStatus.NotReceived || data.FaxingStatus == DB.FaxingStatus.Hold)
                {
                    if (hold == true)
                    {

                        data.FaxingStatus = DB.FaxingStatus.Hold;
                    }
                    else
                    {

                        data.FaxingStatus = DB.FaxingStatus.NotReceived;
                    }

                    var result = ViewMerchantNonCardTransferServices.HoldUnHoldTransaction(data);


                    adminResult.Status = AdminResultStatus.OK;
                    adminResult.Message = "The transaction has been held successfully";




                }
                else
                {
                    adminResult.Status = AdminResultStatus.Warning;
                    adminResult.Message = "Sorry ! the transaction cannot be hold because it has already been" + Common.Common.GetEnumDescription((DB.FaxingStatus)data.FaxingStatus);


                }
                TempData["AdminResult"] = adminResult;

                return RedirectToAction("Index", new { CountryCode = data.NonCardReciever.Business.BusinessOperationCountryCode, City = data.NonCardReciever.Business.BusinessOperationCity });
            }

            return RedirectToAction("Index");
        }

        public void PrintReceipt(int TransactionId) {



            var TransationDetails = ViewMerchantNonCardTransferServices.GetTransactionDetail(TransactionId);

            var MFBCCardDetails = CommonService.GetMFBCCardInformation(TransationDetails.MFBCCardID);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string URL = baseUrl + "/EmailTemplate/NonCardTransferReceipt?MFReceiptNumber=" + TransationDetails.ReceiptNumber +
                         "&TransactionDate=" + TransationDetails.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + TransationDetails.TransactionDate.ToString("HH:mm")
                           + "&FaxerFullName=" + MFBCCardDetails.FirstName + " " + MFBCCardDetails.MiddleName + " "
                           + MFBCCardDetails.LastName +
                         "&MFCN=" + TransationDetails.MFCN + "&ReceiverFullName=" + TransationDetails.NonCardReciever.FirstName + " "
                         + TransationDetails.NonCardReciever.MiddleName + " " + TransationDetails.NonCardReciever.LastName
                         + "&Telephone=" + Common.Common.GetCountryPhoneCode(TransationDetails.NonCardReciever.Country) + " " + TransationDetails.NonCardReciever.PhoneNumber
                         + "&AmountSent=" + TransationDetails.FaxingAmount
                         + "&ExchangeRate=" + TransationDetails.ExchangeRate + "&Fee=" + TransationDetails.FaxingFee
                         + "&AmountReceived=" + TransationDetails.ReceivingAmount + "&SendingCurrency=" + Common.Common.GetCountryCurrency(MFBCCardDetails.Country)
                         + "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(TransationDetails.NonCardReciever.Country);

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