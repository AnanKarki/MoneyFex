using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class AgentCommissionServices
    {
        DB.FAXEREntities dbContext = null;

        public AgentCommissionServices()
        {

            dbContext = new DB.FAXEREntities();

        }

        public decimal GetFaxedCommission(string year = "", int monthId = 0)
        {

            int AgentId = Common.AgentSession.AgentInformation.Id;
            var CountryCode = Common.AgentSession.AgentInformation.CountryCode;

            decimal TotalTransaction = 0;
            if (!string.IsNullOrEmpty(year) && monthId == 0)
            {
                int yearParam = int.Parse(year);
                var result = from c in dbContext.AgentFaxMoneyInformation.Where(x => x.AgentId == AgentId)
                             join d in dbContext.FaxingNonCardTransaction.Where(x => x.TransactionDate.Year == yearParam) on c.NonCardTransactionId equals d.Id
                             select d;

                if (result.Count() > 0)
                {
                    TotalTransaction = result.Sum(x => x.FaxingFee);
                }

            }
            if (monthId != 0 && !string.IsNullOrEmpty(year))
            {
                int yearParam = int.Parse(year);
                var result2 = from c in dbContext.AgentFaxMoneyInformation.Where(x => x.AgentId == AgentId)
                              join d in dbContext.FaxingNonCardTransaction.Where(x => x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId)
                              on c.NonCardTransactionId equals d.Id
                              select d;
                if (result2.Count() > 0)
                {
                    TotalTransaction = result2.Sum(x => x.FaxingFee);
                }
            }
            var AgentCommission = dbContext.AgentCommission.Where(x => x.Country == CountryCode).Select(x => x.SendingRate).FirstOrDefault();
            if (AgentCommission > 1)
            {

                AgentCommission = AgentCommission / 100;
            }
            decimal AgentFaxedCommision = Convert.ToDecimal(AgentCommission * TotalTransaction);

            return AgentFaxedCommision;

        }

        public decimal GetReceivingCommission(string year, int monthId , int StaffId)
        {
            int AgentId = Common.AgentSession.AgentInformation.Id;
            
            if (!string.IsNullOrEmpty(year) && monthId == 0)
            {
                int yearParam = int.Parse(year);
                var Commission1 = dbContext.UserCardWithdrawl.Where(x => x.PayingAgentStaffId == StaffId && x.TransactionDate.Year == yearParam).ToList().Sum(x => x.AgentCommission);
                var Commission2 = dbContext.MFBCCardWithdrawls.Where(x => x.PayingAgentStaffId == StaffId && x.TransactionDate.Year == yearParam).ToList().Sum(x => x.AgentCommission);
                var Commission3 = dbContext.ReceiverNonCardWithdrawl.Where(x => x.PayingAgentStaffId == StaffId && x.TransactionDate.Year == yearParam).ToList().Sum(x => x.AgentCommission);
                var TotalCommission = Commission1 + Commission2 + Commission3;
                return TotalCommission;
            }
            if (!string.IsNullOrEmpty(year) && monthId != 0)
            {
                int yearParam = int.Parse(year);
                var Commission1 = dbContext.UserCardWithdrawl.Where(x => x.PayingAgentStaffId == StaffId && x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var Commission2 = dbContext.MFBCCardWithdrawls.Where(x => x.PayingAgentStaffId == StaffId && x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var Commission3 = dbContext.ReceiverNonCardWithdrawl.Where(x => x.PayingAgentStaffId == StaffId && x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var TotalCommission = Commission1 + Commission2 + Commission3;
                return TotalCommission;
            }
            if (string.IsNullOrEmpty(year) && monthId != 0)
            {
               
                var Commission1 = dbContext.UserCardWithdrawl.Where(x => x.PayingAgentStaffId == StaffId && x.TransactionDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var Commission2 = dbContext.MFBCCardWithdrawls.Where(x => x.PayingAgentStaffId == StaffId && x.TransactionDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var Commission3 = dbContext.ReceiverNonCardWithdrawl.Where(x => x.PayingAgentStaffId == StaffId && x.TransactionDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var TotalCommission = Commission1 + Commission2 + Commission3;
                return TotalCommission;
            }
            if (string.IsNullOrEmpty(year) && monthId == 0)
            {
                
                var Commission1 = dbContext.UserCardWithdrawl.Where(x => x.PayingAgentStaffId == StaffId).ToList().Sum(x => x.AgentCommission);
                var Commission2 = dbContext.MFBCCardWithdrawls.Where(x => x.PayingAgentStaffId == StaffId).ToList().Sum(x => x.AgentCommission);
                var Commission3 = dbContext.ReceiverNonCardWithdrawl.Where(x => x.PayingAgentStaffId == StaffId).ToList().Sum(x => x.AgentCommission);
                var TotalCommission = Commission1 + Commission2 + Commission3;
                return TotalCommission;
            }
            return 0;
        }

        public decimal GetSendingCommission(string year, int monthId , int StaffId)
        {
            int AgentId = Common.AgentSession.AgentInformation.Id;
            if (!string.IsNullOrEmpty(year) && monthId == 0)
            {
                int yearParam = int.Parse(year);
                var Commission1 = dbContext.MobileMoneyTransfer.Where(x => x.PayingStaffId == StaffId && x.TransactionDate.Year == yearParam).ToList().Sum(x => x.AgentCommission);
                var Commission2 = dbContext.BankAccountDeposit.Where(x => x.PayingStaffId == StaffId && x.TransactionDate.Year == yearParam).ToList().Sum(x => x.AgentCommission);
                var Commission3 = dbContext.FaxingNonCardTransaction.Where(x => x.PayingStaffId == StaffId && x.TransactionDate.Year == yearParam).ToList().Sum(x => x.AgentCommission);
                var Commission4 = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == StaffId && x.TransactionDate.Year == yearParam).ToList().Sum(x => x.AgentCommission);
                var Commission5 = dbContext.PayBill.Where(x => x.PayingStaffId == StaffId && x.PaymentDate.Year == yearParam).ToList().Sum(x => x.AgentCommission);
                var Commission6 = dbContext.TopUpToSupplier.Where(x => x.PayingStaffId == StaffId && x.PaymentDate.Year == yearParam).ToList().Sum(x => x.AgentCommission);
                var TotalCommission = Commission1 + Commission2 + Commission3 + Commission4 + Commission5 + Commission6;
                return TotalCommission;
            }
            if (!string.IsNullOrEmpty(year) && monthId != 0)
            {
                int yearParam = int.Parse(year);
                var Commission1 = dbContext.MobileMoneyTransfer.Where(x => x.PayingStaffId == StaffId && x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var Commission2 = dbContext.BankAccountDeposit.Where(x => x.PayingStaffId == StaffId && x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var Commission3 = dbContext.FaxingNonCardTransaction.Where(x => x.PayingStaffId == StaffId && x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var Commission4 = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == StaffId && x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var Commission5 = dbContext.PayBill.Where(x => x.PayingStaffId == StaffId && x.PaymentDate.Year == yearParam && x.PaymentDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var Commission6 = dbContext.TopUpToSupplier.Where(x => x.PayingStaffId == StaffId && x.PaymentDate.Year == yearParam && x.PaymentDate.Month == monthId).ToList().Sum(x => x.AgentCommission);
                var TotalCommission = Commission1 + Commission2 + Commission3 + Commission4 + Commission5 + Commission6;
                return TotalCommission;
            }
            if (string.IsNullOrEmpty(year) && monthId == 0)
            {
                var Commission1 = dbContext.MobileMoneyTransfer.Where(x => x.PayingStaffId == StaffId).ToList().Sum(x => x.AgentCommission);
                var Commission2 = dbContext.BankAccountDeposit.Where(x => x.PayingStaffId == StaffId).ToList().Sum(x => x.AgentCommission);
                var Commission3 = dbContext.FaxingNonCardTransaction.Where(x => x.PayingStaffId == StaffId).ToList().Sum(x => x.AgentCommission);
                var Commission4 = dbContext.TopUpSomeoneElseCardTransaction.Where(x => x.PayingStaffId == StaffId).ToList().Sum(x => x.AgentCommission);
                var Commission5 = dbContext.PayBill.Where(x => x.PayingStaffId == StaffId).ToList().Sum(x => x.AgentCommission);
                var Commission6 = dbContext.TopUpToSupplier.Where(x => x.PayingStaffId == StaffId).ToList().Sum(x => x.AgentCommission);
                var TotalCommission = Commission1 + Commission2 + Commission3 + Commission4 + Commission5 + Commission6;
                return TotalCommission;
            }

            return 0;
        }



        public decimal GetReceivedCommsion(string year = "", int monthId = 0)
        {
            int AgentId = Common.AgentSession.AgentInformation.Id;
            var CountryCode = Common.AgentSession.AgentInformation.CountryCode;

            decimal nonCardReceivedTransactionAmount = 0;
            decimal MFTCCardWithdrawlTransactionAmount = 0;
            decimal MFBCCardWithdrawlTransactionAmount = 0;
            decimal CardUserNonCardPaymentWithdrawal = 0;
            decimal MerchantNonCardPaymentWithdrawal = 0;
            //decimal RefundpaidAmount = 0;

            if (!string.IsNullOrEmpty(year) && monthId == 0)
            {
                int yearparam = int.Parse(year);
                // NonCard Received Total Transaction 

                var nonCardReceivedYearWise = dbContext.ReceiverNonCardWithdrawl.Where(x => x.AgentId == AgentId && x.TransactionDate.Year == yearparam);

                if (nonCardReceivedYearWise.Count() > 0)
                {

                    nonCardReceivedTransactionAmount = nonCardReceivedYearWise.Sum(x => x.TransactionAmount);
                }
                // end
                // MFTC Card Withdrawl transaction 
                var MFTCCardWithdrawl = dbContext.UserCardWithdrawl.Where(x => x.AgentInformationId == AgentId && x.TransactionDate.Year == yearparam);

                if (MFTCCardWithdrawl.Count() > 0)
                {
                    MFTCCardWithdrawlTransactionAmount = MFTCCardWithdrawl.Sum(x => x.TransactionAmount);

                }

                // end
                // MFBC Card Withdrawl Transaction 
                var MFBCCardWithdrawl = dbContext.MFBCCardWithdrawls.Where(x => x.AgentInformationId == AgentId && x.TransactionDate.Year == yearparam);

                if (MFBCCardWithdrawl.Count() > 0)
                {
                    MFBCCardWithdrawlTransactionAmount = MFBCCardWithdrawl.Sum(x => x.TransactionAmount);

                }

                var CardUserNonCardWithdrawal = dbContext.CardUserNonCardWithdrawal.Where(x => x.AgentId == AgentId && x.TransactionDate.Year == yearparam);
                if (CardUserNonCardWithdrawal.Count() > 0)
                {
                    CardUserNonCardPaymentWithdrawal = CardUserNonCardWithdrawal.Sum(x => x.TransactionAmount);
                }

                var MerchantNonCardWithdrawal = dbContext.MerchantNonCardWithdrawal.Where(x => x.AgentId == AgentId && x.TransactionDate.Year == yearparam);

                if (MerchantNonCardWithdrawal.Count() > 0)
                {

                    MerchantNonCardPaymentWithdrawal = MerchantNonCardWithdrawal.Sum(x => x.TransactionAmount);
                }
                // end
                // Refund paid by Agent
                //var RefundPaidYearWise = from c in dbContext.FaxingNonCardTransaction.Where(x => x.TransactionDate.Year == yearparam)
                //                         join d in dbContext.RefundNonCardFaxMoneyByAgent on c.Id equals d.NonCardTransaction_id
                //                         where d.Agent_id == AgentId
                //                         select c;
                //if (RefundPaidYearWise.Count() > 0)
                //{
                //    RefundpaidAmount = RefundPaidYearWise.Sum(x => x.ReceivingAmount);
                //}
                // end
            }
            if (monthId != 0 && !string.IsNullOrEmpty(year))
            {
                int yearparam = int.Parse(year);
                var nonCardReceivedMonthWise = dbContext.ReceiverNonCardWithdrawl.Where(x => x.AgentId == AgentId && x.TransactionDate.Year == yearparam && x.TransactionDate.Month == monthId);

                if (nonCardReceivedMonthWise.Count() > 0)
                {

                    nonCardReceivedTransactionAmount = nonCardReceivedMonthWise.Sum(x => x.TransactionAmount);
                }
                // end
                // MFTC Card Withdrawl transaction 
                var MFTCCardWithdrawlMontwise = dbContext.UserCardWithdrawl.Where(x => x.AgentInformationId == AgentId && x.TransactionDate.Year == yearparam && x.TransactionDate.Month == monthId);

                if (MFTCCardWithdrawlMontwise.Count() > 0)
                {
                    MFTCCardWithdrawlTransactionAmount = MFTCCardWithdrawlMontwise.Sum(x => x.TransactionAmount);

                }

                // end
                // MFBC Card Withdrawl Transaction 
                var MFBCCardWithdrawlMontWise = dbContext.MFBCCardWithdrawls.Where(x => x.AgentInformationId == AgentId && x.TransactionDate.Year == yearparam && x.TransactionDate.Month == monthId);

                if (MFBCCardWithdrawlMontWise.Count() > 0)
                {
                    MFBCCardWithdrawlTransactionAmount = MFBCCardWithdrawlMontWise.Sum(x => x.TransactionAmount);

                }


                var CardUserNonCardWithdrawal = dbContext.CardUserNonCardWithdrawal.Where(x => x.AgentId == AgentId && x.TransactionDate.Year == yearparam && x.TransactionDate.Month == monthId);
                if (CardUserNonCardWithdrawal.Count() > 0)
                {
                    CardUserNonCardPaymentWithdrawal = CardUserNonCardWithdrawal.Sum(x => x.TransactionAmount);
                }

                var MerchantNonCardWithdrawal = dbContext.MerchantNonCardWithdrawal.Where(x => x.AgentId == AgentId && x.TransactionDate.Year == yearparam && x.TransactionDate.Month == monthId);

                if (MerchantNonCardWithdrawal.Count() > 0)
                {

                    MerchantNonCardPaymentWithdrawal = MerchantNonCardWithdrawal.Sum(x => x.TransactionAmount);
                }
                //var RefundPaidMontWise = from c in dbContext.FaxingNonCardTransaction.Where(x => x.TransactionDate.Year == yearparam && x.TransactionDate.Month == monthId)
                //                         join d in dbContext.RefundNonCardFaxMoneyByAgent on c.Id equals d.NonCardTransaction_id
                //                         where d.Agent_id == AgentId
                //                         select c;
                //if (RefundPaidMontWise.Count() > 0)
                //{
                //    RefundpaidAmount = RefundPaidMontWise.Sum(x => x.ReceivingAmount);
                //}

            }
            var AgentCommission = dbContext.AgentCommission.Where(x => x.Country == CountryCode).Select(x => x.ReceivingRate).FirstOrDefault();

            if (AgentCommission > 1)
            {

                AgentCommission = AgentCommission / 100;
            }
            decimal TotalTransactionAmount = nonCardReceivedTransactionAmount +
                                           MFTCCardWithdrawlTransactionAmount +
                                           MFBCCardWithdrawlTransactionAmount +
                                           CardUserNonCardPaymentWithdrawal +
                                           MerchantNonCardPaymentWithdrawal;

            // sending fee rate for agent commission rate 

            var FaxedCommission = dbContext.AgentCommission.Where(x => x.Country == CountryCode).Select(x => x.SendingRate).FirstOrDefault();
            if (FaxedCommission > 1)
            {

                FaxedCommission = FaxedCommission / 100;
            }
            var TotalFeeForReceivedAmount = FaxedCommission * TotalTransactionAmount;


            decimal AgentReceivedCommision = Convert.ToDecimal(TotalFeeForReceivedAmount * AgentCommission);
            return AgentReceivedCommision;
        }

        public AgentCommisionPaymentStatus CommisionPaymentStatus(string year = "", int monthId = 0)
        {

            int AgentId = Common.AgentSession.AgentInformation.Id;
            var result = dbContext.AgentCommissionPayment.Where(x => x.AgentId == AgentId && x.Year == year && x.Month == (Month)monthId).FirstOrDefault();

            if (result != null)
            {

                return AgentCommisionPaymentStatus.Paid;
            }
            return AgentCommisionPaymentStatus.UnPaid;



        }

        public DB.AgentCommissionPayment GetAgentCommissionPaymentDetials(string Year, int monthId)
        {

            int AgentId = Common.AgentSession.AgentInformation.Id;
            var result = dbContext.AgentCommissionPayment.Where(x => x.AgentId == AgentId && x.Year == Year && x.Month == (Month)monthId).FirstOrDefault();

            return result;
        }
        public DB.AgentInformation GetAgentInformation(int AgentId)
        {
            var result = dbContext.AgentInformation.Where(x => x.Id == AgentId).FirstOrDefault();

            return result;

        }
        public string GetStaffName(int staffId)
        {

            var result = dbContext.StaffInformation.Where(x => x.Id == staffId).FirstOrDefault();

            string staffName = result.FirstName + " " + result.MiddleName + " " + result.LastName;

            return staffName;
        }


        public DB.StaffLogin GetStaffLoginInfo(int staffId)
        {

            var result = dbContext.StaffLogin.Where(x => x.StaffId == staffId).FirstOrDefault();
            return result;

        }
    }
}