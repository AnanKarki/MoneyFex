using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using Rotativa;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class PayAReceiverController : Controller
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        CommonServices CommonService = new CommonServices();
        AgentServices.PayAReceiverControllerServices payAReceiverServices = new AgentServices.PayAReceiverControllerServices();
        PayAReceiverControllerServices _payAReceiverServices = null;
        public PayAReceiverController()
        {
            _payAReceiverServices = new PayAReceiverControllerServices();
        }

        // GET: Agent/PayAReceiver
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        #region receipts print functions

        public void PrintMFTCCardWithdrawlReceipt(int TransactionId)
        {


            var CardWithdrawl = dbContext.UserCardWithdrawl.Where(x => x.Id == TransactionId).FirstOrDefault();

            var CarduserInformation = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == CardWithdrawl.KiiPayPersonalWalletInformationId).FirstOrDefault();

            var agentInformatioin = dbContext.AgentInformation.Where(x => x.Id == CardWithdrawl.AgentInformationId).FirstOrDefault();

            // Need Changes Here  Because the sender info is conditional now 
            //the Kiipay Personal might or might not be associated with the faxer
            //var faxerInformation = dbContext.FaxerInformation.Where(x => x.Id == CarduserInformation.FaxerId).FirstOrDefault();
            string CardUserCurrency = Common.Common.GetCountryCurrency(CarduserInformation.CardUserCountry);
            string PhoneCode = Common.Common.GetCountryPhoneCode(CarduserInformation.CardUserCountry);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var ReceiptURL = baseUrl + "/EmailTemplate/MFTCCardUserReceiverReceipt?MFReceiptNumber=" + CardWithdrawl.ReceiptNumber +
                  "&TransactionDate=" + CardWithdrawl.TransactionDate.ToString("dd/MM/yyyy")
                  + "&TransactionTime=" + CardWithdrawl.TransactionDate.ToString("HH:mm") +
                  "&FaxerFullName=" + "" + " " + "" + " " + ""
                  + "&FaxerCountry=" + Common.Common.GetCountryName("") + "&FaxerCity=" + "" +
                  "&MFTCCardNumber=" + CarduserInformation.MobileNo.Decrypt() +
                   "&CardUserFullName=" + CarduserInformation.FirstName + " " + CarduserInformation.MiddleName + " " + CarduserInformation.LastName +
                   "&CardUserCountry=" + Common.Common.GetCountryName(CarduserInformation.CardUserCountry) + "&CardUserCity=" + CarduserInformation.CardUserCity +
                   "&Telephone=" + PhoneCode + " " + CarduserInformation.CardUserTel + "&AgentName=" + agentInformatioin.Name + "&AgentCode=" + agentInformatioin.AccountNo
                   + "&AmountRequested=" + CardUserCurrency + " " + CardWithdrawl.TransactionAmount + "&ExchangeRate=" + "No Exchange Rate"
                   + "&Fee=" + "No Fee" + "&AmountWithdrawn=" + CardUserCurrency + " " + CardWithdrawl.TransactionAmount +
                   "&AgentCountry=" + Common.Common.GetCountryName(agentInformatioin.CountryCode) + "&AgentCity=" + agentInformatioin.City
                   + "&AgentTelephoneNumber=" + Common.Common.GetCountryPhoneCode(agentInformatioin.CountryCode) + " " + agentInformatioin.PhoneNumber;

            var ReceiptPDF = Common.Common.GetPdf(ReceiptURL);
            //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
            //ReceiptPDF.Save(path);
            byte[] bytes = ReceiptPDF.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
            //return File(path, "application/pdf");

        }
        public void PrintNonCardReceiverswithdrawlReceipt(string MFCN)
        {

            var nonCardwithdrawl = dbContext.ReceiverNonCardWithdrawl.Where(x => x.MFCN == MFCN).FirstOrDefault();

            var nonCardTransaction = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            var receiverDetails = dbContext.ReceiversDetails.Where(x => x.Id == nonCardwithdrawl.ReceiverId).FirstOrDefault();

            var faxerInformation = dbContext.FaxerInformation.Where(x => x.Id == receiverDetails.FaxerID).FirstOrDefault();

            var agentInformation = dbContext.AgentInformation.Where(x => x.Id == nonCardwithdrawl.AgentId).FirstOrDefault();

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string FaxerCurrency = Common.Common.GetCountryCurrency(faxerInformation.Country);
            string ReceiverCurrency = Common.Common.GetCountryCurrency(receiverDetails.Country);
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(receiverDetails.Country);


            var ReceiptUrl = baseUrl + "/EmailTemplate/NonCardUserWithrawlReceipt?MFReceiptNumber=" + nonCardwithdrawl.ReceiptNumber + "&TransactionDate="
                + nonCardwithdrawl.TransactionDate.ToString("dd/MM/yyyy") + "&TransactionTime=" + nonCardwithdrawl.TransactionDate.ToString("HH:mm") +
                "&FaxerFullName=" + faxerInformation.FirstName + " " + faxerInformation.MiddleName + " " + faxerInformation.LastName
                + "&MFCN=" + nonCardTransaction.MFCN + "&ReceiverFullName=" + receiverDetails.FirstName + " " +
                receiverDetails.MiddleName + " " + receiverDetails.LastName + "&Telephone=" + ReceiverPhoneCode + " " + receiverDetails.PhoneNumber +
                "&AgentName=" + agentInformation.Name + "&AgentCode=" + agentInformation.AccountNo +
                "&AmountSent=" + nonCardTransaction.FaxingAmount +
                "&ExchangeRate=" + nonCardTransaction.ExchangeRate + "&Fee=" + nonCardTransaction.FaxingFee +
                "&AmountReceived=" + nonCardTransaction.ReceivingAmount + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurrency +
                "&AgentCountry=" + Common.Common.GetCountryName(agentInformation.CountryCode) + "&AgentCity=" + agentInformation.City
                + "&AgentTelephoneNumber=" + Common.Common.GetCountryPhoneCode(agentInformation.CountryCode) + " " + agentInformation.PhoneNumber;


            var ReceiptPdf = Common.Common.GetPdf(ReceiptUrl);

            //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
            //ReceiptPdf.Save(path);
            byte[] bytes = ReceiptPdf.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
            //return File(path, "application/pdf");
            //Process.Start(path);

            //return RedirectToAction("PrintMFTCCardWithdrawlReceipt");

        }
        public void PrintCardUserNonCardWithdrawalReceipt(string MFCN)
        {

            var nonCardwithdrawl = dbContext.CardUserNonCardWithdrawal.Where(x => x.MFCN == MFCN).FirstOrDefault();

            var nonCardTransaction = dbContext.CardUserNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            var receiverDetails = dbContext.CardUserReceiverDetails.Where(x => x.Id == nonCardwithdrawl.ReceiverId).FirstOrDefault();

            var faxerInformation = receiverDetails.MFTCCardInformation;

            var agentInformation = dbContext.AgentInformation.Where(x => x.Id == nonCardwithdrawl.AgentId).FirstOrDefault();

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string FaxerCurrency = Common.Common.GetCountryCurrency(faxerInformation.CardUserCountry);
            string ReceiverCurrency = Common.Common.GetCountryCurrency(receiverDetails.Country);
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(receiverDetails.Country);
            var ReceiptUrl = baseUrl + "/EmailTemplate/NonCardUserWithrawlReceipt?MFReceiptNumber=" + nonCardwithdrawl.ReceiptNumber + "&TransactionDate="
                + nonCardwithdrawl.TransactionDate.ToString("dd/MM/yyyy") + "&TransactionTime=" + nonCardwithdrawl.TransactionDate.ToString("HH:mm") +
                "&FaxerFullName=" + faxerInformation.FirstName + " " + faxerInformation.MiddleName + " " + faxerInformation.LastName
                + "&MFCN=" + nonCardTransaction.MFCN + "&ReceiverFullName=" + receiverDetails.FirstName + " " +
                receiverDetails.MiddleName + " " + receiverDetails.LastName + "&Telephone=" + ReceiverPhoneCode + " " + receiverDetails.PhoneNumber +
                "&AgentName=" + agentInformation.Name + "&AgentCode=" + agentInformation.AccountNo +
                "&AmountSent=" + nonCardTransaction.FaxingAmount +
                "&ExchangeRate=" + nonCardTransaction.ExchangeRate + "&Fee=" + nonCardTransaction.FaxingFee +
                "&AmountReceived=" + nonCardTransaction.ReceivingAmount + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurrency +
                "&AgentCountry=" + Common.Common.GetCountryName(agentInformation.CountryCode) + "&AgentCity=" + agentInformation.City
                + "&AgentTelephoneNumber=" + Common.Common.GetCountryPhoneCode(agentInformation.CountryCode) + " " + agentInformation.PhoneNumber;


            var ReceiptPdf = Common.Common.GetPdf(ReceiptUrl);

            //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
            //ReceiptPdf.Save(path);
            byte[] bytes = ReceiptPdf.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
        }
        public void PrintMerchantNonCardWithdrawalReceipt(string MFCN)
        {


            var nonCardwithdrawl = dbContext.MerchantNonCardWithdrawal.Where(x => x.MFCN == MFCN).FirstOrDefault();

            var nonCardTransaction = dbContext.MerchantNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            var receiverDetails = dbContext.MerchantNonCardReceiverDetail.Where(x => x.Id == nonCardwithdrawl.ReceiverId).FirstOrDefault();

            var faxerInformation = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == nonCardTransaction.MFBCCardID).FirstOrDefault();

            var agentInformation = dbContext.AgentInformation.Where(x => x.Id == nonCardwithdrawl.AgentId).FirstOrDefault();

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string FaxerCurrency = Common.Common.GetCountryCurrency(faxerInformation.Country);
            string ReceiverCurrency = Common.Common.GetCountryCurrency(receiverDetails.Country);
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(receiverDetails.Country);
            var ReceiptUrl = baseUrl + "/EmailTemplate/NonCardUserWithrawlReceipt?MFReceiptNumber=" + nonCardwithdrawl.ReceiptNumber + "&TransactionDate="
                + nonCardwithdrawl.TransactionDate.ToString("dd/MM/yyyy") + "&TransactionTime=" + nonCardwithdrawl.TransactionDate.ToString("HH:mm") +
                "&FaxerFullName=" + faxerInformation.FirstName + " " + faxerInformation.MiddleName + " " + faxerInformation.LastName
                + "&MFCN=" + nonCardTransaction.MFCN + "&ReceiverFullName=" + receiverDetails.FirstName + " " +
                receiverDetails.MiddleName + " " + receiverDetails.LastName + "&Telephone=" + ReceiverPhoneCode + " " + receiverDetails.PhoneNumber +
                "&AgentName=" + agentInformation.Name + "&AgentCode=" + agentInformation.AccountNo +
                "&AmountSent=" + nonCardTransaction.FaxingAmount +
                "&ExchangeRate=" + nonCardTransaction.ExchangeRate + "&Fee=" + nonCardTransaction.FaxingFee +
                "&AmountReceived=" + nonCardTransaction.ReceivingAmount + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurrency +
                "&AgentCountry=" + Common.Common.GetCountryName(agentInformation.CountryCode) + "&AgentCity=" + agentInformation.City
                + "&AgentTelephoneNumber=" + Common.Common.GetCountryPhoneCode(agentInformation.CountryCode) + " " + agentInformation.PhoneNumber;


            var ReceiptPdf = Common.Common.GetPdf(ReceiptUrl);

            //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
            //ReceiptPdf.Save(path);
            byte[] bytes = ReceiptPdf.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
        }
        public void PrintAgentMoneySenderReceiverReceipt(string MFCN)
        {


            var nonCardTransaction = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            var receiverDetails = dbContext.ReceiversDetails.Where(x => x.Id == nonCardTransaction.NonCardRecieverId).FirstOrDefault();

            var faxerInformation = dbContext.FaxerInformation.Where(x => x.Id == receiverDetails.FaxerID).FirstOrDefault();

            var agentInformation = dbContext.AgentFaxMoneyInformation.Where(x => x.NonCardTransactionId == nonCardTransaction.Id).FirstOrDefault();
            var AgentInfo = dbContext.AgentInformation.Where(x => x.Id == agentInformation.AgentId).FirstOrDefault();
            var totalAmount = nonCardTransaction.FaxingAmount + nonCardTransaction.FaxingFee;
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string FaxerCurrency = Common.Common.GetCountryCurrency(AgentInfo.CountryCode);
            string ReceiverCurrency = Common.Common.GetCountryCurrency(receiverDetails.Country);
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(receiverDetails.Country);
            var ReceiptUrl = baseUrl + "/EmailTemplate/AgentMoneySenderReceipt?MFReceiptNumber=" + nonCardTransaction.ReceiptNumber + "&TransactionDate="
                + nonCardTransaction.TransactionDate.ToString("dd/MM/yyyy") + "&TransactionTime=" + nonCardTransaction.TransactionDate.ToString("HH:mm") +
                "&FaxerFullName=" + faxerInformation.FirstName + " " + faxerInformation.MiddleName + " " + faxerInformation.LastName
                + "&MFCN=" + nonCardTransaction.MFCN + "&ReceiverFullName=" + receiverDetails.FirstName + " " +
                receiverDetails.MiddleName + " " + receiverDetails.LastName + "&Telephone=" + ReceiverPhoneCode + " " + receiverDetails.PhoneNumber +
                "&AgentName=" + AgentInfo.Name + "&AgentCode=" + AgentInfo.AccountNo +
                "&AmountSent=" + nonCardTransaction.FaxingAmount +
                "&ExchangeRate=" + nonCardTransaction.ExchangeRate + "&Fee=" + nonCardTransaction.FaxingFee +
                "&AmountReceived=" + nonCardTransaction.ReceivingAmount + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurrency + "&TotalAmountSentAndFee=" + totalAmount;


            var ReceiptPdf = Common.Common.GetPdf(ReceiptUrl);

            //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
            //ReceiptPdf.Save(path);
            byte[] bytes = ReceiptPdf.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
            //return File(path, "application/pdf");
            //Process.Start(path);
            //return RedirectToAction("PrintMFTCCardWithdrawlReceipt");


        }
        public void PrintBusinessCardWithdrawlReceipt(int TransactionId)
        {

            var withdrawl = dbContext.MFBCCardWithdrawls.Where(x => x.Id == TransactionId).FirstOrDefault();

            var MFBCCardInfo = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == withdrawl.KiiPayBusinessWalletInformationId).FirstOrDefault();
            var BusinessInfo = dbContext.KiiPayBusinessInformation.Where(x => x.Id == MFBCCardInfo.KiiPayBusinessInformationId).FirstOrDefault();

            var agentInfo = dbContext.AgentInformation.Where(x => x.Id == withdrawl.AgentInformationId).FirstOrDefault();
            string Currency = Common.Common.GetCountryCurrency(MFBCCardInfo.Country);
            string Phonecode = Common.Common.GetCountryPhoneCode(MFBCCardInfo.Country);
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

            var ReceiptUrl = baseUrl + "/EmailTemplate/MFBCCardUserWithdrawlReceipt?MFReceiptNumber=" + withdrawl.ReceiptNumber +
                "&TransactionDate=" + withdrawl.TransactionDate.ToString("dd/MM/yyyy") + "&TransactionTime=" + withdrawl.TransactionDate.ToString("HH:mm") +
                "&BusinessMerchantName=" + BusinessInfo.BusinessName +
                "&MFBCCardNumber=" + MFBCCardInfo.MobileNo +
            "&BusinessCardUserFullName=" + MFBCCardInfo.FirstName + " " + MFBCCardInfo.MiddleName + " " + MFBCCardInfo.LastName +
            "&BusinessCountry=" + Common.Common.GetCountryName(BusinessInfo.BusinessOperationCountryCode) + "&BusinessCity=" + BusinessInfo.BusinessOperationCity +
            "&Telephone=" + Phonecode + " " + MFBCCardInfo.PhoneNumber +
            "&AgentName=" + agentInfo.Name + "&AgentCode=" + agentInfo.AccountNo +
            "&AmountRequested=" + withdrawl.TransactionAmount +
            "&ExchangeRate=" + "No Exchange Rate " + "&Fee=" + "No Fee" +
            "" + "&AmountWithdrawn=" + withdrawl.TransactionAmount + "&Currency=" + Currency + "&AgentCountry=" + Common.Common.GetCountryName(agentInfo.CountryCode) +
            "&AgentCity=" + agentInfo.City + "&AgentTelephoneNumber=" + Common.Common.GetCountryPhoneCode(agentInfo.CountryCode) + agentInfo.PhoneNumber;

            var ReceiptPdf = Common.Common.GetPdf(ReceiptUrl);
            //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";

            //ReceiptPdf.Save(path);
            //return File(path, "application/pdf");
            byte[] bytes = ReceiptPdf.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
            //Process.Start(path);

            //return RedirectToAction("PrintMFTCCardWithdrawlReceipt");


        }
        public void PrintNonCardUserReceipt(string MFCN)
        {
            var nonCardTransaction = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            var receiverDetails = dbContext.ReceiversDetails.Where(x => x.Id == nonCardTransaction.NonCardRecieverId).FirstOrDefault();

            var faxerInformation = dbContext.FaxerInformation.Where(x => x.Id == receiverDetails.FaxerID).FirstOrDefault();

            //var agentInformation = dbContext.AgentInformation.Where(x => x.Id == nonCardTransaction.).FirstOrDefault();
            string AgentName = "";
            string AgentCode = "";
            if (nonCardTransaction.OperatingUserType == OperatingUserType.Agent)
            {
                var AgentInfo = dbContext.AgentFaxMoneyInformation.Where(x => x.NonCardTransactionId == nonCardTransaction.Id).FirstOrDefault();

                if (AgentInfo != null)
                {
                    var agentDetails = dbContext.AgentInformation.Where(x => x.Id == AgentInfo.AgentId).FirstOrDefault();
                    AgentName = agentDetails.Name;
                    AgentCode = agentDetails.AccountNo;

                }
            }
            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            string FaxerCurrency = Common.Common.GetCountryCurrency(faxerInformation.Country);
            string ReceiverCurrency = Common.Common.GetCountryCurrency(receiverDetails.Country);
            string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(receiverDetails.Country);
            var ReceiptUrl = baseUrl + "/EmailTemplate/NonCardUserReceiver?MFReceiptNumber=" + nonCardTransaction.ReceiptNumber + "&TransactionDate="
                + nonCardTransaction.TransactionDate.ToString("dd/MM/yyyy") + "&TransactionTime=" + nonCardTransaction.TransactionDate.ToString("HH:mm") +
                "&FaxerFullName=" + faxerInformation.FirstName + " " + faxerInformation.MiddleName + " " + faxerInformation.LastName
                + "&MFCN=" + nonCardTransaction.MFCN + "&ReceiverFullName=" + receiverDetails.FirstName + " " +
                receiverDetails.MiddleName + " " + receiverDetails.LastName + "&Telephone=" + ReceiverPhoneCode + " " + receiverDetails.PhoneNumber +
                "&AgentName=" + AgentName + "&AgentCode=" + AgentCode +
                "&AmountSent=" + nonCardTransaction.FaxingAmount +
                "&ExchangeRate=" + nonCardTransaction.ExchangeRate + "&Fee=" + nonCardTransaction.FaxingFee +
                "&AmountReceived=" + nonCardTransaction.ReceivingAmount + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurrency;


            var ReceiptPdf = Common.Common.GetPdf(ReceiptUrl);

            //var path = HostingEnvironment.MapPath(@"/Documents/") + "attachment.pdf";
            //ReceiptPdf.Save(path);
            //return File(path, "application/pdf");
            byte[] bytes = ReceiptPdf.Save();
            string mimeType = "Application/pdf";
            Response.Buffer = true;
            Response.Clear();
            Response.ContentType = mimeType;
            Response.OutputStream.Write(bytes, 0, bytes.Length);
            Response.Flush();
            Response.End();
            //Process.Start(path);

            //return RedirectToAction("PrintMFTCCardWithdrawlReceipt");

        }

        #endregion


        #region OLD

        [HttpGet]
        public ActionResult PayMFTCCardUser(string MFTCCode = "", string AccessCode = "")
        {
            //Session.Remove("FirstLogin");
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            string agencyName = agentInfo.Name;
            string agencyMFSCode = agentInfo.AccountNo;
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                Common.AgentSession.FormURL = "/Agent/PayAReceiver/PayMFTCCardUser";
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            ViewBag.IDTypes = new SelectList(dbContext.IdentityCardType.ToList(), "Id", "CardType");
            ViewBag.IdIssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            PayMFTCCardUserViewModel vm = new PayMFTCCardUserViewModel();
            vm.PayingAgentName = Common.AgentSession.LoggedUser.PayingAgentStaffName;
            if (!string.IsNullOrEmpty(MFTCCode))
            {
                var MFTCCardDetails = new KiiPayPersonalWalletInformation();
                string MFTCNumber = MFTCCode.Encrypt();

                string[] tokens = MFTCCode.Split('-');
                if (tokens.Length < 2)
                {
                    MFTCCardDetails = payAReceiverServices.GetCardInformationByCardNumber(MFTCCode.Trim());

                }
                else
                {
                    MFTCCardDetails = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTCNumber).FirstOrDefault();
                }


                if (MFTCCardDetails == null)
                {

                    AgentResult agentResult = new AgentResult();
                    agentResult.Message = "Please enter a valid Virtual Account Number";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }

                #region Access Code 
                // Validate the accesscode 
                if (string.IsNullOrEmpty(AccessCode))
                {

                    AgentResult agentResult = new AgentResult();
                    ViewBag.AgentResult = agentResult;

                    ViewBag.MFTCCardNo = MFTCCode;
                    ViewBag.AccessCodeIsEntered = 0;
                    return View(vm);

                }
                else
                {

                    var accessCodeIsValid = dbContext.KiiPayPersonalWalletWithdrawalCode.
                                           Where(x => x.AccessCode == AccessCode &&
                                           x.KiiPayPersonalWalletId == MFTCCardDetails.Id
                                           && x.IsExpired == false).FirstOrDefault() == null ? false : true;
                    if (accessCodeIsValid == false)
                    {
                        AgentResult agentResult = new AgentResult();
                        agentResult.Message = "Please enter a valid access code";
                        agentResult.Status = ResultStatus.Warning;
                        ViewBag.AgentResult = agentResult;

                        ViewBag.MFTCCardNo = MFTCCode;
                        ViewBag.AccessCodeIsEntered = 1;
                        return View(vm);
                    }

                    Common.AgentSession.MFTCCardAccessCode = AccessCode;


                }
                #endregion

                if (MFTCCardDetails.CardUserCountry.ToLower() != agentInfo.CountryCode.ToLower())
                {
                    AgentResult agentResult = new AgentResult();
                    agentResult.Message = "The resgitered card  is not of your country, please direct the customer to their respective country.";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);

                }
                if (MFTCCardDetails.CashLimitType != CardLimitType.NoLimitSet)
                {
                    if (MFTCCardDetails.CashWithdrawalLimit == 0)
                    {

                        AgentResult agentResult = new AgentResult();
                        agentResult.Message = "Sorry! You cannot make an withdrawal ,Your withdrawal limit is 0 ";
                        agentResult.Status = ResultStatus.Warning;
                        ViewBag.AgentResult = agentResult;
                        return View(vm);
                    }
                }


                vm = (from card in dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTCCardDetails.MobileNo).ToList()
                      join CardUserCountry in dbContext.Country on card.CardUserCountry equals CardUserCountry.CountryCode
                      select new PayMFTCCardUserViewModel()
                      {
                          FaxerFirstName = "",
                          FaxerMiddleName = "",
                          FaxerLastName = "",
                          FaxerAddress = "",
                          FaxerCity = "",
                          FaxerCountry = "",
                          FaxerEmail = "",
                          FaxerTelephone = "",
                          FaxerPhoneCode = "",
                          //DateTime = tran.TransactionDate,
                          DateTime = DateTime.Now,
                          MFTCCardId = card.Id,
                          MFTCCardNumber = card.MobileNo.Decrypt(),
                          MFTCCity = card.CardUserCity,
                          MFTCCountry = CardUserCountry.CountryName,
                          MFTCAddress = card.Address1,
                          MFTCFirstName = card.FirstName,
                          MFTCMiddleName = card.MiddleName,
                          MFTCLastName = card.LastName,
                          MFTCCardUserEmail = card.CardUserEmail,
                          MFTCCardPhoneCode = Common.Common.GetCountryPhoneCode(card.CardUserCountry),
                          MFTCTelephone = card.CardUserTel,
                          CardStatus = Enum.GetName(typeof(CardStatus), card.CardStatus),
                          CardStatusEnum = card.CardStatus,
                          WithDrawlLimit = card.CashWithdrawalLimit,
                          LimitTypeEnum = card.CashLimitType,
                          LimitType = card.CashLimitType.ToString(),
                          //AmountOnCard = String.Format("{0:n}", card.CurrentBalance),
                          AmountOnCard = card.CurrentBalance,
                          MFTCCardURL = card.UserPhoto,
                          AgencyMFSCode = agencyMFSCode,
                          NameOfAgency = agencyName,
                          AgentId = agentId,
                          TemporalEmailOrSMS = "NO",
                          MFTCCardCurrency = CommonService.getCurrencyCodeFromCountry(card.CardUserCountry),
                          MFTCCardCurrencySymbol = CommonService.getCurrencySymbol(card.CardUserCountry),
                          PayingAgentName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                          // StatusOfFaxName = Enum.GetName(typeof(FaxingStatus), tran.FaxingStatus)
                      }).FirstOrDefault();
                if (vm != null)
                {
                    var MTSCardReceived = dbContext.UserCardWithdrawl.Where(x => x.KiiPayPersonalWalletInformationId == vm.MFTCCardId).FirstOrDefault();
                    if (MTSCardReceived != null)
                    {
                        //    vm.ReceiverFirstName = "";
                        //    vm.ReceiverMiddleName = "";
                        //    vm.ReceiverLastName = "";
                        //}
                        //else
                        //{
                        //vm.IdentificationTypeId = MTSCardReceived.IdentificationTypeId;
                        //vm.IdCardExpiringDate = MTSCardReceived.IdCardExpiringDate;
                        //vm.IdNumber = MTSCardReceived.IdNumber;
                        //vm.IssuingCountryCode = MTSCardReceived.IssuingCountryCode;

                    }
                }
                else
                {
                    AgentResult agentResult = new AgentResult();
                    agentResult.Message = "MFTC Card Number Doesnot Exist";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    var model = new Models.PayMFTCCardUserViewModel();
                    model.CardStatus = "";
                    return View(model);
                }
                ViewBag.AgentResult = GetMFSStatus(vm);
                return View(vm);
            }

            ViewBag.AgentResult = new AgentResult();
            vm.AgentId = agentId;
            vm.CardStatus = "";
            return View(vm); ;


        }
        private AgentResult GetMFSStatus(PayMFTCCardUserViewModel vm)
        {
            AgentResult agentResult = new AgentResult();
            if (vm == null)
            {
                agentResult.Message = "MFTC Card Number does not exist.Please enter a valid MFTC Card";
                agentResult.Status = ResultStatus.Warning;
                return agentResult;
            }
            else
            {
                if (vm.CardStatusEnum == CardStatus.IsDeleted || vm.CardStatusEnum == CardStatus.InActive || vm.CardStatusEnum == CardStatus.IsRefunded)
                {

                    switch (vm.CardStatusEnum)
                    {
                        case CardStatus.InActive:
                            agentResult.Message = "Virtual Card Deactivated, please contact MoneyFex Support";
                            break;
                        case CardStatus.IsDeleted:
                            agentResult.Message = "Virtual Card Deleted, please contact MoneyFex Support";
                            break;
                        case CardStatus.IsRefunded:
                            agentResult.Message = "Virtual Card Refunded, please contact MoneyFex Support";
                            break;
                        default:
                            break;
                    }
                    agentResult.Status = ResultStatus.Warning;
                    agentResult.Data = vm;
                }

            }
            return agentResult;
        }

        private AgentResult GetPayAReceiverStatus(PayAReceiverKiiPayWalletViewModel vm)
        {
            AgentResult agentResult = new AgentResult();
            if (vm == null)
            {
                agentResult.Message = "KiiPay Wallet Number does not exist.Please enter a valid KiiPay Wallet Card";
                agentResult.Status = ResultStatus.Warning;
                return agentResult;
            }
            else
            {
                if (vm.WalletStatus == CardStatus.IsDeleted || vm.WalletStatus == CardStatus.InActive || vm.WalletStatus == CardStatus.IsRefunded)
                {

                    switch (vm.WalletStatus)
                    {
                        case CardStatus.InActive:
                            ModelState.AddModelError("Invalid", "KiiPay Wallet Deactivated, please contact MoneyFex Support");
                            //agentResult.Message = "KiiPay Wallet Deactivated, please contact MoneyFex Support";
                            break;
                        case CardStatus.IsDeleted:
                            ModelState.AddModelError("Invalid", "KiiPay Wallet Deleted, please contact MoneyFex Support");
                            //agentResult.Message = "KiiPay Wallet Deleted, please contact MoneyFex Support";
                            break;
                        case CardStatus.IsRefunded:
                            ModelState.AddModelError("Invalid", "KiiPay Wallet Refunded, please contact MoneyFex Support");
                            //agentResult.Message = "KiiPay Wallet Refunded, please contact MoneyFex Support";
                            break;
                        default:
                            break;
                    }
                    // agentResult.Status = ResultStatus.Warning;
                    agentResult.Data = vm;
                }

            }
            return agentResult;
        }
        [HttpPost]
        public ActionResult PayMFTCCardUser([Bind(Include = PayMFTCCardUserViewModel.BindProperty)] PayMFTCCardUserViewModel vm)
        {
            AgentServices.PayAReceiverControllerServices payAReceiverServices = new AgentServices.PayAReceiverControllerServices();
            AgentResult agentResult = new AgentResult();
            if (vm.AgentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            if (string.IsNullOrEmpty(vm.MFTCCardNumber))
            {
                agentResult.Message = "Please Find the MFTC Information by using MFTCNumber";
                agentResult.Status = ResultStatus.Warning;
            }

            bool isValidWithdrawlAmount = true;

            ViewBag.IDTypes = new SelectList(dbContext.IdentityCardType.ToList(), "Id", "CardType");
            ViewBag.IdIssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            if (ModelState.IsValid)
            {


                if (validateData(agentResult, vm) == false)
                {
                    return View(vm);
                }


                if (vm.PayingAgentName == null)
                {
                    ModelState.AddModelError("PayingAgentName", "Please Enter the Name of Paying Agent");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (!vm.IsConfirmed)
                {
                    agentResult.Message = "Confirmation for the information is required to either pay or rejection this transaction has been fully verified by yourself";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (Convert.ToDecimal(vm.AmountRequested) != 0)
                {


                    Decimal TotaAmountWithDrawl = 0;
                    var FirstWithdrawl = dbContext.UserCardWithdrawl.Where(x => x.KiiPayPersonalWalletInformationId == vm.MFTCCardId).FirstOrDefault();
                    var LastWithdrawl = dbContext.UserCardWithdrawl.Where(x => x.KiiPayPersonalWalletInformationId == vm.MFTCCardId).ToList().LastOrDefault();



                    DateTime currentDate = DateTime.Now.Date;



                    if (vm.LimitType == "Daily")
                    {
                        if ((LastWithdrawl != null) && LastWithdrawl.TransactionDate.Date == currentDate)
                        {
                            TotaAmountWithDrawl = dbContext.UserCardWithdrawl.Where(x => DbFunctions.TruncateTime(x.TransactionDate) == DbFunctions.TruncateTime(currentDate)).Sum(x => x.TransactionAmount);
                        }
                        if ((TotaAmountWithDrawl + Convert.ToDecimal(vm.AmountRequested)) > Convert.ToDecimal(vm.WithDrawlLimit))
                        {
                            agentResult.Message = "Sorry, You have exceeded your withdrawl limit";
                            agentResult.Status = ResultStatus.Warning;
                            ViewBag.AgentResult = agentResult;
                            return View(vm);
                        }



                    }
                    //else if (vm.LimitTypeEnum == AutoPaymentFrequency.NoLimitSet)
                    //{

                    //}
                    else if (vm.LimitTypeEnum == CardLimitType.NoLimitSet)
                    {

                    }
                    else
                    {

                        DateTime StartedTransactionDate = new System.DateTime();

                        if (FirstWithdrawl != null)
                        {
                            // var DateDifference = System.DateTime.Now  - FirstWithdrawl.TransactionDate ;
                            var DateDifference = currentDate - FirstWithdrawl.TransactionDate;

                        }
                        int gannuParneDays = getGannuParneDays(vm.LimitType, currentDate);
                        StartedTransactionDate = currentDate.AddDays(-(gannuParneDays));

                        ///TODO: put this on services
                        //TotaAmountWithDrawl = dbContext.UserCardWithdrawl.Where(x => x.TransactionDate >= StartedTransactionDate).Sum(x => x.TransactionAmount);
                        TotaAmountWithDrawl = payAReceiverServices.getTotalAmountWithDrawl(StartedTransactionDate, vm.MFTCCardId);
                        if (TotaAmountWithDrawl + Convert.ToDecimal(vm.AmountRequested) > Convert.ToDecimal(vm.WithDrawlLimit))
                        {
                            agentResult.Message = "Sorry, You have exceeded your withdrawl limit";
                            agentResult.Status = ResultStatus.Warning;
                            ViewBag.AgentResult = agentResult;

                            return View(vm);
                        }


                    }




                }
                AgentServices.PaymentReceiptServices receiptServices = new AgentServices.PaymentReceiptServices();
                string ReceiptNumber = receiptServices.GetNewMFTCCardPaymentReceiptNumber();

                KiiPayPersonalWalletWithdrawalFromAgent transaction = new KiiPayPersonalWalletWithdrawalFromAgent()
                {
                    AgentInformationId = vm.AgentId,
                    KiiPayPersonalWalletInformationId = vm.MFTCCardId,
                    TransactionAmount = Convert.ToDecimal(vm.AmountRequested),
                    IdCardExpiringDate = vm.IdCardExpiringDate ?? new DateTime(),
                    IdNumber = vm.IdNumber,
                    IssuingCountryCode = vm.IssuingCountryCode,
                    IdentificationType = vm.IdentificationTypeId,
                    TransactionDate = DateTime.Now,
                    TransactionType = 1,
                    PayingAgentName = vm.PayingAgentName,
                    ReceiptNumber = ReceiptNumber,
                    PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,

                };
                dbContext.UserCardWithdrawl.Add(transaction);
                int result = dbContext.SaveChanges();



                #region Expired the Access Code 
                var CardUserWithdrawalCodedata = dbContext.KiiPayPersonalWalletWithdrawalCode.Where(x => x.AccessCode == Common.AgentSession.MFTCCardAccessCode).FirstOrDefault();
                CardUserWithdrawalCodedata.IsExpired = true;
                dbContext.Entry(CardUserWithdrawalCodedata).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                #endregion


                // Send Email for MoneyFax Top-Up Card Usage - Alert

                MailCommon mail = new MailCommon();
                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                string FaxerName = vm.FaxerFirstName + " " + vm.FaxerMiddleName + " " + vm.FaxerLastName;
                string CardUserName = vm.MFTCFirstName + " " + vm.MFTCMiddleName + " " + vm.MFTCLastName;
                string CardUserEmail = vm.MFTCCardUserEmail;
                string body = "";
                string BalanceOnCard = (vm.AmountOnCard - vm.AmountRequested).ToString();
                string PayingAgentCity = Common.AgentSession.AgentInformation.City;
                string TopUpMoneyfaxCard = baseUrl + "/StartFaxingMoney/Index?mFTCCardInformationId=" + vm.MFTCCardId;
                string StopAlert = "";
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFTCCardUsageEmail?FaxerName=" + FaxerName +
                    "&MFTCCardNumber=" + vm.MFTCCardNumber + "&CardUserName=" + CardUserName + "&CardUserCountry="
                    + vm.MFTCCountry + "&CardUserCity=" + vm.MFTCCity
                    + "&CityOfPayingAgentOrRegisteredMerchant=" + PayingAgentCity + "&BalanceOnCard=" + BalanceOnCard +
                    "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard + "&StopAlert=" + StopAlert);

                mail.SendMail(vm.FaxerEmail, "MoneyFex Virtual Account Usage - Alert", body);



                //mail.SendMail("anankarki97@gmail.com", "MoneyFax Top-Up Card Usage - Alert", body);

                // End 
                if (result == 1)
                {
                    var MFTCCardDetails = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == vm.MFTCCardId).FirstOrDefault();

                    if (Convert.ToDecimal(vm.AmountRequested) == MFTCCardDetails.CurrentBalance)
                    {
                        MFTCCardDetails.CurrentBalance = MFTCCardDetails.CurrentBalance - vm.AmountRequested;
                        if (MFTCCardDetails.CurrentBalance >= 0)
                        {
                            dbContext.Entry(MFTCCardDetails).State = EntityState.Modified;
                            dbContext.SaveChanges();
                        }
                        var CardTransaction = dbContext.FaxingCardTransaction.Where(x => x.KiiPayPersonalWalletInformationId == vm.MFTCCardId).FirstOrDefault();
                        CardTransaction.FaxingStatus = FaxingStatus.Received;
                        dbContext.Entry(CardTransaction).State = EntityState.Modified;
                        dbContext.SaveChanges();

                        agentResult.Message = "Payment Completed Successfully";
                        agentResult.Status = ResultStatus.OK;
                        agentResult.Data = transaction.Id;
                        ViewBag.AgentResult = agentResult;
                        ModelState.Clear();
                        //var model = new PayMFTCCardUserViewModel();
                        //model.StatusOfFaxName = "";
                        //return View(model);

                    }
                    else
                    {
                        MFTCCardDetails.CurrentBalance = MFTCCardDetails.CurrentBalance - vm.AmountRequested;
                        dbContext.Entry(MFTCCardDetails).State = EntityState.Modified;
                        dbContext.SaveChanges();

                        agentResult.Message = "Payment Completed Sucessfully";
                        agentResult.Status = ResultStatus.OK;
                        agentResult.Data = transaction.Id;
                        ViewBag.AgentResult = agentResult;

                        ModelState.Clear();
                        //var model = new PayMFTCCardUserViewModel();
                        //model.StatusOfFaxName = "";
                        //return View(model);

                    }
                    var AutoTopEmable = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == vm.MFTCCardId).FirstOrDefault().AutoTopUp;
                    if (MFTCCardDetails.CurrentBalance == 0)
                    {

                        string EmailToSetAutoTopup_body = "";
                        var FaxerCountry = dbContext.Country.Where(x => x.CountryName.ToLower() == vm.FaxerCountry.ToLower()).FirstOrDefault();
                        var CardUserCurrency = dbContext.Country.Where(x => x.CountryCode == MFTCCardDetails.CardUserCountry).FirstOrDefault();
                        string SetAutoTopUp = baseUrl + "/FaxerAutoPayments/MoneyFaxCardAutoTopUp?mftcCardInformationId=" + MFTCCardDetails.Id;
                        decimal exchangeRate = 0;

                        // Calculate exchange Rate 
                        var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == FaxerCountry.CountryCode && x.CountryCode2 == CardUserCurrency.CountryCode).FirstOrDefault();
                        if (exchangeRateObj == null)
                        {
                            var exchangeRateobj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == CardUserCurrency.CountryCode && x.CountryCode2 == FaxerCountry.CountryCode).FirstOrDefault();
                            if (exchangeRateobj2 != null)
                            {
                                exchangeRateObj = exchangeRateobj2;
                            }

                        }
                        else
                        {
                            exchangeRate = exchangeRateObj.CountryRate1;
                        }
                        if (FaxerCountry.CountryCode == CardUserCurrency.CountryCode)
                        {

                            exchangeRate = 1m;

                        }
                        EmailToSetAutoTopup_body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/BalanceonMFTCCardZeroEmail?FaxerName=" + FaxerName +
                            "&MFTCCardNumber=" + MFTCCardDetails.MobileNo.Decrypt() +
                            "&CardUserName" + CardUserName + "&SenderCountryCode=" + FaxerCountry.CountryCode + "&SenderCurrency=" + FaxerCountry.Currency
                            + "&CardUserCountryCode=" + MFTCCardDetails.CardUserCountry +
                            "&CardUserCurrency=" + CardUserCurrency.Currency + "&TopUpMoneyfaxCard=" + TopUpMoneyfaxCard +
                            "&SetAutoTopUp=" + SetAutoTopUp + "&ExchangeRate=" + exchangeRate);
                        //  mail.SendMail(MFTCCardDetails.FaxerInformation.Email, "Balance on MoneyFex Virtuaal Account " + MFTCCardDetails.MobileNo.Decrypt() + "is Zero (0)", EmailToSetAutoTopup_body);
                        //mail.SendMail(MFTCCardDetails.CardUserEmail, "Balance on MoneyFex Top-Up Card " + MFTCCardDetails.MFTCCardNumber.Decrypt() + "is Zero (0)", EmailToSetAutoTopup_body);
                    }
                    if ((AutoTopEmable == true) && MFTCCardDetails.CurrentBalance == 0)
                    {


                        AgentServices.MFTCAutoTopUpServices AutoTopUpServices = new AgentServices.MFTCAutoTopUpServices();

                        var TopUp = AutoTopUpServices.AutoTopUp(vm.MFTCCardId);

                    }

                    var model = new PayMFTCCardUserViewModel();
                    model.StatusOfFaxName = "";
                    return View(model);

                }
                else
                {
                    agentResult.Message = "Payment Failed due to technical error";
                    agentResult.Status = ResultStatus.Error;
                }
            }
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }
        private int getGannuParneDays(string LimitType, DateTime currentDate)
        {
            var Day = 0;
            var gannuParneDays = 0;
            if (gannuParneDays == 0 && LimitType.ToLower() != "daily")
            {
                switch (LimitType.ToLower())
                {
                    case "weekly":
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

                    case "monthly":
                        Day = currentDate.Day;
                        gannuParneDays = Day;

                        break;
                    default:
                        break;
                }
            }
            return gannuParneDays;
        }
        private bool validateData(AgentResult agentResult, PayMFTCCardUserViewModel vm)
        {

            if (vm.CardStatus == "InActive")
            {
                agentResult.Message = "This MFTC Card has been deactivated ";
                agentResult.Status = ResultStatus.Warning;
                ViewBag.AgentResult = agentResult;
                return false;
            }
            if (vm.CardStatus == "IsDeleted")
            {

                agentResult.Message = "This MFTC Card has been deleted";
                agentResult.Status = ResultStatus.Warning;
                ViewBag.AgentResult = agentResult;
                return false;
            }
            if (Convert.ToDecimal(vm.AmountRequested) <= 0)
            {

                agentResult.Message = "Please Enter Amount Above 0";
                agentResult.Status = ResultStatus.Warning;
                ViewBag.AgentResult = agentResult;
                return false;

            }
            if (Convert.ToDecimal(vm.AmountRequested) > Convert.ToDecimal(vm.AmountOnCard))
            {

                agentResult.Message = "You Dont Have Enough Balance.";
                agentResult.Status = ResultStatus.Warning;
                ViewBag.AgentResult = agentResult;
                return false;

            }

            if (vm.IdCardExpiringDate < DateTime.Now)
            {
                agentResult.Message = "Receiver's Identity is expired.";
                agentResult.Status = ResultStatus.Warning;
                ViewBag.AgentResult = agentResult;
                return false;
            }
            return true;
        }

        private bool validatePayAReceiverData(AgentResult agentResult, PayAReceiverKiiPayWalletViewModel vm)
        {

            if (vm.WalletStatusName == "InActive")
            {
                agentResult.Message = "This KiiPayWallet  has been deactivated ";
                agentResult.Status = ResultStatus.Warning;
                ViewBag.AgentResult = agentResult;
                return false;
            }
            if (vm.WalletStatusName == "IsDeleted")
            {

                agentResult.Message = "This KiiPayWallet Card has been deleted";
                agentResult.Status = ResultStatus.Warning;
                ViewBag.AgentResult = agentResult;
                return false;
            }

            return true;
        }
        //Amount withdrawl Limit 
        [HttpGet]
        public ActionResult PayNonMFTCCardUser(string MFCN = "")
        {
            //Session.Remove("FirstLogin");
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            string agencyName = agentInfo.Name;
            string agencyMFSCode = agentInfo.AccountNo;
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                Common.AgentSession.FormURL = "/Agent/PayAReceiver/PayNonMFTCCardUser";
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            Admin.Services.CommonServices getCountry = new Admin.Services.CommonServices();
            var countries = getCountry.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(dbContext.IdentityCardType.ToList(), "Id", "CardType");
            ViewBag.IdIssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            PayNonMFTCCardUserViewModel vm = new PayNonMFTCCardUserViewModel();
            vm.PayingAgentName = Common.AgentSession.LoggedUser.PayingAgentStaffName;
            #region  Merchant Non Card Transfer Withdrawal

            // 34 is Code for cash to cash transfer made by Business Merchant
            if (MFCN.Contains("-34"))
            {

                AgentServices.MerchantNonCardWithdrawalServices merchantNonCardWithdrawalServices = new AgentServices.MerchantNonCardWithdrawalServices();
                vm = merchantNonCardWithdrawalServices.GetTransactionDetails(MFCN);

                if (vm != null)
                {
                    var TransactionCountry = vm.ReceiverCountryCode;
                    if ((TransactionCountry != null) && TransactionCountry.ToLower() != agentInfo.CountryCode.ToLower())
                    {
                        PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                        AgentResult agentResult = new AgentResult();
                        agentResult.Message = "This transaction was not sent to your country, please direct the customer to their respective country.";
                        agentResult.Status = ResultStatus.Warning;
                        ViewBag.AgentResult = agentResult;
                        model.StatusOfFaxName = "";
                        return View(model);
                    }

                    ViewBag.AgentResult = GetMFCNStatus(vm);
                    return View(vm);

                }
                else
                {
                    PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                    AgentResult agentResult = new AgentResult();
                    agentResult.Message = "Please enter a valid MFCN Number";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    model.StatusOfFaxName = "";
                    return View(model);

                }

            }

            #endregion

            #region card User Non Card Transfer Withdrawal
            // 21 is Code for cash to cash transfer made by card user
            else if (MFCN.Contains("-21"))
            {
                AgentServices.CardUserNonCardWithdrawalServices cardUserNonCardWithdrawalServices = new AgentServices.CardUserNonCardWithdrawalServices();

                vm = cardUserNonCardWithdrawalServices.GetTransactionDetails(MFCN);


                if (vm != null)
                {
                    var TransactionCountry = vm.ReceiverCountryCode;
                    if ((TransactionCountry != null) && TransactionCountry.ToLower() != agentInfo.CountryCode.ToLower())
                    {
                        PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                        AgentResult agentResult = new AgentResult();
                        agentResult.Message = "This transaction was not sent to your country, please direct the customer to their respective country.";
                        agentResult.Status = ResultStatus.Warning;
                        ViewBag.AgentResult = agentResult;
                        model.StatusOfFaxName = "";
                        return View(model);
                    }

                    ViewBag.AgentResult = GetMFCNStatus(vm);
                    return View(vm);

                }
                else
                {
                    PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                    AgentResult agentResult = new AgentResult();
                    agentResult.Message = "Please enter a valid MFCN Number";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    model.StatusOfFaxName = "";
                    return View(model);

                }

            }


            #endregion
            #region Sender Non Card Transfer withdrawal
            else
            {
                if (!string.IsNullOrEmpty(MFCN))
                {
                    var nonCardReceived = dbContext.ReceiverNonCardWithdrawl.Where(x => x.MFCN == MFCN).FirstOrDefault();

                    var TransactionCountry = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();

                    if ((TransactionCountry != null) && TransactionCountry.NonCardReciever.Country.ToLower() != agentInfo.CountryCode.ToLower())
                    {
                        PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                        AgentResult agentResult = new AgentResult();
                        agentResult.Message = "This transaction was not sent to your country, please direct the customer to their respective country.";
                        agentResult.Status = ResultStatus.Warning;
                        ViewBag.AgentResult = agentResult;
                        model.StatusOfFaxName = "";
                        return View(model);
                    }


                    vm = (from tran in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).ToList()
                          join receiver in dbContext.ReceiversDetails on tran.NonCardRecieverId equals receiver.Id
                          join faxer in dbContext.FaxerInformation on receiver.FaxerID equals faxer.Id
                          select new PayNonMFTCCardUserViewModel()
                          {
                              FaxedAmount = String.Format("{0:n}", tran.FaxingAmount),
                              MFCN = tran.MFCN,
                              FaxerAddress = faxer.Address1,
                              FaxerCity = faxer.City,
                              FaxerCountryCode = faxer.Country,
                              FaxerCountry = Common.Common.GetCountryName(faxer.Country),
                              FaxerEmail = faxer.Email,
                              FaxerFirstName = faxer.FirstName,
                              FaxerLastName = faxer.LastName,
                              FaxerMiddleName = faxer.MiddleName,
                              FaxerTelephone = faxer.PhoneNumber,
                              FaxerPhoneCode = Common.Common.GetCountryPhoneCode(faxer.Country),
                              DateTime = tran.TransactionDate,
                              ReceiverId = tran.NonCardRecieverId,
                              ReceiverCity = receiver.City,
                              ReceiverFirstName = receiver.FirstName,
                              ReceiverMiddleName = receiver.MiddleName,
                              ReceiverEmail = receiver.EmailAddress,
                              ReceiverLastName = receiver.LastName,
                              ReceiverTelephone = receiver.PhoneNumber,
                              ReceiverCountryCode = receiver.Country,
                              ReceiverCountry = Common.Common.GetCountryName(receiver.Country),
                              ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(receiver.Country),
                              NameOfAgency = agencyName,
                              AgencyMFSCode = agencyMFSCode,
                              AgentId = agentId,
                              StatusOfFax = tran.FaxingStatus,
                              StatusOfFaxName = Enum.GetName(typeof(FaxingStatus), tran.FaxingStatus),
                              RefundRequest = tran.FaxingStatus == FaxingStatus.Refund ? "YES" : "NO",
                              FaxerCurrency = CommonService.getCurrencyCodeFromCountry(faxer.Country),
                              FaxerCurrencySymbol = CommonService.getCurrencySymbol(faxer.Country),
                              ReceiverCurrency = CommonService.getCurrencyCodeFromCountry(receiver.Country),
                              ReceiverCurrencySymbol = CommonService.getCurrencySymbol(receiver.Country),
                              ReceivingAmount = tran.ReceivingAmount.ToString(),
                              PayingAgentName = Common.AgentSession.LoggedUser.PayingAgentStaffName
                          }).FirstOrDefault();
                    if (vm != null)
                    {
                        if (nonCardReceived != null)
                        {
                            //    vm.ReceiverFirstName = "";
                            //    vm.ReceiverMiddleName = "";
                            //    vm.ReceiverLastName = "";
                            //}
                            //else
                            //{
                            vm.IdentificationTypeId = nonCardReceived.IdentificationTypeId;
                            vm.IdCardExpiringDate = nonCardReceived.IdCardExpiringDate;
                            vm.IdNumber = nonCardReceived.IdNumber;
                            vm.IsConfirmed = true;
                            vm.IssuingCountryCode = nonCardReceived.IssuingCountryCode;
                            vm.PayingAgentName = nonCardReceived.PayingAgentName;
                        }
                    }
                    else
                    {
                        PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                        AgentResult agentResult = new AgentResult();
                        agentResult.Message = "MFCN Number Does not Exist";
                        agentResult.Status = ResultStatus.Warning;
                        ViewBag.AgentResult = agentResult;
                        model.StatusOfFaxName = "";
                        return View(model);
                    }
                    ViewBag.AgentResult = GetMFCNStatus(vm);
                    return View(vm);
                }

            }
            #endregion
            ViewBag.AgentResult = new AgentResult();
            vm.AgentId = agentId;
            vm.StatusOfFaxName = "";
            return View(vm);
        }

        private AgentResult GetMFCNStatus(PayNonMFTCCardUserViewModel vm)
        {
            AgentResult agentResult = new AgentResult();
            if (vm == null)
            {
                agentResult.Message = "MFCN does not exist.Please enter a valid MFCN";
                agentResult.Status = ResultStatus.Warning;
                return agentResult;
            }
            else
            {

                if (vm.StatusOfFax == FaxingStatus.Received)
                {

                    agentResult.Message = "Sorry! you cannot be withdrawal make , this transaction has been Received.";
                    agentResult.Status = ResultStatus.Warning;
                    agentResult.Data = vm;
                }
                else if (vm.StatusOfFax == FaxingStatus.Refund)
                {

                    agentResult.Message = "Sorry! you cannot be withdrawal make  ,this transaction has been Refunded";
                    agentResult.Status = ResultStatus.Warning;
                }
                else if (vm.StatusOfFax == FaxingStatus.Cancel)
                {
                    agentResult.Message = "Sorry! you cannot be withdrawal make  ,this transaction has been Cancelled";
                    agentResult.Status = ResultStatus.Warning;
                }
                else if (vm.StatusOfFax == FaxingStatus.Hold)
                {

                    agentResult.Message = "Sorry! This Transaction can not be completed because it has been held by Customer Service";
                    agentResult.Status = ResultStatus.Warning;
                }
            }
            return agentResult;
        }

        [HttpPost]
        public ActionResult PayNonMFTCCardUser([Bind(Include = PayNonMFTCCardUserViewModel.BindProperty)]PayNonMFTCCardUserViewModel vm)
        {
            //Session.Remove("FirstLogin");
            AgentResult agentResult = new AgentResult();
            if (vm.AgentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            if (string.IsNullOrEmpty(vm.MFCN))
            {
                agentResult.Message = "Please Find the Receiver by using MFCN";
                agentResult.Status = ResultStatus.Warning;
            }
            Admin.Services.CommonServices getCountry = new Admin.Services.CommonServices();
            var countries = getCountry.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(dbContext.IdentityCardType.ToList(), "Id", "CardType");
            ViewBag.IdIssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");
            if (ModelState.IsValid)
            {


                if (vm.StatusOfFax == FaxingStatus.Received)
                {
                    agentResult.Message = "Sorry! you cannot be withdrawal make , this transaction has been Received.";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                else if (vm.StatusOfFax == FaxingStatus.Refund)
                {
                    agentResult.Message = "Sorry! you cannot be withdrawal make  ,this transaction has been Refunded";
                    agentResult.Status = ResultStatus.Warning;
                }
                else if (vm.StatusOfFax == FaxingStatus.Cancel)
                {
                    agentResult.Message = "Sorry! you cannot be withdrawal make  ,this transaction has been Cancelled";
                    agentResult.Status = ResultStatus.Warning;
                }
                else if (vm.StatusOfFax == FaxingStatus.Hold)
                {
                    agentResult.Message = "Sorry! This Transaction can not be completed because it has been held by Customer Service";
                    agentResult.Status = ResultStatus.Warning;
                }
                //if (!IsValidReceiver(vm))
                //{
                //    agentResult.Message = "Invalid Receiver details.";
                //    agentResult.Status = ResultStatus.Warning;
                //    ViewBag.AgentResult = agentResult;
                //    return View(vm);
                //}

                if (vm.IdCardExpiringDate < DateTime.Now)
                {
                    agentResult.Message = "Receiver's Identity is expired.";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (vm.PayingAgentName == null)
                {
                    ModelState.AddModelError("PayingAgentName", "Please Enter Paying Agent Name");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (!vm.IsConfirmed)
                {
                    agentResult.Message = "Confirmation for the information is required to either pay or rejection this transaction has been fully verified by yourself";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                AgentServices.PaymentReceiptServices receiptServices = new AgentServices.PaymentReceiptServices();
                string ReceiptNumber = receiptServices.GetNewNonMFTCCardPaymentReceipt();

                // Receiving Amount has been declared to use in email 
                decimal ReceivingAmountForEmail = 0;
                #region Card User Non Card Transfer withdrawal


                if (vm.WithdrawalPaymentOf == WithdrawalPaymentOf.CardUser)
                {
                    AgentServices.CardUserNonCardWithdrawalServices cardUserNonCardWithdrawalServices = new AgentServices.CardUserNonCardWithdrawalServices();

                    CardUserNonCardWithdrawal obj = new CardUserNonCardWithdrawal()
                    {
                        AgentId = vm.AgentId,
                        IdCardExpiringDate = vm.IdCardExpiringDate ?? new DateTime(),
                        IdentificationTypeId = vm.IdentificationTypeId,
                        IdNumber = vm.IdNumber,
                        IssuingCountryCode = vm.IssuingCountryCode,
                        MFCN = vm.MFCN,
                        ReceiverId = vm.ReceiverId,
                        TransactionAmount = decimal.Parse(vm.FaxedAmount),
                        ReceivingAmount = decimal.Parse(vm.ReceivingAmount),
                        TransactionDate = DateTime.Now,
                        TransactionType = 1,
                        PayingAgentName = vm.PayingAgentName,
                        ReceiptNumber = ReceiptNumber,
                        PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                    };


                    var result = cardUserNonCardWithdrawalServices.MakeAnwithdrawal(obj);

                    if (result != null)
                    {

                        var transaction = cardUserNonCardWithdrawalServices.UpdateCardUserNonCardTransaction(obj.MFCN);


                        ReceivingAmountForEmail = decimal.Parse(vm.ReceivingAmount);
                        //MailCommon mail = new MailCommon();

                        //var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                        //string ReceiverCuurency = Common.Common.GetCountryCurrency(vm.ReceiverCountryCode);
                        //string AgentCity = Common.AgentSession.AgentInformation.City;
                        //string AgentCountry = Common.Common.GetCountryName(Common.AgentSession.AgentInformation.CountryCode);
                        //string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationofMoneyReceived?FaxerName=" + vm.FaxerFirstName + " " +
                        //    vm.FaxerMiddleName + " " + vm.FaxerLastName + "&ReceiverName=" + vm.ReceiverFirstName + " " + vm.ReceiverMiddleName + " " +
                        //    vm.ReceiverLastName + "&ReceiverCurrency=" + ReceiverCuurency + " " + transaction.ReceivingAmount
                        //    + "&AgentCity=" + AgentCity + "&AgentCountry=" + AgentCountry);

                        //mail.SendMail(vm.FaxerEmail, "Confirmation of Money Received", body);
                        //agentResult.Message = "Payment Completed Successfully";
                        //agentResult.Status = ResultStatus.OK;
                        //agentResult.Data = vm.MFCN;
                        ////PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                        //ViewBag.AgentResult = agentResult;
                        //ModelState.Clear();
                        //Models.PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                        //model.StatusOfFaxName = "";
                        //return View(model);

                    }
                }
                #endregion


                #region Merchant Non Card Transfer Withdrawal
                else if (vm.WithdrawalPaymentOf == WithdrawalPaymentOf.Merchant)
                {
                    AgentServices.MerchantNonCardWithdrawalServices merchantNonCardWithdrawalServices = new AgentServices.MerchantNonCardWithdrawalServices();
                    MerchantNonCardWithdrawal obj = new MerchantNonCardWithdrawal()
                    {
                        AgentId = vm.AgentId,
                        IdCardExpiringDate = vm.IdCardExpiringDate ?? new DateTime(),
                        IdentificationTypeId = vm.IdentificationTypeId,
                        IdNumber = vm.IdNumber,
                        IssuingCountryCode = vm.IssuingCountryCode,
                        MFCN = vm.MFCN,
                        ReceiverId = vm.ReceiverId,
                        TransactionAmount = decimal.Parse(vm.FaxedAmount),
                        ReceivingAmount = decimal.Parse(vm.ReceivingAmount),
                        TransactionDate = DateTime.Now,
                        TransactionType = 1,
                        PayingAgentName = vm.PayingAgentName,
                        ReceiptNumber = ReceiptNumber,
                        PyingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                    };
                    var result = merchantNonCardWithdrawalServices.MakeAnWithdrawal(obj);

                    if (result != null)
                    {

                        var TransactionStatusChanged = merchantNonCardWithdrawalServices.UpdateMerchantNonCardTransaction(obj.MFCN);
                        ReceivingAmountForEmail = TransactionStatusChanged.ReceivingAmount;

                    }

                }
                #endregion

                #region Sender Non Card Transfer Withdrawal
                else
                {
                    ReceiverNonCardWithdrawl obj = new ReceiverNonCardWithdrawl()
                    {
                        AgentId = vm.AgentId,
                        IdCardExpiringDate = vm.IdCardExpiringDate ?? new DateTime(),
                        IdentificationTypeId = vm.IdentificationTypeId,
                        IdNumber = vm.IdNumber,
                        IssuingCountryCode = vm.IssuingCountryCode,
                        MFCN = vm.MFCN,
                        ReceiverId = vm.ReceiverId,
                        TransactionAmount = decimal.Parse(vm.FaxedAmount),
                        ReceivingAmount = decimal.Parse(vm.ReceivingAmount),
                        TransactionDate = DateTime.Now,
                        TransactionType = 1,
                        PayingAgentName = vm.PayingAgentName,
                        ReceiptNumber = ReceiptNumber,
                        PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,

                    };
                    dbContext.ReceiverNonCardWithdrawl.Add(obj);
                    int result = dbContext.SaveChanges();


                    if (result > 0)
                    {
                        var nonCardTransaction = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == vm.MFCN).FirstOrDefault();
                        nonCardTransaction.FaxingStatus = FaxingStatus.Received;
                        nonCardTransaction.StatusChangedDate = DateTime.Now;
                        dbContext.Entry(nonCardTransaction).State = EntityState.Modified;
                        dbContext.SaveChanges();
                        ReceivingAmountForEmail = nonCardTransaction.ReceivingAmount;

                        // Send Email to Faxer money has been Received 
                        //MailCommon mail = new MailCommon();

                        //var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                        //string ReceiverCuurency = Common.Common.GetCountryCurrency(vm.ReceiverCountryCode);
                        //string AgentCity = Common.AgentSession.AgentInformation.City;
                        //string AgentCountry = Common.Common.GetCountryName(Common.AgentSession.AgentInformation.CountryCode);
                        //string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationofMoneyReceived?FaxerName=" + vm.FaxerFirstName + " " +
                        //    vm.FaxerMiddleName + " " + vm.FaxerLastName + "&ReceiverName=" + vm.ReceiverFirstName + " " + vm.ReceiverMiddleName + " " +
                        //    vm.ReceiverLastName + "&ReceiverCurrency=" + ReceiverCuurency + " " + nonCardTransaction.ReceivingAmount
                        //    + "&AgentCity=" + AgentCity + "&AgentCountry=" + AgentCountry);

                        //mail.SendMail(vm.FaxerEmail, "Confirmation of Money Received", body);
                        //agentResult.Message = "Payment Completed Successfully";
                        //agentResult.Status = ResultStatus.OK;
                        //agentResult.Data = vm.MFCN;
                        ////PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                        //ViewBag.AgentResult = agentResult;
                        //ModelState.Clear();
                        //Models.PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                        //model.StatusOfFaxName = "";
                        //return View(model);
                    }
                    else
                    {
                        agentResult.Message = "Payment Failed due to technical error";
                        agentResult.Status = ResultStatus.Error;
                    }
                }
                #endregion

                // Email to faxer there transfer has been received by receiver 

                MailCommon mail = new MailCommon();

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);

                string ReceiverCuurency = Common.Common.GetCountryCurrency(vm.ReceiverCountryCode);
                string AgentCity = Common.AgentSession.AgentInformation.City;
                string AgentCountry = Common.Common.GetCountryName(Common.AgentSession.AgentInformation.CountryCode);
                string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationofMoneyReceived?FaxerName=" + vm.FaxerFirstName + " " +
                    vm.FaxerMiddleName + " " + vm.FaxerLastName + "&ReceiverName=" + vm.ReceiverFirstName + " " + vm.ReceiverMiddleName + " " +
                    vm.ReceiverLastName + "&ReceiverCurrency=" + ReceiverCuurency + " " + ReceivingAmountForEmail
                    + "&AgentCity=" + AgentCity + "&AgentCountry=" + AgentCountry);

                mail.SendMail(vm.FaxerEmail, "Confirmation of Money Received", body);
                agentResult.Message = "Payment Completed Successfully";
                agentResult.Status = ResultStatus.OK;
                agentResult.Data = vm.MFCN;

                //PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                ViewBag.AgentResult = agentResult;
                ViewBag.WithdrawalOF = (int)vm.WithdrawalPaymentOf;
                ModelState.Clear();
                Models.PayNonMFTCCardUserViewModel model = new PayNonMFTCCardUserViewModel();
                model.StatusOfFaxName = "";
                return View(model);
            }
            else
            {
                ViewBag.AgentResult = agentResult;
                return View(vm);
            }

        }

        private bool IsValidReceiver(PayNonMFTCCardUserViewModel vm)
        {
            bool isValid = false;
            vm.ReceiverMiddleName = vm.ReceiverMiddleName == null ? "" : vm.ReceiverMiddleName;
            var receiverDetails = dbContext.ReceiversDetails.Where(x => x.Id == vm.ReceiverId).FirstOrDefault();
            if (receiverDetails != null)
            {
                string fullName = "";
                string inputFullName = "";
                if (vm.ReceiverMiddleName != "")
                {
                    fullName = receiverDetails.FirstName.Trim().ToLower() + " " + receiverDetails.MiddleName.Trim().ToLower() + " " + receiverDetails.LastName.Trim().ToLower();
                    inputFullName = vm.ReceiverFirstName.Trim().ToLower() + " " + vm.ReceiverMiddleName.Trim().ToLower() + " " + vm.ReceiverLastName.Trim().ToLower();
                }
                else
                {
                    fullName = receiverDetails.FirstName.Trim().ToLower() + " " + receiverDetails.LastName.Trim().ToLower();
                    inputFullName = vm.ReceiverFirstName.Trim().ToLower() + " " + vm.ReceiverLastName.Trim().ToLower();

                }
                if (fullName == inputFullName)
                {
                    isValid = true;
                }
            }
            return isValid;
        }
        [HttpGet]
        public ActionResult PayBusinessCardUser(string MFBCNumber = "", string AccessCode = "")
        {
            //Session.Remove("FirstLogin");
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            string agencyName = agentInfo.Name;
            string agencyMFSCode = agentInfo.AccountNo;
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                Common.AgentSession.FormURL = "/Agent/PayAReceiver/PayBusinessCardUser";
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            Admin.Services.CommonServices getCountry = new Admin.Services.CommonServices();
            var countries = getCountry.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(dbContext.IdentityCardType.ToList(), "Id", "CardType");
            ViewBag.IdIssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");


            AgentServices.MFBCPaymentServices mFBCPaymentServices = new AgentServices.MFBCPaymentServices();

            Models.PayMFBCCardUserViewModel vm = new PayMFBCCardUserViewModel();

            vm.PayingAgentName = Common.AgentSession.LoggedUser.PayingAgentStaffName;
            if (!string.IsNullOrEmpty(MFBCNumber))
            {


                var result = mFBCPaymentServices.GetPayMFBCCardUserDetails(MFBCNumber);


                if (result == null)
                {
                    AgentResult agentResult = new AgentResult();
                    agentResult.Message = "MFBC  Card Number Does not Exist";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);

                }

                #region Access Code 
                // Validate the accesscode 
                if (string.IsNullOrEmpty(AccessCode))
                {

                    AgentResult agentResult = new AgentResult();
                    ViewBag.AgentResult = agentResult;

                    ViewBag.MFBCCardNo = MFBCNumber;
                    ViewBag.AccessCodeIsEntered = 0;
                    return View(vm);

                }
                else
                {

                    var accessCodeIsValid = dbContext.KiiPayBusinessWalletWithdrawalCode.
                                           Where(x => x.AccessCode == AccessCode &&
                                           x.KiiPayBusinessInformationId == result.KiiPayBusinessInformationId
                                           && x.IsExpired == false).FirstOrDefault() == null ? false : true;
                    if (accessCodeIsValid == false)
                    {
                        AgentResult agentResult = new AgentResult();
                        agentResult.Message = "Please enter a valid access code";
                        agentResult.Status = ResultStatus.Warning;
                        ViewBag.AgentResult = agentResult;

                        ViewBag.MFBCCardNo = MFBCNumber;
                        ViewBag.AccessCodeIsEntered = 1;
                        return View(vm);
                    }

                    Common.AgentSession.BusinessCardAccessCode = AccessCode;


                }
                #endregion

                if (result.CountryCode != agentInfo.CountryCode)
                {
                    AgentResult agentResult = new AgentResult();
                    agentResult.Message = "The registered card  is not of your country, please direct the customer to their respective country.";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                result.AgencyMFSCode = agencyMFSCode;
                result.AgentId = agentId;
                result.NameOfAgency = agencyName;
                ViewBag.AgentResult = GetMFBCCardStatus(result);
                return View(result);


            }

            ViewBag.AgentResult = new AgentResult();
            vm.AgentId = agentId;
            vm.MFBCCardStatus = "";
            return View(vm);

        }
        private AgentResult GetMFBCCardStatus(Models.PayMFBCCardUserViewModel vm)
        {
            AgentResult agentResult = new AgentResult();
            if (vm == null)
            {
                agentResult.Message = "MFBC Card Number does not exist.Please enter a valid MFBC Card Number";
                agentResult.Status = ResultStatus.Warning;
                return agentResult;
            }
            else
            {
                if (vm.MFBCCardStatusEnum == CardStatus.InActive || vm.MFBCCardStatusEnum == CardStatus.IsDeleted || vm.MFBCCardStatusEnum == CardStatus.IsRefunded)
                {

                    switch (vm.MFBCCardStatusEnum)
                    {
                        case CardStatus.InActive:
                            agentResult.Message = "Buiness Card has been Deactivated ,please contact MoneyFex Support";
                            break;
                        case CardStatus.IsDeleted:
                            agentResult.Message = "Buiness Card has been Deleted , please contact MoneyFex Support";
                            break;
                        case CardStatus.IsRefunded:
                            agentResult.Message = "Buiness Card has been Refunded , please contact MoneyFex Support";
                            break;
                        default:
                            break;
                    }
                    agentResult.Status = ResultStatus.Warning;
                    agentResult.Data = vm;
                }

            }
            return agentResult;
        }

        [HttpPost]
        public ActionResult PayBusinessCardUser([Bind(Include = PayMFBCCardUserViewModel.BindProperty)]PayMFBCCardUserViewModel model)
        {
            Admin.Services.CommonServices getCountry = new Admin.Services.CommonServices();
            var countries = getCountry.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(dbContext.IdentityCardType.ToList(), "Id", "CardType");
            ViewBag.IdIssuingCountry = new SelectList(dbContext.Country.OrderBy(x => x.CountryName), "CountryCode", "CountryName");

            AgentResult agentResult = new AgentResult();

            //var ExcessiveRequestIsInitiated = ModelState["ExcessiveRequests"].Value;

            //var IsValidRequest = ModelState.IsValidField("ExcessiveRequests");
            //if (IsValidRequest == false) {

            //    return RedirectToAction("PayBusinessCardUser");
            //}
            if (ModelState.IsValid)
            {

                if (model.MFBCCardStatusEnum == CardStatus.IsDeleted || model.MFBCCardStatusEnum == CardStatus.InActive || model.MFBCCardStatusEnum == CardStatus.IsRefunded)
                {
                    agentResult.Message = "Your Card Status" + " " + model.MFBCCardStatus;
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(model);
                }
                if (Convert.ToDecimal(model.AmountRequested) <= 0)
                {

                    agentResult.Message = "Please Enter Amount Above 0";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(model);

                }
                if (Convert.ToDecimal(model.AmountRequested) > Convert.ToDecimal(model.AmountOnCard))
                {

                    agentResult.Message = "You Dont Have Enough Balance.";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(model);

                }
                string CardExpiryYear = Convert.ToString(model.IdCardExpiringDate.Date.Year);
                if (Convert.ToInt32(CardExpiryYear.Length) != 4)
                {
                    agentResult.Message = "Receiver's Identity is Invalid.";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(model);
                }
                if (model.IdCardExpiringDate < DateTime.Now)
                {
                    agentResult.Message = "Receiver's Identity is expired.";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(model);
                }


                if (model.PayingAgentName == null)
                {
                    ModelState.AddModelError("PayingAgentName", "Please Enter the Name of Paying Agent");
                    ViewBag.AgentResult = agentResult;
                    return View(model);
                }
                if (!model.IsConfirmed)
                {
                    agentResult.Message = "Confirmation for the information is required to either pay or rejection this transaction has been fully verified by yourself";
                    agentResult.Status = ResultStatus.Warning;
                    ViewBag.AgentResult = agentResult;
                    return View(model);
                }
                AgentServices.MFBCPaymentServices mFBCPaymentServices = new AgentServices.MFBCPaymentServices();

                AgentServices.PaymentReceiptServices receiptServices = new AgentServices.PaymentReceiptServices();

                string ReceiptNumber = receiptServices.GetNewMFBCBusinessCardPaymentReceiptNumber();

                DB.KiiPayBusinessWalletWithdrawlFromAgent withdrawl = new DB.KiiPayBusinessWalletWithdrawlFromAgent()
                {
                    //AgentInformationId = Common.AgentSession.AgentInformation.Id,
                    AgentInformationId = model.AgentId,
                    KiiPayBusinessWalletInformationId = model.MFBCCardId,
                    TransactionAmount = model.AmountRequested,
                    IdCardExpiringDate = model.IdCardExpiringDate,
                    IdentificationTypeId = model.IdentificationTypeId,
                    PayingAgentName = model.PayingAgentName,
                    IdNumber = model.IdNumber,
                    IssuingCountryCode = model.IssuingCountryCode,
                    ReceiptNumber = ReceiptNumber,
                    TransactionDate = DateTime.Now,
                    TransactionType = 1,
                    PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId
                };

                var result = mFBCPaymentServices.MFBCCardWithdrawl(withdrawl);


                if (result == true)
                {

                    var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                    MailCommon mail = new MailCommon();
                    string body = "";
                    string BalanceOnCard = (model.AmountOnCard - model.AmountRequested).ToString();
                    string PayingAgentCity = Common.AgentSession.AgentInformation.City;
                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/MFBCCardUsageEmail?NameOfBusinessUser=" + model.BusinessName +
                        "&MFBCCardNumber=" + model.MFBCCardNumber + "&CarduserName=" + model.MFBCCardName +
                        "&CardUserCountry=" + model.MFBCCountry + "&CardUserCity=" + model.MFBCCity +
                        "&CityofPayingAgentOrRegisteredMerchant=" + PayingAgentCity + "&BalanceOnCard=" + BalanceOnCard);
                    //mail.SendMail("anankarki97@gmail.com", "MoneyFax Business Card Usage -Alert", body);
                    mail.SendMail(model.BusinessEmail, "MoneyFax Business Card Usage -Alert", body);
                    agentResult.Message = "Payment Completed Successfully";
                    agentResult.Status = ResultStatus.OK;
                    agentResult.Data = withdrawl.Id;
                    ViewBag.AgentResult = agentResult;
                    ModelState.Clear();
                    Models.PayMFBCCardUserViewModel vm = new Models.PayMFBCCardUserViewModel();
                    vm.DateTime = DateTime.Now;
                    vm.MFBCCardStatus = "";
                    return View(vm);
                }

            }
            ViewBag.AgentResult = agentResult;
            return View(model);
        }
        #endregion

        #region KiiPay Wallet
        [HttpGet]
        public ActionResult PayAReceiverKiiPayWallet(string kiiPayWalletNumber)
        {

             AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            int agentId = agentInfo.Id;

            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            ViewBag.IdTypes = new SelectList(GetIdTypes(), "Id", "CardType");
            ViewBag.Countries = new SelectList(GetCountries(), "Code", "Name");
            //Access Code Validation remaining
            PayAReceiverKiiPayWalletViewModel vm = new PayAReceiverKiiPayWalletViewModel();
            vm = _payAReceiverServices.GetPayAReceiver();
            ViewBag.AgentResult = new AgentResult();
            vm.AgentId = agentId;
            vm.WalletStatusName = "Active";
            if (!string.IsNullOrEmpty(kiiPayWalletNumber))
            {
                var receiverData = payAReceiverServices.GetUserDetailByKiipayWalletNumber(kiiPayWalletNumber);
                if (receiverData == null)
                {

                    AgentResult agentResult = new AgentResult();
                    //agentResult.Message = "Please enter a valid mobile no";
                    //agentResult.Status = ResultStatus.Warning;
                    //ViewBag.AgentResult = agentResult;
                    ModelState.AddModelError("Invalid", "Invalid Mobile Number");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                ViewBag.IdTypes = new SelectList(GetIdTypes(), "Id", "CardType", receiverData.IdType);
                var country = Common.Common.GetCountryName(agentInfo.CountryCode);
                if (receiverData.Country.ToLower() != country.ToLower())
                {
                    AgentResult agentResult = new AgentResult();
                    //agentResult.Message = "The resgitered card  is not of your country, please direct the customer to their respective country.";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("Invalid", "Registered card not of your country, please direct the customer to their respective country.");
                    ViewBag.AgentResult = agentResult;
                    return View(receiverData);

                }
                if (receiverData.CashLimitType != CardLimitType.NoLimitSet)
                {
                    if (receiverData.CashWithdrawalLimit == 0)
                    {

                        AgentResult agentResult = new AgentResult();
                        //agentResult.Message = "Sorry! You cannot make an withdrawal ,Your withdrawal limit is 0 ";
                        //agentResult.Status = ResultStatus.Warning;
                        ModelState.AddModelError("Invalid", "Sorry! You cannot make an withdrawal ,Your withdrawal limit is 0 ");
                        ViewBag.AgentResult = agentResult;
                        return View(receiverData);
                    }
                }
                ViewBag.AgentResult = GetPayAReceiverStatus(receiverData);
                return View(receiverData);
            }
            return View(vm);

        }
        [HttpPost]
        public ActionResult PayAReceiverKiiPayWallet([Bind(Include = PayAReceiverKiiPayWalletViewModel.BindProperty)] PayAReceiverKiiPayWalletViewModel vm)
        {
            ViewBag.IdTypes = new SelectList(GetIdTypes(), "Id", "CardType");
            ViewBag.Countries = new SelectList(GetCountries(), "Code", "Name");

            AgentServices.PayAReceiverControllerServices payAReceiverServices = new AgentServices.PayAReceiverControllerServices();

            AgentResult agentResult = new AgentResult();

            if (vm.AgentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            vm.PhoneCode = Common.Common.GetCountryPhoneCode(vm.IssuingCountry);
            if (string.IsNullOrEmpty(vm.WalletNo))
            {
                //agentResult.Message = "Please Find the wallet information by using wallet number";
                //agentResult.Status = ResultStatus.Warning;
                ModelState.AddModelError("Invalid", "Please Find the wallet information by using wallet number");
                ViewBag.AgentResult = agentResult;
                return View(vm);
            }

            if (ModelState.IsValid)
            {

                if (validatePayAReceiverData(agentResult, vm) == false)
                {
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                if (vm.ExpiryDate <= DateTime.Now)
                {
                    ModelState.AddModelError("ExpiredDate", "Expired ID");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }
                PayAReceiverKiiPayWalletEnteramountViewModel enterAmount = new PayAReceiverKiiPayWalletEnteramountViewModel()
                {
                    WalletCurrencySymbol = Common.Common.GetCurrencySymbol(vm.CountryCode),
                    WalletCurrencyCode = Common.Common.GetCountryCurrency(vm.CountryCode),
                    CashLimitType = vm.CashLimitType,
                    LimitBalance = vm.CashWithdrawalLimit,
                    WalletName = vm.FirstName + " " + vm.MiddleName + " " + vm.LastName,
                };

                if (vm.KiiPayWalletType == KiiPayWalletType.Personal)
                {
                    var walletInfo = payAReceiverServices.GetKiiPayPersonalInformation().Where(x => x.MobileNo == vm.WalletNo).FirstOrDefault();
                    enterAmount.WalletCurBalance = walletInfo.CurrentBalance;
                    enterAmount.LimitBalance = vm.CashWithdrawalLimit;
                }
                else
                {
                    var walletInfo = payAReceiverServices.GetKiiPayBusinessInformation().Where(x => x.MobileNo == vm.WalletNo).FirstOrDefault();
                    enterAmount.WalletCurBalance = walletInfo.CurrentBalance;
                    enterAmount.LimitBalance = vm.CashWithdrawalLimit;
                }

                payAReceiverServices.SetPayAReceiver(vm);
                payAReceiverServices.SetPayAReceiverKiiPayWalletEnteramount(enterAmount);

                return RedirectToAction("PayAReceiverKiiPayWalletEnteramount", "PayAReceiver", new { @Area = "Agent" });
            }
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }

        [HttpGet]
        public ActionResult PayAReceiverKiiPayWalletEnteramount()
        {
            PayAReceiverKiiPayWalletEnteramountViewModel vm = payAReceiverServices.GetPayAReceiverKiiPayWalletEnteramount();

            return View(vm);
        }

        [HttpPost]
        public ActionResult PayAReceiverKiiPayWalletEnteramount([Bind(Include = PayAReceiverKiiPayWalletEnteramountViewModel.BindProperty)] PayAReceiverKiiPayWalletEnteramountViewModel vm)
        {
            AgentServices.PayAReceiverControllerServices payAReceiverServices = new AgentServices.PayAReceiverControllerServices();
            AgentResult agentResult = new AgentResult();
            var agentInformation = Common.AgentSession.LoggedUser;
            PayAReceiverKiipayWalletSuccessViewModel transcationSuccessVm = new PayAReceiverKiipayWalletSuccessViewModel();
            if (ModelState.IsValid)
            {
                var receiverData = payAReceiverServices.GetPayAReceiver();

                //Expired used for confirmation check 
                if (vm.IsExpired == false)
                {
                    ModelState.AddModelError("IsConfirm", "Please Confirm!!!");
                    return View(vm);
                }

                #region checking limits for transaction

                if (Convert.ToDecimal(vm.Amount) != 0)
                {

                    Decimal TotaAmountWithDrawl = 0;
                    var cardUserWithdral = payAReceiverServices.GetPersonalUserCardWithdrawl().Data;
                    var FirstWithdrawl = cardUserWithdral.Where(x => x.KiiPayPersonalWalletInformationId == receiverData.Id).FirstOrDefault();
                    var LastWithdrawl = cardUserWithdral.Where(x => x.KiiPayPersonalWalletInformationId == receiverData.Id).ToList().LastOrDefault();

                    DateTime currentDate = DateTime.Now.Date;

                    if (receiverData.CashLimitType == CardLimitType.Daily)
                    {
                        if ((LastWithdrawl != null) && LastWithdrawl.TransactionDate.Date == currentDate)
                        {
                            TotaAmountWithDrawl = dbContext.UserCardWithdrawl.Where(x => DbFunctions.TruncateTime(x.TransactionDate) == DbFunctions.TruncateTime(currentDate)).Sum(x => x.TransactionAmount);
                        }
                        if ((TotaAmountWithDrawl + Convert.ToDecimal(vm.Amount)) > Convert.ToDecimal(receiverData.CashWithdrawalLimit))
                        {
                            //agentResult.Message = "Sorry, You have exceeded your withdrawl limit";
                            //agentResult.Status = ResultStatus.Warning;
                            //ViewBag.AgentResult = agentResult;

                            ModelState.AddModelError("Amount", "Sorry, You have exceeded your withdrawl limit");
                            return View(vm);
                        }

                    }

                    else if (receiverData.CashLimitType == CardLimitType.NoLimitSet)
                    {

                    }
                    else
                    {

                        DateTime StartedTransactionDate = new System.DateTime();

                        if (FirstWithdrawl != null)
                        {
                            // var DateDifference = System.DateTime.Now  - FirstWithdrawl.TransactionDate ;
                            var DateDifference = currentDate - FirstWithdrawl.TransactionDate;

                        }
                        int gannuParneDays = getGannuParneDays(Enum.GetName(typeof(CardLimitType), receiverData.CashLimitType), currentDate);
                        StartedTransactionDate = currentDate.AddDays(-(gannuParneDays));
                        ///TODO: put this on services

                        TotaAmountWithDrawl = payAReceiverServices.getTotalAmountWithDrawl(StartedTransactionDate, receiverData.Id);
                        if (TotaAmountWithDrawl + Convert.ToDecimal(vm.Amount) > Convert.ToDecimal(vm.CashLimitType))
                        {

                            ModelState.AddModelError("Amount", "Sorry, You have exceeded your withdrawl limit");
                            return View(vm);
                        }
                        if (vm.Amount > vm.LimitBalance)
                        {
                            agentResult.Message = "Limit Balance Exceed with your Enter Balance";
                            agentResult.Status = ResultStatus.Warning;
                            ViewBag.AgentResult = agentResult;
                            return View(vm);
                        }

                    }

                }

                #endregion

                AgentServices.PaymentReceiptServices receiptServices = new AgentServices.PaymentReceiptServices();
                string ReceiptNumber = receiptServices.GetNewMFTCCardPaymentReceiptNumber();


                #region Personal Payment Trans
                if (receiverData.KiiPayWalletType == KiiPayWalletType.Personal)
                {
                    var agentCommission = Common.Common.GetAgentReceivingCommission(TransferService.KiiPayWallet, receiverData.AgentId, Convert.ToDecimal(vm.Amount), 0);
                    KiiPayPersonalWalletWithdrawalFromAgent transaction = new KiiPayPersonalWalletWithdrawalFromAgent()
                    {
                        AgentInformationId = receiverData.AgentId,
                        KiiPayPersonalWalletInformationId = receiverData.Id,
                        TransactionAmount = Convert.ToDecimal(vm.Amount),
                        IdCardExpiringDate = receiverData.ExpiryDate,
                        IdNumber = receiverData.IdNumber,
                        IssuingCountryCode = receiverData.IssuingCountry,
                        IdentificationType = receiverData.IdType.ToString(),
                        TransactionDate = DateTime.Now,
                        TransactionType = 1,
                        PayingAgentName = agentInformation.PayingAgentStaffName,
                        ReceiptNumber = ReceiptNumber,
                        PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                        AgentCommission = agentCommission,
                    };

                    var result = payAReceiverServices.AddPayAreceiverPersonalKiiPay(transaction);

                    if (result.Status == ResultStatus.OK)
                    {

                        transcationSuccessVm = new PayAReceiverKiipayWalletSuccessViewModel()
                        {

                            KiiPayWalletType = KiiPayWalletType.Personal,
                            TransactionId = result.Data.Id
                        };

                        payAReceiverServices.SetPayAReceiverKiipayWalletSuccess(transcationSuccessVm);
                        var kiiPayPersonalWalletDtail = payAReceiverServices.GetKiiPayPersonalInformation().Where(x => x.Id == receiverData.Id).FirstOrDefault();

                        if (Convert.ToDecimal(vm.Amount) == kiiPayPersonalWalletDtail.CurrentBalance)
                        {
                            kiiPayPersonalWalletDtail.CurrentBalance = kiiPayPersonalWalletDtail.CurrentBalance - vm.Amount;
                            if (kiiPayPersonalWalletDtail.CurrentBalance >= 0)
                            {
                                payAReceiverServices.UpdateKiiPayPersonalWalletInformation(kiiPayPersonalWalletDtail); ;
                            }

                            agentResult.Message = "Payment Completed Successfully";
                            agentResult.Status = ResultStatus.OK;
                            agentResult.Data = transaction.Id;
                            ViewBag.AgentResult = agentResult;
                            ModelState.Clear();
                            //var model = new PayMFTCCardUserViewModel();
                            //model.StatusOfFaxName = "";
                            //return View(model);

                        }
                        else
                        {
                            kiiPayPersonalWalletDtail.CurrentBalance = kiiPayPersonalWalletDtail.CurrentBalance - vm.Amount;
                            payAReceiverServices.UpdateKiiPayPersonalWalletInformation(kiiPayPersonalWalletDtail);
                            agentResult.Message = "Payment Completed Sucessfully";
                            agentResult.Status = ResultStatus.OK;
                            agentResult.Data = transaction.Id;
                            ViewBag.AgentResult = agentResult;

                            ModelState.Clear();
                            //var model = new PayMFTCCardUserViewModel();
                            //model.StatusOfFaxName = "";
                            //return View(model);

                        }

                        return RedirectToAction("PayAReceiverKiiPayWalletSuccess");

                    }
                    else
                    {
                        //    agentResult.Message = "Payment Failed due to technical error";
                        //    agentResult.Status = ResultStatus.Error;
                        ModelState.AddModelError("TechnicalError", "Payment Failed due to technical error");
                    }
                }

                #endregion

                #region Business wallet

                if (receiverData.KiiPayWalletType == KiiPayWalletType.Business)
                {
                    string ReceiptNo = receiptServices.GetNewMFBCCardWithdrawlsReceiptNumber();

                    AgentServices.MFBCPaymentServices mFBCPaymentServices = new AgentServices.MFBCPaymentServices();
                    var agentCommission = Common.Common.GetAgentReceivingCommission(TransferService.KiiPayWallet, receiverData.AgentId, vm.Amount, 0);
                    DB.KiiPayBusinessWalletWithdrawlFromAgent withdrawl = new DB.KiiPayBusinessWalletWithdrawlFromAgent()
                    {
                        AgentInformationId = receiverData.AgentId,
                        KiiPayBusinessWalletInformationId = receiverData.Id,
                        TransactionAmount = vm.Amount,
                        IdCardExpiringDate = receiverData.ExpiryDate,
                        IdentificationTypeId = 1,
                        PayingAgentName = agentInformation.PayingAgentStaffName,
                        IdNumber = receiverData.IdNumber,
                        IssuingCountryCode = receiverData.CountryCode,
                        ReceiptNumber = ReceiptNo,
                        TransactionDate = DateTime.Now,
                        TransactionType = 1,
                        PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                        AgentCommission = agentCommission

                    };
                    var businessresult = mFBCPaymentServices.MFBCCardWithdrawl(withdrawl);

                    return RedirectToAction("PayAReceiverKiiPayWalletSuccess");
                }

                #endregion

            }
            ViewBag.AgentResult = agentResult;
            return View(vm);

        }

        [HttpGet]
        public ActionResult PayAReceiverKiiPayWalletSuccess()
        {
            PayAReceiverKiipayWalletSuccessViewModel vm = payAReceiverServices.GetPayAReceiverKiipayWalletSuccess();
            AgentCommonServices cs = new AgentCommonServices();
            // cs.ClearPayAReceiverKiiPay();
            return View(vm);
        }
        public List<IdTypeDropDownVm> GetIdTypes()
        {
            var result = (from c in dbContext.IdentityCardType.ToList()
                          select new IdTypeDropDownVm()
                          {
                              Id = c.Id,
                              CardType = c.CardType
                          }).ToList();

            return result;
        }

        public List<CountryDropDownVm> GetCountries()
        {
            var result = (from c in Common.Common.GetCountries()
                          select new CountryDropDownVm()
                          {
                              Code = c.CountryCode,
                              Name = c.CountryName
                          }).ToList();
            return result;
        }
        #endregion

        #region Cash Pickup
        public ActionResult PayAReceiverCashPickup(string MFCN = "")
        {

            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            string agencyName = agentInfo.Name;
            string agencyMFSCode = agentInfo.AccountNo;
            int agentId = agentInfo.Id;

            if (agentId == 0)
            {
                Common.AgentSession.FormURL = "/Agent/PayAReceiver/PayAReceiverCashPickup";
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            PayAReceiverCashPickupViewModel vm = new PayAReceiverCashPickupViewModel();
            vm = _payAReceiverServices.GetPayAReceiverCashPickupViewModel();
            AgentResult agentResult = new AgentResult();
            ViewBag.AgentResult = GetStatusOfMFCN(vm);
            if (!string.IsNullOrEmpty(MFCN))
            {
                _payAReceiverServices.SetMFCN(MFCN);
                var nonCardReceived = _payAReceiverServices.GetNonCardWithdrawl(MFCN);

                var TransactionCountry = _payAReceiverServices.GetTransactionCountry(MFCN);
                try
                {
                    if ((TransactionCountry != null) && TransactionCountry.NonCardReciever.Country.ToLower() != agentInfo.CountryCode.ToLower())
                    {
                        PayAReceiverCashPickupViewModel model = new PayAReceiverCashPickupViewModel();

                        //agentResult.Message = "This transaction was not sent to your country, please direct the customer to their respective country.";
                        //agentResult.Status = ResultStatus.Warning;
                        ModelState.AddModelError("Invalid", "This transaction was not sent to your country, please direct the customer to their respective country.");

                        ViewBag.AgentResult = agentResult;
                        return View(model);
                    }

                }
                catch (Exception)
                {

                }

                vm = _payAReceiverServices.GetSenderInfo(MFCN);
                if (vm != null)
                {
                    ViewBag.AgentResult = GetStatusOfMFCN(vm);
                    if (nonCardReceived != null)
                    {

                        return View(vm);
                    }
                }
                else
                {
                    PayAReceiverCashPickupViewModel model = new PayAReceiverCashPickupViewModel();

                    //agentResult.Message = "MFCN Number Does not Exist";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("Invalid", "MFCN number does not exist");
                    ViewBag.AgentResult = agentResult;
                    return View(model);
                }

                return View(vm);
            }


            return View(vm);
        }
        private AgentResult GetStatusOfMFCN(PayAReceiverCashPickupViewModel vm)
        {
            AgentResult agentResult = new AgentResult();
            if (vm == null)
            {
                //agentResult.Message = "MFCN does not exist.Please enter a valid MFCN";
                //agentResult.Status = ResultStatus.Warning;
                ModelState.AddModelError("Invalid", "MFCN number does not exist");
                return agentResult;
            }
            else
            {
                if (vm.StatusOfFax == FaxingStatus.Received)
                {
                    //agentResult.Message = "Sorry! you cannot be withdrawal make , this transaction has been Received.";
                    //agentResult.Status = ResultStatus.Warning;
                    agentResult.Data = vm;
                }
                else if (vm.StatusOfFax == FaxingStatus.Refund)
                {
                    //agentResult.Message = "Sorry! you cannot be withdrawal make  ,this transaction has been Refunded";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("Invalid", "Sorry! you cannot be withdrawal make  ,this transaction has been Refunded");
                }
                else if (vm.StatusOfFax == FaxingStatus.Cancel)
                {
                    //agentResult.Message = "Sorry! you cannot be withdrawal make  ,this transaction has been Cancelled";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("Invalid", "Sorry! you cannot be withdrawal make  ,this transaction has been Cancelled");
                }
                else if (vm.StatusOfFax == FaxingStatus.Hold)
                {
                    //agentResult.Message = "Sorry! This Transaction can not be completed because it has been held by Customer Service";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("Invalid", "Sorry! This Transaction can not be completed because it has been held by Customer Service");
                }
            }
            return agentResult;
        }

        [HttpPost]
        public ActionResult PayAReceiverCashPickup([Bind(Include = PayAReceiverCashPickupViewModel.BindProperty)]PayAReceiverCashPickupViewModel vm)
        {
            string MFCN = vm.MFCN;
            if (ModelState.IsValid)
            {
                _payAReceiverServices.SetPayAReceiverCashPickupViewModel(vm);
                return RedirectToAction("PayAReceiveCashPickupReceiverDetails", new { @MFCN = MFCN });
            }
            ViewBag.AgentResult = GetStatusOfMFCN(vm);
            return View(vm);

        }

        [HttpGet]
        public ActionResult PayAReceiveCashPickupReceiverDetails(string MFCN)
        {

            PayAReceiveCashPickupReceiverDetailsViewModel vm = new PayAReceiveCashPickupReceiverDetailsViewModel();
            AgentResult agentResult = new AgentResult();

            Admin.Services.CommonServices getCountry = new Admin.Services.CommonServices();
            Admin.Services.CommonServices getIdType = new Admin.Services.CommonServices();
            var countries = getCountry.GetCountries();
            var IdTypes = getIdType.GetCardType();

            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(IdTypes, "Id", "CardType", vm.IdType);
            ViewBag.IdIssuingCountry = new SelectList(countries, "Code", "Name", vm.IssuingCountry);
            ViewBag.BirthCountry = new SelectList(countries, "Code", "Name", vm.BirthCountry);

            vm = _payAReceiverServices.GetReceiverDetails();
            if (!string.IsNullOrEmpty(MFCN))
            {
                vm = _payAReceiverServices.GetReciverInfo(MFCN);


                if (vm != null)
                {
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }


            }
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }
        [HttpPost]
        public ActionResult PayAReceiveCashPickupReceiverDetails([Bind(Include = PayAReceiveCashPickupReceiverDetailsViewModel.BindProperty)] PayAReceiveCashPickupReceiverDetailsViewModel vm, string MFCN)
        {
            Admin.Services.CommonServices getCountry = new Admin.Services.CommonServices();
            Admin.Services.CommonServices getIdType = new Admin.Services.CommonServices();

            var countries = getCountry.GetCountries();
            var IdTypes = getIdType.GetCardType();

            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            ViewBag.IDTypes = new SelectList(IdTypes, "Id", "CardType");
            ViewBag.IdIssuingCountry = new SelectList(countries, "Code", "Name");
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            ViewBag.BirthCountry = new SelectList(countries, "Code", "Name", vm.BirthCountry);
            AgentResult agentResult = new AgentResult();

            if (ModelState.IsValid)
            {

                if (vm.ExpiryDate < DateTime.Now)
                {
                    ModelState.AddModelError("IDExpired", "ID Expired");
                    ViewBag.AgentResult = agentResult;
                    return View(vm);
                }


                ViewBag.AgentResult = agentResult;

                _payAReceiverServices.SetReceiverDetails(vm);
                return RedirectToAction("PayAReceiverCashPickupAmount", "PayAReceiver");

            }
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }
        [HttpGet]
        public ActionResult PayAReceiverCashPickupAmount()
        {
            AgentResult agentResult = new AgentResult();
            string MFCN = AgentSession.MFCN;
            var model = _payAReceiverServices.GetTransactionCountry(MFCN);
            decimal amount = model.ReceivingAmount;
            PayAReceiverCashPickupAmountViewModel vm = new PayAReceiverCashPickupAmountViewModel();
            vm.ReceivingAmount = amount;
            vm.PickUpCurrency = Common.Common.GetCurrencySymbol(model.NonCardReciever.Country);
            vm.SendingCurrency = Common.Common.GetCountryCurrency(model.NonCardReciever.FaxerInformation.Country);
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }
        [HttpPost]
        public ActionResult PayAReceiverCashPickupAmount([Bind(Include = PayAReceiverCashPickupAmountViewModel.BindProperty)] PayAReceiverCashPickupAmountViewModel vm)
        {
            AgentResult agentResult = new AgentResult();
            string MFCN = AgentSession.MFCN;
            var Model = _payAReceiverServices.GetTransactionCountry(MFCN);
            decimal amount = Model.ReceivingAmount;
            vm.PickUpCurrency = Common.Common.GetCurrencySymbol(Model.NonCardReciever.FaxerInformation.Country);
            vm.SendingCurrency = Common.Common.GetCountryCurrency(Model.NonCardReciever.FaxerInformation.Country);
            vm.AmountSent = amount;

            if (vm.IsConfirm == false)

            {
                ModelState.AddModelError("IsConfirm", "Please Confirm!!!");

                return View(vm);
            }

            #region Sender Non Card Transfer Withdrawal
            else
            {
                AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
                int agentId = agentInfo.Id;
                if (agentId == 0)
                {
                    Common.AgentSession.FormURL = "/Agent/PayAReceiver/PayAReceiverCashPickup";
                    return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
                }
                var model = _payAReceiverServices.GetReceiverDetails();
                var agentCommission = Common.Common.GetAgentReceivingCommission(TransferService.CahPickUp, agentId, vm.AmountSent, 0);
                string ReceiptNo = Common.Common.GeneratePayAReceiverCashPickUpReceiptNo(6);
                ReceiverNonCardWithdrawl obj = new ReceiverNonCardWithdrawl()
                {
                    IdCardExpiringDate = model.ExpiryDate ?? new DateTime(),
                    IdentificationTypeId = int.Parse(model.IdType),
                    IdNumber = model.IdNumber,
                    IssuingCountryCode = model.IssuingCountry,
                    MFCN = MFCN,
                    ReceiverId = model.Id,
                    TransactionAmount = model.Amount,
                    //ReceivingAmount = decimal.Parse(vm.ReceivingAmount),
                    TransactionDate = DateTime.Now,
                    TransactionType = 1,
                    PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId,
                    AgentId = agentId,
                    ReceivingAmount = vm.AmountSent,
                    AgentCommission = agentCommission,
                    PayingAgentName = Common.AgentSession.LoggedUser.PayingAgentStaffName,
                    ReceiptNumber = ReceiptNo

                };

                var result = _payAReceiverServices.Add(obj);

                if (result.Status == ResultStatus.OK)
                {
                    var nonCardTransaction = _payAReceiverServices.GetTransactionCountry(MFCN);
                    nonCardTransaction.FaxingStatus = FaxingStatus.Received;
                    nonCardTransaction.StatusChangedDate = DateTime.Now;
                    _payAReceiverServices.Update(nonCardTransaction);
                }
                else
                {

                    ModelState.AddModelError("TechnicalError", "Payment Failed due to technical error");

                }

                #endregion
                #region SMS And Email to Sender
                SmsApi smsService = new SmsApi();
                var transactionInfo = _payAReceiverServices.GetTransInfo(MFCN);
                var senderName = Model.NonCardReciever.FaxerInformation.FirstName;
                var senderCurrencySymbol = Common.Common.GetCurrencySymbol(transactionInfo.NonCardReciever.FaxerInformation.Country);
                var ReceiverCurrySymbol = Common.Common.GetCurrencySymbol(transactionInfo.NonCardReciever.Country);
                var SendingAmountWithCurrecySymbol = senderCurrencySymbol + " " + transactionInfo.FaxingAmount.ToString();
                var FeeWithCurrecySymbol = senderCurrencySymbol + " " + transactionInfo.FaxingFee.ToString();
                var ReceivingAmountWithCurrencySymbol = ReceiverCurrySymbol + " " + transactionInfo.ReceivingAmount.ToString();
                var ReceiverFirstName = transactionInfo.NonCardReciever.FirstName;

                //var msg = smsService.GetCashToCashTransferMessage(senderName, MFCN, SendingAmountWithCurrecySymbol, FeeWithCurrecySymbol, ReceivingAmountWithCurrencySymbol);
                var msg = smsService.GetCashPickUPReceivedMessage(senderName, ReceiverFirstName, MFCN, SendingAmountWithCurrecySymbol, FeeWithCurrecySymbol, ReceivingAmountWithCurrencySymbol);

                var PhoneNo = Common.Common.GetCountryPhoneCode(Model.NonCardReciever.FaxerInformation.Country) + Model.NonCardReciever.FaxerInformation.PhoneNumber;
                smsService.SendSMS(PhoneNo, msg);

                Services.SSenderForAllTransfer sSenderForAllTransfer = new Services.SSenderForAllTransfer();
                sSenderForAllTransfer.SendCashPickUpSuccessEmail(senderName, transactionInfo.NonCardReciever.FullName, ReceiverFirstName, SendingAmountWithCurrecySymbol,
                                                                  transactionInfo.ReceivingCountry, transactionInfo.NonCardReciever.FaxerInformation.Id, transactionInfo.NonCardReciever.City);

                #endregion
                return RedirectToAction("PayAReceiveCashPickupSuccess", "PayAReceiver", new { MFCN = obj.MFCN });
            }


        }
        [HttpGet]
        public ActionResult PayAReceiveCashPickupSuccess(string MFCN)
        {
            AgentCommonServices cs = new AgentCommonServices();
            //cs.ClearPayAReceiverCashPickUp();
            ViewBag.MFCN = MFCN;
            return View();
        }


        #endregion
    }
}