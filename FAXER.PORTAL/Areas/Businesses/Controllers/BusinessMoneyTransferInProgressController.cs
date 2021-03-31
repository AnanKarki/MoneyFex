using FAXER.PORTAL.Areas.Businesses.Services;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class BusinessMoneyTransferInProgressController : Controller
    {
        BusinessMoneyTransferInProgressServices _businessMoneyTransferInProgressServices = null;
        CommonServices _commonServices = new CommonServices();
        public BusinessMoneyTransferInProgressController()
        {
            _businessMoneyTransferInProgressServices = new BusinessMoneyTransferInProgressServices();
            _commonServices = new CommonServices();
        }
        // GET: Businesses/BusinessMoneyTransferInProgress
        public ActionResult Index(string searchText = "")
        {
            var vm = _businessMoneyTransferInProgressServices.GetTransferInProgressList(searchText);
            return View(vm);
        }

        [HttpPost]
        public ActionResult CancelFax(int Id)
        {
            if (Id > 0)
            {
                var nonCardTransaction = _businessMoneyTransferInProgressServices.CancelTransaction(Id);

                int MFTCCardID = _commonServices.getMFTCCardID();
                var RedepositMFBCCardInformation = _businessMoneyTransferInProgressServices.ReDepositMFBCCard(MFTCCardID, nonCardTransaction.FaxingAmount);


                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);


                string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/NonCardTransactionCancellationEmail?SenderName=" +
                    nonCardTransaction.NonCardReciever.Business.BusinessName +
                    "&MFCN=" + nonCardTransaction.MFCN +  "&Amount=" + nonCardTransaction.FaxingAmount + 
                    "&ReceiverName=" + nonCardTransaction.NonCardReciever.FirstName + " " + nonCardTransaction.NonCardReciever.MiddleName  + " "    
                    + nonCardTransaction.NonCardReciever.LastName + "&ReceiverCountry=" + Common.Common.GetCountryName(nonCardTransaction.NonCardReciever.Country)
                    + "&ReceiverCity=" +nonCardTransaction.NonCardReciever.City  + "&TransactionDate=" + nonCardTransaction.TransactionDate);

                mail.SendMail(nonCardTransaction.NonCardReciever.Business.Email, "Refund Confirmation {" + nonCardTransaction.MFCN + "}", body);
                return RedirectToAction("Index", "BusinessMoneyTransferHistory", new { searchText = nonCardTransaction.MFCN });
                //end 
            }

            return RedirectToAction("Index", "BusinessMoneyTransferInProgress");
        }

        public void NonCardTransferReceipt(int Id)
        {

            var nonCardTransaction = _businessMoneyTransferInProgressServices.GetTransactionDetials(Id);
            var ReceiverDetails = _commonServices.GetNonCardReceiverDetails(nonCardTransaction.NonCardRecieverId);

            var senderDetails = _commonServices.GetMFBCCardInformationByKiiPayBusinessInformationId(nonCardTransaction.KiiPayBusinessInformationId);

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string URL = baseUrl + "/EmailTemplate/NonCardTransferReceipt?MFReceiptNumber=" + nonCardTransaction.ReceiptNumber +
                      "&TransactionDate=" + nonCardTransaction.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + nonCardTransaction.TransactionDate.ToString("HH:mm")
                        + "&FaxerFullName=" + senderDetails.FirstName + " " + senderDetails.MiddleName + " " + senderDetails.LastName +
                      "&MFCN=" + nonCardTransaction.MFCN + "&ReceiverFullName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName
                      + "&Telephone=" + Common.Common.GetCountryPhoneCode(ReceiverDetails.Country) + " " + ReceiverDetails.PhoneNumber
                      + "&AmountSent=" + nonCardTransaction.FaxingAmount
                      + "&ExchangeRate=" + nonCardTransaction.ExchangeRate + "&Fee=" + nonCardTransaction.FaxingFee
                      + "&AmountReceived=" + nonCardTransaction.ReceivingAmount + "&SendingCurrency=" + Common.Common.GetCountryCurrency(senderDetails.Country)
                      + "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(ReceiverDetails.Country);

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
