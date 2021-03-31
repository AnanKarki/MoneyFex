using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SAutoTopUpForAll
    {
        DB.FAXEREntities dbContext = null;
        public SAutoTopUpForAll()
        {
            dbContext = new DB.FAXEREntities();

        }


        #region KiiPay Personal Standing Order Payments By Business


        public void KiiPayPersonalAutoPaymentByBusiness()
        {


            var autoPaymentInfo = dbContext.KiiPayBusinessKiiPayPersonalStandingOrderInfo.ToList();
            foreach (var autoPayment in autoPaymentInfo)
            {


                bool IsValidPayment = IsValidKiiPayPersonalAutoPaymentByBusiness(autoPayment);
                if (IsValidPayment)
                {
                    bool ValidDayOfAutoPayment = IsValidDayOfAutoPayment(autoPayment.PaymentFrequency, autoPayment.FrequencyDetials);
                    if (ValidDayOfAutoPayment)
                    {

                        // KB stands for KiiPayBusiness
                        KiiPayBusinessPayAnotherKiiPayWalletServices _kBPayAnotherKiiPayWalletServices = new KiiPayBusinessPayAnotherKiiPayWalletServices();


                        KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();

                        int SenderWalletId = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(autoPayment.SenderId).Id;

                        var HasEnoghBalToPay = _kiiPayBusinessCommonServices.DoesAccountHaveEnoughBal(SenderWalletId, autoPayment.Amount);

                        if (HasEnoghBalToPay)
                        {
                            bool IsLocalPayment = false;
                            var paymentSummary = GetKiiPayBusinessPaymentSummary(autoPayment.Amount, autoPayment.SenderCountry, autoPayment.ReceiverCountry, ref IsLocalPayment);

                            DB.KiiPayPersonalWalletPaymentByKiiPayBusiness model = new KiiPayPersonalWalletPaymentByKiiPayBusiness()
                            {
                                ExchangeRate = paymentSummary.ExchangeRate,
                                PayingAmount = paymentSummary.SendingAmount,
                                Fee = paymentSummary.Fee,
                                RecievingAmount = paymentSummary.ReceivingAmount,
                                TotalAmount = paymentSummary.TotalAmount,
                                PaymentReference = "Auto Payment",
                                ReceiptNumber = _kiiPayBusinessCommonServices.GetReceiptNoForKiiPayPersonalTransaction(),
                                KiiPayBusinessInformationId = autoPayment.SenderId,
                                KiiPayPersonalWalletInformationId = autoPayment.ReceiverId,
                                IsAutoPayment = true,
                                TransactionDate = DateTime.Now,
                                KiiPayBusinessWalletInformationId = SenderWalletId,
                                PaymentType = IsLocalPayment == true ? PaymentType.Local : PaymentType.International

                            };

                            _kBPayAnotherKiiPayWalletServices.SaveKiiPayPersonalInternationalByKiiPayBusinessPayment(model);


                            // Increase kiiPay Personal Wallet Balance  
                            Common.Common.KiiPayPersonalWalletBalanceIN(autoPayment.ReceiverId, paymentSummary.ReceivingAmount);

                            // Decrease KiiPay Business wallet Balance 
                            _kiiPayBusinessCommonServices.BalanceOut(SenderWalletId, paymentSummary.SendingAmount);
                        }
                    }
                }


            }
        }

        public bool IsValidKiiPayPersonalAutoPaymentByBusiness(KiiPayBusinessKiiPayPersonalStandingOrderInfo standingOrderInfo)
        {

            DateTime StartTransactionDate = GetStartTransactionDate(standingOrderInfo.PaymentFrequency);

            var result = dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.
                                  Where(x => x.KiiPayPersonalWalletInformationId == standingOrderInfo.ReceiverId
                                 && DbFunctions.TruncateTime(x.TransactionDate) >= DbFunctions.TruncateTime(StartTransactionDate)
                                                                       && x.IsAutoPayment == true).Count();

            if (result > 0)
            {
                return false;
            }

            return true;

        }

        #endregion


        #region KiiPay Business Standing Order Payments By Business


        public void KiiPayBusinessAutoPaymentByBusiness()
        {

            var autoPaymentInfo = dbContext.KiiPayBusinessBusinessStandingOrderInfo.ToList();
            foreach (var autoPayment in autoPaymentInfo)
            {


                bool IsValidPayment = IsValidKiiPayBusinessAutoPaymentByBusiness(autoPayment);
                if (IsValidPayment)
                {
                    bool ValidDayOfAutoPayment = IsValidDayOfAutoPayment(autoPayment.Frequency, autoPayment.FrequencyDetail);
                    if (ValidDayOfAutoPayment)
                    {

                        // KB stands for KiiPayBusiness
                        KiiPayBusinessPayForServicesServices _kBPayForServicesServices = new KiiPayBusinessPayForServicesServices();

                        KiiPayBusinessCommonServices _kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();

                        int SenderWalletId = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(autoPayment.SenderId).Id;
                        int ReceiverwalletId = _kiiPayBusinessCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(autoPayment.ReceiverId).Id;
                        var HasEnoghBalToPay = _kiiPayBusinessCommonServices.DoesAccountHaveEnoughBal(SenderWalletId, autoPayment.Amount);
                        if (HasEnoghBalToPay)
                        {
                            bool IsLocalPayment = false;
                            var paymentSummary = GetKiiPayBusinessPaymentSummary(autoPayment.Amount, autoPayment.SendingCountry, autoPayment.ReceivingCountry, ref IsLocalPayment);

                            if (IsLocalPayment)
                            {
                                DB.KiiPayBusinessLocalTransaction kiiPayBusinessLocalTransaction = new KiiPayBusinessLocalTransaction()
                                {
                                    AmountSent = autoPayment.Amount,
                                    IsAutoPayment = true,
                                    IsRefunded = false,
                                    PayedFromKiiPayBusinessInformationId = autoPayment.SenderId,
                                    PayedFromKiiPayBusinessWalletInformationId = SenderWalletId,
                                    PayedToKiiPayBusinessInformationId = autoPayment.ReceiverId,
                                    PayedToKiiPayBusinessWalletInformationId = ReceiverwalletId,
                                    PaymentReference = "",
                                    TransactionDate = DateTime.Now
                                };
                                _kBPayForServicesServices.SaveKiiPayBusinessLocalPaymentTransaction(kiiPayBusinessLocalTransaction);


                            }
                            else
                            {

                                DB.KiiPayBusinessInternationalPaymentTransaction kiiPayBusinessInternationalPaymentTransaction = new KiiPayBusinessInternationalPaymentTransaction()
                                {
                                    ExchangeRate = paymentSummary.ExchangeRate,
                                    FaxingAmount = paymentSummary.SendingAmount,
                                    RecievingAmount = paymentSummary.ReceivingAmount,
                                    FaxingFee = paymentSummary.Fee,
                                    TotalAmount = paymentSummary.TotalAmount,
                                    IsAutoPayment = true,
                                    IsRefunded = false,
                                    ReceiptNumber = _kiiPayBusinessCommonServices.GetReceiptNoForKiiPayBusinessInternationalTransaction(),
                                    PaymentReference = "",
                                    PayedFromKiiPayBusinessInformationId = autoPayment.SenderId,
                                    PayedFromKiiPayBusinessWalletId = SenderWalletId,
                                    PayedToKiiPayBusinessInformationId = autoPayment.ReceiverId,
                                    PayedToKiiPayBusinessWalletId = ReceiverwalletId,
                                    TransactionDate = DateTime.Now
                                };
                                _kBPayForServicesServices.SaveKiiPayBusinessInternationalPaymentTransaction(kiiPayBusinessInternationalPaymentTransaction);

                            }

                            // Increase Receiver KiiPay Business wallet Balance 
                            _kiiPayBusinessCommonServices.BalanceIn(ReceiverwalletId, paymentSummary.ReceivingAmount);

                            // Decrease Sender Kiipay Business wallet balance
                            _kiiPayBusinessCommonServices.BalanceOut(SenderWalletId, paymentSummary.SendingAmount);

                        }
                    }
                }


            }

        }
        public bool IsValidKiiPayBusinessAutoPaymentByBusiness(KiiPayBusinessBusinessStandingOrderInfo standingOrderInfo)
        {

            DateTime StartTransactionDate = GetStartTransactionDate(standingOrderInfo.Frequency);

            int resultCount = 0;
            if (standingOrderInfo.SendingCountry.ToLower() == standingOrderInfo.ReceivingCountry.ToLower())
            {

                resultCount = dbContext.KiiPayBusinessLocalTransaction.
                                  Where(x => x.PayedToKiiPayBusinessInformationId == standingOrderInfo.ReceiverId
                                 && DbFunctions.TruncateTime(x.TransactionDate) >= DbFunctions.TruncateTime(StartTransactionDate)
                                                                       && x.IsAutoPayment == true).Count();
            }
            else
            {

                resultCount = dbContext.KiiPayBusinessInternationalPaymentTransaction.
                                  Where(x => x.PayedToKiiPayBusinessInformationId == standingOrderInfo.ReceiverId
                                 && DbFunctions.TruncateTime(x.TransactionDate) >= DbFunctions.TruncateTime(StartTransactionDate)
                                                                       && x.IsAutoPayment == true).Count();
            }

            if (resultCount > 0)
            {
                return false;
            }

            return true;

        }




        #endregion 

        #region KiiPay Personal Standing Order Payments By Personal
        public void KiiPayPersonalAutoPaymentByPersonal()
        {
            var autoPaymentInfo = dbContext.KiiPayPersonalKiiPayPersonalStandingOrderInfo.ToList();
            foreach (var autoPayment in autoPaymentInfo)
            {


                bool IsValidPayment = IsValidKiiPayPersonalAutoPaymentByPersonal(autoPayment);
                if (IsValidPayment)
                {
                    bool ValidDayOfAutoPayment = IsValidDayOfAutoPayment(autoPayment.PaymentFrequency, autoPayment.FrequencyDetials);
                    if (ValidDayOfAutoPayment)
                    {

                        PayForGoodsAndServicesServices _PayForGoodsAndServicesServices = new PayForGoodsAndServicesServices();


                        KiiPayPersonalCommonServices _kiiPayPerosnalCommonServices = new KiiPayPersonalCommonServices();

                        int SenderWalletId = autoPayment.SenderId;

                        var HasEnoghBalToPay = _kiiPayPerosnalCommonServices.DoesAccountHaveEnoughBal(SenderWalletId, autoPayment.Amount);

                        if (HasEnoghBalToPay)
                        {
                            bool IsLocalPayment = false;
                            var paymentSummary = GetKiiPayBusinessPaymentSummary(autoPayment.Amount, autoPayment.SenderCountry, autoPayment.ReceiverCountry, ref IsLocalPayment);

                            DB.KiiPayPersonalWalletPaymentByKiiPayPersonal model = new KiiPayPersonalWalletPaymentByKiiPayPersonal()
                            {
                                ExchangeRate = paymentSummary.ExchangeRate,
                                RecievingAmount = paymentSummary.ReceivingAmount,
                                TotalAmount = paymentSummary.TotalAmount,
                                PaymentReference = "Auto Payment",
                                ReceiptNumber = _kiiPayPerosnalCommonServices.GetReceiptNoForKiiPayPersonalTransaction(),
                                IsAutoPayment = true,
                                TransactionDate = DateTime.Now,
                                FaxingAmount = paymentSummary.SendingAmount,
                                IsRefunded = false,
                                KiiPayPersonalRefundedTransactionId = 0,
                                ReceiverWalletId = autoPayment.ReceiverId,
                                ReceivingMobileNumber = autoPayment.ReceiverMobileNo,
                                SenderWalletId =autoPayment.SenderId,
                                FaxingFee=paymentSummary.Fee,
                                SenderId = autoPayment.SenderId,
                                PaymentType = IsLocalPayment == true ? PaymentType.Local : PaymentType.International

                            };

                            _PayForGoodsAndServicesServices.SaveKiiPayPersonalByKiiPayPersonalPayment(model);


                            // Increase kiiPay Personal Wallet Balance  
                            Common.Common.KiiPayPersonalWalletBalanceIN(autoPayment.ReceiverId, paymentSummary.ReceivingAmount);

                            // Decrease KiiPay Business wallet Balance 
                            Common.Common.KiiPayPersonalWalletBalanceOut(SenderWalletId, paymentSummary.SendingAmount);
                        }
                    }
                }


            }
        }

        public bool IsValidKiiPayPersonalAutoPaymentByPersonal(KiiPayPersonalKiiPayPersonalStandingOrderInfo standingOrderInfo)
        {

            DateTime StartTransactionDate = GetStartTransactionDate(standingOrderInfo.PaymentFrequency);

            var result = dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.
                                  Where(x => x.ReceiverWalletId == standingOrderInfo.ReceiverId
                                 && DbFunctions.TruncateTime(x.TransactionDate) >= DbFunctions.TruncateTime(StartTransactionDate)
                                                                       && x.IsAutoPayment == true).Count();

            if (result > 0)
            {
                return false;
            }

            return true;

        }

        #endregion

        #region KiiPay Business Standing Order Payments By Personal
        public void KiiPayBusinessAutoPaymentByPersonal()
        {

            var autoPaymentInfo = dbContext.KiiPayPersonalBusinessStandingOrderInfo.ToList();
            foreach (var autoPayment in autoPaymentInfo)
            {


                bool IsValidPayment = IsValidKiiPayBusinessAutoPaymentByPersonal(autoPayment);
                if (IsValidPayment)
                {
                    bool ValidDayOfAutoPayment = IsValidDayOfAutoPayment(autoPayment.Frequency, autoPayment.FrequencyDetail);
                    if (ValidDayOfAutoPayment)
                    {

                        PayForGoodsAndServicesServices _PayForGoodsAndServicesServices = new PayForGoodsAndServicesServices();


                        KiiPayPersonalCommonServices _kiiPayPerosnalCommonServices = new KiiPayPersonalCommonServices();

                        int SenderWalletId = autoPayment.SenderId;
                        int ReceiverwalletId = _kiiPayPerosnalCommonServices.GetKiipayBusinessWalletInfoByKiiPayBusinessId(autoPayment.ReceiverId).Id;
                        var HasEnoghBalToPay = _kiiPayPerosnalCommonServices.DoesAccountHaveEnoughBal(SenderWalletId, autoPayment.Amount);
                        if (HasEnoghBalToPay)
                        {
                            bool IsLocalPayment = false;
                            var paymentSummary = GetKiiPayBusinessPaymentSummary(autoPayment.Amount, autoPayment.SendingCountry, autoPayment.ReceivingCountry, ref IsLocalPayment);

                            if (IsLocalPayment)
                            {
                                DB.KiiPayPersonalNationalKiiPayBusinessPayment kiiPayPersonalNationalKiiPayBusinessPayment = new KiiPayPersonalNationalKiiPayBusinessPayment()
                                {
                                    AmountSent = autoPayment.Amount,
                                    IsAutoPayment = true,
                                    PaymentReference = "",
                                    KiiPayBusinessWalletInformationId = ReceiverwalletId,
                                    KiiPayPersonalWalletInformationId = autoPayment.SenderId,
                                    TransactionDate = DateTime.Now
                                };
                                _PayForGoodsAndServicesServices.SaveKiiPayPersonalNationalKiiPayBusinessPayment(kiiPayPersonalNationalKiiPayBusinessPayment);


                            }
                            else
                            {

                                DB.KiiPayPersonalInternationalKiiPayBusinessPayment kiiPayPersonalInternationalKiiPayBusinessPayment = new KiiPayPersonalInternationalKiiPayBusinessPayment()
                                {
                                    ExchangeRate = paymentSummary.ExchangeRate,
                                    FaxingAmount = paymentSummary.SendingAmount,
                                    FaxingFee = paymentSummary.Fee,
                                    TotalAmount = paymentSummary.TotalAmount,
                                    IsAutoPayment = true,
                                    ReceiptNumber = _kiiPayPerosnalCommonServices.GetReceiptNoForKiiPayPersonalInternationalTransaction(),
                                    PaymentReference = "",
                                    PayedToKiiPayBusinessInformationId = autoPayment.ReceiverId,
                                    PayedToKiiPayBusinessWalletId = ReceiverwalletId,
                                    PayedFromKiiPayPersonalWalletId = autoPayment.SenderId,
                                    ReceivingAmount = paymentSummary.ReceivingAmount,
                                    TransactionDate = DateTime.Now,
                                    
                                };
                                _PayForGoodsAndServicesServices.SaveKiiPayPersonalInternationalKiiPayBusinessPayment(kiiPayPersonalInternationalKiiPayBusinessPayment);

                            }

                            // Increase Receiver KiiPay Business wallet Balance 
                            _kiiPayPerosnalCommonServices.BalanceIn(ReceiverwalletId, paymentSummary.ReceivingAmount);

                            // Decrease Sender Kiipay Business wallet balance
                            _kiiPayPerosnalCommonServices.BalanceOut(SenderWalletId, paymentSummary.SendingAmount);

                        }
                    }
                }


            }

        }
        public bool IsValidKiiPayBusinessAutoPaymentByPersonal(KiiPayPersonalBusinessStandingOrderInfo standingOrderInfo)
        {

            DateTime StartTransactionDate = GetStartTransactionDate(standingOrderInfo.Frequency);

            int resultCount = 0;
            if (standingOrderInfo.SendingCountry.ToLower() == standingOrderInfo.ReceivingCountry.ToLower())
            {

                resultCount = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.
                                  Where(x => x.KiiPayBusinessWalletInformationId == standingOrderInfo.ReceiverId
                                 && DbFunctions.TruncateTime(x.TransactionDate) >= DbFunctions.TruncateTime(StartTransactionDate)
                                                                       && x.IsAutoPayment == true).Count();
            }
            else
            {

                resultCount = dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.
                                  Where(x => x.PayedToKiiPayBusinessInformationId == standingOrderInfo.ReceiverId
                                 && DbFunctions.TruncateTime(x.TransactionDate) >= DbFunctions.TruncateTime(StartTransactionDate)
                                                                       && x.IsAutoPayment == true).Count();
            }

            if (resultCount > 0)
            {
                return false;
            }

            return true;

        }
        #endregion





        public bool IsValidDayOfAutoPayment(AutoPaymentFrequency paymentFrequency, string frequencyDetails)
        {


            var currentDate = DateTime.Now.Date;

            if (paymentFrequency == DB.AutoPaymentFrequency.Weekly)
            {

                // Find the day of the week for  which the day value has been stored as a frequency detials 
                // i.e 0 = Sunday and 6 = Saturday  // Single Digit is stored for week

                var day = (DayOfWeek)int.Parse(frequencyDetails);
                if (day == currentDate.DayOfWeek)
                {

                    return true;
                }
                else
                {

                    return false;
                }

            }

            if (paymentFrequency == DB.AutoPaymentFrequency.Monthly)
            {
                // Find the day of the week for  which the day value has been stored as a frequency detials 
                // i.e 01 = firstday of the month //Double Digit is stored for month
                var day = int.Parse(frequencyDetails);

                if (day == currentDate.Day)
                {

                    return true;
                }
                else
                {

                    return false;
                }
            }
            if (paymentFrequency == DB.AutoPaymentFrequency.Yearly)
            {
                // Find the day of the week for  which the day value has been stored as a frequency detials 
                // i.e 0102  : 01= January , 02 = Second Day //Four Digit is stored for year
                int Day = 0;
                int Month = 0;
                //First Two digit is of Month
                Month = int.Parse(frequencyDetails.Substring(0, 2));
                // Last Two digit is of Day
                Day = int.Parse(frequencyDetails.Substring(2, 2));

                if (currentDate.Month == Month && currentDate.Day == Day)
                {

                    return true;
                }
                else
                {

                    return false;
                }

            }
            return false;
        }


        /// <summary>
        /// Get the transaction start date to check whether day is valid to top up or not 
        /// </summary>
        /// <param name="autoPaymentFrequency"></param>
        /// <returns></returns>
        public DateTime GetStartTransactionDate(AutoPaymentFrequency autoPaymentFrequency)
        {

            DateTime currentDate = DateTime.Now.Date;
            DateTime StartedTransactionDate = new System.DateTime();

            var Day = 0;
            var gannuParneDays = 0.0;
            if (gannuParneDays == 0 && autoPaymentFrequency != DB.AutoPaymentFrequency.NoLimitSet)
            {
                switch (autoPaymentFrequency)
                {
                    case DB.AutoPaymentFrequency.Weekly:
                        Day = (int)currentDate.DayOfWeek;//2
                                                         //hamile monday lai firstday manchau
                                                         //day starts =1=monday
                                                         //enum starts=0=sunday;
                        gannuParneDays = Day;
                        if (Day == 0)
                        {
                            gannuParneDays = 7;
                        }


                        break;

                    case DB.AutoPaymentFrequency.Monthly:
                        Day = currentDate.Day;

                        gannuParneDays = Day;

                        break;
                    case DB.AutoPaymentFrequency.Yearly:
                        Day = currentDate.DayOfYear;
                        gannuParneDays = Day;

                        break;
                    default:
                        break;
                }
            }
            StartedTransactionDate = currentDate.AddDays(-(gannuParneDays)).Date;
            return StartedTransactionDate;
        }




        public KiiPayBusinessPaymentSummaryVM GetKiiPayBusinessPaymentSummary(decimal SendingAmount, string SendingCountry, string ReceivingCountry, ref bool IsLocalPayment)
        {


            KiiPayBusinessPaymentSummaryVM paymentSummary = new KiiPayBusinessPaymentSummaryVM();
            if (SendingCountry.ToLower() == ReceivingCountry.ToLower())
            {
                IsLocalPayment = true;
                paymentSummary.ExchangeRate = 1;
                paymentSummary.Fee = 0;
                paymentSummary.SendingAmount = SendingAmount;
                paymentSummary.ReceivingAmount = SendingAmount;
                paymentSummary.TotalAmount = SendingAmount;
                paymentSummary.SmsFee = 0;
                paymentSummary.PaymentReference = "";
            }
            else
            {

                var ExchangeRate = SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry);
                var result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, false,
                ExchangeRate, SEstimateFee.GetFaxingCommision(SendingCountry));
                // Rewrite session with additional value 
                paymentSummary.Fee = result.FaxingFee;
                paymentSummary.SendingAmount = result.FaxingAmount;
                paymentSummary.ReceivingAmount = result.ReceivingAmount;
                paymentSummary.TotalAmount = result.TotalAmount;
                paymentSummary.ExchangeRate = ExchangeRate;
                paymentSummary.SmsFee = 0;
                paymentSummary.PaymentReference = "";
                IsLocalPayment = false;
            }

            return paymentSummary;

        }
    }


}