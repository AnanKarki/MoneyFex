using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class AccountingServices
    {
        FAXEREntities dbContext = null;
        CommonServices _commonServices = null;
        public AccountingServices()
        {
            dbContext = new FAXEREntities();
            _commonServices = new CommonServices();
        }

        public List<AccountingViewModel> GetTransactionStatement(string receivingCountry = "", string date = "",
            int PayingStaffId = 0, int AgentId = 0, bool IsFromAdmin = false)
        {

            var BankDeposit = BankDepositStatement(PayingStaffId, AgentId, IsFromAdmin);
            var CashPickUp = CashPickUpStatement(PayingStaffId, AgentId, IsFromAdmin);
            var Otherwallet = OtherWalletStatement(PayingStaffId, AgentId, IsFromAdmin);
            var kiiPayWallet = KiiPayWalletStatement(PayingStaffId, AgentId, IsFromAdmin);
            var result = BankDeposit.Concat(CashPickUp).Concat(Otherwallet).Concat(kiiPayWallet).OrderByDescending(x => x.DateTime).ToList();
            if (!string.IsNullOrEmpty(receivingCountry))
            {
                result = result.Where(x => x.ReceivingCountryCode == receivingCountry).ToList();
            }
            if (!string.IsNullOrEmpty(date))
            {
                var DateRange = date.Split('-');
                var FromDate = DateTime.Parse(DateRange[0]);
                var ToDate = DateTime.Parse(DateRange[1]);

                result = result.Where(x => x.DateTime >= FromDate && x.DateTime <= ToDate).ToList();
            }

            return result;

        }

        internal int GetNumberofdaysAgentWasCreated(int PayingAgentStaffId)
        {
            int totaldays = 0;
            var agentStaffInformation = dbContext.AgentStaffInformation.Where(x => x.Id == PayingAgentStaffId).FirstOrDefault();
            if (agentStaffInformation != null)
            {
                var Todaysdate = DateTime.Now.Date;
                var agnetCreatedDate = agentStaffInformation.CreatedDate.HasValue == true ? agentStaffInformation.CreatedDate.Value.Date : Todaysdate;
                var days = Todaysdate - agnetCreatedDate;
                totaldays = days.Days + 1;
                if (totaldays > 30)
                {
                    totaldays = 30;
                }
            }
            return totaldays;
        }

        //public decimal GetMFRate(string sendingCountry, string ReceivingCountry, int TransferMethod)
        //{

        //    var data = dbContext.ExchangeRate.Where(x => x.CountryCode1 == sendingCountry && x.CountryCode2 == ReceivingCountry
        //    && x.TransferMethod == (TransactionTransferMethod)TransferMethod).FirstOrDefault();
        //    if (data == null)
        //    {
        //        data = dbContext.ExchangeRate.Where(x => x.CountryCode1 == sendingCountry && x.CountryCode2 == ReceivingCountry
        //        && x.TransferMethod == TransactionTransferMethod.All).FirstOrDefault();
        //        if (data == null)
        //        {
        //            data = dbContext.ExchangeRate.Where(x => x.CountryCode1 == sendingCountry && x.CountryCode2 == ReceivingCountry).FirstOrDefault();
        //        }
        //    }
        //    var Rate = data.Rate;
        //    return Rate;

        //}
        //public decimal GetAgentRate(string sendingCountry, string ReceivingCountry, int TransferMethod, int AgentId)
        //{

        //    var data = dbContext.ExchangeRate.Where(x => x.CountryCode1 == sendingCountry && x.CountryCode2 == ReceivingCountry
        //    && x.TransferMethod == (TransactionTransferMethod)TransferMethod && x.AgentId == AgentId).FirstOrDefault();
        //    if (data == null)
        //    {
        //        data = dbContext.ExchangeRate.Where(x => x.CountryCode1 == sendingCountry && x.CountryCode2 == ReceivingCountry
        //        && x.TransferMethod == TransactionTransferMethod.All && x.AgentId == AgentId).FirstOrDefault();
        //        if (data == null)
        //        {
        //            data = dbContext.ExchangeRate.Where(x => x.CountryCode1 == sendingCountry && x.CountryCode2 == ReceivingCountry && x.AgentId == AgentId).FirstOrDefault();
        //        }
        //    }
        //    var Rate = data.Rate;
        //    return Rate;

        //}
        public List<AccountingViewModel> BankDepositStatement(int PayingStaffId = 0, int AgentId = 0, bool IsFromAdmin = false)
        {
            var payingStaff = dbContext.AgentStaffInformation.Where(x => x.AgentId == AgentId).ToList();
            var data = new List<BankAccountDeposit>();
            if (PayingStaffId > 0)
            {

                data = dbContext.BankAccountDeposit.Where(x => x.PayingStaffId == PayingStaffId).ToList();
            }
            if (AgentId > 0)
            {
                data = (from c in dbContext.BankAccountDeposit.ToList()
                        join d in payingStaff on c.PayingStaffId equals d.Id
                        select c).ToList();
            }
            if (IsFromAdmin)
            {
                var payingAgentStaff = dbContext.AgentStaffInformation;
                data = (from c in dbContext.BankAccountDeposit.ToList()
                        join d in payingAgentStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                        select c).ToList();
            }
            var result = (from c in data.ToList()
                          select new AccountingViewModel()
                          {
                              TransactionId = c.Id,
                              Amount = c.SendingAmount,
                              Fee = c.Fee,
                              DateTime = c.TransactionDate,
                              Identifier = c.ReceiptNo,
                              ReceivingCountryCode = c.ReceivingCountry,
                              ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              Receiver = c.ReceiverName,
                              Sender = _commonServices.GetSenderName(c.SenderId),
                              AgentRate = c.ExchangeRate,
                              SendingCountryCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              TransactionType = TransactionType.BankAccountDeposit,
                              Type = Models.Type.Transfer,
                              Margin = c.Margin,
                              MFRate = c.MFRate
                          }).ToList();
            return result;
        }
        public List<AccountingViewModel> OtherWalletStatement(int PayingStaffId = 0, int AgentId = 0, bool IsFromAdmin = false)
        {

            var payingStaff = dbContext.AgentStaffInformation.Where(x => x.AgentId == AgentId).ToList();
            var data = new List<MobileMoneyTransfer>();
            if (PayingStaffId > 0)
            {

                data = dbContext.MobileMoneyTransfer.Where(x => x.PayingStaffId == PayingStaffId).ToList();
            }
            if (AgentId > 0)
            {
                data = (from c in dbContext.MobileMoneyTransfer.ToList()
                        join d in payingStaff on c.PayingStaffId equals d.Id
                        select c).ToList();
            }
            if (IsFromAdmin)
            {
                var payingAgentStaff = dbContext.AgentStaffInformation;
                data = (from c in dbContext.MobileMoneyTransfer.ToList()
                        join d in payingAgentStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                        select c).ToList();
            }
            var result = (from c in data
                          select new AccountingViewModel()
                          {
                              TransactionId = c.Id,
                              Amount = c.SendingAmount,
                              Fee = c.Fee,
                              DateTime = c.TransactionDate,
                              Identifier = c.ReceiptNo,
                              ReceivingCountryCode = c.ReceivingCountry,
                              ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              Receiver = c.ReceiverName,
                              Sender = _commonServices.GetSenderName(c.SenderId),
                              AgentRate = c.ExchangeRate,
                              SendingCountryCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              TransactionType = TransactionType.BankAccountDeposit,
                              Type = Models.Type.Transfer,
                              Margin = c.Margin,
                              MFRate = c.MFRate
                          }).ToList();
            return result;
        }
        public List<AccountingViewModel> KiiPayWalletStatement(int PayingStaffId = 0, int AgentId = 0, bool IsFromAdmin = false)
        {
            var payingStaff = dbContext.AgentStaffInformation.Where(x => x.AgentId == AgentId).ToList();
            var data = new List<TopUpSomeoneElseCardTransaction>();
            if (PayingStaffId > 0)
            {

                data = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == PayingStaffId).ToList();
            }
            if (AgentId > 0)
            {
                data = (from c in dbContext.TopUpSomeoneElseCardTransaction.ToList()
                        join d in payingStaff on c.PayingStaffId equals d.Id
                        select c).ToList();
            }
            if (IsFromAdmin)
            {
                var payingAgentStaff = dbContext.AgentStaffInformation;
                data = (from c in dbContext.TopUpSomeoneElseCardTransaction.ToList()
                        join d in payingAgentStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                        select c).ToList();
            }
            var result = (from c in data
                          select new AccountingViewModel()
                          {
                              TransactionId = c.Id,
                              Amount = c.FaxingAmount,
                              Fee = c.FaxingFee,
                              DateTime = c.TransactionDate,
                              Identifier = c.ReceiptNumber,
                              ReceivingCountryCode = c.ReceivingCountry,
                              ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              Receiver = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName,
                              Sender = _commonServices.GetSenderName(c.FaxerId),
                              AgentRate = c.ExchangeRate,
                              SendingCountryCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              TransactionType = TransactionType.BankAccountDeposit,
                              Type = Models.Type.Transfer,
                              Margin = c.Margin,
                              MFRate = c.MFRate
                          }).ToList();
            return result;
        }
        public List<AccountingViewModel> CashPickUpStatement(int PayingStaffId = 0, int AgentId = 0, bool IsFromAdmin = false)
        {
            var payingStaff = dbContext.AgentStaffInformation.Where(x => x.AgentId == AgentId).ToList();
            var data = new List<FaxingNonCardTransaction>();
            if (PayingStaffId > 0)
            {

                data = dbContext.FaxingNonCardTransaction.Where(x => x.PayingStaffId == PayingStaffId).ToList();
            }
            if (AgentId > 0)
            {
                data = (from c in dbContext.FaxingNonCardTransaction.ToList()
                        join d in payingStaff on c.PayingStaffId equals d.Id
                        select c).ToList();
            }
            if (IsFromAdmin)
            {
                var payingAgentStaff = dbContext.AgentStaffInformation;
                data = (from c in dbContext.FaxingNonCardTransaction.ToList()
                        join d in payingAgentStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                        select c).ToList();
            }

            var result = (from c in data
                          select new AccountingViewModel()
                          {
                              TransactionId = c.Id,
                              Amount = c.FaxingAmount,
                              Fee = c.FaxingFee,
                              DateTime = c.TransactionDate,
                              Identifier = c.ReceiptNumber,
                              ReceivingCountryCode = c.ReceivingCountry,
                              ReceivingCountry = Common.Common.GetCountryName(c.ReceivingCountry),
                              Receiver = c.NonCardReciever.FullName,
                              Sender = _commonServices.GetSenderName(c.NonCardReciever.FaxerID),
                              AgentRate = c.ExchangeRate,
                              SendingCountryCurrency = Common.Common.GetCountryCurrency(c.SendingCountry),
                              TransactionType = TransactionType.CashPickUp,
                              Type = Models.Type.Transfer,
                              Margin = c.Margin,
                              MFRate = c.MFRate
                          }).ToList();
            return result;
        }

        public decimal get30dayTotalSale(int payingStaffId, int AgentId = 0)

        {
            var payingStaff = dbContext.AgentStaffInformation.Where(x => x.AgentId == AgentId).ToList();
            decimal BankAccountTransferAmount = 0;
            decimal OtherwalletTransferAmount = 0;
            decimal CashPickUpTransferAmount = 0;
            decimal KiiPayWalletTransfer = 0;
            DateTime end = DateTime.Now.Date;
            DateTime start = end.AddDays(-30);

            if (payingStaffId > 0)
            {

                BankAccountTransferAmount = dbContext.BankAccountDeposit.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.SendingAmount);
                OtherwalletTransferAmount = dbContext.MobileMoneyTransfer.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.SendingAmount);
                CashPickUpTransferAmount = dbContext.FaxingNonCardTransaction.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.FaxingAmount);
                KiiPayWalletTransfer = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.FaxingAmount);

            }
            if (AgentId > 0)
            {
                BankAccountTransferAmount = (from c in dbContext.BankAccountDeposit.ToList()
                                             join d in payingStaff on c.PayingStaffId equals d.Id
                                             select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.SendingAmount);

                OtherwalletTransferAmount = (from c in dbContext.MobileMoneyTransfer.ToList()
                                             join d in payingStaff on c.PayingStaffId equals d.Id
                                             select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.SendingAmount);
                CashPickUpTransferAmount = (from c in dbContext.FaxingNonCardTransaction.ToList()
                                            join d in payingStaff on c.PayingStaffId equals d.Id
                                            select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.FaxingAmount);
                KiiPayWalletTransfer = (from c in dbContext.TopUpSomeoneElseCardTransaction.ToList()
                                        join d in payingStaff on c.PayingStaffId equals d.Id
                                        select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.FaxingAmount);


            }


            var TotalMonthlySale = BankAccountTransferAmount + OtherwalletTransferAmount + CashPickUpTransferAmount + KiiPayWalletTransfer;


            return TotalMonthlySale;

        }
        public decimal get30dayTotalFee(int payingStaffId, int AgentId = 0)
        {
            var payingStaff = dbContext.AgentStaffInformation.Where(x => x.AgentId == AgentId).ToList();
            DateTime end = DateTime.Now.Date;
            DateTime start = end.AddDays(-30);
            decimal BankAccountTransferFee = 0;
            decimal OtherwalletTransferFee = 0;
            decimal CashPickUpTransferFee = 0;
            decimal KiiPayWalletTransferFee = 0;
            if (payingStaffId > 0)
            {

                BankAccountTransferFee = dbContext.BankAccountDeposit.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Fee);
                OtherwalletTransferFee = dbContext.MobileMoneyTransfer.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Fee);
                CashPickUpTransferFee = dbContext.FaxingNonCardTransaction.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.FaxingFee);
                KiiPayWalletTransferFee = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.FaxingFee);

            }
            if (AgentId > 0)
            {
                BankAccountTransferFee = (from c in dbContext.BankAccountDeposit.ToList()
                                          join d in payingStaff on c.PayingStaffId equals d.Id
                                          select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Fee);

                OtherwalletTransferFee = (from c in dbContext.MobileMoneyTransfer.ToList()
                                          join d in payingStaff on c.PayingStaffId equals d.Id
                                          select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Fee);
                CashPickUpTransferFee = (from c in dbContext.FaxingNonCardTransaction.ToList()
                                         join d in payingStaff on c.PayingStaffId equals d.Id
                                         select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.FaxingFee);
                KiiPayWalletTransferFee = (from c in dbContext.TopUpSomeoneElseCardTransaction.ToList()
                                           join d in payingStaff on c.PayingStaffId equals d.Id
                                           select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.FaxingFee);


            }

            var TotalMonthlySale = BankAccountTransferFee + OtherwalletTransferFee + CashPickUpTransferFee + KiiPayWalletTransferFee;
            return TotalMonthlySale;
        }
        public decimal get30dayTotalMargin(int payingStaffId, int AgentId = 0)
        {
            MarginCalculationServices _marginCalulationServices = new MarginCalculationServices(payingStaffId);
            var margin = _marginCalulationServices.GetTotalMargin();
            return margin;
            //var payingStaff = dbContext.AgentStaffInformation.Where(x => x.AgentId == AgentId).ToList();
            //DateTime end = DateTime.Now.Date;
            //DateTime start = end.AddDays(-30);
            //decimal BankAccountTransferMargin = 0;
            //decimal OtherwalletTransferMargin = 0;
            //decimal CashPickUpTransferMargin = 0;
            //decimal KiiPayWalletTransferMargin = 0;
            //if (payingStaffId > 0)
            //{
            //    BankAccountTransferMargin = dbContext.BankAccountDeposit.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Margin);
            //    OtherwalletTransferMargin = dbContext.MobileMoneyTransfer.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Margin);
            //    CashPickUpTransferMargin = dbContext.FaxingNonCardTransaction.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Margin);
            //    KiiPayWalletTransferMargin = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Margin);
            //}
            //if (AgentId > 0)
            //{
            //    BankAccountTransferMargin = (from c in dbContext.BankAccountDeposit.ToList()
            //                                 join d in payingStaff on c.PayingStaffId equals d.Id
            //                                 select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Margin);


            //    OtherwalletTransferMargin = (from c in dbContext.MobileMoneyTransfer.ToList()
            //                                 join d in payingStaff on c.PayingStaffId equals d.Id
            //                                 select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Margin);
            //    CashPickUpTransferMargin = (from c in dbContext.FaxingNonCardTransaction.ToList()
            //                                join d in payingStaff on c.PayingStaffId equals d.Id
            //                                select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Margin);
            //    KiiPayWalletTransferMargin = (from c in dbContext.TopUpSomeoneElseCardTransaction.ToList()
            //                                  join d in payingStaff on c.PayingStaffId equals d.Id
            //                                  select c).ToList().Where(x => x.PayingStaffId == payingStaffId && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.Margin);


            //}


            //var TotalMonthlySale = BankAccountTransferMargin + OtherwalletTransferMargin + CashPickUpTransferMargin + KiiPayWalletTransferMargin;
            //return TotalMonthlySale;
        }

        public decimal get30dayTotalSaleOfAllAuxAgent()

        {

            var payingStaff = dbContext.AgentStaffInformation;
            decimal BankAccountTransferAmount = 0;
            decimal OtherwalletTransferAmount = 0;
            decimal CashPickUpTransferAmount = 0;
            decimal KiiPayWalletTransfer = 0;

            DateTime end = DateTime.Now.Date;
            DateTime start = end.AddDays(-30);

            //var bankAccountDeposit = (from c in dbContext.BankAccountDeposit
            //                          join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
            //                          select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
            //                          DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList();


            //BankAccountTransferAmount = bankAccountDeposit.Select(x => x.SendingAmount).Sum();

            //OtherwalletTransferAmount = dbContext.MobileMoneyTransfer.Where(x => x.PaidFromModule == Module.Agent && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.SendingAmount);

            //CashPickUpTransferAmount = dbContext.FaxingNonCardTransaction.Where(x => (x.PayingStaffId != 0 || x.PayingStaffId != null) && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.FaxingAmount);
            //KiiPayWalletTransfer = dbContext.TopUpSomeoneElseCardTransaction.Where(x => (x.PayingStaffId != 0 || x.PayingStaffId != null) && DbFunctions.TruncateTime(x.TransactionDate) >= start && DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Sum(x => x.FaxingAmount);


            BankAccountTransferAmount = (from c in dbContext.BankAccountDeposit
                                         join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                                         select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
                                         DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.SendingAmount).Sum();

            OtherwalletTransferAmount = (from c in dbContext.MobileMoneyTransfer
                                         join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                                         select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
                                         DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.SendingAmount).Sum();



            CashPickUpTransferAmount = (from c in dbContext.FaxingNonCardTransaction
                                        join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                                        select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
                                        DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.FaxingAmount).Sum();


            KiiPayWalletTransfer = (from c in dbContext.TopUpSomeoneElseCardTransaction
                                    join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                                    select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
                                    DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.FaxingAmount).Sum();

            var TotalMonthlySale = BankAccountTransferAmount + OtherwalletTransferAmount + CashPickUpTransferAmount + KiiPayWalletTransfer;


            return TotalMonthlySale;

        }

        public decimal get30dayTotalMarginOfAllAuxAgent()
        {

            MarginCalculationServices _marginCalulationServices = new MarginCalculationServices();
            var margin = _marginCalulationServices.GetTotalMargin();
            return margin;
            //var payingStaff = dbContext.AgentStaffInformation;
            //DateTime end = DateTime.Now.Date;
            //DateTime start = end.AddDays(-30);
            //decimal BankAccountTransferMargin = 0;
            //decimal OtherwalletTransferMargin = 0;
            //decimal CashPickUpTransferMargin = 0;
            //decimal KiiPayWalletTransferMargin = 0;
            //BankAccountTransferMargin = (from c in dbContext.BankAccountDeposit
            //                             join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
            //                             select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
            //                             DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.Margin).Sum();


            //OtherwalletTransferMargin = (from c in dbContext.MobileMoneyTransfer
            //                             join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
            //                             select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
            //                             DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.Margin).Sum();


            //CashPickUpTransferMargin = (from c in dbContext.FaxingNonCardTransaction
            //                            join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
            //                            select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
            //                            DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.Margin).Sum();

            //KiiPayWalletTransferMargin = (from c in dbContext.TopUpSomeoneElseCardTransaction
            //                              join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
            //                              select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
            //                              DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.Margin).Sum();

            //var TotalMonthlySale = BankAccountTransferMargin + OtherwalletTransferMargin + CashPickUpTransferMargin + KiiPayWalletTransferMargin;
            //return TotalMonthlySale;
        }
        public decimal get30dayTotalFeeForAllAuxAgent()
        {

            var payingStaff = dbContext.AgentStaffInformation;
            DateTime end = DateTime.Now.Date;
            DateTime start = end.AddDays(-30);
            decimal BankAccountTransferFee = 0;
            decimal OtherwalletTransferFee = 0;
            decimal CashPickUpTransferFee = 0;
            decimal KiiPayWalletTransferFee = 0;

            BankAccountTransferFee = (from c in dbContext.BankAccountDeposit
                                      join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                                      select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
                                      DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.Fee).Sum();

            OtherwalletTransferFee = (from c in dbContext.MobileMoneyTransfer
                                      join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                                      select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
                                      DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.Fee).Sum();

            CashPickUpTransferFee = (from c in dbContext.FaxingNonCardTransaction
                                     join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                                     select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
                                     DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.FaxingFee).Sum();

            KiiPayWalletTransferFee = (from c in dbContext.TopUpSomeoneElseCardTransaction
                                       join d in payingStaff.Where(x => x.Agent.IsAUXAgent == true) on c.PayingStaffId equals d.Id
                                       select c).Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
                                       DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList().Select(x => x.FaxingFee).Sum();

            var TotalMonthlySale = BankAccountTransferFee + OtherwalletTransferFee + CashPickUpTransferFee + KiiPayWalletTransferFee;
            return TotalMonthlySale;
        }

    }
}