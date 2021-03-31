using FAXER.PORTAL.Areas.Admin.Services;
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
    public class TransferMoneyBetweenMoneyFaxTopUpCardController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        CommonServices CommonService = new CommonServices();
        // GET: TransferMoneyBetweenMoneyFaxTopUpCard
        [HttpGet]
        public ActionResult Index(int mFTCCardInformationId = 0)
        {
            TransferMoneyBetweenMoneyFaxTopUpCardViewModel vm = new TransferMoneyBetweenMoneyFaxTopUpCardViewModel();
            if ((FaxerSession.LoggedUser == null))
            {
                FaxerSession.FromUrl = "/TransferMoneyBetweenMoneyFaxTopUpCard";
                return RedirectToAction("Login", "FaxerAccount");
            }
            List<KiiPayPersonalWalletInformation> list = (from c in dbContext.KiiPayPersonalWalletInformation
                                                          join d in dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == FaxerSession.LoggedUser.Id) on c.Id equals d.KiiPayPersonalWalletId
                                                          select c).ToList();
            foreach (var item in list)
            {
                var MFTCcard = item.MobileNo.Decrypt();
                item.MobileNo = Common.Common.FormatMFTCCard(MFTCcard);
            }
            if (mFTCCardInformationId == 0)
            {
                ViewBag.TopUpCard = new SelectList(list, "Id", "MFTCCardNumber");
            }
            else
            {
                ViewBag.TopUpCard = new SelectList(list, "Id", "MFTCCardNumber", mFTCCardInformationId);
                vm = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == mFTCCardInformationId && x.IsDeleted == false).ToList()
                      join d in dbContext.Country on c.CardUserCountry equals d.CountryCode
                      select new TransferMoneyBetweenMoneyFaxTopUpCardViewModel()
                      {
                          Id = c.Id,
                          TransferringTopUpCardAmount = (double)c.CurrentBalance,
                          TransferringTopUpCardRegisteredCountry = d.CountryName,
                          TransferringTopUpCardRegisteredCountryCode = c.CardUserCountry,
                          CountryCurrency = Common.Common.GetCountryCurrency(c.CardUserCountry),
                          CountryCurrencySymbol = CommonService.getCurrencySymbol(c.CardUserCountry)
                      }).FirstOrDefault();

            }
            

            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = TransferMoneyBetweenMoneyFaxTopUpCardViewModel.BindProperty)]TransferMoneyBetweenMoneyFaxTopUpCardViewModel model)
        {
            if (model.Id == 0)
            {
                ModelState.AddModelError("CardError", "Please select virtual account ");
            }
            else if (model.TransferringTopUpCardAmount == 0)
            {
                ModelState.AddModelError("CardAmount", "You don't have sufficient balance to transfer");
            }
            else if (model.AmountToBeTransferred == 0)
            {
                ModelState.AddModelError("TransferAmount", "Amount to be transferred is required");
            }
            else if ((decimal)model.TransferringTopUpCardAmount < model.AmountToBeTransferred)
            {
                ModelState.AddModelError("TransferAmount", "Your Card doesn't have sufficient balance");
            }
            else
            {

                FaxerSession.TransferringTopUpCardRegisteredCountry = model.TransferringTopUpCardRegisteredCountryCode;
                FaxerSession.AmountToBeTransferred = model.AmountToBeTransferred;
                FaxerSession.TransferCardId = model.Id;
                return RedirectToAction("ReceivingMoneyFaxTopUp", "TransferMoneyBetweenMoneyFaxTopUpCard");
            }
            //List<MFTCCardInformation> list = dbContext.MFTCCardInformation.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).OrderBy(x => x.FirstName).ToList();
            //ViewBag.TopUpCard = new SelectList(list, "Id", "FirstName");
            //List<KiiPayPersonalWalletInformation> list = dbContext.MFTCCardInformation.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id && x.IsDeleted == false).OrderBy(x => x.FirstName).ToList();

            List<KiiPayPersonalWalletInformation> list = (from c in dbContext.KiiPayPersonalWalletInformation.Where( x => x.IsDeleted == false)
                                                         join d in dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == FaxerSession.LoggedUser.Id) on c.Id equals d.KiiPayPersonalWalletId
                                                         select c).ToList();
            foreach (var item in list)
            {
                var MFTCcard = item.MobileNo.Decrypt();
                item.MobileNo = Common.Common.FormatMFTCCard(MFTCcard);
            }
            ViewBag.TopUpCard = new SelectList(list, "Id", "MFTCCardNumber");

            return View(model);
        }

        [HttpGet]
        public ActionResult ReceivingMoneyFaxTopUp(int MFTCCardId = 0)
        {
            ReceivingMoneyBetweenMoneyFaxTopUpCardViewModel vm = new ReceivingMoneyBetweenMoneyFaxTopUpCardViewModel();

            List<KiiPayPersonalWalletInformation> list = (from c in dbContext.KiiPayPersonalWalletInformation
                                                          join d in dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == FaxerSession.LoggedUser.Id) on c.Id equals d.KiiPayPersonalWalletId
                                                          select c).ToList();
            foreach (var item in list)
            {
                var MFTCcard = item.MobileNo.Decrypt();
                item.MobileNo = Common.Common.FormatMFTCCard(MFTCcard);
            }
            if (MFTCCardId == 0)
            {
                ViewBag.TopUpCard = new SelectList(list, "Id", "MFTCCardNumber");
            }
            else
            {
                ViewBag.TopUpCard = new SelectList(list, "Id", "MFTCCardNumber", MFTCCardId);
                vm = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId && x.IsDeleted == false).ToList()
                      join d in dbContext.Country on c.CardUserCountry equals d.CountryCode
                      select new ReceivingMoneyBetweenMoneyFaxTopUpCardViewModel()
                      {
                          Id = c.Id,
                          ReceivingTopUpCardAmount = c.CurrentBalance,
                          ReceivingTopUpCardRegisteredCountry = d.CountryName,
                          CountryCurrency = Common.Common.GetCountryCurrency(c.CardUserCountry),
                          CountryCurrencySymbol = CommonService.getCurrencySymbol(c.CardUserCountry)
                      }).FirstOrDefault();
            }
            return View(vm);
        }
        [HttpPost]
        [PreventSpam]
        public ActionResult ReceivingMoneyFaxTopUp([Bind(Include = ReceivingMoneyBetweenMoneyFaxTopUpCardViewModel.BindProperty)]ReceivingMoneyBetweenMoneyFaxTopUpCardViewModel model)
        {


            // if User Try to Post the form multiple times 

            if (!ModelState.IsValid) {

                return RedirectToAction("GreatMessage", "TransferMoneyBetweenMoneyFaxTopUpCard");
            }

            SFaxingTopUpCardTransaction sFaxingTopUpCardServices = new SFaxingTopUpCardTransaction();
            
            bool valid = true;
            if (model.Confirm == false)
            {
                ModelState.AddModelError("Confirm", "Please confirm before continue.");
                valid = false;
            }
            if (model.Id == 0)
            {
                ModelState.AddModelError("TopUpError", "Please select virtual account");
                valid = false;
            }
            if (valid)
            {
                var receivingCard = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == model.Id).FirstOrDefault();
                receivingCard.CurrentBalance += FaxerSession.AmountToBeTransferred;
                dbContext.Entry(receivingCard).State = EntityState.Modified;
                dbContext.SaveChanges();

                var transferingCard = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == FaxerSession.TransferCardId).FirstOrDefault();
                transferingCard.CurrentBalance -= FaxerSession.AmountToBeTransferred;
                dbContext.Entry(transferingCard).State = EntityState.Modified;
                dbContext.SaveChanges();

                DB.KiiPayPersonalWalletInterMoneyTransfered interMoneyTransfered = new KiiPayPersonalWalletInterMoneyTransfered()
                {
                    SendingKiiPayWalletId = FaxerSession.TransferCardId,
                    ReceivingKiiPayWalletId = model.Id,
                    AmountTransfered = FaxerSession.AmountToBeTransferred,
                    SenderId = Common.FaxerSession.LoggedUser.Id,
                    TransactionDate = DateTime.Now
                };

                dbContext.MFTCCardInterMoneyTransfered.Add(interMoneyTransfered);
                dbContext.SaveChanges();

                // Check if the balance on Transfering card is zero and auto top up is enabled or not 
                if (transferingCard.CurrentBalance == 0) {

                    Areas.CardUsers.Services.CardUserCommonServices cardUserCommonServices = new Areas.CardUsers.Services.CardUserCommonServices();
                    cardUserCommonServices.SendMailWhenBalanceISZero(transferingCard.Id);

                    if (transferingCard.AutoTopUp == true) {



                        cardUserCommonServices.AutoTopUp(transferingCard.Id);
                    }
                }


                return RedirectToAction("GreatMessage", "TransferMoneyBetweenMoneyFaxTopUpCard");
            }

            //List<MFTCCardInformation> list = dbContext.MFTCCardInformation.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id && x.CardUserCountry == FaxerSession.TransferringTopUpCardRegisteredCountry && x.Id != FaxerSession.TransferCardId).OrderBy(x => x.FirstName).ToList();
            //ViewBag.TopUpCard = new SelectList(list, "Id", "FirstName");

            var list = sFaxingTopUpCardServices.GetMFTCCardList();

            ViewBag.TopUpCard = new SelectList(list, "Id", "MFTCCardNumber");


            return View();
        }
        public ActionResult GreatMessage()
        {
            return View();
        }
    }
}