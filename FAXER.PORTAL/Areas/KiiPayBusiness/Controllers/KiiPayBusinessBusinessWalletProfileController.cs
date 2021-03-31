using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessBusinessWalletProfileController : Controller
    {

        KiiPayBusinessBusinessWalletProfileServices _kiiPayBusinessBusinessWalletProfileServices= null;
        public KiiPayBusinessBusinessWalletProfileController()
        {
            _kiiPayBusinessBusinessWalletProfileServices = new KiiPayBusinessBusinessWalletProfileServices();
        }
        // GET: KiiPayBusiness/KiiPayBusinessBusinessWalletProfile
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult ViewProfile()
        {
            var vm = _kiiPayBusinessBusinessWalletProfileServices.GetBusinessWalletProfile(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId);
            return View(vm);

        }

        [HttpGet]
        public ActionResult UpdateBusinessWalletProfile()
        {

            var vm = _kiiPayBusinessBusinessWalletProfileServices.GetBusinessWalletProfile(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId);
            return View(vm);

        }

        [HttpPost]
        public ActionResult UpdateBusinessWalletProfile([Bind(Include = KiiPayBusinessBusinessWalletProfileVM.BindProperty)]KiiPayBusinessBusinessWalletProfileVM vm)
        {
            if (ModelState.IsValid)
            {
                _kiiPayBusinessBusinessWalletProfileServices.UpdateBusinessWalletProfile(vm);
                
            }
            return View(vm);
        }
        
    }
}