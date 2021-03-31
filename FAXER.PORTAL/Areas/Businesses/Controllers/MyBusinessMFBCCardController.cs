using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses.Controllers
{
    public class MyBusinessMFBCCardController : Controller
    {
        // GET: Businesses/MyBusinessMFBCCard
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult MyBusinessMFBCCardDetails() {
            if (Common.BusinessSession.LoggedBusinessMerchant == null)
            {

                return RedirectToAction("Login", "BusinessLogin");
            }
            Services.MFBCCardProfileServices businessProfileServices = new Services.MFBCCardProfileServices();
            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;

            var model = businessProfileServices.GetMFBCCardInformation(KiiPayBusinessInformationId);
            if (model == null) {
                var vm = new ViewModels.MFBCCardProfileViewModel();
                return View(vm);
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult UpdateMyBusinessCardDetails( int cardId=0) {
            if (Common.BusinessSession.LoggedBusinessMerchant == null)
            {

                return RedirectToAction("Login", "BusinessLogin");
            }
            Services.MFBCCardProfileServices businessProfileServices = new Services.MFBCCardProfileServices();
            var model = new ViewModels.MFBCCardProfileViewModel();
            if (cardId > 0)
            {
                model = businessProfileServices.GetMFBCCardProfileByCardId(cardId);
            }
            else {
                int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
                model = businessProfileServices.GetMFBCCardInformation(KiiPayBusinessInformationId);
            }
                return View(model);
        }
        public ActionResult UpdateCity(string City , int CardId)
        {
            Services.MFBCCardProfileServices services = new Services.MFBCCardProfileServices();
            var result = services.UpdateCardUserCity(City, CardId);
            if (result == true) {
                string BusinessEmail = Common.BusinessSession.LoggedBusinessMerchant.BusinessEmailAddress;
                MailCommon mail = new MailCommon();
                try
                {

                    string msg = "Your City has been changed to " + City + "at" + DateTime.Now;
                    mail.SendMail(BusinessEmail, "Your Business Card Information has been Changed ", msg);
                }
                catch (Exception)
                {

                    throw;
                }
                return RedirectToAction("UpdateMyBusinessCardDetails", "MyBusinessMFBCCard", new { area = "Businesses" });

            }

            return RedirectToAction("UpdateMyBusinessCardDetails", "MyBusinessMFBCCard", new { area = "Businesses" });

        }
        public ActionResult UpdateEmailAddress(string Email, int CardId)
        {

            Services.MFBCCardProfileServices services = new Services.MFBCCardProfileServices();
            var result = services.UpdateCardUserEmail(Email, CardId);
            if (result == true)
            {
                string BusinessEmail = Common.BusinessSession.LoggedBusinessMerchant.BusinessEmailAddress;
                MailCommon mail = new MailCommon();
                try
                {
                    string msg= "Your email address has been changed to" + Email + "at" + DateTime.Now;
                    mail.SendMail(BusinessEmail, "Your Business Card Information has been Changed ",msg );
                }
                catch (Exception)
                {

                    throw;
                }
                return RedirectToAction("UpdateMyBusinessCardDetails", "MyBusinessMFBCCard", new { area = "Businesses" });

            }
            return RedirectToAction("UpdateMyBusinessCardDetails", "MyBusinessMFBCCard", new { area = "Businesses" });

        }
        public ActionResult UpdatePhoneNumber(string Telephone , int CardId)
        {
            Services.MFBCCardProfileServices services = new Services.MFBCCardProfileServices();
            var result = services.UpdateCardUserPhoneNumber(Telephone, CardId);
            if (result == true)
            {
                string BusinessEmail = Common.BusinessSession.LoggedBusinessMerchant.BusinessEmailAddress;
                MailCommon mail = new MailCommon();
                try
                {

                    string msg = "Your phone number has been changed to " + Telephone + " at " + DateTime.Now;
                    mail.SendMail(BusinessEmail, "Your Business Card Information has been Changed ", msg);
                }
                catch (Exception)
                {

                    throw;
                }
                return RedirectToAction("UpdateMyBusinessCardDetails", "MyBusinessMFBCCard", new { area = "Businesses" });

            }

            return RedirectToAction("UpdateMyBusinessCardDetails", "MyBusinessMFBCCard", new { area = "Businesses" });

        }
        public ActionResult UpdateCardPhoto(int id) {

            Services.MFBCCardProfileServices services = new Services.MFBCCardProfileServices();
            var fileName = "";
            if (Request.Files.Count > 0)
            {
                
                var deletePreviousImage = services.GetImageURL(id);
                var path =deletePreviousImage;
                if (System.IO.File.Exists(Server.MapPath("~/Documents") + "\\" + path))
                {
                    System.IO.File.Delete(Server.MapPath("~/Documents") + "\\" + path);
                }
                string directory = Server.MapPath("/Documents");
                var upload = Request.Files[0];
                fileName = "";
                if (upload != null && upload.ContentLength > 0)
                {
                    fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1]; //Path.GetFileName(upload.FileName);
                    upload.SaveAs(Server.MapPath("~/Documents") + "\\" + fileName);

                }
            }
           string CardPhotoURL = "/Documents/" + fileName;
            

            var result = services.UpdateCardUserPhoto(CardPhotoURL, id);
            if (result == true)
            {
                return RedirectToAction("UpdateMyBusinessCardDetails", "MyBusinessMFBCCard", new { area = "Businesses" });

            }

            return RedirectToAction("UpdateMyBusinessCardDetails", "MyBusinessMFBCCard", new { area = "Businesses" });

        }
    }
}