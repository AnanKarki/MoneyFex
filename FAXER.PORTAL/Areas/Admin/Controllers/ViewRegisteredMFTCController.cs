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
    public class ViewRegisteredMFTCController : Controller
    {
        Services.ViewRegisteredMFTCServices Service = new Services.ViewRegisteredMFTCServices();

        Services.CommonServices services = new Services.CommonServices();
        // GET: Admin/ViewRegisteredMFTC
        public ActionResult Index(string CountryCode , string City, string message="")
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
                ViewBag.Message = "MFTC Card uploaded successfully !";
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
                ViewBag.Message = "Sorry, this card has already been deleted !";
                message = "";
            }
            var countries = services.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            SetViewBagForSCities(CountryCode);
            var viewmodel = new List<ViewModels.ViewRegisteredMFTCFaxerViewModel>();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                viewmodel = Service.getFilterMFTCList(CountryCode, City);
                ViewBag.Country = CountryCode;

            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                viewmodel = Service.getFilterMFTCList(CountryCode, City);
                ViewBag.Country = CountryCode;
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {

                viewmodel = Service.getFilterMFTCList(CountryCode, City);
                ViewBag.Country = CountryCode;
                ViewBag.City = City;


            }
            else
            {
                viewmodel = Service.getMFTCFaxerList();
                ViewBag.Country = "";
            }
            // var vm = Service.getMFTCList();
            
            return View(viewmodel);
        }

        public ActionResult MFTCCardInformation (int id, string message="")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (message == "deleted")
            {
                ViewBag.Message = "Card deleted successfully !";
                ViewBag.ToastrVal = 4;
                message = "";
            }
            else if (message == "notDeleted")
            {
                ViewBag.Message = "Card was not deleted. Please try again !";
                ViewBag.ToastrVal = 1;
                message = "";
            }

                var vm = Service.getMFTCList(id);
                return View(vm);
            
            
        }

        public ActionResult DeleteMFTC(int id, int faxerid)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (id != 0)
            {
                var result = Service.DeleteMFTCCard(id);
                if (result)
                {
                    return RedirectToAction("MFTCCardInformation", new { id = faxerid, message="deleted" });
                }
            }
            return RedirectToAction("MFTCCardInformation", new { id = faxerid, message="notDeleted" });
        }

        public ActionResult ActivateCard(int id, int faxerid)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (id != 0)
                {
                var result = Service.ActivateCard(id);
                if (result)
                {
                    return RedirectToAction("MFTCCardInformation", new { id = faxerid });
                }
            }
            return RedirectToAction("MFTCCardInformation", new { id = faxerid });

        }

        public void SetViewBagForSCities(string Country = "")
        {
            
            var cities = SCity.GetCities(DB.Module.Faxer, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }


        [HttpGet]
        public ActionResult UploadMFTCCardPhoto(string cardNum)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (!string.IsNullOrEmpty(cardNum))
            {
                bool checkCard = Service.cardExist(cardNum);
                if (checkCard)
                {
                    bool isDeleted = Service.deletedOrNot(cardNum);
                    if (isDeleted == false)
                    {
                        return RedirectToAction("Index", new { @message="isDeleted"});
                    }
                    var vm = Service.getMFTCardInfomn(cardNum);
                    return View(vm);
                }
                return RedirectToAction("Index", new { @message = "invalidCard" });
            }
            return RedirectToAction("Index", new { @message = "empty" });
        }

        [HttpPost]
        public ActionResult UploadMFTCCardPhoto([Bind(Include = UploadMFTCardPhotoViewModel.BindProperty)]UploadMFTCardPhotoViewModel model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
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
                    return RedirectToAction("Index", new { @message = "noPhoto" });
                }
                bool savePhoto = Service.saveCardPhoto(model);
                if (savePhoto)
                {
                    return RedirectToAction("Index", new { @message = "cardSaved" });
                }
            }
            return RedirectToAction("Index", new { @message = "wrong" });
        }
    }
}