using FAXER.PORTAL.Areas.Admin.Controllers;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers.MyRegisteredKiiPayWallets;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using Microsoft.Ajax.Utilities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class EmailServices
    {
        FAXEREntities dbContext = null;
        TransferExchangeRateHistory rate;
        public EmailServices()
        {
            dbContext = new FAXEREntities();
        }


        public EmailModel GetAllTransactionDetailsForEmail(EmailModel model)
        {

            var bankDepositData = (from c in dbContext.BankAccountDeposit.ToList()
                                   join d in model.Transactions.Where(x => x.Method.ToLower() == "bank deposit").ToList()
                                   on c.Id equals d.TransactionId
                                   select new EmailTransactionList()
                                   {
                                       Method = "Bank Deposit",
                                       SenderId = c.SenderId,
                                       TransactionId = c.Id,
                                       RecevingCountry = c.ReceivingCountry
                                   }).ToList().GroupBy(c => new
                                   {
                                       c.SenderId,
                                       c.RecevingCountry,
                                   }).Select(x => x.FirstOrDefault()).ToList();

            var mobileTransferData = (from c in dbContext.MobileMoneyTransfer.ToList()
                                      join d in model.Transactions.Where(x => x.Method.ToLower() == "mobile wallet").ToList()
                                      on c.Id equals d.TransactionId
                                      select new EmailTransactionList()
                                      {
                                          Method = "Mobile Wallet",
                                          SenderId = c.SenderId,
                                          TransactionId = c.Id,
                                          RecevingCountry = c.ReceivingCountry
                                      }).ToList().GroupBy(c => new
                                      {
                                          c.SenderId,
                                          c.RecevingCountry,
                                      }).Select(x => x.FirstOrDefault()).ToList();

            //var cashpickData = (from c in dbContext.BankAccountDeposit.GroupBy(x => x.SenderId).Select(x => x.LastOrDefault()).ToList()
            //                    join d in model.Transactions.ToList() on c.Id equals d.TransactionId
            //                    where d.Method == "cash pickup"
            //                    select d).ToList();
            var cashpickData = (from c in dbContext.FaxingNonCardTransaction.ToList()
                                join d in model.Transactions.Where(x => x.Method.ToLower() == "cash pickup").ToList()
                                on c.Id equals d.TransactionId
                                select new EmailTransactionList()
                                {
                                    Method = "Cash Pickup",
                                    SenderId = c.SenderId,
                                    TransactionId = c.Id,
                                    RecevingCountry = c.ReceivingCountry
                                }).ToList().GroupBy(c => new
                                {
                                    c.SenderId,
                                    c.RecevingCountry,
                                }).Select(x => x.FirstOrDefault()).ToList();

            var Transactions = new List<EmailTransactionList>();
            Transactions.AddRange(bankDepositData);
            Transactions.AddRange(mobileTransferData);
            Transactions.AddRange(cashpickData);
            //model.Transactions = Transactions.ToList();
            var result = Transactions.ToList().GroupBy(c => new
            {
                c.SenderId,
                c.RecevingCountry,
            })
        .Select(gcs => new EmailTransactionList()
        {
            SenderId = gcs.FirstOrDefault().SenderId,
            RecevingCountry = gcs.Key.RecevingCountry,
            TransactionId = gcs.FirstOrDefault().TransactionId,
            Method = gcs.FirstOrDefault().Method,
        });
            model.Transactions = result.ToList();
            return model;

        }
        public void BankTransactionInProgressEmail(SenderTransactionActivityVm item)
        {
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = Common.Common.getBankName(item.BankId);
            string ReceiverCountryName = Common.Common.GetCountryName(item.ReceivingCountry);

            string WalletName = Common.Common.GetMobileWalletInfo(item.SenderId).Name;
            string SendingCurrency = Common.Common.GetCountryName(item.SendingCountry);
            string ReceivingCurrency = Common.Common.GetCountryName(item.ReceivingCountry);

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/TransactionStillInProgress?" +
                     "&SenderFristName=" + senderInfo.FirstName + "&RecipentName=" + item.ReceiverName +
                     "&ReceiverCountry=" + ReceiverCountryName + "&TransactionNumber=" + item.identifier
                     + "&BankName=" + bankName + "&BankAccount=" + item.BankAccount);

            mail.SendMail(senderInfo.Email, "Money Transferred still in progress -" + " " + item.identifier, body);
        }

        public void BankIdCheckEmail(SenderTransactionActivityVm item)
        {
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = Common.Common.getBankName(item.BankId);

            string ReceivingCurrency = Common.Common.GetCurrencyCode(item.ReceivingCountry);
            string SendingCountryName = Common.Common.GetCountryName(item.SendingCountry);
            string SendingCurrency = Common.Common.GetCurrencyCode(item.SendingCountry);
            string SendingCurrencySymbol = Common.Common.GetCurrencySymbol(item.SendingCountry);

            string ReceiverFirstName = item.ReceiverName.Split(' ')[0];

            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/IdCheckRemeinder?" +
                     "&SenderFristName=" + senderInfo.FirstName + "&ReceiverFirstName=" + ReceiverFirstName +
                     "&TransactionNumber=" + item.identifier + "&SendingCurrency=" + SendingCurrency +
                     "&SendingAmount=" + item.SendingAmount + "&SendingCountry=" + SendingCountryName + "&Fee=" + item.Fee +
                     "&ReceivingCurrency=" + ReceivingCurrency + "&ReceivingAmount=" + item.ReceivingAmount + "&BankName=" + bankName +
                     "&BankAccount=" + item.BankAccount + "&BranchCode=" + item.BranchCode + "&SendingCountry=" + item.SendingCountry);

            mail.SendMail(senderInfo.Email, " ID Check Reminder-" + " " + item.identifier, body);
        }
        public void RatesAlertEmail(SenderTransactionActivityVm item)

        {
            var senderInfo = Common.Common.GetSenderInfo(item.SenderId);
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string senderCountry = senderInfo.Country;
            string receiverCountry = item.ReceivingCountry;
            string sendingCurrency = Common.Common.GetCountryCurrencyName(senderCountry);
            string sendingCurrencySymbol = Common.Common.GetCurrencySymbol(senderCountry);
            string receivingCurrency = Common.Common.GetCountryCurrencyName(receiverCountry);
            var todayDate = DateTime.Now;
            var yesterDayDate = todayDate.AddDays(-1);
            var transactionTransferMethod = TransactionTransferMethod.BankDeposit;

            switch (item.TransactionServiceType)
            {
                case Models.TransactionServiceType.All:
                    break;
                case Models.TransactionServiceType.MobileWallet:
                    transactionTransferMethod = TransactionTransferMethod.OtherWallet;
                    break;
                case Models.TransactionServiceType.KiiPayWallet:
                    break;
                case Models.TransactionServiceType.BillPayment:
                    break;
                case Models.TransactionServiceType.ServicePayment:
                    break;
                case Models.TransactionServiceType.CashPickUp:

                    transactionTransferMethod = TransactionTransferMethod.CashPickUp;
                    break;
                case Models.TransactionServiceType.BankDeposit:
                    transactionTransferMethod = TransactionTransferMethod.BankDeposit;
                    break;
                default:
                    break;
            }

            var todayRateInfo = SExchangeRate.GetExchangeRate(senderCountry, receiverCountry, transactionTransferMethod);

            //var todayRateInfo = dbContext.TransferExchangeRateHistory.Where(x => DbFunctions.TruncateTime(x.CreatedDate) ==
            //DbFunctions.TruncateTime(todayDate) && x.SendingCountry == senderCountry
            //&& x.ReceivingCountry == receiverCountry).FirstOrDefault() ??  new TransferExchangeRateHistory();

            //var yesterDayRateInfo = dbContext.TransferExchangeRateHistory.Where(x => DbFunctions.TruncateTime(x.CreatedDate) <
            //DbFunctions.TruncateTime(yesterDayDate) && x.SendingCountry == senderCountry &&
            //x.ReceivingCountry == receiverCountry && x.TransferMethod == TransactionTransferMethod.BankDeposit).FirstOrDefault()
            // ?? new TransferExchangeRateHistory();
            var yesterDayRateInfo = new TransferExchangeRateHistory();
            GetYesterdayRate(senderCountry, receiverCountry,
               transactionTransferMethod, yesterDayDate);
            yesterDayRateInfo = rate;

            //yesterDayRateInfo = new TransferExchangeRateHistory() { 
            
            //    Rate = 515M

            //};

            if (todayRateInfo != null && yesterDayRateInfo != null)
            {
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/RateAlertEmail/Index?" +
                                  "&SenderFristName=" + senderInfo.FirstName + "&TodayDate=" +
                                  todayDate.ToString("dd/MM/yyyy")
                                  + "&TodayRate=" + Math.Round(todayRateInfo.Rate, 2)
                                  + "&YesterdayDate=" + yesterDayDate.ToString("dd/MM/yyyy") +
                                  "&YesterdayRate=" + yesterDayRateInfo.Rate + "&RateAlert=" + "emailType"
                                  + "&SendingCurrency=" + sendingCurrency + "&ReceivingCurrency="
                                  + receivingCurrency + "&SendingCurrencySymbol=" + sendingCurrencySymbol
                                  + "&ReceivingCountry=" + receiverCountry.ToLower() + "&SendingCountry=" + senderInfo.Country);


                mail.SendMail(senderInfo.Email, "Moneyfex Rate Alert", body);
            }

            try
            {

                Log.Write("Rate Alert Today Rate " + todayRateInfo.Rate);
                Log.Write("Rate Alert Today Rate " + yesterDayRateInfo.Rate);

            }
            catch (Exception)
            {
                Log.Write("Rate Alert Today Rate Exception occurs");


            }

        }

        public void GetYesterdayRate(string SendingCountry, string receivingCountry,
            TransactionTransferMethod transferMethod, DateTime date)
        {

            var data = dbContext.TransferExchangeRateHistory;

            var yesterDayRateInfo = dbContext.TransferExchangeRateHistory.Where(x => DbFunctions.TruncateTime(x.CreatedDate) ==
                                              DbFunctions.TruncateTime(date) && x.SendingCountry == SendingCountry &&
                                              x.ReceivingCountry == receivingCountry && x.TransferMethod == transferMethod
                                              && x.TransferType == TransactionTransferType.Online).FirstOrDefault();
            if (yesterDayRateInfo == null)
            {
                yesterDayRateInfo = dbContext.TransferExchangeRateHistory.Where(x => DbFunctions.TruncateTime(x.CreatedDate) ==
                                                  DbFunctions.TruncateTime(date) && x.SendingCountry == SendingCountry &&
                                                  x.ReceivingCountry == receivingCountry && x.TransferMethod == TransactionTransferMethod.All
                                                  && x.TransferType == TransactionTransferType.Online).FirstOrDefault();
            }

            if (yesterDayRateInfo != null)
            {

                rate = yesterDayRateInfo;

            }
            else
            {
                GetYesterdayRate(SendingCountry, receivingCountry, transferMethod, date.AddDays(-1));
                //return yesterDayRateInfo;
            }


        }

        public void SendEmailToSender(EmailTransactionList item, string emailType)
        {
            SenderTransactionActivityVm vm = new SenderTransactionActivityVm();


            switch (item.Method.ToLower())
            {
                case "mobile wallet":
                    var mobile = dbContext.MobileMoneyTransfer.Where(x => x.Id == item.TransactionId).FirstOrDefault();
                    vm.SenderId = mobile.SenderId;
                    vm.ReceiverName = mobile.ReceiverName;
                    vm.identifier = mobile.ReceiptNo;
                    vm.Amount = mobile.TotalAmount.ToString();
                    vm.ReceivingCountry = mobile.ReceivingCountry;
                    vm.SendingAmount = mobile.SendingAmount.ToString();
                    vm.ReceivingAmount = mobile.ReceivingAmount.ToString();
                    break;

                case "kiipay wallet":
                    var kiipay = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.Id == item.TransactionId).FirstOrDefault();
                    vm.SenderId = kiipay.FaxerId;
                    vm.identifier = kiipay.ReceiptNumber;
                    vm.Amount = kiipay.TotalAmount.ToString();
                    vm.ReceivingCountry = kiipay.ReceivingCountry;
                    vm.SendingAmount = kiipay.FaxingAmount.ToString();
                    vm.ReceivingAmount = kiipay.RecievingAmount.ToString();
                    break;

                case "cash pickup":
                    var cash = dbContext.FaxingNonCardTransaction.Where(x => x.Id == item.TransactionId).FirstOrDefault();
                    vm.SenderId = cash.SenderId;
                    vm.identifier = cash.ReceiptNumber;
                    vm.Amount = cash.TotalAmount.ToString();
                    vm.ReceivingCountry = cash.ReceivingCountry;
                    vm.SendingAmount = cash.FaxingAmount.ToString();
                    vm.ReceivingAmount = cash.ReceivingAmount.ToString();
                    vm.ReceiverName = cash.NonCardReciever.FullName;
                    break;

                case "bank deposit":
                    var bankdeposit = dbContext.BankAccountDeposit.Where(x => x.Id == item.TransactionId).FirstOrDefault();
                    vm.SenderId = bankdeposit.SenderId;
                    vm.ReceiverName = bankdeposit.ReceiverName;
                    vm.identifier = bankdeposit.ReceiptNo;
                    vm.Amount = bankdeposit.TotalAmount.ToString();
                    vm.ReceivingCountry = bankdeposit.ReceivingCountry;
                    vm.BankAccount = bankdeposit.ReceiverAccountNo;
                    vm.BankId = bankdeposit.BankId;
                    vm.BranchCode = bankdeposit.BankCode;
                    vm.SendingAmount = bankdeposit.SendingAmount.ToString();
                    vm.ReceivingAmount = bankdeposit.ReceivingAmount.ToString();

                    // SendEmail(vm, emailType);
                    break;
            }

            if (emailType.ToLower() == "ratesalert")
            {

                Common.Common.SetTransactionEmailTypeSession(Models.TransactionEmailType.Rates);
                RatesAlertEmail(vm);
            }
            else
            {
                SendEmail(vm, emailType);
            }



        }

        public void SendEmail(SenderTransactionActivityVm vm, string emailType)
        {

            switch (emailType.ToLower())
            {
                case "inprogress":
                    Common.Common.SetTransactionEmailTypeSession(Models.TransactionEmailType.TransactionInProgress);
                    BankTransactionInProgressEmail(vm);
                    break;
                case "idcheck":
                    Common.Common.SetTransactionEmailTypeSession(Models.TransactionEmailType.IDCheck);
                    BankIdCheckEmail(vm);
                    break;
                case "incorrectrecipient":
                    Common.Common.SetTransactionEmailTypeSession(Models.TransactionEmailType.CustomerSupport);
                    BankIncorrectRecipientDetailEmail(vm);
                    break;
                case "proofofsourceoffunds":
                    Common.Common.SetTransactionEmailTypeSession(Models.TransactionEmailType.CustomerSupport);
                    ProofOfSourceOfFundsEmail(vm);
                    break;
                    //case "ratesalert":
                    //    RatesAlertEmail(vm);
                    //    break;
            }
        }

        public void ProofOfSourceOfFundsEmail(SenderTransactionActivityVm vm)
        {
            var senderInfo = Common.Common.GetSenderInfo(vm.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ProofOfSourceOfFunds?" +
                     "&senderFirstname=" + senderInfo.FirstName + "&receiverFullName=" + vm.ReceiverName);

            mail.SendMail(senderInfo.Email, "Money Transfer Source of Funds -" + " " + vm.identifier, body);
        }
        public void BankIncorrectRecipientDetailEmail(SenderTransactionActivityVm vm)
        {
            var senderInfo = Common.Common.GetSenderInfo(vm.SenderId);

            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = "";
            string bankName = Common.Common.getBankName(vm.BankId);
            string ReceiverCountryName = Common.Common.GetCountryName(vm.ReceivingCountry);


            body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/IncorrectRecipientDetails?" +
                     "&SenderFristName=" + senderInfo.FirstName + "&ReceiverFullName=" + vm.ReceiverName +
                     "&BankName=" + bankName + "&BankAccount=" + vm.BankAccount + "&Country=" + ReceiverCountryName);

            mail.SendMail(senderInfo.Email, "Incorrect Recipient Details-" + " " + vm.identifier, body);
        }
        public void sendAnouncementEmail(string emails, string emailType)
        {
            string[] emailList = emails.Split(',');
            var emailArray = (from c in emailList
                              select c).Distinct().ToArray();

            Common.Common.SetTransactionEmailTypeSession(Models.TransactionEmailType.CustomerSupport);
            switch (emailType)
            {
                case "closure":
                    foreach (var senderEmail in emailArray)
                    {
                        ClosureEmail(senderEmail);
                    }
                    break;
                case "reopening":
                    foreach (var senderEmail in emailArray.Distinct().ToList())
                    {
                        Log.Write(senderEmail + "Email Sent To");

                        ReOpeningEmail(senderEmail);
                    }
                    break;
                case "sales":
                    break;
                case "nationaldays":
                    break;
                case "internationalwomenday":
                    break;
                case "christmas":
                    break;
                case "newyear":
                    break;
                case "eid":
                    break;
                case "southafrica":
                    foreach (var senderEmail in emailArray)
                    {
                        NewCorridorEmail(senderEmail, "ZA");
                    }
                    break;
                case "morocco":
                    foreach (var senderEmail in emailArray)
                    {
                        NewCorridorEmail(senderEmail, "MA");
                    }
                    break;
                case "senegal":
                    foreach (var senderEmail in emailArray)
                    {
                        NewCorridorEmail(senderEmail, "SN");
                    }
                    break;
                case "mobileapplaunch":
                    foreach (var senderEmail in emailArray)
                    {
                        AppAvailableEmail(senderEmail);
                    }
                    break;

            }
        }

        public void ClosureEmail(string senderEmail)
        {
            var senderInfo = senderInfoByEmail(senderEmail);
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ClosureEmail?" +
                     "&senderFirstName=" + senderInfo.FirstName);
            mail.SendMail(senderEmail, "Announcement halting Operation", body);
        }

        public void ReOpeningEmail(string senderEmail)
        {
            var senderInfo = senderInfoByEmail(senderEmail);
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ReOpeningEmail?" +
                     "&senderFirstName=" + senderInfo.FirstName);
            mail.SendMail(senderEmail, "We Are Back - MoneyFex", body);
        }
        public void NewCorridorEmail(string senderEmail, string countryCode)
        {
            var senderInfo = senderInfoByEmail(senderEmail);
            var countryInfo = dbContext.Country.Where(x => x.CountryCode == countryCode).FirstOrDefault();
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            Log.Write("Corridor email Initiated");
            if (countryInfo != null)
            {
                string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/NewCorridorEmail?" +
                         "&countryName=" + countryInfo.CountryName +
                         "&countryCurrency=" + countryInfo.CurrencyName +
                         "&countryCode=" + countryCode);
                Log.Write("Corridor email Sent");
                mail.SendMail(senderEmail, "Welcome to " + countryInfo.CountryName + " - MoneyFex", body);
            }

        }
        public void AppAvailableEmail(string senderEmail)
        {
            var senderInfo = senderInfoByEmail(senderEmail);
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/AppAvailableEmail?" +
                     "&senderFirstName=" + senderInfo.FirstName);
            mail.SendMail(senderEmail, "MoneyFex App Now Available ", body);
        }

        public void CardDeletionEmail(SavedCard model)
        {
            MailCommon mail = new MailCommon();
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            var senderInfo = dbContext.FaxerInformation.Where(x => x.Id == model.UserId).FirstOrDefault();
            string formattedCardNo = model.Num.Decrypt().FormatSavedCardNumber();
            string body = FAXER.PORTAL.Common.Common.GetTemplate(baseUrl + "/EmailTemplate/CardDeletionEmail?" +
                  "&cardOwnerFirstName=" + senderInfo.FirstName + "&formattedCardNumber=" + formattedCardNo);
            mail.SendMail(senderInfo.Email, "Credit/Debit card deleted", body);

        }

        public FaxerInformation senderInfoByEmail(string email)
        {
            email = email.Trim();
            var senderInfo = dbContext.FaxerInformation.Where(x => x.Email == email).FirstOrDefault();
            return senderInfo;
        }
    }
}