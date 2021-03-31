using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class MFBCPaymentServices
    {
        DB.FAXEREntities dbContext = null;
        CommonServices CommonService = new CommonServices();

        public MFBCPaymentServices()
        {

            dbContext = new DB.FAXEREntities();
        }



        public Models.PayMFBCCardUserViewModel GetPayMFBCCardUserDetails(string MFBCCardNumber)
        {

            string MFBC = "";
            var MFBCCardDetails = new KiiPayBusinessWalletInformation();

            string[] tokens = MFBCCardNumber.Split('-');
            if (tokens.Length < 2)
            {
                MFBC = GetCardInformationByCardNumber(MFBCCardNumber);
            }
            else
            {
                MFBC = MFBCCardNumber.Trim().Encrypt();
            }

            var result = (from c in dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == MFBC).ToList()
                          join CardCountry in dbContext.Country on c.Country equals CardCountry.CountryCode
                          join BusinessCountry in dbContext.Country on c.KiiPayBusinessInformation.BusinessOperationCountryCode equals BusinessCountry.CountryCode
                          select new Models.PayMFBCCardUserViewModel()
                          {
                              KiiPayBusinessInformationId = c.KiiPayBusinessInformationId,
                              BusinessName = c.KiiPayBusinessInformation.BusinessName,
                              BusinessEmail = c.KiiPayBusinessInformation.Email,
                              Address = c.KiiPayBusinessInformation.BusinessOperationAddress1,
                              BusinessLicenseNumber = c.KiiPayBusinessInformation.BusinessLicenseNumber,
                              City = c.KiiPayBusinessInformation.BusinessOperationCity,
                              Country = BusinessCountry.CountryName,
                              CountryCode = c.Country,
                              Telephone = c.KiiPayBusinessInformation.PhoneNumber,
                              DateTime = DateTime.Now,
                              AmountOnCard = c.CurrentBalance,
                              MFBCCurrency = CommonService.getCurrencyCodeFromCountry(c.Country),
                              MFBCCurrencySymbol = CommonService.getCurrencySymbol(c.Country),
                              MFBCAddress = c.AddressLine1,
                              MFBCCardId = c.Id,
                              MFBCCardName = c.FirstName,
                              MFBCCardNumber = c.MobileNo.Decrypt(),
                              MFBCCardStatus = Enum.GetName(typeof(CardStatus), c.CardStatus),
                              MFBCCardStatusEnum = c.CardStatus,
                              MFBCCardURL = c.KiiPayUserPhoto,
                              MFBCCarduserDOB = c.DOB.ToString("dd/MM/yyyy"),
                              MFBCCardUserEmail = c.Email,
                              MFBCCardUserGander = Enum.GetName(typeof(Gender), c.Gender),
                              MFBCCity = c.City,
                              MFBCCountry = CardCountry.CountryName,
                              MFBCFirstName = c.FirstName,
                              MFBCLastName = c.LastName,
                              MFBCMiddleName = c.MiddleName,
                              MFBCTelephone = c.PhoneNumber,
                              MFBCCardPhoneCode = CardCountry.CountryPhoneCode,
                              TemporalEmailOrSMS = c.TempSMS ? "YES" : "NO",


                          }).FirstOrDefault();
            return result;

        }
        public bool MFBCCardWithdrawl(DB.KiiPayBusinessWalletWithdrawlFromAgent withdrawl)
        {

            dbContext.MFBCCardWithdrawls.Add(withdrawl);
            int result = dbContext.SaveChanges();
            if (result > 0)
            {
                #region  Setting Success Vm
                PayAReceiverKiipayWalletSuccessViewModel transactionSuccessVm = new PayAReceiverKiipayWalletSuccessViewModel()
                {

                    KiiPayWalletType = KiiPayWalletType.Business,
                    TransactionId = withdrawl.Id
                };

                PayAReceiverControllerServices payAReceiverServices = new PayAReceiverControllerServices();
                payAReceiverServices.SetPayAReceiverKiipayWalletSuccess(transactionSuccessVm);

                #endregion

                #region Notification Section 

                var KiiPayBusinessWalletInfo = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == withdrawl.KiiPayBusinessWalletInformationId).FirstOrDefault();
                DB.Notification notification = new DB.Notification()
                {
                    SenderId = withdrawl.PayingAgentStaffId,
                    ReceiverId = withdrawl.KiiPayBusinessWalletInformationId,
                    Amount = Common.Common.GetCountryCurrency(withdrawl.IssuingCountryCode) + " " + withdrawl.TransactionAmount,
                    CreationDate = DateTime.Now,
                    Title = DB.Title.KiiPayWalletWithdrawal,
                    Message = "Wallet No :" + KiiPayBusinessWalletInfo.MobileNo,
                    NotificationReceiver = DB.NotificationFor.KiiPayBusiness,
                    NotificationSender = DB.NotificationFor.Agent,
                    Name = KiiPayBusinessWalletInfo.FirstName,
                };
                KiiPayBusinessCommonServices kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
                kiiPayBusinessCommonServices.SendNotification(notification);
                #endregion
                var data = dbContext.KiiPayBusinessWalletInformation.Where(x => x.Id == withdrawl.KiiPayBusinessWalletInformationId).FirstOrDefault();
                data.CurrentBalance = data.CurrentBalance - withdrawl.TransactionAmount;
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                int result2 = dbContext.SaveChanges();
                if (result2 > 0)
                {

                    #region Expired the Access Code 

                    //var BusinessWithdrawalCodedata = dbContext.KiiPayBusinessWalletWithdrawalCode.Where(x => x.AccessCode == Common.AgentSession.BusinessCardAccessCode).FirstOrDefault();
                    //BusinessWithdrawalCodedata.IsExpired = true;
                    //dbContext.Entry(BusinessWithdrawalCodedata).State = System.Data.Entity.EntityState.Modified;
                    //dbContext.SaveChanges();
                    #endregion

                    return true;
                }

            }
            return false;

        }

        internal string GetCardInformationByCardNumber(string MFBCCardNumber)

        {
            var result = dbContext.KiiPayBusinessWalletInformation.ToList();

            for (int i = 0; i < result.Count; i++)
            {
                if (!result[i].MobileNo.Contains("MF"))
                {
                    string[] tokens = result[i].MobileNo.Decrypt().Split('-');
                    if (tokens[1] == MFBCCardNumber)
                    {

                        var MFBCCard = result[i].MobileNo;
                        var model = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == MFBCCard && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
                        return model.MobileNo;
                    }
                }

            }
            return null;

        }
    }
}