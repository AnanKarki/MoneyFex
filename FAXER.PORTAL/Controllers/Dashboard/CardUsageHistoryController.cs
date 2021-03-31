using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Dashboard
{
    public class CardUsageHistoryController : Controller
    {
        DB.FAXEREntities dbContext = null;
        SMFTCCardUsageHistory cardUsageHistorySevices = null;
        public CardUsageHistoryController()
        {
            dbContext = new FAXEREntities();
            cardUsageHistorySevices = new SMFTCCardUsageHistory();
        }
        // GET: CardUsageHistory
        public ActionResult Index(int mFTCCardInformationId = 0, int card_usage_HistoryOption = 0, string FromDate = "", string ToDate = "")
        {
            MFTCCardUsageHistoryViewModel vm = new MFTCCardUsageHistoryViewModel();
            if ((FaxerSession.LoggedUser == null))
            {

                return RedirectToAction("Login", "FaxerAccount");
            }
            List<KiiPayPersonalWalletInformation> list = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.IsDeleted == false)
                                                         join d in dbContext.SenderKiiPayPersonalAccount.Where(x => x.SenderId == FaxerSession.LoggedUser.Id ) on c.Id equals d.KiiPayPersonalWalletId
                                                         select c).ToList();
            foreach (var item in list)
            {
                var MFTCcard = item.MobileNo.Decrypt();
                item.MobileNo = Common.Common.FormatMFTCCard(MFTCcard);
            }
            if (mFTCCardInformationId == 0)
            {
                ViewBag.MFTCCard = new SelectList(list, "Id", "MFTCCardNumber");
            }
            else
            {
                FaxerSession.TopUpCardId = mFTCCardInformationId.ToString();
                ViewBag.MFTCCard = new SelectList(list, "Id", "MFTCCardNumber", mFTCCardInformationId);
                vm = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == mFTCCardInformationId && x.IsDeleted == false).ToList()
                      join d in dbContext.Country on c.CardUserCountry equals d.CountryCode
                      select new MFTCCardUsageHistoryViewModel()
                      {
                          CurrentBalance = c.CurrentBalance.ToString(),
                          MFTCCardCurrency = Common.Common.GetCountryCurrency(c.CardUserCountry),
                          MFTCCardCurrencySymbol = Common.Common.GetCurrencySymbol(c.CardUserCountry)
                      }).FirstOrDefault();

                if (string.IsNullOrEmpty(FromDate) && string.IsNullOrEmpty(ToDate))
                {
                    if (card_usage_HistoryOption == (int)CardUsageHistoryOption.TopUp)
                    {

                        var MFTCCardTopUp = cardUsageHistorySevices.GetTopUpDetails(mFTCCardInformationId);

                        vm.MFTCCardTopUpViewModel = MFTCCardTopUp;
                    }
                    else if (card_usage_HistoryOption == (int)CardUsageHistoryOption.Card_withdrawl)
                    {
                        var MFTCCardWithdrawl = cardUsageHistorySevices.GetCashWithDrawlDetails(mFTCCardInformationId);

                        vm.MFTCCardwithdrawlViewModel = MFTCCardWithdrawl;
                    }
                    else if (card_usage_HistoryOption == (int)CardUsageHistoryOption.Card_Purchase)
                    {
                        var MFTCCardMerchantPayment = cardUsageHistorySevices.GetMerchantPaymentDetials(mFTCCardInformationId);

                        vm.MFTCCardBusinessMerchantPaymentViewModel = MFTCCardMerchantPayment;
                    }

                }
                else
                {
                    vm = FilterCardInfoByDate(mFTCCardInformationId, card_usage_HistoryOption, FromDate, ToDate);
                }
                vm.CardHistoryOption = (CardUsageHistoryOption)card_usage_HistoryOption;
                vm.MFTCCardId = mFTCCardInformationId;



            }
            if (Request.UrlReferrer != null)
            {
                Common.FaxerSession.BackButtonURL = Request.UrlReferrer.ToString();
            }
            


            return View(vm);
        }

        public MFTCCardUsageHistoryViewModel FilterCardInfoByDate(int MFTCCardId, int CardUsageHistory, string FromDate, string ToDate)
        {

            MFTCCardUsageHistoryViewModel vm = new MFTCCardUsageHistoryViewModel();

            var model = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).ToList()
                         join d in dbContext.Country on c.CardUserCountry equals d.CountryCode
                         select c).FirstOrDefault();
            var fromDate = Convert.ToDateTime(FromDate);
            var toDate = Convert.ToDateTime(ToDate);


            vm.MFTCCardId = MFTCCardId;
            vm.CardHistoryOption = (CardUsageHistoryOption)CardUsageHistory;

            if (CardUsageHistory == (int)CardUsageHistoryOption.TopUp)
            {


                var MFTCTopupHistory = cardUsageHistorySevices.GetTopUpDetailsFilterByDate(MFTCCardId, fromDate, toDate);
                vm.MFTCCardTopUpViewModel = MFTCTopupHistory;


            }
            else if (CardUsageHistory == (int)CardUsageHistoryOption.Card_withdrawl)
            {
                vm.MFTCCardwithdrawlViewModel = cardUsageHistorySevices.GetCashWithDrawlDetailsFilterByDate(MFTCCardId, fromDate, toDate);
            }
            else if (CardUsageHistory == (int)CardUsageHistoryOption.Card_Purchase)
            {
                vm.MFTCCardBusinessMerchantPaymentViewModel = cardUsageHistorySevices.GetMerchantPaymentDetialsFilterByDate(MFTCCardId, fromDate, toDate);
            }
            else
            {

            }

            vm.CurrentBalance = model.CurrentBalance.ToString();
            vm.MFTCCardCurrency = Common.Common.GetCountryCurrency(model.CardUserCountry);
            vm.MFTCCardCurrencySymbol = Common.Common.GetCurrencySymbol(model.CardUserCountry);
            return vm;


        }
    }
}