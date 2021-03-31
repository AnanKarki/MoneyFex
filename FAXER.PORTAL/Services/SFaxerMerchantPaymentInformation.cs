using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SFaxerMerchantPaymentInformation
    {
        DB.FAXEREntities db = new DB.FAXEREntities();

        public DB.SenderKiiPayBusinessPaymentInformation SaveTransaction(DB.SenderKiiPayBusinessPaymentInformation obj, int BusinessInformationId)
        {
            //savae payment information
            int FaxerId = Common.FaxerSession.LoggedUser.Id;
            var findMerchant = db.FaxerMerchantPaymentInformation.Where(x => x.KiiPayBusinessInformationId == BusinessInformationId && x.SenderInformationId == FaxerId).FirstOrDefault();
            if (findMerchant == null)
            {
                db.FaxerMerchantPaymentInformation.Add(obj);
                db.SaveChanges();
            }
            if (obj.Id != 0)
            {
                string ReceiptNumber = GetNewPayGoodsandServicesReceipt();
                Common.FaxerSession.PayGoodsAndServicesReceiptNumber = ReceiptNumber;

                DB.SenderKiiPayBusinessPaymentTransaction tran = new DB.SenderKiiPayBusinessPaymentTransaction()
                {
                    SenderKiiPayBusinessPaymentInformationId = obj.Id,
                    PaymentAmount = obj.PaymentAmount,
                    PaymentDate = System.DateTime.Now,
                    PaymentReference = obj.PaymentRefrence,
                    FaxingFee = Common.FaxerSession.FaxingAmountSummary.FaxingFee,
                    PaymentMethod = Common.FaxerSession.PaymentMethod,
                    ReceiptNumber = ReceiptNumber,
                    ExchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate,
                    ReceivingAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount

                };
                db.FaxerMerchantPaymentTransaction.Add(tran);
                db.SaveChanges();
            }
            if (obj.Id == 0)
            {
                string ReceiptNumber = GetNewPayGoodsandServicesReceipt();
                Common.FaxerSession.PayGoodsAndServicesReceiptNumber = ReceiptNumber;

                DB.SenderKiiPayBusinessPaymentTransaction tran = new DB.SenderKiiPayBusinessPaymentTransaction()
                {
                    SenderKiiPayBusinessPaymentInformationId = findMerchant.Id,
                    PaymentAmount = obj.PaymentAmount,
                    PaymentDate = System.DateTime.Now,
                    PaymentReference = Common.FaxerSession.PaymentRefrence,
                    FaxingFee = Common.FaxerSession.FaxingAmountSummary.FaxingFee,
                    PaymentMethod = Common.FaxerSession.PaymentMethod,
                    ReceiptNumber = ReceiptNumber,
                    ExchangeRate = Common.FaxerSession.FaxingAmountSummary.ExchangeRate,
                    ReceivingAmount = Common.FaxerSession.FaxingAmountSummary.ReceivingAmount


                };
                db.FaxerMerchantPaymentTransaction.Add(tran);
                db.SaveChanges();
            }

            DB.KiiPayBusinessWalletInformation cardInfo = new DB.KiiPayBusinessWalletInformation();
            cardInfo = db.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == obj.KiiPayBusinessInformationId && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
            if (cardInfo != null)
            {
                cardInfo.CurrentBalance += Common.FaxerSession.FaxingAmountSummary.ReceivingAmount;
                db.Entry<DB.KiiPayBusinessWalletInformation>(cardInfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return obj;
        }

        public DB.KiiPayBusinessInformation GetBusinessMerchantDetials(int KiiPayBusinessInformationId) {

            var result = db.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessInformationId).FirstOrDefault();
            return result;
        }
        public string GetBusinessEmail(int KiiPayBusinessInformationId)
        {

            var result = db.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessInformationId).Select(x => x.Email).FirstOrDefault();
            return result;
        }

        internal string GetNewPayGoodsandServicesReceipt()
        {


            //this code should be unique and random with 8 digit length
            var val = "Os-Mp-MF" + Common.Common.GenerateRandomDigit(5);

            while (db.FaxerMerchantPaymentTransaction.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Os-Mp-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }


        public DB.SenderKiiPayBusinessPaymentInformation PayBusinessMerchant(DB.SenderKiiPayBusinessPaymentInformation obj, int BusinessInformationId)
        {
            //savae payment information
            int FaxerId = Common.FaxerSession.LoggedUser.Id;
            var findMerchant = db.FaxerMerchantPaymentInformation.Where(x => x.KiiPayBusinessInformationId == BusinessInformationId && x.SenderInformationId == FaxerId).FirstOrDefault();
            obj.Id = findMerchant == null ? 0 : findMerchant.Id;
            DB.SenderKiiPayBusinessPaymentTransaction tran = new DB.SenderKiiPayBusinessPaymentTransaction();
            if (findMerchant == null)
            {
                db.FaxerMerchantPaymentInformation.Add(obj);
                db.SaveChanges();
            }
            if (obj.Id != 0)
            {
                string ReceiptNumber = GetNewPayGoodsandServicesReceipt();
                Common.FaxerSession.PayGoodsAndServicesReceiptNumber = ReceiptNumber;

                 tran = new DB.SenderKiiPayBusinessPaymentTransaction()
                {
                    SenderKiiPayBusinessPaymentInformationId = obj.Id,
                    PaymentAmount = obj.PaymentAmount,
                    PaymentDate = System.DateTime.Now,
                    PaymentReference = obj.PaymentRefrence,
                    FaxingFee = Common.FaxerSession.TransactionSummary.KiiPayTransferPaymentSummary.Fee,
                    PaymentMethod = Common.FaxerSession.PaymentMethod,
                    ReceiptNumber = ReceiptNumber,
                    ExchangeRate = Common.FaxerSession.TransactionSummary.KiiPayTransferPaymentSummary.ExchangeRate,
                    ReceivingAmount = Common.FaxerSession.TransactionSummary.KiiPayTransferPaymentSummary.ReceivingAmount,
                    SendingCountry = Common.FaxerSession.TransactionSummary.SenderAndReceiverDetail.SenderCountry,
                    ReceivingCountry = Common.FaxerSession.TransactionSummary.SenderAndReceiverDetail.ReceiverCountry,
                    TotalAmount = Common.FaxerSession.TransactionSummary.KiiPayTransferPaymentSummary.TotalAmount,
                    PaymentType = Common.FaxerSession.TransactionSummary.IsLocalPayment == true ? DB.PaymentType.Local : DB.PaymentType.International,
                    SenderPaymentMode  = Common.FaxerSession.TransactionSummary.PaymentMethodAndAutoPaymentDetail.SenderPaymentMode

                };
                db.FaxerMerchantPaymentTransaction.Add(tran);
                db.SaveChanges();
            }
           
            DB.KiiPayBusinessWalletInformation cardInfo = new DB.KiiPayBusinessWalletInformation();
            cardInfo = db.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == obj.KiiPayBusinessInformationId && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
            if (cardInfo != null)
            {

                
                cardInfo.CurrentBalance += tran.ReceivingAmount;
                db.Entry<DB.KiiPayBusinessWalletInformation>(cardInfo).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return obj;
        }


    }
}