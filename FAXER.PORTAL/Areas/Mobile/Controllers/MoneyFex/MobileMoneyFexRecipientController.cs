using FAXER.PORTAL.Areas.Mobile.Models.Common;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex;
using FAXER.PORTAL.Areas.Mobile.Services.Common;
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
    public class MobileMoneyFexRecipientController : Controller
    {
        // GET: Mobile/MobileMoneyFexRecipient
        MobileCommonServices mobileCommonServices = null;
        MobileMoneyFexRecipientServices _mobileMoneyFexRecipientServices = null;
        public MobileMoneyFexRecipientController()
        {
            mobileCommonServices = new MobileCommonServices();
            _mobileMoneyFexRecipientServices = new MobileMoneyFexRecipientServices();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetRecipientsBySenderId(int senderId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = (from c in _mobileMoneyFexRecipientServices.RecipientsList().Data.Where(x => x.SenderId == senderId).OrderByDescending(x=>x.Id).Take(20).ToList()
                              select new MobileRecipientDropdownViewModel()
                              {
                                  Service = c.Service,
                                  SenderId = c.SenderId,
                                  AccountNo = c.Service == Service.CashPickUP ? c.MobileNo : c.AccountNo,
                                  BankId = c.BankId,
                                  BranchCode = c.BranchCode,
                                  Country = FAXER.PORTAL.Common.Common.GetCountryName(c.Country),
                                  CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(c.Country),
                                  CountryCode = c.Country,
                                  IBusiness = c.IBusiness,
                                  Id = c.Id,
                                  MobileNo = c.MobileNo,
                                  MobileWalletProvider = c.MobileWalletProvider,
                                  Reason = c.Reason,
                                  ReceiverName = c.ReceiverName,
                                  BankName = c.Service == Service.BankAccount ? mobileCommonServices.GetBankName(c.BankId) :
                                  (c.Service == Service.MobileWallet ? mobileCommonServices.GetMobileWalletname(c.MobileWalletProvider)
                                   : FAXER.PORTAL.Common.Common.GetEnumDescription(Service.CashPickUP)),
                                  MobileWalletProviderName = mobileCommonServices.GetMobileWalletname(c.MobileWalletProvider),
                                  ServiceName = c.Service.ToString(),


                              });
                return Json(new ServiceResult<List<MobileRecipientDropdownViewModel>>()
                {
                    Data = result.ToList(),
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileRecipientDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetLimitedRecipientsBySenderId(int senderId, int count)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                var result = (from c in _mobileMoneyFexRecipientServices.LimitedRecipientsList(count).Data.Where(x => x.SenderId == senderId).ToList()
                              select new MobileRecipientDropdownViewModel()
                              {
                                  Service = c.Service,
                                  SenderId = c.SenderId,
                                  AccountNo = c.Service == Service.CashPickUP ? c.MobileNo : c.AccountNo,
                                  BankId = c.BankId,
                                  BranchCode = c.BranchCode,
                                  Country = FAXER.PORTAL.Common.Common.GetCountryName(c.Country),
                                  CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(c.Country),
                                  CountryCode = c.Country,
                                  IBusiness = c.IBusiness,
                                  Id = c.Id,
                                  MobileNo = c.MobileNo,
                                  MobileWalletProvider = c.MobileWalletProvider,
                                  Reason = c.Reason,
                                  ReceiverName = c.ReceiverName,
                                  BankName = c.Service == Service.BankAccount ? mobileCommonServices.GetBankName(c.BankId) :
                                  (c.Service == Service.MobileWallet ? mobileCommonServices.GetMobileWalletname(c.MobileWalletProvider)
                                   : FAXER.PORTAL.Common.Common.GetEnumDescription(Service.CashPickUP)),
                                  MobileWalletProviderName = mobileCommonServices.GetMobileWalletname(c.MobileWalletProvider),
                                  ServiceName = c.Service.ToString(),


                              }).OrderByDescending(x => x.Id).ToList();
                return Json(new ServiceResult<List<MobileRecipientDropdownViewModel>>()
                {
                    Data = result.Take(count).ToList(),
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileRecipientDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AddReceipentPost(MobileRecipentViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                model = _mobileMoneyFexRecipientServices.AddReceipients(model);


                return Json(new ServiceResult<MobileRecipentViewModel>()
                {
                    Data = model,
                    Message = "AddedSuccess",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<MobileRecipentViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }

        }
        [HttpPost]
        public JsonResult UpdateReceipentPost(MobileRecipentViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                model = _mobileMoneyFexRecipientServices.UpdateReceipients(model);
                return Json(new ServiceResult<MobileRecipentViewModel>()
                {
                    Data = model,
                    Message = "Updated",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<MobileRecipentViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public JsonResult DeleteReceipentPost(int ReceipentId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                var result = _mobileMoneyFexRecipientServices.DeleteReceipients(ReceipentId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>() {
                    Data = false,
                    Message = "",
                    Status = ResultStatus.Warning,
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetReceipentDetailById(int ReceipentId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var model = _mobileMoneyFexRecipientServices.ReceipientsDetails(ReceipentId);
                return Json(new ServiceResult<MobileRecipentViewModel>()
                {
                    Data = model,
                    Message = "AddedSuccess",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<MobileRecipentViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public JsonResult GetTransactionByRecipientId(int ReceipentId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var model = _mobileMoneyFexRecipientServices.TransacationStatementDetails(ReceipentId);
                return Json(new ServiceResult<List<TransactionDetailsViewModel>>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<TransactionDetailsViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }

        }

    }
}