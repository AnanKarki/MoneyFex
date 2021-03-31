using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class PayForGoodsAndServicesController : Controller
    {
        CommonServices CommonService = new CommonServices(); 
        // GET: Businesses/PayForGoodsAndServices
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult PayForGoodsAndServices()
        {

            if (Common.BusinessSession.LoggedBusinessMerchant == null)
            {

                return RedirectToAction("Login", "BusinessLogin");
            }
            Services.PayForGoodsServices services = new Services.PayForGoodsServices();
            var model = new ViewModels.PayForGoodAndServicesViewModel();
            var CardExist = services.MFBCCardInformationByID();
            if (CardExist != null)
            {
                if (CardExist.CardStatus == DB.CardStatus.IsDeleted)
                {

                    model.AmountOnCard = (decimal)0.0;
                    ViewBag.Vaildation = "Your Card has been Deleted, please contact money fax suppport team";
                    return View(model);
                }
                if (CardExist.CardStatus == DB.CardStatus.InActive)
                {

                    model.AmountOnCard = (decimal)0.0;
                    ViewBag.Vaildation = "Your Card is currently Deactive, please contact money fax suppport team";
                    return View(model);
                }
                if (CardExist.CardStatus == DB.CardStatus.IsRefunded)
                {

                    model.AmountOnCard = (decimal)0.0;
                    ViewBag.Vaildation = "Your Card Amount has already been refunded ";
                    return View(model);
                }
                model.CardId = CardExist.Id;
                model.AmountOnCard = services.AmountOnCard();
                ViewBag.Currency = Common.Common.GetCountryCurrency(CardExist.Country);
                model.Currency = Common.Common.GetCountryCurrency(CardExist.Country);
                model.CurrencySymbol = Common.Common.GetCurrencySymbol(CardExist.Country);

            }
            else
            {
                model.AmountOnCard = (decimal)0.0;
                ViewBag.Vaildation = "Please Register MFBC Card first";
            }
            
            
            return View(model);
        }
        [HttpPost]
        public ActionResult PayForGoodsAndServices([Bind(Include = ViewModels.PayForGoodAndServicesViewModel.BindProperty)]ViewModels.PayForGoodAndServicesViewModel model)
        {

            if (Common.BusinessSession.LoggedBusinessMerchant == null)
            {

                return RedirectToAction("Login", "BusinessLogin");
            }
            Services.PayForGoodsServices services = new Services.PayForGoodsServices();
            var CardExist = services.MFBCCardInformationByID();
            if (CardExist == null)
            {
                model.AmountOnCard = (decimal)0.0;
                ViewBag.Vaildation = "Please Register MFBC Card first";
            }

            ViewBag.Currency = Common.Common.GetCountryCurrency(CardExist.Country);
            if (ModelState.IsValid)
            {
                if (model.IsConfirmed == false)
                {
                    ModelState.AddModelError("IsConfirmed", "Please check the box");
                    return View(model);
                }
                if (model.PayingAmount <= 0)
                {
                    ModelState.AddModelError("PayingAmount", "The Paying Amount should be greater than 0");
                    return View(model);
                }
                //var result = services.GetMFBCCardInformation(model.MFBCCardNumber.Trim());
                var result = services.GetMFBCCardByCardId(model.ReceiverMFBCCardId);
                if (result != null)
                {

                    DB.KiiPayBusinessLocalTransaction transaction = new DB.KiiPayBusinessLocalTransaction()
                    {

                        PayedFromKiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId,
                        PayedFromKiiPayBusinessWalletInformationId = CardExist.Id,
                        AmountSent = model.PayingAmount,
                        PayedToKiiPayBusinessWalletInformationId = result.Id,
                        PayedToKiiPayBusinessInformationId = result.KiiPayBusinessInformationId,
                        TransactionDate = DateTime.Now,
                        PaymentReference = model.PaymentReference
                    };
                    var transationResult = services.MFBCCardTransaction(transaction);
                    if (transationResult == true)
                    {
                        var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                        //MoneyFax Business Card Usage -Alert
                        string BusinessUser = Common.BusinessSession.LoggedBusinessMerchant.BusinessName;
                        string BusinessEmail = Common.BusinessSession.LoggedBusinessMerchant.BusinessEmailAddress;
                        string BalanceOnCard = services.AmountOnCard().ToString();
                        MailCommon mail = new MailCommon();
                        string body = "";
                        string CardUserCountry = Common.Common.GetCountryName(CardExist.Country);

                        body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFBCCardUsageEmail?NameOfBusinessUser=" + BusinessUser +
                            "&MFBCCardNumber=" + CardExist.MobileNo.Decrypt() + "&CarduserName=" + CardExist.FirstName + " " + CardExist.LastName +
                            "&CardUserCountry=" + CardUserCountry + "&CardUserCity=" + CardExist.City +
                            "&CityofPayingAgentOrRegisteredMerchant=" + CardExist.City + "&BalanceOnCard=" + BalanceOnCard);
                        //mail.SendMail("anankarki97@gmail.com", "MoneyFax Business Card Usage -Alert", body);
                        mail.SendMail(BusinessEmail, "MoneyFax Business Card Usage -Alert", body);
                        string PayedToMerchantName = services.GetBusinessMerchantName(result.KiiPayBusinessInformationId);
                        string body1 = "";
                        body1 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfBusinessPayingForGoodsAndServices?BusinessUserName=" + BusinessUser + "&PayedToBusinessMerchantName=" + PayedToMerchantName);
                        mail.SendMail(BusinessEmail, "Confirmation of Merchant Payment", body1);

                        string body2 = "";
                        string ReceiverEmail = services.GetBusinessInformation(result.KiiPayBusinessInformationId).Email;
                        //Send Email for Confirmation of Payment to a Merchant 
                        body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMerchantPaymentMerchant?BusinessMerchantName="
                            + BusinessUser + "&FaxerName=" + model.ReceiverBusinessCardUserName);
                        mail.SendMail(ReceiverEmail, "Confirmation of Payment to a Merchant", body2);

                        return RedirectToAction("PayaMerchantConfirmation");
                    }
                }


            }
            return View(model);
        }

        public ActionResult PayaMerchantConfirmation()
        {
            if (Common.BusinessSession.LoggedBusinessMerchant == null)
            {

                return RedirectToAction("Login", "BusinessLogin");
            }
            Services.PayForGoodsServices services = new Services.PayForGoodsServices();
            ViewBag.AmountCredited = services.AmountOnCard();
            return View();
        }
        public ActionResult GetCardDetails(string CardNumber)
        {

            Services.PayForGoodsServices services = new Services.PayForGoodsServices();

            string[] tokens = CardNumber.Split('-');
            var result = new DB.KiiPayBusinessWalletInformation();
            if (tokens.Length < 2)
            {
                result = services.GetCardInformationByCardNumber(CardNumber.Trim());

            }
            else
            {
                result = services.GetMFBCCardInformation(CardNumber.Trim());
            }

           

            if ((result != null) && result.CardStatus == DB.CardStatus.Active)
            {
                if (result.KiiPayBusinessInformation.Id == Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId)
                {

                    return Json(new
                    {
                        InvalidCard = true
                    }, JsonRequestBehavior.AllowGet);
                }
                else if (result.Country != Common.BusinessSession.LoggedBusinessMerchant.CountryCode)
                {


                    return Json(new
                    {
                        InvalidCountry = true
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new
                    {
                        InvalidCard = false,
                        InvalidCountry = false,
                        ReceiverMFBCCardId = result.Id,
                        ReceiverBusinessCardUserName = result.FirstName + " " + result.LastName,
                        ReceiverCardUserAccountNo = result.KiiPayBusinessInformation.BusinessMobileNo,
                        Currency = CommonService.getCurrencyCodeFromCountry(result.Country),
                        CurrencySymbol = CommonService.getCurrencySymbol(result.Country),
                    }, JsonRequestBehavior.AllowGet);
                }
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