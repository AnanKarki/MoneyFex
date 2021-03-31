using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using Twilio.Rest.Trunking.V1;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class FundAccountServices
    {
        FAXEREntities dbContext = null;
        CommonServices _CommonServices = null;
        public FundAccountServices()
        {
            dbContext = new FAXEREntities();
            _CommonServices = new CommonServices();
        }
        public List<AgentFundAccount> List()
        {
            var data = dbContext.AgentFundAccount.ToList();
            return data;
        }
        public List<AgentFundAccountViewModel> GetAgentFundAccountList(int AgentId = 0, string DateRange = "", string Country = "", string AuXAgentCode = "",
            string Reference = "", string ResponsiblePerson = "", string Status = "", string Details = "")
        {
            var data = dbContext.AgentFundAccount.ToList();
            if (!string.IsNullOrEmpty(Country))
            {
                data = data.Where(x => x.AgentCountry == Country).ToList();
            }
            if (!string.IsNullOrEmpty(DateRange))
            {

                var Date = DateRange.Split('-');
                var FromDate = DateTime.Parse(Date[0]);
                var ToDate = DateTime.Parse(Date[1]);
                data = data.Where(x => x.DateTime >= FromDate && x.DateTime <= ToDate).ToList();

            }
            if (AgentId != 0)
            {
                data = data.Where(x => x.AgentId == AgentId).ToList();
            }


            var result = (from c in data.ToList()

                          select new AgentFundAccountViewModel()
                          {
                              Id = c.Id,
                              AgentCountry = Common.Common.GetCountryName(c.AgentCountry),
                              AgentCountryFlag = c.AgentCountry.ToLower(),
                              AgentCode = c.AgentId == 0 ? "" : _CommonServices.GetAgentLoginInformation(c.AgentId).LoginCode,
                              ResponsiblePerson = _CommonServices.getStaffName(c.ApprovedBy),
                              AgentCountryCurrency = Common.Common.GetCountryCurrency(c.AgentCountry),
                              AgentId = c.AgentId,
                              AgentName = c.AgentId == 0 ? "" : _CommonServices.GetAgentInformation(c.AgentId).Name,
                              AgentAccountNo = c.AgentId == 0 ? "" : _CommonServices.GetAgentInformation(c.AgentId).AccountNo,
                              Amount = c.Amount,
                              BankAccountNo = c.BankAccountNo,
                              BankSortCode = c.BankSortCode,
                              PaymentReference = c.PaymentReference,
                              Receipt = c.Receipt,
                              Status = c.Status,
                              StatusName = c.Status.ToString(),
                              DateTime = c.DateTime,
                              Date = c.DateTime.ToFormatedString(),
                              SenderPaymentMode = c.SenderPaymentMode,
                              CardProcessorApi = c.CardProcessorApi.HasValue == true ? c.CardProcessorApi.Value : CardProcessorApi.Select,
                              SenderPaymentModeName = Common.Common.GetEnumDescription(c.SenderPaymentMode),
                              FormattedCardNumber = c.CardNumber
                          }).OrderByDescending(x => x.DateTime).ToList();

            if (!string.IsNullOrEmpty(AuXAgentCode))
            {
                AuXAgentCode = AuXAgentCode.Trim();
                result = result.Where(x => x.AgentCode.ToLower().Contains(AuXAgentCode.ToLower())).ToList();

            }

            if (!string.IsNullOrEmpty(Reference))
            {
                Reference = Reference.Trim();
                result = result.Where(x => x.PaymentReference.ToLower().Contains(Reference.ToLower())).ToList();

            }

            if (!string.IsNullOrEmpty(ResponsiblePerson))
            {
                ResponsiblePerson = ResponsiblePerson.Trim();
                result = result.Where(x => x.ResponsiblePerson.ToLower().Contains(ResponsiblePerson.ToLower())).ToList();

            }
            if (!string.IsNullOrEmpty(Status))
            {
                Status = Status.Trim();
                result = result.Where(x => x.StatusName.ToLower().Contains(Status.ToLower())).ToList();

            }
            if (!string.IsNullOrEmpty(Details))
            {
                Details = Details.Trim();
                result = result.Where(x => x.Receipt.ToLower().Contains(Details.ToLower())).ToList();

            }

            return result;
        }



        public List<CityDropDownViewModel> GetCities(string CountryCode = "")
        {
            if (!string.IsNullOrEmpty(CountryCode))
            {
                var result = (from c in dbContext.City.Where(x => x.CountryCode == CountryCode)
                              select new CityDropDownViewModel()
                              {
                                  City = c.Name,

                              }).GroupBy(x => x.City).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }
            else
            {
                var result = (from c in dbContext.City
                              select new CityDropDownViewModel()
                              {

                                  City = c.Name,
                              }).GroupBy(x => x.City).Select(x => x.FirstOrDefault()).ToList();
                return result;
            }


        }

        internal AgentFundAccountViewModel GetFundAccountDetails(int id)
        {
            var data = List().Where(x => x.Id == id).ToList();
            var result = (from c in data
                          select new AgentFundAccountViewModel()
                          {
                              Id = c.Id,
                              AgentCountry = c.AgentCountry,
                              AgentId = c.AgentId,
                              AgentName = c.AgentId == 0 ? "" : _CommonServices.GetAgentInformation(c.AgentId).Name,
                              AgentAccountNo = c.AgentId == 0 ? "" : _CommonServices.GetAgentInformation(c.AgentId).AccountNo,
                              Amount = c.Amount,
                              BankAccountNo = c.BankAccountNo,
                              BankSortCode = c.BankSortCode,
                              PaymentReference = c.PaymentReference,
                              Receipt = c.Receipt,
                              Status = c.Status,
                              StatusName = c.Status.ToString(),
                              IsPaid = c.IsPaid,
                              City = c.City,
                          }).FirstOrDefault();
            return result;
        }

        internal PGTransactionResultVm CheckPaymentStatus(string refno)
        {
            var cardProcessorAPi = dbContext.AgentFundAccount.Where(x => x.Receipt == refno).Select(x => x.CardProcessorApi).FirstOrDefault();
            var result = new PGTransactionResultVm();
            switch (cardProcessorAPi)
            {
                case CardProcessorApi.TrustPayment:
                    var pgTransaction = Common.Common.GetPGRefNo(refno);
                    if (pgTransaction.transactionreference != null || pgTransaction.currencyiso3a != null)
                    {
                        result = StripServices.GetTransactionDetails(pgTransaction.transactionreference, pgTransaction.currencyiso3a);
                    }
                    break;
                case CardProcessorApi.T365:
                    var uid = dbContext.Transact365ApiResponseTransationLog.Where(x => x.TrackingId == refno).Select(x => x.UId).FirstOrDefault();
                    result = Transact365Serivces.GetTranstionStatus(uid , refno);
                    break;
            }
            return result;
        }

        internal void updateFundAccount(AgentFundAccountViewModel vm)
        {

            var data = List().Where(x => x.Id == vm.Id).FirstOrDefault();
            data.AgentCountry = vm.AgentCountry;
            data.AgentId = vm.AgentId;
            data.BankAccountNo = vm.BankAccountNo;
            data.City = vm.City;
            data.Amount = vm.Amount;
            data.Status = vm.Status;
            data.IsPaid = vm.IsPaid;
            data.ApprovedBy = vm.ApprovedBy;
            dbContext.Entry<AgentFundAccount>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            if (data.Status == AgentFundStatus.Approved)
            {
                AddAgentAccountBalance(data);
            }
        }

        public void SetAccountFund(AgentFundAccountViewModel vm)
        {

            Common.AgentSession.AgentFundAccount = vm;
        }

        public AgentFundAccountViewModel GetAccountFund()
        {

            AgentFundAccountViewModel vm = new AgentFundAccountViewModel();

            if (Common.AgentSession.AgentFundAccount != null)
            {

                vm = Common.AgentSession.AgentFundAccount;
            }
            return vm;
        }

        public AgentFundAccountViewModel GetBankDetails()
        {
            AgentFundAccountViewModel vm = new AgentFundAccountViewModel();
            var AmountDetails = GetAccountFund();
            var bankdetails = dbContext.BankAccount.Where(x => x.CountryCode == AmountDetails.AgentCountry).FirstOrDefault();
            var agentInfo = Common.AgentSession.AgentInformation;
            vm.Amount = AmountDetails.Amount;
            vm.AgentCountryCurrency = AmountDetails.AgentCountryCurrency;
            vm.City = agentInfo.City;
            vm.AgentCountryCurrencySymbol = AmountDetails.AgentCountryCurrencySymbol;
            vm.AgentCountry = AmountDetails.AgentCountry;
            vm.AgentId = AmountDetails.AgentId;
            vm.BankAccountNo = bankdetails.AccountNo;
            vm.BankSortCode = bankdetails.LabelValue;
            vm.PaymentReference = Common.Common.GenerateFundAccountPaymentRefrence();
            vm.Receipt = AmountDetails.Receipt;
            vm.SenderPaymentMode = SenderPaymentMode.MoneyFexBankAccount;
            SetAccountFund(vm);
            return vm;
        }

        public BankAccount GetMFBankDetails(string Country)
        {
            var bankdetails = dbContext.BankAccount.Where(x => x.CountryCode == Country).FirstOrDefault();
            return bankdetails;
        }

        internal void AddFundInAccount(AgentFundAccountViewModel vm)
        {

            AgentFundAccount model = new AgentFundAccount()
            {
                AgentCountry = vm.AgentCountry,
                AgentId = vm.AgentId,
                Amount = vm.Amount,
                BankAccountNo = vm.BankAccountNo,
                BankSortCode = vm.BankSortCode,
                DateTime = DateTime.Now,
                PaymentReference = vm.PaymentReference,
                Status = vm.Status,
                Receipt = vm.Receipt,
                IsPaid = vm.IsPaid,
                City = vm.City,
                ApprovedBy = vm.ApprovedBy,
                SenderPaymentMode = vm.SenderPaymentMode,
                CardNumber = vm.FormattedCardNumber,
                CardProcessorApi = vm.CardProcessorApi

            };

            dbContext.AgentFundAccount.Add(model);
            dbContext.SaveChanges();
            if (model.Status == AgentFundStatus.Approved)
            {
                AddAgentAccountBalance(model);
            }
        }

        public void Approved(int id, int StaffId)
        {
            var data = List().Where(x => x.Id == id).FirstOrDefault();
            data.Status = AgentFundStatus.Approved;
            data.ApprovedBy = StaffId;

            dbContext.Entry<AgentFundAccount>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            AddAgentAccountBalance(data);
        }

        private void AddAgentAccountBalance(AgentFundAccount data)
        {
            AgentAccountBalance model = new AgentAccountBalance()
            {
                AgentId = data.AgentId,
                TotalBalance = data.Amount,
                UpdateDateTime = DateTime.Now
            };
            var accountBalance = dbContext.AgentAccountBalance.Where(x => x.AgentId == data.AgentId).FirstOrDefault();
            if (accountBalance != null)
            {

                accountBalance.UpdateDateTime = DateTime.Now;
                accountBalance.TotalBalance = accountBalance.TotalBalance + model.TotalBalance;
                dbContext.Entry(accountBalance).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
            }
            else
            {
                dbContext.AgentAccountBalance.Add(model);
                dbContext.SaveChanges();
            }
        }

        public void SetDebitCreditCardDetail(CreditDebitCardViewModel vm)
        {

            Common.AgentSession.CreditDebitDetails = vm;
        }

        public CreditDebitCardViewModel GetDebitCreditCardDetail()
        {

            CreditDebitCardViewModel vm = new CreditDebitCardViewModel();

            if (Common.AgentSession.CreditDebitDetails != null)
            {

                vm = Common.AgentSession.CreditDebitDetails;
            }
            return vm;
        }



    }
}