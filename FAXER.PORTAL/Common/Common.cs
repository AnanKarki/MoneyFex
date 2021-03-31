using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Models.PaymentSummary;
using FAXER.PORTAL.Services;
using FAXER.PORTAL.Services.ApiService;
using SelectPdf;
using Stripe;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Common
{
    public static class Common
    {
        static DB.FAXEREntities dbContext;
        public static bool ValidateToken(string token)
        {
            //return true;
            if (token != "")
            {
                //SContext con = new SContext();
                return ListContext().Where(x => x.Token == token).FirstOrDefault() != null;
            }
            else
            {
                return false;
            }
        }

        public static string RequestToken { get { return HttpContext.Current.Request.Headers.GetValues("Token").FirstOrDefault() ?? ""; } }
        public static List<Context> ListContext()
        {
            dbContext = new DB.FAXEREntities();
            List<Context> listcontext = new List<Context>();
            listcontext = dbContext.Context.ToList();
            return listcontext;

        }
        public static bool sendBulkMail(string toEmailIds = "", string subject = "", string body = "")
        {
            if (!string.IsNullOrEmpty(toEmailIds))
            {
                string[] emails = toEmailIds.Split(',');

                var emailArray = (from c in emails
                                  select c).Distinct().ToArray();
                MailCommon mail = new MailCommon();
                foreach (var item in emailArray)
                {
                    mail.SendMail(item, subject, body);
                    // change later-->  mail.SendMail(item, subject, body, "*****@gmail.com", "*******");
                }
                return true;
            }
            return false;

        }

        public static string GetUserID(this IIdentity identity)
        {
            dbContext = new DB.FAXEREntities();

            string userID = FaxerSession.LoggedUser.Id.ToString(); //dbContext.AspNetUsers.Where(x => x.UserName == identity.Name).Select(x => x.Id).FirstOrDefault();
            return userID;
        }

        public static int GetTransactionInProgressCount()
        {
            dbContext = new DB.FAXEREntities();

            int FaxerId = FaxerSession.LoggedUser.Id;

            int Count = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.FaxingStatus == FaxingStatus.NotReceived || x.FaxingStatus == FaxingStatus.Hold).ToList()
                         join d in dbContext.ReceiversDetails.Where(x => x.FaxerID == FaxerId) on c.NonCardRecieverId equals d.Id
                         select c).Count();
            //int Count = dbContext.FaxingNonCardTransaction.Where(x => x.NonCardReciever.FaxerID == FaxerId && x.FaxingStatus == DB.FaxingStatus.NotReceived || x.FaxingStatus == DB.FaxingStatus.Hold).Count();

            return Count;



        }

        internal static string IgnoreZero(this string values)
        {
            if (string.IsNullOrEmpty(values))
            {
                return "";
            }
            string mobileno = values;
            try
            {

                if (mobileno.Substring(0, 1) == "0")
                {

                    mobileno = mobileno.Substring(1, mobileno.Length - 1);
                }
            }
            catch (Exception)
            {

                throw;
            }

            return mobileno;
        }


        internal static string GetNewAccessCodeForCardUser()
        {
            dbContext = new DB.FAXEREntities();

            //this code should be unique and random with8 digit length
            var val = GenerateRandomDigit(8);

            while (dbContext.KiiPayPersonalWalletWithdrawalCode.Where(x => x.AccessCode == val).Count() > 0)
            {
                val = GenerateRandomDigit(8);
            }
            return val;


        }

        public static DB.BankAccount GetBankAccountInfo(string CountryCode)
        {
            dbContext = new DB.FAXEREntities();

            var data = dbContext.BankAccount.Where(x => x.CountryCode == CountryCode).FirstOrDefault();
            return data;

        }
        public static DB.BankAccount GetBankAccountInfo(string CountryCode, TransferTypeForBankAccount transferType)
        {
            dbContext = new DB.FAXEREntities();
            var data = dbContext.BankAccount.Where(x => x.CountryCode == CountryCode);
            var bankAccountInfo = data.Where(x => x.TransferType == transferType).FirstOrDefault();
            if (bankAccountInfo == null)
            {
                bankAccountInfo = data.Where(x => x.TransferType == TransferTypeForBankAccount.All).FirstOrDefault();
            }
            return bankAccountInfo;
        }

        //internal static string GetTrackingNo(string pageName)
        //{
        //    dbContext = new FAXEREntities();
        //    var trackingCode = dbContext.SocialMediaTracking.Where(x => x.TrackingPage.ToLower() == pageName.ToLower()).FirstOrDefault();
        //    if (trackingCode != null)
        //    {

        //        return trackingCode.TrackingCode;
        //    }
        //    //<option>Home</option>
        //    //                     <option>Transfer Money</option>
        //    //                     <option>Registration Confirmation</option>
        //    //                     <option>Dashboard</option>
        //    //                     <option>Payment Confirmation</option>

        //    return "";
        //}
        internal static List<string> GetTrackingNo(string pageName)
        {
            dbContext = new FAXEREntities();
            var trackingCode = dbContext.SocialMediaTracking.Where(x => x.TrackingPage.ToLower() == pageName.ToLower()).Select(x => x.TrackingCode).ToList();
            if (trackingCode != null)
            {

                return trackingCode;
            }

            return null;
        }
        public static DocumentApprovalStatus? SenderStatus(int senderId = 0)
        {
            dbContext = new FAXEREntities();
            var senderStatus = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == senderId).FirstOrDefault();
            if (senderStatus == null)
            {
                return null;
            }
            return senderStatus.Status;

        }
        public static ExchangeRateType SystemExchangeRateType(string sendingCountry, string receivingCountry, TransactionTransferMethod transactionTransferMethod)
        {
            dbContext = new FAXEREntities();
            string sendingCurrency = GetCountryCurrency(sendingCountry);
            string receivingCurrency = GetCountryCurrency(receivingCountry);

            var systemExchangeRateType = dbContext.SystemExchangeRateType.Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == receivingCurrency
            && x.TransferMethod == transactionTransferMethod).FirstOrDefault();
            if (systemExchangeRateType == null)
            {
                systemExchangeRateType = dbContext.SystemExchangeRateType.Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == receivingCurrency
               && x.TransferMethod == TransactionTransferMethod.All).FirstOrDefault();
                if (systemExchangeRateType == null)
                {

                    return ExchangeRateType.TransactionExchangeRate;
                }
                return systemExchangeRateType.ExchangeRateType;

            }
            return systemExchangeRateType.ExchangeRateType;

        }
        public static Areas.Agent.Models.CashPickUpEnterAmountViewModel GetModifedAmount(string sendingCountry, string receivingCountry, TransactionTransferMethod transferMethod,
            decimal sendingAmount, TransactionTransferType transactionTransferType)
        {
            dbContext = new FAXEREntities();
            var feeInfo = SEstimateFee.GetTransferFee(sendingCountry, receivingCountry, transferMethod,
               sendingAmount, transactionTransferType);

            var newExchange = SEstimateFee.CalculateFaxingFee(sendingAmount, false, false,
            SExchangeRate.GetExchangeRateValue(sendingCountry, receivingCountry, transferMethod, 0,
            transactionTransferType), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);

            Areas.Agent.Models.CashPickUpEnterAmountViewModel newModifiedAmount = new Areas.Agent.Models.CashPickUpEnterAmountViewModel()
            {
                ReceivingAmount = newExchange.ReceivingAmount,
                ExchangeRate = newExchange.ExchangeRate,
                Fee = newExchange.FaxingFee,
                TotalAmount = newExchange.TotalAmount,
            };

            return newModifiedAmount;

        }
        public static bool IsPayoutFlowControlEnabled(string sendingCountry, string receivingCountry, Apiservice? apiService,
            TransactionTransferMethod transferMethod, int payoutProviderId)
        {
            FAXEREntities dbContext = new FAXEREntities();
            var sendingCurrency = GetCountryCurrency(sendingCountry);
            var receivingCurrency = GetCountryCurrency(receivingCountry);
            var payoutFlowControl = dbContext.PayoutFlowControl.Where(x => x.SendingCurrency == sendingCurrency && x.ReceivingCurrency == receivingCurrency
            && x.PayoutApi == apiService && x.TransferMethod == transferMethod).FirstOrDefault();

            if (payoutFlowControl == null)
            {
                return true;
            }
            else
            {
                var payoutFlowControlDetails = dbContext.PayoutFlowControlDetails.Where(x => x.PayoutFlowControlId == payoutFlowControl.Id &&
                 x.PayoutProviderId == payoutProviderId).FirstOrDefault();
                if (payoutFlowControlDetails == null)
                {
                    return true;
                }
                else
                {
                    return payoutFlowControl.IsPayoutEnabled;
                }
            }

        }
        public static bool IsVerifedSender(int senderId = 0)
        {
            dbContext = new FAXEREntities();
            var senderStatus = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == senderId).FirstOrDefault();
            if (senderStatus == null)
            {
                return false;
            }
            return true;
        }
        public static string GetSenderLastName()
        {

            dbContext = new DB.FAXEREntities();
            string LastName = dbContext.FaxerInformation.Where(x => x.Id == FaxerSession.LoggedUser.Id).FirstOrDefault().LastName;

            return LastName;
        }

        internal static bool IsEuropeTransfer(string country)
        {
            dbContext = new FAXEREntities();
            var continentCode = dbContext.Country.Where(x => x.CountryCode == country).FirstOrDefault();

            if (continentCode.Currency == "EURO" || continentCode.Currency == "EUR") {

                return true;
            }
            switch (continentCode.ContinentCode)
            {
                case "02":
                    return true;
                    break;
                default:
                    break;
            }
            return false;
        }

        internal static bool IsWestAfricanTransfer(string country)
        {
            dbContext = new FAXEREntities();
            //var continentCode = dbContext.Country.Where(x => x.CountryCode == country).FirstOrDefault();
            var Country = dbContext.Country.Where(x => x.CountryCode == country).FirstOrDefault();
            switch (Country.Currency)
            {
                case "XOF":
                    return true;
                    break;
                default:
                    break;
            }
            return false;
        }
        internal static bool IsSouthAfricanTransfer(string country)
        {
            dbContext = new FAXEREntities();
            //var continentCode = dbContext.Country.Where(x => x.CountryCode == country).FirstOrDefault();
            var Country = dbContext.Country.Where(x => x.CountryCode == country).FirstOrDefault();
            switch (Country.Currency)
            {
                case "ZAR":
                    return true;
                    break;
                default:
                    break;
            }
            return false;
        }

        internal static string getOrderNo()
        {
            if (FaxerSession.TransactionSummary != null)
            {
                switch (FaxerSession.TransactionSummary.TransferType)
                {
                    case TransferType.KiiPayWallet:

                        return GenerateKiiPayPersonalReceiptNo(6);
                        break;
                    case TransferType.PayForServices:
                        return GenerateMobileMoneyTransferReceiptNo(6);
                        break;
                    case TransferType.CashPickup:
                        return GenerateCashPickUpReceiptNo(6);
                        break;
                    case TransferType.MobileTransfer:
                        return GenerateMobileMoneyTransferReceiptNo(6);
                        break;
                    case TransferType.BankDeposit:
                        return GenerateBankAccountDepositReceiptNo(6);
                        break;
                    case TransferType.PayARequest:
                        return GeneratePayRequestReceiptNo(6);
                        break;
                    case TransferType.BillPayment:
                        return GenerateAgentPayBillMonthlyReceiptNo(6);
                        break;
                    default:
                        break;
                }
            }

            return "1246242";
        }

        internal static List<DB.TransferServiceDetails> GetTransferServices(string sendingCountry, string receivingCountry)
        {
            ServiceSettingsServices serviceSettingsServices = new ServiceSettingsServices();
            var data = serviceSettingsServices.GetTransferServiceDetails(sendingCountry, receivingCountry).ToList();
            var services = (from c in data
                            select new DB.TransferServiceDetails
                            {
                                Id = c.Id,
                                ServiceType = c.ServiceType,
                            }).ToList();
            return services;

        }
        internal static List<DB.TransferServiceDetails> GetTransferServicesByCurrency(string sendingCountry, string receivingCountry, string receivingCurrency)
        {
            ServiceSettingsServices serviceSettingsServices = new ServiceSettingsServices();
            var data = serviceSettingsServices.GetTransferServiceDetailsByCurrency(sendingCountry, receivingCountry, receivingCurrency).ToList();
            var services = (from c in data
                            select new DB.TransferServiceDetails
                            {
                                Id = c.Id,
                                ServiceType = c.ServiceType,
                            }).ToList();
            return services;

        }


        internal static bool IsValidBankDepositReceiver(string accountNumber, Service service)
        {
            dbContext = new FAXEREntities();
            //var data = dbContext.BlacklistedReceiver.Where(x => x.ReceiverAccountNo == accountNumber &&
            //x.IsBlocked == true && x.IsDeleted == false && x.TransferMethod == transferMethod).FirstOrDefault();
            var data = dbContext.Recipients.Where(x => (x.AccountNo == accountNumber || x.MobileNo == accountNumber) &&
            x.IsBanned == true && x.Service == (Service)service).FirstOrDefault();
            if (data != null)
            {
                if (accountNumber.Trim() == data.AccountNo || accountNumber.Trim() == data.MobileNo)
                {
                    return false;
                }
            }
            return true;
        }

        internal static bool HasExceededBankDepositReceiverLimit(string Country, string accountNumber, int BankId)
        {
            dbContext = new FAXEREntities();
            var data = dbContext.BankAccountDeposit.Where(x => x.ReceiverCountry == Country && x.BankId == BankId && x.ReceiverAccountNo == accountNumber
             && DbFunctions.TruncateTime(x.TransactionDate)
             == DbFunctions.TruncateTime(DateTime.Now) && x.Status == BankDepositStatus.Confirm).Count();
            int limit = Convert.ToInt32(Common.GetAppSettingValue("TransactionLimitPerReceiver"));
            if (data < limit)
            {

                return false;
            }
            return true;
        }
        internal static bool HasExceededBankDepositLimit(int senderId)
        {
            dbContext = new FAXEREntities();
            var data = dbContext.BankAccountDeposit.Where(x => x.SenderId == senderId
             && DbFunctions.TruncateTime(x.TransactionDate)
             == DbFunctions.TruncateTime(DateTime.Now) && x.Status == BankDepositStatus.Confirm).Count();
            int limit = Convert.ToInt32(Common.GetAppSettingValue("TotalTransactionLimit"));
            if (data < limit)
            {
                return false;
            }
            return true;
        }

        public static bool HasExceededReceiverLimit(int senderId, int RecipientId, string sendingCountry, string receivingCountry, TransactionTransferMethod transferMethod)
        {
            dbContext = new FAXEREntities();
            int data = 0;
            switch (transferMethod)
            {
                case TransactionTransferMethod.BankDeposit:
                    data = dbContext.BankAccountDeposit.Where(x => x.RecipientId == RecipientId && x.SenderId == senderId
                         && DbFunctions.TruncateTime(x.TransactionDate) == DbFunctions.TruncateTime(DateTime.Now)
                         && (x.Status == BankDepositStatus.Confirm
                         || x.Status == BankDepositStatus.Incomplete || x.Status == BankDepositStatus.Held)).Count();

                    break;

                case TransactionTransferMethod.OtherWallet:
                    data = dbContext.MobileMoneyTransfer.Where(x => x.RecipientId == RecipientId && x.SenderId == senderId
                           && DbFunctions.TruncateTime(x.TransactionDate) == DbFunctions.TruncateTime(DateTime.Now)
                           && (x.Status == MobileMoneyTransferStatus.Paid ||
                           x.Status == MobileMoneyTransferStatus.Held || x.Status == MobileMoneyTransferStatus.InProgress)).Count();

                    break;
                case TransactionTransferMethod.CashPickUp:
                    data = dbContext.FaxingNonCardTransaction.Where(x => x.RecipientId == RecipientId && x.SenderId == senderId
                    && DbFunctions.TruncateTime(x.TransactionDate) == DbFunctions.TruncateTime(DateTime.Now)
                           && (x.FaxingStatus == FaxingStatus.Completed ||
                           x.FaxingStatus == FaxingStatus.Completed || x.FaxingStatus == FaxingStatus.Hold)).Count();
                    break;
            }
            var recipientTransactionCounts = dbContext.RecipientTransactionCount;
            var limit = recipientTransactionCounts.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry
                         && x.RecipientId == RecipientId && x.SenderId == senderId).Select(x => x.TransactionCount).FirstOrDefault();
            if (limit == 0)
            {
                limit = recipientTransactionCounts.Where(x => x.SendingCountry == sendingCountry &&
                x.ReceivingCountry == receivingCountry && x.SenderId == senderId).
                    Select(x => x.TransactionCount).FirstOrDefault();
                if (limit == 0)
                {
                    limit = recipientTransactionCounts.Where(x => x.SendingCountry == sendingCountry &&
                                                                  x.ReceivingCountry == receivingCountry).Select(x => x.TransactionCount).FirstOrDefault();
                    if (limit == 0)
                    {
                        limit = recipientTransactionCounts.Where(x => x.SendingCountry.ToLower() == "all" &&
                                                                      x.ReceivingCountry.ToLower() == "all").
                                                                      Select(x => x.TransactionCount).FirstOrDefault();
                    }
                }
            }
            if (limit == 0)
            {
                limit = Convert.ToInt32(Common.GetAppSettingValue("TransactionLimitPerReceiver"));
                //return false;
            }
            if (data < limit)
            {
                return false;
            }
            return true;
        }

        public static ServiceResult<string> GetDocumentPath(HttpPostedFileBase identificationDoc)
        {
            string DocumentPhotoUrl = null;
            var allowedExtensions = new string[] { "gif", "jpg", "png", "jpeg", "pdf" };

            int fileLength = identificationDoc.FileName.Split('.').Length;
            var extension = identificationDoc.FileName.Split('.')[fileLength - 1];
            extension = extension.ToLower();
            var identificationDocPath = Guid.NewGuid() + "." + identificationDoc.FileName.Split('.')[fileLength - 1];

            if (allowedExtensions.Contains(extension))
            {
                try
                {
                    identificationDoc.SaveAs(HttpContext.Current.Server.MapPath("~/Documents") + "\\" + identificationDocPath);
                }
                catch (Exception ex)
                {
                }
                DocumentPhotoUrl = "/Documents/" + identificationDocPath;

                return new ServiceResult<string>()
                {
                    Data = DocumentPhotoUrl,
                    Status = ResultStatus.OK,
                    Message = ""
                };
            }
            else
            {
                return new ServiceResult<string>()
                {
                    Data = DocumentPhotoUrl,
                    Status = ResultStatus.Error,
                    Message = "File type not allowed to upload."
                };
            }

        }


        public static IQueryable<TransactionAmountLimit> GetLimitAmountBySenderId(int senderId, string receivingCountry)
        {

            FAXEREntities dbContext = new FAXEREntities();
            var limit = dbContext.TransactionAmountLimit.Where(x => x.SenderId == senderId &&
                            (x.ReceivingCountry.ToLower() == receivingCountry
                                || x.ReceivingCountry.ToLower() == "all"));

            if (limit.Count() > 0)
            {
                return limit;
            }
            return null;
        }



        public static bool HasExceededAmountLimit(int senderId, string sendingCountry,
            string receivingCountry, decimal Amount, DB.Module module, int staffId = 0)
        {
            if (Amount < GetAppSettingValue("TransactionLimit").ToDecimal())
            {

                return false;
            }
            return true;
            dbContext = new FAXEREntities();
            var senderCur = GetCountryCurrency(sendingCountry);
            var receivingCur = GetCountryCurrency(receivingCountry);

            var limit = dbContext.TransactionAmountLimit.Where(x => x.SendingCurrency == senderCur &&
                                                                    x.ReceivingCurrency == receivingCur &&
                                                                    x.ForModule == module);
            var finalLimit = limit.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry);
            if (finalLimit.Count() == 0)
            {
                finalLimit = limit.Where(x => x.SendingCountry.ToLower() == "all" && x.ReceivingCountry == receivingCountry);
                if (finalLimit.Count() == 0)
                {
                    finalLimit = limit.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry.ToLower() == "all");
                }
                if (finalLimit.Count() == 0)
                {
                    finalLimit = limit.Where(x => x.SendingCountry.ToLower() == "all" && x.ReceivingCountry.ToLower() == "all");
                }
            }

            switch (module)
            {
                case DB.Module.Faxer:
                    var limitToSender = finalLimit.Where(x => x.SenderId == senderId);
                    if (limitToSender.Count() == 0)
                    {
                        limitToSender = finalLimit.Where(x => x.SenderId == 0);
                    }
                    finalLimit = limitToSender;
                    break;
                case DB.Module.CardUser:
                    break;
                case DB.Module.BusinessMerchant:
                    break;
                case DB.Module.Agent:
                    //var limittoAgent = Finallimit;
                    //if (staffId > 0)
                    //{
                    //    limittoAgent = Finallimit.Where(x => x.StaffId == staffId && x.ForModule == DB.Module.Agent);
                    //    if (limittoAgent.Count() == 0)
                    //    {

                    //        limittoAgent = Finallimit.Where(x => x.StaffId == 0 && x.ForModule == DB.Module.Agent);
                    //    }
                    //}
                    //Finallimit = limittoAgent;
                    break;
                case DB.Module.Staff:
                    //var limittoStaff = Finallimit.Where(x => x.StaffId == 0);
                    //if (staffId > 0)
                    //{
                    //    limittoStaff = Finallimit.Where(x => x.StaffId == staffId && x.ForModule == DB.Module.Staff);
                    //    if (limittoStaff.Count() == 0)
                    //    {
                    //        limittoStaff = Finallimit.Where(x => x.StaffId == 0 && x.ForModule == DB.Module.Staff);
                    //    }
                    //}
                    //Finallimit = limittoStaff;
                    break;
                case DB.Module.KiiPayBusiness:
                    break;
                case DB.Module.KiiPayPersonal:
                    break;
                default:
                    break;
            }

            if (finalLimit.Count() == 0)
            {
                if (Amount < GetAppSettingValue("TransactionLimit").ToDecimal())
                {

                    return false;
                }
                return true;
            }

            var limitAmount = finalLimit.FirstOrDefault().Amount;
            if (Amount < limitAmount)
            {
                return false;
            }
            return true;
        }

        internal static RegisteredAgentType GetRegisteredAgentType(int id)
        {
            DB.FAXEREntities db = new FAXEREntities();
            var isAux = db.AgentInformation.Where(x => x.Id == id).Select(x => x.IsAUXAgent).FirstOrDefault();
            if (isAux)
            {
                return RegisteredAgentType.AuxAgent;
            }
            else
            {
                return RegisteredAgentType.Agent;
            }
        }

        internal static SecureTradingApiResponseTransactionLog GetPGRefNo(string refno)
        {

            dbContext = new FAXEREntities();
            var reference = dbContext.SecureTradingApiResponseTransactionLog.
                 Where(x => x.requesttypedescription == "AUTH"
                       && x.orderreference == refno && x.status == "Y"
                       && x.errorcode == "0").FirstOrDefault();
            if (reference != null)
                return reference;

            return new SecureTradingApiResponseTransactionLog();
        }

        public static bool HasExceededSenderTransactionLimit(int senderId, string sendingCountry, string receivingCountry, TransactionTransferMethod transferMethod)
        {
            dbContext = new FAXEREntities();
            int data = 0;
            switch (transferMethod)
            {
                case TransactionTransferMethod.BankDeposit:
                    data = dbContext.BankAccountDeposit.Where(x => x.SenderId == senderId && x.SendingCountry == sendingCountry &&
                           x.ReceivingCountry == receivingCountry && DbFunctions.TruncateTime(x.TransactionDate) == DbFunctions.TruncateTime(DateTime.Now) &&
                           (x.Status == BankDepositStatus.Confirm || x.Status == BankDepositStatus.Incomplete || x.Status == BankDepositStatus.Held)).Count();
                    break;

                case TransactionTransferMethod.OtherWallet:
                    data = dbContext.MobileMoneyTransfer.Where(x => x.SenderId == senderId && x.SendingCountry == sendingCountry &&
                           x.ReceivingCountry == receivingCountry && DbFunctions.TruncateTime(x.TransactionDate) == DbFunctions.TruncateTime(DateTime.Now)
                           && (x.Status == MobileMoneyTransferStatus.Paid
                           || x.Status == MobileMoneyTransferStatus.InProgress || x.Status == MobileMoneyTransferStatus.Held)).Count();
                    break;

                case TransactionTransferMethod.CashPickUp:
                    data = dbContext.FaxingNonCardTransaction.Where(x => x.SenderId == senderId &&
                            x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry
                           && DbFunctions.TruncateTime(x.TransactionDate) == DbFunctions.TruncateTime(DateTime.Now)
                           && (x.FaxingStatus == FaxingStatus.Completed ||
                           x.FaxingStatus == FaxingStatus.Hold
                           || x.FaxingStatus == FaxingStatus.NotReceived)).Count();
                    break;
            }
            var senderTransctionCounts = dbContext.SenderTransactionCount;
            var limit = senderTransctionCounts.Where(x => x.SendingCountry == sendingCountry && x.ReceivingCountry == receivingCountry
                        && x.SenderId == senderId).Select(x => x.TransactionCount).FirstOrDefault();
            if (limit == 0)
            {
                limit = senderTransctionCounts.Where(x => x.SendingCountry == sendingCountry &&
                                                          x.ReceivingCountry == receivingCountry).Select(x => x.TransactionCount).FirstOrDefault();
                if (limit == 0)
                {
                    limit = senderTransctionCounts.Where(x => x.SendingCountry.ToLower() == "all" &&
                                                              x.ReceivingCountry.ToLower() == "all").Select(x => x.TransactionCount).FirstOrDefault();
                }
            }
            if (limit == 0)
            {
                limit = Convert.ToInt32(Common.GetAppSettingValue("TransactionLimitPerReceiver"));
            }
            if (data < limit)
            {
                return false;
            }
            return true;
        }


        internal static bool SenderExist(string mobileNo = "", string email = "")
        {
            dbContext = new FAXEREntities();
            var data = dbContext.FaxerInformation.Where(x => x.PhoneNumber == mobileNo || x.Email == email).FirstOrDefault();
            if (data != null)
            {
                return false;
            }

            return true;
        }

        internal static bool IsManualDeposit(string sendingCountryCode, string receivingCountryCode)
        {
            // Logic goes here 
            // According to the Manual Deposit Configuration for specific country by Admin

            dbContext = new FAXEREntities();
            var ManualBankDeposit = dbContext.ManualDepositEnable.Where(x => x.PayingCountry == receivingCountryCode && x.IsEnabled == true).FirstOrDefault();
            if (ManualBankDeposit == null)
            {
                return false;
            }

            return true;
        }
        internal static bool IsRetryAbleCountry(int TransactionId)
        {
            dbContext = new FAXEREntities();
            var result = dbContext.BankAccountDeposit.Where(x => x.Id == TransactionId).FirstOrDefault();
            if (result.ReceivingCountry == "NG")
            {
                return true;
            }

            return false;

        }

        internal static string GetNewRefIdForMobileTransfer()
        {

            dbContext = new FAXEREntities();
            var refId = Guid.NewGuid().ToString();
            while (dbContext.MobileMoneyTransferResposeStatus.Where(x => x.refId == refId).Count() > 0)
            {
                refId = Guid.NewGuid().ToString();
            }
            return refId;
        }

        internal static AccountValidationRequest.CountryEnum? getAccountValidationCountryCodeForTZ(string countryCode)
        {
            switch (countryCode.ToLower())
            {
                case "gh":
                    return AccountValidationRequest.CountryEnum.GH;
                    break;
                case "ng":
                    return AccountValidationRequest.CountryEnum.NG;
                    break;
                default:
                    break;
            }

            return null;
        }
        internal static AccountValidationRequest.CurrencyEnum? getAccountValidationCountryCurrencyForTZ(string countryCode)
        {
            switch (countryCode.ToLower())
            {
                case "gh":
                    return AccountValidationRequest.CurrencyEnum.GHS;
                    break;
                case "ng":
                    return AccountValidationRequest.CurrencyEnum.NGN;
                    break;
                default:
                    break;
            }

            return null;
        }

        public static bool IDCardHasExpired()
        {
            //dbContext = new DB.FAXEREntities();
            //int FaxerId = FaxerSession.LoggedUser.Id;
            //var data = dbContext.FaxerInformation.Where(x => x.Id == FaxerId).FirstOrDefault();
            //var CurrentDate = DateTime.Now.Date;
            //var CardExpiryDate = data.IdCardExpiringDate.Date;
            //if (CurrentDate > CardExpiryDate)
            //{
            //    return true;
            //}
            //else
            //{
            //    return false;
            //}
            return true;
        }

        public static bool UserIsActive()
        {

            dbContext = new FAXEREntities();
            int senderId = FaxerSession.LoggedUser.Id;
            var IsActive = dbContext.FaxerLogin.Where(x => x.FaxerId == senderId).FirstOrDefault().IsActive;

            return IsActive;


        }
        internal static string GetKiiPayPersWalletReceiptNo()
        {
            return "";
        }


        /// <summary>
        /// The Amount will be compare to the 1000 pound sterling 
        /// if User has not uploaded their document then they will
        /// not be able to tranfer the amount above 1000 pound
        /// </summary>
        /// <param name="FaxingAmount ,ReceivingAmount , Country"></param>
        internal static bool IsValidAmountToTransfer(decimal FaxingAmount, decimal ReceivingAmount, string Country)
        {

            dbContext = new FAXEREntities();

            string ReceivingCountryName = "United Kingdom";
            var ReceivingCountry = dbContext.Country.Where(x => x.CountryName.ToLower() == ReceivingCountryName.ToLower())
                                   .Select(x => x.CountryCode).FirstOrDefault();

            decimal exchangeRate = 0;
            var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == Country && x.CountryCode2 == ReceivingCountry).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateobj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == ReceivingCountry && x.CountryCode2 == Country).FirstOrDefault();
                if (exchangeRateobj2 != null)
                {
                    exchangeRateObj = exchangeRateobj2;
                    exchangeRate = Math.Round(1 / exchangeRateObj.CountryRate1, 6, MidpointRounding.AwayFromZero);
                }

            }
            else
            {
                exchangeRate = exchangeRateObj.CountryRate1;
            }

            if (ReceivingCountry == Country)
            {

                exchangeRate = 1m;

            }
            if (exchangeRate == 0)
            {
                return false;
            }
            //   exchangeRate = Convert.ToDouble(dbContext.ExchangeRates.Where(x => x.CountryCode1 == ReceivingCountryCode && x.CountryCode2==FaxingCountryCode).Select(x => x.CountryRate2).FirstOrDefault());
            if (ReceivingAmount > 0)
            {
                FaxingAmount = ReceivingAmount;
            }
            //var feeSummary = Services.SEstimateFee.CalculateFaxingFee(model.FaxingAmount, model.IncludeFaxingFee == "Yes", model.ReceivingAmount > 0, exchangeRateObj.CountryRate1, exchangeRateObj.FaxingFee1 ?? 0);
            var feeSummary = Services.SEstimateFee.CalculateFaxingFee(FaxingAmount, false, ReceivingAmount > 0, exchangeRate, 0);


            if (feeSummary.ReceivingAmount < 1000)
            {

                return true;
            }
            return false;
        }

        internal static string GenerateBankPaymentReceiptNo()
        {
            dbContext = new FAXEREntities();
            var paymentReference = "MD" + GenerateRandomDigit(5);

            while (dbContext.BankAccountDeposit.Where(x => x.PaymentReference == paymentReference).Count() > 0
                || dbContext.MobileMoneyTransfer.Where(x => x.PaymentReference == paymentReference).Count() > 0
                || dbContext.FaxingNonCardTransaction.Where(x => x.PaymentReference == paymentReference).Count() > 0)
            {
                GenerateBankPaymentReceiptNo();
            }
            return paymentReference;
        }
        internal static string GenerateFundAccountPaymentRefrence()
        {
            dbContext = new FAXEREntities();
            var paymentReference = "MD" + GenerateRandomDigit(5);

            while (dbContext.AgentFundAccount.Where(x => x.PaymentReference == paymentReference).Count() > 0)
            {
                GenerateFundAccountPaymentRefrence();
            }
            return paymentReference;
        }

        internal static int GetTotalTransactionCount(int id)
        {

            dbContext = new FAXEREntities();
            var count = 0;
            count = count + dbContext.FaxingNonCardTransaction.Count(x => x.NonCardReciever.FaxerID == id && x.FaxingStatus != FaxingStatus.PaymentPending);
            count = count + dbContext.BankAccountDeposit.Count(x => x.SenderId == id && x.Status != BankDepositStatus.PaymentPending);
            count = count + dbContext.MobileMoneyTransfer.Count(x => x.SenderId == id && x.Status != MobileMoneyTransferStatus.PaymentPending);
            //count = count + dbContext..Count(x => x.SenderId == id);

            return count;

        }
        internal static int GetTotalTransactionAgentCount(int AgentId, int payingStaffId)
        {

            dbContext = new FAXEREntities();
            var count = 0;
            var bankdeposit = (from c in dbContext.BankAccountDeposit.Where(x => x.PayingStaffId == payingStaffId && x.PaidFromModule == DB.Module.Agent)
                               join d in dbContext.AgentStaffInformation on c.PayingStaffId equals d.Id
                               where d.AgentId == AgentId
                               select c).Count();

            var cashpickup = (from c in dbContext.FaxingNonCardTransaction.Where(x => x.PayingStaffId == payingStaffId && x.OperatingUserType == OperatingUserType.Agent)
                              join d in dbContext.AgentStaffInformation on c.PayingStaffId equals d.Id
                              where d.AgentId == AgentId
                              select c).Count();

            var mobiletransfer = (from c in dbContext.MobileMoneyTransfer.Where(x => x.PayingStaffId == payingStaffId && x.PaidFromModule == DB.Module.Agent)
                                  join d in dbContext.AgentStaffInformation on c.PayingStaffId equals d.Id
                                  where d.AgentId == AgentId
                                  select c).Count();
            count = bankdeposit + cashpickup + mobiletransfer;
            //count = count + dbContext..Count(x => x.SenderId == id);

            return count;

        }

        internal static void SetTransactionEmailTypeSession(TransactionEmailType transactionEmailType)
        {
            FaxerSession.TransactionEmailTypeSession = transactionEmailType;
        }

        public static string GetUserFullName(this IIdentity identity)
        {
            //dbContext = new DB.FAXEREntities();
            //string firstName = dbContext.AspNetUsers.Where(x => x.UserName == identity.Name).Select(x => x.FirstName).FirstOrDefault();
            //string lastName = dbContext.AspNetUsers.Where(x => x.UserName == identity.Name).Select(x => x.LastName).FirstOrDefault();
            if (FaxerSession.LoggedUser != null)
            {
                return FaxerSession.LoggedUser.FullName;
            }
            else
            {
                return "";
            }
        }

        public static bool HasOneCardSaved()
        {
            try
            {
                dbContext = new DB.FAXEREntities();
                var data = dbContext.SavedCard.Where(x => x.UserId == FaxerSession.LoggedUser.Id && x.IsDeleted == false).ToList();
                if (data.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }
        public static bool HasOneCardSaved(int userId, DB.Module module = DB.Module.Faxer)
        {
            try
            {
                dbContext = new DB.FAXEREntities();
                var data = dbContext.SavedCard.Where(x => x.UserId == userId && x.Module == module && x.IsDeleted == false).ToList();
                if (data.Count > 0)
                {
                    return true;
                }
            }
            catch (Exception)
            {

            }

            return false;
        }
        internal static ServiceResult<bool> ValidationOfSenderMoneyCardCard(SenderAddMoneyCardViewModel vm)
        {

            var serviceResult = new ServiceResult<bool>()
            {
                Data = true,
                Message = "Success",
                Status = ResultStatus.OK
            };
            if (string.IsNullOrEmpty(vm.CardNumber))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter card number";
                return serviceResult;
            }
            if (string.IsNullOrEmpty(vm.ExpiringDateYear))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter date";
                return serviceResult;
            }

            if (string.IsNullOrEmpty(vm.SecurityCode))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter security code";
                return serviceResult;
            }
            return serviceResult;
        }

        public static bool IsValidDigitCount(string CountryCode, string accountNo)
        {
            switch (CountryCode)
            {
                case "NG":
                    if (accountNo.Length < 10 || accountNo.Length > 10)
                    {
                        return false;
                    }
                    break;
                default:
                    break;
            }
            return true;
        }

        internal static string GetCreditCardMasked(string v)
        {
            string val = "**** **** **** " + v.Substring(v.Length - 4, 4);
            return val;
        }

        internal static string GetCountryCodeByCurrency(string currency)
        {
            dbContext = new FAXEREntities();
            string code = dbContext.Country.Where(x => x.Currency.ToLower() == currency).Select(x => x.CountryCode).FirstOrDefault();
            return code;
        }


        //Get Country Phone Code
        public static string GetCountryPhoneCode(this string CountryCode)
        {
            dbContext = new DB.FAXEREntities();
            var CountryDetalls = dbContext.Country.Where(x => x.CountryCode == CountryCode).FirstOrDefault();
            if (CountryDetalls != null)
            {

                return CountryDetalls.CountryPhoneCode;
            }
            return "";
        }

        public static decimal GetExchangeRate(string sendingCountry, string receivingCountry)
        {

            dbContext = new DB.FAXEREntities();
            if (sendingCountry.ToLower() == receivingCountry.ToLower())
            {
                return 1M;
            }
            var exchangeRate = dbContext.ExchangeRate.Where(x => x.CountryCode1 == sendingCountry && x.CountryCode2 == receivingCountry).Select(x => x.Rate).FirstOrDefault();

            return Math.Round(exchangeRate, 3);

        }


        public static List<SavedDebitCreditCardDropDownVm> GetSavedCardDetails()
        {

            dbContext = new DB.FAXEREntities();
            var result = (from c in dbContext.SavedCard.Where(x => x.UserId == FaxerSession.LoggedUser.Id).ToList()
                          select new SavedDebitCreditCardDropDownVm()
                          {
                              Id = c.Id,
                              CardNo = "**** **** **** " + c.Num.Decrypt().Substring(c.Num.Decrypt().Length - 4, 4)

                          }).ToList();


            return result;
        }

        public static List<CountryViewModel> GetCountriesForDropDown()
        {

            dbContext = new DB.FAXEREntities();
            var result = (from c in dbContext.Country.ToList()
                          select new CountryViewModel()
                          {
                              CountryCode = c.CountryCode,
                              CountryName = c.CountryName
                          }).ToList();

            return result;
        }



        // get auto payment frequency details 
        public static string GetPaymentFrequncyDetail(AutoPaymentFrequency paymentFrequency, string frequencyDetail)
        {

            string FrequencyDetail = "None";
            var paymentDay = Convert.ToInt32(frequencyDetail);
            if (paymentFrequency == AutoPaymentFrequency.Weekly)
            {
                FrequencyDetail = "Every " + Enum.GetName(typeof(DayOfWeek), paymentDay);
            }
            else if (paymentFrequency == AutoPaymentFrequency.Monthly)
            {
                string abbreviation = "";
                if (paymentDay == 01 || paymentDay == 21 || paymentDay == 31)
                {

                    abbreviation = "st";
                }
                else if (paymentDay == 02 || paymentDay == 22)
                {
                    abbreviation = "nd";
                }
                else if (paymentDay == 03 || paymentDay == 23)
                {
                    abbreviation = "rd";
                }
                else
                {
                    abbreviation = "th";
                }

                FrequencyDetail = "Every " + paymentDay + abbreviation;
            }
            else if (paymentFrequency == AutoPaymentFrequency.Yearly)
            {
                string PaymentDate = frequencyDetail;
                int Month = int.Parse(PaymentDate.Substring(0, 2));
                int Day = int.Parse(PaymentDate.Substring(2, 2));
                string MonthName = Enum.GetName(typeof(Month), Month);
                FrequencyDetail = "Every " + MonthName;

            }
            else
            {
                FrequencyDetail = "None";

            }
            return FrequencyDetail;


        }

        internal static ServiceResult<bool> ValidateCreditDebitCard(CreditDebitCardViewModel model)
        {
            var serviceResult = new ServiceResult<bool>()
            {
                Data = true,
                Message = "Success",
                Status = ResultStatus.OK
            };
            if (string.IsNullOrEmpty(model.CardNumber))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter card number";
                return serviceResult;
            }
            if (string.IsNullOrEmpty(model.EndMM))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter month";
                return serviceResult;
            }
            if (string.IsNullOrEmpty(model.EndYY))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter year";
                return serviceResult;
            }
            if (string.IsNullOrEmpty(model.SecurityCode))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter security code";
                return serviceResult;
            }
            if (string.IsNullOrEmpty(model.NameOnCard))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter name on card";
                return serviceResult;
            }
            if (!string.IsNullOrEmpty(model.NameOnCard))
            {
                //FAXEREntities db = new FAXEREntities();
                //int senderId = FaxerSession.LoggedUser.Id;
                //var lastName = db.FaxerInformation.Where(x => x.Id == senderId).Select(x => x.LastName).FirstOrDefault();
                //bool Contains = model.NameOnCard.Contains(lastName);
                //if (!Contains)
                //{
                //    serviceResult.Data = false;
                //    serviceResult.Message = "Name On card doesn't match";
                //    return serviceResult;
                //}
            }

            int senderId = 0;
            if (FaxerSession.LoggedUser != null)
            {
                senderId = FaxerSession.LoggedUser.Id;
            }

            SCreditDebitCardUsage creditDebitCardUsage = new SCreditDebitCardUsage();
            var cardusage = creditDebitCardUsage.HasExceededUsageLimit(new CreditCardUsageLog()
            {

                CardNum = model.CardNumber,
                Module = DB.Module.Faxer,
                SenderId = senderId,
                UpdatedDateTime = DateTime.Now
            });
            if (cardusage.Data)
            {

                serviceResult.Data = false;
                serviceResult.Message = cardusage.Message;
                serviceResult.Status = cardusage.Status;
            }
            return serviceResult;
        }

        internal static ServiceResult<bool> SecureTradingValidateCreditDebitCard(StripeResultIsValidCardVm model)
        {
            var serviceResult = new ServiceResult<bool>()
            {
                Data = true,
                Message = "Success",
                Status = ResultStatus.OK
            };
            if (string.IsNullOrEmpty(model.Number))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter card number";
                return serviceResult;
            }
            if (string.IsNullOrEmpty(model.ExpirationMonth))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter month";
                return serviceResult;
            }
            if (string.IsNullOrEmpty(model.ExpiringYear))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter year";
                return serviceResult;
            }
            if (string.IsNullOrEmpty(model.SecurityCode))
            {

                serviceResult.Data = false;
                serviceResult.Message = "Enter security code";
                return serviceResult;
            }
            return serviceResult;
        }

        public static string ToHash(this string input)
        {
            StringBuilder sb = new StringBuilder();
            HashAlgorithm alg = MD5.Create();
            byte[] field = alg.ComputeHash(Encoding.UTF8.GetBytes(input));
            //foreach (var item in field)
            //{
            //    sb.Append(item.ToString("x2"));

            //}
            var outPut = BitConverter.ToString(field).Replace("-", String.Empty);
            return outPut;

            //another way
            ///return input.GetHashCode().ToString();
        }

        /// <summary>
        /// MFS-xxx-xxx-1234-firstname 
        /// this format is only for faxer
        /// agent and admin will see full number
        /// 
        /// </summary>
        /// <param name="mftcCardNumber">card number stored in database</param>
        /// <returns></returns>
        internal static string FormatMFTCCard(this string mftcCardNumber)
        {
            //split into three
            //mfs
            //number
            //firstname
            var mftcs = mftcCardNumber.Split('-');
            var number = mftcs[1];

            //string MFTC = "MFS-xxx-xxx-" + number.Substring(6) + "-" + ConvertTo_ProperCase(mftcs[2]);

            ////MFS-xxx-xxx-1234-Binod

            string MFTC = number + "-" + ConvertTo_ProperCase(mftcs[2]);

            return MFTC;
        }
        internal static string FormatSavedCardNumber(this string CardNumber)
        {
            //split into three
            //mfs
            //number
            //firstname



            //1234-1234-1234-1234
            //xxxx-xxxx-xxxx-1234
            if (CardNumber.Length == 16)
            {
                return "xxxx-" + CardNumber.Right(4);
            }
            return "";
        }
        internal static string FormatSavedBankNumber(this string AccountNumber)
        {
            //split into three
            //mfs
            //number
            //firstname



            //1234-1234-1234-1234
            //xxxx-xxxx-xxxx-1234
            if (AccountNumber.Length >= 4)
            {
                return "xxxx-" + AccountNumber.Right(4);
            }
            return AccountNumber;
        }



        internal static string FormatMFBCCard(this string mfbcCardNumber)
        {
            //split into three
            //mfs
            //number
            //firstname
            var mfbcs = mfbcCardNumber.Split('-');
            var number = mfbcs[1];

            string MFTC = "MFBC-xxx-xxx-" + number.Substring(6) + "-" + ConvertTo_ProperCase(mfbcs[2]);
            //MFBC-xxx-xxx-1234-Binod
            return MFTC;
        }
        internal static string Right(this string mftcCardNumber, int length)
        {
            //123456789 : 4
            //9-4=5 
            try
            {
                return mftcCardNumber.Substring(mftcCardNumber.Length - length);
            }
            catch (Exception ex)
            {

                return "";
            }

        }

        static readonly string PasswordHash = "P@@Sw0rd";
        static readonly string SaltKey = "S@LT&KEY";
        static readonly string VIKey = "@1B2c3D4e5F6g7H8";

        //static readonly string PasswordHash = GetAppSettingValue("PasswordHash");
        //static readonly string SaltKey = GetAppSettingValue("SaltKey");
        //static readonly string VIKey = GetAppSettingValue("VIKey");

        public static string Encrypt(this string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash1, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            return Convert.ToBase64String(cipherTextBytes);
        }

        internal static DocumentApprovalStatus? GetSenderIdentificationStatus(int id)
        {

            dbContext = new FAXEREntities();
            var result = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == id).FirstOrDefault();
            if (result != null)
            {
                return result.Status;
            }
            return null;

        }

        internal static bool IsSenderIdApproved(int id, RegisteredAgentType agentType)
        {
            switch (agentType)
            {
                case RegisteredAgentType.Agent:

                    return true;
                    break;
                case RegisteredAgentType.AuxAgent:

                    dbContext = new FAXEREntities();
                    var result = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == id).FirstOrDefault();
                    if (result != null)
                    {
                        switch (result.Status)
                        {
                            case DocumentApprovalStatus.Select:
                                break;
                            case DocumentApprovalStatus.Approved:
                                return true;
                                break;
                            case DocumentApprovalStatus.Disapproved:
                                return false;
                                break;
                            case DocumentApprovalStatus.InProgress:
                                return false;
                                break;
                            default:
                                return false;
                                break;
                        }
                    }
                    return false;
                    break;
                default:
                    break;
            }
            return false;

        }
        internal static DocumentApprovalStatus? GetStaffIdentificationStatus(int id)
        {

            dbContext = new FAXEREntities();
            var result = dbContext.StaffDocumentation.Where(x => x.StaffId == id).FirstOrDefault();
            if (result != null)
            {
                return result.Status;
            }
            return null;

        }
        public static bool IsSenderIdCheckInProgress(int id)
        {

            var documentStatus = GetSenderIdentificationStatus(id);
            if (documentStatus == null)
            {

                return true;
            }
            switch (documentStatus)
            {
                case null:
                    return true;
                    break;
                case DocumentApprovalStatus.Approved:
                    return false;
                case DocumentApprovalStatus.Disapproved:
                    return true;
                case DocumentApprovalStatus.InProgress:
                    return true;
                default:
                    return true;
                    break;
            }
            return false;
        }

        public static bool IsStaffIdCheckInProgress(int id)
        {

            var documentStatus = GetStaffIdentificationStatus(id);
            if (documentStatus == null)
            {

                return true;
            }
            switch (documentStatus)
            {
                case null:
                    return true;
                    break;
                case DocumentApprovalStatus.Approved:
                    return false;
                case DocumentApprovalStatus.Disapproved:
                    return true;
                case DocumentApprovalStatus.InProgress:
                    return true;
                default:
                    return true;
                    break;
            }
            return false;
        }

        public static string Decrypt(this string encryptedText)
        {
            if (encryptedText == null)
                return "";
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash1, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());

        }

        internal static string GetCountryFlagCode(this string CountryCode)
        {
            dbContext = new DB.FAXEREntities();
            string FlagCode = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.FlagCode).FirstOrDefault();
            return FlagCode;
        }

        internal static string getBankName(int bankId)
        {
            dbContext = new FAXEREntities();
            var name = dbContext.Bank.Where(x => x.Id == bankId).Select(x => x.Name).FirstOrDefault();
            return name;
        }

        internal static Bank getBank(int bankId)
        {
            dbContext = new FAXEREntities();
            var bank = dbContext.Bank.Where(x => x.Id == bankId).FirstOrDefault();
            return bank;
        }


        public static decimal GetSmsFee(string countryCode)
        {
            dbContext = new DB.FAXEREntities();
            decimal fee = dbContext.SmsFee.Where(x => x.CountryCode == countryCode && x.IsDeleted == false).Select(x => x.SmsFee).FirstOrDefault();
            return fee;
        }
        public static decimal GetMargin(decimal MFRate, decimal AgentRate, decimal SendingAmount, decimal Fee)
        {
            decimal magrin = ((MFRate - AgentRate) * SendingAmount) / MFRate + Fee;
            return magrin;
        }
        public static decimal GetMFRate(string SendingCountry, string ReceivingCountry, TransactionTransferMethod TransferMethod)
        {
            dbContext = new FAXEREntities();
            var datetime = DateTime.Now.Date;
            var MFRate = dbContext.TransferExchangeRateHistory.Where(x => x.SendingCountry == SendingCountry && x.ReceivingCountry == ReceivingCountry
            && DbFunctions.TruncateTime(x.CreatedDate) == datetime && x.TransferMethod == TransferMethod).Select(x => x.Rate).FirstOrDefault();
            if (MFRate == 0)
            {
                MFRate = dbContext.TransferExchangeRateHistory.Where(x => x.SendingCountry == SendingCountry && x.ReceivingCountry == ReceivingCountry
              && DbFunctions.TruncateTime(x.CreatedDate) == datetime && x.TransferMethod == TransactionTransferMethod.All).Select(x => x.Rate).FirstOrDefault();
                if (MFRate == 0)
                {
                    return 1;

                }
            }
            return MFRate;
        }

        public static string GetVirtualAccountNo(this string number)
        {


            string[] token = number.Split('-');
            if (token.Length > 2)
            {
                return token[1];

            }
            return token[0];

        }

        public static string GetCountryCodeByName(this string Code)
        {

            dbContext = new FAXEREntities();
            var result = dbContext.Country.Where(x => x.CountryCode.ToLower() == Code.ToLower()).Select(x => x.CountryName).FirstOrDefault();
            return result;
        }
        public static string GetCountryCodeByCountryName(this string Code)
        {

            dbContext = new FAXEREntities();
            var result = dbContext.Country.Where(x => x.CountryName.ToLower() == Code.ToLower()).Select(x => x.CountryCode).FirstOrDefault();
            return result;
        }
        public static List<string> GetCountriesByCurrency(this string Currency)
        {
            dbContext = new FAXEREntities();
            var result = dbContext.Country.Where(x => x.Currency.ToLower() == Currency.ToLower()).Select(x => x.CountryCode).ToList();
            return result;
        }
        public static BillingAddressViewModel GetBillingAddress(int SavedCardId)
        {


            dbContext = new DB.FAXEREntities();

            int FaxerId = dbContext.SavedCard.Where(x => x.Id == SavedCardId).Select(x => x.UserId).FirstOrDefault();

            var result = (from c in dbContext.FaxerInformation.Where(x => x.Id == FaxerId).ToList()
                          select new BillingAddressViewModel()
                          {
                              Address1 = c.Address1,
                              Address2 = c.Address2,
                              City = c.City,
                              Country = c.Country,
                              PostalCode = c.PostalCode
                          }).FirstOrDefault();

            return result;

        }

        public static decimal GetAgentSendingCommission(TransferService transferService, int AgentId, decimal amount, decimal Fee)
        {
            dbContext = new DB.FAXEREntities();
            var commisiondata = new AgentCommission();
            commisiondata = dbContext.AgentCommission.Where(x => x.TransferSevice == transferService && x.AgentId == AgentId).FirstOrDefault();
            if (commisiondata == null)
            {
                commisiondata = dbContext.AgentCommission.Where(x => x.TransferSevice == TransferService.All && x.AgentId == AgentId).FirstOrDefault();
                if (commisiondata == null)
                {
                    commisiondata = dbContext.AgentCommission.Where(x => x.TransferSevice == transferService).FirstOrDefault();

                    if (commisiondata == null)
                    {
                        commisiondata = dbContext.AgentCommission.Where(x => x.TransferSevice == TransferService.All).FirstOrDefault();
                    }
                }
            }
            try
            {
                decimal CommissonAmount = 0;
                switch (commisiondata.CommissionType)
                {
                    case CommissionType.Select:
                        break;
                    case CommissionType.CommissionOnFee:
                        CommissonAmount = Fee * ((decimal)commisiondata.SendingRate / 100);
                        break;
                    case CommissionType.CommissionOnAmount:
                        CommissonAmount = amount * ((decimal)commisiondata.SendingRate / 100);
                        break;
                    case CommissionType.FlatFee:
                        CommissonAmount = (decimal)commisiondata.SendingRate;
                        break;
                    default:
                        break;
                }
                return CommissonAmount;

            }
            catch (Exception ex)
            {

                return 0;
            }
            return 0;
        }

        public static decimal GetAgentReceivingCommission(TransferService transferService, int AgentId, decimal amount, decimal Fee)
        {

            dbContext = new DB.FAXEREntities();
            var commisiondata = new AgentCommission();
            commisiondata = dbContext.AgentCommission.Where(x => x.TransferSevice == transferService && x.AgentId == AgentId).FirstOrDefault();
            if (commisiondata == null)
            {
                commisiondata = dbContext.AgentCommission.Where(x => x.TransferSevice == TransferService.All && x.AgentId == AgentId).FirstOrDefault();
                if (commisiondata == null)
                {
                    commisiondata = dbContext.AgentCommission.Where(x => x.TransferSevice == transferService).FirstOrDefault();

                    if (commisiondata == null)
                    {
                        commisiondata = dbContext.AgentCommission.Where(x => x.TransferSevice == TransferService.All).FirstOrDefault();
                    }
                }
            }
            try
            {
                decimal CommissonAmount = 0;
                switch (commisiondata.CommissionType)
                {
                    case CommissionType.Select:
                        break;
                    case CommissionType.CommissionOnFee:
                        CommissonAmount = Fee * ((decimal)commisiondata.ReceivingRate / 100);
                        break;
                    case CommissionType.CommissionOnAmount:
                        CommissonAmount = amount * ((decimal)commisiondata.ReceivingRate / 100);
                        break;
                    case CommissionType.FlatFee:
                        CommissonAmount = (decimal)commisiondata.ReceivingRate;
                        break;
                    default:
                        break;
                }
                return CommissonAmount;

            }
            catch (Exception ex)
            {

                return 0;
            }
            return 0;
        }


        public static decimal GetPayoutProviderRate(string sendingCountry, string receivingCountry, Apiservice payoutProvider)
        {
            PayoutProviderRateServices _payoutProviderRateServices = new PayoutProviderRateServices();
            string sendingCurrency = GetCountryCurrency(sendingCountry);
            string receivingCurrency = GetCountryCurrency(receivingCountry);
            var rate = _payoutProviderRateServices.PayoutProviderRates().Where(x => x.SendingCurrency == sendingCurrency &&
                                                                          (x.SendingCountry == sendingCountry ||
                                                                           x.SendingCountry.ToLower() == "all") &&
                                                                           x.RecevingCurrency == receivingCurrency &&
                                                                          (x.RecevingCountry == receivingCountry ||
                                                                           x.RecevingCountry.ToLower() == "all") &&
                                                                           x.PayoutProvider == payoutProvider
                                                                          ).Select(x => x.Rate).FirstOrDefault();
            return rate;
        }
        public static decimal CreditTypeFee(string cardType, decimal amount)
        {
            decimal TotalAmount = 0;
            string VISA = "VISA";
            string MASTER = "MASTER";
            if (VISA == cardType || MASTER == cardType)
            {
                TotalAmount = amount + (amount * (decimal)(0.06));
                return TotalAmount;
            }
            return amount;
        }
        public static CustomerPaymentFee CustomerPaymentFee(string Country)
        {
            FAXEREntities dbContext = new FAXEREntities();
            var data = dbContext.CustomerPaymentFee.Where(x => x.Country == Country).FirstOrDefault();
            return data;
        }

        public static void KiiPayPersonalWalletBalanceIN(int Id, decimal Amount)
        {

            dbContext = new FAXEREntities();

            var KiiPayPersonalWalletInfo = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == Id).FirstOrDefault();
            KiiPayPersonalWalletInfo.CurrentBalance += Amount;
            dbContext.Entry<KiiPayPersonalWalletInformation>(KiiPayPersonalWalletInfo).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

        }
        public static void KiiPayPersonalWalletBalanceOut(int Id, decimal Amount)
        {

            dbContext = new FAXEREntities();

            var KiiPayPersonalWalletInfo = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == Id).FirstOrDefault();
            KiiPayPersonalWalletInfo.CurrentBalance = KiiPayPersonalWalletInfo.CurrentBalance - Amount;
            dbContext.Entry<KiiPayBusinessWalletInformation>(KiiPayPersonalWalletInfo).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

        }
        public static string ConvertTo_ProperCase(string text)
        {
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            return myTI.ToTitleCase(text.ToLower());
        }

        public static int GetAgentIdByPayingId(int? payingStaffId)
        {
            dbContext = new FAXEREntities();
            int agentId = 0;
            if (payingStaffId != null)
            {
                agentId = dbContext.AgentStaffInformation.Where(x => x.Id == payingStaffId).Select(x => x.AgentId).FirstOrDefault();
            }
            return agentId;
        }  public static int GetPayingIdByAgentId(int? agentId)
        {
            dbContext = new FAXEREntities();
            int payingStaffId = 0; 
            if (agentId != null)
            {
                payingStaffId = dbContext.AgentStaffInformation.Where(x => x.AgentId == agentId).Select(x => x.Id).FirstOrDefault();
            }
            return payingStaffId;
        }

        public static string GetAppSettingValue(this string key)
        {
            string value = System.Configuration.ConfigurationManager.AppSettings[key];
            return value;
        }
        public static string GmailUserName { get { return "GmailUserName"; } }
        public static string GmailPassword { get { return "GmailPassword"; } }

        public static string DemoUserName { get { return "DemoUsername"; } }
        public static string DemoPassword { get { return "DemoPassword"; } }


        public static string MoneyFexDemoUserName { get { return "MoneyFexDemoUsername"; } }
        public static string MoneyFexDemoPassword { get { return "MoneyFexDemoPassword"; } }

        public static string PasswordHash1 => PasswordHash;

        public static string GenerateRandomDigit(int length)
        {
            Random generator = new Random();
            int len = (int)Math.Pow(10, length);
            if (length == 10)
            {
                len = 2147483647;
            }
            string s = generator.Next(0, len).ToString("D" + length);

            return s;
        }

        public static string GenerateMobileMoneyTransferReceiptNo(int length)
        {
            dbContext = new FAXEREntities();
            var code = "OW" + GenerateRandomDigit(length);
            while (dbContext.MobileMoneyTransfer.Where(x => x.ReceiptNo == code).Count() > 0)
            {
                code = GenerateMobileMoneyTransferReceiptNo(length);

            }
            return code;
        }
        public static string GenerateMobileMoneyTransferReceiptNoForAgent(int length)
        {
            dbContext = new FAXEREntities();
            var code = "OW" + GenerateRandomDigit(length);
            while (dbContext.MobileMoneyTransfer.Where(x => x.ReceiptNo == code).Count() > 0)
            {
                code = GenerateMobileMoneyTransferReceiptNoForAgent(length);

            }
            return code;
        }

        public static string GeneratePayRequestReceiptNo(int length)
        {

            dbContext = new FAXEREntities();
            var code = "OW" + GenerateRandomDigit(length);
            while (dbContext.MobileMoneyTransfer.Where(x => x.ReceiptNo == code).Count() > 0)
            {
                code = GeneratePayRequestReceiptNo(length);
            }

            return code;
        }
        public static string GenerateCashPickUpReceiptNo(int length)
        {
            dbContext = new FAXEREntities();
            var code = "" + GenerateRandomDigit(length);
            while (dbContext.FaxingNonCardTransaction.Where(x => x.ReceiptNumber == code).Count() > 0)
            {
                code = GenerateCashPickUpReceiptNo(length);
            }

            return code;
        }
        public static string GenerateFundAccountReceiptNo(int length)
        {
            dbContext = new FAXEREntities();
            var code = "AF" + GenerateRandomDigit(length);
            while (dbContext.AgentFundAccount.Where(x => x.Receipt == code).Count() > 0)
            {
                code = GenerateFundAccountReceiptNo(length);
            }

            return code;
        }
        public static string GeneratePayAReceiverCashPickUpReceiptNo(int length)
        {
            dbContext = new FAXEREntities();
            var code = "CP" + GenerateRandomDigit(length);
            while (dbContext.ReceiverNonCardWithdrawl.Where(x => x.ReceiptNumber == code).Count() > 0)
            {
                code = GeneratePayAReceiverCashPickUpReceiptNo(length);
            }

            return code;
        }
        public static string GenerateKiiPayPersonalReceiptNo(int length)
        {

            dbContext = new FAXEREntities();
            var code = "KP" + GenerateRandomDigit(length);
            while (dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.ReceiptNumber == code).Count() > 0)
            {
                code = GenerateKiiPayPersonalReceiptNo(length);
            }

            return code;
        }
        public static string GeneratePayForServicesReceiptNo(int length)
        {

            dbContext = new FAXEREntities();
            var code = "KP" + GenerateRandomDigit(length);
            while (dbContext.FaxerMerchantPaymentTransaction.Where(x => x.ReceiptNumber == code).Count() > 0)
            {
                code = GeneratePayForServicesReceiptNo(length);
            }

            return code;
        }

        internal static MobileWalletOperator GetMobileWalletInfo(int walletOperatorId)
        {
            DB.FAXEREntities db = new FAXEREntities();
            var result = db.MobileWalletOperator.Where(x => x.Id == walletOperatorId).FirstOrDefault();

            return result ?? new MobileWalletOperator();
        }

        public static string GenerateBankAccountDepositReceiptNo(int length)
        {
            dbContext = new FAXEREntities();
            //var code = "BD" + GenerateRandomDigit(length);
            //while (dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == code).Count() > 0)
            //{
            //    GenerateBankAccountDepositReceiptNo(length);
            //}
            var code = dbContext.Sp_GetBankDepositReceiptNo_result(false).ReceiptNo;



            return code;

            return code;
        }
        public static string GenerateBankAccountDepositReceiptNoforAgnet(int length)
        {
            dbContext = new FAXEREntities();
            //var code = "BD" + GenerateRandomDigit(length);
            //while (dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == code).Count() > 0)
            //{
            //    GenerateBankAccountDepositReceiptNoforAgnet(length);
            //}
            var code = dbContext.Sp_GetBankDepositReceiptNo_result(false).ReceiptNo;



            return code;
            return code;
        }
        public static string GenerateManualBankAccountDepositReceiptNo(int length)
        {
            dbContext = new FAXEREntities();
            //var code = "MBD" + GenerateRandomDigit(length);
            //while (dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == code).Count() > 0)
            //{
            //    GenerateManualBankAccountDepositReceiptNo(length);
            //}

            var code = dbContext.Sp_GetBankDepositReceiptNo_result(true).ReceiptNo;



            return code;
            return code;
        }

        public static string GenerateAgentPayBillMonthlyReceiptNo(int length)
        {
            dbContext = new FAXEREntities();
            var code = "PM" + GenerateRandomDigit(length);
            while (dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == code).Count() > 0)
            {
                GenerateAgentPayBillMonthlyReceiptNo(length);
            }
            return code ?? "";
        }


        public static string GenerateAgentPayBillTopUpReceiptNo(int length)
        {
            dbContext = new FAXEREntities();
            var code = "TU" + GenerateRandomDigit(length);
            while (dbContext.BankAccountDeposit.Where(x => x.ReceiptNo == code).Count() > 0)
            {
                GenerateAgentPayBillTopUpReceiptNo(length);
            }

            return code ?? "";
        }


        public static string GenerateRandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            string password = builder.ToString() + "" + builder.ToString().Substring(builder.Length - 1, 1).ToLower() + '$' + GenerateRandomDigit(3);
            //if (lowerCase)
            //    return  builder.ToString().ToLower();
            return password;
        }
        public static bool ValidatePassword(string password, int minLength = 8)
        {
            int MIN_LENGTH = minLength;
            int MAX_LENGTH = 15;

            if (password == null) throw new ArgumentNullException();

            bool meetsLengthRequirements = password.Length >= MIN_LENGTH && password.Length <= MAX_LENGTH;
            bool hasUpperCaseLetter = true;
            bool hasLowerCaseLetter = true;
            bool hasDecimalDigit = false;
            bool hasSpecialCharacter = true;

            if (meetsLengthRequirements)
            {
                foreach (char c in password)
                {
                    if (char.IsDigit(c)) hasDecimalDigit = true;
                    //if (char.IsUpper(c)) hasUpperCaseLetter = true;
                    //else if (char.IsLower(c)) hasLowerCaseLetter = true;
                    //else if (char.IsDigit(c)) hasDecimalDigit = true;
                    //else if (!char.IsLetterOrDigit(c)) hasSpecialCharacter = true;
                }

            }

            bool isValid = meetsLengthRequirements
                        && hasUpperCaseLetter
                        && hasLowerCaseLetter
                        && hasDecimalDigit
                        && hasSpecialCharacter;

            return isValid;

        }
        /// <summary>
        /// Sms Verification Code For registration 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateVerificationCode(int length)
        {

            DB.FAXEREntities db = new DB.FAXEREntities();
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(10).ToString());
            //return s;
            while (db.RegistrationVerificationCode.Where(x => x.VerificationCode == s).Count() > 0)
            {
                for (int i = 0; i < length; i++)
                    s = String.Concat(s, random.Next(10).ToString());
            }
            return s;
        }
        public static string GetEnumDescription<T>(this T value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }



        /// <summary>
        /// get html template as string from controller and action
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static string GetTemplate(string link)
        {
            try
            {
                using (var myWebClient = new WebClient())
                {

                    ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, errors) => { return true; };
                    //ServicePointManager.SecurityProtocol = (SecurityProtocolType)3072  | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                    //ServicePointManager.DefaultConnectionLimit = 9999;
                    //myWebClient.Headers["User-Agent"] = "MOZILLA/5.0 (WINDOWS NT 6.1; WOW64) APPLEWEBKIT/537.1 (KHTML, LIKE GECKO) CHROME/21.0.1180.75 SAFARI/537.1";
                    myWebClient.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:24.0) Gecko/20100101 Firefox/24";

                    string page = myWebClient.DownloadString(link);

                    return page;
                }
            }
            catch (Exception ex)
            {
                Log.Write(ex.Message);
            }

            return "";
        }
        public static PdfDocument GetPdf(string link)
        {
            // instantiate the html to pdf converter
            HtmlToPdf converter = new HtmlToPdf();
            converter.Options.PdfPageSize = PdfPageSize.A4;
            converter.Options.PdfPageOrientation = PdfPageOrientation.Portrait;
            // convert the url to pdf


            //string url = "/Registration/Card?code=" + Id;
            PdfDocument doc = converter.ConvertUrl(link);

            // Convert the HTML page given by an URL to a PDF document in a memory buffer
            // byte[] outPdfBuffer = doc.Save();

            return doc;

        }
        public static string GetCountryCurrency(string CountryCode)
        {
            dbContext = new DB.FAXEREntities();
            var result = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.Currency).FirstOrDefault();
            return result;

        }
        public static string GetCountryCurrencyName(string CountryCode)
        {
            dbContext = new DB.FAXEREntities();
            var result = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.CurrencyName).FirstOrDefault();
            return result;

        }

        public static List<DB.Country> GetCountries()
        {
            dbContext = new DB.FAXEREntities();
            var result = dbContext.Country.ToList();
            return result;

        }
        public static List<CountryViewModel> GetSendingCountries()
        {

            dbContext = new DB.FAXEREntities();
            var enabledCountries = dbContext.TransferServiceMaster.GroupBy(x => x.SendingCountry).Select(x => x.FirstOrDefault()).ToList();
            var result = (from c in enabledCountries
                          join d in dbContext.Country on c.SendingCountry equals d.CountryCode
                          select new CountryViewModel()
                          {
                              CountryCode = d.CountryCode,
                              CountryName = d.CountryName,
                              FlagCountryCode = d.CountryCode,
                              Currency =  d.Currency.ToUpper()
                          }).GroupBy(x => x.CountryCode).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }
        public static List<CountryViewModel> GetReceivingCountries()
        {

            dbContext = new DB.FAXEREntities();
            //var enabledCountries = dbContext.TransferServiceMaster.GroupBy(x => x.ReceivingCountry).Select(x => x.FirstOrDefault()).ToList();
            var enabledCountries = (from c in dbContext.TransferServiceMaster
                                    group c by new
                                    {
                                        c.SendingCountry,
                                        c.ReceivingCountry,
                                        c.ReceivingCurrency,
                                    } into gcs
                                    select gcs.FirstOrDefault()).ToList();
            var result = (from c in enabledCountries
                              //join d in dbContext.Country on c.ReceivingCountry equals d.CountryCode
                          select new CountryViewModel()
                          {
                              CountryCode = c.ReceivingCountry,
                              CountryName = Common.GetCountryName(c.ReceivingCountry),
                              FlagCountryCode = Common.GetCountryName(c.ReceivingCountry),
                              Currency = (c.ReceivingCurrency == null ? Common.GetCurrencyCode(c.ReceivingCountry)
                                          : c.ReceivingCurrency.ToUpper()),
                              CountryWithCurrency =  
                                                   (c.ReceivingCurrency == null ? Common.GetCurrencyCode(c.ReceivingCountry)
                                                   : c.ReceivingCurrency.ToUpper())
                          }).GroupBy(x => x.CountryCode).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }

        public static List<CountryViewModel> GetRecentTransferCountries()
        {


            dbContext = new DB.FAXEREntities();
            var result = (from d in dbContext.Country.ToList()
                          select new CountryViewModel()
                          {
                              CountryCode = d.CountryCode,
                              CountryName = d.CountryName,
                              FlagCountryCode = d.CountryCode,
                              Currency = d.Currency.ToUpper()
                          }).ToList();
            return result;

        }

        public static List<DB.PartnerInformation> GetPartners()
        {

            dbContext = new DB.FAXEREntities();
            var result = dbContext.PartnerInformation.ToList();
            return result;

        }
        public static List<DB.IdentityCardType> GetIdTypes()
        {

            dbContext = new DB.FAXEREntities();
            var result = dbContext.IdentityCardType.ToList();
            return result;

        }


        public static string GetCurrencySymbol(string CountryCode)
        {
            dbContext = new DB.FAXEREntities();
            var result = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.CurrencySymbol).FirstOrDefault();
            return result;

        }
        public static string GetCurrencySymbolByCurrencyCode(string code)
        {
            dbContext = new DB.FAXEREntities();
            var result = dbContext.Country.Where(x => x.Currency == code).Select(x => x.CurrencySymbol).FirstOrDefault();
            return result;

        }
        public static string GetCurrencySymbolByCurrency(string currency)
        {
            dbContext = new DB.FAXEREntities();
            var result = dbContext.Country.Where(x => x.Currency == currency).Select(x => x.CurrencySymbol).FirstOrDefault();
            return result;

        }
        public static string GetCurrencyCode(string CountryCode)
        {
            dbContext = new DB.FAXEREntities();
            var result = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.Currency).FirstOrDefault();
            return result;

        }
        public static string GetCountryName(string CountryCode)
        {

            if (!string.IsNullOrEmpty(CountryCode) && CountryCode.Trim().ToLower() == "all")
            {

                return "All";
            }
            dbContext = new DB.FAXEREntities();
            var Country = dbContext.Country.Where(x => x.CountryCode == CountryCode).Select(x => x.CountryName).FirstOrDefault();
            if (Country == null) {

                return "";
            }
            return Country;
        }

        public static string GetCurrencyByCurrencyOrCountry(string currency, string countryCode)
        {
            return currency != null ? currency : GetCountryCurrency(countryCode);
        }
        public static string GetCurrencySymbolByCurrencyOrCountry(string currency, string countryCode)
        {
            return currency != null ? GetCurrencySymbolByCurrency(currency) : GetCurrencySymbol(countryCode);
        }
        public static string GetIDCardTypeName(int IDCardTypeID)
        {
            dbContext = new DB.FAXEREntities();
            try
            {


                var IDCardTypName = dbContext.IdentityCardType.Where(x => x.Id == IDCardTypeID).Select(x => x.CardType).FirstOrDefault();
                return IDCardTypName;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static string GetRangeName(string Range)
        {
            var range = "";
            if (string.IsNullOrEmpty(Range)) {
                return "";
            }
            switch (Range)
            {
                case "0.00-0.00":
                    range = "All";
                    break;
                case "1.00-100.00":
                    range = "1-100";
                    break;
                case "101.00-500.00":
                    range = "101-500";
                    break;
                case "501.00-1000.00":
                    range = "501-1000";
                    break;
                case "1001.00-1500.00":
                    range = "1001-1500";
                    break;
                case "2001.00-3000.00":
                    range = "2001-3000";
                    break;
                case "3001.00-5000.00":
                    range = "3001-5000 ";
                    break;
                case "5001.00-10000.00":
                    range = "5001-10000";
                    break;
                case "11000.00-2147483647.00":
                    range = "11000+";
                    break;
            }
            return range;
        }

        public static List<DropDownViewModel> GetIdCardType()
        {
            dbContext = new DB.FAXEREntities();
            var result = (from c in dbContext.IdentityCardType.Where(x => x.IsDeleted == false)
                          select new DropDownViewModel()
                          {
                              Id = c.Id,
                              Name = c.CardType
                          }).ToList();
            return result;
        }
        public static bool compareFaxerNameOnCard(string nameOnCard, int faxerId)
        {
            dbContext = new DB.FAXEREntities();
            var faxerName = dbContext.FaxerInformation.Where(x => x.Id == faxerId).FirstOrDefault();

            string faxerFirstName = faxerName.FirstName.ToLower();
            string faxerMiddleName = faxerName.MiddleName == null ? "" : faxerName.MiddleName.ToLower();
            string faxerLastName = faxerName.LastName.ToLower();

            nameOnCard = nameOnCard.TrimEnd();
            var name = nameOnCard.Split(' ');

            List<string> cardNames = new List<string>(name);
            foreach (var item in name)
            {
                if (item == "")
                {
                    cardNames.Remove(item);
                }
            }

            if ((cardNames.Count == 2) && (string.IsNullOrEmpty(faxerMiddleName)))
            {
                string cardFirstName = cardNames[0].ToLower();
                string cardSecondName = cardNames[1].ToLower();
                if (cardFirstName == faxerFirstName && cardSecondName == faxerLastName)
                {
                    return true;
                }
            }
            else if ((cardNames.Count == 3) && (!string.IsNullOrEmpty(faxerMiddleName)))
            {
                string cardFirstName = cardNames[0].ToLower();
                string cardSecondName = cardNames[1].ToLower();
                string cardThirdName = cardNames[2].ToLower();
                if (cardFirstName == faxerFirstName && cardSecondName == faxerMiddleName && cardThirdName == faxerLastName)
                {
                    return true;
                }

            }
            return false;
        }

        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }

        public static void DeductCreditOnCard(int MFTCCardId, decimal Amount)
        {

            dbContext = new FAXEREntities();
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == MFTCCardId).FirstOrDefault();
            data.CurrentBalance = data.CurrentBalance - Amount;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            CardUserSession.LoggedCardUserViewModel.BalanceOnCard = data.CurrentBalance;

        }

        public static void IncreaseCreditOnMFTCCard(int ReceivingMFTCCardId, decimal Amount)
        {

            dbContext = new FAXEREntities();
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == ReceivingMFTCCardId).FirstOrDefault();
            data.CurrentBalance = data.CurrentBalance + Amount;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

        }

        public static void IncreaseBalanceOnMFBCCard(int MFBCCardId, decimal Amount)
        {

            dbContext = new FAXEREntities();
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == MFBCCardId).FirstOrDefault();
            data.CurrentBalance = data.CurrentBalance + Amount;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

        }

        public static string CalulateTimeAgo(this DateTime TransactionDate)
        {

            var timeDiff = (DateTime.Now - TransactionDate).Ticks;
            var timespan = new TimeSpan(timeDiff);
            var msg = "";
            if (timespan.Days > 0)
            {

                msg = timespan.Days + " days ";
            }
            if (timespan.Hours > 0)
            {

                msg = msg + timespan.Hours + " hours ";
            }
            if (timespan.Minutes > 0)
            {
                msg = msg + timespan.Minutes + " mins ";
            }


            msg = msg + timespan.Seconds + " secs ";

            return msg + "ago.";

        }

        public static string FormatSavedBankAccountNumber(string bankAccountNumber)
        {
            if (!string.IsNullOrEmpty(bankAccountNumber))
            {
                return "XXX-" + bankAccountNumber.Right(5);
            }
            return "";
        }

       

        public static Apiservice? GetApiservice(string SendingCountry, string ReceivingCountry, decimal SendingAmount
            , TransactionTransferMethod transactionTransferMethod = TransactionTransferMethod.All,
            TransactionTransferType transferType = TransactionTransferType.Online, int agentId = 0)
        {


            dbContext = new FAXEREntities();

            //SApiServiceConfig apiServiceConfig = new SApiServiceConfig(dbContext);
            //var apiService =  apiServiceConfig.GetApiServiceType(new ApiServiceRequestParam()
            //{
            //    SendingCountry = SendingCountry,
            //    ReceivingCountry = ReceivingCountry,
            //    SendingAmount = SendingAmount,
            //    TransactionTransferMethod = transactionTransferMethod,
            //    TransferType = transferType,
            //    AgentId = agentId
            //});
            //return apiService;

            var allApiProviderSelections = dbContext.APIProviderSelection.Where(x => x.SendingCountry == SendingCountry &&
                                                                                  x.ReceivingCountry == ReceivingCountry);

            var apiProviderSelections = allApiProviderSelections.Where(x => x.TransferType == transferType &&
                                                                           (x.FromRange <= SendingAmount &&
                                                                            x.ToRange >= SendingAmount));

            if (apiProviderSelections.Count() == 0)
            {
                apiProviderSelections = allApiProviderSelections.Where(x => x.TransferType == transferType &&
                                                                            (x.FromRange == 0 &&
                                                                             x.ToRange == 0));
                Log.Write("apiProviderSelections " + apiProviderSelections.Count());
                if (apiProviderSelections.Count() == 0) {

                    apiProviderSelections = allApiProviderSelections.Where(x => x.TransferType == transferType);
                }
                if (apiProviderSelections.Count() == 0)
                {
                    apiProviderSelections = allApiProviderSelections.Where(x => x.TransferType == TransactionTransferType.All &&
                                                                          (x.FromRange <= SendingAmount &&
                                                                            x.ToRange >= SendingAmount));
                    if (apiProviderSelections.Count() == 0) { 
                        
                    apiProviderSelections = allApiProviderSelections.Where(x => x.TransferType == TransactionTransferType.All &&
                                                                             (x.FromRange == 0 &&
                                                                              x.ToRange == 0));
                        }

                    SApiServiceConfig apiServiceConfig = new SApiServiceConfig();
                    apiProviderSelections = apiServiceConfig.filterApiProviderByRange(apiProviderSelections , SendingAmount);
                }
            }
            var data = apiProviderSelections.Where(x => x.TransferMethod == transactionTransferMethod &&
                                                        x.AgentId == agentId).FirstOrDefault();
            if (data == null)
            {
                data = apiProviderSelections.Where(x => x.TransferMethod == TransactionTransferMethod.All &&
                                                       x.AgentId == agentId).FirstOrDefault();
                if (data == null)
                {
                    data = apiProviderSelections.Where(x => x.TransferMethod == transactionTransferMethod).FirstOrDefault();

                    if (data == null)
                    {
                        data = apiProviderSelections.Where(x => x.TransferMethod == TransactionTransferMethod.All).FirstOrDefault();
                    }
                }
            }
            if (data != null)
            {
                var apiservice = data.Apiservice;
                return apiservice;
            }
            return null;
        }


        private static IQueryable<APIProviderSelection> GetRangeQueryResult(IQueryable<APIProviderSelection> query,decimal SendingAmount)
        {
            var rangequery = query;
            var queryWithRange = rangequery.Where(x => x.FromRange <= SendingAmount &&
                                                                             x.ToRange >= SendingAmount);
            if (queryWithRange.Count() > 0)
            {
                rangequery = queryWithRange;
            }
            else
            {
                queryWithRange = rangequery.Where(x => x.FromRange == 0 &&
                                                                             x.ToRange == 0);
                if (queryWithRange.Count() > 0)
                {

                    rangequery = queryWithRange;
                }
                else {

                    rangequery = query;
                }

            }
            return rangequery;

        }
        public static ServiceResult<bool> IsValidTransactionLimit(PaymentSummaryRequestParamVm request, decimal ReceivingAmount)
        {
            var exchangeRateReceivingCountry = Common.GetCountryCodeByCurrency(request.ReceivingCurrency);
            decimal rate = SExchangeRate.GetExchangeRateValue(
                 request.SendingCountry, exchangeRateReceivingCountry,
                (TransactionTransferMethod)request.TransferMethod, request.AgentId, (TransactionTransferType)request.TransferType, request.IsAuxAgnet);

                  var result = new ServiceResult<bool>();
            result.Data = true;
            string CurrencySymbol = GetCurrencySymbol(request.SendingCountry);
            decimal Amount = request.ReceivingAmount;
            string ReceivingCountry = request.ReceivingCountry;
            TransactionTransferMethod transactionTransferMethod = (TransactionTransferMethod)request.TransferMethod;
            if (rate <= 0)
            {
                result.Data = false;
                result.Message = "No Service available";
                return result;
            }

            if (request.ReceivingCountry == "NG")
            {

                if (Amount > 2500000)
                {
                    decimal amount = Math.Round((2500000 / rate), 3);
                    // result.Data = false;
                    // result.Message = "Sending amount should be less than " + CurrencySymbol + amount;
                }
                if (Amount < 1000)
                {
                    decimal amount = Math.Round((1000 / rate), 3);
                    // result.Data = false;
                    // result.Message = "Sending amount should be greater than " + CurrencySymbol + amount;
                }

                //result.Data = false;
                //result.Message = "Payments into Naira accounts have been temporally suspended";
            }
            else if (ReceivingCountry == "GH")
            {
                if (Amount > 5000)
                {

                    decimal amount = Math.Round((5000 / rate), 3);
                    result.Data = false;
                    result.Message = "Sending amount should be less than " + CurrencySymbol + amount;
                }
                if (Amount < 20)
                {
                    decimal amount = Math.Round((20 / rate), 3);
                    result.Data = false;
                    result.Message = "Sending amount should be greater than " + CurrencySymbol + amount;
                }
            }
            else if (ReceivingCountry == "CM")
            {
                #region old limit
                //if (Amount > 2000000)
                //{
                //    decimal amount = Math.Round((2000000 / rate), 3);
                //    result.Data = false;
                //    result.Message = "Sending amount should be less than " + CurrencySymbol + amount;
                //}
                //if (transactionTransferMethod == TransactionTransferMethod.OtherWallet)
                //{
                //    if (Amount < 100)
                //    {
                //        decimal amount = Math.Round((100 / rate), 3);
                //        result.Data = false;
                //        result.Message = "Sending amount should be greater than " + CurrencySymbol + amount;
                //    }
                //}
                #endregion
                if (transactionTransferMethod == TransactionTransferMethod.OtherWallet)
                {
                    if (Amount > 1000000)
                    {
                        decimal amount = Math.Round((1000000 / rate), 3);
                        result.Data = false;
                        result.Message = "Sending amount should be less than " + CurrencySymbol + amount;
                    }
                }

            }
            else if (ReceivingCountry == "UG")
            {
                if (Amount > 4000000)
                {
                    decimal amount = Math.Round((4000000 / rate), 3);
                    result.Data = false;
                    result.Message = "Sending amount should be less than " + CurrencySymbol + amount;
                }
                if (Amount < 1000)
                {
                    decimal amount = Math.Round((1000 / rate), 3);
                    result.Data = false;
                    result.Message = "Sending amount should be greater than " + CurrencySymbol + amount;
                }
            }

            return result;
        }

            
        public static ServiceResult<bool> IsValidTransactionLimit(string SendingCountry, string ReceivingCountry, decimal Amount,
            TransactionTransferMethod transactionTransferMethod)
        {

            var result = new ServiceResult<bool>();
            result.Data = true;
            return result;
            var rate = SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry, 
                (TransactionTransferMethod)transactionTransferMethod);
            string CurrencySymbol = GetCurrencySymbol(SendingCountry);
            if (rate <= 0)
            {
                result.Data = false;
                result.Message = "No Service available";
                return result;
            }

            if (ReceivingCountry == "NG")
            {

                if (Amount > 2500000)
                {
                    decimal amount = Math.Round((2500000 / rate), 3);
                    // result.Data = false;
                    // result.Message = "Sending amount should be less than " + CurrencySymbol + amount;
                }
                if (Amount < 1000)
                {
                    decimal amount = Math.Round((1000 / rate), 3);
                    // result.Data = false;
                    // result.Message = "Sending amount should be greater than " + CurrencySymbol + amount;
                }

                //result.Data = false;
                //result.Message = "Payments into Naira accounts have been temporally suspended";
            }
            else if (ReceivingCountry == "GH")
            {
                if (Amount > 5000)
                {

                    decimal amount = Math.Round((5000 / rate), 3);
                    result.Data = false;
                    result.Message = "Sending amount should be less than " + CurrencySymbol + amount;
                }
                if (Amount < 20)
                {
                    decimal amount = Math.Round((20 / rate), 3);
                    result.Data = false;
                    result.Message = "Sending amount should be greater than " + CurrencySymbol + amount;
                }
            }
            else if (ReceivingCountry == "CM")
            {
                #region old limit
                //if (Amount > 2000000)
                //{
                //    decimal amount = Math.Round((2000000 / rate), 3);
                //    result.Data = false;
                //    result.Message = "Sending amount should be less than " + CurrencySymbol + amount;
                //}
                //if (transactionTransferMethod == TransactionTransferMethod.OtherWallet)
                //{
                //    if (Amount < 100)
                //    {
                //        decimal amount = Math.Round((100 / rate), 3);
                //        result.Data = false;
                //        result.Message = "Sending amount should be greater than " + CurrencySymbol + amount;
                //    }
                //}
                #endregion
                if (transactionTransferMethod == TransactionTransferMethod.OtherWallet)
                {
                    if (Amount > 1000000)
                    {
                        decimal amount = Math.Round((1000000 / rate), 3);
                        result.Data = false;
                        result.Message = "Sending amount should be less than " + CurrencySymbol + amount;
                    }
                }

            }
            else if (ReceivingCountry == "UG")
            {
                if (Amount > 4000000)
                {
                    decimal amount = Math.Round((4000000 / rate), 3);
                    result.Data = false;
                    result.Message = "Sending amount should be less than " + CurrencySymbol + amount;
                }
                if (Amount < 1000)
                {
                    decimal amount = Math.Round((1000 / rate), 3);
                    result.Data = false;
                    result.Message = "Sending amount should be greater than " + CurrencySymbol + amount;
                }
            }

            return result;
        }

        public static BankDepositStatus GetTransferZeroTransactionStatus(Transaction transactionState)
        {
            BankDepositStatus status;
            var transcationStatus = transactionState.State;

            if (transcationStatus == TransactionState.Initial || transcationStatus == TransactionState.Approved
                || transcationStatus == TransactionState.Pending || transcationStatus == TransactionState.Received
                || transcationStatus == TransactionState.Processing)
            {
                return BankDepositStatus.Incomplete;
            }
            if (transcationStatus == TransactionState.Paid)
            {
                return BankDepositStatus.Confirm;
            }
            if (transcationStatus == TransactionState.Canceled)
            {
                return BankDepositStatus.Cancel;
            }
            else
            {

                return BankDepositStatus.Incomplete;
            }

            return status;

        }

        public static CommonEnterAmountViewModel GetAmountSummarySession()
        {

            CommonEnterAmountViewModel vm = new CommonEnterAmountViewModel();
            if (FaxerSession.CommonEnterAmountViewModel != null)
            {

                vm = FaxerSession.CommonEnterAmountViewModel;
            }

            return vm;
        }

        internal static FaxerInformation GetSenderInfo(int senderId)
        {

            dbContext = new FAXEREntities();
            var result = dbContext.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();
            return result;
        }

        public static string GetDefaultReceivingCountryCode()
        {


            string code = GetAppSettingValue("DefaultReceivingCountryCode").ToString();
            return code;
        }

        public static string GetDefaultReceivingCurrency()
        {


            string code = GetAppSettingValue("DefaultReceivingCurrency").ToString();
            return code;
        }
        internal static DocumentApprovalStatus GetSenderDocumentStatus(int senderId)
        {


            dbContext = new FAXEREntities();
            var result = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == senderId).FirstOrDefault();
            if (result != null)
            {
                return result.Status;
            }
            return DocumentApprovalStatus.InProgress;
        }

        public static MobileMoneyTransferStatus GetTransferZeroTransactionStatusForMobileWallet(Transaction transactionState)
        {

            try
            {
                var transcationStatus = transactionState.State;
                if (transcationStatus == TransactionState.Initial || transcationStatus == TransactionState.Approved
                    || transcationStatus == TransactionState.Pending || transcationStatus == TransactionState.Received
                    || transcationStatus == TransactionState.Processing)
                {
                    return MobileMoneyTransferStatus.InProgress;
                }
                else if (transcationStatus == TransactionState.Paid)
                {
                    return MobileMoneyTransferStatus.Paid;
                }
                else if (transcationStatus == TransactionState.Canceled)
                {
                    return MobileMoneyTransferStatus.Cancel;
                }
                else
                {
                    return MobileMoneyTransferStatus.Failed;
                }
            }
            catch (Exception)
            {

                return MobileMoneyTransferStatus.Failed;
            }


        }
        public static MobileMoneyTransferStatus GetTransferZeroTransactionStatusForMobileWalletMTN(string status)
        {

            return MobileMoneyTransferStatus.Paid;

            if (status == null)
            {
                return MobileMoneyTransferStatus.InProgress;
            }
            status = status.ToLower();
            if (status == "failed")
            {
                return MobileMoneyTransferStatus.InProgress;

            }
            else if (status == "sucesss")
            {
                return MobileMoneyTransferStatus.Paid;
            }
            else if (status == "processing")
            {
                return MobileMoneyTransferStatus.InProgress;
            }
            else
            {
                return MobileMoneyTransferStatus.InProgress;
            }

        }
        public static int GetNumberOfPage(int totalCount, int pageSize)
        {
            decimal NumberOfPage = totalCount.ToDecimal() / pageSize.ToDecimal();
            int numberOfPage = Math.Ceiling(NumberOfPage).ToInt();
            return numberOfPage;
        }

    }


    public static class FaxerStripe
    {
        public static StripeCharge CreateTransaction(decimal faxingAmount, string faxingCurrency, string nameOnCard, string SourceToken)
        {
            StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");

            var chargeOptions = new StripeChargeCreateOptions()
            {
                Amount = (Int32)faxingAmount * 100,
                //todo : change currency later
                Currency = "USD",//faxingCurrency
                Description = "Charge for " + nameOnCard,
                SourceTokenOrExistingSourceId = SourceToken // obtained with Stripe.js
            };
            var chargeService = new StripeChargeService();
            StripeCharge charge = chargeService.Create(chargeOptions);
            return charge;
        }
        public static StripeRefund RefundTransaction(string ChargeId)
        {
            // Set your secret key: remember to change this to your live secret key in production
            // See your keys here: https://dashboard.stripe.com/account/apikeys
            StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");
            var options = new StripeRefundCreateOptions
            {
                //Charge = "ch_GE4Tp5SpgG2nKcKdAMgH",
                //Amount = 1000,
            };

            var service = new StripeRefundService();
            StripeRefund refund = service.Create(ChargeId);
            return refund;
        }

        public static StripeToken GetStripeToken(StripeTokenCreateOptions stripeCreditCard)
        {


            string Sourcetoken = "";
            var tokenService = new StripeTokenService();

            StripeResponse stripeResponse = new StripeResponse();
            var stripeToken = new StripeToken();
            try
            {
                stripeToken = tokenService.Create(stripeCreditCard);

            }
            catch (Exception ex)
            {

                throw ex;

                //Log.Write("Merchant Auto Payment Exception : " + ex.Message);
            }
            return stripeToken;


        }

    }
    public static class ConversionExtension
    {
        public static string ToString(this TimeSpan? obj, string format)
        {
            try
            {

                return TimeSpan.Parse(obj.ToString()).ToString(format);
            }
            catch
            {

                return new TimeSpan().ToString(format);
            }
        }
        public static TimeSpan? ToNullableTimeSpan(this string obj)
        {
            try
            {
                return TimeSpan.Parse(obj);
            }
            catch
            {

                return null;
            }
        }
        public static TimeSpan ToTimeSpan(this string obj)
        {
            try
            {
                return TimeSpan.Parse(obj);
            }
            catch
            {

                return new TimeSpan(0);
            }
        }
        public static int? ToNullableInt(this object obj)
        {
            try
            {
                return int.Parse(obj.ToString());
            }
            catch
            {

                return null;
            }
        }
        public static int ToInt(this object obj)
        {
            try
            {
                return int.Parse(obj.ToString());
            }
            catch
            {

                return 0;
            }
        }
        public static decimal ToDecimal(this object obj)
        {
            try
            {
                return decimal.Parse(obj.ToString());
            }
            catch
            {

                return 0;
            }
        }

        public static DateTime? ToNullableDatetime(this string value)
        {
            try
            {
                return DateTime.Parse(value);
            }
            catch (Exception)
            {

                return null;
            }
        }
        public static DateTime ToDateTime(this string value)
        {
            try
            {

                return DateTime.Parse(value);
            }
            catch (Exception)
            {

                return new DateTime();
            }
        }
        public static string ToFormatedString(this DateTime value)
        {
            try
            {

                return value.ToString("dd/MM/yyyy");
            }
            catch (Exception)
            {

                return "";
            }
        }

        public static string ToFormatedStringTime(this TimeSpan value)
        {
            try
            {
                return value.ToString("hh:mm");
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string ToFormatedString(this DateTime? value, string errorText = "")
        {
            try
            {
                var date = DateTime.Parse(value.ToString());
                return date.ToString("dd/MM/yyyy");
            }
            catch (Exception)
            {

                return errorText;
            }
        }


        public static string FormatPhoneNo(this string PhoneNo)
        {

            try
            {


                long phoneno = long.Parse(PhoneNo);

                return phoneno.ToString();

            }
            catch (Exception ex)
            {

                Log.Write("Phone no Format exception " + ex.Message);
            }

            return "";
        }




    }

    public class Log
    {
        public static void Write(string text, ErrorType errorType = ErrorType.UnSpecified, string source = "")
        {
            Logger model = new Logger()
            {
                ErrorType = errorType,
                ErrorMessage = text,
                DateTime = DateTime.Now,
                Source = source ?? ""
            };
            var dbContext = new DB.FAXEREntities();
            dbContext.Logger.Add(model);
            dbContext.SaveChanges();
        }

    }




    //public static CheckBalanceForMessage()
    //{
    //    Month
    //}


    public class DropDownViewModel
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CountryCode { get; set; }

    }
    public enum ResultStatus
    {
        Default,
        OK,
        Warning,
        Error,
        Info
    }
 

}