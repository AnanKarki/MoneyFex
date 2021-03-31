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
    public class SenderControlWalletUsageController : Controller
    {
        private SSenderRegisteredWallets _senderRegisteredWalletsServices = null;
        public SenderControlWalletUsageController()
        {
            _senderRegisteredWalletsServices = new SSenderRegisteredWallets();
        }
        // GET: SenderControlWalletUsage
        [HttpGet]
        public ActionResult SenderControlWalletUsageIndex(int WalletId = 0)
        {

            int SenderId = FaxerSession.LoggedUser.Id;
            List<KiiPayPersonalWalletInformation> list = (from c in _senderRegisteredWalletsServices.List().Data.Where(x => x.IsDeleted == false)
                                                          join d in _senderRegisteredWalletsServices.ListofSender().Data.Where(x => x.SenderId == SenderId) on c.Id equals d.KiiPayPersonalWalletId
                                                          select c).ToList();
            SenderControlWalletUsageVM model = new SenderControlWalletUsageVM();
            if (SenderId == 0)
            {
                ViewBag.Wallets = new SelectList(list, "Id", "FirstName");
            }
            else
            {

                ViewBag.Wallets = new SelectList(list, "Id", "FirstName", WalletId);

            }

            ViewBag.LimitTypes = new SelectList(GetLimitType(), "Id", "Name");

            return View(model);
        }


        [HttpPost]
        public ActionResult SenderControlWalletUsageIndex([Bind(Include = SenderControlWalletUsageVM.BindProperty)]SenderControlWalletUsageVM vm)
        {

            int SenderId = FaxerSession.LoggedUser.Id;
            List<KiiPayPersonalWalletInformation> list = (from c in _senderRegisteredWalletsServices.List().Data.Where(x => x.IsDeleted == false)
                                                          join d in _senderRegisteredWalletsServices.ListofSender().Data.Where(x => x.SenderId == SenderId) on c.Id equals d.KiiPayPersonalWalletId
                                                          select c).ToList();
            ViewBag.Wallets = new SelectList(list, "Id", "FirstName");
            ViewBag.LimitTypes = new SelectList(GetLimitType(), "Id", "Name");
            if (ModelState.IsValid)
            {

                var WalletInfo = _senderRegisteredWalletsServices.ListofSender().Data.Where(x => x.KiiPayPersonalWalletId == vm.WalletId).FirstOrDefault();

                SenderControlWalletUsageAmountVM amountVm = new SenderControlWalletUsageAmountVM()
                {

                    PreviousAmount = WalletInfo.CashWithdrawalLimitAmount,
                    Currency = Common.Common.GetCountryCurrency(WalletInfo.KiiPayPersonalWalletInformation.CardUserCountry),
                    CurrencySymbol = Common.Common.GetCurrencySymbol(WalletInfo.KiiPayPersonalWalletInformation.CardUserCountry),
                    WalletId = vm.WalletId,
                    LimitTypeId = vm.LimitTypeId,
                    AvailableBal = WalletInfo.KiiPayPersonalWalletInformation.CurrentBalance
                };

                Common.FaxerSession.SenderControlWalletUsageAmountVM = amountVm;
                return RedirectToAction("SenderControlWalletUsageAmount");
            }
            return View(vm);
        }


        public List<SenderWalletDropDownVM> GetSenderWallet()
        {
            var result = new List<SenderWalletDropDownVM>();
            return result;

        }
        public List<LimitTypeDropDownVM> GetLimitType()
        {
            var result = new List<LimitTypeDropDownVM>();

            var limit1 = new LimitTypeDropDownVM()
            {
                Id = 1,
                Name = "Purchase"
            };
            var limit2 = new LimitTypeDropDownVM()
            {
                Id = 2,
                Name = "Withdrawal"
            };
            result.Add(limit1);
            result.Add(limit2);
            return result;

        }


        [HttpGet]
        public ActionResult SenderControlWalletUsageAmount()
        {

            var model = _senderRegisteredWalletsServices.GetSenderControlWalletUsageAmount();

            return View(model);
        }

        [HttpPost]
        public ActionResult SenderControlWalletUsageAmount([Bind(Include = SenderControlWalletUsageAmountVM.BindProperty)]SenderControlWalletUsageAmountVM vm)
        {
            if (ModelState.IsValid)
            {
                SenderKiiPayPersonalAccount successmodel = _senderRegisteredWalletsServices.ListofSender().Data.Where(x => x.KiiPayPersonalWalletId == vm.WalletId).FirstOrDefault();
                if (vm.LimitTypeId == 2)
                {
                    successmodel.CashWithdrawalLimitAmount = vm.Amount;
                    successmodel.CashLimitType = vm.LimitFrequencyId;
                }
                else
                {
                    successmodel.GoodPurchaseLimitAmount = vm.Amount;
                    successmodel.GoodFrequency = vm.GoodFrequencyId;
                }

                var updatemodal = _senderRegisteredWalletsServices.UpdateSender(successmodel);
                SenderControlWalletUsageVM successModel = new SenderControlWalletUsageVM()
                {
                    Balance = vm.AvailableBal,
                    CurrencySymbol = vm.CurrencySymbol
                };
                return RedirectToAction("SenderControlWalletUsageSuccess", successModel);
            }
            return View(vm);
        }
        public ActionResult SenderControlWalletUsageSuccess([Bind(Include = SenderControlWalletUsageVM.BindProperty)]SenderControlWalletUsageVM model)
        {

            return View(model);
        }

    }
}