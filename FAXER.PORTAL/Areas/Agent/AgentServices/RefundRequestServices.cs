using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class RefundRequestServices
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        CommonServices CommonService = new CommonServices();
        public Models.RefundRequestViewModel getFaxerReceiverInformation(string MFCN)
        {
            var result = (from noncardTransaction in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).ToList()
                          join receiver in dbContext.ReceiversDetails on noncardTransaction.NonCardRecieverId equals receiver.Id
                          join data in dbContext.FaxerInformation on receiver.FaxerID equals data.Id
                          join faxerCountry in dbContext.Country on data.Country equals faxerCountry.CountryCode
                          join receiverCountry in dbContext.Country on receiver.Country equals receiverCountry.CountryCode
                          select new Models.RefundRequestViewModel()
                          {
                              FaxerId = data.Id,
                              FaxerFirstName = data.FirstName,
                              FaxerMiddleName = data.MiddleName,
                              FaxerLastName = data.LastName,
                              FaxerAddress = data.Address1,
                              FaxerTelephone = data.PhoneNumber,
                              FaxerCity = data.City,
                              FaxerCountryCode = data.Country,
                              FaxerCountry = faxerCountry.CountryName,
                              FaxerEmail = data.Email,
                              ReceiverId = receiver.Id,
                              ReceiverFirstName = receiver.FirstName,
                              ReceiverMiddleName = receiver.MiddleName,
                              ReceiverLastName = receiver.LastName,
                              ReceiverCity = receiver.City,
                              ReceiverCountryCode = receiver.Country,
                              ReceiverCountry = receiverCountry.CountryName,
                              ReceiverTelephone = receiver.PhoneNumber,
                              ReceiverEmailAddress = receiver.EmailAddress,
                              FaxingAmount = String.Format("{0:n}", noncardTransaction.FaxingAmount),
                              FaxingFee = noncardTransaction.FaxingFee.ToString(),
                              StatusOfFax = noncardTransaction.FaxingStatus,
                              StatusOfFaxName = Enum.GetName(typeof(FaxingStatus), noncardTransaction.FaxingStatus),
                              TransactionDate = noncardTransaction.TransactionDate,
                              TransactionId = noncardTransaction.Id,
                              MFCNNumber = noncardTransaction.MFCN,
                              FaxReceiptNumber = noncardTransaction.ReceiptNumber,
                              DateTime = noncardTransaction.TransactionDate,
                              FaxerCurrency = CommonService.getCurrencyCodeFromCountry(data.Country),
                              FaxerCurrencySymbol = CommonService.getCurrencySymbol(data.Country),
                              FaxerPhoneCode = CommonService.getPhoneCodeFromCountry(data.Country),
                              ReceiverPhoneCode = CommonService.getPhoneCodeFromCountry(receiver.Country)
                              
                          }).FirstOrDefault();

            return result;



        }
        public string getFaxingFee(string FaxingCountry, string ReceiverCountry, Decimal FaxedAmount)
        {
            var feeSummary = new EstimateFaxingFeeSummary();
            var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == FaxingCountry && x.CountryCode2 == ReceiverCountry).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateObj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == ReceiverCountry && x.CountryCode2 == FaxingCountry).FirstOrDefault();
                if (exchangeRateObj2 != null)
                {

                    exchangeRateObj = exchangeRateObj2;
                    exchangeRateObj.CountryRate1 = Math.Round(1 / exchangeRateObj2.CountryRate1, 6, MidpointRounding.AwayFromZero);
                }
            }
            if (FaxingCountry == ReceiverCountry) {

                exchangeRateObj.CountryRate1 = 1m;
            }
            feeSummary = Services.SEstimateFee.CalculateFaxingFee(FaxedAmount, true, false, exchangeRateObj.CountryRate1, Services.SEstimateFee.GetFaxingCommision(FaxingCountry));
            //Common.FaxerSession.FaxingAmountSummary = feeSummary;

            string FaxingFee = String.Format("{0:n}", feeSummary.FaxingFee);


            return FaxingFee;

        }

        public bool RefundFaxMoney(Models.RefundRequestViewModel vm)
        {

            var nonCardTransaction = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == vm.MFCNNumber).FirstOrDefault();
            if (nonCardTransaction != null)
            {
                nonCardTransaction.FaxingStatus = FaxingStatus.Refund;
                dbContext.Entry(nonCardTransaction).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            RefundNonCardFaxMoneyByAgent refundDetails = new RefundNonCardFaxMoneyByAgent();
            refundDetails.NonCardTransaction_id = nonCardTransaction.Id;
            refundDetails.Agent_id = vm.AgentId;
            refundDetails.NameofRefunder = vm.NameOfRefunder;
            refundDetails.RefundReason = vm.RefundReason;
            refundDetails.RefundedDate = DateTime.Now;
            refundDetails.ReceiptNumber = vm.RefundReceiptNumber;
            refundDetails.PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId;
            dbContext.RefundNonCardFaxMoneyByAgent.Add(refundDetails);
            int result = dbContext.SaveChanges();

            if (result == 1)
            {
                MailCommon mail = new MailCommon();
                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";
                string FaxerName = vm.FaxerFirstName + " " + vm.FaxerMiddleName + " " + vm.FaxerLastName;
                string RecevierName = vm.ReceiverFirstName + " " + vm.ReceiverMiddleName + " " + vm.ReceiverLastName;

                string ReceiverCounty = Common.Common.GetCountryName(vm.ReceiverCountryCode);
                string ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(vm.ReceiverCountryCode);
                string FaxerCuurency = Common.Common.GetCountryCurrency(vm.FaxerCountryCode);
                string ReceiverCurrrency = Common.Common.GetCountryCurrency(vm.ReceiverCountryCode);

                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/RefundRequestEmailByAgent?FaxerName=" + FaxerName +
                "&MFCNNumber=" + vm.MFCNNumber + "&FaxedAmount=" + vm.FaxingAmount + " " + FaxerCuurency + "&ReceiverName=" + RecevierName +
                "&ReceiverCountry=" + ReceiverCounty + "&ReceiverCity=" + vm.ReceiverCity
                + "&FaxedDate=" + vm.TransactionDate + "&NameOfAgentRefundRequester=" + vm.NameOfRefunder);

                var ReceiptUrl = baseUrl + "/EmailTemplate/AgentRefundReceipt?ReceiptNumber=" + refundDetails.ReceiptNumber +
             "&TransactionReceiptNumber=" + vm.FaxReceiptNumber + "&Date=" + refundDetails.RefundedDate.ToString("dd/MM/yyyy") +
             "&Time=" + refundDetails.RefundedDate.ToString("HH:mm") +
             "&SenderFullName=" + FaxerName
             + "&MFCN=" + vm.MFCNNumber +
             "&ReceiverFullName=" + RecevierName +
             "&Telephone=" + ReceiverPhoneCode + " " + vm.ReceiverTelephone
             + "&RefundingAgentName=" + refundDetails.NameofRefunder + "&RefundingAgentCode=" + vm.AgencyMFSCode
             + "&OrignalAmountSent=" + vm.FaxingAmount + " " + FaxerCuurency +
             "&RefundedAmount=" + vm.FaxingAmount + " " + FaxerCuurency;


                var ReceiptPdf = Common.Common.GetPdf(ReceiptUrl);

                mail.SendMail(vm.FaxerEmail, "Refund Request from MoneyFax Service Agent", body, ReceiptPdf);
                //mail.SendMail("anankarki97@gmail.com", "Refund Request from MoneyFax Service Admin", body, ReceiptPdf);

                return true;
            }
            else
            {
                return false;
            }

        }
        internal string GetNewRefundReceiptNumber()
        {
            //this code should be unique and random with 8 digit length
            var val = "Ag-Tr-MF" + Common.Common.GenerateRandomDigit(5);

            while (dbContext.UserCardWithdrawl.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Ag-Tr-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }


        public DB.FaxingNonCardTransaction GetFaxingNonCardTransaction(string MFCN)
        {

            var result = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            return result;
        }

        public DB.RefundNonCardFaxMoneyByAgent GetRefundNonCardDetails(int transactionId)
        {

            var result = dbContext.RefundNonCardFaxMoneyByAgent.Where(x => x.NonCardTransaction_id == transactionId).FirstOrDefault();
            return result;

        }

        public DB.FaxerInformation GetFaxerInformation(int faxerId)
        {
            var result = dbContext.FaxerInformation.Where(x => x.Id == faxerId).FirstOrDefault();
            return result;
        }

        public DB.ReceiversDetails GetReceiversDetails(int ReceiverId)
        {
            var result = dbContext.ReceiversDetails.Where(x => x.Id == ReceiverId).FirstOrDefault();
            return result;
        }

        public DB.AgentInformation GetAgentInformation(int AgentId)
        {

            var result = dbContext.AgentInformation.Where(x => x.Id == AgentId).FirstOrDefault();
            return result;

        }
    }
}