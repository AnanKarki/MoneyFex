using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using Stripe;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SCreditDebitCardUsage
    {
        DB.FAXEREntities dbContext = null;
        public SCreditDebitCardUsage()
        {
            dbContext = new DB.FAXEREntities();
        }

        public bool AddCreditDebitCardUsageLimit(CreditDebitCardUsageLimit creditDebitCardUsage)
        {
            dbContext.CreditDebitCardUsageLimit.Add(creditDebitCardUsage);
            dbContext.SaveChanges();
            return true;
        }

        public bool AddCreditDebitCardUsageLog(CreditCardUsageLog creditCardUsageLog) {

            dbContext.CreditCardUsageLog.Add(creditCardUsageLog);
            dbContext.SaveChanges();
            return true;
        }

        public bool UpdateCreditDebitCardUsageLog(CreditCardUsageLog creditCardUsageLog)
        {

            
            creditCardUsageLog.Count = creditCardUsageLog.Count + 1;
            creditCardUsageLog.UpdatedDateTime = DateTime.Now;
            dbContext.Entry<CreditCardUsageLog>(creditCardUsageLog).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;

        }

        public bool AddOrUpdateCreditCardUsageLog(CreditCardUsageLog creditCardUsageLog)
        {

            var data = dbContext.CreditCardUsageLog.Where(x => x.SenderId == creditCardUsageLog.SenderId && x.Module == Module.Faxer 
            && x.CardNum == creditCardUsageLog.CardNum && DbFunctions.TruncateTime(x.UpdatedDateTime) == creditCardUsageLog.UpdatedDateTime.Date).FirstOrDefault();
            if (data == null)
            {
                AddCreditDebitCardUsageLog(creditCardUsageLog);
            }
            else{

                UpdateCreditDebitCardUsageLog(data);
            }

            return true;
        }

        public int GetCreditCardUsageCount(CreditCardUsageLog creditCardUsageLog) {


            var data = dbContext.CreditCardUsageLog.Where(x => x.Module == Module.Faxer && 
            x.CardNum == creditCardUsageLog.CardNum && 
            DbFunctions.TruncateTime( x.UpdatedDateTime) ==  creditCardUsageLog.UpdatedDateTime.Date).FirstOrDefault();
            if (data != null) {
                return data.Count;
            }
            return 0;
        }

        public ServiceResult<bool> HasExceededUsageLimit(CreditCardUsageLog creditCardUsage)
        {
            var result = new ServiceResult<bool>();
            var creditCardUsageLimit = dbContext.CreditDebitCardUsageLimit.FirstOrDefault();
            var count = GetCreditCardUsageCount(creditCardUsage);
            if (count >= creditCardUsageLimit.UsageLimit)
            {
                result.Data = true;
                result.Message = "Number of card payment attempts exceeded";
                result.Status = ResultStatus.Error;
            }

            else {

                result.Data = false;
                result.Status = ResultStatus.OK;
            }
            return result;

        }
        public ServiceResult<bool> GetLimitLeftMessage(CreditCardUsageLog creditCardUsage)
        {
            var result = new ServiceResult<bool>();
            var creditCardUsageLimit = dbContext.CreditDebitCardUsageLimit.FirstOrDefault();
            var count = GetCreditCardUsageCount(creditCardUsage);
            if (count != 0 && count < creditCardUsageLimit.UsageLimit)
            {
                result.Data = true;
                result.Message = "You have " + (creditCardUsageLimit.UsageLimit - count) +
                    " payment attempt(s) left with this card today.";
                result.Status = ResultStatus.Warning;
            }
            else {

                result.Data = false;
                result.Status = ResultStatus.OK;
            }
            return result;

        }


    }
}