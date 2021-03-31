using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Controllers
{
    public class KiiPayBusinessHomeController : Controller
    {
        KiiPayBusinessCommonServices _commonServices = null;

        public KiiPayBusinessHomeController()
        {
            _commonServices = new KiiPayBusinessCommonServices();
        }

        // GET: KiiPayBusiness/KiiPayBusinessHome
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult DashBoard() {
            if (Common.BusinessSession.LoggedKiiPayBusinessUserInfo != null)
            {
                int businessId = (int)Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
                GetViewBagForKiipayUser(businessId);
            }
            GetViewBagForKiipayUser(0);
            return View();
        }

        public JsonResult UpdateCurrentBal() {


            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
            var data = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId);
            Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CurrentBalanceOnCard = data.CurrentBalance;

            return Json(new
            {
                CurBal = data.CurrentBalance
            }, JsonRequestBehavior.AllowGet);
        }


        public JsonResult GetWithdrawalCode(int kiiPayUSerId)
        {
            int KiiPayBusinessInformationId = (int) Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();


            DB.FAXEREntities db = new DB.FAXEREntities();


            var data = _kiiPayBusinessCommonServices.GetBusinessWithdrawalCode(KiiPayBusinessInformationId);
            if (data == null)
            {

                DB.KiiPayBusinessWalletWithdrawalCode BusinessWithdrawalCode = new DB.KiiPayBusinessWalletWithdrawalCode()
                {
                    KiiPayBusinessInformationId = KiiPayBusinessInformationId,
                    AccessCode = _kiiPayBusinessCommonServices.GenerateWithdrawalCode(),
                    IsExpired = false,
                    CreatedDateTime = DateTime.Now,
                    KiiPayUserId = kiiPayUSerId
                };
                data = _kiiPayBusinessCommonServices.AddBusinessWithdrawalCode(BusinessWithdrawalCode);
                

            }

            return Json(new
            {
                AccessCode = data.AccessCode
            }, JsonRequestBehavior.AllowGet);

        }


        public void GetViewBagForKiipayUser(int kiiPayBusinessId)
        {
            var list = _commonServices.GetViewBagForKiipayBusinessWalletInfoByKiiPayBusinessId(kiiPayBusinessId);
            ViewBag.KiiBusinessUsers = new SelectList(list,"Id", "Name");

        }
    }
}