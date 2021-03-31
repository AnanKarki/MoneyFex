using FAXER.PORTAL.Common;
using FAXER.PORTAL.Services;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class RefundsFormNonCardFaxingRefundRequestController : Controller
    {
        Services.CommonServices CommonService = new Services.CommonServices();
        // GET: Admin/RefundsFormNonCardFaxingRefundRequest
        [HttpGet]
        public ActionResult Index(string MFCN)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (MFCN == "success")
            {
                ViewBag.Message = "Refund Request Completed !";
                MFCN = "";
            }
            if (!string.IsNullOrEmpty(MFCN))
            {
                Services.NonCardFaxingRefundRequestServices services = new Services.NonCardFaxingRefundRequestServices();
                var model = services.GetFaxingNonCardDetails(MFCN);
                if (model != null)
                {
                    return View(model);
                }
                else
                {
                    ModelState.AddModelError("InvalidMFCN", "Invalid MFCN Number");
                }

            }
            var vm = new ViewModels.NonCardFaxingRefundRequestViewModel();
            vm.StatusOfFaxName = "";
            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = ViewModels.NonCardFaxingRefundRequestViewModel.BindProperty)]ViewModels.NonCardFaxingRefundRequestViewModel model)
        {

            if (ModelState.IsValid)
            {

                if (model.YesConfirmed == false)
                {

                    ModelState.AddModelError("YesConfirmed", "Please check yes if you want to refund");
                    return View(model);
                }

                if ((model.StatusOfFax == DB.FaxingStatus.Received) || (model.StatusOfFax == DB.FaxingStatus.Refund))
                {

                    ModelState.AddModelError("StatusOfFaxName", "Request Money has already been received or refunded");
                    return View(model);
                }
                if (model.OperatingUserType == DB.OperatingUserType.Agent)
                {

                    return View(model);
                }
                Services.NonCardFaxingRefundRequestServices services = new Services.NonCardFaxingRefundRequestServices();

                var noncardTransaction = services.GetFaxingNonCardTransactionDetails(model.MFCNNumber);
                if (noncardTransaction != null)
                {


                    #region  Strip portion
                    // StripeConfiguration.SetApiKey("sk_test_OJiTq50I3SNoO3tL3bOJBhTy");
                    StripeConfiguration.SetApiKey(System.Configuration.ConfigurationManager.AppSettings["StripeApiLiveKey"]);

                    var refundService = new StripeRefundService();
                    //var refundOptions = new StripeRefundListOptions
                    //{
                    //    ChargeId = noncardTransaction.stripe_ChargeId

                    //};
                    var refundOptions = new StripeRefundCreateOptions()
                    {
                        Amount = (int)noncardTransaction.FaxingAmount * 100,

                    };
                    try
                    {
                        StripeRefund refund = refundService.Create(noncardTransaction.stripe_ChargeId, refundOptions);

                    }
                    catch (Exception e)
                    {



                    }
                    #endregion



                    DB.ReFundNonCardFaxMoneyByAdmin reFundNonCard = new DB.ReFundNonCardFaxMoneyByAdmin()
                    {

                        NonCardTransaction_id = noncardTransaction.Id,
                        Staff_id = Common.StaffSession.LoggedStaff.StaffId,
                        RefundReason = model.RefundReason,
                        RefundedDate = DateTime.Now,
                        NameofRefunder = model.NameOfRefunder,
                        ReceiptNumber = services.GetNewRefundReceiptNumber()

                    };
                    var result = services.RefundFaxMoney(reFundNonCard);
                    if (result == true)
                    {
                        var updateFaxingStatus = services.UpdateStatusofFax(model.MFCNNumber);

                        // Send Email For Refund Request bu admin 
                        MailCommon mail = new MailCommon();
                        var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                        string body = "";
                        string FaxerName = model.FaxerFirstName + " " + model.FaxerMiddleName + " " + model.FaxerLastName;
                        string RecevierName = model.ReceiverFirstName + " " + model.ReceiverMiddleName + " " + model.ReceiverLastName;
                        string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(model.ReceiverCountryCode);
                        //string ReceiverCounty = CommonService.getCountryNameFromCode(model.ReceiverCountry);
                        string FaxerCurrency = Common.Common.GetCountryCurrency(model.FaxerCountryCode);
                        body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/RefundRequestByAdminEmail?FaxerName=" + FaxerName +
                        "&MFCNNumber=" + model.MFCNNumber + "&FaxedAmount=" + model.FaxingAmount + "&ReceiverName=" + RecevierName +
                        "&ReceiverCountry=" + model.ReceiverCountry + "&ReceiverCity=" + model.ReceiverCity
                        + "&FaxedDate=" + model.TransactionDate.ToString("dd/MM/yyyy") + "&NameOfAdminRefundRequester=" + model.NameOfRefunder + "");

                        var ReceiptUrl = baseUrl + "/EmailTemplate/AdminRefundReceipt?ReceiptNumber=" + reFundNonCard.ReceiptNumber +
                          "&TransactionReceiptNumber=" + model.FaxReceiptNumber + "&Date=" + reFundNonCard.RefundedDate.ToString("dd/MM/yyyy") +
                           "&Time=" + reFundNonCard.RefundedDate.ToString("HH:mm") + "&SenderFullName=" + FaxerName + "&MFCN=" + model.MFCNNumber +
                           "&ReceiverFullName=" + RecevierName + "&Telephone=" + ReceiverPhoneCode + " " + model.ReceiverTelephone +
                           "&RefundingAdminName=" + reFundNonCard.NameofRefunder + "&RefundingAdminCode=" + Common.StaffSession.LoggedStaff.StaffMFSCode
                           + "&OrignalAmountSent=" + model.FaxingAmount +
                           "&RefundedAmount=" + model.FaxingAmount + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + FaxerCurrency +
                             "&ReceiverCountry=" + Common.Common.GetCountryName(model.ReceiverCountryCode) + "&ReceiverCity=" + model.ReceiverCity
                             + "&RefundingType=" + Common.StaffSession.LoggedStaff.LoginCode;


                        var ReceiptPdf = Common.Common.GetPdf(ReceiptUrl);
                        //mail.SendMail("anankarki97@gmail.com", "Refund Request from MoneyFex ", body, ReceiptPdf);
                        mail.SendMail(model.FaxerEmail, "Refund Request from MoneyFex ", body, ReceiptPdf);
                        var vm = new ViewModels.NonCardFaxingRefundRequestViewModel();
                        TempData["Refunded"] = "The Amount has been Refunded";
                        vm.StatusOfFaxName = "";
                        ViewBag.Message = "The Amount has been Succesfully Refunded";
                        return RedirectToAction("Index", new { MFCN = "success" });
                    }
                }
            }

            return View(model);
        }

        public void printReceipt(string MFCN)
        {
            Services.NonCardFaxingRefundRequestServices services = new Services.NonCardFaxingRefundRequestServices();
            var data = services.getDetailsFromMFCN(MFCN);
            if (data != null)
            {
                if (data.OperatingUserType == DB.OperatingUserType.Admin)
                {
                    var details = services.getAdminTransferData(MFCN);
                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    var ReceiptUrl = baseUrl + "/EmailTemplate/AdminNonCardMoneyTransfer?MFReceiptNumber=" + details.MFReceiptNumber
                        + "&TransactionDate=" + details.TransactionDate + "&TransactionTime=" + details.TransactionTime + "&FaxerFullName="
                        + details.FaxerFullName + "&MFCN=" + details.MFCN + "&ReceiverFullName=" + details.ReceiverFullName + "&Telephone="
                        + details.SenderTelephoneNo + "&StaffName=" + details.StaffName + "&StaffCode=" + details.StaffCode + "&AmountSent=" + details.AmountSent
                        + "&ExchangeRate=" + details.ExchangeRate + "&Fee=" + details.Fee + "&AmountReceived=" + details.AmountReceived + "&TotalAmountSentAndFee="
                        + details.TotalAmountSentAndFee + "&SendingCurrency=" + details.SendingCurrency + "&ReceivingCurrency=" + details.ReceivingCurrency +
                        "&PaymentType=" + details.StaffLoginCode;

                    var ReceiptPDF = Common.Common.GetPdf(ReceiptUrl);
                    byte[] bytes = ReceiptPDF.Save();
                    string mimeType = "Application/pdf";
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = mimeType;
                    Response.OutputStream.Write(bytes, 0, bytes.Length);
                    Response.Flush();
                    Response.End();

                }
                else if (data.OperatingUserType == DB.OperatingUserType.Agent)
                {
                    var details = services.getAgentTransferData(MFCN);

                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    var ReceiptUrl = baseUrl + "/EmailTemplate/AgentMoneySenderReceipt?MFReceiptNumber=" + details.MFReceiptNumber
                        + "&TransactionDate=" + details.TransactionDate + "&TransactionTime=" + details.TransactionTime + "&FaxerFullName="
                        + details.FaxerFullName + "&MFCN=" + details.MFCN + "&ReceiverFullName=" + details.ReceiverFullName + "&Telephone="
                        + details.Telephone + "&AgentName=" + details.AgentName + "&AgentCode=" + details.AgentCode + "&AmountSent="
                        + details.AmountSent + "&ExchangeRate=" + details.ExchangeRate + "&Fee=" + details.Fee + "&AmountReceived="
                        + details.AmountReceived + "&TotalAmountSentAndFee=" + details.TotalAmountSentAndFee;

                    var ReceiptPDF = Common.Common.GetPdf(ReceiptUrl);
                    byte[] bytes = ReceiptPDF.Save();
                    string mimeType = "Application/pdf";
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = mimeType;
                    Response.OutputStream.Write(bytes, 0, bytes.Length);
                    Response.Flush();
                    Response.End();
                }
                else if (data.OperatingUserType == DB.OperatingUserType.Sender)
                {
                    var details = services.getFaxerTransferData(MFCN);


                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    var ReceiptUrl = baseUrl + "/EmailTemplate/NonCardUserReceiver?MFReceiptNumber=" + details.MFReceiptNumber + "&TransactionDate="
                        + details.TransactionDate + "&TransactionTime=" + details.TransactionTime + "&FaxerFullName=" + details.FaxerFullName + "&MFCN="
                        + details.MFCN + "&ReceiverFullName=" + details.ReceiverFullName + "&Telephone=" + details.Telephone + "&AgentName=" + details.AgentName
                        + "&AgentCode=" + details.AgentCode + "&AmountSent=" + details.AmountSent + "&ExchangeRate=" + details.ExchangeRate + "&Fee=" + details.Fee + "&AmountReceived="
                        + details.AmountReceived + "&SendingCurrency=" + details.SendingCurrency + "&ReceivingCurrency=" + details.ReceivingCurrency;

                    var ReceiptPDF = Common.Common.GetPdf(ReceiptUrl);
                    byte[] bytes = ReceiptPDF.Save();
                    string mimeType = "Application/pdf";
                    Response.Buffer = true;
                    Response.Clear();
                    Response.ContentType = mimeType;
                    Response.OutputStream.Write(bytes, 0, bytes.Length);
                    Response.Flush();
                    Response.End();
                }
            }
        }
        public void printReceiptOfManualDeposit(string AccountNo, int Id)
        {

            SSenderBankAccountDeposit _service = new SSenderBankAccountDeposit();

            var details = _service.GetBankDepositInfo(AccountNo, Id);
            string SenderName = _service.GetSenderName(details.SenderId);

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var ReceiptUrl = baseUrl + "/EmailTemplate/BankDepositReceipt?MFReceiptNumber=" + details.ReceiptNo +
                "&TransactionDate=" + details.TransactionDate.ToShortDateString() +
                "&TransactionTime=" + details.TransactionDate.ToShortTimeString() +
                "&FaxerFullName=" + SenderName +
                "&AccountNo=" + details.ReceiverAccountNo +
                "&ReceiverFullName=" + details.ReceiverName +
                "&Telephone=" + details.ReceiverMobileNo +
                "&AmountSent=" + details.SendingAmount +
                "&ExchangeRate=" + details.ExchangeRate +
                "&Fee=" + details.Fee +
                "&AmountReceived=" + details.ReceivingAmount +
                "&SendingCurrency=" + Common.Common.GetCountryCurrency(details.SendingCountry) +
                "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(details.ReceivingCountry) +
                "&BankName=" + Common.Common.getBankName(details.BankId) +
                "&BranchCode=" + details.BankCode +
                "&ReceivingCountry=" + Common.Common.GetCountryName(details.ReceivingCountry);

            var ReceiptPDF = Common.Common.GetPdf(ReceiptUrl);
            byte[] bytes = ReceiptPDF.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();


        }
        public void printReceiptOfMobileWallet(string AccountNo, int Id)
        {
            SSenderMobileMoneyTransfer _service = new SSenderMobileMoneyTransfer();
            SSenderBankAccountDeposit _services = new SSenderBankAccountDeposit();

            var details = _service.GetMobileInformationWithMoblieNoAndId(AccountNo, Id);

            string SenderName = _services.GetSenderName(details.SenderId);

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var ReceiptUrl = baseUrl + "/EmailTemplate/MoblieWaletReceipt?MFReceiptNumber=" + details.ReceiptNo +
                "&TransactionDate=" + details.TransactionDate.ToShortDateString() +
                "&TransactionTime=" + details.TransactionDate.ToShortTimeString() +
                "&FaxerFullName=" + SenderName +
                "&AccountNo=" + details.PaidToMobileNo +
                "&ReceiverFullName=" + details.ReceiverName +
                "&Telephone=" + details.PaidToMobileNo +
                "&AmountSent=" + details.SendingAmount +
                "&ExchangeRate=" + details.ExchangeRate +
                "&Fee=" + details.Fee +
                "&AmountReceived=" + details.ReceivingAmount +
                "&SendingCurrency=" + Common.Common.GetCountryCurrency(details.SendingCountry) +
                "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(details.ReceivingCountry);

            var ReceiptPDF = Common.Common.GetPdf(ReceiptUrl);
            byte[] bytes = ReceiptPDF.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();


        }

        public void printReceiptKiiPay(int id)
        {
            Services.NonCardFaxingRefundRequestServices services = new Services.NonCardFaxingRefundRequestServices();
            if (id != 0)
            {
                var data = services.getDetailsFromId(id);
                SSenderBankAccountDeposit _services = new SSenderBankAccountDeposit();

                string SenderName = _services.GetSenderName(data.FaxerId);

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                var ReceiptUrl = baseUrl + "/EmailTemplate/KiiPayWallet?MFReceiptNumber=" + data.ReceiptNumber +
                    "&TransactionDate=" + data.TransactionDate.ToShortDateString() +
                    "&TransactionTime=" + data.TransactionDate.ToShortTimeString() +
                    "&FaxerFullName=" + SenderName +
                    "&AccountNo=" + data.TransferToMobileNo +
                    "&ReceiverFullName=" + data.KiiPayPersonalWalletInformation.FirstName + " " + data.KiiPayPersonalWalletInformation.MiddleName + " " + data.KiiPayPersonalWalletInformation.LastName +
                    "&Telephone=" + data.TransferToMobileNo +
                    "&AmountSent=" + data.FaxingAmount +
                    "&ExchangeRate=" + data.ExchangeRate +
                    "&Fee=" + data.FaxingFee +
                    "&AmountReceived=" + data.RecievingAmount +
                    "&SendingCurrency=" + Common.Common.GetCountryCurrency(data.SendingCountry) +
                    "&ReceivingCurrency=" + Common.Common.GetCountryCurrency(data.ReceivingCountry);

                var ReceiptPDF = Common.Common.GetPdf(ReceiptUrl);
                byte[] bytes = ReceiptPDF.Save();
                string mimeType = "Application/pdf";
                Response.Buffer = true;
                Response.Clear();
                Response.ContentType = mimeType;
                Response.OutputStream.Write(bytes, 0, bytes.Length);
                Response.Flush();
                Response.End();
            }
        }

    }
}