using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Transfer
{
    public class SenderTransferMoneyNowController : Controller
    {
        // GET: SenderTransferMoneyNow

        public ActionResult GoToDashboard()
        {


            SenderCommonFunc funcServices = new SenderCommonFunc();

            Session.Remove("FaxingAmountSummary");
            Session.Remove("TransactionSummaryUrl");
            Session.Remove("CreditDebitDetails");
            Session.Remove("SavedCreditDebitCardDetails");
            Session.Remove("ReceivingCountry");
            Session.Remove("NonCardReceiverId");
            Session.Remove("TransactionId");
            Session.Remove("RecipientId");
            Session.Remove("FromUrl");
            Session.Remove("CommonEnterAmountViewModel");

            funcServices.ClearCashPickUpSession();
            funcServices.ClearFamilyAndFriendSession();
            funcServices.ClearKiiPayTransferSession();
            funcServices.ClearPayBillsSession();
            funcServices.ClearTransferBankDepositSession();
            funcServices.ClearPayForServiceSession();
            funcServices.ClearMobileTransferSession();

            return RedirectToAction("Index");
        }
        public ActionResult Index(bool IsFormHomePage = false)
        {

            //Common.FaxerSession.TransactionId = 0;
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            SenderCommonFunc funcServices = new SenderCommonFunc();
            int senderId = Common.FaxerSession.LoggedUser.Id;
            string sendingCountry = Common.FaxerSession.LoggedUser.CountryCode;

            if (Common.FaxerSession.IsRedirectedFromEmail == true)
            {
                SSenderCashPickUp _cashPickUpServices = new SSenderCashPickUp();
                var TransactionPending = _cashPickUpServices.GetTransactionPendingViewModel();
                return RedirectToAction("RecentTransfersSendAgain", "SenderTransferMoneyNow",
                    new { id = TransactionPending.TransactionId, transactionServiceType = TransactionPending.TransferMethod });
            }
            string receivingCountry = Common.Common.GetDefaultReceivingCountryCode(); // Default receiving Country is Nigeria 
            string receivingCurrency = Common.Common.GetDefaultReceivingCurrency(); // Default receiving Cuurency is Nigeria 
            Common.FaxerSession.IsCommonEstimationPage = true; // IsCommonEstimationPage is Used  because every transfer used same estimation Page
            var amountSummary = _kiiPaytrasferServices.GetCommonEnterAmount(); // GetSummary set in Session 

            if (amountSummary.ReceivingCountryCode != null)
            {
                receivingCountry = amountSummary.ReceivingCountryCode;
            }

            RecentTranferAndRecipientViewModel vm = new RecentTranferAndRecipientViewModel();
            vm.RecentTransfer = GetRecentTransfers(senderId);
            vm.Recipients = _kiiPaytrasferServices.GetReceiverInformation(senderId).Take(10).ToList();
            var balance = funcServices.GetMonthlyTransactionMeter(senderId);
            vm.SenderMonthlyTransaction = new SenderMonthlyTransactionMeterViewModel();
            vm.SenderMonthlyTransaction.SenderMonthyTransactionMeterBalance = balance;
            vm.SenderMonthlyTransaction.SenderCurrencySymbol = Common.Common.GetCurrencySymbol(Common.FaxerSession.LoggedUser.CountryCode);


            // Case For Back on This page
            var transferMethod = amountSummary.TransactionTransferMethod;
            switch (transferMethod)
            {
                case TransactionTransferMethod.Select:
                    break;
                case TransactionTransferMethod.All:
                    break;
                case TransactionTransferMethod.CashPickUp:
                    ViewBag.TransferMethod = "CashPickup";
                    break;
                case TransactionTransferMethod.KiiPayWallet:
                    break;
                case TransactionTransferMethod.OtherWallet:
                    ViewBag.TransferMethod = "Mobile";
                    break;
                case TransactionTransferMethod.BankDeposit:
                    ViewBag.TransferMethod = "BankDeposit";
                    break;
                case TransactionTransferMethod.BillPayment:
                    break;
                case TransactionTransferMethod.ServicePayment:
                    break;
                default:
                    ViewBag.TransferMethod = "";
                    break;
            }
            // End Case For Back on This page


            var paymentSummary = _kiiPaytrasferServices.GetCommonEnterAmount();
            if (paymentSummary.SendingAmount > 0) {

                receivingCurrency = paymentSummary.ReceivingCurrency;
            }

            ViewBag.SendingCountry = sendingCountry;
            ViewBag.ReceivingCountry = receivingCountry;
            ViewBag.SendingAmount = _kiiPaytrasferServices.GetCommonEnterAmount().SendingAmount == 0 ? 3
                : _kiiPaytrasferServices.GetCommonEnterAmount().SendingAmount;

            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Transfer Money");
            ViewBag.SendingCurrency = Common.Common.GetCountryCurrency(sendingCountry).ToUpper();
            ViewBag.ReceivingCurrency = receivingCurrency;

            ViewBag.DefaultReceivingCurrency = receivingCurrency;

            
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();

            return View(vm);
        }

        
        public List<SenderTransactionHistoryList> GetRecentTransfers(int senderId)
        {

            SSenderTransactionHistory _transactionHistoryServices = new SSenderTransactionHistory();

            var bankDeposit = _transactionHistoryServices.GetBankDepositDetails(senderId).Where(x => x.StatusName != "Cancelled").OrderByDescending(x => x.TransactionDate).Take(5).ToList();
            var mobileDeposit = _transactionHistoryServices.GetMobileTransferDetails(senderId).Where(x => x.StatusName != "Cancelled").OrderByDescending(x => x.TransactionDate).Take(5).ToList();
            var CashPickUp = _transactionHistoryServices.GetCashPickUpDetails(senderId).Where(x => x.StatusName != "Cancelled").OrderByDescending(x => x.TransactionDate).Take(5).ToList();

            List<SenderTransactionHistoryList> result = new List<SenderTransactionHistoryList>();
            result.AddRange(bankDeposit);
            result.AddRange(mobileDeposit);
            result.AddRange(CashPickUp);

            return result.OrderByDescending(x => x.TransactionDate).ToList();


        }

        public ActionResult SendToDashBoard()
        {

            SenderCommonFunc funcServices = new SenderCommonFunc();


            if (Common.FaxerSession.IsTransactionOnpending == true)
            {
                //send email to sender and clear session
                SSenderForAllTransfer _sSenderForAllTransfer = new SSenderForAllTransfer();
                _sSenderForAllTransfer.SendTransactionPendingEmail();
                Session.Remove("IsTransactionOnpending");
            }

            Session.Remove("FaxingAmountSummary");
            Session.Remove("TransactionSummaryUrl");
            Session.Remove("CreditDebitDetails");
            Session.Remove("SavedCreditDebitCardDetails");
            Session.Remove("ReceivingCountry");
            Session.Remove("NonCardReceiverId");
            Session.Remove("TransactionId");
            Session.Remove("RecipientId");
            Session.Remove("FromUrl");
            Session.Remove("CommonEnterAmountViewModel");
            Session.Remove("TransactionPendingViewModel");
            Session.Remove("IsRedirectedFromEmail");

            funcServices.ClearCashPickUpSession();
            funcServices.ClearFamilyAndFriendSession();
            funcServices.ClearKiiPayTransferSession();
            funcServices.ClearPayBillsSession();
            funcServices.ClearTransferBankDepositSession();
            funcServices.ClearPayForServiceSession();
            funcServices.ClearMobileTransferSession();


            return RedirectToAction("Index", "SenderTransferMoneyNow");
        }

        [HttpPost]
        public ActionResult UploadDocumentation(string File = "")
        {
            SenderDocumentationServices _senderDocumentationServices = new SenderDocumentationServices();

            if (Request.Files.Count < 1)
            {
                var identificationdoc = Request.Files["FileName"];
            }
            string identificationDocPath = "";
            string DocumentPhotoUrl = "";
            var IdentificationDoc = Request.Files["FileName"];
            if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
            {
                var allowedExtensions = new string[] { "gif", "jpg", "png", "jpeg", "pdf" };
                var extension = IdentificationDoc.FileName.Split('.')[1];
                extension = extension.ToLower();
                identificationDocPath = Guid.NewGuid() + "." + IdentificationDoc.FileName.Split('.')[1];

                if (allowedExtensions.Contains(extension))
                {
                    try
                    {
                        IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + identificationDocPath);
                    }
                    catch (Exception ex)
                    {

                    }
                    DocumentPhotoUrl = "/Documents/" + identificationDocPath;
                }
                else
                {
                    ModelState.AddModelError("ChooseAfile", "File type not allowed to upload. ");
                }

            }
            int senderId = Common.FaxerSession.LoggedUser.Id;
            string DocumentName = IdentificationDoc.FileName.Split('.')[0];
            CommonServices _CommonServices = new CommonServices();
            var senderInfo = _CommonServices.GetSenderInfo(senderId);
            var senderDocumentation = _CommonServices.GetSenderDocumentation(senderId);
            if (senderDocumentation.Count > 0)
            {
                SenderDocumentationViewModel vm = (from c in senderDocumentation
                                                   select new SenderDocumentationViewModel()
                                                   {
                                                       AccountNo = c.AccountNo,
                                                       DocumentPhotoUrl = DocumentPhotoUrl,
                                                       City = c.City,
                                                       Country = c.Country,
                                                       CreatedBy = c.CreatedBy,
                                                       CreatedDate = c.CreatedDate,
                                                       DocumentName = c.DocumentName,
                                                       DocumentExpires = c.DocumentExpires,
                                                       DocumentType = c.DocumentType,
                                                       IssuingCountry = c.IssuingCountry,
                                                       SenderId = c.SenderId,
                                                       Status = DocumentApprovalStatus.InProgress,
                                                       Id = c.Id,
                                                       ExpiryDate = c.ExpiryDate,
                                                   }).FirstOrDefault();
                _senderDocumentationServices.UpdateDocument(vm);
            }
            else
            {
                SenderDocumentationViewModel vm = new SenderDocumentationViewModel()
                {
                    SenderId = senderId,
                    AccountNo = senderInfo.AccountNo,
                    City = senderInfo.City,
                    Country = senderInfo.Country,
                    CreatedDate = DateTime.Now,
                    DocumentName = DocumentName,
                    DocumentPhotoUrl = DocumentPhotoUrl,
                    SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName,
                    Status = DocumentApprovalStatus.InProgress,
                    IsUploadedFromSenderPortal = true,
                    IssuingCountry = senderInfo.IssuingCountry

                };
                _senderDocumentationServices.UploadDocument(vm);
            }

            _senderDocumentationServices.SendIdentiVerificationInProgressEmail(senderId);
            return RedirectToAction("Index", "SenderTransferMoneyNow");
        }

        public ActionResult RecipientsList(string ReceiverName = "")
        {
            int faxerId = Common.FaxerSession.LoggedUser.Id;
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            List<RecipientsViewModel> vm = _kiiPaytrasferServices.GetReceiverInformation(faxerId, ReceiverName).ToList();
            ViewBag.ReceiverName = ReceiverName;
            return View(vm);
        }

        public ActionResult RecentTransfersSendAgain(int id = 0, TransactionServiceType transactionServiceType = TransactionServiceType.All, int recipientId = 0)
        {
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            //SetPaymentSummarySession();
            if (Common.FaxerSession.LoggedUser == null)
            {
                Common.FaxerSession.IsRedirectedFromEmail = true;
                TransactionPendingViewModel transactionPending = new TransactionPendingViewModel
                {
                    TransactionId = id,
                    TransferMethod = transactionServiceType
                };
                SSenderCashPickUp _cashPickUpServices = new SSenderCashPickUp();
                _cashPickUpServices.SetTransactionPendingViewModel(transactionPending);

                return RedirectToAction("Login", "FaxerAccount");
            }
            int senderId = Common.FaxerSession.LoggedUser.Id;

            string sendingCountry = Common.FaxerSession.LoggedUser.CountryCode;

            if (_kiiPaytrasferServices.GetCommonEnterAmount().SendingCountryCode != null
                && _kiiPaytrasferServices.GetCommonEnterAmount().SendingCountryCode.ToLower() == sendingCountry)
            {

                sendingCountry = _kiiPaytrasferServices.GetCommonEnterAmount().SendingCountryCode;
            }


            ViewBag.SendingCountry = sendingCountry;
            ViewBag.SenderName = Common.FaxerSession.LoggedUser.FullName;
            ViewBag.sendingAmount = 1;
            string receivingCountry = "NG";

            SSenderTransactionHistory _transactionHistoryServices = new SSenderTransactionHistory();

            if (id != 0)
            {

                string ReceiverName = "";
                CommonEnterAmountViewModel enterAmountViewModel = new CommonEnterAmountViewModel();

                // SenderTransactionHistoryViewModel data = _transactionHistoryServices.GetTransactionHistories(transactionServiceType, senderId, 0, 0);

                switch (transactionServiceType)
                {
                    case TransactionServiceType.BankDeposit:
                        SSenderBankAccountDeposit _senderBankAccountDeposit = new SSenderBankAccountDeposit();
                        var bankDeposit = _senderBankAccountDeposit.List().Data.Where(x => x.Id == id).FirstOrDefault();
                        enterAmountViewModel = new CommonEnterAmountViewModel()
                        {
                            SendingAmount = bankDeposit.SendingAmount,
                            ReceivingCountryCode = bankDeposit.ReceivingCountry,
                            SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(bankDeposit.SendingCurrency, bankDeposit.SendingCountry),
                            ReceivingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(bankDeposit.ReceivingCurrency, bankDeposit.ReceivingCountry),
                        };
                        ReceiverName = bankDeposit.ReceiverName;
                        ViewBag.TransferMethod = "BankDeposit";
                        break;
                    case TransactionServiceType.MobileWallet:
                        SSenderMobileMoneyTransfer _senderMobileMoneyTransfer = new SSenderMobileMoneyTransfer();
                        var mobileDeposit = _senderMobileMoneyTransfer.list().Data.Where(x => x.Id == id).FirstOrDefault();
                        enterAmountViewModel = new CommonEnterAmountViewModel()
                        {
                            SendingAmount = mobileDeposit.SendingAmount,
                            ReceivingCountryCode = mobileDeposit.ReceivingCountry,
                            SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(mobileDeposit.SendingCurrency, mobileDeposit.SendingCountry),
                            ReceivingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(mobileDeposit.ReceivingCurrency, mobileDeposit.ReceivingCountry),

                        };
                        ReceiverName = mobileDeposit.ReceiverName;
                        ViewBag.TransferMethod = "Mobile";
                        break;
                    case TransactionServiceType.CashPickUp:
                        SSenderCashPickUp _senderCashPickUp = new SSenderCashPickUp();
                        var cashpickup = _senderCashPickUp.List().Data.Where(x => x.Id == id).FirstOrDefault();
                        enterAmountViewModel = new CommonEnterAmountViewModel()
                        {
                            SendingAmount = cashpickup.FaxingAmount,
                            ReceivingCountryCode = cashpickup.ReceivingCountry,
                            SendingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashpickup.SendingCurrency, cashpickup.SendingCountry),
                            ReceivingCurrency = Common.Common.GetCurrencyByCurrencyOrCountry(cashpickup.ReceivingCurrency, cashpickup.ReceivingCountry),

                        };
                        ReceiverName = cashpickup.NonCardReciever.FirstName + " " + cashpickup.NonCardReciever.MiddleName + " " + cashpickup.NonCardReciever.LastName;
                        ViewBag.TransferMethod = "CashPickup";
                        break;
                    default:
                        break;
                }

                _kiiPaytrasferServices.SetCommonEnterAmount(enterAmountViewModel);

                receivingCountry = enterAmountViewModel.ReceivingCountryCode;
                ViewBag.ReceiverName = ReceiverName;
                ViewBag.sendingAmount = enterAmountViewModel.SendingAmount;
                ViewBag.ReceiverCountry = receivingCountry.ToLower();
                Common.FaxerSession.TransactionId = id;

            }

            var paymentInfo = _kiiPaytrasferServices.GetCommonEnterAmount();
            if (recipientId != 0)
            {
                var recipientInfo = _kiiPaytrasferServices.GetRecipientsInfo(recipientId);
                receivingCountry = recipientInfo.Country;
                ViewBag.ReceiverName = recipientInfo.ReceiverName;
                ViewBag.ReceiverCountry = receivingCountry.ToLower();

                decimal sendingAmount = paymentInfo.SendingAmount == 0 ? 1 : paymentInfo.SendingAmount;
                ViewBag.SendingAmount = sendingAmount * 1.00M;
                var senderInfo = FaxerSession.LoggedUser;
                paymentInfo.SendingCountryCode = senderInfo.CountryCode;
                paymentInfo.SendingCurrency = Common.Common.GetCurrencyCode(senderInfo.CountryCode);
                paymentInfo.ReceivingCurrency = Common.Common.GetCurrencyCode(receivingCountry);
                paymentInfo.ReceivingCountryCode = receivingCountry;
                Common.FaxerSession.RecipientId = recipientId;
            }


            ViewBag.SendingCurrencyCode = paymentInfo.SendingCurrency;
            ViewBag.ReceivingCurrencyCode = paymentInfo.ReceivingCurrency;
            ViewBag.ReceivingCountry = receivingCountry;

            //ViewBag.SendingAmount = sendingAmount;
            Common.FaxerSession.IsCommonEstimationPage = true;
            ViewBag.Method = Common.Common.GetEnumDescription(transactionServiceType);

            ViewBag.TrackingNo = Common.Common.GetTrackingNo("Transfer Money");

            //switch (transactionServiceType)
            //{
            //    case TransactionServiceType.MobileWallet:
            //        ViewBag.TransferMethod = "Mobile";
            //        break;
            //    case TransactionServiceType.BankDeposit:
            //        ViewBag.TransferMethod = "BankDeposit";
            //        break;
            //    case TransactionServiceType.CashPickUp:
            //        ViewBag.TransferMethod = "CashPickUp";
            //        break;
            //    default:
            //        break;
            //}

            ViewBag.SendingCurrency = paymentInfo.SendingCurrency.ToUpper();
            ViewBag.ReceivingCurrency = paymentInfo.ReceivingCurrency.ToUpper();

            //ViewBag.ReceivingCountries = Common.Common.GetRecentTransferCountries();
            ViewBag.ReceivingCountries = Common.Common.GetReceivingCountries();

            return View();
        }



        [HttpGet]
        public JsonResult GetPaymentSummary(decimal SendingAmount = 0, decimal ReceivingAmount = 0,
           string SendingCountry = "", string ReceivingCountry = "", bool IsReceivingAmount = false, int transferMethod = 0)
        {
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }

            var feeInfo = SEstimateFee.GetTransferFee(SendingCountry, ReceivingCountry, (TransactionTransferMethod)transferMethod, SendingAmount);
            if (feeInfo == null)
            {

                return Json(new
                {
                    Fee = "",
                    TotalAmount = "",
                    ReceivingAmount = "",
                    SendingAmount = "",
                    ExchangeRate = "",
                    SendingCurrencySymbol = "",
                    ReceivingCurrencySymbol = "",
                    SendingCurrency = "",
                    ReceivingCurrency = "",

                }, JsonRequestBehavior.AllowGet);
            }
            var result = new EstimateFaxingFeeSummary();


            result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry, (TransactionTransferMethod)transferMethod), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(SendingCountry, ReceivingCountry, result.FaxingAmount
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, false, (TransactionTransferMethod)transferMethod);
            if (introductoryRateResult != null)
            {

                result = introductoryRateResult;
            }


            CommonEnterAmountViewModel enterAmount = new CommonEnterAmountViewModel()
            {
                Fee = result.FaxingFee,
                SendingAmount = result.FaxingAmount,
                ReceivingAmount = IsReceivingAmount == true ? SendingAmount : result.ReceivingAmount,
                TotalAmount = result.TotalAmount,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SendingCountry),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry),
                SendingCountryCode = SendingCountry,
                ReceivingCountryCode = ReceivingCountry,
                SendingCurrency = Common.Common.GetCountryCurrency(SendingCountry),
                ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry),
                TransactionTransferMethod = (TransactionTransferMethod)transferMethod

            };
            // Rewrite session with additional value 

            _kiiPaytrasferServices.SetCommonEnterAmount(enterAmount);
            return Json(new
            {
                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = IsReceivingAmount == true ? SendingAmount : result.ReceivingAmount,
                SendingAmount = result.FaxingAmount,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = enterAmount.SendingCurrencySymbol,
                ReceivingCurrencySymbol = enterAmount.ReceivingCurrencySymbol,
                SendingCurrency = enterAmount.SendingCurrency,
                ReceivingCurrency = enterAmount.ReceivingCurrency,
                IsIntroductoryRate = result.IsIntroductoryRate,
                IsIntroductoryFee = result.IsIntroductoryFee,
                ActualFee = result.ActualFee,
                IsValid = Common.Common.IsValidTransactionLimit(SendingCountry, ReceivingCountry, result.ReceivingAmount, (TransactionTransferMethod)transferMethod)
            }, JsonRequestBehavior.AllowGet);
        }




        private TransferSummaryViewModel GetTransactionSummary(decimal SendingAmount = 0, decimal ReceivingAmount = 0,
          string SendingCountry = "", string ReceivingCountry = "", bool IsReceivingAmount = false, int transferMethod = 0)
        {
            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            if (IsReceivingAmount)
            {

                SendingAmount = ReceivingAmount;
            }

            var feeInfo = SEstimateFee.GetTransferFee(SendingCountry, ReceivingCountry, (TransactionTransferMethod)transferMethod, SendingAmount);
            if (feeInfo == null)
            {

                return new TransferSummaryViewModel();
            }
            var result = new EstimateFaxingFeeSummary();


            result = SEstimateFee.CalculateFaxingFee(SendingAmount, false, IsReceivingAmount,
                SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry, (TransactionTransferMethod)transferMethod), feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false);


            var introductoryRateResult = SEstimateFee.GetIntroductoryTransferSummary(SendingCountry, ReceivingCountry, result.FaxingAmount
                , feeInfo.Fee, feeInfo.FeeType == DB.FeeType.FlatFee ? true : false, false, (TransactionTransferMethod)transferMethod);
            if (introductoryRateResult != null)
            {

                result = introductoryRateResult;
            }


            CommonEnterAmountViewModel enterAmount = new CommonEnterAmountViewModel()
            {
                Fee = result.FaxingFee,
                SendingAmount = result.FaxingAmount,
                ReceivingAmount = IsReceivingAmount == true ? SendingAmount : result.ReceivingAmount,
                TotalAmount = result.TotalAmount,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(SendingCountry),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry),
                SendingCountryCode = SendingCountry,
                ReceivingCountryCode = ReceivingCountry,
                SendingCurrency = Common.Common.GetCountryCurrency(SendingCountry),
                ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry),
                TransactionTransferMethod = (TransactionTransferMethod)transferMethod

            };
            // Rewrite session with additional value 

            _kiiPaytrasferServices.SetCommonEnterAmount(enterAmount);

            return new TransferSummaryViewModel()
            {

                Fee = result.FaxingFee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = IsReceivingAmount == true ? SendingAmount : result.ReceivingAmount,
                SendingAmount = result.FaxingAmount,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = enterAmount.SendingCurrencySymbol,
                ReceivingCurrencySymbol = enterAmount.ReceivingCurrencySymbol,
                SendingCurrency = enterAmount.SendingCurrency,
                ReceivingCurrency = enterAmount.ReceivingCurrency,
                IsIntroductoryRate = result.IsIntroductoryRate,
                IsIntroductoryFee = result.IsIntroductoryFee,
                ActualFee = result.ActualFee,
                IsValid = Common.Common.IsValidTransactionLimit(SendingCountry, ReceivingCountry, result.ReceivingAmount, (TransactionTransferMethod)transferMethod)

            };
            //return Json(new
            //{
            //    Fee = result.FaxingFee,
            //    TotalAmount = result.TotalAmount,
            //    ReceivingAmount = IsReceivingAmount == true ? SendingAmount : result.ReceivingAmount,
            //    SendingAmount = result.FaxingAmount,
            //    ExchangeRate = result.ExchangeRate,
            //    SendingCurrencySymbol = enterAmount.SendingCurrencySymbol,
            //    ReceivingCurrencySymbol = enterAmount.ReceivingCurrencySymbol,
            //    SendingCurrency = enterAmount.SendingCurrency,
            //    ReceivingCurrency = enterAmount.ReceivingCurrency,
            //    IsIntroductoryRate = result.IsIntroductoryRate,
            //    IsIntroductoryFee = result.IsIntroductoryFee,
            //    ActualFee = result.ActualFee,
            //    IsValid = Common.Common.IsValidTransactionLimit(SendingCountry, ReceivingCountry, result.ReceivingAmount, (TransactionTransferMethod)transferMethod)
            //}, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetEstimateTransactionSummary(decimal SendingAmount = 0, decimal ReceivingAmount = 0,
          string SendingCurrency = "", string ReceivingCurrency = "", bool IsReceivingAmount = false, int transferMethod = 0)
        {
            var sendingCountry = Common.Common.GetCountryCodeByCurrency(SendingCurrency);
            var receivingCountry = Common.Common.GetCountryCodeByCurrency(ReceivingCurrency);
            var result = GetTransactionSummary(SendingAmount, ReceivingAmount, sendingCountry,
                receivingCountry, IsReceivingAmount, transferMethod);
            return Json(new
            {
                Fee = result.Fee,
                TotalAmount = result.TotalAmount,
                ReceivingAmount = result.ReceivingAmount,
                SendingAmount = result.SendingAmount,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = result.SendingCurrencySymbol,
                ReceivingCurrencySymbol = result.ReceivingCurrencySymbol,
                SendingCurrency = result.SendingCurrency,
                ReceivingCurrency = result.ReceivingCurrency,
                IsIntroductoryRate = result.IsIntroductoryRate,
                IsIntroductoryFee = result.IsIntroductoryFee,
                ActualFee = result.ActualFee,
                IsValid = result.IsValid
            }, JsonRequestBehavior.AllowGet);

        }
    }

    public class TransferSummaryViewModel
    {

        public decimal Fee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal SendingAmount { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal ExchangeRate { get; set; }
        public string SendingCurrencySymbol { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public bool IsIntroductoryRate { get; set; }
        public bool IsIntroductoryFee { get; set; }
        public decimal ActualFee { get; set; }
        public ServiceResult<bool> IsValid { get; set; }

    }


}