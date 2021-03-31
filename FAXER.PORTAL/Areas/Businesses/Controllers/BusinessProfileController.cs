using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class BusinessProfileController : Controller
    {
        DB.FAXEREntities context = new DB.FAXEREntities();
        // GET: Businesses/BusinessProfile
        public ActionResult Index()
        {
            return RedirectToAction("BusinessProfile");
        }
        [HttpGet]
        public ActionResult BusinessProfile()
        {
            if (Common.BusinessSession.LoggedBusinessMerchant == null)
            {

                return RedirectToAction("Login", "BusinessLogin");
            }

            Services.MyBusinessProfileServices services = new Services.MyBusinessProfileServices();
            ViewBag.Countries = new SelectList(context.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            var model = services.GetBusinessInformation();
            return View(model);
        }

        [HttpPost]
        public ActionResult BusinessProfile([Bind(Include = ViewModels.MyBusinessProfileViewModel.BindProperty)] ViewModels.MyBusinessProfileViewModel model)
        {
            ViewBag.Countries = new SelectList(context.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            bool ISValid = true;
            if (string.IsNullOrEmpty(model.Address1))
            {

                ModelState.AddModelError("Address1", "Please enter your address");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(model.City))
            {

                ModelState.AddModelError("City", "Please enter a City");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(model.State))
            {

                ModelState.AddModelError("State", "Please enter your state");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(model.PostalCode))
            {

                ModelState.AddModelError("PostalCode", "Please enter your postal code");
                ISValid = false;
            }
            if (string.IsNullOrEmpty(model.Country))
            {

                ModelState.AddModelError("Country", "Please select your country");
                ISValid = false;
            }

            if (ISValid)
            {
                var data = context.KiiPayBusinessInformation.Find(model.Id);
                data.BusinessOperationAddress1 = model.Address1;
                data.BusinessOperationAddress2 = model.Address2;
                data.BusinessOperationCity = model.City;
                data.BusinessOperationState = model.State;
                //data.CountryCode = model.Country;
                data.BusinessOperationPostalCode = model.PostalCode;

                context.Entry(data).State = EntityState.Modified;
                context.SaveChanges();

                return RedirectToAction("Index");
            }
            else
            {
                TempData["InvalidInformation"] = true;
                return View(model);
            }
        }
        public ActionResult UpdateBusinessAddress(string Address, int id)
        {

            Services.MyBusinessProfileServices services = new Services.MyBusinessProfileServices();
            var result = services.UpdateBusinessAddress(Address, id);
            string BusinessEmail = Common.BusinessSession.LoggedBusinessMerchant.BusinessEmailAddress;
            MailCommon mail = new MailCommon();
            try
            {
                string msg = "Your Business Adress has been changed to " + Address + " at " + DateTime.Now;
                mail.SendMail(BusinessEmail, "Your Business Information has been Changed", msg);
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("BusinessProfile", "BusinessProfile", new { area = "Businesses" });
        }
        public ActionResult UpdatePhone(string Phone, int id)
        {
            Services.MyBusinessProfileServices services = new Services.MyBusinessProfileServices();
            var result = services.UpdatePhone(Phone, id);
            string BusinessEmail = Common.BusinessSession.LoggedBusinessMerchant.BusinessEmailAddress;
            MailCommon mail = new MailCommon();
            try
            {

                string msg = "Your phone number has been changed to" + Phone + " at " + DateTime.Now;
                mail.SendMail(BusinessEmail, "YOur Business Information  has been Changed ", msg);
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("BusinessProfile", "BusinessProfile", new { area = "Businesses" });
        }

        public ActionResult UpdateFaxNo(string FaxNo, int id)
        {
            Services.MyBusinessProfileServices services = new Services.MyBusinessProfileServices();
            var result = services.UpdateFaxNo(FaxNo, id);
            string BusinessEmail = Common.BusinessSession.LoggedBusinessMerchant.BusinessEmailAddress;
            MailCommon mail = new MailCommon();
            try
            {
                string msg = "Your Business Fax No has been changed to " + FaxNo + " at " + DateTime.Now;
                mail.SendMail(BusinessEmail, "YOur Business Information has been Changed", msg);
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("BusinessProfile", "BusinessProfile", new { area = "Businesses" });
        }
        public ActionResult UpdateEmailAddress(string Email, int id)
        {

            Services.MyBusinessProfileServices services = new Services.MyBusinessProfileServices();
            bool isvalidEmail = services.IsValidEmail(Email);
            string BusinessEmail = Common.BusinessSession.LoggedBusinessMerchant.BusinessEmailAddress;
            if (isvalidEmail == true)
            {
                var result = services.UpdateEmailAddress(Email, id);
                if (result == false)
                {
                    TempData["InValid"] = "Email Address already exist";
                }
                MailCommon mail = new MailCommon();
                try
                {
                    string msg = "Your Business Email Address has benn changed to" + Email + " at " + DateTime.Now;
                    mail.SendMail(Email, "YOur Business information has been changed", msg);
                }
                catch (Exception)
                {

                    throw;
                }

            }
            else
            {
                TempData["InValid"] = "Please Enter Valid Email Address";
            }
            return RedirectToAction("BusinessProfile", "BusinessProfile", new { area = "Businesses" });
        }
        public ActionResult UpdateWebsite(string website, int id)
        {
            Services.MyBusinessProfileServices services = new Services.MyBusinessProfileServices();
            var result = services.UpdateWebsite(website, id);
            string BusinessEmail = Common.BusinessSession.LoggedBusinessMerchant.BusinessEmailAddress;
            MailCommon mail = new MailCommon();
            try
            {
                string msg = "Your Website has been changed to " + website + " at " + DateTime.Now;
                mail.SendMail(BusinessEmail, "Your Business Information has been changed ", msg);
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("BusinessProfile", "BusinessProfile", new { area = "Businesses" });
        }
        public ActionResult UpdateContactPerson(string ContactPerson, int id)
        {
            Services.MyBusinessProfileServices services = new Services.MyBusinessProfileServices();
            var result = services.UpdateContactPerson(ContactPerson, id);
            string BusinessEmail = Common.BusinessSession.LoggedBusinessMerchant.BusinessEmailAddress;
            MailCommon mail = new MailCommon();
            try
            {
                string msg = "Your contact person name has been changed to " + ContactPerson + " at " + DateTime.Now;
                mail.SendMail(BusinessEmail, "Your Business Information has been changed", msg);
            }
            catch (Exception)
            {

                throw;
            }
            return RedirectToAction("BusinessProfile", "BusinessProfile", new { area = "Businesses" });
        }


    }
}