using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewRegisteredMFBCCardsController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        Services.ViewRegisteredMFBCCardsServices Service = new Services.ViewRegisteredMFBCCardsServices();
        // GET: Admin/ViewRegisteredMFBCCards
        public ActionResult Index(string CountryCode = "", string City = "", string message="")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "wrong")
            {
                ViewBag.Message = "Something went wrong. Please try again !";
                message = "";
            }
            else if (message == "cardSaved")
            {
                ViewBag.Message = "MFBC Card uploaded successfully !";
                message = "";
            }
            else if (message == "empty")
            {
                ViewBag.Message = "Sorry, Card Number can't be empty !";
                message = "";
            }
            else if (message == "invalidCard")
            {
                ViewBag.Message = "Sorry ! MFBC Card with that number doesn't exist.";
                message = "";
            }
            else if (message == "noPhoto")
            {
                ViewBag.Message = "Sorry, Photo Not Found !";
                message = "";
            }
            else if (message == "isDeleted")
            {
                ViewBag.Message = "Sorry, This card has already been deleted !";
                message = "";
            }
            SetViewBagForCountries();
            SetViewBagForSCities(CountryCode);
            var RegisteredFBCCards = Service.getMFBCCardsInfo(CountryCode, City);
            if (RegisteredFBCCards != null)
            {
                ViewBag.Country = CountryCode;
            }
            Common.MiscSession.CountryCode = CountryCode;
            Common.MiscSession.City = City;
            return View(RegisteredFBCCards);
        }


        public ActionResult UpdateCardStatus(int id)
        {
            if (id != 0)
            {
                var result = Service.UpdateCardStatus(id);
                if (result)
                {
                    return RedirectToAction("Index", "ViewRegisteredMFBCCards");
                }
                return RedirectToAction("Index", "ViewRegisteredMFBCCards");
            }
            return RedirectToAction("Index", "ViewRegisteredMFBCCards");

        }

        public ActionResult DeleteCard(int id)
        {
            if (id != 0)
            {
                var result = Service.DeleteCard(id);
                if (result)
                {
                    return RedirectToAction("Index", "ViewRegisteredMFBCCards");
                }
                return RedirectToAction("Index", "ViewRegisteredMFBCCards");
            }
            return RedirectToAction("Index", "ViewRegisteredMFBCCards");
        }

        public ActionResult ViewRegisteredMFBCCardsMore(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (id != 0)
            {
                var viewmodel = Service.getMFBCCardsInfoMore(id);
                return View(viewmodel);

            }
            return View();
        }
        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.BusinessMerchant, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }
        private void SetViewBagForCountries()
        {
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }

        [HttpGet]
        public ActionResult UploadMFBCCardPhoto(string cardNum)
        {
            if (!string.IsNullOrEmpty(cardNum))
            {
                bool checkCard = Service.cardExist(cardNum);
                if (checkCard)
                {
                    bool isDeleted = Service.cardDeletedOrNot(cardNum);
                    if (isDeleted == false)
                    {
                        return RedirectToAction("Index",new { @message = "isDeleted" });
                    }
                    var vm = Service.getMFBCardInfomn(cardNum);
                    return View(vm);
                }
                return RedirectToAction("Index", new { @message = "invalidCard" });
            }
            return RedirectToAction("Index", new { @message = "empty" });
        }

        [HttpPost]
        public ActionResult UploadMFBCCardPhoto([Bind(Include = UploadMFBCardPhotoViewModel.BindProperty)]UploadMFBCardPhotoViewModel model)
        {
            if (model != null)
            {
                if (Request.Files.Count > 0)
                {
                    string fileName = "";
                    string directory = Server.MapPath("/Documents");
                    var CardImage = Request.Files["CardImage"];

                    if (CardImage != null && CardImage.ContentLength > 0)
                    {
                        fileName = Guid.NewGuid() + "." + CardImage.FileName.Split('.')[1];
                        CardImage.SaveAs(Path.Combine(directory, fileName));
                        model.PhotoURL = "/Documents/" + fileName;
                    }                    
                }
                if (string.IsNullOrEmpty(model.PhotoURL))
                {
                    return RedirectToAction("Index", new { @message="noPhoto"});
                }
                bool savePhoto = Service.saveCardPhoto(model);
                if (savePhoto)
                {
                    return RedirectToAction("Index", new { @message="cardSaved"});
                }
            }
            return RedirectToAction("Index", new { @message="wrong"});
        }
    }

}