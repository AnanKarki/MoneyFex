using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.Mobile.Controllers.MoneyFex;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex.Login;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;

namespace FAXER.PORTAL.Areas.Mobile.Services.MoneyFex
{
    public class MobileMoneyFexLoginServices
    {
        DB.FAXEREntities dbContext;
        SenderCommonFunc senderCommonFunc;
        public MobileMoneyFexLoginServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        internal ServiceResult<SenderLoginResponseModel> PersonalLogin(LoginUserInfoModel model)
        {
            model.MobileNo = model.MobileNo.Trim();
            model.MobileNo = FAXER.PORTAL.Common.Common.IgnoreZero(model.MobileNo);

            var passHash = model.Password.Trim().ToHash();
            var count = dbContext.FaxerLogin.Where(x => (x.UserName == model.MobileNo || x.MobileNo == model.MobileNo)
            && x.Password == passHash && x.IsActive == true).FirstOrDefault();
            if ((count != null))
            {
                var faxerInfo = dbContext.FaxerInformation.Where(x => x.Id == count.FaxerId).FirstOrDefault();
                if (faxerInfo.IsBusiness)
                {
                    return new ServiceResult<SenderLoginResponseModel>()
                    {
                        Data = new SenderLoginResponseModel(),
                        Message = "Invalid Username or Password",
                        Status = ResultStatus.Error
                    };

                }

                var faxer = count.Faxer;
                var data = new SenderLoginResponseModel();
                senderCommonFunc = new SenderCommonFunc();
                if (!string.IsNullOrEmpty(faxerInfo.MiddleName))
                {

                    data.CountryCode = faxerInfo.Country;
                    data.CountryPhoneCode = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(faxerInfo.Country);
                    data.CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(faxerInfo.Country);
                    data.CurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(faxerInfo.Country);
                    data.Email = faxerInfo.Email;
                    data.MobileNo = faxerInfo.PhoneNumber;
                    data.MonthlyTransactionLimitAmount = senderCommonFunc.GetMonthlyTransactionMeter(faxerInfo.Id);
                    data.SenderId = faxerInfo.Id;
                    data.SenderName = faxerInfo.FirstName.Trim() + " " + faxerInfo.MiddleName.Trim() + " " + faxerInfo.LastName.Trim();
                    data.IsBusiness = faxerInfo.IsBusiness;
                }
                else
                {
                    data.CountryCode = faxerInfo.Country;
                    data.CountryPhoneCode = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(faxerInfo.Country);
                    data.CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(faxerInfo.Country);
                    data.CurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(faxerInfo.Country);
                    data.Email = faxerInfo.Email;
                    data.MobileNo = faxerInfo.PhoneNumber;
                    data.MonthlyTransactionLimitAmount = senderCommonFunc.GetMonthlyTransactionMeter(faxerInfo.Id);
                    data.SenderId = faxerInfo.Id;
                    data.SenderName = faxerInfo.FirstName.Trim() + " " + faxerInfo.LastName.Trim();
                    data.IsBusiness = faxerInfo.IsBusiness;
                }
                count.LoginFailedCount = 0;
                count.IsActive = true;
                dbContext.Entry(count).State = EntityState.Modified;
                dbContext.SaveChanges();
                var context = UpdateSession(faxerInfo);
                return new ServiceResult<SenderLoginResponseModel>()
                {
                    Data = data,
                    Message = "Success",
                    Status = ResultStatus.OK,
                    Token = context.Token
                };
            }
            else
            {
                return new ServiceResult<SenderLoginResponseModel>()
                {
                    Data = new SenderLoginResponseModel(),
                    Message = "Invalid Username or Password",
                    Status = ResultStatus.Error
                };

            }



        }

