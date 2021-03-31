using FAXER.PORTAL.Areas.Mobile.Models.Common;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex.SignUp;
using FAXER.PORTAL.Areas.Mobile.Services.Common;
using FAXER.PORTAL.Areas.Mobile.Services.MoneyFex;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Mobile.Controllers.MoneyFex
{
    public class MoneyFexMobileBusinessSignUpController : Controller
    {
        // GET: Mobile/MoneyFexMobileBusinessSignUp


        MobileMoneyFexSignUpServices _mobileMoneyFexSignUpServices;

        public MoneyFexMobileBusinessSignUpController()
        {
            _mobileMoneyFexSignUpServices = new MobileMoneyFexSignUpServices();
        }

        /// <summary>
        /// Not Used
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        { 
            return View();
        }

        //BusinessPost
        [HttpPost]
        public JsonResult BusinessSignUpPost([Bind(Include = MobileBusinessSignUpModel.BindProperty)]MobileBusinessSignUpModel model)
        {
            try
            {
              var result = _mobileMoneyFexSignUpServices.CompleteBusinessSignUp(model);
                return Json(new ServiceResult<MobileBusinessSignUpModel>()
                {
                    Data = result.Data,
                    Message = result.Message,
                    Status = result.Status,
                    Token = result.Token,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new ServiceResult<MobilePersonalSignUpModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
           

        }
        //PersonalPost
        [HttpPost]
        public JsonResult PersonalSignUpPost([Bind(Include = MobilePersonalSignUpModel.BindProperty)]MobilePersonalSignUpModel model)
        {

            try
            {
                var result = _mobileMoneyFexSignUpServices.CompletePersonalSignUp(model);


                return Json(new ServiceResult<MobilePersonalSignUpModel>()
                {
                    Data = result.Data,
                    Message = result.Message,
                    Status = result.Status,
                    Token = result.Token,
                }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new ServiceResult<MobilePersonalSignUpModel>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.Error
                }, JsonRequestBehavior.AllowGet);
            }
         
           
            

        }
    }
}