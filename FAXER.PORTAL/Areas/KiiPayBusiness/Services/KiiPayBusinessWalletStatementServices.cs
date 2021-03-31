using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessWalletStatementServices
    {
        DB.FAXEREntities dbContext = null;

        KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = null;
        public KiiPayBusinessWalletStatementServices()
        {
            dbContext = new DB.FAXEREntities();
            _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
        }

        public List<WalletStatementVM> GetWalletStatement(WalletStatementFilterType WalletStatementFilterType,int BusinessId)
        {


            List<WalletStatementVM> result = new List<WalletStatementVM>();

            switch (WalletStatementFilterType)
            {
                case WalletStatementFilterType.All:

                    result = GetKiiPersonalPaymentOut(BusinessId).Concat(GetKiiBusinessInternationalPaymentByPersonalIN(BusinessId))
                                            .Concat(GetKiiBusinessNationalPaymentByPersonalIN(BusinessId))
                                            .Concat(GetKiiBusinessNationalPaymentByBusinessIN(BusinessId))
                                            .Concat(GetKiiBusinessNationalPaymentByBusinessOut(BusinessId))
                                            .Concat(GetKiiBusinessInternationalPaymentByBusinessIN(BusinessId))
                                            .Concat(GetKiiBusinessInternationalPaymentByBusinessOUT(BusinessId))
                                            .Concat(GetCreditDebitCardPayment(BusinessId))
                                            .OrderByDescending(x => x.TransactionDateTime).ToList();
                    break;
                case WalletStatementFilterType.In:
                    result = GetKiiBusinessInternationalPaymentByPersonalIN(BusinessId)
                                            .Concat(GetKiiBusinessNationalPaymentByPersonalIN(BusinessId))
                                            .Concat(GetKiiBusinessNationalPaymentByBusinessIN(BusinessId))
                                            .Concat(GetKiiBusinessInternationalPaymentByBusinessIN(BusinessId))
                                            .Concat(GetCreditDebitCardPayment(BusinessId))
                                            .OrderByDescending(x => x.TransactionDateTime).ToList();
                    break;
                case WalletStatementFilterType.Out:
                    result = GetKiiPersonalPaymentOut(BusinessId)
                                            .Concat(GetKiiBusinessNationalPaymentByBusinessOut(BusinessId))
                                            .Concat(GetKiiBusinessInternationalPaymentByBusinessOUT(BusinessId))
                                            .OrderByDescending(x => x.TransactionDateTime).ToList();
                    break;


            }

            return result;


        }

        public List<WalletStatementVM> GetKiiPersonalPaymentOut(int BusinessId)
        {


            var data = dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayBusinessInformationId == BusinessId).ToList();

            var result = (from c in data
                          join d in dbContext.KiiPayBusinessWalletStatement on c.Id equals d.TransactionId
                          where d.WalletStatmentStatus == WalletStatmentStatus.OutBound && d.WalletStatmentType == WalletStatmentType.KiiPayPersoanlPayment
                          select new WalletStatementVM()
                          {
                              Amount = d.Amount,
                              TrasactionStatus = TrasactionStatus.Out,
                              ReceivingCurrency = Common.Common.GetCurrencySymbol(d.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCurrencySymbol(d.SenderCountry),
                              Fee = d.Fee,
                              MobileNo = c.KiiPayPersonalWalletInformation.MobileNo,
                              PaymentType = "Kii Pay persoanl payment",
                              ReceiverORSenderName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + "" + c.KiiPayPersonalWalletInformation.LastName,
                              TotalAmount = c.TotalAmount,
                              TransactionDate = c.TransactionDate.ToString("yyyy/MM/dd"),
                              TransactionId = c.Id,
                              TransactionDateTime = c.TransactionDate,
                              PaymentReference = c.PaymentReference,
                              WalletStatementType = WalletStatementType.KiiPayPersonalOut,
                              Balance = d.CurBal
                          }).ToList();
            return result;
        }
        public List<WalletStatementVM> GetKiiBusinessInternationalPaymentByPersonalIN(int BusinessId)
        {


            var data = dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedToKiiPayBusinessInformationId == BusinessId).ToList();

            var result = (from c in data
                          join d in dbContext.KiiPayBusinessWalletStatement on c.Id equals d.TransactionId
                          where d.WalletStatmentStatus == WalletStatmentStatus.InBound && d.WalletStatmentType == WalletStatmentType.KiiPayPersonalToBusinessInternationalPayment
                          select new WalletStatementVM()
                          {
                              Amount = d.Amount,
                              TrasactionStatus = TrasactionStatus.IN,
                              SendingCurrency = Common.Common.GetCurrencySymbol(d.SenderCountry),
                              ReceivingCurrency = Common.Common.GetCurrencySymbol(d.SenderCountry),
                              Fee = d.Amount,
                              MobileNo = c.KiiPayPersonalWalletInformation.MobileNo,
                              PaymentType = "Kii Pay persoanl payment",
                              ReceiverORSenderName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + "" + c.KiiPayPersonalWalletInformation.LastName,
                              TotalAmount = c.TotalAmount,
                              TransactionDate = c.TransactionDate.ToString("yyyy/MM/dd"),
                              TransactionId = c.Id,
                              TransactionDateTime = c.TransactionDate,
                              PaymentReference = c.PaymentReference,
                              WalletStatementType = WalletStatementType.KiiPayPersonalIn,
                              Balance = d.CurBal

                          }).ToList();
            return result;
        }

        public List<WalletStatementVM> GetKiiBusinessNationalPaymentByPersonalIN( int BusinessId)
        {


            var data = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayBusinessWalletInformation.KiiPayBusinessInformationId == BusinessId).ToList();

            var result = (from c in data
                          join d in dbContext.KiiPayBusinessWalletStatement on c.Id equals d.TransactionId
                          where d.WalletStatmentStatus == WalletStatmentStatus.InBound && d.WalletStatmentType == WalletStatmentType.KiiPayPersoanlPayment
                          select new WalletStatementVM()
                          {
                              Amount = d.Amount,
                              TrasactionStatus = TrasactionStatus.IN,
                              SendingCurrency = Common.Common.GetCurrencySymbol(d.SenderCountry),
                              ReceivingCurrency = Common.Common.GetCurrencySymbol(d.ReceivingCountry),
                              Fee = d.Fee,
                              MobileNo = c.KiiPayPersonalWalletInformation.MobileNo,
                              PaymentType = "Kii Pay persoanl payment",
                              ReceiverORSenderName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + "" + c.KiiPayPersonalWalletInformation.LastName,
                              TotalAmount = c.AmountSent,
                              TransactionDate = c.TransactionDate.ToString("yyyy/MM/dd"),
                              TransactionId = c.Id,
                              TransactionDateTime = c.TransactionDate,
                              PaymentReference = c.PaymentReference,
                              WalletStatementType = WalletStatementType.KiiPayPersonalIn,
                              Balance = d.CurBal
                          }).ToList();
            return result;
        }

        public List<WalletStatementVM> GetKiiBusinessNationalPaymentByBusinessIN(int BusinessId)
        {


            var data = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedToKiiPayBusinessInformationId == BusinessId).ToList();
            var result = (from c in data
                          join d in dbContext.KiiPayBusinessWalletStatement on c.Id equals d.TransactionId
                          where d.WalletStatmentStatus == WalletStatmentStatus.InBound && d.WalletStatmentType == WalletStatmentType.BusinessNationalPayment
                          select new WalletStatementVM()
                          {
                              Amount = d.Amount,
                              TrasactionStatus = TrasactionStatus.IN,
                              ReceivingCurrency = Common.Common.GetCurrencySymbol(d.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCurrencySymbol(d.SenderCountry),
                              Fee = d.Amount,
                              MobileNo = c.PayedFromKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              PaymentType = "Business National Payment",
                              ReceiverORSenderName = c.PayedFromKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                              TotalAmount = c.AmountSent,
                              TransactionDate = c.TransactionDate.ToString("yyyy/MM/dd"),
                              TransactionId = c.Id,
                              TransactionDateTime = c.TransactionDate,
                              PaymentReference = c.PaymentReference,
                              WalletStatementType = WalletStatementType.BusinessPaymentNationalIn,
                              Balance = d.CurBal

                          }).ToList();
            return result;
        }


        public List<WalletStatementVM> GetKiiBusinessNationalPaymentByBusinessOut(int BusinessId)
        {


            var data = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessInformationId == BusinessId).ToList();
            var result = (from c in data
                          join d in dbContext.KiiPayBusinessWalletStatement on c.Id equals d.TransactionId
                          where d.WalletStatmentStatus == WalletStatmentStatus.OutBound && d.WalletStatmentType == WalletStatmentType.BusinessNationalPayment
                          select new WalletStatementVM()
                          {
                              Amount = c.AmountSent,
                              TrasactionStatus = TrasactionStatus.Out,
                              ReceivingCurrency = Common.Common.GetCurrencySymbol(d.ReceivingCountry),
                              SendingCurrency= Common.Common.GetCurrencySymbol(d.SenderCountry),
                              Fee = 0,
                              MobileNo = c.PayedToKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              PaymentType = "Business National Payment",
                              ReceiverORSenderName = c.PayedToKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessName,
                              TotalAmount = c.AmountSent,
                              TransactionDate = c.TransactionDate.ToString("yyyy/MM/dd"),
                              TransactionId = c.Id,
                              TransactionDateTime = c.TransactionDate,
                              PaymentReference = c.PaymentReference,
                              WalletStatementType = WalletStatementType.BusinessPaymentNationalOut,
                              Balance = d.CurBal
                          }).ToList();
            return result;
        }



        public List<WalletStatementVM> GetKiiBusinessInternationalPaymentByBusinessIN(int BusinessId)
        {



            var data = dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.PayedToKiiPayBusinessInformationId == BusinessId).ToList();
            var result = (from c in data
                          join receiver in dbContext.KiiPayBusinessWalletInformation on c.PayedToKiiPayBusinessWalletId equals receiver.Id
                          join d in dbContext.KiiPayBusinessWalletStatement on c.Id equals d.TransactionId
                          where d.WalletStatmentStatus == WalletStatmentStatus.InBound && d.WalletStatmentType == WalletStatmentType.BusinessInternationalPayment
                          select new WalletStatementVM()
                          {
                              Amount = d.Amount,
                              TrasactionStatus = TrasactionStatus.IN,
                              SendingCurrency= Common.Common.GetCurrencySymbol(d.SenderCountry),
                              ReceivingCurrency = Common.Common.GetCurrencySymbol(d.ReceivingCountry),
                              Fee = c.FaxingFee,
                              MobileNo = receiver.KiiPayBusinessInformation.BusinessMobileNo,
                              PaymentType = "Business National Payment",
                              ReceiverORSenderName = receiver.KiiPayBusinessInformation.BusinessName,
                              TotalAmount = c.TotalAmount,
                              TransactionDate = c.TransactionDate.ToString("yyyy/MM/dd"),
                              TransactionId = c.Id,
                              TransactionDateTime = c.TransactionDate,
                              PaymentReference = c.PaymentReference,
                              WalletStatementType = WalletStatementType.BusinessPaymentInternationalIn,
                              Balance = d.CurBal

                          }).ToList();
            return result;
        }


        public List<WalletStatementVM> GetKiiBusinessInternationalPaymentByBusinessOUT(int BusinessId)
        {



            var data = dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.PayedFromKiiPayBusinessInformationId == BusinessId).ToList();
            var result = (from c in data
                          join sender in dbContext.KiiPayBusinessWalletInformation on c.PayedFromKiiPayBusinessWalletId equals sender.Id
                          join d in dbContext.KiiPayBusinessWalletStatement on c.Id equals d.TransactionId
                          where d.WalletStatmentStatus == WalletStatmentStatus.OutBound && d.WalletStatmentType == WalletStatmentType.BusinessInternationalPayment
                          select new WalletStatementVM()
                          {
                              Amount = d.Amount,
                              TrasactionStatus = TrasactionStatus.Out,
                              ReceivingCurrency = Common.Common.GetCurrencySymbol(d.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCurrencySymbol(d.SenderCountry),
                              Fee = d.Fee,
                              MobileNo = sender.KiiPayBusinessInformation.BusinessMobileNo,
                              PaymentType = "Business National Payment",
                              ReceiverORSenderName = sender.KiiPayBusinessInformation.BusinessName,
                              TotalAmount = c.TotalAmount,
                              TransactionDate = c.TransactionDate.ToString("yyyy/MM/dd"),
                              TransactionDateTime = c.TransactionDate,
                              TransactionId = c.Id,
                              PaymentReference = c.PaymentReference,
                              WalletStatementType = WalletStatementType.BusinessPaymentInternationalOut,
                              Balance = d.CurBal

                          }).ToList();
            return result;
        }


        public List<WalletStatementVM> GetCreditDebitCardPayment(int BusinessId)
        {



            var KiipayInfo = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(BusinessId);


            var data = dbContext.AddMoneyToKiiPayBusinessWallet.Where(x => x.KiipayBusinessWalletId == KiipayInfo.Id).ToList();
            var result = (from c in data
                          join d in dbContext.KiiPayBusinessWalletStatement on c.Id equals d.TransactionId
                          where d.WalletStatmentStatus == WalletStatmentStatus.OutBound && d.WalletStatmentType == WalletStatmentType.CreditDebitCard
                          select new WalletStatementVM()
                          {
                              Amount = c.Amount,
                              TrasactionStatus = TrasactionStatus.Out,
                              SendingCurrency = Common.Common.GetCurrencySymbol(KiipayInfo.KiiPayBusinessInformation.BusinessCountry),
                              ReceivingCurrency = Common.Common.GetCurrencySymbol(KiipayInfo.KiiPayBusinessInformation.BusinessCountry),
                              Fee = 0,
                              MobileNo = Common.Common.FormatSavedCardNumber(c.CardNum),
                              PaymentType = "Credit Debit Card Deposit",
                              ReceiverORSenderName = KiipayInfo.KiiPayBusinessInformation.BusinessName,
                              TotalAmount = c.Amount,
                              TransactionDate = c.TransactionDate.ToString("yyyy/MM/dd"),
                              TransactionDateTime = c.TransactionDate,
                              TransactionId = c.Id,
                              PaymentReference = Common.Common.FormatSavedCardNumber(c.CardNum),
                              WalletStatementType = WalletStatementType.BusinessPaymentInternationalOut,
                              Balance = d.CurBal,
                          }).ToList();
            return result;
        }



        public bool KiiPayPersonalPaymentRefund(int Id)
        {


            var data = dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.Id == Id).FirstOrDefault();
            data.IsRefunded = true;
            dbContext.Entry<KiiPayPersonalWalletPaymentByKiiPayBusiness>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();


            SaveRefundedTransaction(data.PayingAmount, data.Id, TypeOfTransaction.KiiPayPersonalPayment);

            _kiiPayBusinessCommonServices.BalanceIn(data.KiiPayBusinessWalletInformationId, data.PayingAmount);
            _kiiPayBusinessCommonServices.BalanceOut(data.KiiPayPersonalWalletInformationId, data.RecievingAmount);
            return true;
        }

        public bool RefundBusinessNationalPayment(int Id)
        {

            var data = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.Id == Id).FirstOrDefault();
            data.IsRefunded = true;
            dbContext.Entry<KiiPayBusinessLocalTransaction>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            SaveRefundedTransaction(data.AmountSent, data.Id, TypeOfTransaction.BusinessNationalPayment);


            _kiiPayBusinessCommonServices.BalanceIn(data.PayedFromKiiPayBusinessInformationId, data.AmountSent);
            _kiiPayBusinessCommonServices.BalanceOut(data.PayedToKiiPayBusinessWalletInformationId, data.AmountSent);
            return true;

        }
        public bool RefundBusinessInternationalPayment(int Id)
        {

            var data = dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.Id == Id).FirstOrDefault();
            data.IsRefunded = true;
            dbContext.Entry<KiiPayBusinessInternationalPaymentTransaction>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            SaveRefundedTransaction(data.FaxingAmount, data.Id, TypeOfTransaction.BusinessInternationalPayment);

            _kiiPayBusinessCommonServices.BalanceIn(data.PayedFromKiiPayBusinessWalletId, data.FaxingAmount);
            _kiiPayBusinessCommonServices.BalanceOut(data.PayedToKiiPayBusinessWalletId, data.RecievingAmount);
            return true;

        }
        public DB.KiiPayBusinessRefundedTransaction SaveRefundedTransaction(decimal Amount, int TransactionId, TypeOfTransaction typeOfTransaction)
        {


            DB.KiiPayBusinessRefundedTransaction refundedTransaction = new KiiPayBusinessRefundedTransaction()
            {
                AmountRefunded = Amount,
                RefundedDate = DateTime.Now,
                TransactionId = TransactionId,
                TypeOfTransaction = typeOfTransaction
            };

            dbContext.RefundedTransaction.Add(refundedTransaction);
            dbContext.SaveChanges();
            return refundedTransaction;


        }



        public void AddkiiPayBusinessWalletStatement(KiiPayBusinessWalletStatementVM model) {

            #region Balance In Bound
            DB.KiiPayBusinessWalletStatement kiiPayBusinessWalletStatementOutBound = new KiiPayBusinessWalletStatement()
            {
                Amount = model.SendingAmount,
                SenderCountry  = model.SenderCountry,
                CurBal = model.SenderCurBal,
                Fee = model.Fee,
                ReceivingCountry = model.ReceiverCountry,
                TransactionDate = model.TransactionDate,
                TransactionId = model.TransactionId,
                WalletStatmentStatus = WalletStatmentStatus.OutBound,
                WalletStatmentType = model.WalletStatmentType                
                
            };
            #endregion

            #region Balance Out Bound
            DB.KiiPayBusinessWalletStatement kiiPayBusinessWalletStatementInBound = new KiiPayBusinessWalletStatement()
            {
                Amount = model.ReceivingAmount,
                SenderCountry = model.SenderCountry,
                CurBal = model.ReceiverCurBal,
                Fee = model.Fee,
                ReceivingCountry = model.ReceiverCountry,
                TransactionDate = model.TransactionDate,
                TransactionId = model.TransactionId,
                WalletStatmentStatus = WalletStatmentStatus.InBound,
                WalletStatmentType = model.WalletStatmentType
            };

            #endregion
            dbContext.KiiPayBusinessWalletStatement.Add(kiiPayBusinessWalletStatementInBound);
            dbContext.KiiPayBusinessWalletStatement.Add(kiiPayBusinessWalletStatementOutBound);
            dbContext.SaveChanges();
        }
        public void AddkiiPayBusinessWalletStatementofCreditDebitCard(KiiPayBusinessWalletStatementVM model) {

            #region Balance In Bound
            DB.KiiPayBusinessWalletStatement kiiPayBusinessWalletStatementOutBound = new KiiPayBusinessWalletStatement()
            {
                Amount = model.SendingAmount,
                SenderCountry  = model.SenderCountry,
                CurBal = model.SenderCurBal,
                Fee = model.Fee,
                ReceivingCountry = model.ReceiverCountry,
                TransactionDate = model.TransactionDate,
                TransactionId = model.TransactionId,
                WalletStatmentStatus = WalletStatmentStatus.InBound,
                WalletStatmentType = model.WalletStatmentType                
                
            };
            #endregion

            dbContext.KiiPayBusinessWalletStatement.Add(kiiPayBusinessWalletStatementOutBound);
            dbContext.SaveChanges();
        }
        public void AddOutboundkiiPayBusinessWalletStatementofCreditDebitCard(KiiPayBusinessWalletStatementVM model) {

            #region Balance OutBound
            DB.KiiPayBusinessWalletStatement kiiPayBusinessWalletStatementOutBound = new KiiPayBusinessWalletStatement()
            {
                Amount = model.SendingAmount,
                SenderCountry  = model.SenderCountry,
                CurBal = model.SenderCurBal,
                Fee = model.Fee,
                ReceivingCountry = model.ReceiverCountry,
                TransactionDate = model.TransactionDate,
                TransactionId = model.TransactionId,
                WalletStatmentStatus = WalletStatmentStatus.OutBound,
                WalletStatmentType = model.WalletStatmentType                
                
            };
            #endregion

            dbContext.KiiPayBusinessWalletStatement.Add(kiiPayBusinessWalletStatementOutBound);
            dbContext.SaveChanges();
        }
    }



}