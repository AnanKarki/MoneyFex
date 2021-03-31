using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Dashboard
{
    public class FaxingHistoryController : Controller
    {
        DB.FAXEREntities context = new DB.FAXEREntities();
        // GET: FaxingHistory
        public ActionResult Index(string searchText = "", string FromDate = "", string ToDate = "")
        {
            List<Models.TrackAFaxViewModel> data = new List<Models.TrackAFaxViewModel>();
            int FaxerId = Common.FaxerSession.LoggedUser.Id;


            if (string.IsNullOrEmpty(FromDate) || string.IsNullOrEmpty(ToDate))
            {

                if (!string.IsNullOrEmpty(searchText))
                {


                    if (Common.FaxerSession.TrackATransfer == true)
                    {
                        var ValidTransactionByFaxer = context.FaxingNonCardTransaction.Where(x => x.MFCN == searchText && x.NonCardReciever.FaxerID == FaxerId).FirstOrDefault();
                        if (ValidTransactionByFaxer == null)
                        {

                            TempData["ValidData"] = "This Transction is not made by You";

                            return RedirectToAction("FaxDetails", "TrackFax");

                        }
                    }
                    data = (from c in context.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.Received || x.FaxingStatus == FaxingStatus.Refund || x.FaxingStatus == FaxingStatus.Cancel).OrderByDescending(x => x.TransactionDate).ToList()
                            join d in context.ReceiversDetails.Where(x => x.FaxerID == FaxerId) on c.NonCardRecieverId equals d.Id
                            where (c.MFCN.ToLower() == searchText.ToLower() || (d.FirstName.ToLower() + " " + d.LastName.ToLower()) == searchText.ToLower()  || (d.FirstName.ToLower() == searchText.ToLower()) ||  (d.LastName.ToLower()== searchText.ToLower()) )
                            select new Models.TrackAFaxViewModel()
                            {
                                Id = d.Id,
                                ReceiverName = (d.FirstName + " " + d.LastName),
                                ReceiverCity = d.City,
                                ReceiverCountryCode = d.Country,
                                ReceiverCountry = Common.Common.GetCountryName(d.Country),
                                FaxedAmount = c.FaxingAmount.ToString("#00.00") + " " + Common.Common.GetCountryCurrency(d.FaxerInformation.Country),
                                FaxedDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                FaxedTime = c.TransactionDate.ToString("HH:mm"),
                                MoneyFaxControlNumber = c.MFCN,
                                StatusOfFax = c.FaxingStatus,
                                Faxingstatus = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)
                            }).ToList();
                }
                else
                {
                    data = (from c in context.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.Received || x.FaxingStatus == FaxingStatus.Refund || x.FaxingStatus == FaxingStatus.Cancel).ToList().OrderByDescending(x => x.TransactionDate)
                            join d in context.ReceiversDetails.Where(x => x.FaxerID == FaxerId) on c.NonCardRecieverId equals d.Id
                            select new Models.TrackAFaxViewModel()
                            {
                                Id = d.Id,
                                ReceiverName = (d.FirstName + " " + d.LastName),
                                ReceiverCity = d.City,
                                ReceiverCountryCode = d.Country,
                                ReceiverCountry = Common.Common.GetCountryName(d.Country),
                                FaxedAmount = c.FaxingAmount.ToString("#00.00") + " " + Common.Common.GetCountryCurrency(d.FaxerInformation.Country),
                                FaxedDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                                FaxedTime = c.TransactionDate.ToString("HH:mm"),
                                MoneyFaxControlNumber = c.MFCN,
                                StatusOfFax = c.FaxingStatus,
                                Faxingstatus = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)
                            }
                            ).ToList();

                }
            }
            else
            {
                data = GetSendingHistoryFilterByDate(FromDate, ToDate);

            }
            Common.FaxerSession.TrackATransfer = false;

            return View(data);
        }

        public List<Models.TrackAFaxViewModel> GetSendingHistoryFilterByDate(string FromDate, string ToDate)
        {


            var fromDate = Convert.ToDateTime(FromDate);
            var toDate = Convert.ToDateTime(ToDate);
            int FaxerId = Common.FaxerSession.LoggedUser.Id;
            var data = (from c in context.FaxingNonCardTransaction.Where(x => (x.FaxingStatus == FaxingStatus.Received || x.FaxingStatus == FaxingStatus.Refund || x.FaxingStatus == FaxingStatus.Cancel)
                        && (DbFunctions.TruncateTime(x.TransactionDate) >= fromDate.Date
                          && DbFunctions.TruncateTime(x.TransactionDate) <= toDate.Date)).ToList().OrderByDescending(x => x.TransactionDate)
                        join d in context.ReceiversDetails.Where(x => x.FaxerID == FaxerId) on c.NonCardRecieverId equals d.Id
                        select new Models.TrackAFaxViewModel()
                        {
                            Id = d.Id,
                            ReceiverName = (d.FirstName + " " + d.LastName),
                            ReceiverCity = d.City,
                            ReceiverCountryCode = d.Country,
                            ReceiverCountry = Common.Common.GetCountryName(d.Country),
                            FaxedAmount = c.FaxingAmount.ToString("#00.00") + " " + Common.Common.GetCountryCurrency(d.FaxerInformation.Country),
                            FaxedDate = c.TransactionDate.ToString("dd/MM/yyyy"),
                            FaxedTime = c.TransactionDate.ToString("HH:mm"),
                            MoneyFaxControlNumber = c.MFCN,
                            StatusOfFax = c.FaxingStatus,
                            Faxingstatus = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)
                        }
                      ).ToList();

            return data;

        }

        public ActionResult SendMoneyAgainToReceiver(string ReceivingCountry, int ReceiverId)
        {


            Common.FaxerSession.ReceivingCountry = ReceivingCountry;
            Common.FaxerSession.NonCardReceiverId = ReceiverId;

            return RedirectToAction("Index", "NonCardMoneyFax");
        }

    }
}