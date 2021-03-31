using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SEstimateFee
    {

        public static EstimateFaxingFeeSummary CalculateFaxingFee(decimal amount, bool includeFaxingFee, bool isAmountToBeRecieved, decimal exchangeRate, decimal faxingFeeRate, bool IsFlatFee = false)
        {
            /*Faxing amount 

                Does this include fee?

                Yes 

                Faxing amount - Commission fee (in %)

                (instant faxing = 5%) - Fax money without using a card

                (Card faxing (top-up Card = 6%) - Fax money using a card

            
                ---------------------------------------------------------
                Faxing amount 

                Does this include fee?

                No 

                Faxing amount + Commission fee (in %)

                (instant faxing = 5%) - Fax money without using a card

                (Card faxing (top-up Card = 6%) - Fax money using a card

                ----------------------------------------------------------

                Note:

                No conversion with fee?




                Nepal --> UK 

                100/Exchange Rate = Amount to be received

                UK --> Nepal 



                100*Exchange Rate = Amount to be Received



                Amount to be Received: 

                Nepal --> UK 

                £100

                Behind the scene ->

                £100 * Exchange Rate  = Rupees*/
            var obj = new EstimateFaxingFeeSummary();


            if (!isAmountToBeRecieved)
            {
                obj.FaxingAmount = amount;
                if (IsFlatFee == true)
                {
                    obj.FaxingFee = faxingFeeRate;
                }
                else
                {
                    obj.FaxingFee = Math.Round(amount / 100 * faxingFeeRate, 3);
                }
                obj.ExchangeRate = exchangeRate;
                if (includeFaxingFee)
                {
                    obj.TotalAmount = amount;

                    obj.FaxingAmount = amount - obj.FaxingFee;




                    obj.FaxingFee = amount - obj.FaxingAmount;
                    obj.ReceivingAmount = Math.Round(obj.FaxingAmount * exchangeRate, 3);
                }
                else
                {
                    obj.TotalAmount = amount + obj.FaxingFee;
                    obj.ReceivingAmount = Math.Round(obj.FaxingAmount * exchangeRate, 3);
                }
            }
            else
            {//from receiving amount 
             //RA=FA with out including faxing fee

                //exchange currency to local
                decimal amounttosend = amount;
                if (exchangeRate != 0)
                {
                    amounttosend = Math.Round(amount / exchangeRate, 3);
                    obj.FaxingAmount = amounttosend;

                    if (IsFlatFee == true)
                    {
                        obj.FaxingFee = faxingFeeRate;
                    }
                    else
                    {
                        obj.FaxingFee = Math.Round(amounttosend * faxingFeeRate / 100, 3);
                    }
                    obj.ExchangeRate = exchangeRate;

                    obj.TotalAmount = amounttosend + obj.FaxingFee;
                    obj.ReceivingAmount = Math.Round((amount / exchangeRate) * exchangeRate, 3);

                }
                //amounttosend = Math.Round(amount / exchangeRate, 2);
                //obj.FaxingAmount = amounttosend;
                //obj.FaxingFee = Math.Round(amounttosend * faxingFeeRate, 2);
                //obj.ExchangeRate = exchangeRate;

                //obj.TotalAmount = amounttosend + obj.FaxingFee;
                //obj.ReceivingAmount = Math.Round((amount / exchangeRate) * exchangeRate, 2);
            }
            if (IsFlatFee == true)
            {

                obj.FaxingFee = faxingFeeRate;
            }

            return obj;
        }

        public static decimal GetFaxingCommision(string CountryCode)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var continentCode = (from c in dbContext.Country.ToList().Where(x => x.CountryCode == CountryCode)
                                 select c.ContinentCode
                                 ).FirstOrDefault();
            var commision = (from c in dbContext.Commission
                             where c.Continent1 == continentCode
                             select c.Rate
                           ).FirstOrDefault();

            return commision ?? 0.00m;


        }


        public static DB.TransferFeePercentage  GetTransferFee(TransferFeeRequestParam feeparam)
        {
            DB.FAXEREntities dbContext = new FAXEREntities();
            IQueryable<TransferFeePercentage> fees = (from c in dbContext.TransferFeePercentage
                                                      where c.SendingCountry == feeparam.SendingCountry && c.ReceivingCountry == feeparam.ReceivingCountry
                                                      select c);
            fees = filterFeeByTransferType(fees, feeparam.transactiontransfertype);
            fees = filterFeeByMethod(fees, feeparam.TransactionTransferMethod);
            if (feeparam.AgentId > 0)
            {
                fees = filterFeeByAgent(fees, feeparam.AgentId);
            }

            decimal Amount = feeparam.Amount;
            foreach (var item in fees.ToList())
            {
                switch (item.Range)
                {
                    case DB.TranfserFeeRange.Select:
                        break;
                    case DB.TranfserFeeRange.All:
                        if (Amount >= 1)
                        {

                            return item;


                        }
                        break;
                    case DB.TranfserFeeRange.OneToHundred:
                        if (Amount >= 1 && Amount <= 100)
                        {

                            return item;

                        }
                        break;
                    case DB.TranfserFeeRange.HundredOnetoFiveHundred:
                        if (Amount >= 100 && Amount <= 500)
                        {
                            return item;

                        }
                        break;
                    case DB.TranfserFeeRange.FivehundredOneToThousand:
                        if (Amount >= 500 && Amount <= 1000)
                        {

                            return item;
                        }
                        break;
                    case DB.TranfserFeeRange.ThousandOneToFifteenHundred:
                        if (Amount >= 1000 && Amount <= 1500)
                        {

                            return item;
                        }
                        break;
                    case DB.TranfserFeeRange.FifteenHundredOneToTwoThousand:
                        if (Amount >= 1500 && Amount <= 2000)
                        {

                            return item;

                        }
                        break;
                    case DB.TranfserFeeRange.TwothousandOneToThreeThousand:
                        if (Amount >= 2000 && Amount <= 3000)
                        {
                            return item;

                        }
                        break;
                    case DB.TranfserFeeRange.ThreeTHousandOneToFiveThousand:
                        if (Amount >= 3000 && Amount <= 5000)
                        {
                            return item;
                        }
                        break;
                    case DB.TranfserFeeRange.FivethousandOneToTenThousand:
                        if (Amount >= 5000 && Amount <= 10000)
                        {

                            return item;
                        }
                        break;
                    default:


                        return item;
                        break;
                }


            }
            return fees.FirstOrDefault();


        }
        private static IQueryable<TransferFeePercentage> filterFeeByTransferType(IQueryable<TransferFeePercentage> transferFees,
            TransactionTransferType transferType)
        {
            var data = transferFees.Where(x => x.TransferType == transferType);
            if (data.Count() == 0)
            {
                data = transferFees.Where(x => x.TransferType == TransactionTransferType.All);
                if (data.Count() == 0) {
                    return transferFees;
                }
            }
            return data;
        }

        private static IQueryable<TransferFeePercentage> filterFeeByAgent(IQueryable<TransferFeePercentage> transferFees,
            int AgentId)
        {
            var data = transferFees.Where(x => x.AgetnId == AgentId);
            if (data.Count() == 0)
            {
                return transferFees;
            }
            return data;
        }

        private static IQueryable<TransferFeePercentage> filterFeeByMethod(IQueryable<TransferFeePercentage> transferFees , TransactionTransferMethod TransactionTransferMethod)
        {
            var data = transferFees.Where(x => x.TransferMethod == TransactionTransferMethod);
            if (data.Count() == 0) {    
                data = transferFees.Where(x => x.TransferMethod == TransactionTransferMethod.All);
                if (data.Count() == 0) {

                    return transferFees;
                }
            }
            return data;
        }

        public static DB.TransferFeePercentage GetTransferFee(string SendingCountry, string ReceivingCountry,
            TransactionTransferMethod TransactionTransferMethod = TransactionTransferMethod.All, decimal Amount = 0,
            TransactionTransferType transactiontransfertype = TransactionTransferType.Online, int AgentId = 0, bool IsAuxAgnet = false)
        {
            DB.FAXEREntities dbContext = new DB.FAXEREntities();

            var fees = (from c in dbContext.TransferFeePercentage
                        where c.SendingCountry == SendingCountry && c.ReceivingCountry == ReceivingCountry
                        && c.TransferMethod == TransactionTransferMethod && c.TransferType == transactiontransfertype
                        && c.AgetnId == AgentId
                        select c
                           ).ToList();

            if (IsAuxAgnet)
            {
                if (fees.Count == 0)
                {
                    fees = (from c in dbContext.TransferFeePercentage
                            where c.SendingCountry == SendingCountry && c.ReceivingCountry == ReceivingCountry
                            && c.TransferMethod == TransactionTransferMethod.All && c.TransferType == transactiontransfertype
                            && c.AgetnId == AgentId
                            select c
                           ).ToList();
                    if (fees.Count == 0)
                    {
                        return null;

                    }
                }
            }


            if (fees.Count == 0)
            {
                fees = (from c in dbContext.TransferFeePercentage
                        where c.SendingCountry == SendingCountry && c.ReceivingCountry == ReceivingCountry
                        && c.TransferMethod == TransactionTransferMethod && c.TransferType == transactiontransfertype
                        select c
                           ).ToList();

                if (fees.Count == 0)
                {
                    fees = (from c in dbContext.TransferFeePercentage
                            where c.SendingCountry == SendingCountry && c.ReceivingCountry == ReceivingCountry
                            && c.TransferType == transactiontransfertype
                            select c
                                       ).ToList();

                    if (fees.Count == 0)
                    {
                        fees = (from c in dbContext.TransferFeePercentage
                                where c.SendingCountry == SendingCountry && c.ReceivingCountry == ReceivingCountry
                                && c.TransferMethod == TransactionTransferMethod
                                select c
                               ).ToList();
                        if (fees.Count == 0)
                        {
                            fees = (from c in dbContext.TransferFeePercentage
                                    where c.SendingCountry == SendingCountry && c.ReceivingCountry == ReceivingCountry
                                    select c
                                   ).ToList();

                        }
                    }

                }
            }
            foreach (var item in fees)
            {
                switch (item.Range)
                {
                    case DB.TranfserFeeRange.Select:
                        break;
                    case DB.TranfserFeeRange.All:
                        if (Amount >= 1)
                        {

                            return item;


                        }
                        break;
                    case DB.TranfserFeeRange.OneToHundred:
                        if (Amount >= 1 && Amount <= 100)
                        {

                            return item;

                        }
                        break;
                    case DB.TranfserFeeRange.HundredOnetoFiveHundred:
                        if (Amount >= 100 && Amount <= 500)
                        {
                            return item;

                        }
                        break;
                    case DB.TranfserFeeRange.FivehundredOneToThousand:
                        if (Amount >= 500 && Amount <= 1000)
                        {

                            return item;
                        }
                        break;
                    case DB.TranfserFeeRange.ThousandOneToFifteenHundred:
                        if (Amount >= 1000 && Amount <= 1500)
                        {

                            return item;
                        }
                        break;
                    case DB.TranfserFeeRange.FifteenHundredOneToTwoThousand:
                        if (Amount >= 1500 && Amount <= 2000)
                        {

                            return item;

                        }
                        break;
                    case DB.TranfserFeeRange.TwothousandOneToThreeThousand:
                        if (Amount >= 2000 && Amount <= 3000)
                        {
                            return item;

                        }
                        break;
                    case DB.TranfserFeeRange.ThreeTHousandOneToFiveThousand:
                        if (Amount >= 3000 && Amount <= 5000)
                        {
                            return item;
                        }
                        break;
                    case DB.TranfserFeeRange.FivethousandOneToTenThousand:
                        if (Amount >= 5000 && Amount <= 10000)
                        {

                            return item;
                        }
                        break;
                    default:


                        return item;
                        break;
                }


            }
            return fees.FirstOrDefault();
        }





        public static EstimateFaxingFeeSummary GetIntroductoryTransferSummary(string sendingCountry, string receivingCountry, decimal Amount, decimal faxingFeeRate, bool IsFlatFee = false,
            bool IsReceivedAmount = false,
            TransactionTransferMethod TransactionTransferMethod = TransactionTransferMethod.Select, int AgentId = 0, bool ForAgent = false , int SenderId =0)
        {

            int transactionCount = 0;
            SSenderTransactionHistory senderTransactionHistory = new SSenderTransactionHistory();

            if (SenderId > 0) {
                transactionCount = Common.Common.GetTotalTransactionCount(SenderId);

                if (transactionCount > 0)
                {

                    return null;
                }
            }
            else if (Common.FaxerSession.LoggedUser != null)
            {
                //transactionCount = senderTransactionHistory.GetTransactionHistories(TransactionServiceType.All, Common.FaxerSession.LoggedUser.Id).TransactionHistoryList.Count;

                transactionCount = Common.Common.GetTotalTransactionCount(Common.FaxerSession.LoggedUser.Id);

                if (transactionCount > 0)
                {

                    return null;
                }
            }

            if (ForAgent == true)
            {
                transactionCount = Common.Common.GetTotalTransactionAgentCount(Common.AgentSession.LoggedUser.Id, Common.AgentSession.LoggedUser.PayingAgentStaffId);
                if (transactionCount > 0)
                {

                    return null;
                }

            }
            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var obj = new EstimateFaxingFeeSummary();

            decimal refAmount = Amount < 1 ? 1 : Amount;

            var rate = new IntroductoryRate();
            #region Get Exchange Rate
            if (ForAgent == false)
            {
                rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount
                && x.TransactionTransferMethod == TransactionTransferMethod && x.TransactionTransferType == TransactionTransferType.Online).FirstOrDefault();

                if (rate == null)
                {
                    rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount
                 && x.TransactionTransferType == TransactionTransferType.Online).FirstOrDefault();
                }
            }
            else
            {
                rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount
                && x.TransactionTransferMethod == TransactionTransferMethod && x.AgentId == AgentId).FirstOrDefault();

                if (rate == null)
                {

                    rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount
                && x.TransactionTransferMethod == TransactionTransferMethod && x.TransactionTransferType == TransactionTransferType.Agent).FirstOrDefault();

                    if (rate == null)
                    {

                        rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount
                && x.TransactionTransferType == TransactionTransferType.Agent).FirstOrDefault();

                    }
                }
                return null;
            }
            #endregion
            if (rate == null)
            {
                rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount).FirstOrDefault();
            }

            var fees = new List<IntroductoryFee>();
            if (ForAgent == false)
            {
                fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                            && x.ReceivingCountry == receivingCountry &&
                            x.TransferMethod == TransactionTransferMethod && x.TransferType == TransactionTransferType.Online).ToList();

                if (fees.Count == 0)
                {

                    fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                            && x.ReceivingCountry == receivingCountry && x.TransferType == TransactionTransferType.Online).ToList();
                }

            }
            else
            {
                fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                            && x.ReceivingCountry == receivingCountry &&
                            x.TransferMethod == TransactionTransferMethod && x.TransferType == TransactionTransferType.Agent && x.AgentId == AgentId).ToList();

                if (fees.Count == 0)
                {

                    fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                                && x.ReceivingCountry == receivingCountry &&
                                x.TransferMethod == TransactionTransferMethod && x.TransferType == TransactionTransferType.Agent).ToList();

                    if (fees.Count == 0)
                    {

                        fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                                && x.ReceivingCountry == receivingCountry && x.TransferType == TransactionTransferType.Online).ToList();
                    }
                }

            }

            if (fees.Count == 0)
            {
                fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry).ToList();
            }
            decimal feeRate = 0;
            bool feehasSetuped = false;
            foreach (var item in fees)
            {
                switch (item.Range)
                {
                    case DB.TranfserFeeRange.Select:
                        feeRate = 0;
                        break;
                    case DB.TranfserFeeRange.All:
                        if (refAmount >= 1)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;

                        }
                        break;
                    case DB.TranfserFeeRange.OneToHundred:
                        if (refAmount >= 1 && refAmount <= 100)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;

                        }
                        break;
                    case DB.TranfserFeeRange.HundredOnetoFiveHundred:
                        if (refAmount >= 100 && refAmount <= 500)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.FivehundredOneToThousand:
                        if (refAmount >= 500 && refAmount <= 1000)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.ThousandOneToFifteenHundred:
                        if (refAmount >= 1000 && refAmount <= 1500)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.FifteenHundredOneToTwoThousand:
                        if (refAmount >= 1500 && refAmount <= 2000)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.TwothousandOneToThreeThousand:
                        if (refAmount >= 2000 && refAmount <= 3000)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.ThreeTHousandOneToFiveThousand:
                        if (refAmount >= 3000 && refAmount <= 5000)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.FivethousandOneToTenThousand:
                        if (refAmount >= 5000 && refAmount <= 10000)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    default:

                        feeRate = item.Fee;

                        break;
                }

                decimal amounttosend = Amount;
                if (IsReceivedAmount)
                {
                    amounttosend = Math.Round(Amount / (rate == null ? 1 : rate.Rate), 2);
                }
                feeRate = GetIntroductoryFee(amounttosend, item.FeeType, feeRate);
                
                if (feehasSetuped == true)
                {
                    IsFlatFee = item.FeeType == FeeType.FlatFee ? true : false;
                    break;
                }


            }
            if (rate != null)
            {
                if (transactionCount > rate.NoOfTransaction)
                {
                    return null;
                }

                obj.ExchangeRate = rate.Rate;
                decimal amounttosend = 0;
                if (IsReceivedAmount)
                {
                    amounttosend = Math.Round(Amount / obj.ExchangeRate, 2);
                    obj.FaxingAmount = amounttosend;

                }
                else
                {

                    amounttosend = Amount;
                }

                obj.FaxingAmount = amounttosend;
                obj.ReceivingAmount = Math.Round(amounttosend * obj.ExchangeRate, 2);
                if (feehasSetuped == true)
                {
                    obj.TotalAmount = obj.FaxingAmount + feeRate;
                    obj.FaxingFee = feeRate;
                }
                else
                {
                    if ( IsFlatFee == true)
                    {
                        obj.TotalAmount = obj.FaxingAmount + faxingFeeRate;
                        obj.FaxingFee = faxingFeeRate;
                    }
                    else
                    {
                        obj.TotalAmount = obj.FaxingAmount + (faxingFeeRate / 100);
                        obj.FaxingFee = faxingFeeRate / 100;
                    }
                }
                obj.IsIntroductoryFee = feehasSetuped;
                obj.IsIntroductoryRate = true;
                if (obj.FaxingFee == 0)
                {

                    obj.ActualFee = IsFlatFee == true ? faxingFeeRate : obj.FaxingAmount * (faxingFeeRate / 100);
                }
                return obj;

            }
            return null;

        }

        public static EstimateFaxingFeeSummary GetIntroductoryRateEstimation(string sendingCountry, string receivingCountry, decimal Amount, decimal faxingFeeRate, bool IsFlatFee = false,
            bool IsReceivedAmount = false,
            TransactionTransferMethod TransactionTransferMethod = TransactionTransferMethod.Select, int AgentId = 0, bool ForAgent = false)
        {

            DB.FAXEREntities dbContext = new DB.FAXEREntities();
            var obj = new EstimateFaxingFeeSummary();

            decimal refAmount = Amount < 1 ? 1 : Amount;

            var rate = new IntroductoryRate();
            #region Get Exchange Rate
            if (ForAgent == false)
            {
                rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount
                && x.TransactionTransferMethod == TransactionTransferMethod && x.TransactionTransferType == TransactionTransferType.Online).FirstOrDefault();

                if (rate == null)
                {
                    rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount
                 && x.TransactionTransferType == TransactionTransferType.Online).FirstOrDefault();
                }
            }
            else
            {
                rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount
                && x.TransactionTransferMethod == TransactionTransferMethod && x.AgentId == AgentId).FirstOrDefault();

                if (rate == null)
                {

                    rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount
                && x.TransactionTransferMethod == TransactionTransferMethod && x.TransactionTransferType == TransactionTransferType.Agent).FirstOrDefault();

                    if (rate == null)
                    {

                        rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount
                && x.TransactionTransferType == TransactionTransferType.Agent).FirstOrDefault();

                    }
                }
                return null;
            }
            #endregion
            if (rate == null)
            {
                rate = dbContext.IntroductoryRate.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry && x.ToRange >= refAmount && x.FromRange <= refAmount).FirstOrDefault();
            }

            var fees = new List<IntroductoryFee>();
            if (ForAgent == false)
            {
                fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                            && x.ReceivingCountry == receivingCountry &&
                            x.TransferMethod == TransactionTransferMethod && x.TransferType == TransactionTransferType.Online).ToList();

                if (fees.Count == 0)
                {

                    fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                            && x.ReceivingCountry == receivingCountry && x.TransferType == TransactionTransferType.Online).ToList();
                }

            }
            else
            {
                fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                            && x.ReceivingCountry == receivingCountry &&
                            x.TransferMethod == TransactionTransferMethod && x.TransferType == TransactionTransferType.Agent && x.AgentId == AgentId).ToList();

                if (fees.Count == 0)
                {

                    fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                                && x.ReceivingCountry == receivingCountry &&
                                x.TransferMethod == TransactionTransferMethod && x.TransferType == TransactionTransferType.Agent).ToList();

                    if (fees.Count == 0)
                    {

                        fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                                && x.ReceivingCountry == receivingCountry && x.TransferType == TransactionTransferType.Online).ToList();
                    }
                }

            }

            if (fees.Count == 0)
            {
                fees = dbContext.IntroductoryFee.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry).ToList();
            }
            decimal feeRate = 0;
            bool feehasSetuped = false;
            foreach (var item in fees)
            {
                switch (item.Range)
                {
                    case DB.TranfserFeeRange.Select:
                        feeRate = 0;
                        break;
                    case DB.TranfserFeeRange.OneToHundred:
                        if (refAmount >= 1 && refAmount <= 100)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;

                        }
                        break;
                    case DB.TranfserFeeRange.HundredOnetoFiveHundred:
                        if (refAmount >= 100 && refAmount <= 500)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.FivehundredOneToThousand:
                        if (refAmount >= 500 && refAmount <= 1000)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.ThousandOneToFifteenHundred:
                        if (refAmount >= 1000 && refAmount <= 1500)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.FifteenHundredOneToTwoThousand:
                        if (refAmount >= 1500 && refAmount <= 2000)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.TwothousandOneToThreeThousand:
                        if (refAmount >= 2000 && refAmount <= 3000)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.ThreeTHousandOneToFiveThousand:
                        if (refAmount >= 3000 && refAmount <= 5000)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    case DB.TranfserFeeRange.FivethousandOneToTenThousand:
                        if (refAmount >= 5000 && refAmount <= 10000)
                        {
                            feeRate = item.Fee;
                            feehasSetuped = true;


                        }
                        break;
                    default:
                        feehasSetuped = true;
                        feeRate = item.Fee;
                        IsFlatFee = item.FeeType == FeeType.FlatFee ? true : false;
                        break;
                }

                decimal amounttosend = Amount;
                if (IsReceivedAmount)
                {
                    amounttosend = Math.Round(Amount / (rate == null ? 1 : rate.Rate), 2);
                }
                feeRate = GetIntroductoryFee(amounttosend, item.FeeType, feeRate);

                if (feehasSetuped == true)
                {
                    IsFlatFee = item.FeeType == FeeType.FlatFee ? true : false;
                    break;
                }


            }
            if (rate != null)
            {


                obj.ExchangeRate = rate.Rate;
                decimal amounttosend = 0;
                if (IsReceivedAmount)
                {
                    amounttosend = Math.Round(Amount / obj.ExchangeRate, 2);
                    obj.FaxingAmount = amounttosend;

                }
                else
                {

                    amounttosend = Amount;
                }

                obj.FaxingAmount = amounttosend;
                obj.ReceivingAmount = Math.Round(amounttosend * obj.ExchangeRate, 2);
                if (feehasSetuped == true)
                {
                    if (IsFlatFee == true)
                    {
                        obj.TotalAmount = obj.FaxingAmount + feeRate;
                        obj.FaxingFee = feeRate;
                    }
                    else
                    {
                        obj.TotalAmount = obj.FaxingAmount + (feeRate / 100);
                        obj.FaxingFee = feeRate / 100;
                    }
                    //obj.TotalAmount = obj.FaxingAmount + feeRate;
                    //obj.FaxingFee = feeRate;
                }
                else
                {
                    if (IsFlatFee == true)
                    {
                        obj.TotalAmount = obj.FaxingAmount + faxingFeeRate;
                        obj.FaxingFee = faxingFeeRate;
                    }
                    else
                    {
                        obj.TotalAmount = obj.FaxingAmount + (faxingFeeRate / 100);
                        obj.FaxingFee = faxingFeeRate / 100;
                    }
                }
                obj.IsIntroductoryFee = feehasSetuped;
                obj.IsIntroductoryRate = true;
                if (obj.FaxingFee == 0)
                {

                    obj.ActualFee = IsFlatFee == true ? faxingFeeRate : obj.FaxingAmount * (faxingFeeRate / 100);
                }
                return obj;

            }
            return null;
        }

        private static decimal GetIntroductoryFee(decimal Amount, DB.FeeType feeType, decimal feeRate)
        {

            decimal fee = 0;

            if (feeType == DB.FeeType.FlatFee)
            {
                fee = feeRate;
            }
            else
            {
                fee = Amount * (feeRate / 100);
            }


            return fee;
        }
    }

    public class TransferFeeRequestParam {

        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public string SendingCurrency { get; set; }
        public string ReceivingCurrency { get; set; }
        public TransactionTransferType transactiontransfertype { get; set; }
        public TransactionTransferMethod TransactionTransferMethod { get; set; }

        public decimal  Amount { get; set; }
        public int AgentId { get; set; }
        public bool IsAuxAgent { get; set; }

    }
}