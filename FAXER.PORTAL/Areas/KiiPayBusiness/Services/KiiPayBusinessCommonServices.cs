
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using FAXER.PORTAL.SignalR;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.Services
{
    public class KiiPayBusinessCommonServices
    {

        DB.FAXEREntities dbContext = null;
        public KiiPayBusinessCommonServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public bool IsValidEmailAddress(string emailAddress)
        {

            var result = GetBusinessInformation(emailAddress);
            if (result == null)
            {

                return false;
            }
            return true;
        }

        public DB.KiiPayBusinessInformation GetBusinessInformation(string emailAddress)
        {


            var data = dbContext.KiiPayBusinessInformation.Where(x => x.Email == emailAddress).FirstOrDefault();
            return data;
        }
        public void SendSecurityCode(string emailAddress)
        {

            var data = GetBusinessInformation(emailAddress);
            MailCommon mail = new MailCommon();
            string passwordSecurityCode = Common.Common.GenerateRandomDigit(8);
            Common.BusinessSession.PasswordSecurityCode = passwordSecurityCode;
            Common.BusinessSession.EmailAddress = emailAddress;
            var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
            //mail.SendMail(email, "Business Password Security Code.", "Your Password Security Code is " + passwordSecurityCode);
            string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/PasswordSecirityCodeEmail?UserName=" + data.BusinessName +
                "&SecurityCode=" + passwordSecurityCode);

            mail.SendMail(data.Email, "MoneyFex Password Reset Key", body);

            SmsApi smsApiServices = new SmsApi();
            var message = smsApiServices.GetPasswordResetMessage(passwordSecurityCode);
            string PhoneNo = Common.Common.GetCountryPhoneCode(data.BusinessOperationCountryCode) + "" + data.PhoneNumber;
            smsApiServices.SendSMS(PhoneNo, message);
        }

        internal bool IsValidMobileNo(string mobileNo)
        {
            var result = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo == mobileNo).FirstOrDefault();
            return result == null ? false : true;

        }

        public bool IsValidSecurityCode(string securityCode)
        {
            if (Common.BusinessSession.PasswordSecurityCode == securityCode)
            {

                return true;
            }
            return false;
        }


        public decimal GetAccountBalance()
        {

            int KiiPayBusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            var WalletInfo = GetKiipayBusinessWalletInfoByKiiPayBusinessId(KiiPayBusinessId);

            if (WalletInfo == null)
            {

                return 0;
            }
            return WalletInfo.CurrentBalance;

        }
        public void UpdateAccountBalance()
        {

            int KiiPayBusinessId = Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId;
            var WalletInfo = GetKiipayBusinessWalletInfoByKiiPayBusinessId(KiiPayBusinessId);
            Common.BusinessSession.LoggedKiiPayBusinessUserInfo.CurrentBalanceOnCard = WalletInfo.CurrentBalance;


        }


        public void BalanceIn(int KiiPayBusinessWalletId, decimal Amount)
        {
            var data = GetKiipayBusinessWalletInfo(KiiPayBusinessWalletId);
            data.CurrentBalance += Amount;
            dbContext.Entry<DB.KiiPayBusinessWalletInformation>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

        public void BalanceOut(int KiiPayBusinessWalletId, decimal Amount)
        {

            var data = GetKiipayBusinessWalletInfo(KiiPayBusinessWalletId);
            data.CurrentBalance = data.CurrentBalance - Amount;
            dbContext.Entry<DB.KiiPayBusinessWalletInformation>(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
        }

        internal DB.KiiPayBusinessInformation GetKiipayBusinessInfo(int KiiPayBusinessId)
        {
            var result = dbContext.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessId).FirstOrDefault();
            return result;
        }

        internal DB.KiiPayBusinessWalletWithdrawalCode GetBusinessWithdrawalCode(int kiiPayBusinessInformationId)
        {
            var data = dbContext.KiiPayBusinessWalletWithdrawalCode.Where(x => x.KiiPayBusinessInformationId == kiiPayBusinessInformationId && x.IsExpired == false).FirstOrDefault();
            return data;
        }

        public bool DoesAccountHaveEnoughBal(decimal SendingAmount)
        {

            int WalletId = GetKiipayBusinessWalletInfoByKiiPayBusinessId(Common.BusinessSession.LoggedKiiPayBusinessUserInfo.KiiPayBusinessInformationId).Id;
            var Curbal = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == WalletId).FirstOrDefault().CurrentBalance;
            if (SendingAmount > Curbal)
            {
                return false;
            }
            return true;
        }

        internal decimal GetAccountBalanceByWalletId(int walletId)
        {
            var Curbal = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == walletId).FirstOrDefault().CurrentBalance;
            return Curbal;

        }
        internal DB.KiiPayPersonalWalletInformation GetPersonalAccountBalanceByWalletId(int walletId)
        {
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == walletId).FirstOrDefault();
            return data;

        }
        internal string GetCountryOfBusinessByBusinessInfoID(int buinessInfoId)
        {
            var data = dbContext.KiiPayBusinessInformation.Where(x => x.Id == buinessInfoId).Select(x => x.BusinessCountry).FirstOrDefault();
            return data;

        }
        public bool DoesAccountHaveEnoughBal(int SenderWalletId , decimal SendingAmount)
        {

            var Curbal = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == SenderWalletId).FirstOrDefault().CurrentBalance;
            if (SendingAmount > Curbal)
            {
                return false;
            }
            return true;
        }
        internal DB.KiiPayBusinessWalletWithdrawalCode AddBusinessWithdrawalCode(KiiPayBusinessWalletWithdrawalCode businessWithdrawalCode)
        {
            dbContext.KiiPayBusinessWalletWithdrawalCode.Add(businessWithdrawalCode);
            dbContext.SaveChanges();
            return businessWithdrawalCode;
        }

        public DB.KiiPayBusinessUserPersonalInfo GetKiipayBusinessPersonalInfo(int id)
        {
            var data = dbContext.KiiPayBusinessUserPersonalInfo.Where(x => x.KiiPayBusinessInformationId == id).FirstOrDefault();
            return data;
        }


        public string GetBusinessFullName(int id)
        {
            var data = dbContext.KiiPayBusinessUserPersonalInfo.Where(x => x.KiiPayBusinessInformationId == id).FirstOrDefault();
            return data.FirstName + " " + data.MiddleName + " " + data.LastName;
        }

        public DB.KiiPayBusinessWalletInformation GetKiipayBusinessWalletInfoByKiiPayBusinessId(int Id)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == Id).FirstOrDefault();
            return data == null ? null : data;

        }


        public List<DropDownViewModel> GetViewBagForKiipayBusinessWalletInfoByKiiPayBusinessId(int id)
        {
            var data =   (from c in dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == id).ToList()
                          
                          select new DropDownViewModel() {
                              Name = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              Id = c.Id
                          }).ToList();

            return data;
        }
        public DB.KiiPayBusinessWalletInformation GetKiipayBusinessWalletInfo(int Id)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == Id).FirstOrDefault();
            return data;

        }
       
        public string GetBusinessWalletFullName(int id)
        {
            var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == id).FirstOrDefault();
            return data == null ? null : data.FirstName + " " + data.MiddleName + " " + data.LastName;
        }

        internal string GetReceiptNoForKiiPayPersonalTransaction()
        {
            return "";
        }

        
        internal string GetReceiptNoForKiiPayBusinessInternationalTransaction()
        {



            return "";
        }

        public string GenerateWithdrawalCode()
        {


            //this code should be unique and random with8 digit length
            var val = Common.Common.GenerateRandomDigit(8);

            while (dbContext.KiiPayBusinessWalletWithdrawalCode.Where(x => x.AccessCode == val).Count() > 0)
            {
                val = Common.Common.GenerateRandomDigit(8);
            }
            return val;

        }

        

        public List<RecenltyPaidKiiPayBusinessVM> GetRecentlyPaidNationalKiiPayBusiness(int KiiPayBusinessInfoId)
        {

            var result = (from c in dbContext.KiiPayBusinessLocalTransaction.Where(x => x.PayedFromKiiPayBusinessInformationId == KiiPayBusinessInfoId)
                          select new RecenltyPaidKiiPayBusinessVM()
                          {
                              BusinessId = c.PayedToKiiPayBusinessWalletInformation.KiiPayBusinessInformation.Id,
                              MobileNo = c.PayedToKiiPayBusinessWalletInformation.KiiPayBusinessInformation.BusinessMobileNo
                          }).GroupBy(x => x.MobileNo).Select(x => x.FirstOrDefault()).ToList();

            return result;
        }
        public List<RecenltyPaidKiiPayBusinessVM> GetRecentlyPaidInternationalKiiPayBusiness(int KiiPayBusinessInfoId)
        {

            var result = (from c in dbContext.KiiPayBusinessInternationalPaymentTransaction.Where(x => x.PayedFromKiiPayBusinessInformationId == KiiPayBusinessInfoId)
                          select new RecenltyPaidKiiPayBusinessVM()
                          {
                              BusinessId = c.PayedToKiiPayBusinessWallet.KiiPayBusinessInformation.Id,
                              MobileNo = c.PayedToKiiPayBusinessWallet.KiiPayBusinessInformation.BusinessMobileNo,
                              Country = c.PayedToKiiPayBusinessWallet.KiiPayBusinessInformation.BusinessCountry
                          }).GroupBy(x => x.MobileNo).Select(x => x.FirstOrDefault()).ToList();

            return result;
        }

        public List<RecenltyPaidKiiPayBusinessVM> GetAllRecenltyPaidKiiPayBusinesses(int KiiPayBusinessInfoId)
        {

            var result = GetRecentlyPaidNationalKiiPayBusiness(KiiPayBusinessInfoId).
                Concat(GetRecentlyPaidInternationalKiiPayBusiness(KiiPayBusinessInfoId)).
                GroupBy(x => x.MobileNo).Select(x => x.FirstOrDefault()).ToList();

            return result;
        }



        public List<RecentlyPaidKiiPayPersonalVM> GetAllRecenltyPaidKiiPayPersonalWalletInfo(int KiiPayBusinessInfoId) {

            var result = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInfoId)
                          select new RecentlyPaidKiiPayPersonalVM()
                          {
                              WalletId = c.KiiPayPersonalWalletInformation.Id,
                              MobileNo = c.KiiPayPersonalWalletInformation.MobileNo,
                              ReceiverIsLocal = c.PaymentType == PaymentType.Local ? true : false,
                              Country = c.KiiPayPersonalWalletInformation.CardUserCountry
                          }).GroupBy(x => x.MobileNo).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }


       

        public KiiPayBusinessResult ValidCardStatus(DB.CardStatus? cardStatus)
        {


            KiiPayBusinessResult kiiPayBusinessResult = new KiiPayBusinessResult();
            string Message = "You cannot make transfer..";
            switch (cardStatus)
            {
                case DB.CardStatus.Active:
                    kiiPayBusinessResult.Status = ResultStatus.OK;

                    break;
                case DB.CardStatus.InActive:
                    kiiPayBusinessResult.Status = ResultStatus.Error;
                    Message = "Receiver wallet is currenlty deactive ," + Message;
                    break;
                case DB.CardStatus.IsDeleted:

                    kiiPayBusinessResult.Status = ResultStatus.Error;
                    Message = "Receiver wallet has been deleted ," + Message;
                    break;
                case DB.CardStatus.IsRefunded:

                    kiiPayBusinessResult.Status = ResultStatus.Error;
                    Message = "Receiver wallet has been delete ," + Message;
                    break;
                default:
                    kiiPayBusinessResult.Status = ResultStatus.Error;
                    break;
            }
            kiiPayBusinessResult.Message = Message;
            return kiiPayBusinessResult;
        }

        internal DB.KiiPayPersonalWalletInformation GetKiiPayPersonalWalletInfo(int KiiPayPersonalWalletId)
        {
            var data = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == KiiPayPersonalWalletId).FirstOrDefault();
            return data;
        }

        internal DB.KiiPayBusinessWalletInformation GetBusinessInformationByMobileNo(string MobileNo)
        {
            var BusinessWalletInfo = dbContext.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformation.BusinessMobileNo == MobileNo).FirstOrDefault();

            return BusinessWalletInfo;

        }

        public void SendNotification(DB.Notification notification) {

            SNotification _notificationServices = new SNotification();
            var result =  _notificationServices.SaveNotification(notification);

            string hourago = result.CreationDate.CalulateTimeAgo();
            //HubController.SendToDashBoard(result.ReceiverId.ToString(), result.Name ,result.Message ,  result.Amount , result.CreationDate.ToString() , hourago , null);
            HubController.SendToKiiPayBusiness(result.ReceiverId.ToString(), result.Id.ToString() , result.Name ,result.Message ,  result.Amount , result.CreationDate.ToString() , hourago , null);
        }
    }

}