using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Businesses.Services
{
    public class MoneyFaxCreditSheetServices
    {

        DB.FAXEREntities dbContext = null;

        int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;

        public MoneyFaxCreditSheetServices()
        {

            dbContext = new DB.FAXEREntities();
        }
        public decimal MFBCCardCurrentBalance()
        {
            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            var currentBalance = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId && x.CardStatus == DB.CardStatus.Active || x.CardStatus == DB.CardStatus.InActive).Select(x => x.CurrentBalance).FirstOrDefault();

            return currentBalance;
        }
        public decimal FaxerMerchantPaymentSumYearWise(int yearParam)
        {

            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            //var MFBCCardId = dbContext.MFBCCardInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).Select(x => x.KiiPayBusinessInformationId).FirstOrDefault();
            var result = (from c in dbContext.FaxerMerchantPaymentInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId)
                          join d in dbContext.FaxerMerchantPaymentTransaction.Where(x => x.PaymentDate.Year == yearParam) on c.Id equals d.SenderKiiPayBusinessPaymentInformationId
                          select d.PaymentAmount).ToList();
            decimal sum = 0;
            if (result != null)
            {
                sum = result.Sum();
            }
            return sum;
        }
        public Dictionary<string, decimal> FaxerMerchantPaymentSumMonthWise(int yearParam, int monthId)
        {

            decimal Sum = 0;
            int Count = 0;
            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            //var MFBCCardId = dbContext.MFBCCardInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).Select(x => x.KiiPayBusinessInformationId).FirstOrDefault();
            var result = (from c in dbContext.FaxerMerchantPaymentInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId)
                          join d in dbContext.FaxerMerchantPaymentTransaction.Where(x => x.PaymentDate.Year == yearParam && x.PaymentDate.Month == monthId) on c.Id equals d.SenderKiiPayBusinessPaymentInformationId
                          select d).ToList();



            Sum = result.Sum(x => (Decimal?)x.PaymentAmount) ?? 0;
            Count = result.Count();
            var val = new Dictionary<string, decimal>()
            {
                { "Sum" , Sum },
                { "Count", Count}
            };
            return val;
        }

        public decimal CardUserMerchantPaymentSumYearWise(int yearParam)
        {

            decimal sum = 0;
            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            int MFBCCardID = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).FirstOrDefault().Id;
            //var MFBCCardId = dbContext.MFBCCardInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).Select(x => x.KiiPayBusinessInformationId).FirstOrDefault();
            var LocalPayment = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayBusinessWalletInformation.KiiPayBusinessInformationId == KiiPayBusinessInformationId && x.TransactionDate.Year == yearParam);

            var InternationalPayment = dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId && x.TransactionDate.Year == yearParam);

            sum = (LocalPayment.Sum(x => (Decimal?)x.AmountSent) ?? 0) + (InternationalPayment.Sum(x => (Decimal?)x.ReceivingAmount) ?? 0);



            return sum;
        }

        public Dictionary<string, decimal> CardUserMerchantPaymentSumMonthWise(int yearParam, int monthId)
        {

            decimal Sum = 0;
            int Count = 0;
            int KiiPayBusinessInformationId = Common.BusinessSession.LoggedBusinessMerchant.KiiPayBusinessInformationId;
            int MFBCCardID = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).FirstOrDefault().Id;
            //var MFBCCardId = dbContext.MFBCCardInformation.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInformationId).Select(x => x.KiiPayBusinessInformationId).FirstOrDefault();
            var localPayment = dbContext.KiiPayPersonalNationalKiiPayBusinessPayment.Where(x => x.KiiPayBusinessWalletInformation.KiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId));

            var InternationalPayment = dbContext.KiiPayPersonalInternationalKiiPayBusinessPayment.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId));

            Sum = (localPayment.Sum(x => (Decimal?)x.AmountSent) ?? 0) + (InternationalPayment.Sum(x => (Decimal?)x.ReceivingAmount) ?? 0);

            Count = localPayment.Count() + localPayment.Count();
            var val = new Dictionary<string, decimal>
            {
                {"Sum", Sum },
                { "Count", Count}
            };
            
            return val;
        }

        public decimal MerchantPaymentyYearWiseByBusinessMerchant(int yearParam)
        {


            decimal Sum = 0;
            var localPayment = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId && x.TransactionDate.Year == yearParam).Sum(x => (Decimal?)x.AmountSent) ?? 0;

            var InternationalPayment = dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId && x.TransactionDate.Year == yearParam).Sum(x => (Decimal?)x.RecievingAmount) ?? 0;

            Sum = localPayment + InternationalPayment;
            return Sum;
        }

        public Dictionary<string, decimal> MerchantPaymentyMonthWiseByBusinessMerchant(int yearParam, int monthId)
        {



            decimal Sum = 0;
            int Count = 0;
            var localPayment = dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId));

            var InternationalPayment = dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.PayedToKiiPayBusinessInformationId == KiiPayBusinessInformationId && (x.TransactionDate.Year == yearParam && x.TransactionDate.Month == monthId));

            Sum = (localPayment.Sum(x => (Decimal?)x.AmountSent) ?? 0) + (InternationalPayment.Sum(x => (Decimal?)x.RecievingAmount) ?? 0);

            Count = localPayment.Count() + InternationalPayment.Count();



            var val = new Dictionary<string, decimal>();
            val.Add("Sum", Sum);
            val.Add("Count", Count);
            return val;

        }

    }
}