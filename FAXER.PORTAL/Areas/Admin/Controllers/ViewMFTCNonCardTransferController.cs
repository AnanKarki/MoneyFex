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
    public class ViewMFTCNonCardTransferController : Controller
    {

        CommonServices CommonService = null;
        ViewMFTCNonCardTransferServices ViewNonCardTransferServices = null;

        public ViewMFTCNonCardTransferController()
        {
            CommonService = new CommonServices();
            ViewNonCardTransferServices = new ViewMFTCNonCardTransferServices();
        }
        // GET: Admin/ViewMFTCNonCardTransfer
        public ActionResult Index(string CountryCode = "", string City = "" )
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var cities = SCity.GetCities(DB.Module.CardUser, CountryCode);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
            ViewBag.Country = CountryCode;
            ViewBag.City = City;
            var vm = ViewNonCardTransferServices.GetMFTCNonCardTransaction(CountryCode, City);
            
            return View(vm);
        }

        public ActionResult HoldOrUnhold(int TransactionId, bool hold)
        {

            AdminResult adminResult = new AdminResult();
            if (TransactionId > 0)
            {

                var data = ViewNonCardTransferServices.GetTransactionDetails(TransactionId);

                if (data.FaxingStatus == DB.FaxingStatus.NotReceived || data.FaxingStatus == DB.FaxingStatus.Hold)
                {
                    if (hold == true)
                    {

                        adminResult.Message = "The transaction has been held successfully";

                        data.FaxingStatus = DB.FaxingStatus.Hold;
                    }
                    else
                    {
                        adminResult.Message = "The transaction  unhold successfully";

                        data.FaxingStatus = DB.FaxingStatus.NotReceived;
                    }

                    var result = ViewNonCardTransferServices.HoldUnholdTransaction(data);


                    adminResult.Status = AdminResultStatus.OK;
                    
                    


                }
                else
                {
                    adminResult.Status = AdminResultStatus.Warning;
                    adminResult.Message = "Sorry ! the transaction cannot be hold because it has already been" + Common.Common.GetEnumDescription((DB.FaxingStatus)data.FaxingStatus);
                    
                    
                }
                TempData["AdminResult"] = adminResult;

                return RedirectToAction("Index", new { CountryCode = data.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry, City = data.CardUserReceiverDetails.MFTCCardInformation.CardUserCity});
            }

            return RedirectToAction("Index");
        }
        public void PrintReceipt(int TransactionId)
        {


            var TransationDetails = ViewNonCardTransferServices.GetCardUserNonCardTransaction(TransactionId);

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            string URL = baseUrl + "/EmailTemplate/NonCardTransferReceipt?MFReceiptNumber=" + TransationDetails.ReceiptNumber +
                         "&TransactionDate=" + TransationDetails.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + TransationDetails.TransactionDate.ToString("HH:mm")
                           + "&FaxerFullName=" + TransationDetails.CardUserReceiverDetails.MFTCCardInformation.FirstName + " " + TransationDetails.CardUserReceiverDetails.MFTCCardInformation.MiddleName + " "
                           + TransationDetails.CardUserReceiverDetails.MFTCCardInformation.LastName +
                         "&MFCN=" + TransationDetails.MFCN + "&ReceiverFullName=" + TransationDetails.CardUserReceiverDetails.FirstName + " "
                         + TransationDetails.CardUserReceiverDetails.MiddleName + " " + TransationDetails.CardUserReceiverDetails.LastName
                         + "&Telephone=" + Common.Common.GetCountryPhoneCode(TransationDetails.CardUserReceiverDetails.Country) + " " + TransationDetails.CardUserReceiverDetails.PhoneNumber
                         + "&AmountSent=" + TransationDetails.FaxingAmount
                         + "&ExchangeRate=" + TransationDetails.ExchangeRate + "&Fee=" + TransationDetails.FaxingFee
                         + "&AmountReceived=" + TransationDetails.ReceivingAmount + "&SendingCurrency=" + Common.Common.GetCountryCurrency(TransationDetails.CardUserReceiverDetails.MFTCCardInformation.CardUserCountry)
                         + "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(TransationDetails.CardUserReceiverDetails.Country) ;

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