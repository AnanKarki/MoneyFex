using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class NonCardFaxingRefundRequestServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices CommonService = new CommonServices();

        public NonCardFaxingRefundRequestServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public ViewModels.NonCardFaxingRefundRequestViewModel GetFaxingNonCardDetails(string MFCN)
        {

            var result = (from noncardTransaction in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).ToList()
                          join receiver in dbContext.ReceiversDetails on noncardTransaction.NonCardRecieverId equals receiver.Id
                          join data in dbContext.FaxerInformation on receiver.FaxerID equals data.Id
                          join faxerCountry in dbContext.Country on data.Country equals faxerCountry.CountryCode
                          join receiverCountry in dbContext.Country on receiver.Country equals receiverCountry.CountryCode
                          join Agent in dbContext.AgentFaxMoneyInformation on noncardTransaction.Id equals Agent.NonCardTransactionId
                          into j
                          from joined in j.DefaultIfEmpty()
                          select new ViewModels.NonCardFaxingRefundRequestViewModel()
                          {
                              FaxerId = data.Id,
                              FaxerFirstName = data.FirstName,
                              FaxerMiddleName = data.MiddleName,
                              FaxerLastName = data.LastName,
                              FaxerAddress = data.Address1,
                              FaxerTelephone = CommonService.getPhoneCodeFromCountry(data.Country) + data.PhoneNumber,
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
                              StatusOfFax = noncardTransaction.FaxingStatus,
                              StatusOfFaxName = Enum.GetName(typeof(FaxingStatus), noncardTransaction.FaxingStatus),
                              TransactionDate = noncardTransaction.TransactionDate,
                              TransactionId = noncardTransaction.Id,
                              MFCNNumber = noncardTransaction.MFCN,
                              FaxReceiptNumber = noncardTransaction.ReceiptNumber,
                              DateTime = noncardTransaction.TransactionDate,
                              FaxingFee = noncardTransaction.FaxingFee.ToString(),
                              NameOfAgency = joined == null ? " " : joined.Agent.Name,
                              AgencyMFSCode = joined == null ? " " : joined.Agent.AccountNo,
                              NameOfRefunder = CommonService.getStaffName(Common.StaffSession.LoggedStaff.StaffId),
                              FaxingCurrency = CommonService.getCurrency(data.Country),
                              FaxingCurrencySymbol = CommonService.getCurrencySymbol(data.Country),
                              OperatingUserType = noncardTransaction.OperatingUserType
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
                return null;
            }
            feeSummary = SEstimateFee.CalculateFaxingFee(FaxedAmount, true, false, exchangeRateObj.CountryRate1, SEstimateFee.GetFaxingCommision(FaxingCountry));
            //Common.FaxerSession.FaxingAmountSummary = feeSummary;

            string FaxingFee = String.Format("{0:n}", feeSummary.FaxingFee);


            return FaxingFee;

        }
        public DB.FaxingNonCardTransaction GetFaxingNonCardTransactionDetails(string MFCN)
        {

            var result = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            return result;
        }

        public bool RefundFaxMoney(DB.ReFundNonCardFaxMoneyByAdmin reFund)
        {
            dbContext.ReFundNonCardFaxMoneyByAdmins.Add(reFund);
            dbContext.SaveChanges();

            return true;

        }
        internal string GetNewRefundReceiptNumber()
        {
            //this code should be unique and random with 8 digit length
            var val = "Ad-Tr-MF" + Common.Common.GenerateRandomDigit(5);

            while (dbContext.UserCardWithdrawl.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Ad-Tr-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }
        public bool UpdateStatusofFax(string MFCN)
        {

            var result = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            result.FaxingStatus = DB.FaxingStatus.Refund;
            dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;

        }

        public FaxingNonCardTransaction getDetailsFromMFCN(string MFCN)
        {
            if (MFCN != null)
            {
                var result = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();

                return result;
            }
            return null;
        }
        public TopUpSomeoneElseCardTransaction getDetailsFromId( int id)
        {
            if (id != 0)
            {
                var result = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.Id == id).FirstOrDefault();

                return result;
            }
            return null;
        }

        public AdminRefundReceiptViewModel getAdminRefundReceiptDetails(string MFCN)
        {
            var transactionData = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            var faxerDetails = dbContext.FaxerInformation.Where(x => x.Id == transactionData.NonCardReciever.FaxerID).FirstOrDefault();
            var adminRefundData = dbContext.ReFundNonCardFaxMoneyByAdmins.Where(x => x.NonCardTransaction_id == transactionData.Id).FirstOrDefault();
            var receiptData = new AdminRefundReceiptViewModel()
            {
                ReceiptNumber = adminRefundData.ReceiptNumber,
                TransactionReceiptNumber = transactionData.ReceiptNumber,
                Date = adminRefundData.RefundedDate.ToFormatedString(),
                Time = adminRefundData.RefundedDate.ToString("HH:mm"),
                SenderFullName = faxerDetails.FirstName + " " + faxerDetails.MiddleName + " " + faxerDetails.LastName,
                MFCN = MFCN,
                ReceiverFullName = transactionData.NonCardReciever.FirstName + " " + transactionData.NonCardReciever.MiddleName + " " + transactionData.NonCardReciever.LastName,
                Telephone = transactionData.NonCardReciever.PhoneNumber,
                RefundingAdminName = adminRefundData.StaffInformation.FirstName + " " + adminRefundData.StaffInformation.MiddleName + " " + adminRefundData.StaffInformation.LastName,
                RefundingAdminCode = adminRefundData.StaffInformation.StaffMFSCode,
                OrignalAmountSent = transactionData.FaxingAmount.ToString(),
                RefundedAmount = transactionData.FaxingAmount.ToString(),
                SendingCurrency = CommonService.getCurrency(faxerDetails.Country),
                ReceivingCurrency = CommonService.getCurrency(transactionData.NonCardReciever.Country),
                StaffLoginCode = dbContext.StaffLogin.Where(x => x.StaffId == adminRefundData.Staff_id).Select(x => x.LoginCode).FirstOrDefault()
            };
            return receiptData;
        }

        public AgentRefundReceiptViewModel getAgentRefundReceiptDetails(string MFCN)
        {
            var transactionData = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            var faxerDetails = dbContext.FaxerInformation.Where(x => x.Id == transactionData.NonCardReciever.FaxerID).FirstOrDefault();
            var agentRefundData = dbContext.RefundNonCardFaxMoneyByAgent.Where(x => x.NonCardTransaction_id == transactionData.Id).FirstOrDefault();

            var receiptData = new AgentRefundReceiptViewModel()
            {
                ReceiptNumber = agentRefundData.ReceiptNumber,
                TransactionReceiptNumber = transactionData.ReceiptNumber,
                Date = agentRefundData.RefundedDate.ToFormatedString(),
                Time = agentRefundData.RefundedDate.ToString("HH:mm"),
                SenderFullName = faxerDetails.FirstName + " " + faxerDetails.MiddleName + " " + faxerDetails.LastName,
                MFCN = MFCN,
                ReceiverFullName = transactionData.NonCardReciever.FirstName + " " + transactionData.NonCardReciever.MiddleName + " " + transactionData.NonCardReciever.LastName,
                Telephone = transactionData.NonCardReciever.PhoneNumber,
                RefundingAgentName = agentRefundData.AgentInformation.Name,
                RefundingAgentCode = agentRefundData.AgentInformation.AccountNo,
                OrignalAmountSent = transactionData.FaxingAmount.ToString() + CommonService.getCurrencyCodeFromCountry(transactionData.NonCardReciever.FaxerInformation.Country),
                RefundedAmount = transactionData.FaxingAmount.ToString() + CommonService.getCurrencyCodeFromCountry(transactionData.NonCardReciever.FaxerInformation.Country)
            };
            return receiptData;
        }

        public AdminNonCardMoneyTransferViewModel getAdminTransferData(string MFCN)
        {
            var transactionData = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            AdminNonCardMoneyTransferViewModel receiptData = new AdminNonCardMoneyTransferViewModel()
            {
                MFReceiptNumber = transactionData.ReceiptNumber,
                TransactionDate = transactionData.TransactionDate.ToFormatedString(),
                TransactionTime = transactionData.TransactionDate.ToString("HH:mm"),
                FaxerFullName = transactionData.NonCardReciever.FaxerInformation.FirstName + " " + transactionData.NonCardReciever.FaxerInformation.MiddleName + " " + transactionData.NonCardReciever.FaxerInformation.LastName,
                MFCN = transactionData.MFCN,
                ReceiverFullName = transactionData.NonCardReciever.FirstName + " " + transactionData.NonCardReciever.MiddleName + " " + transactionData.NonCardReciever.LastName,
                Telephone = transactionData.NonCardReciever.PhoneNumber,
                SenderTelephoneNo = Common.Common.GetCountryPhoneCode(transactionData.NonCardReciever.FaxerInformation.Country) + " " + transactionData.NonCardReciever.FaxerInformation.PhoneNumber,
                StaffLoginCode = CommonService.getStaffLoginCode(transactionData.UserId),
                StaffName = CommonService.getStaffName(transactionData.UserId),
                StaffCode = CommonService.getStaffMFSCode(transactionData.UserId),
                AmountSent = transactionData.FaxingAmount.ToString(),
                ExchangeRate = transactionData.ExchangeRate.ToString(),
                Fee = transactionData.FaxingFee.ToString(),
                AmountReceived = transactionData.ReceivingAmount.ToString(),
                TotalAmountSentAndFee = (transactionData.FaxingAmount + transactionData.FaxingFee).ToString(),
                SendingCurrency = CommonService.getCurrencyCodeFromCountry(transactionData.NonCardReciever.FaxerInformation.Country),
                ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(transactionData.NonCardReciever.Country)
            };
            return receiptData;
        }

        public AgentMoneySenderReceiptViewModel getAgentTransferData(string MFCN)
        {
            var transactionData = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            var agentDetails = dbContext.AgentFaxMoneyInformation.Where(x => x.NonCardTransactionId == transactionData.Id).FirstOrDefault();
            string Agentname = "";
            string AgentCode = "";
            if (agentDetails != null)
            {
                Agentname = CommonService.getAgentName(agentDetails.AgentId);
                AgentCode = agentDetails.Agent.AccountNo;
            }


            AgentMoneySenderReceiptViewModel receiptData = new AgentMoneySenderReceiptViewModel()
            {
                MFReceiptNumber = transactionData.ReceiptNumber,
                TransactionDate = transactionData.TransactionDate.ToFormatedString(),
                TransactionTime = transactionData.TransactionDate.ToString("HH:mm"),
                FaxerFullName = transactionData.NonCardReciever.FaxerInformation.FirstName + " " + transactionData.NonCardReciever.FaxerInformation.MiddleName + " " + transactionData.NonCardReciever.FaxerInformation.LastName,
                MFCN = transactionData.MFCN,
                ReceiverFullName = transactionData.NonCardReciever.FirstName + " " + transactionData.NonCardReciever.MiddleName + " " + transactionData.NonCardReciever.LastName,
                Telephone = transactionData.NonCardReciever.PhoneNumber,
                AgentName = Agentname,
                AgentCode = AgentCode,
                AmountSent = transactionData.FaxingAmount.ToString() + CommonService.getCurrencyCodeFromCountry(transactionData.NonCardReciever.FaxerInformation.Country),
                ExchangeRate = transactionData.ExchangeRate.ToString(),
                Fee = transactionData.FaxingFee.ToString(),
                AmountReceived = transactionData.ReceivingAmount.ToString() + CommonService.getCurrencyCodeFromCountry(transactionData.NonCardReciever.Country),
                TotalAmountSentAndFee = (transactionData.FaxingAmount + transactionData.FaxingFee).ToString() + CommonService.getCurrencyCodeFromCountry(transactionData.NonCardReciever.FaxerInformation.Country)
            };
            return receiptData;
        }

        public NonCardUserReceiverReceiptViewModel getFaxerTransferData(string MFCN)
        {
            var transactionData = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            NonCardUserReceiverReceiptViewModel receiptData = new NonCardUserReceiverReceiptViewModel()
            {
                MFReceiptNumber = transactionData.ReceiptNumber,
                TransactionDate = transactionData.TransactionDate.ToFormatedString(),
                TransactionTime = transactionData.TransactionDate.ToString("HH:mm"),
                FaxerFullName = transactionData.NonCardReciever.FaxerInformation.FirstName + " " + transactionData.NonCardReciever.FaxerInformation.MiddleName + " " + transactionData.NonCardReciever.FaxerInformation.LastName,
                MFCN = transactionData.MFCN,
                ReceiverFullName = transactionData.NonCardReciever.FirstName + " " + transactionData.NonCardReciever.MiddleName + " " + transactionData.NonCardReciever.LastName,
                Telephone = transactionData.NonCardReciever.PhoneNumber,
                AgentName = "",
                AgentCode = "",
                AmountSent = transactionData.FaxingAmount.ToString(),
                ExchangeRate = transactionData.ExchangeRate.ToString(),
                Fee = transactionData.FaxingFee.ToString(),
                AmountReceived = transactionData.ReceivingAmount.ToString(),
                SendingCurrency = CommonService.getCurrencyCodeFromCountry(transactionData.NonCardReciever.FaxerInformation.Country),
                ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(transactionData.NonCardReciever.Country)
            };
            return receiptData;
        }


        public List<ViewModels.NonCardFaxingRefundRequestViewModel> NonCardRefundDetails(string CountryCode = "", string City = "")
        {
            var info = new List<DB.ReFundNonCardFaxMoneyByAdmin>();

            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                info = dbContext.ReFundNonCardFaxMoneyByAdmins.Where(x => x.FaxingNonCardTransaction.NonCardReciever.FaxerInformation.Country == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                info = dbContext.ReFundNonCardFaxMoneyByAdmins.Where(x => x.FaxingNonCardTransaction.NonCardReciever.FaxerInformation.City.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                info = dbContext.ReFundNonCardFaxMoneyByAdmins.Where(x => (x.FaxingNonCardTransaction.NonCardReciever.FaxerInformation.City.ToLower() == City.ToLower()) && (x.FaxingNonCardTransaction.NonCardReciever.FaxerInformation.Country == CountryCode)).ToList();
            }





            var result = (from c in info
                          join noncardTransaction in dbContext.FaxingNonCardTransaction on c.NonCardTransaction_id equals noncardTransaction.Id
                          join receiver in dbContext.ReceiversDetails on noncardTransaction.NonCardRecieverId equals receiver.Id
                          join data in dbContext.FaxerInformation on receiver.FaxerID equals data.Id
                          join faxerCountry in dbContext.Country on data.Country equals faxerCountry.CountryCode
                          join receiverCountry in dbContext.Country on receiver.Country equals receiverCountry.CountryCode
                          join Agent in dbContext.AgentFaxMoneyInformation on noncardTransaction.Id equals Agent.NonCardTransactionId
                          into j
                          from joined in j.DefaultIfEmpty()
                          select new ViewModels.NonCardFaxingRefundRequestViewModel()
                          {
                              FaxerId = data.Id,
                              FaxerFirstName = data.FirstName + " " + data.MiddleName + " " + data.LastName,
                              FaxerAddress = data.Address1,
                              FaxerTelephone = CommonService.getPhoneCodeFromCountry(data.Country) + data.PhoneNumber,
                              FaxerCity = data.City,
                              FaxerCountry = faxerCountry.CountryName,
                              FaxerCountryCode = data.Country,
                              FaxerEmail = data.Email,
                              ReceiverFirstName = receiver.FirstName + receiver.MiddleName + receiver.LastName,
                              ReceiverCountryCode = receiver.Country,
                              ReceiverTelephone = receiver.PhoneNumber,
                              FaxingAmount = String.Format("{0:n}", noncardTransaction.FaxingAmount),
                              FaxingCurrency = CommonService.getCurrencyCodeFromCountry(data.Country),
                              FaxingCurrencySymbol = CommonService.getCurrencySymbol(data.Country),
                              StatusOfFax = noncardTransaction.FaxingStatus,
                              MFCNNumber = noncardTransaction.MFCN,
                              FaxReceiptNumber = noncardTransaction.ReceiptNumber,
                              ReceiptNumber = c.ReceiptNumber,
                              NameOfRefunder = c.NameofRefunder,
                              AdminCode = CommonService.getStaffMFSCode(c.Staff_id),
                              RefundReason = c.RefundReason,
                              RefundedDate = c.RefundedDate,
                              RefendedTime = c.RefundedDate.ToString("HH:mm"),
                              FaxingFee = noncardTransaction.FaxingFee.ToString(),
                              ExchangeRate = noncardTransaction.ExchangeRate.ToString(),
                              ReceivedAmount = noncardTransaction.ReceivingAmount.ToString(),
                              NameOfAgency = joined == null ? " " : joined.Agent.Name,
                              AgencyMFSCode = joined == null ? " " : joined.Agent.AccountNo,
                              TransactionDate = noncardTransaction.TransactionDate,
                              ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(receiver.Country)
                          }).OrderByDescending(x => x.RefundedDate).ToList();

            var info2 = new List<DB.RefundNonCardFaxMoneyByAgent>();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                info2 = dbContext.RefundNonCardFaxMoneyByAgent.Where(x => x.FaxingNonCardTransaction.NonCardReciever.FaxerInformation.Country == CountryCode).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                info2 = dbContext.RefundNonCardFaxMoneyByAgent.Where(x => x.FaxingNonCardTransaction.NonCardReciever.FaxerInformation.City.ToLower() == City.ToLower()).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                info2 = dbContext.RefundNonCardFaxMoneyByAgent.Where(x => (x.FaxingNonCardTransaction.NonCardReciever.FaxerInformation.City.ToLower() == City.ToLower()) && (x.FaxingNonCardTransaction.NonCardReciever.FaxerInformation.Country == CountryCode)).ToList();
            }

            var result2 = (from c in info2
                           join noncardTransaction in dbContext.FaxingNonCardTransaction on c.NonCardTransaction_id equals noncardTransaction.Id
                           join receiver in dbContext.ReceiversDetails on noncardTransaction.NonCardRecieverId equals receiver.Id
                           join data in dbContext.FaxerInformation on receiver.FaxerID equals data.Id
                           join faxerCountry in dbContext.Country on data.Country equals faxerCountry.CountryCode
                           join receiverCountry in dbContext.Country on receiver.Country equals receiverCountry.CountryCode
                           select new ViewModels.NonCardFaxingRefundRequestViewModel()
                           {
                               FaxerId = data.Id,
                               FaxerFirstName = data.FirstName + " " + data.MiddleName + " " + data.LastName,
                               FaxerAddress = data.Address1,
                               FaxerTelephone = CommonService.getPhoneCodeFromCountry(data.Country) + data.PhoneNumber,
                               FaxerCity = data.City,
                               FaxerCountry = faxerCountry.CountryName,
                               FaxerCountryCode = data.Country,
                               FaxerEmail = data.Email,
                               ReceiverFirstName = receiver.FirstName + " " + receiver.MiddleName + " " + receiver.LastName,
                               ReceiverCountryCode = receiver.Country,
                               ReceiverTelephone = receiver.PhoneNumber,
                               FaxingAmount = String.Format("{0:n}", noncardTransaction.FaxingAmount),
                               FaxingCurrency = CommonService.getCurrencyCodeFromCountry(data.Country),
                               FaxingCurrencySymbol = CommonService.getCurrencySymbol(data.Country),
                               ExchangeRate = noncardTransaction.ExchangeRate.ToString(),
                               StatusOfFax = noncardTransaction.FaxingStatus,
                               MFCNNumber = noncardTransaction.MFCN,
                               FaxReceiptNumber = noncardTransaction.ReceiptNumber,
                               ReceiptNumber = c.ReceiptNumber,
                               NameoFAgentRefunder = c.NameofRefunder,
                               RefundReason = c.RefundReason,
                               RefundedDate = c.RefundedDate,
                               RefendedTime = c.RefundedDate.ToString("HH:mm"),
                               FaxingFee = noncardTransaction.FaxingFee.ToString(),
                               ReceivedAmount = noncardTransaction.ReceivingAmount.ToString(),
                               TransactionDate = noncardTransaction.TransactionDate,
                               ReceivingCurrency = CommonService.getCurrencyCodeFromCountry(receiver.Country)
                           }).ToList();

            var model = new List<ViewModels.NonCardFaxingRefundRequestViewModel>();
            model = result.Concat(result2).OrderByDescending(x => x.RefundedDate).ToList();
            return model;
        }


    }
}