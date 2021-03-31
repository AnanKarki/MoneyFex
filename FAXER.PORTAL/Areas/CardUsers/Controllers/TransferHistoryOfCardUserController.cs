using FAXER.PORTAL.Areas.CardUsers.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class TransferHistoryOfCardUserController : Controller
    {

        CardUser_TransferHistoryServices _cardUser_TransferHistoryServices = null;
        public TransferHistoryOfCardUserController()
        {
            _cardUser_TransferHistoryServices = new CardUser_TransferHistoryServices();
        }
        // GET: CardUsers/TransferHistoryOfCardUser
        public ActionResult Index(string searchText = "")
        {
            var vm = _cardUser_TransferHistoryServices.getTransactionhistoryList(searchText);
            return View(vm);
            return View();
        }
        [HttpGet]
        public ActionResult SenderThisReceiverAgain(int ReceiverId)
        {

            var receiverdetails = _cardUser_TransferHistoryServices.GetReceiversDetails(ReceiverId);
            

            
            ViewModels.CardUser_NonCardPayingAmountViewModel payingAmountvm = new ViewModels.CardUser_NonCardPayingAmountViewModel()
            {
                IncludingFee = false,
                ReceivingAmount = 0,
                ReceivingCountry = receiverdetails.Country,
                TopUpAmount = 0
            };
            Common.CardUserSession.CardUser_NonCardPayingAmountViewModel = payingAmountvm;
            Common.CardUserSession.NonCardReceiverId = receiverdetails.Id;
            Common.CardUserSession.ReceivingCountry = receiverdetails.Country;
            Common.CardUserSession.ReceivingCurrency = Common.Common.GetCountryCurrency(receiverdetails.Country);
            Common.CardUserSession.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(receiverdetails.Country);

            return RedirectToAction("PayingAmount", "NonCardPaymentByCardUser");

        }
    }
}