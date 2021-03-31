using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Mobile.Services.MoneyFex
{
    public class MobileTransacationHistoryServices
    {
        DB.FAXEREntities dbContext = null;
        public MobileTransacationHistoryServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public List<MobileTransacationListvm> GetTransactionHistory(int SenderId, int pageIndex, int pageSize)
        {
            var data = new List<MobileTransacationListvm>();

            data.AddRange(GetBankDepositTransaction(SenderId));
            data.AddRange(GetMobileWalletTransaction(SenderId));
            //data.AddRange(GetBankDepositTransaction(SenderId));
            return data;

            //var result = (from c in data.GroupBy(x => x.TransactionDate)
            //              select new MobileTransacationHistoryViewModel()
            //              {
            //                  TransacationDate = c.FirstOrDefault().TransactionDate,
            //                  TransacationListvm = data.Where(x => x.TransactionDate == c.FirstOrDefault().TransactionDate).ToList()
            //              }).ToList();
            //return result.ToList();
        }

        public List<MobileTransacationListvm> GetBankDepositTransaction(int SenderId)
        {
            var result = (from c in dbContext.BankAccountDeposit.Where(x => x.SenderId == SenderId).ToList()
                          select new MobileTransacationListvm()
                          {
                              TransactionId = c.Id,
                              TransactionDate = c.TransactionDate.Date == DateTime.Now.Date ? "Today" : c.TransactionDate.ToString("dddd, dd MMMM "),
                              AccountNumber = c.ReceiverAccountNo,
                              //ExchangeRateAmount = c.ExchangeRate,
                              //Fee = c.Fee,
                              //ReceipentId = c.RecipientId,
                              PaymentMethod = FAXER.PORTAL.Common.Common.GetEnumDescription(c.SenderPaymentMode),
                              //ReceiptNumber = c.ReceiptNo,
                              ReceiverFirstName = c.ReceiverName == null ? " " : c.ReceiverName.Split(' ')[0],
                              //ReceiverName = c.ReceiverName,
                              //ReceivingAmount = c.ReceivingAmount,
                              //ReceivingCountryCode = c.ReceiverCountry,
                              //ReceivingCountryName = FAXER.PORTAL.Common.Common.GetCountryName(c.ReceiverCountry),
                              //ReceivingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(c.ReceiverCountry),
                              //SendingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(c.SendingCountry),
                              ReceivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(c.ReceiverCountry),
                              //ReceivingTelephoneno = c.ReceiverMobileNo,
                              //SendingAmount = c.SendingAmount,
                              SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(c.SendingCountry),
                              Service = DB.Service.BankAccount,
                              ServiceName = "Bank Account",
                              //BankStatus = c.Status,
                              StatusName = FAXER.PORTAL.Common.Common.GetEnumDescription(c.Status),
                              TotalAmount = c.TotalAmount

                          }).ToList();
            return result;

        }

        internal bool UpdateNotificationToSeen(int notificationId)
        {
            SNotification _notificationServices = new SNotification();
            var data = _notificationServices.GetAllNotification().Where(x => x.Id == notificationId).FirstOrDefault();
            data.IsSeen = true;
            _notificationServices.UpdateNotificationToSeen(data);
            return true;
        }

        public List<MobileNotificationViewModel> GetNotificationHistory(int senderId)
        {
            SNotification _notificationServices = new SNotification();

            var result = (from c in _notificationServices.GetAllNotification(senderId, DB.NotificationFor.Sender).ToList()
                          select new MobileNotificationViewModel()
                          {
                              Id = c.Id,
                              Message = c.Message,
                              Title = c.NotificationTitle,
                              CreationDate = c.CreationDate.ToString(),
                              CreatedTime = getTimePeroid(c.CreationDate)
                          }).ToList();

            return result;
        }
        private string getTimePeroid(DateTime tranDate)
        {
            const int SECOND = 1;
            const int MINUTE = 60 * SECOND;
            const int HOUR = 60 * MINUTE;
            const int DAY = 24 * HOUR;
            const int MONTH = 30 * DAY;

            var ts = new TimeSpan(DateTime.Now.Ticks - tranDate.Ticks);
            double delta = Math.Abs(ts.TotalSeconds);

            if (delta < 1 * MINUTE)
                return ts.Seconds == 1 ? "one second ago" : ts.Seconds + " seconds ago";

            if (delta < 2 * MINUTE)
                return "a minute ago";

            if (delta < 45 * MINUTE)
                return ts.Minutes + " minutes ago";

            if (delta < 90 * MINUTE)
                return "an hour ago";

            if (delta < 24 * HOUR)
                return Math.Abs(ts.Hours) + " hours ago";

            if (delta < 48 * HOUR)
                return "yesterday";

            if (delta < 30 * DAY)
                return Math.Abs(ts.Days) + " days ago";

            if (delta < 12 * MONTH)
            {
                int months = Convert.ToInt32(Math.Floor((double)ts.Days / 30));
                return months <= 1 ? "one month ago" : months + " months ago";
            }
            else
            {
                int years = Convert.ToInt32(Math.Floor((double)ts.Days / 365));
                return years <= 1 ? "one year ago" : years + " years ago";
            }
        }

        internal List<YearlyTransactionDataListvm> GetYearlyTransactionDataList(int senderId)
        {



            List<YearlyTransactionDataListvm> model = new List<YearlyTransactionDataListvm>();
            GetYearlyTransaction(model, senderId);
            return model;




        }

        private List<YearlyTransactionDataListvm> GetYearlyTransaction(List<YearlyTransactionDataListvm> model, int senderId)
        {
            model = new List<YearlyTransactionDataListvm>();
            //January
            var result = dbContext.MonthlySummary.Where(x => x.SenderId == senderId).ToList();
            if (result.Count > 0)
            {
                model = (from c in result
                         select new YearlyTransactionDataListvm()
                         {
                             LimitAmount = c.LimitAmount,
                             LimitTitle = FormatNumber(c.LimitAmount),
                             Month = c.Month,
                             SenderId = c.SenderId,
                             TransactionAmount = c.TransactionAmount



                         }).ToList();

                return model;

            }
            return model;

        }

        private string FormatNumber(decimal num)
        {
            if (num >= 100000)
                return FormatNumber(num / 1000) + "K";
            if (num >= 10000)
            {
                return (num / 1000M).ToString("0.#") + "K";
            }
            return num.ToString("#,0");
        }

        internal bool ChangeNotificationStatusForIsSeen(int notificationId)
        {
            Notification notification = dbContext.Notification.Where(x => x.Id == notificationId).FirstOrDefault();
            if (notification != null)
            {
                notification.IsSeen = true;
                dbContext.Entry(notification).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        //model = 
        //    return model;
        //static string FormatNumber(decimal num)
        //{

        //}
        public List<MobileTransacationListvm> GetMobileWalletTransaction(int SenderId)
        {
            var result = (from c in dbContext.MobileMoneyTransfer.Where(x => x.SenderId == SenderId).ToList()
                          select new MobileTransacationListvm()
                          {
                              TransactionId = c.Id,
                              TransactionDate = c.TransactionDate.Date == DateTime.Now.Date ? "Today" : c.TransactionDate.ToString("dddd, dd MMMM "),
                              AccountNumber = c.PaidToMobileNo,
                              //ExchangeRateAmount = c.ExchangeRate,
                              //Fee = c.Fee,
                              //ReceipentId = c.RecipientId,
                              PaymentMethod = FAXER.PORTAL.Common.Common.GetEnumDescription(c.SenderPaymentMode),
                              //ReceiptNumber = c.ReceiptNo,
                              ReceiverFirstName = c.ReceiverName == null ? " " : c.ReceiverName.Split(' ')[0],
                              ReceiverName = c.ReceiverName,
                              ReceivingCountryCode = c.ReceivingCountry,
                              //ReceivingAmount = c.ReceivingAmount,
                              //ReceivingCountryName = FAXER.PORTAL.Common.Common.GetCountryName(c.ReceivingCountry),
                              //ReceivingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(c.ReceivingCountry),
                              ReceivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                              //ReceivingTelephoneno = c.PaidToMobileNo,
                              SendingAmount = c.SendingAmount,
                              //SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(c.SendingCountry),
                              //SendingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(c.SendingCountry),
                              Service = DB.Service.BankAccount,
                              ServiceName = "Mobile",
                              MobileStatus = c.Status,
                              StatusName = FAXER.PORTAL.Common.Common.GetEnumDescription(c.Status),
                              TotalAmount = c.TotalAmount

                          }).ToList();
            return result;

        }
        public List<MobileTransacationListvm> GetCashPickUpTransaction(int SenderId)
        {
            var result = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.NonCardReciever.FaxerID == SenderId).ToList()
                          select new MobileTransacationListvm()
                          {
                              TransactionId = c.Id,
                              TransactionDate = c.TransactionDate.Date == DateTime.Now.Date ? "Today" : c.TransactionDate.ToString("dddd, dd MMMM "),
                              AccountNumber = "",
                              ExchangeRateAmount = c.ExchangeRate,
                              Fee = c.FaxingFee,
                              ReceipentId = c.RecipientId,
                              ReceivingCountryCode = c.NonCardReciever.Country,
                              PaymentMethod = FAXER.PORTAL.Common.Common.GetEnumDescription(c.SenderPaymentMode),
                              ReceiptNumber = c.MFCN,
                              ReceiverFirstName = c.NonCardReciever.FirstName,
                              ReceiverName = c.NonCardReciever.FirstName + " " + c.NonCardReciever.MiddleName + " " + c.NonCardReciever.LastName,
                              ReceivingAmount = c.ReceivingAmount,
                              ReceivingCountryName = FAXER.PORTAL.Common.Common.GetCountryName(c.NonCardReciever.Country),
                              ReceivingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(c.NonCardReciever.Country),
                              ReceivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(c.NonCardReciever.Country),
                              ReceivingTelephoneno = c.NonCardReciever.PhoneNumber,
                              SendingAmount = c.FaxingAmount,
                              SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(c.SendingCountry),
                              SendingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(c.SendingCountry),
                              Service = DB.Service.CashPickUP,
                              ServiceName = "Cash Pickup",
                              CashPickUpStatus = c.FaxingStatus,
                              StatusName = FAXER.PORTAL.Common.Common.GetEnumDescription(c.FaxingStatus),
                              TotalAmount = c.TotalAmount

                          }).ToList();
            return result;

        }


    }
}