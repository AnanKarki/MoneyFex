using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.Controllers
{
    public class SavedCreditDebitCardController : Controller
    {
        SavedCreditDebitCardServices _service = null;
        public SavedCreditDebitCardController()
        {
            _service = new SavedCreditDebitCardServices();
        }
        // GET: KiiPayPersonal/SavedCreditDebitCard
        public ActionResult Index()
        {
            var vm = _service.getSavedCardsList();
            return View(vm);
        }

        [HttpGet]
        public ActionResult AddNewCard()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddNewCard([Bind(Include = AddNewCardViewModel.BindProperty)]AddNewCardViewModel model)
        {
            if (ModelState.IsValid)
            {
                StripeResultIsValidCardVm validateCardData = new StripeResultIsValidCardVm()
                {
                    //Number = model.CardType,
                    Number = model.CardNumber.ToString(),
                    ExpirationMonth = model.ExpMonth.ToString(),
                    ExpiringYear = model.ExpYear.ToString(),
                    SecurityCode = model.SecurityCode.ToString(),
                    billingpostcode = Common.FaxerSession.LoggedUser.PostCode,
                    billingpremise = Common.FaxerSession.LoggedUser.HouseNo
                };
                var stripeResult = StripServices.IsValidCardNo(validateCardData);
                if (stripeResult.IsValid == true)
                {
                    bool addCard = _service.addNewCard(model);
                    if (addCard == true)
                    {
                        return RedirectToAction("Index");
                    }
                }
                else {
                    ModelState.AddModelError("CardNumber", "Invalid Card !");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult EditCard(int id)
        {
            if(id != 0)
            {
                var vm = _service.getCardInfo(id);
                return View(vm);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult EditCard([Bind(Include = AddNewCardViewModel.BindProperty)]AddNewCardViewModel model)
        {
            if (ModelState.IsValid)
            {
                StripeResultIsValidCardVm validateCardData = new StripeResultIsValidCardVm()
                {
                    //Number = model.CardType,
                    Number = model.CardNumber.ToString(),
                    ExpirationMonth = model.ExpMonth.ToString(),
                    ExpiringYear = model.ExpYear.ToString(),
                    SecurityCode = model.SecurityCode.ToString(),
                };
                var stripeResult = StripServices.IsValidCardNo(validateCardData);
                if (stripeResult.IsValid == true)
                {
                    bool updateCard = _service.updateCard(model);
                    if (updateCard == true)
                    {
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    ModelState.AddModelError("CardNumber", "Invalid Card !");
                }
            }
            return View(model);

        }
        public ActionResult DeleteCard(int id)
        {
            if(id != 0)
            {
                _service.deleteCard(id);
            }
            return RedirectToAction("Index");
        }

        public ActionResult ConfirmAddCard()
        {
            return View();
        }
    }
}