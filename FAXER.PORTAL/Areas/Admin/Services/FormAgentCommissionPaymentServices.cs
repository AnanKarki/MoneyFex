using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class FormAgentCommissionPaymentServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices CommonService = new CommonServices();

        public bool CommissionRateSetup(string Country)
        {

            var result = dbContext.AgentCommission.Where(x => x.Country == Country).FirstOrDefault();
            if (result == null)
            {
                return false;
            }
            return true;

        }

        public FormAgentCommissionPaymentViewModel getInfo(FormAgentCommissionPaymentViewModel model)
        {
            AgentCommissionServices _CommissionService = new AgentCommissionServices();
            if (model.AgentId != 0)
            {
                model.CurrencySymbol = CommonService.getCurrencySymbol(model.Country);
                model.SentCommissionRate = (decimal)dbContext.AgentCommission.Where(x => x.Country == model.Country).FirstOrDefault().SendingRate;
                model.ReceiverCommissionRate = (decimal)dbContext.AgentCommission.Where(x => x.Country == model.Country).FirstOrDefault().ReceivingRate;
                model.MFSCode = dbContext.AgentInformation.Where(x => x.Id == model.AgentId).FirstOrDefault().AccountNo;
                //model.Status = dbContext.AgentInformation.Where(x => x.Id == model.AgentId).FirstOrDefault().AgentStatus;
                model.AdminName = CommonService.getStaffName(Common.StaffSession.LoggedStaff.StaffId);

                int AgentStaffId = dbContext.AgentStaffInformation.Where(x => x.AgentId == model.AgentId).FirstOrDefault().Id;
                int intYear = int.Parse(model.Year);
                int intMonth = (int)model.Month;

                //checking if payment is already done
                bool paymentDone = checkPayment(model.AgentId, model.Year, model.Month);
                if (paymentDone)
                {
                    model.Status = "Paid";
                }
                else model.Status = "Not Paid";

                //calculating sent amount
                decimal TotalSentPayment = 0;
                var sentAmount = from c in dbContext.AgentFaxMoneyInformation.Where(x => x.AgentId == model.AgentId)
                                 join d in dbContext.FaxingNonCardTransaction.Where(x => x.TransactionDate.Year == intYear && x.TransactionDate.Month == intMonth)
                                 on c.NonCardTransactionId equals d.Id
                                 select d;
                if (sentAmount.Count() > 0)
                {
                    TotalSentPayment = sentAmount.Sum(x => x.FaxingFee);

                }

                var SendingCommission = _CommissionService.GetSendingCommission(model.Year, intMonth, AgentStaffId);

                //model.TotalSentPayment = Math.Round(TotalSentPayment, 2);
                model.TotalSentPayment = Math.Round(SendingCommission, 2);
                if (model.SentCommissionRate > 1)
                {

                    model.SentCommissionRate = model.SentCommissionRate / 100;
                }
                //model.TotalSentCommission = Math.Round(model.SentCommissionRate * TotalSentPayment, 2);

                model.TotalSentCommission = Math.Round(model.SentCommissionRate * SendingCommission, 2);


                //calculating received amount
                decimal TotalReceivedPayment = 0;

                decimal nonCardReceivedTransactionAmount = 0;
                decimal MFTCCardWithdrawlTransactionAmount = 0;
                decimal MFBCCardWithdrawlTransactionAmount = 0;

                var nonCardReceivedAmount = dbContext.ReceiverNonCardWithdrawl.Where(x => x.AgentId == model.AgentId && x.TransactionDate.Year == intYear && x.TransactionDate.Month == intMonth);
                if (nonCardReceivedAmount.Count() > 0)
                {
                    nonCardReceivedTransactionAmount = nonCardReceivedAmount.Sum(x => x.TransactionAmount);
                }

                var MFTCCardWithdrawlAmount = dbContext.UserCardWithdrawl.Where(x => x.AgentInformationId == model.AgentId && x.TransactionDate.Year == intYear && x.TransactionDate.Month == intMonth);
                if (MFTCCardWithdrawlAmount.Count() > 0)
                {
                    MFTCCardWithdrawlTransactionAmount = MFTCCardWithdrawlAmount.Sum(x => x.TransactionAmount);
                }

                var MFBCCardWithdrawlAmount = dbContext.MFBCCardWithdrawls.Where(x => x.AgentInformationId == model.AgentId && x.TransactionDate.Year == intYear && x.TransactionDate.Month == intMonth);
                if (MFBCCardWithdrawlAmount.Count() > 0)
                {
                    MFBCCardWithdrawlTransactionAmount = MFBCCardWithdrawlAmount.Sum(x => x.TransactionAmount);
                }

                TotalReceivedPayment = nonCardReceivedTransactionAmount + MFTCCardWithdrawlTransactionAmount + MFBCCardWithdrawlTransactionAmount;

                //model.TotalReceivedPayment = Math.Round(TotalReceivedPayment, 2);



                if (model.ReceiverCommissionRate > 1)
                {

                    model.ReceiverCommissionRate = model.ReceiverCommissionRate / 100;
                }

                var receivingCommissionAmount = _CommissionService.GetReceivingCommission(model.Year, model.Month.ToInt(), AgentStaffId);

                decimal FaxedFeeAmountForReceivedAmount = model.SentCommissionRate * TotalReceivedPayment;

                //model.TotalReceivedCommission = Math.Round(model.ReceiverCommissionRate * FaxedFeeAmountForReceivedAmount , 2);
                model.TotalReceivedPayment = Math.Round(receivingCommissionAmount, 2);

                model.TotalReceivedCommission = Math.Round(model.ReceiverCommissionRate * receivingCommissionAmount, 2);

                model.TotalSentCommission = Math.Round(model.SentCommissionRate * SendingCommission, 2);

                model.TotalCommission = Math.Round(model.TotalReceivedCommission + model.TotalSentCommission, 2);



                return model;
            }
            return model;
        }

        public bool checkPayment(int agentId, string year, Month month)
        {
            var data = dbContext.AgentCommissionPayment.Where(x => x.AgentId == agentId && x.Year == year && x.Month == month).FirstOrDefault();
            if (data == null)
            {
                return false;
            }
            return true;
        }

        public bool savePaymentDetails(FormAgentCommissionPaymentViewModel model)
        {
            if (model != null)
            {
                AgentCommissionPayment data = new AgentCommissionPayment()
                {
                    AgentId = model.AgentId,
                    Month = model.Month,
                    Year = model.Year,
                    TotalSentPayment = model.TotalSentPayment,
                    SendingCommissionRate = model.SentCommissionRate,
                    TotalSentCommission = model.TotalSentCommission,
                    TotalReceivedPayment = model.TotalReceivedPayment,
                    ReceivingCommissionRate = model.ReceiverCommissionRate,
                    TotalReceivedCommission = model.TotalReceivedCommission,
                    ReceiptNo = GetAgentCommissionPaymentReceiptNumber(),
                    TransactionDateTime = DateTime.Now,
                    VerifiedBy = Common.StaffSession.LoggedStaff.StaffId,
                    TotalCommission = model.TotalCommission
                };
                dbContext.AgentCommissionPayment.Add(data);
                dbContext.SaveChanges();
                MailCommon mail = new MailCommon();
                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";
                var AgentInfo = dbContext.AgentInformation.Where(x => x.Id == data.AgentId).FirstOrDefault();
                string AgentCurrency = dbContext.Country.Where(x => x.CountryCode == AgentInfo.CountryCode).Select(x => x.Currency).FirstOrDefault();
                body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/AgentCommissionPaymentEmail?NameOfAgent=" + AgentInfo.Name +
                    "&AgentMFCode=" + AgentInfo.AccountNo + "&FaxedAndRcVedComm=" + data.TotalCommission + " " + AgentCurrency
                    + "&TotalSendPayment=" + data.TotalSentCommission + " " + AgentCurrency + "&TotalRcVedPayment=" + data.TotalReceivedCommission + " " + AgentCurrency);

                var ReceiptURL = baseUrl + "/EmailTemplate/AgentCommisionPaymentReceipt?ReceiptNumber=" + data.ReceiptNo +
                    "&AgentName=" + AgentInfo.Name + "&AgentMFCode=" + AgentInfo.AccountNo + "&TransferredCommision=" + data.TotalSentCommission +
                    "&ReceivedCommission=" + data.TotalReceivedCommission + "&TotalCommission=" + data.TotalCommission +
                    "&StaffName=" + Common.StaffSession.LoggedStaff.FirstName + " " + Common.StaffSession.LoggedStaff.MiddleName + " " + Common.StaffSession.LoggedStaff.LastName
                    + "&Date=" + data.TransactionDateTime.ToString("dd/MM/yyyy") + "&Time=" + data.TransactionDateTime.ToString("HH:mm") + "&AgentCurrency=" + AgentCurrency;

                var Receipt = Common.Common.GetPdf(ReceiptURL);

                mail.SendMail(AgentInfo.Email, "Commission payment - confirmation  ", body, Receipt);
                return true;
            }
            return false;
        }

        internal string GetAgentCommissionPaymentReceiptNumber()
        {

            //this code should be unique and random with 8 digit length
            Start:
            var val = "Ad-Ag-Comm-MF" + Common.Common.GenerateRandomDigit(5);

            while (dbContext.AgentCommissionPayment.Where(x => x.ReceiptNo == val).Count() > 0)
            {
                goto Start;
                //val = "Ag-wcu-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }

        public List<AgentsListDropDown> GetAgents(string country, string city)
        {
            var result = (from c in dbContext.AgentInformation.Where(x => x.CountryCode == country && x.City.ToLower() == city.ToLower())
                          select new AgentsListDropDown()
                          {
                              AgentId = c.Id,
                              AgentName = c.Name
                          }).ToList();
            return result;
        }

    }


    public class AgentsListDropDown
    {
        public int AgentId { get; set; }
        public string AgentName { get; set; }
        public string Country { get; set; }
        public string AgentCode { get; set; }
    }
    public class SenderListDropDown
    {
        public int senderId { get; set; }
        public string senderName { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
    }
}