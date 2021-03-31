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
    public class KiiPayBusinessMobilePayForServicesController : Controller
    {
        // GET: Mobile/KiiPayBusinessMobilePayForServices
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public JsonResult PostLocal([Bind(Include = KiiPayBusinessMobileLocalPayForServicesModel.BindProperty)]KiiPayBusinessMobileLocalPayForServicesModel model)
        {

            int BusinessId = model.SenderKiiPayBusinessId;


            KiiPayBusinessCommonServices _KiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();

            DB.KiiPayBusinessLocalTransaction kiiPayBusinessLocalTransaction = new DB.KiiPayBusinessLocalTransaction()
            {
                AmountSent = model.SendingAmount,
                IsAutoPayment = false,
                IsRefunded = false,
                PayedFromKiiPayBusinessInformationId = BusinessId,
                PayedToKiiPayBusinessInformationId = model.ReceiverKiiPayBusinessId,
                PayedToKiiPayBusinessWalletInformationId = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(model.ReceiverKiiPayBusinessId).Id,
                PayedFromKiiPayBusinessWalletInformationId = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(BusinessId).Id,
                PaymentReference = model.PaymentReference,
                TransactionDate = DateTime.Now

            };
            var result = SaveKiiPayBusinessLocalTransaction(kiiPayBusinessLocalTransaction);
            //// Balance In To Account
            _KiiPayBusinessCommonServices.BalanceOut(result.PayedFromKiiPayBusinessWalletInformationId ?? 0, result.AmountSent);

            //// Balance Out from Account
            _KiiPayBusinessCommonServices.BalanceIn(result.PayedToKiiPayBusinessWalletInformationId, result.AmountSent);


            #region Wallet Statement Add 

            KiiPayBusinessWalletStatementServices _kiiPayBusinessWalletStatementServices = new KiiPayBusinessWalletStatementServices();
            KiiPayBusinessWalletStatementVM KiiPayBusinessWalletStatement = new KiiPayBusinessWalletStatementVM()
            {
                SendingAmount = result.AmountSent,
                Fee = model.Fee,
                ReceivingAmount = model.ReceivingAmount,
                SenderCurBal = _KiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.PayedFromKiiPayBusinessWalletInformationId),
                SenderCountry = model.SendingCountryCode,
                TransactionDate = result.TransactionDate,
                TransactionId = result.Id,
                ReceiverCurBal = _KiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.PayedToKiiPayBusinessWalletInformationId),
                ReceiverCountry = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId((int)result.PayedToKiiPayBusinessInformationId).Country,
                WalletStatmentType = DB.WalletStatmentType.KiiPayPersoanlPayment,
            };

            _kiiPayBusinessWalletStatementServices.AddkiiPayBusinessWalletStatement(KiiPayBusinessWalletStatement);

            #endregion

            // Send SMS 

            var senderInfo = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(result.PayedFromKiiPayBusinessWalletInformationId ?? 0);
            var receiverInfo = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(result.PayedToKiiPayBusinessWalletInformationId);
            KiiPayBusinessPaymentSmsVM smsModel = new KiiPayBusinessPaymentSmsVM()
            {

                PaymentReference = result.PaymentReference,
                ReceiverBusinessMobileNo = receiverInfo.MobileNo,
                ReceiverBusinessName = receiverInfo.KiiPayBusinessInformation.BusinessName,
                SenderName = senderInfo.KiiPayBusinessInformation.BusinessName,
                ReceiverCountry = receiverInfo.Country,
                ReceivingAmount = result.AmountSent.ToString(),
                SendingAmount = result.AmountSent.ToString(),
                SenderCountry = senderInfo.KiiPayBusinessInformation.BusinessCountry,
                SenderPhoneNo = senderInfo.KiiPayBusinessInformation.BusinessMobileNo
            };
            SendPaymentSms(smsModel);


            return Json(new ServiceResult<KiiPayBusinessMobileLocalPayForServicesModel>()
            {
                Data = model,
                Message = "AddedSuccess",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public JsonResult PostInternational([Bind(Include = KiiPayBusinessMobileInternationalPayForServicesModel.BindProperty)]KiiPayBusinessMobileInternationalPayForServicesModel model)
        {

            int BusinessId = model.SenderKiiPayBusinessId;


            KiiPayBusinessCommonServices _KiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();

            DB.KiiPayBusinessInternationalPaymentTransaction kiiPayBusinessInternationalPaymentTransaction = new DB.KiiPayBusinessInternationalPaymentTransaction()
            {
                //AmountSent = model.SendingAmount,
                //IsAutoPayment = false,
                //IsRefunded = false,
                //PayedFromKiiPayBusinessInformationId = BusinessId,
                //PayedToKiiPayBusinessInformationId = model.ReceiverKiiPayBusinessId,
                //PayedToKiiPayBusinessWalletInformationId = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(model.ReceiverKiiPayBusinessId).Id,
                //PayedFromKiiPayBusinessWalletInformationId = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(BusinessId).Id,
                //PaymentReference = model.PaymentReference,
                //TransactionDate = DateTime.Now

                ExchangeRate = model.ExchangeRate,
                FaxingAmount = model.SendingAmount,
                FaxingFee = model.Fee,
                IsAutoPayment = false,
                IsRefunded = false,
                PayedFromKiiPayBusinessInformationId = BusinessId,
                PayedFromKiiPayBusinessWalletId = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(BusinessId).Id,
                PayedToKiiPayBusinessInformationId = model.ReceiverKiiPayBusinessId,
                PayedToKiiPayBusinessWalletId = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(model.ReceiverKiiPayBusinessId).Id,
                PaymentReference = model.PaymentReference,
                ReceiptNumber = FAXER.PORTAL.Common.Common.GeneratePayForServicesReceiptNo(6),
                RecievingAmount = model.ReceivingAmount,
                TotalAmount = model.TotalAmount,
                TransactionDate = DateTime.Now,


            };
            var result = SaveKiiPayBusinessInternationalTransaction(kiiPayBusinessInternationalPaymentTransaction);
            //// Balance In To Account
            _KiiPayBusinessCommonServices.BalanceOut(result.PayedFromKiiPayBusinessWalletId, result.TotalAmount);

            //// Balance Out from Account
            _KiiPayBusinessCommonServices.BalanceIn(result.PayedToKiiPayBusinessWalletId, result.FaxingAmount);


            #region Wallet Statement Add 

            KiiPayBusinessWalletStatementServices _kiiPayBusinessWalletStatementServices = new KiiPayBusinessWalletStatementServices();
            KiiPayBusinessWalletStatementVM KiiPayBusinessWalletStatement = new KiiPayBusinessWalletStatementVM()
            {
                SendingAmount = result.FaxingAmount,
                Fee = model.Fee,
                ReceivingAmount = model.ReceivingAmount,
                SenderCurBal = _KiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.PayedFromKiiPayBusinessWalletId),
                SenderCountry = model.SendingCountryCode,
                TransactionDate = result.TransactionDate,
                TransactionId = result.Id,
                ReceiverCurBal = _KiiPayBusinessCommonServices.GetAccountBalanceByWalletId((int)result.PayedToKiiPayBusinessWalletId),
                ReceiverCountry = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId((int)result.PayedToKiiPayBusinessInformationId).Country,
                WalletStatmentType = DB.WalletStatmentType.KiiPayPersoanlPayment,
            };

            _kiiPayBusinessWalletStatementServices.AddkiiPayBusinessWalletStatement(KiiPayBusinessWalletStatement);

            #endregion

            // Send SMS 

            var senderInfo = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(result.PayedFromKiiPayBusinessWalletId);
            var receiverInfo = _KiiPayBusinessCommonServices.GetKiipayBusinessWalletInfo(result.PayedToKiiPayBusinessWalletId);
            KiiPayBusinessPaymentSmsVM smsModel = new KiiPayBusinessPaymentSmsVM()
            {

                PaymentReference = result.PaymentReference,
                ReceiverBusinessMobileNo = receiverInfo.MobileNo,
                ReceiverBusinessName = receiverInfo.KiiPayBusinessInformation.BusinessName,
                SenderName = senderInfo.KiiPayBusinessInformation.BusinessName,
                ReceiverCountry = receiverInfo.Country,
                ReceivingAmount = result.FaxingAmount.ToString(),
                SendingAmount = result.FaxingAmount.ToString(),
                SenderCountry = senderInfo.KiiPayBusinessInformation.BusinessCountry,
                SenderPhoneNo = senderInfo.KiiPayBusinessInformation.BusinessMobileNo
            };
            SendPaymentSms(smsModel);


            return Json(new ServiceResult<KiiPayBusinessMobileInternationalPayForServicesModel>()
            {
                Data = model,
                Message = "AddedSuccess",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);

        }


        public DB.KiiPayBusinessLocalTransaction SaveKiiPayBusinessLocalTransaction(DB.KiiPayBusinessLocalTransaction model)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            dbContext.KiiPayBusinessLocalTransaction.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        public DB.KiiPayBusinessInternationalPaymentTransaction SaveKiiPayBusinessInternationalTransaction(DB.KiiPayBusinessInternationalPaymentTransaction model)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            dbContext.KiiPayBusinessInternationalPaymentTransaction.Add(model);
            dbContext.SaveChanges();
            return model;
        }

        public void SendPaymentSms(KiiPayBusinessPaymentSmsVM model)
        {

            SmsApi smsApiServices = new SmsApi();
            smsApiServices.SendKiiPayBusinessPaymentSMS(model);

        }
    }
}