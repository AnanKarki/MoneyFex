using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Services
{
    public class SCreditCardAttempt
    {
        DB.FAXEREntities dbContext = null;

        public SCreditCardAttempt()
        {
            dbContext = new DB.FAXEREntities();
        }
        public SCreditCardAttempt(DB.FAXEREntities db)
        {
            dbContext = db;
        }

        public bool AddCreditCardAttemptLimit(CreditCardAttemptLimit cardAttemptLimit)
        {
            try
            {


                dbContext.CreditCardAttemptLimit.Add(cardAttemptLimit);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public CreditCardAttemptLimit GetCardAttemptLimit()
        {

            return dbContext.CreditCardAttemptLimit.FirstOrDefault();
        }
        public CreditCardAttemptLimit GetCardAttemptLimit(int senderId, string sendingCountry, string receivingCountry)
        {
            var data = dbContext.CreditCardAttemptLimit.Where(x => x.SenderId == senderId && x.SendingCountry == sendingCountry
            && x.ReceivingCountry == receivingCountry).FirstOrDefault();
            if (data == null)
            {
                data = dbContext.CreditCardAttemptLimit.Where(x => x.SendingCountry == sendingCountry
                && x.ReceivingCountry == receivingCountry).FirstOrDefault();
                if (data == null)
                {
                    data = dbContext.CreditCardAttemptLimit.Where(x => x.SendingCountry.ToLower() == "all"
                    && x.ReceivingCountry.ToLower() == "all").FirstOrDefault();
                }
            }
            if (data == null)
            {
                data = new CreditCardAttemptLimit()
                {
                    AttemptLimit = 3
                };
            }
            return data;

        }


        public bool UpdateCreditCardAttemptLimit(CreditCardAttemptLimit cardAttemptLimit)
        {

            try
            {


                dbContext.Entry<CreditCardAttemptLimit>(cardAttemptLimit).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public bool DeleteCreditCardAttemptLimit(CreditCardAttemptLimit cardAttemptLimit)
        {

            try
            {

                dbContext.CreditCardAttemptLimit.Remove(cardAttemptLimit);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                return false;
            }

        }
        public bool AddCreditCardAttemptLog(CreditCardAttemptLog cardAttemptLog)
        {

            try
            {


                dbContext.CreditCardAttemptLog.Add(cardAttemptLog);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public ServiceResult<bool> IsValidCardAttempt(int SenderId, string CardNum)
        {
            string encryptedCardNum = CardNum.Encrypt();
            var count = dbContext.CreditCardAttemptLog.Where(x => x.SenderId == SenderId
           && x.CardNum == encryptedCardNum && x.CreditCardAttemptStatus == CreditCardAttemptStatus.Failure
           && DbFunctions.TruncateTime(x.AttemptedDateTime) == DbFunctions.TruncateTime(DateTime.Now)).Count();
            try
            {

                if (GetCardAttemptLimit() == null)
                {

                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "",
                        Status = ResultStatus.OK
                    };
                }
                if (count < GetCardAttemptLimit().AttemptLimit)
                {
                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "You have <span class=" + "text-danger" + ">" + (GetCardAttemptLimit().AttemptLimit - count) + "</span> " +
                        " payment attempt(s) left with this card today.",
                        Status = ResultStatus.OK,
                        IsCardUsageMsg = true
                    };
                }
                return new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "Number of card payment attempts exceeded please use a different card or payment method",
                    Status = ResultStatus.OK
                };
            }
            catch (Exception)
            {

                return new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "Please try again , Something went wrong",
                    Status = ResultStatus.OK
                };
            }


        }

        public ServiceResult<bool> IsValidCardAttempt(int SenderId, string CardNum , Module module)
        {
            string encryptedCardNum = CardNum.Encrypt();
            var count = dbContext.CreditCardAttemptLog.Where(x => x.SenderId == SenderId
           && x.CardNum == encryptedCardNum && x.Module == module && x.CreditCardAttemptStatus == CreditCardAttemptStatus.Failure
           && DbFunctions.TruncateTime(x.AttemptedDateTime) == DbFunctions.TruncateTime(DateTime.Now)).Count();
            try
            {

                if (GetCardAttemptLimit() == null)
                {

                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "",
                        Status = ResultStatus.OK
                    };
                }
                if (count < GetCardAttemptLimit().AttemptLimit)
                {
                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "You have <span class=" + "text-danger" + ">" + (GetCardAttemptLimit().AttemptLimit - count) + "</span> " +
                        " payment attempt(s) left with this card today.",
                        Status = ResultStatus.OK,
                        IsCardUsageMsg = true
                    };
                }
                return new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "Number of card payment attempts exceeded please use a different card or payment method",
                    Status = ResultStatus.OK
                };
            }
            catch (Exception)
            {

                return new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "Please try again , Something went wrong",
                    Status = ResultStatus.OK
                };
            }


        }


        public ServiceResult<bool> IsValidCardForMobileAttempt(int SenderId, string CardNum)
        {
            string encryptedCardNum = CardNum.Encrypt();
            var count = dbContext.CreditCardAttemptLog.Where(x => x.SenderId == SenderId
           && x.CardNum == encryptedCardNum && x.CreditCardAttemptStatus == CreditCardAttemptStatus.Failure
           && DbFunctions.TruncateTime(x.AttemptedDateTime) == DbFunctions.TruncateTime(DateTime.Now)).Count();
            try
            {

                if (GetCardAttemptLimit() == null)
                {

                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "",
                        Status = ResultStatus.OK
                    };
                }
                if (count < GetCardAttemptLimit().AttemptLimit)
                {
                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "You have " + (GetCardAttemptLimit().AttemptLimit - count) +  
                        " payment attempt(s) left with this card today.",
                        Status = ResultStatus.OK,
                        IsCardUsageMsg = true
                    };
                }
                return new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "Number of card payment attempts exceeded, please use a different card or payment method",
                    Status = ResultStatus.OK
                };
            }
            catch (Exception)
            {

                return new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "Please try again , Something went wrong",
                    Status = ResultStatus.OK
                };
            }


        }

        public ServiceResult<bool> IsValidCardAttempt(int SenderId, string CardNum , string SendingCountry, string ReceivingCountry)
        {
            string encryptedCardNum = CardNum.Encrypt();
            var count = dbContext.CreditCardAttemptLog.Where(x => x.SenderId == SenderId
           && x.CardNum == encryptedCardNum && x.CreditCardAttemptStatus == CreditCardAttemptStatus.Failure
           && DbFunctions.TruncateTime(x.AttemptedDateTime) == DbFunctions.TruncateTime(DateTime.Now)).Count();
            try
            {
                if (GetCardAttemptLimit(SenderId, SendingCountry, ReceivingCountry) == null)
                {

                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "",
                        Status = ResultStatus.OK
                    };
                }
                if (count < GetCardAttemptLimit(SenderId, SendingCountry, ReceivingCountry).AttemptLimit)
                {
                    return new ServiceResult<bool>()
                    {
                        Data = true,
                        Message = "",
                        Status = ResultStatus.OK
                    };
                }
                return new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "Card attempt exceeded",
                    Status = ResultStatus.OK
                };
            }
            catch (Exception)
            {

                return new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "Please try again , Something went wrong",
                    Status = ResultStatus.OK
                };
            }


        }


    }
}