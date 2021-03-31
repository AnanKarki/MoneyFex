using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex;
using FAXER.PORTAL.Areas.Mobile.Services.MoneyFex;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Mobile.Controllers.MoneyFex
{
    public class MobileActivityController : Controller
    {
        MobileActivityServices _mobileActivityService = null;
        public MobileActivityController()
        {
            _mobileActivityService = new MobileActivityServices();
        }
        // GET: Mobile/MobileActivity
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetActivityList(int senderId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = _mobileActivityService.GetActivityList(senderId);
                return Json(new ServiceResult<List<MobileActivityListvm>>()
                {
                    Data = result.Data.ToList(),
                    Message = result.Message,
                    Status = result.Status
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileActivityListvm>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpGet]
        public JsonResult GetActivityDetailsByTransactionIdAdTransferMethod(int transactionId, TransactionTransferMethod transferMethod)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = _mobileActivityService.GetActivityDetails(transactionId, transferMethod);
                return Json(new ServiceResult<MobileActivityViewModel>()
                {
                    Data = result.Data,
                    Message = result.Message,
                    Status = result.Status
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<MobileActivityViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public JsonResult GetCountOfUnseenNotification(int senderId)
        {

            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = _mobileActivityService.GetCountOfUnseenNotification(senderId);
            return Json(new ServiceResult<int>()
            {
                Data = result,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
                }
            else
            {
                return Json(new ServiceResult<int>()
                {
                    Data = 0,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetRecepientExist(int senderId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = _mobileActivityService.GetRecepientExist(senderId);
                return Json(new ServiceResult<bool>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public JsonResult UpdateUserNotificationToken(int senderId , string notificationToken)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = _mobileActivityService.UpdateUserNotificationToken(senderId, notificationToken);
                return Json(new ServiceResult<bool>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }
    }
}