using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class BecomeAMerchantController : Controller
    {
        // GET: Businesses/BecomeAMerchant
        [HttpGet]
        public ActionResult Index()
        {
            Services.CommonServices commonServices = new Services.CommonServices();

            var countries = commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            return View();
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = ViewModels.BecomeAMerchant.BindProperty)]ViewModels.BecomeAMerchant model)
        {
            Services.CommonServices commonServices = new Services.CommonServices();

            var countries = commonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            if (ModelState.IsValid)
            {
                Services.BecomeAMerchantServices merchantServices = new Services.BecomeAMerchantServices();
                var invalidEmail = merchantServices.EmailExist(model.BusinessEmailAddress);
                if (invalidEmail == true)
                {
                    ModelState.AddModelError("BusinessEmailAddress", "Email Already exist");
                    return View(model);
                }
                Services.BusinessSignUpServices businessSignUpServices = new Services.BusinessSignUpServices();
                var RegistrationNumber = businessSignUpServices.GetResgistrationNumber(7);
                var MFSCode = businessSignUpServices.GetMFSCode(10);
                string activationCode = Guid.NewGuid().ToString();
                DB.BecomeAMerchant merchant = new DB.BecomeAMerchant()
                {
                    RegistrationNumber = RegistrationNumber,
                    CompanyBusinessName = model.CompanyBusinessName,
                    Address1 = model.Address1,
                    Address2 = model.Address2,
                    BusinessLicenseRegistrationNumber = model.BusinessLicenseRegistrationNumber,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    City = model.City,
                    CountryCode = model.CountryCode,
                    ContactPhone = model.ContactPhone,
                    BusinessEmailAddress = model.BusinessEmailAddress,
                    PostZipCode = model.PostZipCode,
                    StateProvince = model.StateProvince,
                    FaxNo = model.FaxNo,
                    Website = model.Website,
                    Street = model.Street,
                    ActivationCode = activationCode,
                };

                var result = merchantServices.AddBusinessMerchant(merchant);
                if (result != null)
                {


                    string msg = "";
                    MailCommon mail = new MailCommon();
                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    string CountryCode = Common.Common.GetCountryPhoneCode(merchant.CountryCode);
                    //var link = string.Format("{0}/Businesses/BecomeAMerchant/ValidateLink?activationCode={1}", baseUrl, activationCode);
                    string body = "";
                    try
                    {

                        body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BusinessMerchantActivationEmailToAdmin?MerchantName=" + merchant.CompanyBusinessName
                            + "&Telephone=" + merchant.ContactPhone + "&Email=" + merchant.BusinessEmailAddress
                            + "&RegistrationCode=" + merchant.RegistrationNumber);
                        mail.SendMail("merchantregistration@moneyfex.com", "Alert: New Merchant Registration - " + merchant.CompanyBusinessName, body);


                        string body2 = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BusinessRegistrationInitialEmail?ContactPerson=" + merchant.FirstName + " " + merchant.LastName
                            + "&RegistrationCode=" + merchant.RegistrationNumber);

                        mail.SendMail(merchant.BusinessEmailAddress, "Initial Merchant Registration Code", body2);
                        msg = "Registraion Completed";
                    }
                    catch (Exception)
                    {

                        msg = "Problem finding mail";
                    }

                    return RedirectToAction("MerChantConfirmation");
                }
            }
            return View();
        }
        public ActionResult MerChantConfirmation()
        {

            return View();
        }
        [HttpGet]
        public ActionResult ValidateLink(string activationCode)
        {
            Services.BecomeAMerchantServices merchantServices = new Services.BecomeAMerchantServices();
            var merchantInfo = merchantServices.GetBecomeAMerchantDetails(activationCode);
            MailCommon mail = new MailCommon();
            try
            {
                mail.SendMail(merchantInfo.BusinessEmailAddress, "About Business Registration Code.", "Your Registration Code is " + merchantInfo.RegistrationNumber);
            }
            catch (Exception)
            {
                throw;
            }
            ViewBag.ActivationCode = activationCode;
            return PartialView();

        }
        public ActionResult ValidateLink(string RegistrationCode, string ActivationCode)
        {
            Services.BecomeAMerchantServices merchantServices = new Services.BecomeAMerchantServices();
            var merchantInfo = merchantServices.GetBecomeAMerchantDetails(ActivationCode);
            if (merchantInfo.RegistrationNumber == RegistrationCode)
            {
                return RedirectToAction("BusinessDetails", "BusinessSignUp", new { area = "Businesses", RegistrationNumber = RegistrationCode });
            }
            ModelState.AddModelError("RegistrationCode", "Please enter a valid registration code");
            ViewBag.ActivationCode = ActivationCode;
            return View();
        }
    }
}