        internal ServiceResult<DefaultReceivingCountryViewModel> GetDefaultRecevingCountryAndCurrency()
        {
            string DefaultReceivingCountryCode = "";
            string DefaultReceivingCurrency = "";
            DefaultReceivingCountryViewModel model = new DefaultReceivingCountryViewModel();
            try
            {
                DefaultReceivingCountryCode = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings.Get("DefaultReceivingCountryCode"));
                DefaultReceivingCurrency = Convert.ToString(System.Configuration.ConfigurationSettings.AppSettings.Get("DefaultReceivingCurrency"));

            }
            catch (Exception ex)
            {
                DefaultReceivingCountryCode = "";
                DefaultReceivingCurrency = "";
                return new ServiceResult<DefaultReceivingCountryViewModel>()
                {
                    Data = null,
                    Message = "Cannot find default info",
                    Status = ResultStatus.Error
                };
            }
            model.DefaultReceivingCurrency = DefaultReceivingCurrency;
            model.DefaultReceivingCountryCode = DefaultReceivingCountryCode;
            return new ServiceResult<DefaultReceivingCountryViewModel>()
            {
                Data = model,
                Message = "",
                Status = ResultStatus.OK
            };

        }

        internal int GetApiVersion()
        {
            int ApiVersion = 0;
            try
            {
                ApiVersion = Convert.ToInt32(System.Configuration.ConfigurationSettings.AppSettings.Get("ApiVersion"));

            }
            catch (Exception ex)
            {
                ApiVersion = 0;
            }
            return ApiVersion;
        }

       

        internal ServiceResult<SenderLoginResponseModel> BusinessLogin(LoginUserInfoModel model)
        {
            model.MobileNo = model.MobileNo.Trim();
            model.MobileNo = FAXER.PORTAL.Common.Common.IgnoreZero(model.MobileNo);

            var passHash = model.Password.Trim().ToHash();
            var count = dbContext.FaxerLogin.Where(x => (x.UserName == model.MobileNo || x.MobileNo == model.MobileNo)
            && x.Password == passHash).FirstOrDefault();
            if ((count != null) && count.IsActive == true)
            {
                var faxerInfo = dbContext.FaxerInformation.Where(x => x.Email == count.UserName && x.IsDeleted == false).FirstOrDefault();
                if (!faxerInfo.IsBusiness)
                {
                    return new ServiceResult<SenderLoginResponseModel>()
                    {
                        Data = new SenderLoginResponseModel(),
                        Message = "Invalid Username or Password",
                        Status = ResultStatus.Error
                    };

                }

                var faxer = count.Faxer;
                var data = new SenderLoginResponseModel();
                senderCommonFunc = new SenderCommonFunc();
                if (!string.IsNullOrEmpty(faxerInfo.MiddleName))
                {

                    data.CountryCode = faxerInfo.Country;
                    data.CountryPhoneCode = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(faxerInfo.Country);
                    data.CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(faxerInfo.Country);
                    data.CurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(faxerInfo.Country);
                    data.Email = faxerInfo.Email;
                    data.MobileNo = faxerInfo.PhoneNumber;
                    data.SenderId = faxerInfo.Id;
                    data.SenderName = faxerInfo.FirstName.Trim() + " " + faxerInfo.MiddleName.Trim() + " " + faxerInfo.LastName.Trim();
                    data.IsBusiness = faxerInfo.IsBusiness;
                    data.MonthlyTransactionLimitAmount = senderCommonFunc.GetMonthlyTransactionMeter(faxerInfo.Id);
                }
                else
                {
                    data.CountryCode = faxerInfo.Country;
                    data.CountryPhoneCode = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(faxerInfo.Country);
                    data.CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(faxerInfo.Country);
                    data.CurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(faxerInfo.Country);
                    data.Email = faxerInfo.Email;
                    data.MobileNo = faxerInfo.PhoneNumber;
                    data.SenderId = faxerInfo.Id;
                    data.SenderName = faxerInfo.FirstName.Trim() + " " + faxerInfo.LastName.Trim();
                    data.IsBusiness = faxerInfo.IsBusiness;
                    data.MonthlyTransactionLimitAmount = senderCommonFunc.GetMonthlyTransactionMeter(faxerInfo.Id);
                }
                count.LoginFailedCount = 0;
                count.IsActive = true;
                dbContext.Entry(count).State = EntityState.Modified;
                dbContext.SaveChanges();
                var context = UpdateSession(faxerInfo);
                return new ServiceResult<SenderLoginResponseModel>()
                {
                    Data = data,
                    Message = "Success",
                    Status = ResultStatus.OK,
                    Token = context.Token
                };
            }
            else
            {
                return new ServiceResult<SenderLoginResponseModel>()
                {
                    Data = new SenderLoginResponseModel(),
                    Message = "Invalid Username or Password",
                    Status = ResultStatus.Error
                };

            }



        }

