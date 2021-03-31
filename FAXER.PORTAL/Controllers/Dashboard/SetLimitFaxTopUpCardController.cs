using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Dashboard
{
    /// <summary>
    /// Set Limit in Receiver Virtual Account 
    /// </summary>
    public class SetLimitFaxTopUpCardController : Controller
    {
        DB.FAXEREntities context = new DB.FAXEREntities();
        // GET: SetLimitFaxTopUpCard
        public ActionResult Index(int mftcCardId = 0)
        {
            int FaxerInformationId = FaxerSession.LoggedUser.Id;
            List<KiiPayPersonalWalletInformation> list =(from c in  context.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false)
                                                        join d in context.SenderKiiPayPersonalAccount.Where(x => x.SenderId == FaxerInformationId) on c.Id equals d.KiiPayPersonalWalletId
                                                        select c).ToList();
            foreach (var item in list)
            {
                var MFTCcard = item.MobileNo.Decrypt();
                
                item.MobileNo =  Common.Common.FormatMFTCCard(MFTCcard);
            }
            SetLimitFaxTopUpCardViewModel vm = new SetLimitFaxTopUpCardViewModel();
            if (mftcCardId == 0)
            {
                ViewBag.MFTCCardNumber = new SelectList(list, "Id", "MFTCCardNumber");
            }
            else
            {
                ViewBag.MFTCCardNumber = new SelectList(list, "Id", "MFTCCardNumber", mftcCardId);
                vm = (from c in context.KiiPayPersonalWalletInformation.Where(x => x.Id == mftcCardId && x.IsDeleted == false && (x.CardStatus != CardStatus.IsDeleted && x.CardStatus != CardStatus.IsRefunded)).ToList()
                      select new SetLimitFaxTopUpCardViewModel()
                      {
                          CurrentBalance = c.CurrentBalance,
                          PreviousCashWithDrawLimit = c.CashWithdrawalLimit,
                          MFTCCardId = c.Id,
                          PreviousPurchasingLimit = c.GoodsPurchaseLimit,
                          WithDrawAutoPaymentFrequency = c.CashLimitType,
                          PurchasingAutoPaymentFrequency = c.GoodsLimitType,
                          CardUserCurrency = Common.Common.GetCountryCurrency(c.CardUserCountry)
                      }).FirstOrDefault();
                FaxerSession.MFTCCardInformationId = mftcCardId;
            }
            if (Request.UrlReferrer != null)
            {
                Common.FaxerSession.BackButtonURL = Request.UrlReferrer.ToString();
            }
            return View(vm);
        }


        [HttpPost]
        public ActionResult Index([Bind(Include = SetLimitFaxTopUpCardViewModel.BindProperty)]SetLimitFaxTopUpCardViewModel vm)
        {


            //List<MFTCCardInformation> lst = context.MFTCCardInformation.Where(x => x.FaxerId == FaxerSession.LoggedUser.Id).ToList();
            //foreach (var item in lst)
            //{
            //    var MFTCcard = item.MFTCCardNumber.Decrypt();
            //    item.MFTCCardNumber = Common.Common.FormatMFTCCard(MFTCcard);
            //}
            //ViewBag.MFTCCardNumber = new SelectList(lst, "Id", "MFTCCardNumber");
            if (vm.MFTCCardId != 0)
            {
                //if (vm.PurchasingAmount != 0 && vm.WithDrawAmount != 0)
                if (vm.UpdateBothLimitType == true)
                {
                    var data = context.KiiPayPersonalWalletInformation.Where(x => x.Id == vm.MFTCCardId).FirstOrDefault();
                    data.GoodsPurchaseLimit = vm.PurchasingAmount;
                    data.GoodsLimitType = vm.PurchasingAutoPaymentFrequency;
                    data.CashWithdrawalLimit = vm.WithDrawAmount;
                    data.CashLimitType = vm.WithDrawAutoPaymentFrequency;
                    context.Entry(data).State = EntityState.Modified;
                    context.SaveChanges();
                    return RedirectToAction("Index", "SetLimitFaxTopUpCard");
                }
                //if (vm.PurchasingAmount != 0)
                if (vm.UpdatePurchaseLimit == true)
                {
                    var data = context.KiiPayPersonalWalletInformation.Where(x => x.Id == vm.MFTCCardId).FirstOrDefault();
                    data.GoodsPurchaseLimit = vm.PurchasingAmount;
                    data.GoodsLimitType = vm.PurchasingAutoPaymentFrequency;
                    context.Entry(data).State = EntityState.Modified;
                    context.SaveChanges();
                }
                //if (vm.WithDrawAmount != 0)
                if (vm.UpdateWithdrawlLimit == true)
                {
                    var data = context.KiiPayPersonalWalletInformation.Where(x => x.Id == vm.MFTCCardId).FirstOrDefault();
                    data.CashWithdrawalLimit = vm.WithDrawAmount;
                    data.CashLimitType = vm.WithDrawAutoPaymentFrequency;
                    context.Entry(data).State = EntityState.Modified;
                    context.SaveChanges();
                }
                return RedirectToAction("SetLimitHurray", "SetLimitFaxTopUpCard");
            }
            else
            {
                int FaxerInformationId = FaxerSession.LoggedUser.Id;
                List<KiiPayPersonalWalletInformation> list = (from c in context.KiiPayPersonalWalletInformation
                                                             join d in context.SenderKiiPayPersonalAccount.Where(x => x.SenderId == FaxerInformationId) on c.Id equals d.KiiPayPersonalWalletId
                                                             select c).ToList();
                foreach (var item in list)
                {
                    if (!item.MobileNo.Contains("MF"))
                    {
                        var MFTCcard = item.MobileNo.Decrypt();
                        item.MobileNo = Common.Common.FormatMFTCCard(MFTCcard);
                    }
                }
                ViewBag.MFTCCardNumber = new SelectList(list, "Id", "MFTCCardNumber");
                ModelState.AddModelError("CardError", "Please select receiver virtual account");

            }
            return View(vm);
        }

        public ActionResult SetLimitHurray()
        {
            return View();
        }

    }
}