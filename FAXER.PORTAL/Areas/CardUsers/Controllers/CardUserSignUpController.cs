using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class CardUserSignUpController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        // GET: CardUsers/CardUserSignUp
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult SignUp()
        {
            var idCardTypes = dbContext.IdentityCardType.ToList();
            ViewBag.IdCardType = new SelectList(dbContext.IdentityCardType.OrderBy(x => x.CardType), "CardType", "CardType");
            ViewBag.IssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            return View();
        }
        [HttpPost]
        public ActionResult SignUp([Bind(Include = ViewModels.MFCTCardSignUpViewModel.BindProperty)]ViewModels.MFCTCardSignUpViewModel model)
        {
            var idCardTypes = dbContext.IdentityCardType.ToList();
            ViewBag.IdCardType = new SelectList(dbContext.IdentityCardType.OrderBy(x => x.CardType), "CardType", "CardType");
            ViewBag.IssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            if (ModelState.IsValid)
            {

                if (model.Confirm == false) {
                    ModelState.AddModelError("Confirm", "Please check the terms and condition to sign up");
                    return View(model);
                }
                if (model.DateOfBirth == default(DateTime))
                {

                    ModelState.AddModelError("DateOfBirth", "Please enter a valid Date");
                    return View(model);
                }
                Services.CardUserSignUpServices signUpServices = new Services.CardUserSignUpServices();

                bool MFTCCardNumberExist = signUpServices.CardUserExist(model.MFTCCardNumber);
                if (MFTCCardNumberExist == true)
                {
                    ModelState.AddModelError("MFTCCardNumber", "Virtual Account has already been registered");
                    return View(model);
                }
                bool CardUserEmallExist = signUpServices.CardUserEmailExist(model.EmailAddress);
                if (CardUserEmallExist == true)
                {

                    ModelState.AddModelError("EmailAddress", "Email Already Exist");
                    return View(model);
                }
                var MFTCCardInformation = signUpServices.GetMFTCCardInformation(model.MFTCCardNumber);

                if (MFTCCardInformation == null)
                {
                    ModelState.AddModelError("MFTCCardNumber", "Please Enter a vaild virtual account no");
                    return View(model);
                }
                else
                {

                    if (model.Surname.ToLower() != MFTCCardInformation.LastName.ToLower())
                    {

                        ModelState.AddModelError("Surname", "Your Surname didn't match");
                        return View(model);
                    }
                    if (model.DateOfBirth != MFTCCardInformation.CardUserDOB)
                    {
                        ModelState.AddModelError("DateOfBirth", "Your date of birth does not match");
                        return View(model);
                    }
                    if (model.EmailAddress != MFTCCardInformation.CardUserEmail)
                    {

                        ModelState.AddModelError("EmailAddress", "Your email address didn't match");
                        return View(model);
                    }
                    bool Validpassword = Common.Common.ValidatePassword(model.Password);
                    if (Validpassword == false)
                    {
                        ModelState.AddModelError("Password", "Password Should Contain Atleast 1 UpperCase, 1 number and 1 Special Character");
                        return View(model);
                    }
                    if (model.Password != model.ConfirmPassword)
                    {

                        ModelState.AddModelError("ConfirmPassword", "Your Password didn't match");
                        return View(model);
                    }

                }

                DB.KiiPayPersonalUserInformation cardUserInformation = new DB.KiiPayPersonalUserInformation()
                {

                    Lastname = model.Surname,
                    MobileNo = model.MFTCCardNumber.Encrypt(),
                    EmailAddress = model.EmailAddress,
                    KiiPayPersonalWalletInformationId = MFTCCardInformation.Id,
                    IdCardNumber  = model.IdCardNumber,
                    IdCardExpiringDate = model.IdCardExpiringDate,
                    IdCardType =model.IdCardType,
                    IssuingCountry =model.IssuingCountry
                };

                var result = signUpServices.AddcardUserInformation(cardUserInformation);
                if (result != null)
                {
                    string ActivationCode = Guid.NewGuid().ToString();
                    DB.KiiPayPersonalUserLogin cardUserLogin = new DB.KiiPayPersonalUserLogin()
                    {

                        Email = model.EmailAddress,
                        Password = model.Password.Encrypt(),
                        ActivationCode = ActivationCode,
                        LoginFailedCount = 0,
                        IsActive = true,
                        KiiPayPersonalUserInformationId = result.Id
                    };

                    var result2 = signUpServices.AddCarduserLogin(cardUserLogin);
                    if (result2 != null)
                    {


                        return RedirectToAction("Login", "CardUserLogin");
                    }
                }

            }

            return View(model);
        }
        public ActionResult GetRegisteredCard(string MFTCNumber)
        {
            Services.CardUserSignUpServices services = new Services.CardUserSignUpServices();

            var result = services.CardAlreadyExist(MFTCNumber);
            if (result == true)
            {

                return Json(new
                {
                    Invalidcard = true
                }, JsonRequestBehavior.AllowGet);
            }
            else {
                return Json(new { Invalidcard = false }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}