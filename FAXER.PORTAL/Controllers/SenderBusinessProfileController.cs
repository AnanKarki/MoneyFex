using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class SenderBusinessProfileController : Controller
    {
        SenderBusinessProfileServices _services = null;

        public SenderBusinessProfileController()
        {
            _services = new SenderBusinessProfileServices();
        }
        // GET: SenderBusinessProfile
        public ActionResult Index()
        {
            if (Common.FaxerSession.LoggedUser == null)
            {
                return RedirectToAction("Login", "FaxerAccount");
            }

            int FaxerId = Common.FaxerSession.LoggedUser.Id;
            SenderBusinessprofileViewModel vm = new SenderBusinessprofileViewModel();
            vm = _services.GetSenderBusinessProfileData(FaxerId);

            ViewBag.IsPinCodeSend = 0;
            ViewBag.PinCode = "";
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = SenderBusinessprofileViewModel.BindProperty)]SenderBusinessprofileViewModel vm)
        {
            if (Common.FaxerSession.LoggedUser == null)
            {
                return RedirectToAction("Login", "FaxerAccount");
            }
            int FaxerId = Common.FaxerSession.LoggedUser.Id;
            vm = _services.GetSenderBusinessProfileData(FaxerId);
            ViewBag.IsPinCodeSend = 1;
            ViewBag.PinCode = _services.GetMobilePin();

            return View(vm);
        }
        public ActionResult UpdateProfile(string PinCode = " ")
        {
            if (Common.FaxerSession.LoggedUser == null)
            {
                return RedirectToAction("Login", "FaxerAccount");
            }
            ViewBag.IsPinCodeSend = 1;
            if (PinCode != _services.GetMobilePin())
            {
                return RedirectToAction("Index");
            }
            SenderBusinessprofileViewModel vm = new SenderBusinessprofileViewModel();
            int FaxerId = Common.FaxerSession.LoggedUser.Id;
            vm = _services.GetSenderBusinessProfileData(FaxerId);
            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdateProfile([Bind(Include = SenderBusinessprofileViewModel.BindProperty)]SenderBusinessprofileViewModel vm)
        {
            if (Common.FaxerSession.LoggedUser == null)
            {
                return RedirectToAction("Login", "FaxerAccount");
            }
            int FaxerId = Common.FaxerSession.LoggedUser.Id;


            if (ModelState.IsValid)
            {
                var faxerInfo = _services.GetFaxerInfo(FaxerId);
                if (faxerInfo.PhoneNumber != vm.PhoneNumber)
                {
                    vm.PhoneNumber = Common.Common.IgnoreZero(vm.PhoneNumber);
                    bool IsMobileNoExist = Common.OtherUtlities.IsMobileNoExist(vm.PhoneNumber);
                    if (IsMobileNoExist == false)
                    {
                        ModelState.AddModelError("MobileNo", "Mobile number already exists");
                        return View(vm);
                    }

                }
                if (faxerInfo.Email != vm.Email)
                {
                    vm.Email.Trim();
                    bool isEmailExist = Common.OtherUtlities.IsEmailExist(vm.Email);
                    if (isEmailExist == false)
                    {
                        ModelState.AddModelError("EmailAddress", "Email address already exists");
                        return View(vm);
                    }
                }


                _services.Update(vm, FaxerId);
                return RedirectToAction("Index", "SenderBusinessProfile");
            }


            return View(vm);
        }
    }
}