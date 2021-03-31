using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace FAXER.PORTAL.Controllers.Dashboard
{
    public class FaxInProgressController : Controller
    {
        DB.FAXEREntities context = new DB.FAXEREntities();
        // GET: FaxInProgress
        public ActionResult Index(string searchText = "")
        {
            List<Models.TrackAFaxViewModel> data = new List<Models.TrackAFaxViewModel>();
            int FaxerId = Common.FaxerSession.LoggedUser.Id;

            if (!string.IsNullOrEmpty(searchText))
            {

                if (Common.FaxerSession.TrackATransfer == true)
                {
                    var ValidTransactionByFaxer = context.FaxingNonCardTransaction.Where(x => x.MFCN == searchText && x.NonCardReciever.FaxerID == FaxerId).FirstOrDefault();
                    if (ValidTransactionByFaxer == null)
                    {


                        TempData["ValidData"] = "This Transction is  wasn't made by You";

                        return RedirectToAction("FaxDetails", "TrackFax");
                    }
                }
                data = (from c in context.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.NotReceived || x.FaxingStatus == FaxingStatus.Hold).ToList()
                        join d in context.ReceiversDetails.Where(x => x.FaxerID == FaxerId) on c.NonCardRecieverId equals d.Id
                        where (c.MFCN.ToLower() == searchText.ToLower() || (d.FirstName.ToLower() + " " + d.LastName.ToLower()) == searchText.ToLower() 
                        || (d.FirstName.ToLower()) == searchText.ToLower() || (d.LastName.ToLower()) == searchText.ToLower())
                        select new Models.TrackAFaxViewModel()
                        {
                            Id = c.Id,
                            ReceiverName = (d.FirstName + " " + d.LastName),
                            ReceiverCity = d.City,
                            ReceiverCountry = Common.Common.GetCountryName(d.Country),
                            FaxedAmount = c.FaxingAmount.ToString("#00.00") + " " + Common.Common.GetCountryCurrency(FaxerSession.LoggedUser.CountryCode),
                            FaxedDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                            FaxedTime = c.TransactionDate.ToString("HH:mm"),
                            MoneyFaxControlNumber = c.MFCN,
                            OperatingUserType = c.OperatingUserType,
                            StatusOfFax = c.FaxingStatus,
                            Faxingstatus = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)
                        }).ToList();
                FaxerSession.MFCN = searchText;
            }
            else
            {
                data = (from c in context.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.NotReceived ||  x.FaxingStatus == FaxingStatus.Hold).ToList().OrderByDescending(x => x.TransactionDate)
                        join d in context.ReceiversDetails.Where(x => x.FaxerID == FaxerId) on c.NonCardRecieverId equals d.Id
                        select new Models.TrackAFaxViewModel()
                        {
                            Id = c.Id,
                            ReceiverName = (d.FirstName + " " + d.LastName),
                            ReceiverCity = d.City,
                            ReceiverCountry = Common.Common.GetCountryName(d.Country),
                            FaxedAmount = c.FaxingAmount.ToString("#00.00") + " " + Common.Common.GetCountryCurrency(FaxerSession.LoggedUser.CountryCode),
                            FaxedDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                            FaxedTime = c.TransactionDate.ToString("HH:mm"),
                            MoneyFaxControlNumber = c.MFCN,
                            OperatingUserType = c.OperatingUserType,
                            StatusOfFax = c.FaxingStatus,
                            Faxingstatus = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)
                        }).ToList();

            }

            Common.FaxerSession.TrackATransfer = false;

            FaxerSession.MFCN = searchText;
            return View(data);
        }
        [HttpPost]
        public ActionResult CancelFax(int Id)
        {
            if (Id > 0)
            {
                var nonCardTransaction = context.FaxingNonCardTransaction.Where(x => x.Id == Id).FirstOrDefault();
                FaxerSession.MFCN = nonCardTransaction.MFCN;


                
                if (nonCardTransaction.FaxingStatus == FaxingStatus.Hold || nonCardTransaction.FaxingStatus == FaxingStatus.Cancel || nonCardTransaction.FaxingStatus == FaxingStatus.Received || nonCardTransaction.FaxingStatus == FaxingStatus.Refund)
                {
                    return RedirectToAction("Index", "FaxInProgress", new { searchText = FaxerSession.MFCN });
                }
                else
                {
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


                    return RedirectToAction("Index", "FaxingHistory", new { searchText = FaxerSession.MFCN });
                    //end 
                }
            }
            return RedirectToAction("Index", "FaxInProgress");
        }
    }
}