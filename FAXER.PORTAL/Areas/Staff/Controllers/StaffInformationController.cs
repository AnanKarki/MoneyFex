using FAXER.PORTAL.Areas.Staff.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Staff.Controllers
{
    public class StaffInformationController : Controller
    {
        // GET: Staff/StaffInformation
        StaffMessageServices Service = new StaffMessageServices();
        public ActionResult Index()
        {
            return RedirectToAction("StaffMainLogin", "StaffLogin");
        }
        [HttpGet]
        public ActionResult StaffInformation()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
    
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            else
            {
                Admin.Services.CommonServices getCardServices = new Admin.Services.CommonServices();
                var IdCardType = getCardServices.GetCardType();
                var countries = getCardServices.GetCountries();
                ViewBag.IDCardType = new SelectList(IdCardType, "Id", "CardType");
                ViewBag.Countries = new SelectList(countries, "Code", "Name");
                var loggedstaffDetails = Common.StaffSession.LoggedStaff;
                Services.StaffInformationServices services = new Services.StaffInformationServices();
                var result = services.GetStaffInformation(loggedstaffDetails.StaffId);
                ViewModels.StaffInformationViewModel vm = new ViewModels.StaffInformationViewModel()
                {
                    StaffId = result.Id,
                    FullName = result.FirstName + " " + result.MiddleName + " " + result.LastName,
                    Address = result.Address1 + "," + result.City + "," + result.Country,
                    Telephone = result.PhoneNumber,
                    Email = result.EmailAddress,

                    KinFullName = result.NOKFirstName + " " + result.NOKMiddleName + " " + result.NOKLastName,
                    KinAddress = result.NOKAddress1 + "," + result.NOKCity + "," + result.NOKCountry,
                    KinTelephone = result.NOKPhoneNumber,

                    ResidentPermit = result.ResidencePermitUrl,
                    PassportSide1 = result.PassportSide1Url,
                    PassportSide2 = result.PassportSide2Url,
                    UtilityBill = result.UtilityBillUrl,
                    CiriculamVitae = result.CVUrl,
                    HighestQual = result.HighestQualificationUrl

                };
                ViewBag.Count = Service.getInboxCount();
                ViewBag.DraftCount = Service.getDraftCount();
                return View(vm);
            }
        }
        public ActionResult UpdateStaffAddress(string Address, int id)
        {

            Services.StaffInformationServices services = new Services.StaffInformationServices();
            var updated = services.UpdateStaffAddress(Address, id);


            if (updated != null)
            {
                Common.StaffSession.StaffInformation = updated;

            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
        }
        public ActionResult UpdateStaffTelephone(string Telephone, int id)
        {

            Services.StaffInformationServices services = new Services.StaffInformationServices();
            var updated = services.UpdateStaffTelephone(Telephone, id);

            if (updated != null)
            {
                Common.StaffSession.StaffInformation = updated;

            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
        }
        public ActionResult UpdateStaffEmailAddress(string Email, int id)
        {

            Services.StaffInformationServices services = new Services.StaffInformationServices();
            var updated = services.UpdateStaffEmailAddress(Email, id);

            if (updated != null)
            {
                Common.StaffSession.StaffInformation = updated;

            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
        }
        public ActionResult UpdateKinFullName(string FullName, int id)
        {

            Services.StaffInformationServices services = new Services.StaffInformationServices();
            var updated = services.UpdateKinFullName(FullName, id);

            if (updated != null)
            {
                Common.StaffSession.StaffInformation = updated;

            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
        }
        public ActionResult UpdateKinTelephone(string Telephone, int id)
        {

            Services.StaffInformationServices services = new Services.StaffInformationServices();
            var updated = services.UpdateKinTelephone(Telephone, id);

            if (updated != null)
            {
                Common.StaffSession.StaffInformation = updated;

            }
            return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
        }
        public ActionResult UpdateKinAddress(string Address, int id)
        {

            Services.StaffInformationServices services = new Services.StaffInformationServices();
            var updated = services.UpdateKinAddress(Address, id);

            if (updated != null)
            {
                Common.StaffSession.StaffInformation = updated;

            }
            return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
        }
       
        public ActionResult UpdateResidencePermit(int id )
        {
            if (Request.Files.Count > 0) {
                Services.StaffInformationServices informationServices = new Services.StaffInformationServices();
                var deletePreviousImage = informationServices.GetImageURL(id);
                var path = deletePreviousImage.ResidencePermitUrl;
                string[] tokens = path.Split('/');
                string URL = tokens[2];
                if (System.IO.File.Exists(Server.MapPath("~/Documents") + "\\" + URL))
                {
                    System.IO.File.Delete(Server.MapPath("~/Documents") + "\\" + URL);
                }
                var fileName = "";
                var ImageFileURL = "";
                var upload = Request.Files[0];
                if (upload != null)
                {
                    string directory = Server.MapPath("~/Documents");
                    fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1];
                    if (upload != null && upload.ContentLength > 0)
                    {
                        upload.SaveAs(Server.MapPath("~/Documents") + "\\" + fileName);
                        ImageFileURL = "/Documents/" + fileName;
                    }
                }
                
                var result = informationServices.UpdateResidenePermit(ImageFileURL, id);
                if (result != null) {
                    Common.StaffSession.StaffInformation = result;


                    return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
                }

            }

            return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
        }
        public ActionResult UpdatePassportSide1(int id)
        {
            if (Request.Files.Count > 0)
            {
                Services.StaffInformationServices informationServices = new Services.StaffInformationServices();
                var deletePreviousImage = informationServices.GetImageURL(id);
                var path = deletePreviousImage.PassportSide1Url;
                string[] tokens = path.Split('/');
                string URL = tokens[2];
                if (System.IO.File.Exists(Server.MapPath("~/Documents") + "\\" + URL))
                {
                    System.IO.File.Delete(Server.MapPath("~/Documents") + "\\" + URL);
                }
                var fileName = "";
                var ImageFileURL = "";
                var upload = Request.Files[0];
                if (upload != null)
                {
                    string directory = Server.MapPath("~/Documents");
                    fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1];
                    if (upload != null && upload.ContentLength > 0)
                    {
                        upload.SaveAs(Server.MapPath("~/Documents") + "\\" + fileName);
                        ImageFileURL = "/Documents/" + fileName;
                    }
                }
                
                var result = informationServices.UpdatePassportSide1(ImageFileURL, id);
                if (result != null)
                {
                    Common.StaffSession.StaffInformation = result;

                    return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
                }

              
            }

            return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
        }
        public ActionResult UpdatePassportSide2(int id)
        {
            if (Request.Files.Count > 0)
            {
                Services.StaffInformationServices informationServices = new Services.StaffInformationServices();
                var deletePreviousImage = informationServices.GetImageURL(id);
                var path = deletePreviousImage.PassportSide2Url;
                string[] tokens = path.Split('/');
                string URL = tokens[2];
                if (System.IO.File.Exists(Server.MapPath("~/Documents") + "\\" + URL))
                {
                    System.IO.File.Delete(Server.MapPath("~/Documents") + "\\" + URL);
                }
                var fileName = "";
                var ImageFileURL = "";
                var upload = Request.Files[0];
                if (upload != null)
                {
                    string directory = Server.MapPath("~/Documents");
                    fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1];
                    if (upload != null && upload.ContentLength > 0)
                    {
                        upload.SaveAs(Server.MapPath("~/Documents") + "\\" + fileName);
                        ImageFileURL = "/Documents/" + fileName;
                    }
                }
                var result = informationServices.UpdatePassportSide2(ImageFileURL, id);
                if (result != null)
                {
                    Common.StaffSession.StaffInformation = result;

                    return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
                }


            }

            return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
        }
        public ActionResult UpdateUtilityBill(int id)
        {
            if (Request.Files.Count > 0)
            {
                Services.StaffInformationServices informationServices = new Services.StaffInformationServices();
                var deletePreviousImage = informationServices.GetImageURL(id);
                var path = deletePreviousImage.UtilityBillUrl;
                string[] tokens = path.Split('/');
                string URL = tokens[2];
                if (System.IO.File.Exists(Server.MapPath("~/Documents") + "\\" + URL))
                {
                    System.IO.File.Delete(Server.MapPath("~/Documents") + "\\" + URL);
                }
                var fileName = "";
                var ImageFileURL = "";
                var upload = Request.Files[0];
                if (upload != null)
                {
                    string directory = Server.MapPath("~/Documents");
                    fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1];
                    if (upload != null && upload.ContentLength > 0)
                    {
                        upload.SaveAs(Server.MapPath("~/Documents") + "\\" + fileName);
                        ImageFileURL = "/Documents/" + fileName;
                    }
                }
                var result = informationServices.UpdateUtilityBill(ImageFileURL, id);
                if (result != null)
                {
                    Common.StaffSession.StaffInformation = result;

                    return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
                }


            }

            return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
        }
        public ActionResult UpdateCV(int id)
        {
            if (Request.Files.Count > 0)
            {
                Services.StaffInformationServices informationServices = new Services.StaffInformationServices();
                var deletePreviousImage = informationServices.GetImageURL(id);
                var path = deletePreviousImage.CVUrl;
                string[] tokens = path.Split('/');
                string URL = tokens[2];
                if (System.IO.File.Exists(Server.MapPath("~/Documents") + "\\" + URL))
                {
                    System.IO.File.Delete(Server.MapPath("~/Documents") + "\\" + URL);
                }
                var fileName = "";
                var ImageFileURL = "";
                var upload = Request.Files[0];
                if (upload != null)
                {
                    string directory = Server.MapPath("~/Documents");
                    fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1];
                    if (upload != null && upload.ContentLength > 0)
                    {
                        upload.SaveAs(Server.MapPath("~/Documents") + "\\" + fileName);
                        ImageFileURL = "/Documents/" + fileName;
                    }
                }
                var result = informationServices.UpdateCV(ImageFileURL, id);
                if (result != null)
                {
                    Common.StaffSession.StaffInformation = result;

                    return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
                }


            }

            return RedirectToAction("StaffInformation", "StaffInformation", new { area = "Staff" });
        }
    }
}