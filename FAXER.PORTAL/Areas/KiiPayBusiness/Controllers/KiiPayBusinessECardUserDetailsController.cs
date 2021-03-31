using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessECardUserDetailsController : Controller
    {
        KiiPayBusinessECardUserDetailServices _kiiPayBusinessECardUserDetailServices = null;
        public KiiPayBusinessECardUserDetailsController()
        {
            _kiiPayBusinessECardUserDetailServices = new KiiPayBusinessECardUserDetailServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessECardUserDetails
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult EcardUserDetails()
        {
            GetALLWalletInformation();

            //var vm = _kiiPayBusinessECardUserDetailServices.GetEcardUserDetails();
            var vm = new KiiPayECardUserDetailsVm();
            return View(vm);

        }

        [HttpPost]
        public ActionResult DeleteEcardUser([Bind(Include = KiiPayECardUserDetailsVm.BindProperty)]KiiPayECardUserDetailsVm vm)
        {
            GetALLWalletInformation();
            return View();
        }

        [HttpGet]
        public ActionResult UpdateEcardUserDetails(int WalletId)
        {
            var vm = _kiiPayBusinessECardUserDetailServices.GetEcardUserDetails(WalletId);
            return View(vm);

        }

        [HttpPost]
        public ActionResult UpdateEcardUserDetails([Bind(Include = KiiPayECardUserDetailsVm.BindProperty)]KiiPayECardUserDetailsVm vm)
        {
            if (ModelState.IsValid)
            {
                _kiiPayBusinessECardUserDetailServices.UpdateEcardUserInfo(vm);
                return View();
            }
            return View(vm);
        }
        private void GetALLWalletInformation()
        {
            ViewBag.Wallet = new SelectList(_kiiPayBusinessECardUserDetailServices.GetWalletInformation( Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId),
                "Id", "UserName");
        }

        public JsonResult GetWalletInfo(int Id) {

            var data = _kiiPayBusinessECardUserDetailServices.GetEcardUserDetails(Id);
            return Json(new
            {
                Name = data.Name,
                City = data.City,
                Country = data.Country,
                Address = data.Address,
                DateOfBirth = data.DateOfBirth,
                EmailAddress = data.EmailAddress,
                PhoneNo = data.PhoneNo,
                MobileNo = data.MobileNo,
                IDCardNumber = data.IDCardNumber,
                ExpiringDateDay = data.ExpiringDateDay,
                ExpiringDateMonth = data.ExpiringDateMonth,
                ExpiringDateYear = data.ExpiringDateYear,
                IDCardType = data.IDCardType,
                IDIssuingCountry = data.IDIssuingCountry,
            }, JsonRequestBehavior.AllowGet);

        }
    }
}