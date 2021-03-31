using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Dashboard
{
    public class SenderCashPickUpDetailController : Controller
    {
        SSenderCashPickUp _senderCashPickServices = null;
        public SenderCashPickUpDetailController()
        {
            _senderCashPickServices = new SSenderCashPickUp();
        }
        // GET: SenderCashPickUpDetail
        public ActionResult Index()
        {

            var result = _senderCashPickServices.GetCashPickUpInProgressTrans().Take(5).ToList();
            var ManualBankDeposit = _senderCashPickServices.GetBAnkDepositInProgressTrans().Take(5).ToList();
            int SenderId = Common.FaxerSession.LoggedUser.Id;

            var Result = _senderCashPickServices.GetTransactionInProgress(SenderId);
            return View(Result);

        }


        public ActionResult MoreDetails(int TransactionId)
        {

            //var result = _senderCashPickServices.GetCashPickUpInProgressTrans().Where(x => x.Id == TransactionId).FirstOrDefault();
            //return View(result);

            int SenderId = Common.FaxerSession.LoggedUser.Id;

            SenderTransactionHistoryViewModel vm = _senderCashPickServices.GetAllTransactionInProgress(SenderId);
            vm.TransactionHistoryList = vm.TransactionHistoryList.Where(x => x.Id == TransactionId).ToList();
            return View(vm);


        }


        public ActionResult ViewAllDetails()
        {

            //var result = _senderCashPickServices.GetCashPickUpInProgressTrans();
            //return View(result);
            int SenderId = Common.FaxerSession.LoggedUser.Id;

            var Result = _senderCashPickServices.GetAllTransactionInProgress(SenderId);
            return View(Result);

        }

        [HttpPost]
        public ActionResult CancelFax(int Id)
        {
            DB.FAXEREntities context = new DB.FAXEREntities();
            if (Id > 0)
            {
                var nonCardTransaction = context.FaxingNonCardTransaction.Where(x => x.Id == Id).FirstOrDefault();
                nonCardTransaction.FaxingStatus = FaxingStatus.Cancel;
                context.Entry(nonCardTransaction).State = EntityState.Modified;
                context.SaveChanges();

                #region If Transaction is made by Agent 
                if (nonCardTransaction.OperatingUserType == OperatingUserType.Agent)
                {

                    var bankAccountCreditUpdate = context.BaankAccountCreditUpdateByAgent.Where(x => x.NonCardTransactionId == nonCardTransaction.Id).FirstOrDefault();
                    if (bankAccountCreditUpdate != null)
                    {
                        bankAccountCreditUpdate.CustomerDeposit = bankAccountCreditUpdate.CustomerDeposit - nonCardTransaction.FaxingAmount;
                        context.Entry(bankAccountCreditUpdate).State = EntityState.Modified;
                        context.SaveChanges();
                    }

                }

                #endregion
                //send email for Fax Cancellation 
                MailCommon mail = new MailCommon();

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                var receiverDetails = context.ReceiversDetails.Where(x => x.Id == nonCardTransaction.NonCardRecieverId).FirstOrDefault();
                string body = "";
                string ReceiverCountry = Common.Common.GetCountryName(receiverDetails.Country);
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxCancellationEmail?FaxerName=" + FaxerName +
                    "&MFCN=" + nonCardTransaction.MFCN + "&SentAmount=" + nonCardTransaction.FaxingAmount + " " + FaxerCurrency +
                    "&ReceiverName=" + receiverDetails.FirstName + " " + receiverDetails.MiddleName + " " + receiverDetails.LastName +
                    "&ReceiverCountry=" + ReceiverCountry + "&ReceiverCity=" + receiverDetails.City + "&SentDate=" + nonCardTransaction.TransactionDate);

                mail.SendMail(FaxerEmail, "Transaction Cancellation Request", body);


                string body2 = "";
                body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MoneyFexTransactionCancellation?NameOfCancellar=" + FaxerName
                    + "&SenderName=" + FaxerName + "&ReceiverName=" + receiverDetails.FirstName + " " + receiverDetails.MiddleName + " " + receiverDetails.LastName +
                    "&MFCN=" + nonCardTransaction.MFCN + "&TransactionAmount=" + nonCardTransaction.FaxingAmount + " " + FaxerCurrency);

                //mail.SendMail("transactioncellation@moneyfex.com", "Alert - Transaction Cancellation " + "MFCN {" + nonCardTransaction.MFCN + "}", body2);
                mail.SendMail("refund@moneyfex.com", "Alert - Transaction Cancellation " + "MFCN" + nonCardTransaction.MFCN, body2);


                return RedirectToAction("Index", "SenderTransactionHistory", new { transactionService = TransactionServiceType.CashPickUp });
                //end 

            }
            return RedirectToAction("Index", "SenderCashPickUpDetail");
        }
    }
}