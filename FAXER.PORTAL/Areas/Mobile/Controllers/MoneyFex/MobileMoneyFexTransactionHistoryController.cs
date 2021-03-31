using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex;
using FAXER.PORTAL.Areas.Mobile.Services.MoneyFex;
using FAXER.PORTAL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Mobile.Controllers.MoneyFex
{
    public class MobileMoneyFexTransactionHistoryController : Controller
    {
        MobileTransacationHistoryServices _mobileTransacationHistoryServices = null;

        public MobileMoneyFexTransactionHistoryController()
        {
            _mobileTransacationHistoryServices = new MobileTransacationHistoryServices();
        }
        // GET: Mobile/MobileMoneyFexTransactionHistory
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetTransactionList(int senderId, int skip, int take)
        {
            try
            {
                string token = FAXER.PORTAL.Common.Common.RequestToken;
                if (FAXER.PORTAL.Common.Common.ValidateToken(token))
                {
                    if (senderId != 0)
                    {
                        var result = _mobileTransacationHistoryServices.GetTransactionHistory(senderId, 0, 0);
                        return Json(new ServiceResult<List<MobileTransacationListvm>>()
                        {
                            Data = result.Skip(skip).Take(take).ToList(),
                            Message = "",
                            Status = ResultStatus.OK
                        }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new ServiceResult<List<MobileTransacationListvm>>()
                    {
                        Data = null,
                        Message = "",
                        Status = ResultStatus.Error
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ServiceResult<List<MobileTransacationListvm>>()
                    {
                        Data = null,
                        Message = "",
                        Status = ResultStatus.Warning
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new ServiceResult<List<MobileTransacationListvm>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult GetNotificationList(int senderId)
        {
            try
            {
                string token = FAXER.PORTAL.Common.Common.RequestToken;
                if (FAXER.PORTAL.Common.Common.ValidateToken(token))
                {
                    if (senderId != 0)
                    {
                        var result = _mobileTransacationHistoryServices.GetNotificationHistory(senderId).ToList();

                        return Json(new ServiceResult<List<MobileNotificationViewModel>>()
                        {
                            Data = result,
                            Message = "",
                            Status = ResultStatus.OK
                        }, JsonRequestBehavior.AllowGet);


                    }
                    else
                    {
                        return Json(new ServiceResult<List<MobileNotificationViewModel>>()
                        {
                            Data = null,
                            Message = "",
                            Status = ResultStatus.Error
                        }, JsonRequestBehavior.AllowGet);

                    }
                }
                else
                {
                    return Json(new ServiceResult<List<MobileNotificationViewModel>>()
                    {
                        Data = null,
                        Message = "",
                        Status = ResultStatus.Warning
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {

                return Json(new ServiceResult<List<MobileNotificationViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }




        }

        [HttpGet]
        public JsonResult NoticationSeen(int notificationId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = _mobileTransacationHistoryServices.UpdateNotificationToSeen(notificationId);
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
        public JsonResult GetYearlyTransactionDataList(int senderId)
        {
            try
            {
                string token = FAXER.PORTAL.Common.Common.RequestToken;
                if (FAXER.PORTAL.Common.Common.ValidateToken(token))
                {
                    if (senderId != 0)
                    {
                        var result = _mobileTransacationHistoryServices.GetYearlyTransactionDataList(senderId);
                        return Json(new ServiceResult<List<YearlyTransactionDataListvm>>()
                        {
                            Data = result.ToList(),
                            Message = "",
                            Status = ResultStatus.OK
                        }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new ServiceResult<List<YearlyTransactionDataListvm>>()
                    {
                        Data = null,
                        Message = "",
                        Status = ResultStatus.Error
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ServiceResult<List<MobileTransacationListvm>>()
                    {
                        Data = null,
                        Message = "",
                        Status = ResultStatus.Warning
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new ServiceResult<List<MobileTransacationListvm>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }


        }




        [HttpGet]
        public JsonResult ChangeNotificationStatusForIsSeen(int notificationId)
        {
            try
            {
                string token = FAXER.PORTAL.Common.Common.RequestToken;
                if (FAXER.PORTAL.Common.Common.ValidateToken(token))
                {
                    if (notificationId != 0)
                    {
                        bool result = _mobileTransacationHistoryServices.ChangeNotificationStatusForIsSeen(notificationId);
                        return Json(new ServiceResult<bool>()
                        {
                            Data = result,
                            Message = "",
                            Status = ResultStatus.OK
                        }, JsonRequestBehavior.AllowGet);
                    }
                    return Json(new ServiceResult<bool>()
                    {
                        Data = false,
                        Message = "",
                        Status = ResultStatus.Error
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
            catch (Exception ex)
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
