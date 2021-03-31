using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class AgentFaxMoneyServices
    {
        DB.FAXEREntities dbContext = new DB.FAXEREntities();
        CommonServices CommonService = new CommonServices();
        public EstimateFaxingFeeSummary getCalculateDetails(string FaxingCountry, string ReceivingCountry, Decimal FaxAmount, decimal receivingAmount)
        {


            var feeSummary = new EstimateFaxingFeeSummary();
            decimal exchangeRate = 0;
            var exchangeRateObj = dbContext.ExchangeRate.Where(x => x.CountryCode1 == FaxingCountry && x.CountryCode2 == ReceivingCountry).FirstOrDefault();
            if (exchangeRateObj == null)
            {
                var exchangeRateObj2 = dbContext.ExchangeRate.Where(x => x.CountryCode1 == ReceivingCountry && x.CountryCode2 == FaxingCountry).FirstOrDefault();
                if (exchangeRateObj2 != null)
                {
                    exchangeRateObj = exchangeRateObj2;
                    exchangeRateObj.CountryRate1 = Math.Round(1 / exchangeRateObj2.CountryRate1, 6, MidpointRounding.AwayFromZero);
                }

            }
            if (exchangeRateObj != null)
            {

                exchangeRate = exchangeRateObj.CountryRate1;
            }
            if (ReceivingCountry.ToLower() == FaxingCountry.ToLower())
            {

                exchangeRate = 1m;
            }
            if (exchangeRate == 0)
            {
                return null;

            }

           
            feeSummary = SEstimateFee.CalculateFaxingFee(((receivingAmount > 0) ? receivingAmount : FaxAmount), true, receivingAmount > 0, exchangeRate, SEstimateFee.GetFaxingCommision(FaxingCountry)); //+ 0.01m

            return feeSummary;

        }



        public Models.AgentFaxMoneyViewModel getFaxer(string AccountNoORPHoneNo, Models.AgentFaxMoneyViewModel vm)
        {
            var data = (from c in dbContext.FaxerInformation.Where(x => x.AccountNo == AccountNoORPHoneNo || x.PhoneNumber == AccountNoORPHoneNo).ToList()
                        select new Models.AgentFaxMoneyViewModel()
                        {
                            FaxerId = c.Id,
                            FaxerFirstName = c.FirstName,
                            FaxerMiddleName = c.MiddleName,
                            FaxerLastName = c.LastName,
                            FaxerAddress = c.Address1,
                            FaxerCity = c.City,
                            FaxerCountry = c.Country,
                            IdCardExpiringDate = c.IdCardExpiringDate,
                            IdNumber = c.IdCardNumber,
                            IssuingCountryCode = c.IssuingCountry,
                            IdentificationTypeId = c.IdCardType,
                            FaxerEmail = c.Email,
                            FaxerTelephone = c.PhoneNumber,
                            FaxerCountryPhoneCode = Common.Common.GetCountryPhoneCode(c.Country),
                            FaxerSearched = true,
                            AgencyMFSCode = vm.AgencyMFSCode,
                            NameOfAgency = vm.NameOfAgency,
                            AgentId = vm.AgentId,
                            DateTime = DateTime.Now,
                            FaxingCountry = c.Country,
                            FaxerCurrency = CommonService.getCurrency(c.Country),
                            FaxerCurrencySymbol = CommonService.getCurrencySymbol(c.Country),
                            FaxerDOB = c.DateOfBirth,
                            FaxerGender = c.GGender == 0 ?"Male" : c.GGender == 1 ? "Female" : "Others"
                        }).FirstOrDefault();

            return data;
        }
        public List<ReceiversDetails> getExistingReceiver(int faxerid)
        {

            var data = dbContext.ReceiversDetails.Where(x => x.FaxerID == faxerid).ToList();

            //var data = (from c in dbContext.ReceiversDetails.Where(x => x.FaxerID == faxerid).ToList()
            //            select new ReceiversDetails()
            //            {
            //                Id  = c.Id ,
            //                FirstName = c.FirstName + " " + c.MiddleName + " " + c.LastName
            //            }).ToList();
            return data;
        }
        public DB.ReceiversDetails getReceiverDetails(int receiverId)
        {

            var data = dbContext.ReceiversDetails.Where(x => x.Id == receiverId).FirstOrDefault();


            //    Models.AgentFaxMoneyViewModel  model = new Models.AgentFaxMoneyViewModel() {
            //    ReceiverFirstName = data.FirstName,
            //    ReceiverMiddleName = data.MiddleName,
            //    ReceiverLastName = data.LastName,
            //    ReceiverAddress = data.EmailAddress,
            //    ReceiverTelephone = data.PhoneNumber,
            //    ReceiverCountry = data.Country,
            //    ReceiverCity = data.City
            //};

            return data;

        }
        //Check faxer Email Already Exist or NOt 
        public bool getFaxerEmail(string email)
        {
            var data = dbContext.FaxerInformation.Where(x => x.Email == email).FirstOrDefault();
            if (data != null)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        /// <summary>
        /// Add Faxer details , Receiver details , Faxing Non Card Transaction Details
        /// </summary>
        /// <returns></returns>
        public DB.FaxingNonCardTransaction FaxNonCardTransactionByAgent(Models.AgentFaxMoneyViewModel vm)
        {
            // Faxer Details
            var FaxerExist = dbContext.FaxerInformation.Where(x => (x.Id == vm.FaxerId) && (x.Email == vm.FaxerEmail)).FirstOrDefault();
            FAXER.PORTAL.Services.SFaxingNonCardTransaction getMFCN = new Services.SFaxingNonCardTransaction();

            SFaxerSignUp faxerSignUpService = new SFaxerSignUp(); 
            string accountNo = "AMF" + faxerSignUpService.GetNewAccount(6);

            string FaxerCountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.FaxerCountry);
            if (FaxerExist == null)
            {
                DB.FaxerInformation FaxerDetails = new DB.FaxerInformation()
                {
                    FirstName = vm.FaxerFirstName,
                    MiddleName = vm.FaxerMiddleName,
                    LastName = vm.FaxerLastName,
                    Address1 = vm.FaxerAddress,
                    City = vm.FaxerCity,
                    Country = vm.FaxerCountry,
                    Email = vm.FaxerEmail,
                    PhoneNumber = FaxerCountryPhoneCode + vm.FaxerTelephone,
                    IdCardNumber = vm.IdNumber,
                    IdCardType = vm.IdentificationTypeId,
                    IssuingCountry = vm.IssuingCountryCode,
                    RegisteredByAgent = true,
                    IsDeleted = false,
                    IdCardExpiringDate = vm.IdCardExpiringDate,
                    AccountNo = accountNo
                };


                dbContext.FaxerInformation.Add(FaxerDetails);
                dbContext.SaveChanges();

            }
            else
            {
                FaxerExist.IdCardNumber = vm.IdNumber;
                FaxerExist.IdCardType = vm.IdentificationTypeId;
                FaxerExist.IdCardExpiringDate = vm.IdCardExpiringDate;
                FaxerExist.IssuingCountry = vm.IssuingCountryCode;

                dbContext.Entry(FaxerExist).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            var FaxerInfo = dbContext.FaxerInformation.Where(x => x.Email == vm.FaxerEmail).FirstOrDefault();
            if (FaxerInfo != null)
            {
                int faxerid = FaxerInfo.Id;
                string ReceiverCountryPhoneCode = Common.Common.GetCountryPhoneCode(vm.ReceiverCountry);

                // Receiver Details
                DB.ReceiversDetails receiversDetails = new DB.ReceiversDetails()
                {
                    FirstName = vm.ReceiverFirstName,
                    MiddleName = vm.ReceiverMiddleName ?? "",
                    LastName = vm.ReceiverLastName,
                    City = vm.ReceiverCity,
                    Country = vm.ReceiverCountry,
                    CreatedDate = DateTime.Now,
                    EmailAddress = vm.ReceiverEmailAddress,
                    PhoneNumber = ReceiverCountryPhoneCode + vm.ReceiverTelephone,
                    FaxerID = faxerid,
                };
                var nonCardReceiverExist = dbContext.ReceiversDetails.Where(x => x.EmailAddress == vm.ReceiverEmailAddress).FirstOrDefault();
                if (nonCardReceiverExist == null)
                {
                    dbContext.ReceiversDetails.Add(receiversDetails);
                    dbContext.SaveChanges();
                }

            }
            var nonCardReceiver = dbContext.ReceiversDetails.Where(x => x.EmailAddress == vm.ReceiverEmailAddress).FirstOrDefault();
            if (nonCardReceiver != null)
            {

                // Faxing NOn card Transaction
                var MFCN = getMFCN.GetNewMFCNToSave();
                var receiptNumber = GetNewAgentMoneyTransferReceipt();

                DB.FaxingNonCardTransaction nonCardTransaction = new DB.FaxingNonCardTransaction()
                {
                    FaxingAmount = vm.FaxedAmount,
                    FaxingFee = Decimal.Parse(vm.FaxingFee),
                    ExchangeRate = Decimal.Parse(vm.CurrentExchangeRate),
                    TransactionDate = DateTime.Now,
                    ReceivingAmount = vm.RecevingAmount,
                    NonCardRecieverId = nonCardReceiver.Id,
                    MFCN = MFCN,
                    ReceiptNumber = receiptNumber,
                    OperatingUserType = OperatingUserType.Agent


                };
                dbContext.FaxingNonCardTransaction.Add(nonCardTransaction);
                dbContext.SaveChanges();
                var agentFaxingInformation = new AgentFaxMoneyInformation()
                {
                    AgentId = vm.AgentId,
                    NameOfPayingAgent = vm.PayingAgentName,
                    NonCardTransactionId = nonCardTransaction.Id,
                    Date = nonCardTransaction.TransactionDate,
                    FaxingCountry= vm.FaxingCountry,
                    ReceivingCountry = vm.RecevingCountry,
                    PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId
                };
                dbContext.AgentFaxMoneyInformation.Add(agentFaxingInformation);
                dbContext.SaveChanges();


                #region Bank Account Credit Update 
                BankAccountCreditUpdateServices bankAccountCreditUpdateServices = new BankAccountCreditUpdateServices();
                DB.BaankAccountCreditUpdateByAgent baankAccountCreditUpdateByAgent = new BaankAccountCreditUpdateByAgent()
                {
                    AgentId = Common.AgentSession.AgentInformation.Id,
                    CustomerDeposit = nonCardTransaction.FaxingAmount  ,
                    NameOfUpdater = agentFaxingInformation.NameOfPayingAgent,
                    NonCardTransactionId = nonCardTransaction.Id,
                    CreatedDateTime = DateTime.Now,
                    CustomerDepositFees =nonCardTransaction.FaxingFee,
                    ReceiptNo = bankAccountCreditUpdateServices.GetReceiptNoForBankAccountDeposit()

                };
                dbContext.BaankAccountCreditUpdateByAgent.Add(baankAccountCreditUpdateByAgent);
                dbContext.SaveChanges();

                #endregion

                if (!string.IsNullOrEmpty(vm.FaxerEmail))
                {
                    // Send Email For Cofirmation of Moneyfaxed
                    MailCommon mail = new MailCommon();

                    var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                    string FaxerName = vm.FaxerFirstName + " " + vm.FaxerMiddleName + " " + vm.FaxerLastName;
                    string FaxerEmail = vm.FaxerEmail;
                    string body = "";
                    string ReceiverName = vm.ReceiverFirstName + " " + vm.ReceiverMiddleName + " " + vm.ReceiverLastName;
                    string ReceiverCity = vm.ReceiverCity;
                    string FaxerCountry = Common.Common.GetCountryName(vm.FaxerCountry);
                    string RegisterMFTCLink = baseUrl + "/TopUpRegestration/UserContactDetails";
                    body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/ConfirmationOfMoneyFaxedWithReceipt?FaxerName=" + FaxerName +
                        "&ReceiverName=" + ReceiverName + "&ReceiverCity=" + ReceiverCity
                        + "&MFCN=" + MFCN + "&FaxAmount=" + vm.FaxedAmount + "&RegisterMFTC=" + RegisterMFTCLink + "&FaxerCountry=" + FaxerCountry);

                    //mail.SendMail(FaxerEmail, "Confirmation of Money Faxed with MFCN", body);
                    string CountryCode = "+" + Common.Common.GetCountryPhoneCode(vm.RecevingCountry);
                    string FaxerCurrency = Common.Common.GetCountryCurrency(Common.AgentSession.AgentInformation.CountryCode);
                    string ReceiverCurrency = Common.Common.GetCountryCurrency(vm.RecevingCountry);
                    string URL = baseUrl + "/EmailTemplate/AgentMoneySenderReceipt?MFReceiptNumber=" + nonCardTransaction.ReceiptNumber +
                        "&TransactionDate=" + nonCardTransaction.TransactionDate.ToString("dd/MM/yyy") + "&TransactionTime=" + nonCardTransaction.TransactionDate.ToString("HH:mm")
                          + "&FaxerFullName=" + vm.FaxerFirstName + " " + vm.FaxerMiddleName + " " + vm.FaxerLastName +
                        "&MFCN=" + nonCardTransaction.MFCN + "&ReceiverFullName=" + vm.ReceiverFirstName + " " + vm.ReceiverMiddleName + " " + vm.ReceiverLastName
                        + "&Telephone=" + CountryCode + " " + vm.ReceiverTelephone + "&AgentName=" + vm.NameOfAgency + "&AgentCode=" + vm.AgencyMFSCode + "&AmountSent=" + FaxerCurrency + " " + vm.FaxedAmount
                        + "&ExchangeRate=" + vm.CurrentExchangeRate + "&Fee=" + vm.FaxingFee +
                        "&AmountReceived=" + vm.RecevingAmount + "&TotalAmountSentAndFee=" + vm.TotalAmountIncludingFee + "&SendingCurrency=" + FaxerCurrency + "&ReceivingCurrency=" + ReceiverCurrency;

                    var output = Common.Common.GetPdf(URL);

                    mail.SendMail(FaxerEmail, "Confirmation of Money Transfer", body, output);


                    #region Cash To Cash Transfer SMS


                    SmsApi smsApiServices = new SmsApi();

                    string sentAmount = Common.Common.GetCurrencySymbol(vm.FaxerCountry) + vm.FaxedAmount;
                    string Fee = Common.Common.GetCurrencySymbol(vm.FaxerCountry) + vm.FaxingFee;
                    string message = smsApiServices.GetCashToCashTransferMessage(FaxerName, MFCN, Common.Common.GetCurrencySymbol(vm.FaxerCountry) + vm.FaxedAmount , Fee);
                    long SenderPhonenoToLong = 0;
                    if (!string.IsNullOrEmpty( vm.FaxerTelephone)){
                        SenderPhonenoToLong = long.Parse(vm.FaxerTelephone);
                    }
                    
                    string PhoneNo = Common.Common.GetCountryPhoneCode(vm.FaxerCountry) + "" +  SenderPhonenoToLong.ToString();
                    smsApiServices.SendSMS(PhoneNo, message);
                    string ReceiverFUllName = vm.ReceiverFirstName + " " + vm.ReceiverMiddleName + " " + vm.ReceiverLastName;

                    long ReceiverPhonenoToLong = 0;
                    if (!string.IsNullOrEmpty(vm.ReceiverTelephone)) {
                        ReceiverPhonenoToLong = long.Parse(vm.ReceiverTelephone);
                    }
                    string ReceiverPhoneNo = Common.Common.GetCountryPhoneCode(vm.ReceiverCountry) + " " + ReceiverPhonenoToLong.ToString();

                   
                    smsApiServices.SendSMS(ReceiverPhoneNo, message);

                    #endregion


                }

                // End
                return nonCardTransaction;
            }
            else
            {
                return null;
            }
        }

        internal string GetNewAgentMoneyTransferReceipt()
        {


            //this code should be unique and random with 8 digit length
            var val = "Ag-Tp-MF" + Common.Common.GenerateRandomDigit(5);

            while (dbContext.ReceiverNonCardWithdrawl.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Ag-Tp-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }


    }
}
