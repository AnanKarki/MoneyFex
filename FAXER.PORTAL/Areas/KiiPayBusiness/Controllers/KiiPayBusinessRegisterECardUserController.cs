using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessRegisterECardUserController : Controller
    {

        KiiPayBusinessRegisterECardUserService _kiiPayBusinessRegisterECardUserService = null;
        public KiiPayBusinessRegisterECardUserController()
        {
            _kiiPayBusinessRegisterECardUserService = new KiiPayBusinessRegisterECardUserService();
        }
        // GET: KiiPayBusiness/KiiPayBusinessRegisterECardUser
        public ActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public ActionResult PersonalDetail()
        {
            var vm = _kiiPayBusinessRegisterECardUserService.GetKiiPayBusinessPersonalDetail();
            return View(vm);
        }

        [HttpPost]
        public ActionResult PersonalDetail([Bind(Include = KiiPayBusinessRegisterECardUserPersonalDetailVM.BindProperty)]KiiPayBusinessRegisterECardUserPersonalDetailVM vm)
        {

            if (ModelState.IsValid)
            {
                _kiiPayBusinessRegisterECardUserService.SetKiiPayBusinessPersonalDetail(vm);
                return RedirectToAction("AddressInformation");
            }
            return View(vm);

        }

        [HttpGet]
        public ActionResult AddressInformation()
        {
            GetCountryDropDown();
            var vm = _kiiPayBusinessRegisterECardUserService.GetKiiPayBusinessAddressInformation();
            return View(vm);
        }

        [HttpPost]
        public ActionResult AddressInformation([Bind(Include = KiiPayBusinessRegisterECardUserAddressInformationVM.BindProperty)] KiiPayBusinessRegisterECardUserAddressInformationVM vm)
        {
            GetCountryDropDown();
            if (ModelState.IsValid)
            {
                _kiiPayBusinessRegisterECardUserService.SetKiiPayBusinessAddressInformation(vm);
                return RedirectToAction("IdentificationInformation");
            }
            return View(vm);

        }
        [HttpGet]
        public ActionResult IdentificationInformation()
        {
            GetCountryDropDown();
            GetIDCardTypeDropDown();
            var vm = _kiiPayBusinessRegisterECardUserService.GetKiiPayBusinessIdentificationInformation();
            return View(vm);
        }
        [HttpPost]
        public ActionResult IdentificationInformation([Bind(Include = KiiPayBusinessRegisterECardUserIdentificationInformationVM.BindProperty)] KiiPayBusinessRegisterECardUserIdentificationInformationVM vm)
        {
            GetCountryDropDown();
            GetIDCardTypeDropDown();
            if (ModelState.IsValid)
            {
                _kiiPayBusinessRegisterECardUserService.SetKiiPayBusinessIdentificationInformation(vm);
                return RedirectToAction("PhotoInformation");
            }
            return View(vm);

        }

        [HttpGet]
        public ActionResult PhotoInformation()
        {
            KiiPayBusinessRegisterECardUserPhotoInformationVM vm = new KiiPayBusinessRegisterECardUserPhotoInformationVM();
            return View(vm);
        }
        [HttpPost]
        public ActionResult PhotoInformation([Bind(Include = KiiPayBusinessRegisterECardUserPhotoInformationVM.BindProperty)] KiiPayBusinessRegisterECardUserPhotoInformationVM vm)
        {
            if (ModelState.IsValid)
            {
                if (Request.Files.Count > 0)
                {

                    string FileName = "";
                    var uploadFile = Request.Files[0];
                    if (uploadFile != null && uploadFile.ContentLength > 0)
                    {

                        FileName = Guid.NewGuid() + "." + uploadFile.FileName.Split('.')[1];
                        uploadFile.SaveAs(Server.MapPath("~/Documents") + "\\" + FileName);
                    }
                    vm.PhotoUrl = "/Documents/" + FileName;
                }
                _kiiPayBusinessRegisterECardUserService.CompleteKiiPayBusinessRegisterECardUser(vm);
                return RedirectToAction("RegisterECardUserSuccess");
            }
            return View(vm);

        }

        public ActionResult RegisterECardUserSuccess()
        {
            return View();
        }
        private void GetCountryDropDown()
        {
            ViewBag.Countries = new SelectList(Common.Common.GetCountriesForDropDown(), "CountryCode", "CountryName");
        }
        private void GetIDCardTypeDropDown()
        {
            ViewBag.IDCardTypes = new SelectList(Common.Common.GetIdCardType(), "Name", "Name");
        }
    }
}