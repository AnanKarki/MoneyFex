using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex.Login;
using FAXER.PORTAL.Areas.Mobile.Services.MoneyFex;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Mobile.Controllers.MoneyFex
{
    public class MoneyFexMobilePersonalLoginController : Controller
    {
        // GET: Mobile/MoneyFexMobilePersonalLogin

        MobileMoneyFexLoginServices _mobileMoneyFexLoginServices;


        public MoneyFexMobilePersonalLoginController()
        {
            _mobileMoneyFexLoginServices = new MobileMoneyFexLoginServices();

        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetOtpCodeAndUserIdByUserNameAndCountryCode(

            string userName , bool isForPasswordReset = false)
        {

            //please Check in FaxerLogin Table

            //TopUpType be removed
            try
            {
                var result = new MobileForgetPasswordModel();
                result = _mobileMoneyFexLoginServices.SendOTPCode(userName , isForPasswordReset);
                if (result != null)
                {
                    return Json(new ServiceResult<MobileForgetPasswordModel>()
                    {
                        Data = result,
                        Message = "",
                        Status = ResultStatus.OK
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ServiceResult<MobileForgetPasswordModel>()
                    {
                        Data = null,
                        Message = "User does not exist",
                        Status = ResultStatus.Error
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message, DB.ErrorType.UnSpecified, "GetOtpCodeAndUserIdByUserNameAndCountryCode");
                return Json(new ServiceResult<MobileForgetPasswordModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }


        }

        [HttpGet]
        public JsonResult GetApiVersion()
        {
            var result = _mobileMoneyFexLoginServices.GetApiVersion();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult IsPasswordChanged(int senderId, string newPassword)
        {

            //please update user id password 
            //in Faxer Login Table 

            //Note:Use Service to update

            var result = _mobileMoneyFexLoginServices.ChangePassword(senderId, newPassword);

            //TopUpType be removed
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Login(LoginUserInfoModel model)
        {
            try
            {
                var result = new ServiceResult<SenderLoginResponseModel>();
                if (!model.IsBusiness)
                {

                    result = _mobileMoneyFexLoginServices.PersonalLogin(model);
                }
                else
                {
                    result = _mobileMoneyFexLoginServices.BusinessLogin(model);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ServiceResult<LoginUserInfoModel>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }

        }


    }
    public class LoginUserInfoModel
    {
        public string MobileNo { get; set; }
        public string Password { get; set; }
        public bool IsBusiness { get; set; }

    }

    public class SenderLoginResponseModel
    {

        public int SenderId { get; set; }
        public string MobileNo { get; set; }
        public string SenderName { get; set; }
        public string CountryCode { get; set; }
        public string CurrencyCode { get; set; }
        public string CountryPhoneCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string Email { get; set; }
        public bool IsBusiness { get; set; }
        public decimal MonthlyTransactionLimitAmount { get; set; }
    }

}