using FAXER.PORTAL.Areas.CardUsers.Services;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class PayGoodsAndServicesAbroadController : Controller
    {
        // GET: CardUsers/PayGoodsAndServicesAbroad
      
        public ActionResult Index()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel == null) {

                return RedirectToAction("Login","CardUserLogin");
            }
            return View();
        }
        [HttpGet]
        public ActionResult PayGoodsAndServicesAbroad()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel == null)
            {
                return RedirectToAction("Login", "CardUserLogin");
            }
            Services.PayGoodsAndServicesAbroadServices services = new Services.PayGoodsAndServicesAbroadServices();
            var model = new ViewModels.PayGoodsAndServicesAbroadViewModel();
            model.Currency = services.getMFTCCardCurrency(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId);
            model.CurrencySymbol = services.getMFTCCurrencySymbol(Common.CardUserSession.LoggedCardUserViewModel.KiiPayPersonalId);
            var Card = services.GetMFTCCardInformation();
            if (Card != null)
            {
                if (Card.CardStatus == DB.CardStatus.IsDeleted)
                {

                    model.AmountOnCard = (decimal)0.0;
                    ViewBag.Vaildation = "Your Card has been Deleted, please contact moneyfex suppport team";
                    return View(model);
                }
                if (Card.CardStatus == DB.CardStatus.InActive)
                {

                    model.AmountOnCard = (decimal)0.0;
                    ViewBag.Vaildation = "Your Card is currently Deactive, please contact moneyfex suppport team";
                    return View(model);
                }
                if (Card.CardStatus == DB.CardStatus.IsRefunded)
                {

                    model.AmountOnCard = (decimal)0.0;
                    ViewBag.Vaildation = "Your Card Amount has already been refunded ";
                    return View(model);
                }
                model.CardId = Card.Id;
                model.GoodsPurchaseLimit = Card.GoodsLimitType;
                model.GoodsPurchaseLimitAmount = Card.GoodsPurchaseLimit;
                string Currency = Common.Common.GetCountryCurrency(Card.CardUserCountry);
                model.AmountOnCard = Card.CurrentBalance;
            }
            else
            {
                model.AmountOnCard = (decimal)0.0;

            }
            return View(model);
        }
        [HttpPost]
        public ActionResult PayGoodsAndServicesAbroad([Bind(Include = ViewModels.PayGoodsAndServicesAbroadViewModel.BindProperty)]ViewModels.PayGoodsAndServicesAbroadViewModel model)
        {

            if (Common.CardUserSession.LoggedCardUserViewModel == null)
            {

                return RedirectToAction("Login", "CardUserLogin");
            }
            Services.PayGoodsAndServicesAbroadServices services = new Services.PayGoodsAndServicesAbroadServices();
            if (ModelState.IsValid)
            {
                if (model.IsConfirmed == false)
                {
                    ModelState.AddModelError("IsConfirmed", "Please check the box");
                    return View(model);
                }
                if (model.PayingAmount <= 0)
                {
                    ModelState.AddModelError("PayingAmount", "The Paying Amount Should be greater than 0");
                    return View(model);
                }

                if (model.PayingAmount > 0)
                {


                    Decimal TotaAmountWithDrawl = 0;



                    DateTime currentDate = DateTime.Now.Date;



                    //else if (vm.LimitTypeEnum == AutoPaymentFrequency.NoLimitSet)
                    //{

                    //}
                    if (model.GoodsPurchaseLimit == DB.AutoPaymentFrequency.NoLimitSet)
                    {

                    }
                    else
                    {

                        DateTime StartedTransactionDate = new System.DateTime();

                        var Day = 0;
                        var gannuParneDays = 0.0;
                        if (gannuParneDays == 0 && model.GoodsPurchaseLimit != DB.AutoPaymentFrequency.NoLimitSet)
                        {
                            switch (model.GoodsPurchaseLimit)
                            {
                                case DB.AutoPaymentFrequency.Weekly:
                                    Day = (int)currentDate.DayOfWeek;//2
                                                                     //hamile monday lai firstday manchau
                                                                     //day starts =1=monday
                                                                     //enum starts=0=sunday;
                                    gannuParneDays = Day;
                                    if (Day == 0)
                                    {
                                        gannuParneDays = 7;
                                    }


                                    break;

                                case DB.AutoPaymentFrequency.Monthly:
                                    Day = currentDate.Day;
                                    gannuParneDays = Day;

                                    break;

                                case DB.AutoPaymentFrequency.Yearly:

                                    Day = currentDate.DayOfYear;
                                    gannuParneDays = Day;

                                    break;


                                default:
                                    break;
                            }
                        }
                        StartedTransactionDate = currentDate.AddDays(-(gannuParneDays));

                        ///TODO: put this on services
                        //TotaAmountWithDrawl = dbContext.UserCardWithdrawl.Where(x => x.TransactionDate >= StartedTransactionDate).Sum(x => x.TransactionAmount);
                        TotaAmountWithDrawl = services.TotalGoodsPurchaseAmount(StartedTransactionDate, model.CardId);
                        if (TotaAmountWithDrawl + Convert.ToDecimal(model.PayingAmount) > Convert.ToDecimal(model.GoodsPurchaseLimitAmount))
                        {

                            ModelState.AddModelError("PayingAmount", "Sorry, You have exceeded your withdrawl limit");
                            return View(model);
                        }


                    }




                }
                //var result = services.GetMFBCCardInformation(model.MFBCCardNumber.Trim());


                DB.KiiPayPersonalNationalKiiPayBusinessPayment tran = new DB.KiiPayPersonalNationalKiiPayBusinessPayment()
                {

                    KiiPayPersonalWalletInformationId = model.CardId,
                    KiiPayBusinessWalletInformationId = model.ReceiverMFBCCardId,
                    PaymentReference = model.PaymentReference,
                    TransactionDate = DateTime.Now,
                    AmountSent = model.PayingAmount
                };
                var transationResult = services.AddTransaction(tran);
                if (transationResult == true)
                {
                    MailCommon mail = new MailCommon();
                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    try
                    {
                        string CardUserName = Common.CardUserSession.LoggedCardUserViewModel.FirstName + " " + Common.CardUserSession.LoggedCardUserViewModel.LastName;
                        string CardUserEmail = Common.CardUserSession.LoggedCardUserViewModel.Email;
                        string BusinessMerchantName = services.GetBusinessInformation(model.ReceiverMFBCCardId).BusinessName;
                        string BusinessMerchantCity = services.GetBusinessInformation(model.ReceiverMFBCCardId).BusinessOperationCity;
                        string body = "";
                        body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfCardUserPayingForGoodsAndServices?CardUserName=" + CardUserName + "&BusinessMerchantName=" + BusinessMerchantName);

                        mail.SendMail(CardUserEmail, "Confirmation of Payment ", body);
                        string body1 = "";

                        var FaxerDetails = services.GetFaxerDetails(Common.FaxerSession.LoggedUser.Id);
                        string BalanceOnCard = services.GetAmountOnCard();
                        var CardInformation = services.GetMFTCCardInformation();
                        string TopUpMoneyfaxCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=" + model.CardId;
                        string StopAlert = "";
                        string CardUserCountry = Common.Common.GetCountryName(CardInformation.CardUserCountry);
                        body1 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardUsageEmail?FaxerName=" + FaxerDetails.FirstName
                            + " " + FaxerDetails.MiddleName + " " + FaxerDetails.LastName +
                            "&MFTCCardNumber=" + CardInformation.MobileNo.Decrypt() + "&CardUserName=" + CardUserName + "&CardUserCountry="
                            + CardUserCountry + "&CardUserCity=" + CardInformation.CardUserCity
                            + "&CityOfPayingAgentOrRegisteredMerchant=" + BusinessMerchantCity + "&BalanceOnCard=" + BalanceOnCard
                            + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&StopAlert=" + StopAlert);
                        mail.SendMail(FaxerDetails.Email, "MoneyFax Top-Up Card Usage - Alert", body1);
                        //mail.SendMail("anankarki97@gmail.com", "MoneyFax Top-Up Card Usage - Alert", body1);


                        string body2 = "";
                        string ReceiverEmail = services.GetBusinessInformation(tran.KiiPayBusinessWalletInformationId).Email;
                        //Send Email for Confirmation of Payment to a Merchant 
                        body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMerchantPaymentMerchant?BusinessMerchantName="
                            + BusinessMerchantName + "&FaxerName=" + model.ReceiverBusinessCardUserName);
                        mail.SendMail(ReceiverEmail, "Confirmation of Payment to a Merchant", body2);


                    }
                    catch (Exception)
                    {

                        throw;
                    }
                    return RedirectToAction("PaymentSucessConfirmation");
                }
            }

            return View(model);

        }

        public ActionResult PaymentSucessConfirmation()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel == null)
            {

                return RedirectToAction("Login", "CardUserLogin");
            }
            return View();
        }
        public ActionResult GetCardDetails(string CardNumber)
        {

            Services.PayGoodsAndServicesAbroadServices services = new Services.PayGoodsAndServicesAbroadServices();

            string[] tokens = CardNumber.Split('-');
            var result = new DB.KiiPayBusinessWalletInformation();
            if (tokens.Length < 2)
            {
                result = services.GetMFBCCardInformationByCardNumber(CardNumber.Trim());

            }
            else
            {
                result = services.GetMFBCCardInformation(CardNumber.Trim());
            }

            if ((result != null) && result.Country != Common.CardUserSession.LoggedCardUserViewModel.Country) {

                return Json(new { InvalidCountry  = true}, JsonRequestBehavior.AllowGet);

            }


            if ((result != null) && result.CardStatus == DB.CardStatus.Active)
            {

                return Json(new
                {
                    InvalidCountry = false,
                    InvalidCard = false,
                    ReceiverMFBCCardId = result.Id,
                    ReceiverBusinessCardUserName = result.KiiPayBusinessInformation.BusinessName,
                    ReceiverCardUserAccountNo = result.KiiPayBusinessInformation.BusinessMobileNo
                }, JsonRequestBehavior.AllowGet);

            }


            else
            {
                return Json(new
                {
                    InvalidCard = true
                }, JsonRequestBehavior.AllowGet);
            }


        }
    }
}