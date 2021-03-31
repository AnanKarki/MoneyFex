using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.MyRegisteredKiiPayWallets
{
    public class SenderTransferMoneyBetweenWalletsController : Controller
    {
        private SSenderRegisteredWallets _senderRegisteredWalletsServices = null;

        public SenderTransferMoneyBetweenWalletsController()
        {
            _senderRegisteredWalletsServices = new SSenderRegisteredWallets();
        }
        // GET: SenderTransferMoneyBetweenWallets
        public ActionResult Index(int TransferringWalletId = 0, int ReceivingWalletId = 0)
        {
            SenderTransferMoneyBetweenWalletsVm vm = new SenderTransferMoneyBetweenWalletsVm();
            List<KiiPayPersonalWalletInformation> ReceivingWallets = SendingWallets().Where(x => x.Id != TransferringWalletId).ToList();

            vm.AvailableBalance = 0.00M;


            if (TransferringWalletId > 0)
            {

                var SendingCountry = SendingWallets().Where(x => x.Id == TransferringWalletId)
                                        .Select(x => x.CardUserCountry).FirstOrDefault();
                ReceivingWallets = ReceivingWallets.Where(x => x.CardUserCountry == SendingCountry).ToList();

            }

            if (TransferringWalletId == 0)
            {
                ViewBag.Wallets = new SelectList(SendingWallets(), "Id", "FirstName");
                vm.AvailableBalance = 0.00M;
            }
            else
            {
                var sendingWalletData = _senderRegisteredWalletsServices.List().Data.
                                      Where(x => x.Id == TransferringWalletId && x.IsDeleted == false).FirstOrDefault();
                var country = Common.Common.GetCountries();
                ViewBag.Wallets = new SelectList(SendingWallets(), "Id", "FirstName", TransferringWalletId);
                vm.Currency = Common.Common.GetCurrencySymbol(sendingWalletData.CardUserCountry);
                vm.AvailableBalance = sendingWalletData.CurrentBalance;
            }
            vm.TransferringWalletId = TransferringWalletId;
            vm.ReceivingWalletId = ReceivingWalletId;
            ViewBag.ReceivingWallets = new SelectList(ReceivingWallets, "Id", "FirstName", ReceivingWalletId);


            return View(vm);
        }

        [HttpPost]

        public ActionResult Index([Bind(Include = SenderTransferMoneyBetweenWalletsVm.BindProperty)]SenderTransferMoneyBetweenWalletsVm model)
        {
            var SenderId = FaxerSession.LoggedUser.Id;
            List<KiiPayPersonalWalletInformation> ReceivingWallets = SendingWallets().Where(x => x.Id != model.TransferringWalletId).ToList();

            if (model.TransferringWalletId == 0)
            {
                ModelState.AddModelError("CardError", "Please select virtual account ");
            }
            else if (model.Amount == 0)
            {
                ModelState.AddModelError("CardAmount", "You don't have sufficient balance to transfer");
            }
            else if (model.Amount == 0)
            {
                ModelState.AddModelError("TransferAmount", "Amount to be transferred is required");
            }
            else if ((decimal)model.AvailableBalance < model.Amount)
            {
                ModelState.AddModelError("TransferAmount", "Your Card doesn't have sufficient balance");
            }
            else
            {
                var ReceivingCard = _senderRegisteredWalletsServices.List().Data.Where(x => x.Id == model.ReceivingWalletId).FirstOrDefault();
                ReceivingCard.CurrentBalance = ReceivingCard.CurrentBalance + model.Amount;
                _senderRegisteredWalletsServices.Update(ReceivingCard);

                var SendeningCard = _senderRegisteredWalletsServices.List().Data.Where(x => x.Id == model.TransferringWalletId).FirstOrDefault();
                SendeningCard.CurrentBalance = SendeningCard.CurrentBalance - model.Amount;
                _senderRegisteredWalletsServices.Update(SendeningCard);

                return RedirectToAction("TransferMoneyBetweenWalletsSuccess", "SenderTransferMoneyBetweenWallets");
            }

            ViewBag.Wallets = new SelectList(SendingWallets(), "Id", "FirstName");
            ViewBag.ReceivingWallets = new SelectList(ReceivingWallets, "Id", "FirstName");
            return View(model);

        }

        public ActionResult TransferMoneyBetweenWalletsSuccess()
        {
            SenderTransferMoneyBetweenWalletSuccess vm = new SenderTransferMoneyBetweenWalletSuccess();
            return View(vm);
        }

        public List<KiiPayPersonalWalletInformation> SendingWallets()
        {
            var SenderId = FaxerSession.LoggedUser.Id;
            var SendingWallets = (from c in _senderRegisteredWalletsServices.List().Data.ToList()
                                  join d in _senderRegisteredWalletsServices.ListofSender().Data.Where(x => x.SenderId == SenderId) on c.Id equals d.KiiPayPersonalWalletId
                                  select c).ToList();
            return SendingWallets;

        }

    }
}