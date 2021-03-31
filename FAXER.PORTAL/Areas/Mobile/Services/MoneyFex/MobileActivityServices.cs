using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Areas.Mobile.Services.MoneyFex
{
    public class MobileActivityServices
    {
        FAXEREntities dbContext = null;
        public MobileActivityServices()
        {
            dbContext = new FAXEREntities();
        }

        public ServiceResult<List<MobileActivityListvm>> GetActivityList(int senderId)
        {
            List<MobileActivityListvm> vm = new List<MobileActivityListvm>();
            var countries = dbContext.Country.ToList();
            if (senderId > 0)
            {
                var bankDeposit = (from c in dbContext.BankAccountDeposit.Where(x => x.SenderId == senderId
                                   && x.Status != BankDepositStatus.Cancel).ToList()
                                       //--join sendingCountry in dbContext.Country on c.SendingCountry equals sendingCountry.CountryCode
                                       //--join receivingCountry in dbContext.Country on c.ReceivingCountry equals receivingCountry.CountryCode
                                   select new MobileActivityViewModel()
                                   {
                                       TransactionId = c.Id,
                                       ReceiverName = c.ReceiverName,
                                       ReceivingCountryCode = c.ReceivingCountry,
                                       ReceivingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),
                                       ReecivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),
                                       SendingAmount = c.SendingAmount,
                                       ReceivingAmount = c.ReceivingAmount,
                                       TransferMethodName = "Bank account",
                                       //AccountNo = string.IsNullOrEmpty(c.ReceiverAccountNo) ? " " : "**" + c.ReceiverAccountNo.Substring((Math.Max(0, c.ReceiverAccountNo.Length - 3))),
                                       AccountNo = string.IsNullOrEmpty(c.ReceiverAccountNo) ? " " : c.ReceiverAccountNo,
                                       StatusDescription = FAXER.PORTAL.Common.Common.GetEnumDescription(c.Status),
                                       Status = c.Status.ToString(),
                                       TransferMethod = TransactionTransferMethod.BankDeposit,
                                       TransactionDate = c.TransactionDate.Date,
                                       TransactionDateTime = c.TransactionDate,
                                       ReceiptNo = c.ReceiptNo
                                   });
                var cashPickUp = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.SenderId == senderId
                                  && x.FaxingStatus != FaxingStatus.Cancel).ToList()
                                      //join sendingCountry in dbContext.Country on c.SendingCountry equals sendingCountry.CountryCode
                                      //join receivingCountry in dbContext.Country on c.ReceivingCountry equals receivingCountry.CountryCode
                                  join d in dbContext.Recipients on c.RecipientId equals d.Id
                                  select new MobileActivityViewModel()
                                  {
                                      TransactionId = c.Id,
                                      ReceiverName = d.ReceiverName,
                                      ReceivingCountryCode = c.ReceivingCountry,
                                      ReceivingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),
                                      ReecivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),
                                      SendingAmount = c.FaxingAmount,
                                      ReceivingAmount = c.ReceivingAmount,
                                      TransferMethodName = "Cash Pickup",
                                      //AccountNo = string.IsNullOrEmpty(c.NonCardReciever.PhoneNumber) ? "" : "**" + c.NonCardReciever.PhoneNumber.Substring((Math.Max(0, c.NonCardReciever.PhoneNumber.Length - 3))),
                                      AccountNo = string.IsNullOrEmpty(c.NonCardReciever.PhoneNumber) ? "" : c.NonCardReciever.PhoneNumber,
                                      StatusDescription = FAXER.PORTAL.Common.Common.GetEnumDescription(c.FaxingStatus),
                                      Status = c.FaxingStatus.ToString(),
                                      TransferMethod = TransactionTransferMethod.CashPickUp,
                                      TransactionDate = c.TransactionDate.Date,
                                      TransactionDateTime = c.TransactionDate,
                                      ReceiptNo = c.ReceiptNumber
                                  });
                var mobileWallet = (from c in dbContext.MobileMoneyTransfer.Where(x => x.SenderId == senderId
                                    && x.Status != MobileMoneyTransferStatus.Cancel).ToList()
                                        //join sendingCountry in dbContext.Country on c.SendingCountry equals sendingCountry.CountryCode
                                        //join receivingCountry in dbContext.Country on c.ReceivingCountry equals receivingCountry.CountryCode
                                    select new MobileActivityViewModel()
                                    {
                                        TransactionId = c.Id,
                                        ReceiverName = c.ReceiverName,
                                        ReceivingCountryCode = c.ReceivingCountry,
                                        ReceivingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),
                                        ReecivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyOrCountry(c.ReceivingCurrency, c.ReceivingCountry),
                                        SendingAmount = c.SendingAmount,
                                        ReceivingAmount = c.ReceivingAmount,
                                        TransferMethodName = "Mobile Wallet",
                                        //AccountNo = string.IsNullOrEmpty(c.PaidToMobileNo) ? " " : "**" + c.PaidToMobileNo.Substring((Math.Max(0, c.PaidToMobileNo.Length - 3))),
                                        AccountNo = string.IsNullOrEmpty(c.PaidToMobileNo) ? " " : c.PaidToMobileNo,
                                        StatusDescription = FAXER.PORTAL.Common.Common.GetEnumDescription(c.Status),
                                        Status = c.Status.ToString(),
                                        TransferMethod = TransactionTransferMethod.OtherWallet,
                                        TransactionDate = c.TransactionDate.Date,
                                        TransactionDateTime = c.TransactionDate,
                                        ReceiptNo = c.ReceiptNo
                                    });

                var data = bankDeposit.Concat(cashPickUp).Concat(mobileWallet).OrderByDescending(x => x.TransactionDateTime).ToList();

                foreach (var item in data.GroupBy(x => x.TransactionDate).Select(x => x.FirstOrDefault()).ToList())
                {

                    MobileActivityListvm model = new MobileActivityListvm();
                    string DateToday = DateTime.Now.Date.ToString("dddd, dd MMMM ");
                    string Itemdate = item.TransactionDate.ToString("dddd, dd MMMM ");
                    if (DateToday == Itemdate)
                    {
                        model.TransacationDate = "Today";
                    }
                    else
                    {
                        model.TransacationDate = Itemdate;
                    }
                    model.ActivityListvm = data.Where(x => x.TransactionDate == item.TransactionDate).OrderByDescending(x => x.TransactionDateTime).ToList();
                    vm.Add(model);
                }
                return new ServiceResult<List<MobileActivityListvm>>()
                {
                    Data = vm.Take(5).ToList(),
                    Message = "",
                    Status = ResultStatus.OK,
                };



                //var result = (from c in data.GroupBy(x => x.TransactionDate)
                //              select new MobileActivityListvm()
                //              {
                //                  TransacationDate = c.FirstOrDefault().TransactionDate,
                //                  ActivityListvm = data.Where(x => x.TransactionDate == c.FirstOrDefault().TransactionDate).OrderByDescending(x=>x.TransactionDate).ToList()
                //              }).ToList();

                //return new ServiceResult<List<MobileActivityListvm>>()
                //{
                //    Data = result.ToList(),
                //    Message = "",
                //    Status = ResultStatus.OK,
                //};
            }
            else
            {
                return new ServiceResult<List<MobileActivityListvm>>()
                {
                    Data = null,
                    Message = "Sender doesnt exist",
                    Status = ResultStatus.Error,
                };
            }
        }

        internal bool UpdateUserNotificationToken(int senderId, string notificationToken)
        {
            var faxerInformation = dbContext.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();
            if (faxerInformation != null && !string.IsNullOrEmpty(notificationToken))
            {
                faxerInformation.ClientToken = notificationToken;
                dbContext.Entry(faxerInformation).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        internal bool GetRecepientExist(int senderId)
        {
            bool isExist = false;
            int count = 0;
            count = dbContext.Recipients.Where(x => x.SenderId == senderId).Count();
            if (count > 0)
            {
                isExist = true;
            }
            return isExist;

        }

        internal int GetCountOfUnseenNotification(int senderId)
        {
            int count = 0;
            count = dbContext.Notification.Where(x => x.ReceiverId == senderId && x.IsSeen == false && x.NotificationSender == NotificationFor.Sender).Count();
            return count;
        }

        internal ServiceResult<MobileActivityViewModel> GetActivityDetails(int transactionId, TransactionTransferMethod transferMethod)
        {
            if (transactionId > 0)
            {
                MobileActivityViewModel vm = new MobileActivityViewModel();
                switch (transferMethod)
                {
                    case TransactionTransferMethod.BankDeposit:
                        var bankdeposit = dbContext.BankAccountDeposit.SingleOrDefault(x => x.Id == transactionId);
                        vm = new MobileActivityViewModel()
                        {
                            ReceiverName = bankdeposit.ReceiverName,
                            ReceivingAmount = bankdeposit.ReceivingAmount,
                            ReceivingCountryCode = bankdeposit.ReceivingCountry,
                            ReceivingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyByCurrencyOrCountry(bankdeposit.ReceivingCurrency, bankdeposit.ReceivingCountry),
                            
                            SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(bankdeposit.SendingCountry),
                            SendingAmount = bankdeposit.SendingAmount,
                            ReceiptNo = bankdeposit.ReceiptNo,
                            TransferMethod = TransactionTransferMethod.BankDeposit,
                            ExchangeRate = bankdeposit.ExchangeRate,
                            TransferMethodName = "Bank Deposit",
                            Fee = bankdeposit.Fee,
                            PaymentMethodName = FAXER.PORTAL.Common.Common.GetEnumDescription(bankdeposit.SenderPaymentMode),
                            ReceivingCity = bankdeposit.ReceiverCity,
                            ReceivingCountry = FAXER.PORTAL.Common.Common.GetCountryName(bankdeposit.ReceivingCountry),
                            SendingCurrencyCode = FAXER.PORTAL.Common.Common.GetCountryCurrency(bankdeposit.SendingCountry),
                            StatusDescription = FAXER.PORTAL.Common.Common.GetEnumDescription(bankdeposit.Status),
                            Status = bankdeposit.Status.ToString(),
                            TotalAmountPaid = bankdeposit.TotalAmount,
                            TransactionId = bankdeposit.Id,
                            TransactionDate = bankdeposit.TransactionDate,
                            FormattedDate = bankdeposit.TransactionDate.ToString("dddd, dd MMM yyyy"),
                            ReceiverFirstName = bankdeposit.ReceiverName == null ? " " : bankdeposit.ReceiverName.Split(' ')[0],
                            AccountNo = bankdeposit.ReceiverAccountNo,
                            BankId = bankdeposit.BankId,
                            BankName = FAXER.PORTAL.Common.Common.getBankName(bankdeposit.BankId),
                            BranchCode = bankdeposit.BankCode,
                            ReceipentId = bankdeposit.RecipientId,
                            ReceivingCountryPhoneCode = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(bankdeposit.ReceivingCountry),
                            ReecivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyOrCountry(bankdeposit.ReceivingCurrency, bankdeposit.ReceivingCountry),

                            SendingCountryCode = bankdeposit.SendingCountry,
                            MobileNo = bankdeposit.ReceiverMobileNo,
                            Reason = bankdeposit.ReasonForTransfer,
                            ReasonName = bankdeposit.ReasonForTransfer.ToString(),
                            TransactionDateTime = bankdeposit.TransactionDate,
                        };

                        break;
                    case TransactionTransferMethod.OtherWallet:
                        var otherWallet = dbContext.MobileMoneyTransfer.SingleOrDefault(x => x.Id == transactionId);
                        vm = new MobileActivityViewModel()
                        {
                            ReceiverName = otherWallet.ReceiverName,
                            TransferMethod = TransactionTransferMethod.OtherWallet,
                            ReceivingAmount = otherWallet.ReceivingAmount,
                            ReceivingCountryCode = otherWallet.ReceivingCountry,
                            ReceivingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyByCurrencyOrCountry(otherWallet.ReceivingCurrency, otherWallet.ReceivingCountry),

                            SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(otherWallet.SendingCountry),
                            SendingCountryCode = otherWallet.SendingCountry,
                            MobileWalletProviderName = FAXER.PORTAL.Common.Common.GetMobileWalletInfo(otherWallet.WalletOperatorId).Name,
                            SendingAmount = otherWallet.SendingAmount,
                            ReceiptNo = otherWallet.ReceiptNo,
                            ExchangeRate = otherWallet.ExchangeRate,
                            TransferMethodName = "Other Wallet",
                            Fee = otherWallet.Fee,
                            PaymentMethodName = FAXER.PORTAL.Common.Common.GetEnumDescription(otherWallet.SenderPaymentMode),
                            ReceivingCity = otherWallet.ReceiverCity,
                            ReceivingCountry = FAXER.PORTAL.Common.Common.GetCountryName(otherWallet.ReceivingCountry),
                            SendingCurrencyCode = FAXER.PORTAL.Common.Common.GetCountryCurrency(otherWallet.SendingCountry),
                            StatusDescription = FAXER.PORTAL.Common.Common.GetEnumDescription(otherWallet.Status),
                            Status = otherWallet.Status.ToString(),
                            TotalAmountPaid = otherWallet.TotalAmount,
                            TransactionId = otherWallet.Id,
                            TransactionDate = otherWallet.TransactionDate.Date,
                            FormattedDate = otherWallet.TransactionDate.ToString("dddd, dd MMM yyyy"),
                            ReceiverFirstName = otherWallet.ReceiverName == null ? " " : otherWallet.ReceiverName.Split(' ')[0],
                            AccountNo = otherWallet.PaidToMobileNo,
                            WalletId = otherWallet.WalletOperatorId,

                            ReceipentId = otherWallet.RecipientId,
                            ReceivingCountryPhoneCode = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(otherWallet.ReceivingCountry),
                            ReecivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyOrCountry(otherWallet.ReceivingCurrency, otherWallet.ReceivingCountry),
                            MobileNo = otherWallet.PaidToMobileNo,
                            Reason = otherWallet.ReasonForTransfer,
                            ReasonName = otherWallet.ReasonForTransfer.ToString(),
                            TransactionDateTime = otherWallet.TransactionDate,

                        };

                        break;
                    case TransactionTransferMethod.CashPickUp:
                        var cashPickUp = dbContext.FaxingNonCardTransaction.SingleOrDefault(x => x.Id == transactionId);
                        vm = new MobileActivityViewModel()
                        {
                            ReceiverName = cashPickUp.NonCardReciever.FullName,
                            ReceivingAmount = cashPickUp.ReceivingAmount,
                            ReceivingCountryCode = cashPickUp.ReceivingCountry,
                            ReceivingCountryPhoneCode = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(cashPickUp.ReceivingCountry),
                            ReceivingCurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.ReceivingCurrency, cashPickUp.ReceivingCountry),
                            SendingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(cashPickUp.SendingCountry),
                            SendingAmount = cashPickUp.FaxingAmount,
                            ReceiptNo = "MFCN:" + cashPickUp.MFCN,
                            SendingCountryCode = cashPickUp.SendingCountry,
                            ExchangeRate = cashPickUp.ExchangeRate,
                            TransferMethodName = "Cash Pick Up",
                            TransferMethod = TransactionTransferMethod.CashPickUp,
                            Fee = cashPickUp.FaxingFee,
                            PaymentMethodName = FAXER.PORTAL.Common.Common.GetEnumDescription(cashPickUp.SenderPaymentMode),
                            ReceivingCity = cashPickUp.NonCardReciever.City,
                            ReceivingCountry = FAXER.PORTAL.Common.Common.GetCountryName(cashPickUp.ReceivingCountry),
                            SendingCurrencyCode = FAXER.PORTAL.Common.Common.GetCountryCurrency(cashPickUp.SendingCountry),
                            StatusDescription = FAXER.PORTAL.Common.Common.GetEnumDescription(cashPickUp.FaxingStatus),
                            Status = cashPickUp.FaxingStatus.ToString(),
                            TotalAmountPaid = cashPickUp.TotalAmount,
                            TransactionId = cashPickUp.Id,
                            TransactionDate = cashPickUp.TransactionDate.Date,
                            FormattedDate = cashPickUp.TransactionDate.ToString("dddd, dd MMM yyyy"),
                            ReceiverFirstName = cashPickUp.NonCardReciever.FirstName,
                            AccountNo = cashPickUp.NonCardReciever.PhoneNumber,
                            MobileNo = cashPickUp.NonCardReciever.PhoneNumber,
                            ReecivingCurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbolByCurrencyOrCountry(cashPickUp.ReceivingCurrency, cashPickUp.ReceivingCountry),
                            
                        };
                        break;
                }
                return new ServiceResult<MobileActivityViewModel>()
                {
                    Data = vm,
                    Message = "",
                    Status = ResultStatus.OK
                };
            }
            else
            {
                return new ServiceResult<MobileActivityViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Error
                };
            }

        }
    }
}