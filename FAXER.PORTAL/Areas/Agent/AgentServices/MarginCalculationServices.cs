using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Windows.Media.TextFormatting;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class MarginCalculationServices
    {
        FAXEREntities dbContext = null;
        private int PayingStaffId;
        private IQueryable<TransferExchangeRateHistory> TransferExchangeRateHistory;
        private IQueryable<TransferFeePercentageHistory> TransferFeePercentageHistory;
        public MarginCalculationServices()
        {
            dbContext = new FAXEREntities();
        }
        public MarginCalculationServices(int payingStaffId)
        {
            dbContext = new FAXEREntities();
            this.PayingStaffId = payingStaffId;
        }
        public decimal GetTotalMargin()
        {
            GetTransferExchangeRateHistory();
            GetTransferFeePercentageHistory();
            var bankMargin = GetBankAccountMargin();
            var cashPickUpMargin = GetCashPickUpMargin();
            var OtherWalletMargin = GetOtherWalletMargin();
            decimal totalMargin = bankMargin + cashPickUpMargin + OtherWalletMargin;
            return totalMargin;
        }

        private void GetTransferExchangeRateHistory()
        {
            TransferExchangeRateHistory = dbContext.TransferExchangeRateHistory;
        }
        private void GetTransferFeePercentageHistory()
        {
            TransferFeePercentageHistory = dbContext.TransferFeePercentageHistory;
        }

        public void PrepareMFRate(DateTime fromdate, DateTime todate)
        {

            var rateHistory = dbContext.TransferExchangeRateHistory.Where(x => x.CreatedDate >= fromdate && x.CreatedDate <= todate);


        }

        public decimal GetBankAccountMargin()
        {
            decimal margin = 0m;
            DateTime end = DateTime.Now.Date;
            DateTime start = end.AddDays(-30);
            var bankAccount = dbContext.BankAccountDeposit.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
            DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList();
            if (PayingStaffId > 0)
            {
                bankAccount = bankAccount.Where(x => x.PayingStaffId == PayingStaffId).ToList();
            }
            else
            {
                bankAccount = bankAccount.Where(x => x.PayingStaffId != null).ToList();
            }

           var data = (from c in bankAccount
                        select new MarginCaluclationVm()
                        {
                            SendingAmount = c.SendingAmount,
                            AgentFee = c.Fee,
                            AgentRate = c.ExchangeRate,
                            MFRate = GetMFRate(c.TransactionDate, c.SendingCountry, c.ReceivingCountry, TransactionTransferMethod.BankDeposit),
                            MFFee = GetMFFee(c.TransactionDate, c.SendingCountry, c.ReceivingCountry, TransactionTransferMethod.BankDeposit)
                        });
            margin = data.ToList().Sum(x => x.GetTotalMargin);
            return margin;
        }


        private decimal GetMFFee(DateTime transactionDate, string sendingCountry, string receivingCountry, TransactionTransferMethod transferMethod)
        {
            decimal fee = 0;
            var transferFeeHistory = TransferFeePercentageHistory.Where(x => x.SendingCountry == sendingCountry &&
                                                                                    x.ReceivingCountry == receivingCountry &&
                                                                                    DbFunctions.TruncateTime(x.CreatedDate) == DbFunctions.TruncateTime(transactionDate)).ToList();

            var transferFee = transferFeeHistory.Where(x => x.TransferMethod == transferMethod).FirstOrDefault();
            if (transferFee == null)
            {
                transferFee = transferFeeHistory.Where(x => x.TransferMethod == TransactionTransferMethod.All).FirstOrDefault();
            }
            if (transferFee != null)
            {
                fee = transferFee.Fee;
            }
            if (fee == 0)
            {
                GetMFFee(transactionDate.AddDays(-1), sendingCountry, receivingCountry, transferMethod);
            }
            return fee;
        }

        private decimal GetMFRate(DateTime transactionDate, string sendingCountry, string receivingCountry, TransactionTransferMethod transferMethod)
        {
            decimal rate = 0M;
            var transferRateHistory = TransferExchangeRateHistory.Where(x => x.SendingCountry == sendingCountry &&
                                                                                       x.ReceivingCountry == receivingCountry &&
                                                                                       DbFunctions.TruncateTime(x.CreatedDate)
                                                                                       == DbFunctions.TruncateTime(transactionDate)).ToList();
            var transferRate = transferRateHistory.Where(x => x.TransferMethod == transferMethod).FirstOrDefault();
            if (transferRate == null)
            {
                transferRate = transferRateHistory.Where(x => x.TransferMethod == TransactionTransferMethod.All).FirstOrDefault();

            }
            if (transferRate != null)
            {
                rate = transferRate.Rate;
            }
            if (rate == 0)
            {
                GetMFRate(transactionDate.AddDays(-1), sendingCountry, receivingCountry, transferMethod);
            }
            return rate;
        }

        public decimal GetCashPickUpMargin()
        {
            decimal margin = 0m;
            DateTime end = DateTime.Now.Date;
            DateTime start = end.AddDays(-30);
            var cashPickUp = dbContext.FaxingNonCardTransaction.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
            DbFunctions.TruncateTime(x.TransactionDate) <= end ).ToList();

            if (PayingStaffId > 0)
            {
                cashPickUp = cashPickUp.Where(x => x.PayingStaffId == PayingStaffId).ToList();
            }
            else
            {
                cashPickUp = cashPickUp.Where(x => x.PayingStaffId != null).ToList();
            }
            var data = (from c in cashPickUp
                        select new MarginCaluclationVm()
                        {
                            SendingAmount = c.FaxingAmount,
                            AgentFee = c.FaxingFee,
                            AgentRate = c.ExchangeRate,
                            MFRate = GetMFRate(c.TransactionDate, c.SendingCountry, c.ReceivingCountry, TransactionTransferMethod.CashPickUp),
                            MFFee = GetMFFee(c.TransactionDate, c.SendingCountry, c.ReceivingCountry, TransactionTransferMethod.CashPickUp)
                        });

            margin = data.ToList().Sum(x => x.GetTotalMargin);

            return margin;
        }
        public decimal GetOtherWalletMargin()
        {
            decimal margin = 0m;
            DateTime end = DateTime.Now.Date;
            DateTime start = end.AddDays(-30);
            var otherWallet = dbContext.MobileMoneyTransfer.Where(x => DbFunctions.TruncateTime(x.TransactionDate) >= start &&
            DbFunctions.TruncateTime(x.TransactionDate) <= end).ToList();
            if (PayingStaffId > 0)
            {
                otherWallet = otherWallet.Where(x => x.PayingStaffId == PayingStaffId).ToList();
            }
            else
            {
                otherWallet = otherWallet.Where(x =>  x.PayingStaffId != null).ToList();
            }
            var data = (from c in otherWallet
                        select new MarginCaluclationVm()
                        {
                            SendingAmount = c.SendingAmount,
                            AgentFee = c.Fee,
                            AgentRate = c.ExchangeRate,
                            MFRate = GetMFRate(c.TransactionDate, c.SendingCountry, c.ReceivingCountry, TransactionTransferMethod.OtherWallet),
                            MFFee = GetMFFee(c.TransactionDate, c.SendingCountry, c.ReceivingCountry, TransactionTransferMethod.OtherWallet)
                        });

            margin = data.ToList().Sum(x => x.GetTotalMargin);
            return margin;
        }
    }
    public class MarginCaluclationVm
    {
        public decimal SendingAmount { get; set; }
        public decimal MFRate { get; set; }
        public decimal MFFee { get; set; }
        public decimal AgentRate { get; set; }
        public decimal AgentFee { get; set; }

        public decimal GetTotalMargin
        {
            get
            {
                var ExchangeRateMargin = ((MFRate = AgentRate) / MFRate) * SendingAmount;
                var FeeMargin = AgentFee - MFFee;

                return ExchangeRateMargin + FeeMargin;
            }
        }
    }
}