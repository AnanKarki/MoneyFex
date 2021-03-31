using Antlr.Runtime.Tree;
using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Admin.ViewModels.SearchParamViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using Microsoft.Ajax.Utilities;
using Microsoft.Office.Core;
using PagedList;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Services
{
    public class SSenderTransactionHistory
    {
        FAXER.PORTAL.DB.FAXEREntities dbContext = null;
        SSenderForAllTransfer _senderForAllTransfer = null;

        RecipientServices _recipientServices = null;

        private string TotalAmountWithCurrency = "";
        private string TotalFeePaidwithCurrency = "";
        public SSenderTransactionHistory()
        {
            dbContext = new DB.FAXEREntities();
            _senderForAllTransfer = new SSenderForAllTransfer();
            _recipientServices = new RecipientServices();
        }
        public List<Common.DropDownViewModel> GetWallets(string Country)
        {
            var data = dbContext.MobileWalletOperator.ToList();
            if (!string.IsNullOrEmpty(Country))
            {
                data = data.Where(x => x.Country == Country).ToList();
            }
            var list = (from c in data
                        select new Common.DropDownViewModel()
                        {
                            Id = c.Id,
                            Name = c.Name,
                            CountryCode = c.Country.ToUpper()
                        }).ToList();
            return list;

        }


        public List<Common.DropDownViewModel> GetRecentPaidReceivers(int senderId, DB.Module module, int WalletId, string Country)
        {
            SSenderMobileMoneyTransfer _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            var result = _mobileMoneyTransferServices.GetRecentlyPaidNumbers(senderId, module, WalletId).
             Where(x => x.CountryCode == Country).ToList();
            return result;

        }
        public SenderTransactionHistoryViewModel GetTransactionHistories(TransactionServiceType transactionService,
            int faxerId = 0, int year = 0, int month = 0, int PageNum = 1, int pageSize = 10, bool IsBusiness = false)
        {
            SenderTransactionHistoryViewModel transactionHistory = new SenderTransactionHistoryViewModel();
            transactionHistory.FilterKey = transactionService;
            transactionHistory.TransactionHistoryList = new List<SenderTransactionHistoryList>();
            string FromDate = "";
            string ToDate = "";
            if (year != 0 && month != 0)
            {
                int days = DateTime.DaysInMonth(year, month);
                var DateRange = year + "/" + month + "/1-" + year + "/" + month + "/" + days;
                var Date = DateRange.Split('-');
                FromDate = Date[0].ToDateTime().ToString("yyyy/MM/dd");
                ToDate = Date[1].ToDateTime().ToString("yyyy/MM/dd");
            }
            if (month == 0)
            {
                FromDate = year + "/01/01";
                ToDate = year + "12/31";
            }
            if (year == 0)
            {
                FromDate = "";
                ToDate = "";
            }
            SenderTransactionSearchParamVm searchParamVm = new SenderTransactionSearchParamVm()
            {
                DateRange = "",
                FromDate = FromDate,
                ToDate = ToDate,
                MFCode = "",
                PageNum = PageNum,
                PageSize = pageSize,
                PhoneNumber = "",
                ReceiverName = "",
                ReceivingCountry = "",
                ResponsiblePerson = "",
                SearchByStatus = "",
                searchString = "",
                SenderName = "",
                SenderEmail = "",
                SendingCountry = "",
                SendingCurrency = "",
                Status = "",
                TransactionServiceType = (int)transactionService,
                senderId = faxerId,
                TransactionWithAndWithoutFee = "",
                IsBusiness = IsBusiness
            };
            //transactionHistory.TransactionHistoryList = GetTransactionHistoryOfSender(searchParamVm);
            //return transactionHistory;
            switch (transactionService)
            {
                case TransactionServiceType.MobileWallet:
                    transactionHistory.TransactionHistoryList = GetMobileTransferDetails(faxerId, year, month);
                    break;
                case TransactionServiceType.KiiPayWallet:
                    transactionHistory.TransactionHistoryList = GetKiiPayWalletPayment(faxerId, year, month);
                    break;
                case TransactionServiceType.BillPayment:
                    transactionHistory.TransactionHistoryList = GetBillPaymentDetails(faxerId, year, month);
                    break;
                case TransactionServiceType.ServicePayment:
                    transactionHistory.TransactionHistoryList = GetServicePaymentDetails(faxerId, year, month);
                    break;
                case TransactionServiceType.CashPickUp:
                    transactionHistory.TransactionHistoryList = GetCashPickUpDetails(faxerId, year, month);
                    break;
                case TransactionServiceType.BankDeposit:
                    transactionHistory.TransactionHistoryList = GetBankDepositDetails(faxerId, year, month);
                    break;
                case TransactionServiceType.All:
                    transactionHistory.TransactionHistoryList = GetMobileTransferDetails(faxerId, year, month).
                                                                 Concat(GetKiiPayWalletPayment(faxerId, year, month)).
                                                                 Concat(GetBillPaymentDetails(faxerId, year, month))
                                                                 .Concat(GetServicePaymentDetails(faxerId, year, month)).
                                                                 Concat(GetCashPickUpDetails(faxerId, year, month))
                                                                 .Concat(GetBankDepositDetails(faxerId, year, month)).ToList();
                    break;
                default:

                    transactionHistory.TransactionHistoryList = GetMobileTransferDetails(faxerId, year, month).
                                                                 Concat(GetKiiPayWalletPayment(faxerId, year, month)).
                                                                 Concat(GetBillPaymentDetails(faxerId, year, month))
                                                                 .Concat(GetServicePaymentDetails(faxerId, year, month)).
                                                                 Concat(GetCashPickUpDetails(faxerId, year, month))
                                                                 .Concat(GetBankDepositDetails(faxerId, year, month)).ToList();
                    break;
            }
            transactionHistory.TransactionHistoryList = transactionHistory.TransactionHistoryList.OrderByDescending(x => x.TransactionDate).ToList();
            return transactionHistory;
        }
        public SenderTransactionHistoryViewModel GetTransactionDetails(TransactionServiceType transactionService,
            int faxerId = 0, int TransactionId = 0)
        {
            SenderTransactionHistoryViewModel transactionHistory = new SenderTransactionHistoryViewModel();
            transactionHistory.FilterKey = transactionService;
            transactionHistory.TransactionDetails = GetTransactiondetails(TransactionId, transactionService);
            return transactionHistory;
        }




        public void SaveNotes(TransactionStatementNoteViewModel vm)
        {
            TransactionStatementNote model = new TransactionStatementNote()
            {
                TransactionId = vm.TransactionId,
                Note = vm.Note,
                CreatedBy = Common.StaffSession.LoggedStaff.StaffId,
                CreatedDateAndTime = DateTime.Now,
                NoteType = vm.NoteType
            };
            switch (vm.TransactionMethodName.ToLower())
            {
                case "mobilewallet":
                    model.TransactionMethod = TransactionServiceType.MobileWallet;
                    break;
                case "kiipaywallet":
                    model.TransactionMethod = TransactionServiceType.KiiPayWallet;
                    break;
                case "billpayment":
                    model.TransactionMethod = TransactionServiceType.BillPayment;
                    break;
                case "servicepayment":
                    model.TransactionMethod = TransactionServiceType.ServicePayment;
                    break;
                case "cashpickup":
                    model.TransactionMethod = TransactionServiceType.CashPickUp;
                    break;
                case "bankdeposit":
                    model.TransactionMethod = TransactionServiceType.BankDeposit;
                    break;
            }

            dbContext.TransactionStatementNote.Add(model);
            dbContext.SaveChanges();
        }

        public List<SenderTransactionHistoryList> GetKiiPayWalletPayment(int senderId, int year = 0, int month = 0)
        {
            var data = dbContext.TopUpSomeoneElseCardTransaction.ToList();

            //if (!IsAdmin)
            //{

            //    data = data.Where(x => x.FaxerId == senderId).ToList();

            //}
            if (senderId != 0)
            {
                data = data.Where(x => x.FaxerId == senderId).ToList();
            }
            if (year != 0)
            {

                data = data.Where(x => x.TransactionDate.Year == year).ToList();
            }
            if (month != 0)
            {

                data = data.Where(x => x.TransactionDate.Month == month).ToList();
            }

            //var senderInfo = GetFaxerInformation(senderId);
            //string senderAccountNo = "";
            //string senderCountry = "";
            //string senderFullName = "";
            //if (senderInfo != null)
            //{
            //    senderAccountNo = senderInfo.AccountNo;
            //    senderCountry = Common.Common.GetCountryName(senderInfo.Country);
            //    senderFullName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;

            //}
            var result = (from c in data
                          join d in dbContext.FaxerInformation on c.FaxerId equals d.Id
                          join creditDebitCardInfo in dbContext.CardTopUpCreditDebitInformation.Where(x => x.TransferType == (int)TransferType.BankDeposit)
                   on c.Id equals creditDebitCardInfo.CardTransactionId into cardInfo
                          from creditDebitCardInfo in cardInfo.DefaultIfEmpty()
                          select new SenderTransactionHistoryList()
                          {
                              Id = c.Id,
                              AccountNumber = c.KiiPayPersonalWalletInformation.MobileNo,
                              SenderTelephoneNo = d.PhoneNumber,
                              Date = c.TransactionDate.ToString("MMMM dd yyyy"),
                              Fee = c.FaxingFee,
                              GrossAmount = c.FaxingAmount,
                              ReceiverName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                              Reference = c.TopUpReference,
                              Status = FaxingStatus.Completed,
                              StatusName = Common.Common.GetEnumDescription(FaxingStatus.Completed),
                              TransactionType = Common.Common.GetEnumDescription(TransactionServiceType.KiiPayWallet),
                              ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                              ReceivingCurrrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                              SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                              ReceivingAmount = c.RecievingAmount,
                              TotalAmount = c.TotalAmount,
                              SenderPaymentMode = c.SenderPaymentMode,
                              CardNumber = creditDebitCardInfo == null ? "" : creditDebitCardInfo.CardNumber,
                              //PaymentMethod = SSenderForAllTransfer.GetCreditCardLastDigit(c.SenderPaymentMode, TransferType.KiiPayWallet, c.Id),
                              ReceiverCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                              ReceiverCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                              ExchangeRate = c.ExchangeRate,
                              TransactionServiceType = TransactionServiceType.KiiPayWallet,
                              TransactionDate = c.TransactionDate,
                              TransactionIdentifier = c.ReceiptNumber,
                              FaxerAccountNo = d.AccountNo,
                              FaxerCountry = Common.Common.GetCountryName(d.Country),
                              SenderName = d.FirstName.Trim() + " " + (string.IsNullOrEmpty(d.MiddleName) == true ? "" : d.MiddleName.Trim() + " ") + d.LastName.Trim(),
                              senderId = c.FaxerId,
                              SenderEmail = d.Email ?? "",
                              IsBusiness = d.IsBusiness,
                              SenderCountryCode = c.SendingCountry,


                          }).ToList();

            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            int SenderWalletId = 0;
            var senderWalletInfo = senderCommonFunc.GetSenderKiiPayWalletInfo(senderId);

            if (senderWalletInfo != null)
            {
                SenderWalletId = senderWalletInfo.Id;
            }

            var kiipaywallet = dbContext.KiiPayPersonalWalletPaymentByKiiPayPersonal.ToList();

            //if (!IsAdmin)
            //{
            //    kiipaywallet = kiipaywallet.Where(x => x.SenderWalletId == SenderWalletId).ToList();

            //}
            if (senderId != 0)
            {
                kiipaywallet = kiipaywallet.Where(x => x.SenderWalletId == SenderWalletId).ToList();
            }

            if (year != 0)
            {
                kiipaywallet = kiipaywallet.Where(x => x.TransactionDate.Year == year).ToList();
            }
            if (month != 0)
            {
                kiipaywallet = kiipaywallet.Where(x => x.TransactionDate.Month == month).ToList();

            }
            var transactionUsingKiipayWallet = (from c in kiipaywallet
                                                join d in dbContext.FaxerInformation on c.SenderId equals d.Id
                                                select new SenderTransactionHistoryList()
                                                {
                                                    Id = c.Id + 777,
                                                    AccountNumber = c.KiiPayPersonalWalletInformation.MobileNo,
                                                    Date = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                    Fee = c.FaxingFee,
                                                    GrossAmount = c.FaxingAmount,
                                                    ReceiverName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                                                    Reference = c.PaymentReference,
                                                    Status = FaxingStatus.Completed,
                                                    StatusName = Common.Common.GetEnumDescription(FaxingStatus.Completed),
                                                    TransactionType = Common.Common.GetEnumDescription(TransactionServiceType.KiiPayWallet),
                                                    ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                                                    ReceivingCurrrency = Common.Common.GetCountryCurrency(c.ReceivingCountry),
                                                    SendingCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                                                    SendingCurrencySymbol = Common.Common.GetCurrencySymbol(c.SendingCountry),
                                                    ReceivingAmount = c.RecievingAmount,
                                                    TotalAmount = c.TotalAmount,
                                                    SenderPaymentMode = SenderPaymentMode.KiiPayWallet,
                                                    //PaymentMethod = SenderPaymentMode.KiiPayWallet.ToString(),
                                                    ReceiverCity = c.KiiPayPersonalWalletInformation.CardUserCity,
                                                    ReceiverCountry = Common.Common.GetCountryName(c.KiiPayPersonalWalletInformation.CardUserCountry),
                                                    ExchangeRate = c.ExchangeRate,
                                                    TransactionServiceType = TransactionServiceType.KiiPayWallet,
                                                    TransactionDate = c.TransactionDate,
                                                    TransactionIdentifier = c.ReceiptNumber,
                                                    FaxerAccountNo = d.AccountNo,
                                                    FaxerCountry = Common.Common.GetCountryName(d.Country),
                                                    SenderName = d.FirstName + " " + (string.IsNullOrEmpty(d.MiddleName) == true ? "" : d.MiddleName + " ") + d.LastName,
                                                    senderId = c.SenderId,
                                                    IsBusiness = d.IsBusiness


                                                }).ToList();
            return result.Concat(transactionUsingKiipayWallet).ToList();

        }


        public List<SenderRecentReceiverDropDownVM> GetRecentReceivers(string CountryCode = "", int SenderId = 0)
        {
            List<Recipients> receiverDetailsList = dbContext.Recipients.Where(x => x.SenderId == SenderId).ToList();

            if (!string.IsNullOrEmpty(CountryCode))
            {
                receiverDetailsList = receiverDetailsList.Where(x => x.Country == CountryCode).ToList();
            }
            var result = (from c in receiverDetailsList
                          select new SenderRecentReceiverDropDownVM()
                          {
                              Id = c.Id,
                              ReceiverName = c.ReceiverName,
                              ReceiverMobileNo = c.MobileNo
                          }).GroupBy(x => x.ReceiverMobileNo).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }
        public SenderTransactionHistoryList GetTransactiondetails(int id, TransactionServiceType transactionServiceType)
        {
            SenderTransactionHistoryList model = new SenderTransactionHistoryList();
            model.TransactionServiceType = transactionServiceType;
            var senderInfo = new FaxerInformation();
            switch (transactionServiceType)
            {
                case TransactionServiceType.BankDeposit:
                    SSenderBankAccountDeposit _senderBankAccountDeposit = new SSenderBankAccountDeposit();
                    var bankAccountDeposit = _senderBankAccountDeposit.List().Data.Where(x => x.Id == id).FirstOrDefault();
                    var recipientData = dbContext.Recipients.Where(x => x.Id == bankAccountDeposit.RecipientId).FirstOrDefault();
                    if (recipientData != null)
                    {
                        model.ReceiverCity = recipientData.City;
                        model.ReceiverEmail = recipientData.Email;
                        model.ReceiverStreet = recipientData.Street;
                        model.ReceiverPostalCode = recipientData.PostalCode;

                    }
                    model.Id = id;
                    model.FaxerCountry = bankAccountDeposit.SendingCountry;
                    model.ReceiverCountry = bankAccountDeposit.ReceivingCountry;
                    model.AccountNumber = bankAccountDeposit.ReceiverAccountNo;
                    model.senderId = bankAccountDeposit.SenderId;
                    model.BankId = bankAccountDeposit.BankId;
                    model.MobileNo = bankAccountDeposit.ReceiverMobileNo;
                    model.TransactionIdentifier = bankAccountDeposit.ReceiptNo;
                    model.ReceiverName = bankAccountDeposit.ReceiverName;
                    model.BankCode = bankAccountDeposit.BankCode;
                    model.ReceivingAmount = bankAccountDeposit.ReceivingAmount;
                    model.Fee = bankAccountDeposit.Fee;
                    model.TotalAmount = bankAccountDeposit.TotalAmount;
                    model.GrossAmount = bankAccountDeposit.SendingAmount;
                    model.IsEuropeTransfer = bankAccountDeposit.IsEuropeTransfer;
                    model.IsManualBankDeposit = bankAccountDeposit.IsManualDeposit;
                    model.IsBusiness = bankAccountDeposit.IsBusiness;
                    senderInfo = Common.Common.GetSenderInfo(bankAccountDeposit.SenderId);
                    model.SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;
                    model.FaxerAccountNo = senderInfo.AccountNo;
                    model.SenderCountryName = Common.Common.GetCountryName(bankAccountDeposit.SendingCountry);
                    model.ExchangeRate = bankAccountDeposit.ExchangeRate;

                    model.Date = bankAccountDeposit.TransactionDate.ToString("yyyy/MM/dd");
                    model.SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(bankAccountDeposit.SendingCurrency, bankAccountDeposit.SendingCountry);
                    model.ReceivingCurrrency = Common.Common.GetCurrencyByCurrencyOrCountry(bankAccountDeposit.ReceivingCurrency, bankAccountDeposit.ReceivingCountry);
                    model.ReceivingCountryName = Common.Common.GetCountryName(bankAccountDeposit.ReceivingCountry);
                    model.SendingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(bankAccountDeposit.SendingCurrency, bankAccountDeposit.SendingCountry);
                    model.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(bankAccountDeposit.SendingCurrency, bankAccountDeposit.SendingCountry);
                    model.BankName = Common.Common.getBankName(bankAccountDeposit.BankId);
                    break;
                case TransactionServiceType.CashPickUp:
                    SSenderCashPickUp SSenderCashPickUp = new SSenderCashPickUp();
                    var cashPickUp = SSenderCashPickUp.List().Data.Where(x => x.Id == id).FirstOrDefault();
                    model.Id = id;
                    model.FaxerCountry = cashPickUp.SendingCountry;
                    model.ReceiverCountry = cashPickUp.ReceivingCountry;
                    model.senderId = cashPickUp.SenderId;
                    model.MobileNo = cashPickUp.NonCardReciever.PhoneNumber;
                    model.TransactionIdentifier = cashPickUp.ReceiptNumber;
                    model.ReceiverName = cashPickUp.NonCardReciever.FullName;
                    model.ReceivingAmount = cashPickUp.ReceivingAmount;
                    model.Fee = cashPickUp.FaxingFee;
                    model.TotalAmount = cashPickUp.TotalAmount;
                    model.GrossAmount = cashPickUp.FaxingAmount;
                    senderInfo = Common.Common.GetSenderInfo(cashPickUp.SenderId);
                    model.SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;
                    model.FaxerAccountNo = senderInfo.AccountNo;
                    model.SenderCountryName = Common.Common.GetCountryName(cashPickUp.SendingCountry);
                    model.RecipientId = cashPickUp.RecipientId;
                    model.ExchangeRate = cashPickUp.ExchangeRate;
                    model.RecipientIdenityCardId = cashPickUp.RecipientIdentityCardId;
                    model.RecipientIdentityCardNumber = cashPickUp.RecipientIdenityCardNumber;

                    model.Date = cashPickUp.TransactionDate.ToString("yyyy/MM/dd");
                    model.SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.SendingCurrency, cashPickUp.SendingCountry);
                    model.ReceivingCurrrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashPickUp.ReceivingCurrency, cashPickUp.ReceivingCountry);
                    model.ReceivingCountryName = Common.Common.GetCountryName(cashPickUp.ReceivingCountry);
                    model.SendingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(cashPickUp.SendingCurrency, cashPickUp.SendingCountry);
                    model.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(cashPickUp.SendingCurrency, cashPickUp.SendingCountry);

                    break;
                case TransactionServiceType.MobileWallet:
                    SSenderMobileMoneyTransfer SSenderMobileMoneyTransfer = new SSenderMobileMoneyTransfer();
                    var mobileMoneyTransfer = SSenderMobileMoneyTransfer.list().Data.Where(x => x.Id == id).FirstOrDefault();
                    model.Id = id;
                    model.WalletId = mobileMoneyTransfer.WalletOperatorId;
                    model.FaxerCountry = mobileMoneyTransfer.SendingCountry;
                    model.ReceiverCountry = mobileMoneyTransfer.ReceivingCountry;
                    model.senderId = mobileMoneyTransfer.SenderId;
                    model.MobileNo = mobileMoneyTransfer.PaidToMobileNo;
                    model.AccountNumber = mobileMoneyTransfer.PaidToMobileNo;
                    model.TransactionIdentifier = mobileMoneyTransfer.ReceiptNo;
                    model.ReceivingAmount = mobileMoneyTransfer.ReceivingAmount;
                    model.Fee = mobileMoneyTransfer.Fee;
                    model.TotalAmount = mobileMoneyTransfer.TotalAmount;
                    model.GrossAmount = mobileMoneyTransfer.SendingAmount;
                    model.ReceiverName = GetReceiverNameByReceipientId(mobileMoneyTransfer.RecipientId);
                    senderInfo = Common.Common.GetSenderInfo(mobileMoneyTransfer.SenderId);
                    model.SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;
                    model.FaxerAccountNo = senderInfo.AccountNo;
                    model.SenderCountryName = Common.Common.GetCountryName(mobileMoneyTransfer.SendingCountry);
                    model.ExchangeRate = mobileMoneyTransfer.ExchangeRate;

                    model.Date = mobileMoneyTransfer.TransactionDate.ToString("yyyy/MM/dd");
                    model.SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(mobileMoneyTransfer.SendingCurrency, mobileMoneyTransfer.SendingCountry);
                    model.ReceivingCurrrency = Common.Common.GetCurrencyByCurrencyOrCountry(mobileMoneyTransfer.ReceivingCurrency, mobileMoneyTransfer.ReceivingCountry);
                    model.ReceivingCountryName = Common.Common.GetCountryName(mobileMoneyTransfer.ReceivingCountry);
                    model.SendingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(mobileMoneyTransfer.SendingCurrency, mobileMoneyTransfer.SendingCountry);
                    model.ReceivingCurrencySymbol = Common.Common.GetCurrencySymbolByCurrencyOrCountry(mobileMoneyTransfer.SendingCurrency, mobileMoneyTransfer.SendingCountry);

                    break;
            }

            return model;
        }

        internal PGTransactionResultVm CheckPGStatus(string refno, TransactionServiceType transferMethod)
        {
            CardProcessorApi? cardProcessorAPi = null;
            switch (transferMethod)
            {
                case TransactionServiceType.MobileWallet:
                    cardProcessorAPi = dbContext.MobileMoneyTransfer.Where(x => x.ReceiptNo == refno).Select(x => x.CardProcessorApi).FirstOrDefault();
                    break;
                case TransactionServiceType.CashPickUp:
                    cardProcessorAPi = dbContext.FaxingNonCardTransaction.Where(x => x.ReceiptNumber == refno).Select(x => x.CardProcessorApi).FirstOrDefault();
                    break;
                case TransactionServiceType.BankDeposit:
                    cardProcessorAPi = dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == refno).Select(x => x.CardProcessorApi).FirstOrDefault();
                    break;
            }
            if (cardProcessorAPi == null)
            {
                cardProcessorAPi = CardProcessorApi.TrustPayment;
            }
            var result = new PGTransactionResultVm();
            switch (cardProcessorAPi)
            {
                case CardProcessorApi.TrustPayment:
                    var pgTransaction = Common.Common.GetPGRefNo(refno);
                    if (pgTransaction.transactionreference != null || pgTransaction.currencyiso3a != null)
                    {
                        result = StripServices.GetTransactionDetails(pgTransaction.transactionreference, pgTransaction.currencyiso3a);
                    }
                    break;
                case CardProcessorApi.T365:
                    //var uid = dbContext.Transact365ApiResponseTransationLog.Where(x => x.TrackingId == refno).Select(x => x.UId).FirstOrDefault();
                    result = Transact365Serivces.GetTranstionStatus("" , refno);
                    break;
            }
            return result;
        }

        public string GetReceiverNameByReceipientId(int recipientId)
        {
            string recipientName = dbContext.Recipients.Where(x => x.Id == recipientId).Select(x => x.ReceiverName).FirstOrDefault();
            return recipientName;
        }

        public void SetSenderTransactionHistoryList(SenderTransactionHistoryList vm)
        {

            Common.AdminSession.SenderTransactionHistoryList = vm;

        }

        public SenderTransactionHistoryList GetSenderTransactionHistoryList()
        {

            SenderTransactionHistoryList vm = new SenderTransactionHistoryList();

            if (Common.AdminSession.SenderTransactionHistoryList != null)
            {


                vm = Common.AdminSession.SenderTransactionHistoryList;

            }
            return vm;
        }
        public SenderAddMoneySuccessVM CompleteDuplicateTransaction()
        {
            var TransactionId = Common.AdminSession.TransactionId;
            var TransactionServiceType = Common.AdminSession.TransactionServiceType;
            Areas.Agent.AgentServices.SCashPickUpTransferService SCashPickUpTransferService = new Areas.Agent.AgentServices.SCashPickUpTransferService();

            var paymentInfo = SCashPickUpTransferService.GetStaffCashPickUpEnterAmount();
            var TransactionDetails = GetSenderTransactionHistoryList();
            var accountDeposit = Common.AdminSession.SenderBankAccountDeposit;
            int recipientId = CreateOrUpdateRecipient();

            switch (TransactionServiceType)
            {
                case TransactionServiceType.BankDeposit:

                    var bankDepositData = new BankAccountDeposit();

                    bankDepositData.SendingAmount = paymentInfo.SendingAmount;
                    bankDepositData.ReceivingAmount = paymentInfo.ReceivingAmount;
                    bankDepositData.TotalAmount = paymentInfo.TotalAmount;
                    bankDepositData.Fee = paymentInfo.Fee;
                    bankDepositData.ExchangeRate = paymentInfo.ExchangeRate;
                    bankDepositData.ReceiverName = accountDeposit.AccountOwnerName;
                    bankDepositData.ReceiverAccountNo = accountDeposit.AccountNumber;
                    bankDepositData.BankId = accountDeposit.BankId;
                    bankDepositData.BankCode = accountDeposit.BranchCode;
                    bankDepositData.BankName = accountDeposit.BankName;
                    bankDepositData.IsEuropeTransfer = accountDeposit.IsEuropeTransfer;
                    bankDepositData.TransactionDate = DateTime.Now;
                    bankDepositData.ReceiverMobileNo = accountDeposit.MobileNumber;
                    bankDepositData.RecipientId = recipientId;
                    bankDepositData.ReceivingCurrency = paymentInfo.ReceivingCurrency;
                    bankDepositData.SendingCurrency = paymentInfo.SendingCurrency;


                    SSenderBankAccountDeposit _senderBankAccountDeposit = new SSenderBankAccountDeposit();
                    _senderBankAccountDeposit.CreateDuplicateTransaction(TransactionId, bankDepositData);
                    break;
                case TransactionServiceType.CashPickUp:

                    var CashPickUpData = new FaxingNonCardTransaction();

                    CashPickUpData.FaxingAmount = paymentInfo.SendingAmount;
                    CashPickUpData.ReceivingAmount = paymentInfo.ReceivingAmount;
                    CashPickUpData.TotalAmount = paymentInfo.TotalAmount;
                    CashPickUpData.FaxingFee = paymentInfo.Fee;
                    CashPickUpData.ExchangeRate = paymentInfo.ExchangeRate;
                    CashPickUpData.RecipientId = recipientId;

                    CashPickUpData.ReceivingCurrency = paymentInfo.ReceivingCurrency;
                    CashPickUpData.SendingCurrency = paymentInfo.SendingCurrency;
                    CreateCashPickUpDuplicateTransaction(TransactionId, CashPickUpData);
                    break;
                case TransactionServiceType.MobileWallet:
                    var MobileWallet = new MobileMoneyTransfer();
                    MobileWallet.SendingAmount = paymentInfo.SendingAmount;
                    MobileWallet.ReceivingAmount = paymentInfo.ReceivingAmount;
                    MobileWallet.TotalAmount = paymentInfo.TotalAmount;
                    MobileWallet.Fee = paymentInfo.Fee;
                    MobileWallet.ExchangeRate = paymentInfo.ExchangeRate;
                    MobileWallet.WalletOperatorId = TransactionDetails.WalletId;
                    MobileWallet.TransactionDate = DateTime.Now;
                    MobileWallet.PaidToMobileNo = accountDeposit.MobileNumber;
                    MobileWallet.ReceiverName = accountDeposit.AccountOwnerName;
                    MobileWallet.RecipientId = recipientId;

                    MobileWallet.ReceivingCurrency = paymentInfo.ReceivingCurrency;
                    MobileWallet.SendingCurrency = paymentInfo.SendingCurrency;
                    CreateMobileWalletDuplicateTransaction(TransactionId, MobileWallet);

                    break;
            }
            SenderAddMoneySuccessVM vm = new SenderAddMoneySuccessVM
            {
                ReceiverName = accountDeposit.AccountOwnerName,
                Amount = paymentInfo.SendingAmount,
                Currnecy = Common.Common.GetCurrencySymbol(paymentInfo.SendingCountry)
            };
            return vm;

        }

        private int CreateOrUpdateRecipient()
        {
            int recipientId = 0;
            var accountDeposit = Common.AdminSession.SenderBankAccountDeposit;
            RecipientServices _recipientServices = new RecipientServices();
            var TransactionServiceType = Common.AdminSession.TransactionServiceType;
            var service = GetService(TransactionServiceType);
            var recipient = _recipientServices.Recipients().Where(x => x.ReceiverName.ToLower() == accountDeposit.AccountOwnerName.ToLower() &&
                                                                       x.Service == service &&
                                                                       (x.AccountNo == accountDeposit.AccountNumber ||
                                                                        x.MobileNo == accountDeposit.MobileNumber)).FirstOrDefault();

            var recipients = GetRecipientsDetais(accountDeposit, service);
            if (recipient != null)
            {
                recipients = recipient;
                recipients.Id = recipient.Id;
                recipientId = recipient.Id;
                _recipientServices.UpdateReceipts(recipients);
            }
            else
            {
                _recipientServices.Add(recipients);
                recipientId = recipients.Id;
            }
            return recipientId;
        }

        public Recipients GetRecipientsDetais(SenderBankAccountDepositVm accountDeposit, Service service)
        {
            Recipients recipient = new Recipients();
            recipient.ReceiverName = accountDeposit.AccountOwnerName.Trim();
            recipient.AccountNo = accountDeposit.AccountNumber;
            recipient.City = accountDeposit.ReceiverCity;
            recipient.Email = accountDeposit.ReceiverEmail;
            recipient.PostalCode = accountDeposit.ReceiverPostalCode;
            recipient.Street = accountDeposit.ReceiverStreet;
            recipient.IdentificationTypeId = accountDeposit.IdenityCardId;
            recipient.IdentificationNumber = accountDeposit.IdentityCardNumber;
            recipient.Service = service;
            recipient.Country = accountDeposit.CountryCode;
            recipient.SenderId = accountDeposit.SenderId;
            switch (service)
            {
                case Service.BankAccount:
                    recipient.BranchCode = Common.Common.getBank(accountDeposit.BankId).Code;
                    recipient.BankId = accountDeposit.BankId;
                    break;
                case Service.MobileWallet:
                    recipient.MobileNo = accountDeposit.MobileNumber;
                    recipient.AccountNo = accountDeposit.MobileNumber;
                    recipient.MobileWalletProvider = accountDeposit.walletId;
                    break;
                case Service.CashPickUP:
                    recipient.MobileNo = accountDeposit.MobileNumber;
                    recipient.AccountNo = accountDeposit.MobileNumber;
                    break;
            }

            return recipient;
        }
        private Service GetService(TransactionServiceType transactionServiceType)
        {
            Service service = Service.Select;
            switch (transactionServiceType)
            {
                case TransactionServiceType.MobileWallet:
                    service = Service.MobileWallet;
                    break;
                case TransactionServiceType.KiiPayWallet:
                    service = Service.KiiPayWallet;
                    break;
                case TransactionServiceType.CashPickUp:
                    service = Service.CashPickUP;
                    break;
                case TransactionServiceType.BankDeposit:
                    service = Service.BankAccount;
                    break;
            }
            return service;
        }
        private void CreateMobileWalletDuplicateTransaction(int transactionId, MobileMoneyTransfer mobileWallet)
        {
            var mobileWalletData = dbContext.MobileMoneyTransfer.Where(x => x.Id == transactionId).FirstOrDefault();
            MobileMoneyTransfer result = new MobileMoneyTransfer()
            {
                Status = MobileMoneyTransferStatus.Held,
                IsComplianceNeededForTrans = true,
                PayingStaffId = Common.StaffSession.LoggedStaff.StaffId,
                ReceiptNo = Common.Common.GenerateMobileMoneyTransferReceiptNo(6),
                ExchangeRate = mobileWallet.ExchangeRate,
                Fee = mobileWallet.Fee,
                MFRate = mobileWallet.MFRate,
                PaymentReference = mobileWalletData.PaymentReference,
                ReceivingAmount = mobileWallet.ReceivingAmount,
                ReceivingCountry = mobileWalletData.ReceivingCountry,
                SendingCountry = mobileWalletData.SendingCountry,
                TotalAmount = mobileWallet.TotalAmount,
                RecipientId = mobileWallet.RecipientId,
                TransactionDate = DateTime.Now,
                SenderPaymentMode = mobileWalletData.SenderPaymentMode,
                SenderId = mobileWalletData.SenderId,
                WalletOperatorId = mobileWallet.WalletOperatorId,
                PaidToMobileNo = mobileWallet.PaidToMobileNo,
                ReceiverName = mobileWallet.ReceiverName,
                SendingAmount = mobileWallet.SendingAmount,
                ExtraFee = mobileWallet.ExtraFee,
                PaidFromModule = Module.Staff,
                ReceivingCurrency = mobileWallet.ReceivingCurrency,
                SendingCurrency = mobileWallet.SendingCurrency,

            };

            dbContext.MobileMoneyTransfer.Add(result);
            dbContext.SaveChanges();

            ////Update Reciepient
            //var recipient = dbContext.Recipients.Where(x => x.Id == result.RecipientId).FirstOrDefault();
            //if (recipient != null)
            //{
            //    SSenderTransactionHistory _senderTransactionHistoryServices = new SSenderTransactionHistory();
            //    var data = Common.AdminSession.SenderBankAccountDeposit;
            //    if (data != null)
            //    {
            //        recipient.City = data.ReceiverCity;
            //        recipient.Email = data.ReceiverEmail;
            //        recipient.PostalCode = data.ReceiverPostalCode;
            //        recipient.ReceiverName = data.AccountOwnerName;
            //        recipient.Street = data.ReceiverStreet;
            //        recipient.MobileNo = data.MobileNumber;
            //        recipient.IdentificationTypeId = data.IdenityCardId;
            //        recipient.IdentificationNumber = data.IdentityCardNumber;
            //        recipient.AccountNo = data.MobileNumber;
            //        recipient.MobileWalletProvider = data.walletId;
            //    }
            //    _recipientServices.UpdateReceipts(recipient);
            //}
        }

        internal void BanReceiverAndSender(int senderId, int recipientId)
        {
            banReciver(recipientId);
            DeactivateSender(senderId);
        }
        private void banReciver(int recipientId)
        {
            RecipientServices _recipientServices = new RecipientServices();
            var recipient = _recipientServices.Recipients().Where(x => x.Id == recipientId).FirstOrDefault();
            if (recipient != null)
            {
                recipient.IsBanned = true;
                _recipientServices.UpdateReceipts(recipient);
            }
        }
        private void DeactivateSender(int senderId)
        {
            ViewRegisteredFaxersServices _faxerServices = new ViewRegisteredFaxersServices();
            _faxerServices.DeleteFaxerInformation(senderId);
        }
        public void CreateCashPickUpDuplicateTransaction(int transactionId, FaxingNonCardTransaction cashPickUpData)
        {

            var CashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == transactionId).FirstOrDefault();
            var ReceiptNumber = Common.Common.GenerateCashPickUpReceiptNo(6);
            FaxingNonCardTransaction result = new FaxingNonCardTransaction()
            {
                FaxingStatus = FaxingStatus.Hold,
                IsComplianceNeededForTrans = true,
                PayingStaffId = Common.StaffSession.LoggedStaff.StaffId,
                ReceiptNumber = ReceiptNumber,
                MFCN = ReceiptNumber,
                ExchangeRate = cashPickUpData.ExchangeRate,
                FaxingFee = cashPickUpData.FaxingFee,
                MFRate = CashPickUp.MFRate,
                PaymentReference = CashPickUp.PaymentReference,
                ReceivingAmount = cashPickUpData.ReceivingAmount,
                ReceivingCountry = CashPickUp.ReceivingCountry,
                FaxingAmount = cashPickUpData.FaxingAmount,
                SendingCountry = CashPickUp.SendingCountry,
                TotalAmount = cashPickUpData.TotalAmount,
                RecipientId = cashPickUpData.RecipientId,
                TransactionDate = DateTime.Now,
                SenderPaymentMode = CashPickUp.SenderPaymentMode,
                SenderId = CashPickUp.SenderId,
                NonCardRecieverId = CashPickUp.NonCardRecieverId,
                RecipientIdenityCardNumber = cashPickUpData.RecipientIdenityCardNumber,
                RecipientIdentityCardId = cashPickUpData.RecipientIdentityCardId,

                ReceivingCurrency = cashPickUpData.ReceivingCurrency,
                SendingCurrency = cashPickUpData.SendingCurrency,
            };
            //Update Reciepient
            //var recipient = dbContext.Recipients.Where(x => x.Id == result.RecipientId).FirstOrDefault();
            //if (recipient != null)
            //{
            //    SSenderTransactionHistory _senderTransactionHistoryServices = new SSenderTransactionHistory();

            //    var data = Common.AdminSession.SenderBankAccountDeposit;
            //    if (data != null)
            //    {
            //        recipient.City = data.ReceiverCity;
            //        recipient.Email = data.ReceiverEmail;
            //        recipient.PostalCode = data.ReceiverPostalCode;
            //        recipient.ReceiverName = data.AccountOwnerName;
            //        recipient.Street = data.ReceiverStreet;
            //        recipient.MobileNo = data.MobileNumber;
            //        recipient.IdentificationTypeId = data.IdenityCardId;
            //        recipient.IdentificationNumber = data.IdentityCardNumber;
            //        recipient.AccountNo = data.MobileNumber;
            //    }
            //    _recipientServices.UpdateReceipts(recipient);
            //}

            dbContext.FaxingNonCardTransaction.Add(result);
            dbContext.SaveChanges();

        }
        public void AddRefundHistory(RefundHistory model)
        {
            dbContext.RefundHistory.Add(model);
            dbContext.SaveChanges();
        }
        public BankAccountDeposit UpdateRefundStatusOfBank(SenderTransactionHistoryViewModel vm)
        {
            var bankAccountDeposit = dbContext.BankAccountDeposit.Where(x => x.Id == vm.Id).FirstOrDefault();

            if (bankAccountDeposit.Status != BankDepositStatus.Cancel)
            {
                _senderForAllTransfer.CancelBankDepositTransaction(bankAccountDeposit.Id);
            }
            var reference = dbContext.SecureTradingApiResponseTransactionLog.
                    Where(x => x.requesttypedescription == "AUTH"
                          && x.orderreference == bankAccountDeposit.ReceiptNo && x.status == "Y"
                          && x.errorcode == "0").FirstOrDefault();
            if (reference != null)
            {
                decimal refundAmount = vm.RefundTransactionViewModel.RefundType
                                       == RefundType.FullRefund ? bankAccountDeposit.TotalAmount
                                       : vm.RefundTransactionViewModel.RefundingAmount;


                StripServices.RefundTransaction(reference.transactionreference,
                    refundAmount, vm.RefundTransactionViewModel.RefundType);
            }
            if (vm.RefundTransactionViewModel.RefundType == RefundType.FullRefund)
            {
                //Services.StripServices stripServices = new StripServices();

                bankAccountDeposit.Status = BankDepositStatus.FullRefund;
            }
            else if (vm.RefundTransactionViewModel.RefundType == RefundType.Partial)
            {
                bankAccountDeposit.Status = BankDepositStatus.PartailRefund;
            }

            dbContext.Entry(bankAccountDeposit).State = EntityState.Modified;
            dbContext.SaveChanges();

            SSenderBankAccountDeposit senderBankAccountDepositservices = new SSenderBankAccountDeposit(dbContext);

            return bankAccountDeposit;
        }
        public MobileMoneyTransfer UpdateRefundStatusOfMobile(SenderTransactionHistoryViewModel vm)
        {
            var mobileMoneyTransfer = dbContext.MobileMoneyTransfer.Where(x => x.Id == vm.Id).FirstOrDefault();
            if (mobileMoneyTransfer.Status != MobileMoneyTransferStatus.Cancel)
            {
                _senderForAllTransfer.CancelMobileWalletTransaction(mobileMoneyTransfer.Id);
            }
            var reference = dbContext.SecureTradingApiResponseTransactionLog.
                    Where(x => x.requesttypedescription == "AUTH"
                          && x.orderreference == mobileMoneyTransfer.ReceiptNo && x.status == "Y"
                          && x.errorcode == "0").FirstOrDefault();
            if (reference != null)
            {
                decimal refundAmount = vm.RefundTransactionViewModel.RefundType
                                       == RefundType.FullRefund ? mobileMoneyTransfer.TotalAmount
                                       : vm.RefundTransactionViewModel.RefundingAmount;


                StripServices.RefundTransaction(reference.transactionreference,
                    refundAmount, vm.RefundTransactionViewModel.RefundType);
            }

            if (vm.RefundTransactionViewModel.RefundType == RefundType.FullRefund)
            {
                mobileMoneyTransfer.Status = MobileMoneyTransferStatus.FullRefund;
            }
            else if (vm.RefundTransactionViewModel.RefundType == RefundType.Partial)
            {
                mobileMoneyTransfer.Status = MobileMoneyTransferStatus.PartailRefund;
            }
            dbContext.Entry(mobileMoneyTransfer).State = EntityState.Modified;
            dbContext.SaveChanges();

            return mobileMoneyTransfer;
        }
        public FaxingNonCardTransaction UpdateRefundStatusOfCashPickUP(SenderTransactionHistoryViewModel vm)
        {
            var cashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == vm.Id).FirstOrDefault();
            if (cashPickUp.FaxingStatus != FaxingStatus.Cancel)
            {
                _senderForAllTransfer.CancelCashPickUPTransaction(cashPickUp.Id);
            }

            var reference = dbContext.SecureTradingApiResponseTransactionLog.
               Where(x => x.requesttypedescription == "AUTH"
                     && x.orderreference == cashPickUp.ReceiptNumber && x.status == "Y"
                     && x.errorcode == "0").FirstOrDefault();
            if (reference != null)
            {
                decimal refundAmount = vm.RefundTransactionViewModel.RefundType
                                       == RefundType.FullRefund ? cashPickUp.TotalAmount
                                       : vm.RefundTransactionViewModel.RefundingAmount;


                StripServices.RefundTransaction(reference.transactionreference,
                    refundAmount, vm.RefundTransactionViewModel.RefundType);
            }

            if (vm.RefundTransactionViewModel.RefundType == RefundType.FullRefund)
            {
                cashPickUp.FaxingStatus = FaxingStatus.FullRefund;
            }
            else if (vm.RefundTransactionViewModel.RefundType == RefundType.Partial)
            {
                cashPickUp.FaxingStatus = FaxingStatus.PartailRefund;
            }
            dbContext.Entry(cashPickUp).State = EntityState.Modified;
            dbContext.SaveChanges();

            return cashPickUp;
        }

        public IQueryable<TransactionStatementNote> TransactionStatementNoteList()
        {
            var data = dbContext.TransactionStatementNote;
            return data;
        }

        internal void UpdateTransactionStatementNote(int transactionId)
        {

            var data = TransactionStatementNoteList().Where(x => x.TransactionId == transactionId).ToList();
            foreach (var item in data)
            {
                item.IsRead = true;
                dbContext.Entry<TransactionStatementNote>(item).State = EntityState.Modified;
                dbContext.SaveChanges();

            }
        }

        //public SenderTransactionActivityWithSenderDetails GetSenderTransactionAndDetails(TransactionServiceType transactionServiceType = TransactionServiceType.All,
        //    int senderId = 0, string DateRange = "", string SendingCountry = "", string ReceivingCountry = "", int PageSize = 0, int PageNum = 0
        //    , string searchString = ""
        //    , string SenderName = "", string ReceiverName = "", string Status = "", string PhoneNumber = "", string SenderStatus = "")
        //{
        //    SenderTransactionActivityWithSenderDetails vm = new SenderTransactionActivityWithSenderDetails();
        //    var transactionHistory = GetTransactionHistories((TransactionServiceType)transactionServiceType, senderId).TransactionHistoryList;
        //    vm.TotalNumberOfTransaction = transactionHistory.Count;
        //    vm.TotalAmountWithCurrency = transactionHistory.Select(x => x.TotalAmount).Sum() + "";
        //    vm.TotalFeePaidwithCurrency = transactionHistory.Select(x => x.Fee).Sum() + "";
        //    #region old

        //    //if (!string.IsNullOrEmpty(DateRange))
        //    //{

        //    //    var Date = DateRange.Split('-');
        //    //    string[] startDate = Date[0].Split('/');
        //    //    string[] endDate = Date[1].Split('/');
        //    //    var FromDate = new DateTime(int.Parse(startDate[2]), int.Parse(startDate[0]), int.Parse(startDate[1]));
        //    //    var ToDate = new DateTime(int.Parse(endDate[2]), int.Parse(endDate[0]), int.Parse(endDate[1]));// Convert.ToDateTime(Date[1]);
        //    //    transactionHistory = transactionHistory.Where(x => x.TransactionDate >= FromDate && x.TransactionDate <= ToDate).ToList();
        //    //}
        //    //if (!string.IsNullOrEmpty(SendingCountry))
        //    //{
        //    //    var Country = Common.Common.GetCountryName(SendingCountry);
        //    //    transactionHistory = transactionHistory.Where(x => x.FaxerCountry == Country).ToList();
        //    //}
        //    //if (!string.IsNullOrEmpty(ReceivingCountry))
        //    //{
        //    //    var Country = Common.Common.GetCountryName(ReceivingCountry);
        //    //    transactionHistory = transactionHistory.Where(x => x.ReceiverCountry == Country).ToList();
        //    //}
        //    //if (!string.IsNullOrEmpty(searchString))
        //    //{
        //    //    searchString = searchString.Trim();
        //    //    transactionHistory = transactionHistory.
        //    //        Where(x => x.TransactionIdentifier == searchString || x.PaymentReference == searchString).ToList();
        //    //}
        //    //if (!string.IsNullOrEmpty(SenderName))
        //    //{
        //    //    SenderName = SenderName.Trim();

        //    //    string Name = "";
        //    //    string[] name = SenderName.Split(' ');
        //    //    for (int i = 0; i < name.Length; i++)
        //    //    {
        //    //        if (!string.IsNullOrEmpty(name[i]))
        //    //        {
        //    //            Name = Name + name[i].Trim() + " ";
        //    //        }
        //    //    }
        //    //    Name = Name.Trim();
        //    //    transactionHistory = transactionHistory.Where(x => x.SenderName.ToLower().Contains(Name.ToLower())).ToList();
        //    //}
        //    //if (!string.IsNullOrEmpty(ReceiverName))
        //    //{

        //    //    ReceiverName = ReceiverName.Trim();
        //    //    transactionHistory = transactionHistory.Where(x => x.ReceiverName.ToLower().Contains(ReceiverName.ToLower())).ToList();

        //    //}
        //    //if (!string.IsNullOrEmpty(Status))
        //    //{
        //    //    Status = Status.Trim();
        //    //    transactionHistory = transactionHistory.Where(x => x.TransactionStatusForAdmin.ToLower().Contains(Status.ToLower())).ToList();
        //    //}
        //    //if (!string.IsNullOrEmpty(PhoneNumber))
        //    //{
        //    //    PhoneNumber = PhoneNumber.Trim();
        //    //    transactionHistory = transactionHistory.Where(x => x.SenderTelephoneNo.Contains(PhoneNumber)).ToList();
        //    //} 
        //    #endregion
        //    vm.SenderTransactionStatement = (from c in transactionHistory.OrderByDescending(x => x.TransactionDate).ToList()
        //                                     join d in dbContext.FaxerLogin on c.senderId equals d.FaxerId
        //                                     select new SenderTransactionActivityVm()
        //                                     {
        //                                         TransactionId = c.Id,
        //                                         SenderPhoneNumber = c.SenderTelephoneNo,
        //                                         Amount = c.SendingCurrencySymbol + " " + c.GrossAmount,
        //                                         DateTime = c.TransactionDate.ToString("dd/MM/yyyy"),
        //                                         Fee = c.SendingCurrencySymbol + " " + c.Fee,
        //                                         identifier = c.TransactionIdentifier,
        //                                         TransferMethod = Common.Common.GetEnumDescription(c.TransactionServiceType),
        //                                         TransferType = c.TransactionType,
        //                                         SenderId = c.senderId,
        //                                         ReceiverName = c.ReceiverName,
        //                                         ReceivingCountry = c.ReceiverCountry,
        //                                         SendingCountry = c.FaxerCountry,
        //                                         SenderName = c.SenderName,
        //                                         Status = c.StatusName,
        //                                         IsAbnormalTrans = c.IsAbnormalTransaction,
        //                                         Reference = c.Reference,
        //                                         IsAwaitForApproval = c.IsAwaitForApproval,
        //                                         IsAwaitForConfirmation = c.IsAwaitForConfirmation,
        //                                         TransactionServiceType = c.TransactionServiceType,
        //                                         TransactionStatusForAdmin = c.TransactionStatusForAdmin,
        //                                         TransactionTime = c.TransactionDate.ToString("HH:mm:ss"),
        //                                         TransactionPerformedBy = c.TransactionPerformedBy,
        //                                         AgentStaffId = c.AgentStaffId,
        //                                         IsDuplicatedTransaction = c.IsDuplicatedTransaction,
        //                                         DuplicatedTransactionReceiptNo = c.DuplicatedTransactionReceiptNo,
        //                                         ReInitializedReceiptNo = c.ReInitializedReceiptNo,
        //                                         IsReInitializedTransaction = c.IsReInitializedTransaction,
        //                                         ReInitializeStaffName = c.ReInitializeStaffName,
        //                                         ReInitializedDateTime = c.ReInitializedDateTime,
        //                                         IsSenderActive = d.IsActive,
        //                                         RecipentId = c.RecipientId
        //                                     }).ToPagedList(PageNum, PageSize);

        //    if (!string.IsNullOrEmpty(SenderStatus) && SenderStatus != "undefined")
        //    {
        //        if (SenderStatus == "1")
        //        {
        //            vm.SenderTransactionStatement = vm.SenderTransactionStatement.Where(x => x.IsSenderActive).ToPagedList(PageNum, PageSize);
        //        }
        //        else
        //        {
        //            vm.SenderTransactionStatement = vm.SenderTransactionStatement.Where(x => !x.IsSenderActive).ToPagedList(PageNum, PageSize);
        //        }
        //    }
        //    //vm.SenderTransactionStatement = senderStatement;
        //    if (senderId != 0)
        //    {
        //        var senderInfo = Common.Common.GetSenderInfo(senderId);
        //        vm.SenderCountry = senderInfo.Country;
        //        vm.SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;
        //        vm.SenderAccountNumber = senderInfo.AccountNo;
        //    }
        //    return vm;
        //}
        public SenderTransactionActivityWithSenderDetails GetSenderTransactionAndDetails(SenderTransactionSearchParamVm searchParamVm)
        {
            SenderTransactionActivityWithSenderDetails vm = new SenderTransactionActivityWithSenderDetails();
            //var transactionHistory = GetTransactionHistories((TransactionServiceType)searchParamVm.TransactionServiceType, searchParamVm.senderId).TransactionHistoryList;
            
            searchParamVm = GetTrimmedSearchParamVm(searchParamVm);

           // transactionHistory = SearchTransctionStatementByParam(searchParamVm, transactionHistory);
            var transactionHistory = GetTransactionHistoryOfSender(searchParamVm);
            if (transactionHistory.Count != 0)
            {
                vm.TotalNumberOfTransaction = transactionHistory.FirstOrDefault().TotalCount;
            }
            vm.TotalAmountWithCurrency = transactionHistory.Select(x => x.TotalAmount).Sum() + "";
            vm.TotalFeePaidwithCurrency = transactionHistory.Select(x => x.Fee).Sum() + "";
            vm.SenderTransactionStatement = (from c in transactionHistory.OrderByDescending(x => x.TransactionDate).ToList()
                                             join d in dbContext.FaxerLogin on c.senderId equals d.FaxerId into fl
                                             from d in fl.DefaultIfEmpty()
                                             select new SenderTransactionActivityVm()
                                             {
                                                 TransactionId = c.Id,
                                                 SenderPhoneNumber = c.SenderTelephoneNo,
                                                 Amount = c.SendingCurrencySymbol + " " + c.GrossAmount,
                                                 DateTime = c.TransactionDate.ToString("dd/MM/yyyy"),
                                                 Fee = c.SendingCurrencySymbol + " " + c.Fee,
                                                 identifier = c.TransactionIdentifier,
                                                 TransferMethod = Common.Common.GetEnumDescription(c.TransactionServiceType),
                                                 TransferType = c.TransactionType,
                                                 SenderId = c.senderId,
                                                 ReceiverName = c.ReceiverName,
                                                 ReceivingCountry = c.ReceiverCountry,
                                                 SendingCountry = c.FaxerCountry,
                                                 SenderName = c.SenderName,
                                                 Status = c.StatusName,
                                                 IsAbnormalTrans = c.IsAbnormalTransaction,
                                                 Reference = c.Reference,
                                                 IsAwaitForApproval = c.IsAwaitForApproval,
                                                 IsAwaitForConfirmation = c.IsAwaitForConfirmation,
                                                 TransactionServiceType = c.TransactionServiceType,
                                                 TransactionStatusForAdmin = c.TransactionStatusForAdmin,
                                                 TransactionTime = c.TransactionDate.ToString("HH:mm:ss"),
                                                 TransactionPerformedBy = c.TransactionPerformedBy,
                                                 AgentStaffId = c.AgentStaffId,
                                                 IsDuplicatedTransaction = c.IsDuplicatedTransaction,
                                                 DuplicatedTransactionReceiptNo = c.DuplicatedTransactionReceiptNo,
                                                 ReInitializedReceiptNo = c.ReInitializedReceiptNo,
                                                 IsReInitializedTransaction = c.IsReInitializedTransaction,
                                                 ReInitializeStaffName = c.ReInitializeStaffName,
                                                 ReInitializedDateTime = c.ReInitializedDateTime,
                                                 IsSenderActive = d != null ? d.IsActive : false,
                                                 RecipentId = c.RecipientId,
                                                 SenderEmail = c.SenderEmail,
                                                 TotalCount = c.TotalCount,
                                                 ReceivingCurrency = c.ReceivingCurrrency,
                                                 SendingCurrency = c.SendingCurrency,
                                                 IsManualApprovalNeeded = c.IsManualApprovalNeeded
                                             }).ToList();
            return vm;
        }
        public SenderTransactionSearchParamVm GetTrimmedSearchParamVm(SenderTransactionSearchParamVm searchParamVm)
        {
            string FromDate = "";
            string ToDate = "";
            if (!string.IsNullOrEmpty(searchParamVm.DateRange))
            {
                var Date = searchParamVm.DateRange.Split('-');
                FromDate = Date[0].ToDateTime().ToString("yyyy/MM/dd");
                ToDate = Date[1].ToDateTime().ToString("yyyy/MM/dd");
            }
            SenderTransactionSearchParamVm vm = new SenderTransactionSearchParamVm()
            {
                PageSize = searchParamVm.PageSize,
                PageNum = searchParamVm.PageNum,
                DateRange = searchParamVm.DateRange != null ? searchParamVm.DateRange : "",
                FromDate = FromDate,
                ToDate = ToDate,
                MFCode = searchParamVm.MFCode != null ? searchParamVm.MFCode.Trim() : "",
                PhoneNumber = searchParamVm.PhoneNumber != null ? searchParamVm.PhoneNumber.Trim() : "",
                ReceiverName = searchParamVm.ReceiverName != null ? searchParamVm.ReceiverName.Trim() : "",
                ReceivingCountry = searchParamVm.ReceivingCountry != null ? searchParamVm.ReceivingCountry.Trim() : "",
                ResponsiblePerson = searchParamVm.ResponsiblePerson != null ? searchParamVm.ResponsiblePerson.Trim() : "",
                SearchByStatus = searchParamVm.SearchByStatus != null ? searchParamVm.SearchByStatus.Trim() : "",
                searchString = searchParamVm.searchString != null ? searchParamVm.searchString.Trim() : "",
                SenderEmail = searchParamVm.SenderEmail != null ? searchParamVm.SenderEmail.Trim() : "",
                senderId = searchParamVm.senderId,
                SenderName = searchParamVm.SenderName != null ? searchParamVm.SenderName.Trim() : "",
                SendingCountry = searchParamVm.SendingCountry != null ? searchParamVm.SendingCountry.Trim() : "",
                SendingCurrency = searchParamVm.SendingCurrency != null ? searchParamVm.SendingCurrency.Trim() : "",
                Status = searchParamVm.Status != null ? searchParamVm.Status.Trim() : "",
                TransactionServiceType = searchParamVm.TransactionServiceType,
                TransactionWithAndWithoutFee = searchParamVm.TransactionWithAndWithoutFee != null ? searchParamVm.TransactionWithAndWithoutFee.Trim() : "",
                CurrentpageCount = searchParamVm.CurrentpageCount,
                IsBusiness = searchParamVm.IsBusiness
            };
            return vm;
        }

        private List<SenderTransactionHistoryList> GetTransactionHistoryOfSender(SenderTransactionSearchParamVm searchParam)
        {
            return dbContext.Sp_GetTransactionStatementOfSender(searchParam);
        }
        private List<SenderTransactionHistoryList> SearchTransctionStatementByParam(SenderTransactionSearchParamVm searchParamVm, List<SenderTransactionHistoryList> transactionHistory)
        {
            if (!string.IsNullOrEmpty(searchParamVm.DateRange))
            {
                var Date = searchParamVm.DateRange.Split('-');
                var FromDate = Date[0].ToDateTime().Date;
                var ToDate = Date[1].ToDateTime().Date;
                transactionHistory = transactionHistory.Where(x => x.TransactionDate.Date >= FromDate
                                                            && x.TransactionDate.Date <= ToDate).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.SendingCountry))
            {
                var Country = Common.Common.GetCountryName(searchParamVm.SendingCountry);
                var Currency = Common.Common.GetCurrencyCode(searchParamVm.SendingCountry);
                transactionHistory = transactionHistory.Where(x => x.FaxerCountry == Country).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.ReceivingCountry))
            {
                var Country = Common.Common.GetCountryName(searchParamVm.ReceivingCountry);
                transactionHistory = transactionHistory.Where(x => x.ReceiverCountry == Country).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.SenderEmail))
            {
                searchParamVm.SenderEmail = searchParamVm.SenderEmail.Trim().ToLower();
                transactionHistory = transactionHistory.Where(x => x.SenderEmail.ToLower() == searchParamVm.SenderEmail).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.searchString))
            {
                searchParamVm.searchString = searchParamVm.searchString.Trim();
                transactionHistory = transactionHistory.
                    Where(x => x.TransactionIdentifier == searchParamVm.searchString || x.PaymentReference == searchParamVm.searchString).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.SenderName))
            {
                searchParamVm.SenderName = searchParamVm.SenderName.Trim();

                string Name = "";
                string[] name = searchParamVm.SenderName.Split(' ');
                for (int i = 0; i < name.Length; i++)
                {
                    if (!string.IsNullOrEmpty(name[i]))
                    {
                        Name = Name + name[i].Trim() + " ";
                    }
                }
                Name = Name.Trim();
                transactionHistory = transactionHistory.Where(x => x.SenderName.ToLower().Contains(Name.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.ReceiverName))
            {
                searchParamVm.ReceiverName = searchParamVm.ReceiverName.Trim();
                transactionHistory = transactionHistory.Where(x => x.ReceiverName.ToLower().Contains(searchParamVm.ReceiverName.ToLower())).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.Status))
            {
                searchParamVm.Status = searchParamVm.Status.Trim();
                transactionHistory = transactionHistory.Where(x => x.TransactionStatusForAdmin.ToLower().Contains(searchParamVm.Status.ToLower())).ToList();

            }
            if (!string.IsNullOrEmpty(searchParamVm.PhoneNumber))
            {
                searchParamVm.PhoneNumber = searchParamVm.PhoneNumber.Trim();
                transactionHistory = transactionHistory.Where(x => x.SenderTelephoneNo.Contains(searchParamVm.PhoneNumber)).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.TransactionWithAndWithoutFee))
            {
                if (searchParamVm.TransactionWithAndWithoutFee == "0")
                {
                    transactionHistory = transactionHistory.Where(x => x.Fee != 0).ToList();
                }
                else
                {
                    transactionHistory = transactionHistory.Where(x => x.Fee == 0).ToList();
                }
            }
            if (!string.IsNullOrEmpty(searchParamVm.SendingCurrency) && searchParamVm.SendingCurrency != "undefined")
            {
                transactionHistory = transactionHistory.Where(x => x.SendingCurrency == searchParamVm.SendingCurrency).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.ResponsiblePerson) && searchParamVm.ResponsiblePerson != "undefined")
            {
                transactionHistory = transactionHistory.Where(x => x.TransactionPerformedBy.ToLower().Contains(searchParamVm.ResponsiblePerson)).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.MFCode) && searchParamVm.MFCode != "undefined")
            {
                searchParamVm.MFCode = searchParamVm.MFCode.Trim().ToLower();
                transactionHistory = transactionHistory.Where(x => x.FaxerAccountNo.ToLower().Contains(searchParamVm.MFCode)).ToList();
            }
            if (!string.IsNullOrEmpty(searchParamVm.SearchByStatus) && searchParamVm.SearchByStatus != "undefined")
            {
                searchParamVm.SearchByStatus = searchParamVm.SearchByStatus.Trim().ToLower();
                transactionHistory = transactionHistory.Where(x => x.TransactionStatusForAdmin.ToLower() == searchParamVm.SearchByStatus).ToList();
            }

            TotalAmountWithCurrency = transactionHistory.Select(x => x.TotalAmount).Sum() + "";
            TotalFeePaidwithCurrency = transactionHistory.Select(x => x.Fee).Sum() + "";
            return transactionHistory;
        }

        public List<TransactionDetailsForExpotExcelViewModel> GetTransactionDetailsForExcel(SenderTransactionSearchParamVm searchParamVm)
        {

            searchParamVm = GetTrimmedSearchParamVm(searchParamVm);
            var transactionHistory = GetTransactionHistoryOfSender(searchParamVm);
            var transationDetails = (from c in transactionHistory
                                     select new TransactionDetailsForExpotExcelViewModel()
                                     {
                                         Amount = c.TotalAmount,
                                         Fee = c.Fee,
                                         DateTime = c.TransactionDate.ToString(),
                                         Identifier = c.TransactionIdentifier,
                                         Receiver = c.ReceiverName,
                                         ReceivingCountry = c.ReceiverCountry,
                                         Responsible = c.TransactionPerformedBy,
                                         Sender = c.SenderName,
                                         SenderPhoneNo = c.SenderTelephoneNo,
                                         SendingCountry = c.FaxerCountry,
                                         Status = c.StatusName,
                                     }).ToList();
            return transationDetails;
        }
        internal void UpdateStatusOfMobileTransfer(MobileMoneyTransfer data)
        {
            data.Status = MobileMoneyTransferStatus.Paid;
            dbContext.Entry(data).State = EntityState.Modified;
            dbContext.SaveChanges();

            SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            _senderMobileMoneyTransferServices.SendEmailAndSms(data);

        }
        public List<TransactionStatementNoteViewModel> TransactionStatementNote(int transactionId)
        {
            var data = TransactionStatementNoteList().Where(x => x.TransactionId == transactionId).ToList();
            var result = (from c in data
                          join d in dbContext.StaffInformation on c.CreatedBy equals d.Id
                          select new TransactionStatementNoteViewModel()
                          {
                              Id = c.Id,
                              TransactionId = c.TransactionId,
                              CreatedBy = c.CreatedBy,
                              CreatedByName = d.FirstName + " " + d.MiddleName + " " + d.LastName,
                              TransactionMethod = c.TransactionMethod,
                              CreatedDate = c.CreatedDateAndTime.ToFormatedString(),
                              CreatedTime = c.CreatedDateAndTime.ToString("HH:mm"),
                              Note = c.Note,
                              TransactionMethodName = c.TransactionMethod.ToString(),
                              IsRead = c.IsRead
                          }).ToList();
            return result;
        }
        public List<SenderTransactionHistoryList> GetCashPickUpDetails(int senderId, int year = 0, int month = 0)
        {
            var data = dbContext.FaxingNonCardTransaction.ToList();

            //if (!IsAdmin)
            //{
            //    data = data.Where(x => x.NonCardReciever.FaxerID == senderId).ToList();

            //}
            if (senderId != 0)
            {
                data = data.Where(x => x.NonCardReciever.FaxerID == senderId).ToList();

            }
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year).ToList();
            }
            if (month != 0)
            {
                data = data.Where(x => x.TransactionDate.Month == month).ToList();
            }
            var senderInfo = GetFaxerInformation(senderId);
            string senderAccountNo = "";
            string senderCountry = "";
            string senderFullName = "";
            if (senderInfo != null)
            {
                senderAccountNo = senderInfo.AccountNo;
                senderCountry = Common.Common.GetCountryName(senderInfo.Country);
                senderFullName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;

            }

            var result = (from c in data
                          join d in dbContext.FaxerInformation on c.NonCardReciever.FaxerID equals d.Id
                          join SendingCountry in dbContext.Country on c.SendingCountry equals SendingCountry.CountryCode
                          join ReceivingCountry in dbContext.Country on c.ReceivingCountry equals ReceivingCountry.CountryCode
                          join creditDebitCardInfo in dbContext.CardTopUpCreditDebitInformation.Where(x => x.TransferType == (int)TransferType.CashPickup)
                          on c.Id equals creditDebitCardInfo.CardTransactionId into cardInfo
                          from creditDebitCardInfo in cardInfo.DefaultIfEmpty()
                          join ReInTrans in dbContext.ReinitializeTransaction on c.ReceiptNumber equals ReInTrans.NewReceiptNo
                          into ReInTransaction
                          from ReInTrans in ReInTransaction.DefaultIfEmpty()
                          select new SenderTransactionHistoryList()
                          {
                              Id = c.Id,
                              AccountNumber = "",
                              Date = c.TransactionDate.ToString("MMMM dd, yyyy"),
                              Fee = c.FaxingFee,
                              SenderTelephoneNo = d.PhoneNumber,
                              GrossAmount = c.FaxingAmount,
                              ReceiverName = c.NonCardReciever.FullName,
                              Reference = c.MFCN,
                              Status = c.FaxingStatus,
                              StatusName = Common.Common.GetEnumDescription(c.FaxingStatus),
                              TransactionType = Common.Common.GetEnumDescription(TransactionServiceType.CashPickUp),
                              ReceivingCurrencySymbol = c.ReceivingCurrency == null ? ReceivingCountry.CurrencySymbol : Common.Common.GetCurrencySymbolByCurrency( c.ReceivingCurrency),
                              ReceivingCurrrency = c.ReceivingCurrency == null ? ReceivingCountry.Currency :c.ReceivingCurrency,
                              SendingCurrency = c.SendingCurrency == null ? SendingCountry.Currency : c.SendingCurrency,
                              SendingCurrencySymbol = c.SendingCurrency == null ? SendingCountry.CurrencySymbol : Common.Common.GetCurrencySymbolByCurrency(c.SendingCurrency),
                              ReceivingAmount = c.ReceivingAmount,
                              TotalAmount = c.TotalAmount,
                              SenderPaymentMode = c.SenderPaymentMode,
                              CardNumber = creditDebitCardInfo == null ? "" : creditDebitCardInfo.CardNumber,

                              //PaymentMethod = SSenderForAllTransfer.GetCreditCardLastDigit(c.SenderPaymentMode, TransferType.CashPickup, c.Id),
                              ReceiverCity = c.NonCardReciever.City,
                              ReceiverCountry = ReceivingCountry.CountryName,
                              ExchangeRate = c.ExchangeRate,
                              TransactionServiceType = TransactionServiceType.CashPickUp,
                              TransactionDate = c.TransactionDate,
                              TransactionIdentifier = c.ReceiptNumber,
                              FaxerAccountNo = d.AccountNo,
                              FaxerCountry = SendingCountry.CountryName,
                              SenderName = d.FirstName.Trim() + " " + (string.IsNullOrEmpty(d.MiddleName) == true ? "" : d.MiddleName.Trim() + " ") + d.LastName.Trim(),
                              senderId = c.NonCardReciever.FaxerID,
                              IsBusiness = d.IsBusiness,
                              PaymentReference = c.PaymentReference,
                              IsAwaitForApproval = c.IsComplianceApproved == true ? false : c.IsComplianceNeededForTrans,
                              //TransactionStatusForAdmin = GetCashPickUpStatusName(c.FaxingStatus),
                              PaidFromModule = (Module)(int)c.OperatingUserType,
                              TransferReference = c.TransferReference,
                              ApiService = c.Apiservice.ToString(),
                              RecipientId = c.RecipientId,
                              ReInitializedReceiptNo = ReInTrans == null ? "" : ReInTrans.ReceiptNo,
                              IsReInitializedTransaction = ReInTrans == null ? false : true,
                              ReInitializeStaffName = ReInTrans == null ? "" : ReInTrans.CreatedByName,
                              ReInitializedDateTime = ReInTrans == null ? "" : ReInTrans.Date.ToString(),
                              SenderEmail = d.Email ?? "",
                          }).ToList();
            return result;

        }

        public string GetCashPickUpStatusName(FaxingStatus faxingStatus)
        {

            string status = "";
            switch (faxingStatus)
            {
                case FaxingStatus.NotReceived:
                    status = "Not Received";
                    break;
                case FaxingStatus.Received:
                    status = "Received";
                    break;
                case FaxingStatus.Cancel:
                    status = "Cancelled";
                    break;
                case FaxingStatus.Refund:
                    status = "Refunded";
                    break;
                case FaxingStatus.Hold:
                    status = "On Hold";
                    break;
                case FaxingStatus.Completed:
                    status = "Paid";
                    break;
                case FaxingStatus.PaymentPending:
                    status = "Payment Pending";
                    break;
                case FaxingStatus.IdCheckInProgress:
                    status = "In Progress (ID Check)";
                    break;
                case FaxingStatus.PendingBankdepositConfirmtaion:
                    status = "In Progress (Bank Payment Confirmation)";
                    break;
                default:
                    break;
            }
            return status;
        }

        public List<SenderTransactionHistoryList> GetServicePaymentDetails(int senderId, int year = 0, int month = 0)
        {
            var data = dbContext.FaxerMerchantPaymentTransaction.ToList();

            //if (!IsAdmin)
            //{
            //    data = data.Where(x => x.SenderKiiPayBusinessPaymentInformation.SenderInformationId == senderId).ToList();

            //}
            if (senderId != 0)
            {
                data = data.Where(x => x.SenderKiiPayBusinessPaymentInformation.SenderInformationId == senderId).ToList();

            }

            if (year != 0)
            {
                data = data.Where(x => x.PaymentDate.Year == year).ToList();

            }
            if (month != 0)
            {
                data = data.Where(x => x.PaymentDate.Month == month).ToList();
            }

            var senderInfo = GetFaxerInformation(senderId);
            string senderAccountNo = "";
            string senderCountry = "";
            string senderFullName = "";
            if (senderInfo != null)
            {
                senderAccountNo = senderInfo.AccountNo;
                senderCountry = Common.Common.GetCountryName(senderInfo.Country);
                senderFullName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;

            }


            var result = (from c in data
                          join d in dbContext.FaxerInformation on c.SenderKiiPayBusinessPaymentInformation.SenderInformationId equals d.Id
                          join creditDebitCardInfo in dbContext.CardTopUpCreditDebitInformation.Where(x => x.TransferType == (int)TransferType.PayForServices)
                          on c.Id equals creditDebitCardInfo.CardTransactionId into cardInfo
                          from creditDebitCardInfo in cardInfo.DefaultIfEmpty()
                          join sendingCountry in dbContext.Country on c.SendingCountry equals sendingCountry.CountryCode
                          join receivingCountry in dbContext.Country on c.SendingCountry equals receivingCountry.CountryCode
                          select new SenderTransactionHistoryList()
                          {
                              Id = c.Id,
                              AccountNumber = c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessMobileNo,
                              Date = c.PaymentDate.ToString("dd/MM/yyyy"),
                              SenderTelephoneNo = d.PhoneNumber,
                              Fee = c.FaxingFee,
                              GrossAmount = c.PaymentAmount,
                              ReceiverName = c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessName,
                              Reference = c.PaymentReference,
                              Status = FaxingStatus.Completed,
                              StatusName = Common.Common.GetEnumDescription(FaxingStatus.Completed),
                              TransactionType = Common.Common.GetEnumDescription(TransactionServiceType.ServicePayment),
                              ReceivingCurrencySymbol = receivingCountry.CurrencySymbol,
                              ReceivingCurrrency = receivingCountry.Currency,
                              SendingCurrency = sendingCountry.Currency,
                              SendingCurrencySymbol = sendingCountry.CurrencySymbol,
                              ReceivingAmount = c.ReceivingAmount,
                              TotalAmount = c.TotalAmount,
                              SenderPaymentMode = c.SenderPaymentMode,
                              CardNumber = creditDebitCardInfo == null ? "" : creditDebitCardInfo.CardNumber,

                              //PaymentMethod = SSenderForAllTransfer.GetCreditCardLastDigit(c.SenderPaymentMode, TransferType.PayForServices, c.Id),
                              ReceiverCity = c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessOperationCity,
                              ReceiverCountry = Common.Common.GetCountryName(c.SenderKiiPayBusinessPaymentInformation.KiiPayBusinessInformation.BusinessCountry),
                              ExchangeRate = c.ExchangeRate,
                              TransactionServiceType = TransactionServiceType.ServicePayment,
                              TransactionDate = c.PaymentDate,
                              TransactionIdentifier = c.ReceiptNumber,
                              FaxerAccountNo = d.AccountNo,
                              FaxerCountry = sendingCountry.CountryName,
                              SenderName = d.FirstName.Trim() + " " + (string.IsNullOrEmpty(d.MiddleName) == true ? "" : d.MiddleName.Trim() + " ") + d.LastName.Trim(),
                              senderId = c.SenderKiiPayBusinessPaymentInformation.SenderInformationId,
                              IsBusiness = d.IsBusiness,
                              SenderEmail = d.Email ?? "",

                          }).ToList();
            return result;
        }


        public List<SenderTransactionHistoryList> GetBillPaymentDetails(int SenderId, int year = 0, int month = 0)
        {

            var data = dbContext.PayBill.Where(x => x.Module == Module.Faxer).ToList();
            //if (!IsAdmin)
            //{
            //    data = data.Where(x => x.PayerId == SenderId).ToList();
            //}
            if (SenderId != 0)
            {

                data = data.Where(x => x.PayerId == SenderId).ToList();
            }
            if (year != 0)
            {
                data = data.Where(x => x.PaymentDate.Year == year).ToList();
            }
            if (month != 0)
            {
                data = data.Where(x => x.PaymentDate.Month == month).ToList();
            }
            var senderInfo = GetFaxerInformation(SenderId);
            string senderAccountNo = "";
            string senderCountry = "";
            string senderFullName = "";
            if (senderInfo != null)
            {
                senderAccountNo = senderInfo.AccountNo;
                senderCountry = Common.Common.GetCountryName(senderInfo.Country);
                senderFullName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;
            }

            var result = (from c in data
                          join d in dbContext.FaxerInformation on c.PayerId equals d.Id
                          where c.Module == Module.Faxer
                          join creditDebitCardInfo in dbContext.CardTopUpCreditDebitInformation.Where(x => x.TransferType == (int)TransferType.BillPayment)
            on c.Id equals creditDebitCardInfo.CardTransactionId into cardInfo
                          from creditDebitCardInfo in cardInfo.DefaultIfEmpty()
                          join sendingCountry in dbContext.Country on c.PayerCountry equals sendingCountry.CountryCode
                          join receivingCountry in dbContext.Country on c.SupplierCountry equals receivingCountry.CountryCode
                          select new SenderTransactionHistoryList()
                          {
                              Id = c.Id,
                              AccountNumber = c.Supplier.KiiPayBusinessInformation.BusinessMobileNo,
                              Date = c.PaymentDate.ToString("dd/MM/yyyy"),
                              SenderTelephoneNo = d.PhoneNumber,
                              Fee = c.Fee,
                              GrossAmount = c.Amount,
                              ReceiverName = c.Supplier.KiiPayBusinessInformation.BusinessName,
                              BillReferenceNo = c.RefCode,
                              BillNo = c.BillNo,
                              Status = FaxingStatus.Completed,
                              StatusName = Common.Common.GetEnumDescription(FaxingStatus.Completed),
                              TransactionType = Common.Common.GetEnumDescription(TransactionServiceType.BillPayment),
                              ReceivingCurrencySymbol = receivingCountry.CurrencySymbol,
                              ReceivingCurrrency = receivingCountry.Currency,
                              SendingCurrency = sendingCountry.Currency,
                              SendingCurrencySymbol = sendingCountry.Currency,
                              ReceivingAmount = c.Amount,
                              TotalAmount = c.SendingAmount,
                              SenderPaymentMode = c.SenderPaymentMode,
                              CardNumber = creditDebitCardInfo == null ? "" : creditDebitCardInfo.CardNumber,

                              //PaymentMethod = SSenderForAllTransfer.GetCreditCardLastDigit(c.SenderPaymentMode, TransferType.BillPayment, c.Id),
                              ReceiverCity = c.Supplier.KiiPayBusinessInformation.BusinessOperationCity,
                              ReceiverCountry = Common.Common.GetCountryName(c.Supplier.KiiPayBusinessInformation.BusinessCountry),
                              ExchangeRate = c.ExchangeRate,
                              TransactionServiceType = TransactionServiceType.BillPayment,
                              TransactionDate = c.PaymentDate,
                              Reference = c.RefCode,
                              TransactionIdentifier = c.ReceiptNo,
                              BillpaymentCode = "BP",
                              FaxerAccountNo = d.AccountNo,
                              FaxerCountry = sendingCountry.CountryName,
                              SenderName = d.FirstName.Trim() + " " + (string.IsNullOrEmpty(d.MiddleName) == true ? "" : d.MiddleName.Trim() + " ") + d.LastName.Trim(),
                              senderId = c.PayerId,
                              IsBusiness = d.IsBusiness,
                              SenderCountryCode = c.SupplierCountry,
                              SenderEmail = d.Email ?? "",
                          }).ToList();

            var topUpData = dbContext.TopUpToSupplier.Where(x => x.PaymentModule == Module.Faxer).ToList();
            //if (!IsAdmin)
            //{
            //    topUpData = topUpData.Where(x => x.PayerId == SenderId).ToList();
            //}
            if (SenderId != 0)
            {

                topUpData = topUpData.Where(x => x.PayerId == SenderId).ToList();
            }
            if (year != 0)
            {
                topUpData = topUpData.Where(x => x.PaymentDate.Year == year).ToList();
            }
            if (month != 0)
            {
                topUpData = topUpData.Where(x => x.PaymentDate.Month == month).ToList();
            }

            var topUp = (from c in topUpData
                         join d in dbContext.Suppliers on c.SuplierId equals d.Id
                         join e in dbContext.FaxerInformation on c.PayerId equals e.Id
                         where c.PaymentModule == Module.Faxer
                         join creditDebitCardInfo in dbContext.CardTopUpCreditDebitInformation.Where(x => x.TransferType == (int)TransferType.BillPayment)
                         on c.Id equals creditDebitCardInfo.CardTransactionId into cardInfo
                         from creditDebitCardInfo in cardInfo.DefaultIfEmpty()
                         join sendingCountry in dbContext.Country on c.SupplierCountry equals sendingCountry.CountryCode
                         join receivingCountry in dbContext.Country on c.PayingCountry equals receivingCountry.CountryCode

                         select new SenderTransactionHistoryList()
                         {
                             Id = c.Id,
                             AccountNumber = c.WalletNo,
                             Date = c.PaymentDate.ToString("dd/MM/yyyy"),
                             Fee = c.Fee,
                             GrossAmount = c.TotalAmount,
                             ReceiverName = d.KiiPayBusinessInformation.BusinessName,
                             BillReferenceNo = c.SupplierAccountNo,
                             //BillNo = c.BillNo,
                             Status = FaxingStatus.Completed,
                             StatusName = Common.Common.GetEnumDescription(FaxingStatus.Completed),
                             TransactionType = Common.Common.GetEnumDescription(TransactionServiceType.BillPayment),
                             ReceivingCurrencySymbol = receivingCountry.CurrencySymbol,
                             ReceivingCurrrency = receivingCountry.Currency,
                             SendingCurrency = sendingCountry.Currency,
                             SendingCurrencySymbol = sendingCountry.CurrencySymbol,
                             ReceivingAmount = c.ReceivingAmount,
                             TotalAmount = c.SendingAmount,
                             SenderPaymentMode = c.SenderPaymentMode,
                             CardNumber = creditDebitCardInfo == null ? "" : creditDebitCardInfo.CardNumber,

                             //PaymentMethod = SSenderForAllTransfer.GetCreditCardLastDigit(c.SenderPaymentMode, TransferType.BillPayment, c.Id),
                             ReceiverCity = d.KiiPayBusinessInformation.BusinessOperationCity,
                             ReceiverCountry = Common.Common.GetCountryName(d.KiiPayBusinessInformation.BusinessCountry),
                             ExchangeRate = c.EcxhangeRate,
                             TransactionServiceType = TransactionServiceType.BillPayment,
                             TransactionDate = c.PaymentDate,
                             Reference = c.SupplierAccountNo,
                             TransactionIdentifier = c.ReceiptNo,
                             BillpaymentCode = "TP",
                             FaxerAccountNo = e.AccountNo,
                             FaxerCountry = sendingCountry.CountryName,
                             SenderName = e.FirstName.Trim() + " " + (string.IsNullOrEmpty(e.MiddleName) == true ? "" : e.MiddleName.Trim() + " ") + e.LastName.Trim(),
                             senderId = c.PayerId,
                             IsBusiness = e.IsBusiness,
                             SenderCountryCode = c.SupplierCountry
                         }).ToList();


            return result.Concat(topUp).ToList();

        }

        public List<SenderTransactionHistoryList> GetMobileTransferDetails(int SenderId = 0, int year = 0, int month = 0)
        {

            List<SenderTransactionHistoryList> result = new List<SenderTransactionHistoryList>();

            var data = dbContext.MobileMoneyTransfer as IQueryable<MobileMoneyTransfer>;
            //if (!IsAdmin)
            //{
            //    data = data.Where(x => x.SenderId == SenderId);

            //}
            if (SenderId != 0)
            {

                data = data.Where(x => x.SenderId == SenderId);
            }
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year);
            }

            if (month != 0)
            {
                data = data.Where(x => x.TransactionDate.Month == month);

            }

            //var senderInfo = GetFaxerInformation(SenderId);
            //string senderAccountNo = "";
            //string senderCountry = "";
            //string senderFullName = "";
            //if (senderInfo != null)
            //{
            //    senderAccountNo = senderInfo.AccountNo;
            //    senderCountry = Common.Common.GetCountryName(senderInfo.Country);
            //    senderFullName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;
            //}


            result = (from c in data.ToList()
                      join d in dbContext.FaxerInformation on c.SenderId equals d.Id
                      join SendingCountry in dbContext.Country on c.SendingCountry equals SendingCountry.CountryCode
                      join ReceivingCountry in dbContext.Country on c.ReceivingCountry equals ReceivingCountry.CountryCode
                      join creditDebitCardInfo in dbContext.CardTopUpCreditDebitInformation.Where(x => x.TransferType == (int)TransferType.MobileTransfer)
                       on c.Id equals creditDebitCardInfo.CardTransactionId into cardInfo
                      from creditDebitCardInfo in cardInfo.DefaultIfEmpty()
                      join wallet in dbContext.MobileWalletOperator on c.WalletOperatorId equals wallet.Id into walletInfo
                      from wallet in walletInfo.DefaultIfEmpty()
                      join ReInTrans in dbContext.ReinitializeTransaction on c.ReceiptNo equals ReInTrans.NewReceiptNo
                          into ReInTransaction
                      from ReInTrans in ReInTransaction.DefaultIfEmpty()
                      select new SenderTransactionHistoryList()
                      {
                          Id = c.Id,
                          AccountNumber = c.PaidToMobileNo,
                          SenderTelephoneNo = d.PhoneNumber,
                          ReceiverName = c.ReceiverName == null ? "No Name" : c.ReceiverName,
                          ReceiverCity = c.ReceiverCity,
                          ReceiverCountry = ReceivingCountry.CountryName,
                          Fee = c.Fee,
                          GrossAmount = c.SendingAmount,
                          Status = FaxingStatus.Completed,
                          statusOfMobileWallet = c.Status,
                          StatusofMobileTransfer = c.Status,
                          StatusName = Common.Common.GetEnumDescription(c.Status),
                          TransactionType = Common.Common.GetEnumDescription(TransactionServiceType.MobileWallet),
                          ReceivingCurrencySymbol = c.ReceivingCurrency ==  null ? ReceivingCountry.CurrencySymbol : Common.Common.GetCurrencySymbolByCurrency(c.ReceivingCurrency),
                          ReceivingCurrrency = c.ReceivingCurrency == null ?  ReceivingCountry.Currency : c.ReceivingCurrency,
                          SendingCurrency = c.SendingCurrency == null ?  SendingCountry.Currency : c.SendingCurrency,
                          SendingCurrencySymbol = c.SendingCurrency == null ? SendingCountry.CurrencySymbol : Common.Common.GetCurrencySymbolByCurrency(c.SendingCurrency),
                          ReceivingAmount = c.ReceivingAmount,
                          TotalAmount = c.TotalAmount,
                          SenderPaymentMode = c.SenderPaymentMode,
                          CardNumber = creditDebitCardInfo == null ? "" : creditDebitCardInfo.CardNumber,

                          //PaymentMethod = SSenderForAllTransfer.GetCreditCardLastDigit(c.SenderPaymentMode, TransferType.MobileTransfer, c.Id),
                          ExchangeRate = c.ExchangeRate,
                          Date = c.TransactionDate.ToString("MMMM dd, yyyy"),
                          TransactionDate = c.TransactionDate,
                          TransactionServiceType = TransactionServiceType.MobileWallet,
                          Reference = c.PaymentReference,
                          TransactionIdentifier = c.ReceiptNo,
                          WalletName = wallet == null ? "" : wallet.Name,  //Common.Common.GetMobileWalletInfo(c.WalletOperatorId).Name,
                          FaxerAccountNo = d.AccountNo,
                          FaxerCountry = SendingCountry.CountryName,
                          SenderEmail = d.Email ?? "",
                          SenderName =
                          (string.IsNullOrEmpty(d.FirstName) == true ? "" : d.FirstName.Trim() + " ")
                          + " " + (string.IsNullOrEmpty(d.MiddleName) == true ? "" : d.MiddleName.Trim() + " ") +
                           (string.IsNullOrEmpty(d.LastName) == true ? "" : d.LastName.Trim() + " "),
                          senderId = c.SenderId,
                          IsBusiness = d.IsBusiness,
                          IsAbnormalTransaction = c.Status == MobileMoneyTransferStatus.Abnormal ? true : false,
                          IsAwaitForApproval = c.IsComplianceApproved == true ? false : c.IsComplianceNeededForTrans,
                          //TransactionStatusForAdmin = GetMobileTransferStatusName(c.Status),
                          PaidFromModule = c.PaidFromModule,
                          AgentStaffId = c.PayingStaffId,
                          ApiService = c.Apiservice.ToString(),
                          TransferReference = c.TransferReference,
                          RecipientId = c.RecipientId,
                          SenderCountryCode = c.SendingCountry,
                          ReInitializedReceiptNo = ReInTrans == null ? "" : ReInTrans.ReceiptNo,
                          IsReInitializedTransaction = ReInTrans == null ? false : true,
                          ReInitializeStaffName = ReInTrans == null ? "" : ReInTrans.CreatedByName,
                          ReInitializedDateTime = ReInTrans == null ? "" : ReInTrans.Date.ToString(),


                      }).GroupBy(x => x.TransactionIdentifier).Select(x => x.FirstOrDefault()).ToList();
            return result;
        }

        public string GetMobileTransferStatusName(MobileMoneyTransferStatus moneyTransferStatus)
        {

            string status = "";
            switch (moneyTransferStatus)
            {
                case MobileMoneyTransferStatus.Failed:
                    status = "Failed";
                    break;
                case MobileMoneyTransferStatus.InProgress:
                    status = "In Progress";
                    break;
                case MobileMoneyTransferStatus.Paid:
                    status = "Paid";
                    break;
                case MobileMoneyTransferStatus.Cancel:
                    status = "Cancelled";
                    break;
                case MobileMoneyTransferStatus.PaymentPending:
                    status = "Payment Pending";
                    break;
                case MobileMoneyTransferStatus.IdCheckInProgress:
                    status = "In Progress (ID Check)";
                    break;
                case MobileMoneyTransferStatus.PendingBankdepositConfirmtaion:
                    status = "In Progress (MoneyFex Bank Deposit) ";
                    break;
                case MobileMoneyTransferStatus.Abnormal:
                    status = "Abnormal";
                    break;
                case MobileMoneyTransferStatus.Held:
                    status = "On Hold";
                    break;
                default:
                    break;
            }

            return status;
        }

        public List<SenderTransactionHistoryList> GetBankDepositDetails(int SenderId, int year = 0, int month = 0)
        {
            List<SenderTransactionHistoryList> result = new List<SenderTransactionHistoryList>();

            var data = dbContext.BankAccountDeposit as IQueryable<BankAccountDeposit>;
            //if (!IsAdmin)
            //{
            //    data = data.Where(x => x.SenderId == SenderId);
            //}
            if (SenderId != 0)
            {

                data = data.Where(x => x.SenderId == SenderId);
            }
            if (year != 0)
            {
                data = data.Where(x => x.TransactionDate.Year == year);
            }
            if (month != 0)
            {
                data = data.Where(x => x.TransactionDate.Month == month);
            }

            //var senderInfo = GetFaxerInformation(SenderId);
            //string senderAccountNo = "";
            //string senderCountry = "";
            //string senderFullName = "";
            //if (senderInfo != null)
            //{
            //    senderAccountNo = senderInfo.AccountNo;
            //    senderCountry = Common.Common.GetCountryName(senderInfo.Country);
            //    senderFullName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;
            //}

            result = (from c in data.ToList()
                      join d in dbContext.FaxerInformation on c.SenderId equals d.Id
                      join SendingCountry in dbContext.Country on c.SendingCountry equals SendingCountry.CountryCode
                      join ReceivingCountry in dbContext.Country on c.ReceivingCountry equals ReceivingCountry.CountryCode
                      join bankInfo in dbContext.Bank.ToList() on c.BankId equals bankInfo.Id into joined
                      from bankInfo in joined.DefaultIfEmpty()
                      join creditDebitCardInfo in dbContext.CardTopUpCreditDebitInformation.Where(x => x.TransferType == (int)TransferType.BankDeposit).ToList()
                      on c.Id equals creditDebitCardInfo.CardTransactionId into cardInfo
                      from creditDebitCardInfo in cardInfo.DefaultIfEmpty()
                      join ReInTrans in dbContext.ReinitializeTransaction on c.ReceiptNo equals ReInTrans.NewReceiptNo
                      into ReInTransaction
                      from ReInTrans in ReInTransaction.DefaultIfEmpty()
                      select new SenderTransactionHistoryList()
                      {
                          Id = c.Id,
                          AccountNumber = c.ReceiverAccountNo,
                          SenderTelephoneNo = d.PhoneNumber,
                          ReceiverName = c.ReceiverName == null ? "No Name" : c.ReceiverName,
                          ReceiverCity = c.ReceiverCity,
                          ReceiverCountry = ReceivingCountry.CountryName,
                          Fee = c.Fee,
                          GrossAmount = c.SendingAmount,
                          StatusOfBankDepoist = c.Status,
                          StatusName = Common.Common.GetEnumDescription(c.Status),
                          TransactionType = Common.Common.GetEnumDescription(TransactionServiceType.BankDeposit),
                          ReceivingCurrencySymbol = c.ReceivingCurrency == null ? ReceivingCountry.CurrencySymbol : Common.Common.GetCurrencySymbolByCurrency(c.ReceivingCurrency),
                          ReceivingCurrrency = c.ReceivingCurrency == null ? ReceivingCountry.Currency : c.ReceivingCurrency,
                          SendingCurrency = c.SendingCurrency== null ? SendingCountry.Currency : c.SendingCurrency,
                          SendingCurrencySymbol = c.SendingCurrency == null ? SendingCountry.CurrencySymbol :  Common.Common.GetCurrencySymbolByCurrency(c.SendingCurrency), //SendingCountry.CurrencySymbol,
                          ReceivingAmount = c.ReceivingAmount,
                          TotalAmount = c.TotalAmount,
                          SenderPaymentMode = c.SenderPaymentMode,
                          CardNumber = creditDebitCardInfo == null ? "" : creditDebitCardInfo.CardNumber, //GetCardNo(c.Id), 
                                                                                                          //PaymentMethod = SSenderForAllTransfer.GetCreditCardLastDigit(c.SenderPaymentMode, TransferType.BankDeposit, c.Id),
                          ExchangeRate = c.ExchangeRate,
                          Date = c.TransactionDate.ToString("MMMM dd, yyyy"),
                          TransactionServiceType = TransactionServiceType.BankDeposit,
                          TransactionDate = c.TransactionDate,
                          Reference = c.PaymentReference,
                          BankCode = c.BankCode,
                          BankName = c.IsEuropeTransfer == true ? c.BankName : (bankInfo == null ? "" : bankInfo.Name),// GetBankName(c.BankId), //Common.Common.getBankName(c.BankId),
                          TransactionIdentifier = c.ReceiptNo,
                          FaxerAccountNo = d.AccountNo,
                          FaxerCountry = SendingCountry.CountryName,
                          SenderName = d.FirstName.Trim() + " " + (string.IsNullOrEmpty(d.MiddleName) == true ? "" : d.MiddleName.Trim() + " ") + d.LastName.Trim(),
                          IsManualBankDeposit = c.IsManualDeposit,
                          senderId = c.SenderId,
                          IsRetryAbleCountry = false,
                          IsBusiness = d.IsBusiness,
                          IsAbnormalTransaction = c.Status == BankDepositStatus.Abnormal ? true : false,
                          IsEuropeTransfer = c.IsEuropeTransfer,
                          IsAwaitForApproval = c.IsComplianceApproved == true ? false : c.IsComplianceNeededForTrans,
                          //TransactionStatusForAdmin = GetBankDepositStatusName(c.Status),
                          PaidFromModule = c.PaidFromModule,
                          AgentStaffId = c.PayingStaffId,
                          ApiService = c.Apiservice.ToString(),
                          TransferReference = c.TransferReference,
                          IsDuplicatedTransaction = c.IsTransactionDuplicated,
                          DuplicatedTransactionReceiptNo = c.ReceiptNo,
                          ReInitializedReceiptNo = ReInTrans == null ? "" : ReInTrans.ReceiptNo,
                          IsReInitializedTransaction = ReInTrans == null ? false : true,
                          ReInitializeStaffName = ReInTrans == null ? "" : ReInTrans.CreatedByName,
                          ReInitializedDateTime = ReInTrans == null ? "" : ReInTrans.Date.ToString(),
                          RecipientId = c.RecipientId,
                          SenderCountryCode = c.SendingCountry,
                          SenderEmail = d.Email ?? "",

                      }).GroupBy(x => x.Id).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }
        private string GetCardNo(int Id)
        {

            var data = dbContext.CardTopUpCreditDebitInformation.Where(x => x.TransferType == (int)TransferType.BankDeposit && x.CardTransactionId == Id).FirstOrDefault();
            if (data != null)
            {

                return data.CardNumber ?? "";
            }
            return "";


        }
        private string GetBankName(int Id)
        {

            var data = dbContext.Bank.Where(x => x.Id == Id).FirstOrDefault();
            if (data != null)
            {

                return data.Name ?? "";
            }
            return "";
        }

        public string GetBankDepositStatusName(BankDepositStatus depositStatus)
        {
            string status = "";
            switch (depositStatus)
            {
                case BankDepositStatus.Held:
                    status = "On Hold";
                    break;
                case BankDepositStatus.UnHold:
                    status = "Un Hold";
                    break;
                case BankDepositStatus.Cancel:
                    status = "Cancelled";
                    break;
                case BankDepositStatus.Confirm:
                    status = "Paid";
                    break;
                case BankDepositStatus.Incomplete:
                    status = "In progress";
                    break;
                case BankDepositStatus.Failed:
                    status = "Failed";
                    break;
                case BankDepositStatus.PaymentPending:
                    status = "Payment Pending";
                    break;
                case BankDepositStatus.IdCheckInProgress:
                    status = "In progress (ID Check)";
                    break;
                case BankDepositStatus.PendingBankdepositConfirmtaion:
                    status = "In progress (MoneyFex Bank Deposit)";
                    break;
                case BankDepositStatus.Abnormal:
                    status = "Abnormal";
                    break;
                default:
                    break;
            }
            return status;

        }

        public FaxerInformation GetFaxerInformation(int FaxerId)
        {
            var result = dbContext.FaxerInformation.Where(x => x.Id == FaxerId).FirstOrDefault();
            return result;
        }
        public bool DeletePendingTransaction(int id, TransactionServiceType transactionService)
        {

            string TransactionNumber = "";
            string SendingAmount = "";
            string Receivingcountry = "";
            string ReceiverName = "";
            string Fee = "";
            string BankAccount = "";
            int bankId = 0;
            int WalletId = 0;
            string BankCode = "";
            string MFCN = "";
            int senderId = 0;
            TransactionServiceType ServiceType = new TransactionServiceType();
            switch (transactionService)
            {
                case TransactionServiceType.BankDeposit:
                    var data = dbContext.BankAccountDeposit.Where(x => x.Id == id).FirstOrDefault();


                    TransactionNumber = data.ReceiptNo;
                    SendingAmount = Common.Common.GetCurrencyCode(data.SendingCountry) + " " + data.SendingAmount;
                    Receivingcountry = Common.Common.GetCountryName(data.ReceivingCountry);
                    ReceiverName = data.ReceiverName;
                    Fee = Common.Common.GetCurrencyCode(data.SendingCountry) + " " + data.Fee;
                    BankAccount = data.ReceiverAccountNo;
                    bankId = data.BankId;
                    BankCode = data.BankCode;
                    senderId = data.SenderId;
                    WalletId = 0;
                    MFCN = "";
                    ServiceType = TransactionServiceType.BankDeposit;
                    dbContext.BankAccountDeposit.Remove(data);
                    dbContext.SaveChanges();

                    break;

                case TransactionServiceType.CashPickUp:
                    var CashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == id).FirstOrDefault();


                    TransactionNumber = CashPickUp.ReceiptNumber;
                    SendingAmount = Common.Common.GetCurrencyCode(CashPickUp.SendingCountry) + " " + CashPickUp.FaxingAmount;
                    Receivingcountry = Common.Common.GetCountryName(CashPickUp.ReceivingCountry);
                    senderId = CashPickUp.NonCardReciever.FaxerID;

                    ReceiverName = CashPickUp.NonCardReciever.FullName;
                    Fee = Common.Common.GetCurrencyCode(CashPickUp.SendingCountry) + " " + CashPickUp.FaxingFee;
                    BankAccount = "";
                    bankId = 0;
                    BankCode = "";
                    ServiceType = TransactionServiceType.CashPickUp;
                    WalletId = 0;
                    MFCN = CashPickUp.MFCN;
                    dbContext.FaxingNonCardTransaction.Remove(CashPickUp);
                    dbContext.SaveChanges();
                    break;

                case TransactionServiceType.MobileWallet:
                    var MobileWallet = dbContext.MobileMoneyTransfer.Where(x => x.Id == id).FirstOrDefault();

                    TransactionNumber = MobileWallet.ReceiptNo;
                    SendingAmount = Common.Common.GetCurrencyCode(MobileWallet.SendingCountry) + " " + MobileWallet.SendingAmount;
                    Receivingcountry = Common.Common.GetCountryName(MobileWallet.ReceivingCountry);
                    ReceiverName = MobileWallet.ReceiverName;
                    Fee = Common.Common.GetCurrencyCode(MobileWallet.SendingCountry) + " " + MobileWallet.Fee;
                    BankAccount = "";
                    bankId = 0;
                    BankCode = "";
                    senderId = MobileWallet.SenderId;
                    WalletId = MobileWallet.WalletOperatorId;
                    ServiceType = TransactionServiceType.MobileWallet;
                    MFCN = "";
                    dbContext.MobileMoneyTransfer.Remove(MobileWallet);
                    dbContext.SaveChanges();

                    break;
            }
            #region
            //if (transactionService == TransactionServiceType.BankDeposit)
            //{
            //    //SenderFristName = data.SenderId;
            //    //TransactionNumber = data.ReceiptNo;
            //    //SendingAmount = Common.Common.GetCurrencyCode(data.SendingCountry) + data.SendingAmount;
            //    //Receivingcountry = Common.Common.GetCountryName(data.ReceivingCountry);
            //    //ReceiverName = data.ReceiverName;
            //    //Fee = "";
            //    //BankAccount = "";
            //    //bankId = 0;
            //    //BankCode = "";
            //    //senderId = 0;



            //    //send pending transaction deleted email
            //}
            //if (transactionService == TransactionServiceType.CashPickUp)
            //{
            //    var data = dbContext.FaxingNonCardTransaction.Where(x => x.Id == id).FirstOrDefault();
            //    dbContext.FaxingNonCardTransaction.Remove(data);
            //    dbContext.SaveChanges();
            //    //send pending transaction deleted email

            //}
            //if (transactionService == TransactionServiceType.MobileWallet)
            //{
            //    var data = dbContext.MobileMoneyTransfer.Where(x => x.Id == id).FirstOrDefault();
            //    dbContext.MobileMoneyTransfer.Remove(data);
            //    dbContext.SaveChanges();
            //    //send pending transaction deleted email

            //}

            #endregion

            SendTransactionCancelledEmail(TransactionNumber, SendingAmount, Receivingcountry, Fee, ReceiverName, BankAccount, bankId, BankCode, senderId, ServiceType, WalletId, MFCN);
            return true;
        }
        public void SendTransactionCancelledEmail(string TransactionNumber, string SendingAmount, string Receivingcountry, string Fee,
            string ReceiverName, string BankAccount, int bankId, string BankCode, int senderId, TransactionServiceType ServiceType, int WalletId, string MFCN)
        {
            var senderInfo = Common.Common.GetSenderInfo(senderId);
            string email = senderInfo.Email;
            string SenderFristName = senderInfo.FirstName;
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = Common.Common.getBankName(bankId);
            //string ReceivingCountry = Common.Common.GetCountryName(Receivingcountry);
            //string receiverFirstName = Common.Common.GetCountryName(Receivingcountry);
            string WalletName = "";
            if (WalletId != 0)
            {
                WalletName = Common.Common.GetMobileWalletInfo(WalletId).Name;

            }

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransactionCancelled?" +
                 "&SenderFristName=" + SenderFristName + "&TransactionNumber=" + TransactionNumber + "&SendingAmount=" + SendingAmount + "&Receivingcountry=" + Receivingcountry
                 + "&Fee=" + Fee + "&ReceiverName=" + ReceiverName + "&BankName=" + bankName + "&BankAccount=" + BankAccount
                 + "&BankCode=" + BankCode + "&TransactionServiceType=" + ServiceType + "&WalletName=" + WalletName + "&MFCN=" + MFCN);

            mail.SendMail(email, "Transfer cancelled" + " " + TransactionNumber, body);

        }
        public UpdateProperyViewModel GetDetailsForUpdateProperty(int transactionId, TransactionServiceType transactionServiceType)
        {
            UpdateProperyViewModel vm = new UpdateProperyViewModel();
            vm.TransactionId = transactionId;
            vm.TransactionServiceType = transactionServiceType;
            var recipient = new Recipients();
            switch (transactionServiceType)
            {
                case TransactionServiceType.BankDeposit:
                    SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();
                    var bankInfo = senderBankAccountDepositServices.List().Data.Where(x => x.Id == transactionId).FirstOrDefault();
                    if (bankInfo != null)
                    {
                        recipient = dbContext.Recipients.Where(x => x.Id == bankInfo.RecipientId).FirstOrDefault();

                        vm.AccountNo = bankInfo.ReceiverAccountNo;
                        vm.BankCode = bankInfo.BankCode;
                        vm.ReceiptNo = bankInfo.ReceiptNo;
                        vm.ReceiverName = bankInfo.ReceiverName;
                        vm.RecipientId = bankInfo.RecipientId;
                        vm.Country = bankInfo.ReceivingCountry;
                        vm.AccountNumber = bankInfo.ReceiverAccountNo;
                        vm.BankId = bankInfo.BankId;
                        vm.BankName = Common.Common.getBankName(bankInfo.BankId);
                        vm.BranchCode = bankInfo.BankCode;
                        vm.IsEuropeTransfer = bankInfo.IsEuropeTransfer;
                        vm.IsSouthAfricaTransfer = Common.Common.IsSouthAfricanTransfer(bankInfo.ReceivingCountry);
                        vm.IsWestAfricaTransfer = Common.Common.IsWestAfricanTransfer(bankInfo.ReceivingCountry); ;
                        vm.PhoneNumber = bankInfo.ReceiverMobileNo;
                        vm.ReceiverCity = bankInfo.ReceiverCity;
                        vm.ReceiverEmail = recipient.Email;
                        vm.ReceiverStreet = recipient.Street;
                        vm.ReceiverPostalCode = recipient.PostalCode;
                    }
                    break;
                case TransactionServiceType.MobileWallet:
                    SSenderMobileMoneyTransfer _senderMobileMoneyTransfer = new SSenderMobileMoneyTransfer();
                    var mobileWalletInfo = _senderMobileMoneyTransfer.list().Data.Where(x => x.Id == transactionId).FirstOrDefault();
                    if (mobileWalletInfo != null)
                    {
                        vm.AccountNo = mobileWalletInfo.PaidToMobileNo;
                        vm.ReceiptNo = mobileWalletInfo.ReceiptNo;
                        vm.ReceiverName = mobileWalletInfo.ReceiverName;
                        vm.WalletId = mobileWalletInfo.WalletOperatorId;
                        vm.RecipientId = mobileWalletInfo.RecipientId;
                        vm.Country = mobileWalletInfo.ReceivingCountry;
                    }
                    break;
                case TransactionServiceType.CashPickUp:
                    SSenderCashPickUp _senderCashPickUp = new SSenderCashPickUp();
                    var cashPickUpInfo = _senderCashPickUp.List().Data.Where(x => x.Id == transactionId).FirstOrDefault();
                    if (cashPickUpInfo != null)
                    {
                        var recipientInfo = dbContext.Recipients.Where(x => x.Id == cashPickUpInfo.RecipientId).FirstOrDefault();
                        vm.AccountNo = recipientInfo.MobileNo;
                        vm.ReceiptNo = cashPickUpInfo.ReceiptNumber;
                        vm.ReceiverName = recipientInfo.ReceiverName;
                        vm.RecipientId = cashPickUpInfo.RecipientId;
                        vm.Country = cashPickUpInfo.ReceivingCountry;
                        vm.PhoneNumber = recipientInfo.MobileNo;
                        vm.ReceiverEmail = recipientInfo.Email;
                        vm.IdenityCardId = cashPickUpInfo.RecipientIdentityCardId;
                        vm.IdentityCardNumber = cashPickUpInfo.RecipientIdenityCardNumber;
                    }
                    break;
            }
            return vm;
        }
        public int UpdatePropertyOfTransaction(UpdateProperyViewModel vm)
        {
            int senderId = 0;
            switch (vm.TransactionServiceType)
            {
                case TransactionServiceType.BankDeposit:
                    SSenderBankAccountDeposit senderBankAccountDepositServices = new SSenderBankAccountDeposit();
                    var bankInfo = senderBankAccountDepositServices.List().Data.Where(x => x.Id == vm.TransactionId).FirstOrDefault();
                    bankInfo.ReceiverName = vm.ReceiverName;
                    bankInfo.ReceiverAccountNo = vm.AccountNo;
                    bankInfo.IsComplianceNeededForTrans = true;
                    bankInfo.IsComplianceApproved = false;
                    bankInfo.Status = BankDepositStatus.Held;
                    bankInfo.BankCode = vm.BankCode.Trim();
                    vm.BankId = dbContext.Bank.Where(x => x.Code == vm.BankCode.Trim()).Select(x => x.Id).FirstOrDefault();
                    bankInfo.BankId = vm.BankId;
                    senderId = bankInfo.SenderId;
                    vm.RecipientId = bankInfo.RecipientId;
                    senderBankAccountDepositServices.Update(bankInfo);
                    break;
                case TransactionServiceType.MobileWallet:
                    SSenderMobileMoneyTransfer _senderMobileMoneyTransfer = new SSenderMobileMoneyTransfer();
                    var mobileWalletInfo = _senderMobileMoneyTransfer.list().Data.Where(x => x.Id == vm.TransactionId).FirstOrDefault();
                    mobileWalletInfo.ReceiverName = vm.ReceiverName;
                    mobileWalletInfo.WalletOperatorId = vm.WalletId;
                    mobileWalletInfo.PaidToMobileNo = vm.AccountNo;
                    mobileWalletInfo.IsComplianceNeededForTrans = true;
                    mobileWalletInfo.IsComplianceApproved = false;
                    mobileWalletInfo.Status = MobileMoneyTransferStatus.Held;
                    senderId = mobileWalletInfo.SenderId;
                    vm.RecipientId = mobileWalletInfo.RecipientId;
                    _senderMobileMoneyTransfer.Update(mobileWalletInfo);
                    break;
                case TransactionServiceType.CashPickUp:
                    SSenderCashPickUp _senderCashPickUp = new SSenderCashPickUp();
                    var cashPickUpInfo = _senderCashPickUp.List().Data.Where(x => x.Id == vm.TransactionId).FirstOrDefault();
                    cashPickUpInfo.IsComplianceNeededForTrans = true;
                    cashPickUpInfo.IsComplianceApproved = false;
                    cashPickUpInfo.FaxingStatus = FaxingStatus.Hold;
                    senderId = cashPickUpInfo.SenderId;
                    vm.RecipientId = cashPickUpInfo.RecipientId;
                    UpdateReceiverDetails(vm, cashPickUpInfo.NonCardRecieverId);
                    _senderCashPickUp.Update(cashPickUpInfo);
                    break;
            }

            UpdateRecipentFromUpdateTransaction(vm);
            return senderId;
        }

        private void UpdateReceiverDetails(UpdateProperyViewModel vm, int receiverId)
        {
            var receiversDetail = dbContext.ReceiversDetails.Where(x => x.Id == receiverId).FirstOrDefault();
            receiversDetail.FullName = vm.ReceiverName;
            receiversDetail.PhoneNumber = vm.AccountNo;
            var splittedName = vm.ReceiverName.Split(' ');
            receiversDetail.FirstName = splittedName[0];
            receiversDetail.MiddleName = splittedName.Count() > 2 ? splittedName[1] : " ";
            receiversDetail.LastName = splittedName[splittedName.Count() - 1];
            dbContext.Entry(receiversDetail).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

        private void UpdateRecipentFromUpdateTransaction(UpdateProperyViewModel vm)
        {
            RecipientServices _recipientServices = new RecipientServices();
            var recipient = _recipientServices.Recipients().Where(x => x.Id == vm.RecipientId).FirstOrDefault();
            if (recipient != null)
            {
                recipient.ReceiverName = vm.ReceiverName;
                recipient.AccountNo = vm.AccountNo;
                recipient.Email = vm.ReceiverEmail;
                if (!string.IsNullOrEmpty(vm.ReceiverStreet))
                {
                    recipient.Street = vm.ReceiverStreet;
                }
                if (!string.IsNullOrEmpty(vm.ReceiverPostalCode))
                {
                    recipient.PostalCode = vm.ReceiverPostalCode;
                }
                switch (recipient.Service)
                {
                    case Service.BankAccount:
                        recipient.BranchCode = vm.BankCode;
                        recipient.BankId = vm.BankId;

                        break;
                    case Service.MobileWallet:
                        recipient.MobileNo = vm.AccountNo;
                        recipient.MobileWalletProvider = vm.WalletId;
                        break;
                    case Service.CashPickUP:
                        recipient.MobileNo = vm.AccountNo;

                        break;
                }
                _recipientServices.UpdateReceipts(recipient);
            }
        }

    }
}