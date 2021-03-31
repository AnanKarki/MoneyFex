using FAXER.PORTAL.Common;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class SenderTransactionHistoryController : Controller
    {
        SSenderTransactionHistory _transactionHistoryServices = null;
        SSenderBankAccountDeposit _senderBankAccountDepositServices = null;
        SSenderCashPickUp _cashPickUpServices = null;
        SSenderKiiPayWalletTransfer _kiiPayWalletTransferServices = null;
        SSenderPayForServices _payForSenderServices = null;
        SSenderMobileMoneyTransfer _mobileMoneyTransferServices = null;
        SPayBill _payBillServices = null;
        STopUpToSupplier _topUpToSupplierServices = null;

        public SenderTransactionHistoryController()
        {
            _transactionHistoryServices = new SSenderTransactionHistory();
            _senderBankAccountDepositServices = new SSenderBankAccountDeposit();
            _cashPickUpServices = new SSenderCashPickUp();
            _kiiPayWalletTransferServices = new SSenderKiiPayWalletTransfer();
            _payForSenderServices = new SSenderPayForServices();
            _mobileMoneyTransferServices = new SSenderMobileMoneyTransfer();
            _payBillServices = new SPayBill();
            _topUpToSupplierServices = new STopUpToSupplier();
        }
        // GET: SenderTransactionHistory
        public ActionResult Index(TransactionServiceType transactionService = TransactionServiceType.All, int year = 0, int month = 0,
             int? page = null, int PageSize = 10, int CurrentpageCount = 0)
        {
            if (Common.FaxerSession.LoggedUser == null)
            {
                return RedirectToAction("Login", "FaxerAccount");
            }
            int faxerId = Common.FaxerSession.LoggedUser.Id;
            SenderTransactionHistoryViewModel vm = new SenderTransactionHistoryViewModel();

            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            vm.Year = year;
            vm.Month = (Month)month;
            ViewBag.SelectedYear = vm.Year;

            if (year == 0 && month == 0)
            {
                vm.Year = DateTime.Now.Year;
                ViewBag.SelectedYear = vm.Year;
                vm.Month = (Month)DateTime.Now.Month;
            }

            SenderTransactionHistoryViewModel data = _transactionHistoryServices.GetTransactionHistories(transactionService, faxerId,
                                                     vm.Year, (int)vm.Month, page ?? 1, PageSize, Common.FaxerSession.LoggedUser.IsBusiness);

            ViewBag.NumberOfPage = 0;
            ViewBag.CurrentpageCount = CurrentpageCount;
            ViewBag.ButtonCount = 0;
            ViewBag.PageSize = PageSize;
            ViewBag.PageNumber = page ?? 1;
            if (data.TransactionHistoryList.Count != 0)
            {
                var TotalCount = data.TransactionHistoryList.FirstOrDefault().TotalCount;
                int NumberOfPage = Common.Common.GetNumberOfPage(TotalCount, PageSize);
                ViewBag.NumberOfPage = NumberOfPage;
                ViewBag.ButtonCount = NumberOfPage > 10 ? 10 : NumberOfPage;
            }
            if (data != null)
            {
                vm.FilterKey = data.FilterKey;

                vm.TransactionHistoryList = data.TransactionHistoryList.ToList();

                //vm.TransactionHistoryList = data.TransactionHistoryList.Where(x => x.TransactionServiceType != TransactionServiceType.BankDeposit && x.TransactionServiceType != TransactionServiceType.CashPickUp  ).ToList();
                //List<SenderTransactionHistoryList> seletedBannkDepositHistory = data.TransactionHistoryList.Where(x => x.TransactionServiceType == TransactionServiceType.BankDeposit && (x.StatusOfBankDepoist == DB.BankDepositStatus.Cancel || x.StatusOfBankDepoist == DB.BankDepositStatus.Confirm)).ToList();
                //List<SenderTransactionHistoryList> seletedCashPickUpHistory = data.TransactionHistoryList.Where(x => x.TransactionServiceType == TransactionServiceType.CashPickUp && (x.Status == DB.FaxingStatus.Completed || x.Status == DB.FaxingStatus.Received)).ToList();
                //vm.TransactionHistoryList.AddRange(seletedBannkDepositHistory);
                //vm.TransactionHistoryList.AddRange(seletedCashPickUpHistory);
            }

            return View(vm);
        }

        public ActionResult TransactionDetail(int id = 0, TransactionServiceType transactionService = TransactionServiceType.All, int SenderId = 0, int Year = 0)
        {
            int faxerId = 0;
            if (SenderId != 0)
            {
                faxerId = SenderId;
            }
            else
            {
                faxerId = Common.FaxerSession.LoggedUser.Id;
            }
            int month = 0;
            SenderTransactionHistoryViewModel vm = _transactionHistoryServices.GetTransactionDetails(transactionService, faxerId, id);
            vm.FilterKey = transactionService;
            ViewBag.FaxerId = faxerId;

            return View(vm);
        }
        public ActionResult DeletePendingTransaction(int id, TransactionServiceType transactionService)
        {
            _transactionHistoryServices.DeletePendingTransaction(id, transactionService);
            return RedirectToAction("Index", "SenderTransferMoneyNow");
        }




        public ActionResult Repeat(string accountNo, int id)
        {
            try
            {


                SenderBankAccountDepositVm vm = new SenderBankAccountDepositVm();
                vm = _senderBankAccountDepositServices.GetAccountInformationFromAccountNumberAndId(accountNo, id);


                SenderBankAccoutDepositEnterAmountVm model = new SenderBankAccoutDepositEnterAmountVm();
                model = _senderBankAccountDepositServices.GetRepeatedTransactionInfo(id);

                _senderBankAccountDepositServices.SetSenderBankAccountDeposit(vm);
                _senderBankAccountDepositServices.SetSenderBankAccoutDepositEnterAmount(model);

                SSenderKiiPayWalletTransfer _senderKiiPayServices = new SSenderKiiPayWalletTransfer();
                if (model != null)
                {
                    _senderKiiPayServices.SetCommonEnterAmount(new CommonEnterAmountViewModel()
                    {

                        ExchangeRate = model.ExchangeRate,
                        Fee = model.Fee,
                        ReceivingAmount = model.ReceivingAmount,
                        SendingAmount = model.SendingAmount,
                        TotalAmount = model.TotalAmount,
                        SendingCountryCode = model.SendingCountryCode,
                        ReceivingCountryCode = model.ReceivingCountryCode,
                        ReceivingCurrency = model.ReceivingCurrency,
                        ReceivingCurrencySymbol = model.ReceivingCurrencySymbol,
                        SendingCurrency = model.SendingCurrency,
                        SendingCurrencySymbol = model.SendingCurrencySymbol,


                    });
                }




                string SenderCountry = Common.FaxerSession.LoggedUser.CountryCode;
                if (vm.CountryCode == SenderCountry)
                {
                    return RedirectToAction("LocalBankAccountDeposit", "SenderBankAccountDeposit");
                }
                return Redirect("/SenderBankAccountDeposit/Index?Country=" + vm.CountryCode);

            }
            catch (Exception)
            {
                return RedirectToAction("Index");
            }
        }
        public ActionResult RepeatCashPickUp(string MFCN, int id)
        {
            SenderCashPickUpVM vm = new SenderCashPickUpVM();

            vm = _cashPickUpServices.GetReceiverInformationFromMFCN(MFCN, id);
            _cashPickUpServices.SetSenderCashPickUp(vm);

            SenderMobileEnrterAmountVm model = new SenderMobileEnrterAmountVm();
            model = _cashPickUpServices.GetRepeatedTransactionInfo(MFCN, id);

            if (model != null)
            {

                SSenderKiiPayWalletTransfer _senderKiiPayServices = new SSenderKiiPayWalletTransfer();
                _senderKiiPayServices.SetCommonEnterAmount(new CommonEnterAmountViewModel()
                {

                    ExchangeRate = model.ExchangeRate,
                    Fee = model.Fee,
                    ReceivingAmount = model.ReceivingAmount,
                    SendingAmount = model.SendingAmount,
                    TotalAmount = model.TotalAmount,
                    SendingCountryCode = model.SendingCountryCode,
                    ReceivingCountryCode = model.ReceivingCountryCode,
                    ReceivingCurrency = model.ReceivingCurrencyCode,
                    ReceivingCurrencySymbol = model.ReceivingCurrencySymbol,
                    SendingCurrency = model.SendingCurrencyCode,
                    SendingCurrencySymbol = model.SendingCurrencySymbol,


                });
            }

            return RedirectToAction("Index", "SenderCashPickUp");

        }
        public ActionResult RepeatKiiPayWallet(string MobileNo, int id)
        {
            SearchKiiPayWalletVM vm = new SearchKiiPayWalletVM();
            vm = _kiiPayWalletTransferServices.GetReceiverInformationFromMobileNo(MobileNo, id);
            _kiiPayWalletTransferServices.SetSearchKiiPayWallet(vm);

            string SenderCountry = Common.FaxerSession.LoggedUser.CountryCode;

            if (vm.CountryCode == SenderCountry)
            {
                return RedirectToAction("SearchLocalKiiPayWallet", "SenderKiiPayWalletTransfer");
            }

            return RedirectToAction("SearchInternationalKiiPayWallet", "SenderKiiPayWalletTransfer");

        }
        public ActionResult RepeatPayForGoodsAndServices(string MobileNo, int id)
        {
            SenderPayForGoodsAndServicesVM vm = new SenderPayForGoodsAndServicesVM();

            vm = _payForSenderServices.GetInformationFromMobileNo(MobileNo);
            _payForSenderServices.SetSenderPayForGoodsAndServices(vm);
            string SenderCountry = Common.FaxerSession.LoggedUser.CountryCode;
            if (vm.CountryCode == SenderCountry)
            {
                return RedirectToAction("PayForServicesLocal", "SenderPayForServices");
            }
            return RedirectToAction("PayForGoodsAndServices", "SenderPayForServices");

        }
        public ActionResult RepeatMobileWallet(string MobileNo, int id)
        {
            SenderMobileMoneyTransferVM vm = new SenderMobileMoneyTransferVM();

            vm = _mobileMoneyTransferServices.GetInformationFromMobileNo(MobileNo, id);
            _mobileMoneyTransferServices.SetSenderMobileMoneyTransfer(vm);

            SenderMobileEnrterAmountVm model = new SenderMobileEnrterAmountVm();
            model = _kiiPayWalletTransferServices.GetRepeatedTranscationIfo(MobileNo, id);

            string SenderCountry = Common.FaxerSession.LoggedUser.CountryCode;
            if (model != null)
            {

                SSenderKiiPayWalletTransfer _senderKiiPayServices = new SSenderKiiPayWalletTransfer();
                _senderKiiPayServices.SetCommonEnterAmount(new CommonEnterAmountViewModel()
                {

                    ExchangeRate = model.ExchangeRate,
                    Fee = model.Fee,
                    ReceivingAmount = model.ReceivingAmount,
                    SendingAmount = model.SendingAmount,
                    TotalAmount = model.TotalAmount,
                    SendingCountryCode = model.SendingCountryCode,
                    ReceivingCountryCode = model.ReceivingCountryCode,
                    ReceivingCurrency = model.ReceivingCurrencyCode,
                    ReceivingCurrencySymbol = model.ReceivingCurrencySymbol,
                    SendingCurrency = model.SendingCurrencyCode,
                    SendingCurrencySymbol = model.SendingCurrencySymbol,

                });
            }
            if (vm.CountryCode == SenderCountry)
            {
                return RedirectToAction("SendingMoneyLocal", "SenderMobileMoneyTransfer");
            }
            return RedirectToAction("Index", "SenderMobileMoneyTransfer", new { CountryCode = vm.CountryCode, WalletId = vm.WalletId });

        }
        public ActionResult RepeatBillPayment(string MobileNo, int id)
        {
            SenderPayMonthlyBillVM vm = new SenderPayMonthlyBillVM();

            vm = _payBillServices.GetInformationFromMobileNo(MobileNo);
            _payBillServices.SetSenderPayMonthlyBillVM(vm);
            string SenderCountry = Common.FaxerSession.LoggedUser.CountryCode;
            if (vm.SupplierCountryCode == SenderCountry)
            {
                return RedirectToAction("PayingALocalSupplier", "SenderPayBills");
            }
            return Redirect("/SenderPayBills/SenderBillingServices?country=" + vm.SupplierCountryCode);

        }
        public ActionResult RepeatTopUp(string MobileNo, int id)
        {
            SenderTopUpAnAccountVM vm = new SenderTopUpAnAccountVM();

            vm = _topUpToSupplierServices.GetInformationFromMobileNo(MobileNo);
            _topUpToSupplierServices.SetSenderTopUpAnAccount(vm);
            string SenderCountry = Common.FaxerSession.LoggedUser.CountryCode;
            if (vm.Country == SenderCountry)
            {
                SenderTopUpSupplierAbroadVm model = new SenderTopUpSupplierAbroadVm();
                model = _topUpToSupplierServices.GetInformationFormMobileOfLocalSupplier(MobileNo);
                _topUpToSupplierServices.SetSenderTopUpSupplierAbroadVm(model);

                return RedirectToAction("TopUpSupplierAbroad", "SenderPayBills");
            }

            return Redirect("/SenderPayBills/SenderTopUpServices?country=" + vm.Country);

        }

        public JsonResult CancelBankDeposit(int Id)
        {

            SSenderBankAccountDeposit _senderBankAccountDepositServices = new SSenderBankAccountDeposit();

            var data = _senderBankAccountDepositServices.List().Data.Where(x => x.Id == Id).FirstOrDefault();
            if (Common.FaxerSession.LoggedUser.Id == data.SenderId)
            {

                data.Status = DB.BankDepositStatus.Cancel;
                _senderBankAccountDepositServices.Update(data);

                return Json(new
                {
                    Status = true,
                    Message = "Cancelled Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Status = false,
                Message = "Cannot cancelled!"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelMobilePayment(int Id)
        {

            SSenderMobileMoneyTransfer _senderMobileMoneyTransferServices = new SSenderMobileMoneyTransfer();

            var data = _senderMobileMoneyTransferServices.list().Data.Where(x => x.Id == Id).FirstOrDefault();
            if (Common.FaxerSession.LoggedUser.Id == data.SenderId)
            {
                data.Status = DB.MobileMoneyTransferStatus.Cancel;
                _senderMobileMoneyTransferServices.Update(data);
                return Json(new
                {
                    Status = true,
                    Message = "Cancelled Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Status = false,
                Message = "Cannot cancelled!"
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult CancelCashPickUp(int Id)
        {
            SSenderCashPickUp _senderCashPickUpServices = new SSenderCashPickUp();

            var data = _senderCashPickUpServices.List().Data.Where(x => x.Id == Id).FirstOrDefault();
            if (Common.FaxerSession.LoggedUser.Id == data.NonCardReciever.FaxerID)
            {
                data.FaxingStatus = DB.FaxingStatus.Cancel;
                data.StatusChangedDate = DateTime.Now;
                _senderCashPickUpServices.Update(data);
                return Json(new
                {
                    Status = true,
                    Message = "Cancelled Successfully"
                }, JsonRequestBehavior.AllowGet);
            }
            return Json(new
            {
                Status = false,
                Message = "Cannot cancelled!"
            }, JsonRequestBehavior.AllowGet);
        }

        //public void PrintReceiptOfBankDeposit(int TransactionId, TransactionServiceType transactionService, int faxerId = 0)
        //{
        //    SenderTransactionHistoryViewModel vm = _transactionHistoryServices.GetTransactionHistories(transactionService, faxerId);
        //    var TransactionHistory = vm.TransactionHistoryList.Where(x => x.Id == TransactionId).FirstOrDefault();



        //    string name = TransactionHistory.ReceiverName;
        //    string firstname = name.Split(' ').First();


        //    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

        //    var ReceiptURL = baseUrl + "/EmailTemplate/AgentTransactionStatement/PrintBankDepositStatement?MFReceiptNumber=" + transactionHistory.ReceiptNumber +
        //     "&TransactionDate=" + transactionHistory.TransactionDate +
        //     "&TransactionTime=" + transactionHistory.TransactionDate +
        //     "&SenderFullName=" + transactionHistory.SenderName +
        //     "&SenderEmail=" + transactionHistory.SenderEmail +
        //     "&SenderTelephone=" + Common.Common.GetCountryPhoneCode(transactionHistory.ReceiverCountry) + " " + transactionHistory.SenderNumber +
        //     "&SenderDOB=" + transactionHistory.SenderDOB +
        //     "&ReceiverFullName=" + transactionHistory.ReceiverName +
        //     "&ReceiverAccount=" + transactionHistory.AccountNumber +
        //     "&ReceiverTelephone=" + Common.Common.GetCountryPhoneCode(transactionHistory.ReceiverCountry) + " " + transactionHistory.ReceiverNumber +
        //     "&AgentName=" + transactionHistory.AgentName +
        //     "&AgentAcountNumber=" + transactionHistory.AgentNumber +
        //     "&StaffName=" + transactionHistory.TransactionStaff +
        //     "&ReceiverCountry=" + transactionHistory.ReceiverCountry +
        //     "&TotalAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountPaid +
        //     "&Fee=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.Fee +
        //     "&SendingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.AmountSent +
        //     "&ReceivingAmount=" + transactionHistory.SendingCurrencySymbol + "" + transactionHistory.ReceivingAmount + transactionHistory.SendingCurrency +
        //     "&ExchangeRate=" + transactionHistory.ExchangeRate +
        //     "&SendingCurrency=" + transactionHistory.SendingCurrency +
        //     "&ReceivingCurrency=" + transactionHistory.ReceivingCurrrency +
        //     "&ReceiverName=" + firstname +
        //     "&PaymentMethod=" + transactionHistory.PaymentMethod +
        //     "&BankName=" + transactionHistory.BankName +
        //     "&BankBranch=" + transactionHistory.BankBranch;


        //    var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);
        //    //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
        //    //ReceiptPDF.Save(path);
        //    byte[] bytes = ReceiptPDF.Save();
        //    string mimeType = "Application/pdf";
        //    Response.Buffer = true;
        //    Response.Clear();
        //    Response.ContentType = mimeType;
        //    Response.OutputStream.Write(bytes, 0, bytes.Length);
        //    Response.Flush();
        //    Response.End();
        //    //return File(path, "application/pdf");

        //}



    }
}