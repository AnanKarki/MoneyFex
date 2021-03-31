using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.BankApi;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using Microsoft.Owin.BuilderProperties;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class MoblieNotificationManager : IMobileNotificationManager
    {
        FAXEREntities dbContext = null;
        public MoblieNotificationManager()
        {
            dbContext = new FAXEREntities();
        }

        public IQueryable<MobileNotification> MobileNotifications()
        {
            return dbContext.MobileNotification;
        }
        public MobileNotification MobileNotificationById(int id)
        {
            return MobileNotifications().SingleOrDefault(x => x.Id == id);
        }
        public void AddMobileNotification(MobileNotification mobileNotification)
        {
            dbContext.MobileNotification.Add(mobileNotification);
            dbContext.SaveChanges();
        }
        public void UpdateMobileNotification(MobileNotification mobileNotification)
        {
            dbContext.Entry(mobileNotification).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }
        public void DeleteMobileNotification(MobileNotification mobileNotification)
        {
            dbContext.MobileNotification.Remove(mobileNotification);
            dbContext.SaveChanges();
        }
        public List<MobileNotifiactionViewModel> GetAllMobileNotificationList(string sendingCurrency = "", string receivingCurrecy = "", int notificationType = 0, string date = "")
        {
            var mobileNotifications = MobileNotifications();
            if (!string.IsNullOrEmpty(sendingCurrency))
            {
                mobileNotifications = mobileNotifications.Where(x => x.SendingCurrency == sendingCurrency);
            }
            if (!string.IsNullOrEmpty(receivingCurrecy))
            {
                mobileNotifications = mobileNotifications.Where(x => x.ReceivingCurrency == receivingCurrecy);
            }
            if (notificationType > 0)
            {
                mobileNotifications = mobileNotifications.Where(x => x.NotificationType == (NotificationType)notificationType);
            }
            if (!string.IsNullOrEmpty(date))
            {
                var Date = date.ToDateTime();
                mobileNotifications = mobileNotifications.Where(x => x.CreatedDate == Date);
            }
            return (from c in mobileNotifications
                    join sendingCountry in dbContext.Country on c.SendingCountry equals sendingCountry.CountryCode into sc
                    from sendingCountry in sc.DefaultIfEmpty()
                    join receivingCountry in dbContext.Country on c.ReceivingCountry equals receivingCountry.CountryCode into rc
                    from receivingCountry in rc.DefaultIfEmpty()
                    join senderInfo in dbContext.FaxerInformation on c.SenderId equals senderInfo.Id into si
                    from senderInfo in si.DefaultIfEmpty()

                    select new MobileNotifiactionViewModel()
                    {
                        Id = c.Id,
                        CreatedBy = c.CreatedBy,
                        CreatedDate = c.CreatedDate,
                        FullNotification = c.FullNotification,
                        NotificationHeading = c.NotificationHeading,
                        NotificationTypeName = c.NotificationType.ToString(),
                        NotificationType = c.NotificationType,
                        ReceivingCountry = c.ReceivingCountry,
                        ReceivingCountryName = receivingCountry == null ? "All" : receivingCountry.CountryName,
                        ReceivingCurrency = c.ReceivingCurrency,
                        SenderId = c.SenderId,
                        SenderFullName = senderInfo == null ? "All" : senderInfo.FirstName + " " + (string.IsNullOrEmpty(senderInfo.MiddleName) == true ? "" : senderInfo.MiddleName + " ") + senderInfo.LastName,
                        SendingCountry = c.SendingCountry,
                        SendingCountryName = sendingCountry == null ? "All" : sendingCountry.CountryName,
                        SendingCurrency = c.SendingCurrency,
                        SendingNotificationMethod = c.SendingNotificationMethod,
                        SendingNotificationMethodName = c.SendingNotificationMethod.ToString(),
                        SenderAccount = senderInfo == null ? "" : senderInfo.AccountNo
                    }).OrderByDescending(x => x.Id).ToList();
        }
        public void SendNotificationToSender(MobileNotification mobileNotification)
        {
            switch (mobileNotification.SendingNotificationMethod)
            {
                case SendingNotificationMethod.MobileNotification:
                    SendNotificationThroughMobileNotification(mobileNotification);
                    break;
                case SendingNotificationMethod.SMS:
                    SendNotificationThroughSms(mobileNotification);
                    break;
                case SendingNotificationMethod.WhatsApp:
                    SendNotificationThroughWhastApp(mobileNotification);
                    break;
            }
        }
        private void SendNotificationThroughMobileNotification(MobileNotification mobileNotification)
        {
            MobileNotificationApi mobileNotificationApi = new MobileNotificationApi();
            List<int> senderIds = new List<int>();
            switch (mobileNotification.NotificationType)
            {
                case NotificationType.General:
                    senderIds = GetSenderIdsFromRegisteredSender(mobileNotification);
                    break;
                case NotificationType.RateAlert:
                    senderIds = GetSenderIdsFromRecentTransaction(mobileNotification);
                    break;
            }
            List<string> ClientToken = GetTokenFromSenderIds(senderIds);
            var result = mobileNotificationApi.SendNotification(ClientToken, mobileNotification.NotificationHeading, mobileNotification.FullNotification).Result;
            if (result)
            {
                SaveLogForSendingNotification(mobileNotification, senderIds);
            }
        }

        private void SaveLogForSendingNotification(MobileNotification mobileNotification, List<int> senderIds)
        {
            CommonServices _commonService = new CommonServices();
            foreach (var item in senderIds)
            {
                Notification notification = new Notification()
                {
                    CreationDate = DateTime.Now,
                    Message = mobileNotification.FullNotification,
                    NotificationTitle = mobileNotification.NotificationHeading,
                    SenderId = StaffSession.LoggedStaff.StaffId,
                    ReceiverId = item,NotificationType = mobileNotification.NotificationType,
                    NotificationReceiver = NotificationFor.Sender,
                    NotificationSender = NotificationFor.Staff,
                    Name = _commonService.GetSenderName(item)
                };
                AddNotifactionLog(notification);
            }
        }

        private void AddNotifactionLog(Notification notification)
        {
            dbContext.Notification.Add(notification);
            dbContext.SaveChanges();
        }

        private void SendNotificationThroughWhastApp(MobileNotification mobileNotification)
        {
            WhatsAppApi whatsAppApi = new WhatsAppApi();
            string message = string.Format("{0}{1}",
                            mobileNotification.NotificationHeading + "\n",
                             mobileNotification.FullNotification + "\n");

            switch (mobileNotification.NotificationType)
            {
                case NotificationType.General:
                    if (mobileNotification.SenderId == 0)
                    {
                        var senderPhonenoWithCountyPhoneCode = new List<NotifyUserSMSVm>();
                        if (mobileNotification.SendingCountry.ToLower() == "all")
                        {
                            var sendingCountries = dbContext.Country.Where(x => x.Currency == mobileNotification.SendingCurrency);
                            senderPhonenoWithCountyPhoneCode = (from c in dbContext.FaxerInformation
                                                                join d in sendingCountries on c.Country equals d.CountryCode
                                                                select new NotifyUserSMSVm()
                                                                {
                                                                    Address = "whatsapp:" + d.CountryPhoneCode + c.PhoneNumber
                                                                }).ToList();
                        }
                        else
                        {
                            senderPhonenoWithCountyPhoneCode = (from c in dbContext.FaxerInformation.Where(x => x.Country == mobileNotification.SendingCountry)
                                                                join d in dbContext.Country on c.Country equals d.CountryCode
                                                                select new NotifyUserSMSVm()
                                                                {
                                                                    Address = "whatsapp:" + d.CountryPhoneCode + c.PhoneNumber
                                                                }).ToList();
                        }
                    }
                    else
                    {
                        var senderInfo = dbContext.FaxerInformation.Where(x => x.Id == mobileNotification.SenderId).FirstOrDefault();
                        whatsAppApi.SendWhatsAppSMS("whatsapp:" + Common.Common.GetCountryPhoneCode(senderInfo.Country) + senderInfo.PhoneNumber, message);
                    }
                    break;
                case NotificationType.RateAlert:
                    var senderIds = GetSenderIdsFromRecentTransaction(mobileNotification);
                    foreach (var senderId in senderIds)
                    {
                        var senderInfo = dbContext.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();
                        if (senderInfo != null)
                        {
                            whatsAppApi.SendWhatsAppSMS("whatsapp:" + Common.Common.GetCountryPhoneCode(senderInfo.Country) + senderInfo.PhoneNumber, message);
                        }
                    }
                    break;
            }

        }
        private void SendNotificationThroughSms(MobileNotification mobileNotification)
        {
            SmsApi smsApi = new SmsApi();
            string message = string.Format("{0}{1}",
                                     mobileNotification.NotificationHeading + "\n",
                                      mobileNotification.FullNotification + "\n");
            List<NotifyUserSMSVm> NotifyUserSMSVms = new List<NotifyUserSMSVm>();
            List<string> UserPhoneNo = new List<string>();
            if (mobileNotification.SenderId == 0)
            {
                switch (mobileNotification.NotificationType)
                {
                    case NotificationType.General:
                        NotifyUserSMSVms = GetSendersInfoFromRegisteredSenders(mobileNotification);
                        UserPhoneNo = smsApi.PrepareSmsGroup(NotifyUserSMSVms, Sms_binding_type.sms);
                        break;
                    case NotificationType.RateAlert:
                        NotifyUserSMSVms = GetSenderInfoFromRecentTransactions(mobileNotification);
                        UserPhoneNo = smsApi.PrepareSmsGroup(NotifyUserSMSVms, Sms_binding_type.sms);
                        break;
                }
                smsApi.SendBulkSMS(UserPhoneNo, message);
            }
            else
            {
                var senderInfo = dbContext.FaxerInformation.Where(x => x.Id == mobileNotification.SenderId).FirstOrDefault();
                smsApi.SendSMS(Common.Common.GetCountryPhoneCode(senderInfo.Country) + senderInfo.PhoneNumber, message);
            }
        }
        private List<int> GetSenderIdsFromRecentTransaction(MobileNotification mobileNotification)
        {
            List<int> senderIds = new List<int>();
            var alltransactionForMobileNotification = GetTransactionDetailByTimeFrequency(mobileNotification.CustomerTpyeAccToTime);
            var bankDeposits = alltransactionForMobileNotification.BankAccountDeposits;
            var cashPickUps = alltransactionForMobileNotification.CashPickUpTransfers;
            var otherWallets = alltransactionForMobileNotification.MobileMoneyTransfers;
            if (mobileNotification.SendingCountry.ToLower() == "all" || mobileNotification.ReceivingCountry.ToLower() == "all")
            {
                var sendingCountries = dbContext.Country.
                                        Where(x => x.Currency == mobileNotification.SendingCurrency).
                                        ToList();

                var receivingCountries = dbContext.Country.
                                         Where(x => x.Currency == mobileNotification.ReceivingCurrency).
                                         ToList();

                var bankDepositSenders = (from c in bankDeposits
                                          join d in sendingCountries on c.SendingCountry equals d.CountryCode
                                          join e in receivingCountries on c.ReceivingCountry equals e.CountryCode

                                          select c)
                                         .GroupBy(x => x.SenderId).
                                         Select(x => x.FirstOrDefault().SenderId).
                                         ToList();
                var cashPickUpSenders = (from c in cashPickUps
                                         join d in sendingCountries on c.SendingCountry equals d.CountryCode
                                         join e in receivingCountries on c.ReceivingCountry equals e.CountryCode
                                         select c)
                                         .GroupBy(x => x.SenderId).
                                         Select(x => x.FirstOrDefault().SenderId).
                                         ToList();

                var otherWalletSenders = (from c in otherWallets
                                          join d in sendingCountries on c.SendingCountry equals d.CountryCode
                                          join e in receivingCountries on c.ReceivingCountry equals e.CountryCode
                                          select c)
                                         .GroupBy(x => x.SenderId).
                                         Select(x => x.FirstOrDefault().SenderId).
                                         ToList();
                var senders = bankDepositSenders.Concat(cashPickUpSenders).Concat(otherWalletSenders).Distinct().ToList();
                senderIds.AddRange(senders);
                senderIds = senderIds.Distinct().ToList();
            }
            else
            {
                var bankDepositSenders = bankDeposits.
                                        Where(x => x.SendingCountry == mobileNotification.SendingCountry
                                                    && x.ReceivingCountry == mobileNotification.ReceivingCountry)
                                                    .GroupBy(x => x.SenderId).Select(x => x.Key);
                var cashPickUpSenders = cashPickUps.
                                         Where(x => x.SendingCountry == mobileNotification.SendingCountry
                                                      && x.ReceivingCountry == mobileNotification.ReceivingCountry).
                                                      GroupBy(x => x.SenderId).Select(x => x.Key);
                var otherWalletSenders = otherWallets.
                                        Where(x => x.SendingCountry == mobileNotification.SendingCountry
                                                    && x.ReceivingCountry == mobileNotification.ReceivingCountry).
                                                    GroupBy(x => x.SenderId).Select(x => x.Key);
                var senders = bankDepositSenders.Concat(cashPickUpSenders).Concat(otherWalletSenders).Distinct().ToList();
            }
            return senderIds;
        }
        private List<NotifyUserSMSVm> GetSenderInfoFromRecentTransactions(MobileNotification mobileNotification)
        {
            List<NotifyUserSMSVm> users = new List<NotifyUserSMSVm>();
            var alltransactionForMobileNotification = GetTransactionDetailByTimeFrequency(mobileNotification.CustomerTpyeAccToTime);
            var senderInfo = dbContext.FaxerInformation;
            var bankDeposits = alltransactionForMobileNotification.BankAccountDeposits;
            var cashPickUps = alltransactionForMobileNotification.CashPickUpTransfers;
            var otherWallets = alltransactionForMobileNotification.MobileMoneyTransfers;
            if (mobileNotification.SendingCountry.ToLower() == "all" || mobileNotification.ReceivingCountry.ToLower() == "all")
            {
                var sendingCountries = dbContext.Country.
                                        Where(x => x.Currency == mobileNotification.SendingCurrency).
                                        ToList();

                var receivingCountries = dbContext.Country.
                                         Where(x => x.Currency == mobileNotification.ReceivingCurrency).
                                         ToList();


                //var bankDepositSenders = (from c in bankDeposits
                //                          join d in sendingCountries on c.SendingCountry equals d.CountryCode
                //                          join e in receivingCountries on c.ReceivingCountry equals e.CountryCode
                //                          join sender in senderInfo on c.SenderId equals sender.Id
                //                          select sender)
                //                          .GroupBy(x => x.Id).Select(cs => new NotifyUserSMSVm()
                //                          {
                //                              Address = Common.Common.GetCountryPhoneCode(cs.FirstOrDefault().Country) + cs.FirstOrDefault().PhoneNumber
                //                          }).ToList();
                //var cashPickUpSenders = (from c in cashPickUps
                //                         join d in sendingCountries on c.SendingCountry equals d.CountryCode
                //                         join e in receivingCountries on c.ReceivingCountry equals e.CountryCode
                //                         join sender in senderInfo on c.SenderId equals sender.Id
                //                         select sender)
                //                          .GroupBy(x => x.Id).Select(cs => new NotifyUserSMSVm()
                //                          {
                //                              Address = Common.Common.GetCountryPhoneCode(cs.FirstOrDefault().Country) + cs.FirstOrDefault().PhoneNumber
                //                          }).ToList();
                //var otherWalletSenders = (from c in otherWallets
                //                          join d in sendingCountries on c.SendingCountry equals d.CountryCode
                //                          join e in receivingCountries on c.ReceivingCountry equals e.CountryCode
                //                          join sender in senderInfo on c.SenderId equals sender.Id
                //                          select sender)
                //                          .GroupBy(x => x.Id).Select(cs => new NotifyUserSMSVm()
                //                          {
                //                              Address = Common.Common.GetCountryPhoneCode(cs.FirstOrDefault().Country) + cs.FirstOrDefault().PhoneNumber
                //                          }).ToList();
                var bankDepositSenders = (from c in bankDeposits
                                          join d in sendingCountries on c.SendingCountry equals d.CountryCode
                                          join e in receivingCountries on c.ReceivingCountry equals e.CountryCode
                                          join sender in senderInfo on c.SenderId equals sender.Id
                                          select new NotifyUserSMSVm()
                                          {
                                              Address = d.CountryPhoneCode + sender.PhoneNumber,
                                          }).ToList();
                var cashPickUpSenders = (from c in cashPickUps
                                         join d in sendingCountries on c.SendingCountry equals d.CountryCode
                                         join e in receivingCountries on c.ReceivingCountry equals e.CountryCode
                                         join sender in senderInfo on c.SenderId equals sender.Id
                                         select new NotifyUserSMSVm()
                                         {
                                             Address = d.CountryPhoneCode + sender.PhoneNumber,
                                         }).ToList();

                var otherWalletSenders = (from c in otherWallets
                                          join d in sendingCountries on c.SendingCountry equals d.CountryCode
                                          join e in receivingCountries on c.ReceivingCountry equals e.CountryCode
                                          join sender in senderInfo on c.SenderId equals sender.Id
                                          select new NotifyUserSMSVm()
                                          {
                                              Address = d.CountryPhoneCode + sender.PhoneNumber,
                                          }).ToList();

                users = bankDepositSenders.Concat(cashPickUpSenders).Concat(otherWalletSenders).ToList();
                users = (from c in users.GroupBy(x => x.Address)
                         select new NotifyUserSMSVm()
                         {
                             Address = c.Key
                         }).ToList();

            }
            else
            {
                var bankDepositSenders = (from c in bankDeposits.
                                        Where(x => x.SendingCountry == mobileNotification.SendingCountry.Trim()
                                        && x.ReceivingCountry == mobileNotification.ReceivingCountry.Trim())
                                          join d in dbContext.Country on c.SendingCountry equals d.CountryCode
                                          join sender in senderInfo on c.SenderId equals sender.Id
                                          select new NotifyUserSMSVm()
                                          {
                                              Address = d.CountryPhoneCode + sender.PhoneNumber,
                                          }).ToList();
                var cashPickUpSenders = (from c in cashPickUps.
                                         Where(x => x.SendingCountry == mobileNotification.SendingCountry.Trim()
                                                      && x.ReceivingCountry == mobileNotification.ReceivingCountry.Trim())
                                         join d in dbContext.Country on c.SendingCountry equals d.CountryCode
                                         join sender in senderInfo on c.SenderId equals sender.Id
                                         select new NotifyUserSMSVm()
                                         {
                                             Address = d.CountryPhoneCode + sender.PhoneNumber,
                                         }).ToList();
                var otherWalletSenders = (from c in otherWallets.
                                        Where(x => x.SendingCountry == mobileNotification.SendingCountry.Trim()
                                         && x.ReceivingCountry == mobileNotification.ReceivingCountry.Trim())
                                          join d in dbContext.Country on c.SendingCountry equals d.CountryCode
                                          join sender in senderInfo on c.SenderId equals sender.Id
                                          select new NotifyUserSMSVm()
                                          {
                                              Address = d.CountryPhoneCode + sender.PhoneNumber,
                                          }).ToList();
                users = bankDepositSenders.Concat(cashPickUpSenders).Concat(otherWalletSenders).ToList();
                users = (from c in users.GroupBy(x => x.Address)
                         select new NotifyUserSMSVm()
                         {
                             Address = c.Key
                         }).ToList();
            }
            return users;
        }
        private TransactionListForMobileNotificationVm GetTransactionDetailByTimeFrequency(CustomerTpyeAccToTime frequency)
        {
            var bankDeposit = dbContext.BankAccountDeposit.ToList();
            var cashPickUp = dbContext.FaxingNonCardTransaction.ToList();
            var otherWallet = dbContext.MobileMoneyTransfer.ToList();
            DateTime date = DateTime.Now.Date;
            DateTime currentDate = DateTime.Now;
            switch (frequency)
            {
                case CustomerTpyeAccToTime.Week:
                    date = date.AddDays(-7);
                    break;
                case CustomerTpyeAccToTime.TwoWeeks:
                    date = date.AddDays(-14);
                    break;
                case CustomerTpyeAccToTime.ThreeWeeks:
                    date = date.AddDays(-21);
                    break;
                case CustomerTpyeAccToTime.TwoMonths:
                    date = date.AddMonths(-2);
                    break;
                case CustomerTpyeAccToTime.ThreeMonths:
                    date = date.AddMonths(-3);
                    break;
                case CustomerTpyeAccToTime.SixMonths:
                    date = date.AddMonths(-6);
                    break;
                case CustomerTpyeAccToTime.Year:
                    date = date.AddYears(-1);
                    break;
                case CustomerTpyeAccToTime.TwoYears:
                    date = date.AddYears(-2);
                    break;
            }

            if (date != DateTime.Now.Date)
            {
                bankDeposit = bankDeposit.Where(x => x.TransactionDate >= date && x.TransactionDate <= currentDate).ToList();
                cashPickUp = cashPickUp.Where(x => x.TransactionDate >= date && x.TransactionDate <= currentDate).ToList();
                otherWallet = otherWallet.Where(x => x.TransactionDate >= date && x.TransactionDate <= currentDate).ToList();
            }
            return new TransactionListForMobileNotificationVm()
            {
                BankAccountDeposits = bankDeposit,
                MobileMoneyTransfers = otherWallet,
                CashPickUpTransfers = cashPickUp
            };
        }
        private List<string> GetTokenFromSenderIds(List<int> senderIds)
        {
            List<string> ClientToken = new List<string>();
            foreach (var senderId in senderIds)
            {
                var senderToken = dbContext.FaxerInformation.Where(x => x.Id == senderId).Select(x => x.ClientToken).FirstOrDefault();
                if (senderToken != null)
                {
                    ClientToken.Add(senderToken);
                }
            }
            return ClientToken;
        }
        private List<int> GetSenderIdsFromRegisteredSender(MobileNotification mobileNotification)
        {
            List<int> senderIds = new List<int>();
            if (mobileNotification.SenderId == 0)
            {
                if (mobileNotification.SendingCountry.ToLower() == "all")
                {
                    var sendingCountries = dbContext.Country.Where(x => x.Currency.ToLower() == mobileNotification.SendingCurrency.ToLower());
                    senderIds = (from c in dbContext.FaxerInformation
                                  join country in sendingCountries on c.Country equals country.CountryCode
                                  select c).Select(x => x.Id).ToList();

                }
                else
                {
                    senderIds = dbContext.FaxerInformation.Where(x => x.Country == mobileNotification.SendingCountry).Select(x => x.Id).ToList();
                }
            }
            else
            {
                senderIds.Add(mobileNotification.SenderId);
            }
            return senderIds;
        }
        private List<NotifyUserSMSVm> GetSendersInfoFromRegisteredSenders(MobileNotification mobileNotification)
        {
            var senderPhonenoWithCountyPhoneCode = new List<NotifyUserSMSVm>();
            if (mobileNotification.SendingCountry.ToLower() == "all")
            {
                var sendingCountries = dbContext.Country.Where(x => x.Currency == mobileNotification.SendingCurrency.Trim());
                senderPhonenoWithCountyPhoneCode = (from c in dbContext.FaxerInformation
                                                    join d in sendingCountries on c.Country equals d.CountryCode
                                                    select new NotifyUserSMSVm()
                                                    {
                                                        Address = d.CountryPhoneCode + c.PhoneNumber
                                                    }).ToList();
            }
            else
            {
                senderPhonenoWithCountyPhoneCode = (from c in dbContext.FaxerInformation.Where(x => x.Country == mobileNotification.SendingCountry.Trim())
                                                    join d in dbContext.Country on c.Country equals d.CountryCode
                                                    select new NotifyUserSMSVm()
                                                    {
                                                        Address = d.CountryPhoneCode + c.PhoneNumber
                                                    }).ToList();
            }
            return senderPhonenoWithCountyPhoneCode;
        }
    }
}