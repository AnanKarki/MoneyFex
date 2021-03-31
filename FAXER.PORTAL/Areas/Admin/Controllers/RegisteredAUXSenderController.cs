using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class RegisteredAUXSenderController : Controller
    {

        CommonServices _commonServices = null;
        SFaxerInformation _faxerinformation = null;
        RegisterAuxSenderServices _registerAuxSenderServices = null;
        public RegisteredAUXSenderController()
        {

            _commonServices = new CommonServices();
            _faxerinformation = new SFaxerInformation();
            _registerAuxSenderServices = new RegisterAuxSenderServices();
        }
        // GET: Admin/RegisteredAUXSender
        public ActionResult Index(string SendingCountry = "", string City = "", string Date = "", string SenderName = "", string AccountNo = "", string Address = "",
            string Telephone = "", string Email = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var Cities = _commonServices.GetCities();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

            ViewBag.SenderName = SenderName;
            ViewBag.Address = Address;
            ViewBag.AccountNo = AccountNo;
            ViewBag.Telephone = Telephone;
            ViewBag.Email = Email;
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            IPagedList<RegisteredAUXSenderViewModel> vm = _registerAuxSenderServices.GetRegisteredAUXSenders(SendingCountry, City, Date, SenderName,
                AccountNo, Address, Telephone, Email).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }
        [HttpGet]
        public JsonResult DeleteAUXSender(int id)
        {
            if (id > 0)
            {
                var Agent = _faxerinformation.list().Data.Where(x => x.Id == id).FirstOrDefault();
                Agent.IsDeleted = true;
                _faxerinformation.Update(Agent);
                return Json(new
                {
                    Data = true,
                    Message = "Deleted Sucessfully"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Data = false,
                    Message = "Something went wrong. Please try again!"
                }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}