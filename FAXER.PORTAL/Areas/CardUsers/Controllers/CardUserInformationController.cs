using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.CardUsers.Controllers
{
    public class CardUserInformationController : Controller
    {
        // GET: CardUsers/CardUserInformation

        Services.CardUserProfileServices services = new Services.CardUserProfileServices();
        string FaxerEmail = null;
        public CardUserInformationController()
        {

            if (Common.CardUserSession.LoggedCardUserViewModel != null)
            {

                FaxerEmail = services.GetFaxerEmailAddress(Common.CardUserSession.LoggedCardUserViewModel.id);

            }
        }
        [HttpGet]
        public ActionResult Index()
        {
            if (Common.CardUserSession.LoggedCardUserViewModel == null)
            {

                return RedirectToAction("Login", "CardUserLogin");
            }
            ViewBag.Countries = new SelectList(services.getCountries(), "CountryCode", "CountryName");
            var model = services.GETCardUserInformation();
            return View(model);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = ViewModels.CardUserInformationViewModel.BindProperty)]ViewModels.CardUserInformationViewModel vm)
        {

            ViewBag.Countries = new SelectList(services.getCountries(), "CountryCode", "CountryName");
            bool ISValid = true;
            if (string.IsNullOrEmpty(vm.Address1))
            {

                ModelState.AddModelError("Address1", "Please enter your address");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(vm.City))
            {

                ModelState.AddModelError("City", "Please enter a City");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(vm.State))
            {

                ModelState.AddModelError("State", "Please enter your state");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(vm.ZipCode))
            {

                ModelState.AddModelError("ZipCode", "Please enter your postal code");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(vm.Country))
            {

                ModelState.AddModelError("Country", "Please select your country");
                ISValid = false;
            }
            if (ISValid)
            {

                var data = services.GetCardInformation();
                data.Address1 = vm.Address1;
                data.Address2 = vm.Address2;
                data.CardUserCity = vm.City;
                data.CardUserCountry = vm.Country;
                data.CardUserState = vm.State;
                data.CardUserPostalCode = vm.ZipCode;


                services.UpdateAddress(data);
                
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                try
                {
                    string email = Common.CardUserSession.LoggedCardUserViewModel.Email;


                    // Need Changes Here  Because the sender info is conditional now 
                    //the Kiipay Personal might or might not be associated with the faxer
                    string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/CardUserEmailAddressUpdateEmail?SenderName="
                        + data.FirstName + " " + data.MiddleName + " " + data.LastName
                        + "&CardUserName=" + data.FirstName + " " + data.MiddleName + "" + data.LastName
                        + "&UpdatedProperty=" + "address" + "&CardNumber=" + data.MobileNo.Decrypt());

                    //string msg = "Your Address has been Changed to " + Address + " at " + DateTime.Now;
                    mail.SendMail(email, "Address update", body);

                    mail.SendMail(FaxerEmail, "Address update", body);




                }
                catch (Exception ex)
                {

                    Log.Write("Card User Portal Update Address" +  ex.Message);
                }
            }

            TempData["InvalidInformation"] = true;
            return View(vm);
        }
        public ActionResult UpdateTelephone(string PhoneNumber, int id)
        {
            if (Common.CardUserSession.LoggedCardUserViewModel == null)
            {

                return RedirectToAction("Login", "CardUserLogin");
            }
            if (string.IsNullOrEmpty(PhoneNumber))
            {

                TempData["PhoneNumberInvalid"] = "Empty Phone Number Cannot be saved";
            }
            else
            {
                Services.CardUserProfileServices services = new Services.CardUserProfileServices();
                var result = services.UpdateTelephone(PhoneNumber, id);
                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                try
                {
                    string email = Common.CardUserSession.LoggedCardUserViewModel.Email;
                    var CardUserDetails = services.GetCardInformation();

                    string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/CardUserEmailAddressUpdateEmail?SenderName="
                        + "" + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName
                        + "&CardUserName=" + CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + "" + CardUserDetails.LastName
                        + "&UpdatedProperty=" + "Telephone number" + "&CardNumber=" + CardUserDetails.MobileNo.Decrypt());

                    mail.SendMail(email, "Telephone number Update", body);

                    mail.SendMail(FaxerEmail, "Telephone number update", body);

                }
                catch (Exception)
                {
                    throw;
                }
            }
            return RedirectToAction("Index", "CardUserInformation", new { area = "CardUsers" });

        }
        public ActionResult UpdateEmail(string Email, int id)
        {
            if (Common.CardUserSession.LoggedCardUserViewModel == null)
            {

                return RedirectToAction("Login", "CardUserLogin");
            }
            if (string.IsNullOrEmpty(Email))
            {

                TempData["EmailAddressInvalid"] = "Empty Email cannot be saved";
            }
            else
            {
                Services.CardUserSignUpServices signUpServices = new Services.CardUserSignUpServices();
                bool emailExist = signUpServices.CardUserEmailExist(Email);
                if (emailExist == true)
                {
                    TempData["EmailAddressInvalid"] = "Email Already Exist";
                }
                else
                {
                    Services.CardUserProfileServices services = new Services.CardUserProfileServices();
                    var result = services.UpdateEmail(Email, id);
                    MailCommon mail = new MailCommon();
                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    try
                    {
                        //EmailTemplate/FaxerEmailUpdateEmail

                        var CardUserDetials = signUpServices.GetCardUserDetials(Email);
                        var CardUserDetails = services.GetCardInformation();

                        string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/CardUserEmailAddressUpdateEmail?SenderName="
                            + "" + " " + CardUserDetails.MiddleName + " " + CardUserDetails.LastName
                            + "&CardUserName=" + CardUserDetails.FirstName + " " + CardUserDetails.MiddleName + "" + CardUserDetails.LastName
                            + "&UpdatedProperty=" + "telephone number" + "&CardNumber=" + CardUserDetails.MobileNo.Decrypt());

                        mail.SendMail(Email, "Telephone number update", body);

                        mail.SendMail(FaxerEmail, "Telephone number update", body);

                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Index", "CardUserInformation", new { area = "CardUsers" });

        }
    }
}