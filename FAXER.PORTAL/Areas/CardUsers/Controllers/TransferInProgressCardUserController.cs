using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class TransferInProgressCardUserController : Controller
    {

        Services.CardUser_TransferInProgressServices _cardUser_TransferInProgressServices = null;

        Services.CardUserCommonServices _commonServices = null;
        public TransferInProgressCardUserController()
        {
            _cardUser_TransferInProgressServices = new Services.CardUser_TransferInProgressServices();
            _commonServices = new Services.CardUserCommonServices();
        }
        // GET: CardUsers/TransferInProgressCardUser
        public ActionResult Index(string searchText = "")
        {
            var vm = _cardUser_TransferInProgressServices.GetTransferInProgressList(searchText);
            return View(vm);
        }

        [HttpPost]
        public ActionResult CancelFax(int Id)
        {
            if (Id > 0)
            {
                var nonCardTransaction = _cardUser_TransferInProgressServices.CancelTransaction(Id);

                var ReDepositMFTCCardCard = _cardUser_TransferInProgressServices.ReDepositCard(nonCardTransaction.MFTCCardId , nonCardTransaction.FaxingAmount);

                var ReceiverDetails = _commonServices.GetReceiverDetails(nonCardTransaction.NonCardRecieverId);
                Common.CardUserSession.LoggedCardUserViewModel.BalanceOnCard = _commonServices.getCurrentBalanceOnCard(nonCardTransaction.MFTCCardId);
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);


                string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/NonCardTransactionCancellationEmail?SenderName=" +
                    ReceiverDetails.MFTCCardInformation.FirstName +" " + ReceiverDetails.MFTCCardInformation.MiddleName +
                    ReceiverDetails.MFTCCardInformation.LastName + 
                    "&MFCN=" + nonCardTransaction.MFCN + "&Amount=" + nonCardTransaction.FaxingAmount +
                    "&ReceiverName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " "
                    + ReceiverDetails.LastName + "&ReceiverCountry=" + Common.Common.GetCountryName(ReceiverDetails.Country)
                    + "&ReceiverCity=" + ReceiverDetails.City + "&TransactionDate=" + nonCardTransaction.TransactionDate);

                mail.SendMail(ReceiverDetails.MFTCCardInformation.CardUserEmail, "Refund Confirmation- MFCN "  + nonCardTransaction.MFCN , body);
                return RedirectToAction("Index", "TransferHistoryOfCardUser", new { searchText = nonCardTransaction.MFCN });
                //end 
            }

            return RedirectToAction("Index", "TransferInProgressCardUser");
        }


        public void NonCardTransferReceipt(int Id) {

            var nonCardTransaction = _cardUser_TransferInProgressServices.getTransactionDetails(Id);
            var ReceiverDetails = _commonServices.GetReceiverDetails(nonCardTransaction.NonCardRecieverId);

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string URL = baseUrl + "/EmailTemplate/NonCardTransferReceipt?MFReceiptNumber=" + nonCardTransaction.ReceiptNumber +
                      "&TransactionDate=" + nonCardTransaction.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + nonCardTransaction.TransactionDate.ToString("HH:mm")
                        + "&FaxerFullName=" + ReceiverDetails.MFTCCardInformation.FirstName + " " + ReceiverDetails.MFTCCardInformation.MiddleName + " " + ReceiverDetails.MFTCCardInformation.LastName +
                      "&MFCN=" + nonCardTransaction.MFCN + "&ReceiverFullName=" + ReceiverDetails.FirstName + " " + ReceiverDetails.MiddleName + " " + ReceiverDetails.LastName
                      + "&Telephone=" + Common.Common.GetCountryPhoneCode(ReceiverDetails.Country) + " " + ReceiverDetails.PhoneNumber
                      + "&AmountSent=" + nonCardTransaction.FaxingAmount
                      + "&ExchangeRate=" + nonCardTransaction.ExchangeRate + "&Fee=" + nonCardTransaction.FaxingFee
                      + "&AmountReceived=" + nonCardTransaction.ReceivingAmount + "&SendingCurrency=" + Common.Common.GetCountryCurrency(ReceiverDetails.MFTCCardInformation.CardUserCountry)
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