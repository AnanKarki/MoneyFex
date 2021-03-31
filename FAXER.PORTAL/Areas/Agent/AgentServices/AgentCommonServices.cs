using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Controllers;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class AgentCommonServices
    {
        FAXEREntities db = null;
        public AgentCommonServices()
        {
            db = new FAXEREntities();
        }

        public decimal getAgentAccountBalance(int agentID)
        {
            var data = (from c in db.BaankAccountCreditUpdateByAgent.Where(x => x.AgentId == agentID)
                        select c).ToList();
            decimal CurrentBankDeposit = data.Select(x => (decimal?)x.BankDeposit).Sum() ?? 0;
            decimal CurrentCustomerDeposit = data.Select(x => (decimal?)x.CustomerDeposit).Sum() ?? 0;
            decimal CustomerDepositFee = data.Select(x => (decimal?)x.CustomerDepositFees).Sum() ?? 0;
            decimal NonCardwithdrawalAmount = db.ReceiverNonCardWithdrawl.Where(x => x.AgentId == agentID).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            decimal MFBCCardWithdrawalAmount = db.MFBCCardWithdrawls.Where(x => x.AgentInformationId == agentID).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            decimal MFTCCardWithdrawalAmount = db.UserCardWithdrawl.Where(x => x.AgentInformationId == agentID).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            decimal CardUserNonCardWithdrawalAmount = db.CardUserNonCardWithdrawal.Where(x => x.AgentId == agentID).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
            decimal MerchantNonCardwithdrawalAmount = db.MerchantNonCardWithdrawal.Where(x => x.AgentId == agentID).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;

            var TotalAmountDeposited = ((CurrentBankDeposit + CurrentCustomerDeposit + CustomerDepositFee) - (NonCardwithdrawalAmount +
               MFBCCardWithdrawalAmount + MFTCCardWithdrawalAmount + CardUserNonCardWithdrawalAmount + MerchantNonCardwithdrawalAmount));
            return TotalAmountDeposited;
        }

        public decimal getAuxAgentAccountBalance(int AgentId, int PayingStaffId)
        {
            //decimal TotalBalanceOfAUXAgent = 0;
            //decimal TransferAmount = 0;
            //decimal BalanceOfAUXAgent = db.AgentAccountBalance.Where(x => x.AgentId == AgentId).Select(x => (decimal?)x.TotalBalance).Sum() ?? 0;
            //decimal bankDeposit = db.BankAccountDeposit.Where(x => x.PayingStaffId == PayingStaffId).Select(x => (decimal?)x.TotalAmount).Sum() ?? 0;
            //decimal CashPickUp = db.FaxingNonCardTransaction.Where(x => x.PayingStaffId == PayingStaffId).Select(x => (decimal?)x.TotalAmount).Sum() ?? 0;
            //decimal OtherWallet = db.MobileMoneyTransfer.Where(x => x.PayingStaffId == PayingStaffId).Select(x => (decimal?)x.TotalAmount).Sum() ?? 0;
            //decimal KiiPay = db.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == PayingStaffId).Select(x => (decimal?)x.TotalAmount).Sum() ?? 0;

            //TransferAmount = bankDeposit + CashPickUp + OtherWallet + KiiPay;
            //TotalBalanceOfAUXAgent = BalanceOfAUXAgent - TransferAmount;
            //return TotalBalanceOfAUXAgent;

            decimal TotalBalanceOfAUXAgent = db.AgentAccountBalance.Where(x => x.AgentId == AgentId).Select(x => (decimal?)x.TotalBalance).Sum() ?? 0;
            return TotalBalanceOfAUXAgent;


        }

        public List<DropDownCardTypeViewModel> GetIDCardTypes()
        {
            var result = (from c in db.IdentityCardType
                          select new DropDownCardTypeViewModel()
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
        public string GetStaffLoginCode()
        {

            var code = Common.Common.GenerateRandomDigit(6);
            while (db.AgentStaffLogin.Where(x => x.StaffLoginCode == code).Count() > 0)
            {

                code = Common.Common.GenerateRandomDigit(6);
            }

            return code;
        }

        internal List<ExchangeRateSettingViewModel> GetExchanegRateList(int agentId)
        {
            DB.FAXEREntities dbContext = new FAXEREntities();
            var data = dbContext.ExchangeRate.Where(x => x.AgentId == agentId).ToList();

            var result = data.ToList().GroupBy(x => new
            {
                x.RecevingCurrency,
                x.SendingCurrency

            }).Select(d => new ExchangeRateSettingViewModel()
            {
                DestinationCountryCode = d.FirstOrDefault().CountryCode2,
                SourceCountryCode = d.FirstOrDefault().CountryCode1,
                DestinationCurrencyCode = Common.Common.GetCountryCurrency(d.FirstOrDefault().CountryCode2),
                SourceCurrencyCode = Common.Common.GetCountryCurrency(d.FirstOrDefault().CountryCode1),
                ExchangeRate = Math.Round(d.FirstOrDefault().Rate, 2),
            }).ToList();
            return result;
        }

        public string GetAgentLoginCode()
        {

            var code = Common.Common.GenerateRandomDigit(6);
            while (db.AgentLogin.Where(x => x.LoginCode == code).Count() > 0)
            {

                code = Common.Common.GenerateRandomDigit(6);
            }

            return code;
        }


        public string getAgentContactPersonName(int AgentId)
        {

            var contactPerson = db.AgentInformation.Where(x => x.Id == AgentId).Select(x => x.ContactPerson).FirstOrDefault();
            return contactPerson;
        }

        public string AgentMFSCode()
        {

            var code = Common.Common.GenerateRandomDigit(10);
            while (db.AgentStaffInformation.Where(x => x.AgentMFSCode == code).Count() > 0)
            {

                code = Common.Common.GenerateRandomDigit(6);
            }
            return code;
        }


        public List<AgentStaffInformation> GetAgentStaffInformation()
        {
            var data = db.AgentStaffInformation.ToList();
            return data;
        }

        public List<AgentStaffLogin> GetAgentStaffLoginInfo()
        {

            var data = db.AgentStaffLogin.Where(x => x.AgentStaff.AgentId == Common.AgentSession.AgentInformation.Id).ToList();
            return data;
        }


        public string GetAgentStaffLoginCode()
        {

            var data = db.AgentStaffLogin.Where(x => x.AgentStaffId == Common.AgentSession.LoggedUser.PayingAgentStaffId).FirstOrDefault();
            return data.StaffLoginCode;

        }

        public string GetAgencyLoginCode(int AgentId)
        {

            var data = db.AgentLogin.Where(x => x.AgentId == AgentId).FirstOrDefault();
            return data.LoginCode;
        }



        public decimal Get30DaysAgentCommission(int AgentId = 0)
        {
            DateTime end = DateTime.Now.Date;
            DateTime start = end.AddDays(-30);
            var Commission1 = db.MobileMoneyTransfer.Where(x => x.PayingStaffId == AgentId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.AgentCommission);
            var Commission2 = db.BankAccountDeposit.Where(x => x.PayingStaffId == AgentId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.AgentCommission);
            var Commission3 = db.FaxingNonCardTransaction.Where(x => x.PayingStaffId == AgentId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.AgentCommission);
            var Commission4 = db.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == AgentId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.AgentCommission);
            var Commission5 = db.PayBill.Where(x => x.PayingStaffId == AgentId && DbFunctions.TruncateTime(x.PaymentDate) >= start && DbFunctions.TruncateTime(x.PaymentDate) <= end).ToList().Sum(x => x.AgentCommission);
            var Commission6 = db.TopUpToSupplier.Where(x => x.PayingStaffId == AgentId && DbFunctions.TruncateTime(x.PaymentDate) >= start && DbFunctions.TruncateTime(x.PaymentDate) <= end).ToList().Sum(x => x.AgentCommission);

            var Commission7 = db.UserCardWithdrawl.Where(x => x.PayingAgentStaffId == AgentId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.AgentCommission);
            var Commission8 = db.MFBCCardWithdrawls.Where(x => x.PayingAgentStaffId == AgentId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.AgentCommission);
            var Commission9 = db.ReceiverNonCardWithdrawl.Where(x => x.PayingAgentStaffId == AgentId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.AgentCommission);

            return Commission1 + Commission2 + Commission3 + Commission4 + Commission5 + Commission6 + Commission7 + Commission8 + Commission9;
        }

        public void ClearAgentKiiPayTransferSession()
        {
            HttpContext.Current.Session.Remove("SendMoneToKiiPayWalletViewModel");
            HttpContext.Current.Session.Remove("KiiPayReceiverDetailsInformationViewModel");
            HttpContext.Current.Session.Remove("SendMoneyToKiiPayEnterAmountViewModel");
            HttpContext.Current.Session.Remove("KiiPayTransferPaymentSummary");

        }

        public void ClearAgentBankAccountDeposit()
        {
            HttpContext.Current.Session.Remove("CashPickupInformationViewModel");
            HttpContext.Current.Session.Remove("AgentBankAccountDeposit");
            HttpContext.Current.Session.Remove("BankDepositAbroadEnterAmount");

        }
        public void ClearAgentCashPickUpTransfer()
        {
            HttpContext.Current.Session.Remove("CashPickupInformationViewModel");
            HttpContext.Current.Session.Remove("CashPickUpReceiverDetailsInformationViewModel");
            HttpContext.Current.Session.Remove("CashPickUpEnterAmount");

        }

        public void ClearBecomeAAgent()
        {
            HttpContext.Current.Session.Remove("AgentInformtionViewModel");
            HttpContext.Current.Session.Remove("StaffDetailsViewModel");
            HttpContext.Current.Session.Remove("StaffContaactDetailsViewModel");
            HttpContext.Current.Session.Remove("StaffComplianceDocViewModel");
        }
        public void ClearOtherMobileWallerTransfer()
        {
            HttpContext.Current.Session.Remove("CashPickupInformationViewModel");
            HttpContext.Current.Session.Remove("ReceiverDetailsInformation");
            HttpContext.Current.Session.Remove("MobileMoneyTransferEnterAmount");

        }


        public void ClearPayAReceiverKiiPay()
        {
            HttpContext.Current.Session.Remove("PayAReceiverKiiPayWalletEnteramount");
            HttpContext.Current.Session.Remove("PayAReceiverKiiPayWallet");
            HttpContext.Current.Session.Remove("PayAReceiverKiipayWalletSuccess");
        }
        public void ClearPayAReceiverCashPickUp()
        {
            HttpContext.Current.Session.Remove("PayAReceiveCashPickupReceiverDetails");
            HttpContext.Current.Session.Remove("PayAReceiverCashPickupViewModel");


        }

        public void ClearCommonEnterAmount()
        {
            HttpContext.Current.Session.Remove("CommonEnterAmountViewModel");


        }
        public void ClearSenderId()
        {
            HttpContext.Current.Session.Remove("SenderId");

        }
        public void ClearAgentTransactionSummaryVm()
        {
            HttpContext.Current.Session.Remove("AgentTransactionSummaryVm");
        }


        public void ClearAgentPayBillsMonthly()
        {
            HttpContext.Current.Session.Remove("AgentPayingSupplierReference");
            HttpContext.Current.Session.Remove("PayMonthlyBillViewModel");

        }

        public void ClearAgentPayBillsTopUp()
        {
            HttpContext.Current.Session.Remove("AgentTopUpSupplierEnterAmount");
            HttpContext.Current.Session.Remove("TopUpAnAccountViewModel");
        }


        public void SetAgentPaymentSummarySession(string ReceivingCountry, TransactionTransferMethod transactionTransferMethod = TransactionTransferMethod.BankDeposit)
        {

            SSenderKiiPayWalletTransfer _kiiPaytrasferServices = new SSenderKiiPayWalletTransfer();
            var model = _kiiPaytrasferServices.GetCommonEnterAmount();
            string AgentCountryCode = db.AgentInformation.Where(x => x.Id == Common.AgentSession.LoggedUser.PayingAgentStaffId).Select(x => x.CountryCode).FirstOrDefault();
            var result = SEstimateFee.CalculateFaxingFee(model.SendingAmount, false, false,
             SExchangeRate.GetExchangeRateValue(AgentCountryCode, ReceivingCountry, transactionTransferMethod, Common.AgentSession.AgentInformation.Id, TransactionTransferType.Agent), SEstimateFee.GetFaxingCommision(AgentCountryCode));

            CommonEnterAmountViewModel enterAmount = new CommonEnterAmountViewModel()
            {
                Fee = result.FaxingFee,
                SendingAmount = result.FaxingAmount,
                ReceivingAmount = result.ReceivingAmount,
                TotalAmount = result.TotalAmount,
                ExchangeRate = result.ExchangeRate,
                SendingCurrencySymbol = Common.Common.GetCurrencySymbol(AgentCountryCode),
                ReceivingCurrencySymbol = Common.Common.GetCurrencySymbol(ReceivingCountry),
                SendingCountryCode = AgentCountryCode,
                ReceivingCountryCode = ReceivingCountry,
                SendingCurrency = Common.Common.GetCountryCurrency(AgentCountryCode),
                ReceivingCurrency = Common.Common.GetCountryCurrency(ReceivingCountry),
                AgentCommission = Common.Common.GetAgentSendingCommission(TransferService.KiiPayWallet
                                              , Common.AgentSession.LoggedUser.Id, result.TotalAmount, result.FaxingFee)
            };
            _kiiPaytrasferServices.SetCommonEnterAmount(enterAmount);
        }


    }
}