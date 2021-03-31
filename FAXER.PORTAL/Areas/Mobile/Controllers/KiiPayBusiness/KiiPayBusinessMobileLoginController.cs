using FAXER.PORTAL.Areas.Mobile.Services.Common;
using FAXER.PORTAL.Areas.Mobile.Services.KiiPayBusiness;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Mobile.Controllers.KiiPayBusiness
{
    // GET: Mobile/KiiPayBusinessMobileLogin
    public class KiiPayBusinessMobileLoginController : Controller
    {
        MobileKiiPayBusinessLoginServices mobileKiiPayBusinessLoginServices = new MobileKiiPayBusinessLoginServices();
        MobileRegistrationVerificationCodeService mobileRegistrationVerificationCodeService = new MobileRegistrationVerificationCodeService();
        
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult UpdateNewPasscode(string CountryCode, string MobileNo, string NewPassCode)
        {
            var data = mobileKiiPayBusinessLoginServices.List().Data.Where(x => x.KiiPayBusinessInformation.BusinessCountry == CountryCode && x.MobileNo == MobileNo).FirstOrDefault();
            if (data != null)
            {
                data.PinCode = NewPassCode.Encrypt();
                var result = mobileKiiPayBusinessLoginServices.Update(data);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult UpdateNewMobileNo(string CountryCode, string MobileNo, string NewMobileNo, string CountryPhoneCode)
        {

            string oldphoneno = CountryPhoneCode + " " + MobileNo;
            string newphoneno = CountryPhoneCode + " " + NewMobileNo;
            var Login = mobileKiiPayBusinessLoginServices.List().Data.Where(x => x.KiiPayBusinessInformation.BusinessCountry == CountryCode && x.MobileNo == MobileNo).FirstOrDefault();
            var RegistrationVerificationCode = mobileRegistrationVerificationCodeService.List().Data.Where(x => x.Country == CountryCode && x.PhoneNo == oldphoneno).FirstOrDefault();
            if (Login != null)
            {
                Login.MobileNo = NewMobileNo;
                var result = mobileKiiPayBusinessLoginServices.Update(Login);

            }
            if (RegistrationVerificationCode != null)
            {
                RegistrationVerificationCode.PhoneNo = newphoneno;
                var result = mobileRegistrationVerificationCodeService.Update(RegistrationVerificationCode);
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return Json(false, JsonRequestBehavior.AllowGet);

        }
    }
}