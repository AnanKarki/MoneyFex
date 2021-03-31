using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;


namespace FAXER.PORTAL.Controllers.Dashboard
{
    public class TrackAFaxController : Controller
    {
        DB.FAXEREntities context = new DB.FAXEREntities();
        // GET: TrackAFax
        public ActionResult Index(string searchText = "")
        {
            List<Models.TrackAFaxViewModel> data = new List<Models.TrackAFaxViewModel>();
            if (!string.IsNullOrEmpty(searchText))
            {
                data = (from c in context.FaxingNonCardTransaction.ToList()
                        join d in context.ReceiversDetails on c.NonCardRecieverId equals d.Id
                        where (c.MFCN.ToLower() == searchText.ToLower() || (d.FirstName.ToLower() + " " + d.LastName.ToLower()) == searchText.ToLower())
                        select new Models.TrackAFaxViewModel()
                        {
                            Id = c.Id,
                            ReceiverName = (d.FirstName + " " + d.LastName),
                            ReceiverCity = d.City,
                            ReceiverCountry = Common.Common.GetCountryName(d.Country),
                            FaxedAmount = c.FaxingAmount.ToString("#00.00") + " " + Common.Common.GetCountryCurrency(d.Country),
                            FaxedDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                            FaxedTime = c.TransactionDate.ToString("hh:mm"),
                            MoneyFaxControlNumber = c.MFCN,
                            StatusOfFax = c.FaxingStatus
                        }).ToList();
                FaxerSession.MFCN = searchText;
            }
            return View(data);
        }
        [HttpPost]
        public ActionResult CancelFax(int Id)
        {
            if (Id != 0)
            {
                var nonCardTransaction = context.FaxingNonCardTransaction.Where(x => x.Id == Id).FirstOrDefault();
                if (nonCardTransaction.FaxingStatus == FaxingStatus.Cancel || nonCardTransaction.FaxingStatus == FaxingStatus.Received || nonCardTransaction.FaxingStatus == FaxingStatus.Refund)
                {
                    return RedirectToAction("Index", "TrackAFax", new { searchText = FaxerSession.MFCN });
                }
                else
                {
                    nonCardTransaction.FaxingStatus = FaxingStatus.Cancel;
                    context.Entry(nonCardTransaction).State = EntityState.Modified;
                    context.SaveChanges();
                    //send email for Fax Cancellation 
                    MailCommon mail = new MailCommon();

                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    string FaxerName = Common.FaxerSession.LoggedUser.FullName;
                    string FaxerEmail = Common.FaxerSession.LoggedUser.UserName;
                    var receiverDetails = context.ReceiversDetails.Where(x => x.Id == nonCardTransaction.NonCardRecieverId).FirstOrDefault();
                    string body = "";
                    string ReceiverCounty = Common.Common.GetCountryName(receiverDetails.Country);
                    string FaxerCurrency = Common.Common.GetCountryCurrency(Common.FaxerSession.LoggedUser.CountryCode);
                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxCancellationEmail?FaxerName=" + FaxerName +
                        "&MFCN=" + nonCardTransaction.MFCN + "&SentAmount=" + nonCardTransaction.FaxingAmount + " " + FaxerCurrency +
                         "&ReceiverName=" + receiverDetails.FirstName + " " + receiverDetails.MiddleName + " " + receiverDetails.LastName +
                        "&ReceiverCountry=" + ReceiverCounty + "&ReceiverCity=" + receiverDetails.City + "&SentDate=" + nonCardTransaction.TransactionDate);

                    mail.SendMail(FaxerEmail, "Transaction Cancellation Request", body);
                    string body2 = "";
                    body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MoneyFexTransactionCancellation?NameOfCancellar=" + FaxerName
                        + "&SenderName=" + FaxerName + "&ReceiverName=" + receiverDetails.FirstName + " " + receiverDetails.MiddleName + " " + receiverDetails.LastName +
                        "&MFCN=" + nonCardTransaction.MFCN + "&TransactionAmount=" + nonCardTransaction.FaxingAmount + " " + FaxerCurrency);

                    //mail.SendMail("transactioncellation@moneyfex.com", "Alert - Transaction Cancellation " + "MFCN {" + nonCardTransaction.MFCN + "}", body2);
                    mail.SendMail(FaxerEmail, "Alert - Transaction Cancellation " + "MFCN = " + nonCardTransaction.MFCN, body2);

                    //end 

                }
            }
            return RedirectToAction("Index", "TrackAFax", new { searchText = FaxerSession.MFCN });
        }
    }
}