        internal bool ChangePassword(int senderId, string newPassword)
        {
            var senderLoginInfo = dbContext.FaxerLogin.Where(x => x.FaxerId == senderId).FirstOrDefault();
            if (senderLoginInfo != null)
            {
                senderLoginInfo.Password = newPassword.ToHash();
                dbContext.Entry<FaxerLogin>(senderLoginInfo).State = EntityState.Modified;
                dbContext.SaveChanges();

                MailCommon mail = new MailCommon();
                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                string body = "";
                var senderInfo = dbContext.FaxerInformation.Where(x => x.Id == senderLoginInfo.FaxerId).FirstOrDefault();

                body = FAXER.PORTAL.Common.Common.GetTemplate(baseUrl + "/EmailTemplate/FaxerAccountPasswordUpdateEmail?SenderName=" + senderInfo.FirstName + " "
                    + senderInfo.MiddleName + " " + senderInfo.LastName);
                mail.SendMail(senderInfo.Email, "Password Update ", body);
                return true;
            }
            return false;
        }

        internal MobileForgetPasswordModel SendOTPCode(string userName, bool isForPasswordReset)
        {
            MobileForgetPasswordModel model = new MobileForgetPasswordModel();
            var senderInfo = dbContext.FaxerInformation.Where(x => (x.Email == userName.Trim() || x.PhoneNumber == userName.Trim()) && x.IsDeleted == false).FirstOrDefault();
            var code = FAXER.PORTAL.Common.Common.GenerateRandomDigit(6);
            if (senderInfo != null)
            {
                #region sms

                if (!string.IsNullOrEmpty(senderInfo.PhoneNumber))
                {
                    SmsApi smsApi = new SmsApi();
                    var phoneNo = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(senderInfo.Country)
                        + senderInfo.PhoneNumber;

                    smsApi.SendSMS(phoneNo, smsApi.GetPinCodeMsg(code.ToString()));
                }

                #endregion
                #region email
                if (isForPasswordReset)
                {
                    if (!string.IsNullOrEmpty(senderInfo.Email))
                    {
                        MailCommon mail = new MailCommon();
                        var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                        string SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName;
                        string body = FAXER.PORTAL.Common.Common.GetTemplate(baseUrl + "/EmailTemplate/PasswordSecirityCodeEmail?UserName=" + SenderName +
                            "&SecurityCode=" + code);
                        mail.SendMail(senderInfo.Email, "MoneyFex Password Reset Key", body);
                    }
                }
                #endregion
                return new MobileForgetPasswordModel()
                {

                    OtpCode = code,
                    SenderId = senderInfo.Id
                };

            }
            else
            {
                return model;
            }
        }

        //private Context UpdateSession(EUser User)
        //{
        //    var newContext = new EContext() { Id = 0, LastLogin = DateTime.Now, TimeOut = TimeSpan.FromMinutes(20), UserId = User.Id, Token = getToken() };
        //    db.Context.Add(newContext);
        //    db.SaveChanges();

        //    db.SessionDetail.Add(new ESessionDetail() { Id = 0, Key = "User", Value = User.Id.ToString(), ContextId = newContext.Id });
        //    db.SaveChanges();
        //    return newContext;
        //}
        private Context UpdateSession(FaxerInformation faxerInfo)
        {
            var newContext = new Context() { Id = 0, LastLogin = DateTime.Now, TimeOut = TimeSpan.FromMinutes(20), UserId = faxerInfo.Id, Token = getToken() };
            dbContext.Context.Add(newContext);
            dbContext.SaveChanges();
            return newContext;
        }

        private string getToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}