using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Mobile.Controllers.KiiPayBusiness
{
    public class KiiPayBusinessTransferKiiPayToKiiPayController : Controller
    {

        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        // GET: Mobile/KiiPayBusinessTransferKiiPayToKiiPay
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult PostLocal([Bind(Include = KiiPayBusinessMobileTransferKiiPayToKiiPayModel.BindProperty)]KiiPayBusinessMobileTransferKiiPayToKiiPayModel model)
        {

            int BusinessId = model.KiiPayBusinessId;


            KiiPayBusinessCommonServices _KiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();

            DB.KiiPayPersonalWalletPaymentByKiiPayBusiness KiiPayBusinessmodel = new DB.KiiPayPersonalWalletPaymentByKiiPayBusiness()
            {
                KiiPayBusinessInformationId = BusinessId,
                KiiPayBusinessWalletInformationId = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(BusinessId).Id,
                KiiPayPersonalWalletInformationId = model.ReciverId,
                ExchangeRate = 1,
                Fee = 0,
                PayingAmount = model.Amount,
                PaymentReference = model.Reference,
                RecievingAmount = model.Amount,
                TotalAmount = model.Amount,
                PaymentType = DB.PaymentType.Local,
                ReceiptNumber = "",
                TransactionDate = DateTime.Now
            };
            var result = SaveKiiPayPersonalInternationalByKiiPayBusinessPayment(KiiPayBusinessmodel);

            KiiPayBusinessPaymentCompletedVM vm = new KiiPayBusinessPaymentCompletedVM()
            {
                SendingAmount = result.PayingAmount,
                ReceivingAmount = result.RecievingAmount,
                ReceiverName = model.ReceiverFullName,
                SendingCurrencySymbol = model.CurrencySymbol,
                ReceivingCurrencySymbol = model.CurrencySymbol

            };

            //// Balance In To Account
            _KiiPayBusinessCommonServices.BalanceOut(result.KiiPayBusinessWalletInformationId, result.PayingAmount);

            //// Balance Out from Account
            FAXER.PORTAL.Common.Common.KiiPayPersonalWalletBalanceIN(result.KiiPayPersonalWalletInformationId, result.RecievingAmount);


            //_KiiPayBusinessCommonServices.UpdateAccountBalance();


            #region Wallet Statement Add 

            KiiPayBusinessWalletStatementServices _kiiPayBusinessWalletStatementServices = new KiiPayBusinessWalletStatementServices();
            KiiPayBusinessWalletStatementVM KiiPayBusinessWalletStatement = new KiiPayBusinessWalletStatementVM()
            {
                SendingAmount = result.PayingAmount,
                Fee = model.Fee,
                ReceivingAmount = model.Amount,
                SenderCurBal = _KiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.KiiPayBusinessWalletInformationId),
                SenderCountry = model.CountryCode,
                TransactionDate = result.TransactionDate,
                TransactionId = result.Id,
                ReceiverCurBal = _KiiPayBusinessCommonServices.GetPersonalAccountBalanceByWalletId((int)result.KiiPayPersonalWalletInformationId).CurrentBalance,
                ReceiverCountry = _KiiPayBusinessCommonServices.GetPersonalAccountBalanceByWalletId((int)result.KiiPayPersonalWalletInformationId).CardUserCountry,
                WalletStatmentType = DB.WalletStatmentType.KiiPayPersoanlPayment,
            };

            _kiiPayBusinessWalletStatementServices.AddkiiPayBusinessWalletStatement(KiiPayBusinessWalletStatement);

            #endregion

            // Send SMS 

            var senderInfo = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(result.KiiPayBusinessWalletInformationId);
            var receiverInfo = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(result.KiiPayPersonalWalletInformationId);
               KiiPayPersonalPaymentSMSVM smsModel = new KiiPayPersonalPaymentSMSVM()
                {
                    SenderName = senderInfo.KiiPayBusinessInformation.BusinessName,
                    ReceiverCountry = receiverInfo.Country,
                    ReceiverPhoneNo = receiverInfo.MobileNo,
                    ReceivingAmount = result.RecievingAmount.ToString(),
                    SendingAmount = result.PayingAmount.ToString(),
                    SenderCountry = senderInfo.KiiPayBusinessInformation.BusinessCountry,
                    SenderPhoneNo = senderInfo.KiiPayBusinessInformation.BusinessMobileNo
                };
                SendLocalPaymentSms(smsModel);


            return Json(new ServiceResult<KiiPayBusinessMobileTransferKiiPayToKiiPayModel>()
            {
                Data = model,
                Message = "AddedSuccess",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        public JsonResult PostInternational([Bind(Include = KiiPayBusinessMobileAbroadTransferKiiPayToKiiPayModel.BindProperty)]KiiPayBusinessMobileAbroadTransferKiiPayToKiiPayModel model)
        {

            int BusinessId = model.KiiPayBusinessId;

            KiiPayBusinessCommonServices _KiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();

            DB.KiiPayPersonalWalletPaymentByKiiPayBusiness KiiPayBusinessmodel = new DB.KiiPayPersonalWalletPaymentByKiiPayBusiness()
            {
                KiiPayBusinessInformationId = BusinessId,
                KiiPayBusinessWalletInformationId = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(BusinessId).Id,
                KiiPayPersonalWalletInformationId = model.ReciverId,
                ExchangeRate = model.ExchangeRate,
                Fee = model.Fee,
                PayingAmount = model.TotalAmount,
                PaymentReference = model.Reference,
                RecievingAmount = model.ReceivingAmount,
                TotalAmount = model.TotalAmount,
                PaymentType = DB.PaymentType.International,
                ReceiptNumber = "",
                TransactionDate = DateTime.Now
            };
            var result = SaveKiiPayPersonalInternationalByKiiPayBusinessPayment(KiiPayBusinessmodel);

            KiiPayBusinessPaymentCompletedVM vm = new KiiPayBusinessPaymentCompletedVM()
            {
                SendingAmount = result.PayingAmount,
                ReceivingAmount = result.RecievingAmount,
                ReceiverName = model.ReceiverFullName,
                SendingCurrencySymbol = model.SenderCurrencySymbol,
                ReceivingCurrencySymbol = model.ReceiverCurrencySymbol

            };

            // Balance In To Account
            _KiiPayBusinessCommonServices.BalanceOut(result.KiiPayBusinessWalletInformationId, result.PayingAmount);

            // Balance Out from Account
           FAXER.PORTAL.Common.Common.KiiPayPersonalWalletBalanceIN(result.KiiPayPersonalWalletInformationId, result.RecievingAmount);


            #region Wallet Statement Add 

            KiiPayBusinessWalletStatementServices _kiiPayBusinessWalletStatementServices = new KiiPayBusinessWalletStatementServices();
            KiiPayBusinessWalletStatementVM KiiPayBusinessWalletStatement = new KiiPayBusinessWalletStatementVM()
            {
                SendingAmount = result.PayingAmount,
                Fee = model.Fee,
                ReceivingAmount = model.ReceivingAmount,
                SenderCurBal = _KiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.KiiPayBusinessWalletInformationId),
                SenderCountry = model.SenderCountryCode,
                TransactionDate = result.TransactionDate,
                TransactionId = result.Id,
                ReceiverCurBal = _KiiPayBusinessCommonServices.GetPersonalAccountBalanceByWalletId((int)result.KiiPayPersonalWalletInformationId).CurrentBalance,
                //result.KiiPayPersonalWalletInformation.CurrentBalance,
                ReceiverCountry = _KiiPayBusinessCommonServices.GetPersonalAccountBalanceByWalletId((int)result.KiiPayPersonalWalletInformationId).CardUserCountry,
                WalletStatmentType = DB.WalletStatmentType.KiiPayPersoanlPayment,
            };

            _kiiPayBusinessWalletStatementServices.AddkiiPayBusinessWalletStatement(KiiPayBusinessWalletStatement);

            #endregion
            return Json(new ServiceResult<KiiPayBusinessMobileAbroadTransferKiiPayToKiiPayModel>()
            {
                Data = model,
                Message = "AddedSuccess",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);

        }


        public DB.KiiPayPersonalWalletPaymentByKiiPayBusiness SaveKiiPayPersonalInternationalByKiiPayBusinessPayment(DB.KiiPayPersonalWalletPaymentByKiiPayBusiness model)
        {

            dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public void SendLocalPaymentSms(KiiPayPersonalPaymentSMSVM model)
        {

            SmsApi smsApiServices = new SmsApi();
            smsApiServices.SendKiiPayPersonalPaymentSMS(model);

        }
    }
}