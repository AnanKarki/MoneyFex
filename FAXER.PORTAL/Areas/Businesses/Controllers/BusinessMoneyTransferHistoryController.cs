using FAXER.PORTAL.Areas.Businesses.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class BusinessMoneyTransferHistoryController : Controller
    {
        BusinessMoneyTransferHistoryServices _businessMoneyTransferHistoryServices = null;
        public BusinessMoneyTransferHistoryController()
        {
            _businessMoneyTransferHistoryServices = new BusinessMoneyTransferHistoryServices();
        }

        // GET: Businesses/BusinessMoneyTransferHistory
        public ActionResult Index(string searchText = "")
        {

            var vm = _businessMoneyTransferHistoryServices.getTransactionhistoryList(searchText);
            return View(vm);
        }

        [HttpGet]
        public ActionResult SenderThisReceiverAgain(int ReceiverId)
        {

            var receiverdetails = _businessMoneyTransferHistoryServices.GetReceiversDetails(ReceiverId);

       

            ViewModels.NonCardPaymentViewModel payingAmountvm = new ViewModels.NonCardPaymentViewModel()
            {
                IncludingFee = false,
                ReceivingAmount = 0,
                ReceivingCountry = receiverdetails.Country,
                TopUpAmount = 0
            };
            Common.BusinessSession.NonCardPaymentViewModel = payingAmountvm;
            Common.BusinessSession.NonCardReceiverId_merchantNonCard = receiverdetails.Id;
            Common.BusinessSession.ReceivingCountry = receiverdetails.Country;
            Common.BusinessSession.ReceivingCurrency = Common.Common.GetCountryCurrency(receiverdetails.Country);
            Common.BusinessSession.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(receiverdetails.Country);
                
            return RedirectToAction("Index", "NoCardPaymentByMerchant");

        }
    }
}