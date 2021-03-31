using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class PaymentReceiptServices
    {

        DB.FAXEREntities dbContext = null;

        public PaymentReceiptServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        internal string GetNewMFTCCardPaymentReceiptNumber()
        {
            //this code should be unique and random with 8 digit length
            var val = "CP" + Common.Common.GenerateRandomDigit(6);

            while (dbContext.UserCardWithdrawl.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = GetNewMFTCCardPaymentReceiptNumber();
            }
            return val;
        }
        internal string GetNewMFBCCardWithdrawlsReceiptNumber()
        {
            //this code should be unique and random with 8 digit length
            var val = "CP" + Common.Common.GenerateRandomDigit(6);

            while (dbContext.MFBCCardWithdrawls.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = GetNewMFBCCardWithdrawlsReceiptNumber();
            }
            return val;
        }
       


        internal string GetNewMFBCBusinessCardPaymentReceiptNumber()
        {
            //this code should be unique and random with 8 digit length
            var val = "SP" + Common.Common.GenerateRandomDigit(6);

            while (dbContext.MFBCCardWithdrawls.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "SP" + Common.Common.GenerateRandomDigit(6);
            }
            return val;
        }

        internal string GetNewNonMFTCCardPaymentReceipt()
        {


            //this code should be unique and random with 8 digit length
            var val = "CP" + Common.Common.GenerateRandomDigit(6);

            while (dbContext.ReceiverNonCardWithdrawl.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "CP" + Common.Common.GenerateRandomDigit(6);
            }
            return val;
        }



    }
}