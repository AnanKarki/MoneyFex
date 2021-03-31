using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class TrackFaxController : Controller
    {
        FAXER.PORTAL.DB.FAXEREntities dbContext = new DB.FAXEREntities();

        // GET: TrackFax
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FaxDetails()
        {
            return View();
        }
        [HttpPost]
        public ActionResult FaxDetails([Bind(Include = TrackFax.BindProperty)]TrackFax model)
        {
            FAXER.PORTAL.Models.TrackFaxDetails faxDetails = new Models.TrackFaxDetails();
            if (ModelState.IsValid)
            {

                // Merchant Transfer 
                if (model.MoneyFaxControlNumber.Contains("SP"))
                {
                    faxDetails = (from c in dbContext.MerchantNonCardTransaction.Where(x => x.MFCN == model.MoneyFaxControlNumber).ToList()
                                  select new Models.TrackFaxDetails()
                                  {
                                      MFCNNumber = c.MFCN,
                                      SenderSurName = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == c.MFBCCardID).Select(x => x.LastName).FirstOrDefault(),
                                      //StatusOfFax = Enum.GetName(typeof(FaxingStatus), c.FaxingStatus)
                                      StatusOfFax = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)
                                  }).FirstOrDefault();
                }
                // CardUser Transfer
                else if (model.MoneyFaxControlNumber.Contains("CU"))
                {

                    faxDetails = (from c in dbContext.CardUserNonCardTransaction.Where(x => x.MFCN == model.MoneyFaxControlNumber ).ToList()
                                  select new Models.TrackFaxDetails()
                                  {
                                      MFCNNumber = c.MFCN,
                                      SenderSurName = c.CardUserReceiverDetails.MFTCCardInformation.LastName,
                                      //StatusOfFax = Enum.GetName(typeof(FaxingStatus), c.FaxingStatus)
                                      StatusOfFax = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)
                                  }).FirstOrDefault();
                }
                else
                {

                    faxDetails = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == model.MoneyFaxControlNumber).ToList()
                                  select new Models.TrackFaxDetails()
                                  {
                                      MFCNNumber = c.MFCN,
                                      SenderSurName = c.NonCardReciever.FaxerInformation.LastName,
                                      //StatusOfFax = Enum.GetName(typeof(FaxingStatus), c.FaxingStatus)
                                      StatusOfFax = Common.Common.GetEnumDescription((FaxingStatus)c.FaxingStatus)
                                  }).FirstOrDefault();
                }
                if (faxDetails != null)
                {
                    if (faxDetails.SenderSurName.ToLower() == model.FaxerSurNam.ToLower())
                    {
                        model.FaxingStatus = faxDetails.StatusOfFax;
                        FaxerSession.TrackAFaxFaxDetails = model;
                        return View(faxDetails);
                    }
                    else {
                        TempData["ValidData"] = "Sender Lastname doesn't match";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["ValidData"] = "Please enter a valid MFCN Number";
                }
            }
            else
            {
                TempData["ValidData"] = "Sender Lastname and MFCN is required";
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }
         
        [HttpGet]
        public ActionResult TrackFaxFromDashboard()
        {

            string faxingStatus = FaxerSession.TrackAFaxFaxDetails.FaxingStatus;
            string MFCN = FaxerSession.TrackAFaxFaxDetails.MoneyFaxControlNumber;
            Common.FaxerSession.TrackATransfer = true;

            if (faxingStatus.ToLower() == "not received")
            {
                if (Common.FaxerSession.LoggedUser == null)
                {
                    //Common.FaxerSession.FromUrl = "/TrackFax/TrackFaxFromDashboard";
                    Common.FaxerSession.FromUrl = "/FaxInProgress/Index?searchText=" + MFCN;
                    return RedirectToAction("Login", "FaxerAccount");

                }
                return RedirectToAction("Index", "FaxInProgress", new { searchText = MFCN });

            }
            else
            {

                if (Common.FaxerSession.LoggedUser == null)
                {
                    //Common.FaxerSession.FromUrl = "/TrackFax/TrackFaxFromDashboard";
                    Common.FaxerSession.FromUrl = "/FaxingHistory/Index?searchText=" + MFCN;
                    return RedirectToAction("Login", "FaxerAccount");

                }
                return RedirectToAction("Index", "FaxingHistory", new { searchText = MFCN });
            }

            //if (FaxerSession.TrackAFaxFaxDetails.MoneyFaxControlNumber != null)
            //{
            //    List<Models.TrackAFaxViewModel> data = new List<Models.TrackAFaxViewModel>();
            //    string searchText = FaxerSession.TrackAFaxFaxDetails.MoneyFaxControlNumber;
            //    if (!string.IsNullOrEmpty(searchText))
            //    {
            //        data = (from c in dbContext.FaxingNonCardTransaction.ToList()
            //                join d in dbContext.ReceiversDetails on c.NonCardRecieverId equals d.Id
            //                where (c.MFCN.ToLower() == searchText.ToLower() || (d.FirstName.ToLower() + " " + d.LastName.ToLower()) == searchText.ToLower())
            //                select new Models.TrackAFaxViewModel()
            //                {
            //                    Id = d.Id,
            //                    ReceiverName = (d.FirstName + " " + d.LastName),
            //                    ReceiverCity = d.City,
            //                    ReceiverCountry = d.Country,
            //                    FaxedAmount = c.FaxingAmount.ToString("#00.00"),
            //                    FaxedDate = c.TransactionDate.ToString("MM/dd/yyyy"),
            //                    FaxedTime = c.TransactionDate.ToString("hh:mm"),
            //                    MoneyFaxControlNumber = c.MFCN,
            //                    StatusOfFax = Enum.GetName(typeof(FaxingStatus), c.FaxingStatus)
            //                }).ToList();
            //    }
            //    return View(data);
            //}
            //else
            //{

            //}
            return View();
        }
        [HttpPost]
        public ActionResult TrackFaxFromDashboard(string searchText = "")
        {
            List<Models.TrackAFaxViewModel> data = new List<Models.TrackAFaxViewModel>();

            if (!string.IsNullOrEmpty(searchText))
            {
                data = (from c in dbContext.FaxingNonCardTransaction.ToList()
                        join d in dbContext.ReceiversDetails on c.NonCardRecieverId equals d.Id
                        where (c.MFCN.ToLower() == searchText.ToLower() || (d.FirstName.ToLower() + " " + d.LastName.ToLower()) == searchText.ToLower())
                        select new Models.TrackAFaxViewModel()
                        {
                            Id = d.Id,
                            ReceiverName = (d.FirstName + " " + d.LastName),
                            ReceiverCity = d.City,
                            ReceiverCountry = d.Country,
                            FaxedAmount = c.FaxingAmount.ToString("#00.00"),
                            FaxedDate = c.TransactionDate.ToString("MM/dd/yyyy"),
                            FaxedTime = c.TransactionDate.ToString("hh:mm"),
                            MoneyFaxControlNumber = c.MFCN,
                            StatusOfFax = c.FaxingStatus
                        }).ToList();
            }
            return View(data);
        }
    }
}