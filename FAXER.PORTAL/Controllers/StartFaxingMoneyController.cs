using FAXER.PORTAL.Areas.Admin.Services;
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
    public class StartFaxingMoneyController : Controller
    {
        // GET: StartFaxingMoney
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public ActionResult Index(int mFTCCardInformationId = 0)
        {
            StartFaxingMoneyViewModel vm = new StartFaxingMoneyViewModel();
            if ((FaxerSession.LoggedUser == null))
            {
                var Url = Request.Url.ToString();
                string[] tokens = Url.Split('/');
                if (tokens.Count() < 5)
                {
                    FaxerSession.FromUrl = "/StartFaxingMoney";
                }
                else
                {
                    Common.FaxerSession.FromUrl = "/" + tokens[3] + "/" + tokens[4];
                }
                return RedirectToAction("Login", "FaxerAccount");
            }
            List<KiiPayPersonalWalletInformation> list = (from c in  dbContext.KiiPayPersonalWalletInformation.Where(x =>  x.IsDeleted == false)
                                                         join d in dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == FaxerSession.LoggedUser.Id) on c.Id equals d.KiiPayPersonalWalletId
                                                         select c).OrderBy(x => x.FirstName).ToList();

            foreach (var item in list)
            {
                var MFTCcard = item.MobileNo.Decrypt();
                string[] token = MFTCcard.Split('-');
                item.MobileNo = token[1] + "-" + token[2]  ;
            }
            if (mFTCCardInformationId == 0)
            {
                ViewBag.TopUpCard = new SelectList(list, "Id", "MFTCCardNumber");
            }
            else
            {
                FaxerSession.TopUpCardId = mFTCCardInformationId.ToString();
                ViewBag.TopUpCard = new SelectList(list, "Id", "MFTCCardNumber", mFTCCardInformationId);
                vm = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == mFTCCardInformationId).ToList()
                      join d in dbContext.Country on c.CardUserCountry equals d.CountryCode
                      select new StartFaxingMoneyViewModel()
                      {
                          TotalCardAmount = System.Convert.ToDouble(c.CurrentBalance),
                          ReceivingCountryCode = d.CountryCode,
                          ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(c.CardUserCountry),
                          ReceivingCurrencySymbol = CommonService.getCurrencySymbol(c.CardUserCountry)

                      }).FirstOrDefault();
                FaxerSession.ReceivingCountry = vm.ReceivingCountryCode;
            }

            ViewBag.CardCount = list.Count;

            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = StartFaxingMoneyViewModel.BindProperty)]StartFaxingMoneyViewModel model)
        {
            List<KiiPayPersonalWalletInformation> list = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false)
                                                         join d in dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == FaxerSession.LoggedUser.Id) on c.Id equals d.KiiPayPersonalWalletId
                                                         select c).OrderBy(x => x.FirstName).ToList();
            ViewBag.TopUpCard = new SelectList(list, "Id", "FirstName");
            ViewBag.CardCount = list.Count();
            if (ModelState.IsValid)
            {
                int CardId = Convert.ToInt32(model.TopUpCard);
                var CardStatus = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == CardId).FirstOrDefault().CardStatus;
                if (CardStatus == CardStatus.InActive)
                {

                    ModelState.AddModelError("TopUpCard", "The card is currently deactivated");
                    return View(model);
                }
                else if (CardStatus == CardStatus.IsRefunded)
                {

                    ModelState.AddModelError("TopUpCard", "The card has already been refunded");
                    return View(model);
                }
                else if (CardStatus == CardStatus.IsDeleted)
                {

                    ModelState.AddModelError("TopUpCard", "The card has been deleted");
                    return View(model);
                }
                if (!string.IsNullOrEmpty(Common.FaxerSession.TransactionSummaryUrl))
                {
                    return Redirect(Common.FaxerSession.TransactionSummaryUrl);
                }
                return RedirectToAction("index", "CalculateFaxingFees");
            }
            else
            {

                return View();
            }
        }

        [HttpGet]
        public ActionResult NoCard()
        {
            if ((FaxerSession.LoggedUser == null))
            {
                FaxerSession.FromUrl = "/StartFaxingMoney/NoCard";
                return RedirectToAction("Login", "FaxerAccount");
            }
            return View();
        }
        [HttpPost]
        public ActionResult NoCard(bool submit = true)
        {
            if (submit)
            {
                return RedirectToAction("Index", "NonCardMoneyFax");
            }
            return View();
        }
    }
}