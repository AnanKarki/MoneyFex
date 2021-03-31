using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Areas.KiiPayPersonal.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class PayAReceiverControllerServices
    {

        DB.FAXEREntities dbContext = null;

        public PayAReceiverControllerServices()
        {
            dbContext = new DB.FAXEREntities();
        }

        public Decimal getTotalAmountWithDrawl(DateTime StartTransactionDate, int cardid)
        {

            //decimal TotalAmount = 0;
            var TotaAmountWithDrawl = dbContext.UserCardWithdrawl.Where(x => x.KiiPayPersonalWalletInformationId == cardid && x.TransactionDate >= StartTransactionDate).Sum(x => (Decimal?)x.TransactionAmount) ?? 0;


            return TotaAmountWithDrawl;

        }

        internal KiiPayPersonalWalletInformation GetCardInformationByCardNumber(string MFTCCardNumber)

        {
            var result = dbContext.KiiPayPersonalWalletInformation.ToList();

            for (int i = 0; i < result.Count; i++)
            {
                if (!result[i].MobileNo.Contains("MF"))
                {
                    string[] tokens = result[i].MobileNo.Decrypt().Split('-');
                    if (tokens[1] == MFTCCardNumber)
                    {

                        var MFTCCard = result[i].MobileNo;
                        var model = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MFTCCard && x.CardStatus == DB.CardStatus.Active).FirstOrDefault();
                        return model;
                    }
                }

            }
            return null;

        }


        public DB.FaxingNonCardTransaction GetTransInfo(string MFCN)
        {

            var info = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();
            return info;
        }
        public Models.PayAReceiveCashPickupReceiverDetailsViewModel GetTransactionDetails(string MFCN)
        {

            var agentInfo = Common.AgentSession.AgentInformation;
            var data = (from tran in dbContext.MerchantNonCardTransaction.Where(x => x.MFCN == MFCN).ToList()
                        join sender in dbContext.KiiPayBusinessWalletInformation on tran.MFBCCardID equals sender.Id
                        select new Models.PayAReceiveCashPickupReceiverDetailsViewModel()
                        {
                            Id = tran.NonCardRecieverId,

                            FirstName = sender.FirstName,
                            LastName = sender.LastName,
                            MiddleName = sender.MiddleName,
                            MobileNo = sender.PhoneNumber,
                            Country = Common.Common.GetCountryName(sender.Country),
                            AddressLine1 = sender.AddressLine1,
                            //   Id = agentInfo.Id,
                            //    FaxerCity = sender.City,
                            //    FaxerCountryCode = sender.Country,
                            //    FaxerPhoneCode = Common.Common.GetCountryPhoneCode(sender.Country),
                            //    DateTime = tran.TransactionDate,
                            //    ReceiverId = tran.NonCardRecieverId,
                            //    ReceiverCity = tran.NonCardReciever.City,
                            //    ReceiverFirstName = tran.NonCardReciever.FirstName,
                            //    ReceiverMiddleName = tran.NonCardReciever.MiddleName,
                            //    ReceiverEmail = tran.NonCardReciever.EmailAddress,
                            //    ReceiverLastName = tran.NonCardReciever.LastName,
                            //    ReceiverTelephone = tran.NonCardReciever.PhoneNumber,
                            //    ReceiverCountryCode = tran.NonCardReciever.Country,
                            //    ReceiverPhoneCode = Common.Common.GetCountryPhoneCode(tran.NonCardReciever.Country),
                            //    ReceiverCountry = Common.Common.GetCountryName(tran.NonCardReciever.Country),
                            //    NameOfAgency = agentInfo.Name,
                            //    AgencyMFSCode = agentInfo.AccountNo,
                            //    StatusOfFax = tran.FaxingStatus,
                            //    StatusOfFaxName = Enum.GetName(typeof(FaxingStatus), tran.FaxingStatus),
                            //    RefundRequest = tran.FaxingStatus == FaxingStatus.Refund ? "YES" : "NO",
                            //    FaxerCurrency = Common.Common.GetCountryCurrency(sender.Country),
                            //    FaxerCurrencySymbol = Common.Common.GetCurrencySymbol(sender.Country),
                            //    ReceiverCurrency = Common.Common.GetCountryCurrency(tran.NonCardReciever.Country),
                            //    ReceiverCurrencySymbol = Common.Common.GetCurrencySymbol(tran.NonCardReciever.Country),
                            //    ReceivingAmount = tran.ReceivingAmount.ToString(),
                            //    WithdrawalPaymentOf = Models.WithdrawalPaymentOf.Merchant
                        }).FirstOrDefault();

            return data;

        }

        public PayAReceiverCashPickupViewModel GetSenderInfo(string MFCN)
        {
            var vm = (from tran in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).ToList()
                      join receiver in dbContext.ReceiversDetails on tran.NonCardRecieverId equals receiver.Id
                      join faxer in dbContext.FaxerInformation on receiver.FaxerID equals faxer.Id
                      select new PayAReceiverCashPickupViewModel()
                      {
                          Id = faxer.Id,
                          FirstName = faxer.FirstName,
                          MiddleName = faxer.MiddleName,
                          LastName = faxer.LastName,
                          MobileNo = faxer.PhoneNumber,
                          Country = Common.Common.GetCountryName(faxer.Country),
                          MFCN = tran.MFCN,
                          Email = faxer.Email,
                          StatusOfFax = tran.FaxingStatus,
                          PhoneCode = Common.Common.GetCountryPhoneCode(faxer.Country),

                      }).FirstOrDefault();

            return vm;
        }

        public PayAReceiveCashPickupReceiverDetailsViewModel GetReciverInfo(string MFCN)
        {
            var vm = (from tran in dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).ToList()
                      join receiver in dbContext.ReceiversDetails on tran.NonCardRecieverId equals receiver.Id
                      join faxer in dbContext.FaxerInformation on receiver.FaxerID equals faxer.Id
                      select new PayAReceiveCashPickupReceiverDetailsViewModel()
                      {
                          Id = receiver.Id,
                          FirstName = receiver.FirstName,
                          MiddleName = receiver.MiddleName,
                          LastName = receiver.LastName,
                          MobileNo = receiver.PhoneNumber,
                          BirthCountry = receiver.Country,
                          StatusOfFax = tran.FaxingStatus,
                          StatusOfFaxName = Enum.GetName(typeof(FaxingStatus), tran.FaxingStatus),


                      }).FirstOrDefault();

            return vm;

        }


        public ReceiverNonCardWithdrawl GetNonCardWithdrawl(string MFCN)
        {
            var nonCardReceived = dbContext.ReceiverNonCardWithdrawl.Where(x => x.MFCN == MFCN).FirstOrDefault();

            return nonCardReceived;
        }

        public FaxingNonCardTransaction GetTransactionCountry(string MFCN)
        {
            var TransactionCountry = dbContext.FaxingNonCardTransaction.Where(x => x.MFCN == MFCN).FirstOrDefault();

            return TransactionCountry;
        }

        internal string CashPickUpReceipt()
        {


            //this code should be unique and random with 8 digit length
            var val = "CP" + Common.Common.GenerateRandomDigit(6);

            while (dbContext.ReceiverNonCardWithdrawl.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "CP" + Common.Common.GenerateRandomDigit(6);
            }
            return val;
        }
        public ServiceResult<ReceiverNonCardWithdrawl> Add(ReceiverNonCardWithdrawl model)
        {
            dbContext.ReceiverNonCardWithdrawl.Add(model);
            dbContext.SaveChanges();
            #region Notification Section 

            var CashPickUp = dbContext.FaxingNonCardTransaction.Where(x => x.Id == model.ReceiverId).FirstOrDefault();
            DB.Notification notification = new DB.Notification()
            {
                SenderId = model.PayingAgentStaffId,
                ReceiverId = CashPickUp.Id,
                Amount = Common.Common.GetCountryCurrency(model.IssuingCountryCode) + " " + CashPickUp.FaxingAmount,
                CreationDate = DateTime.Now,
                Title = DB.Title.KiiPayWalletWithdrawal,
                Message = "Wallet No :" + CashPickUp.MFCN,
                NotificationReceiver = DB.NotificationFor.Sender,
                NotificationSender = DB.NotificationFor.Agent,
                Name = CashPickUp.NonCardReciever.FirstName
            };

            SenderCommonServices senderCommonServices = new SenderCommonServices();
            senderCommonServices.SendNotification(notification);
            #endregion

            return new ServiceResult<ReceiverNonCardWithdrawl>()
            {
                Data = model,
                Message = "Save",
                Status = ResultStatus.OK

            };
        }
        public ServiceResult<IQueryable<KiiPayPersonalWalletInformation>> list()
        {

            return new ServiceResult<IQueryable<KiiPayPersonalWalletInformation>>()
            {
                Data = dbContext.KiiPayPersonalWalletInformation,
                Message = "",
                Status = ResultStatus.OK

            };
        }
        public ServiceResult<FaxingNonCardTransaction> Update(FaxingNonCardTransaction model)
        {
            dbContext.Entry<FaxingNonCardTransaction>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<FaxingNonCardTransaction>()
            {
                Data = model,
                Message = "Update",
                Status = ResultStatus.OK
            };
        }

        public void SetReceiverDetails(PayAReceiveCashPickupReceiverDetailsViewModel vm)
        {
            Common.AgentSession.PayAReceiveCashPickupReceiverDetails = vm;
        }

        public PayAReceiveCashPickupReceiverDetailsViewModel GetReceiverDetails()
        {
            PayAReceiveCashPickupReceiverDetailsViewModel vm = new PayAReceiveCashPickupReceiverDetailsViewModel();

            if (Common.AgentSession.PayAReceiveCashPickupReceiverDetails != null)
            {

                vm = Common.AgentSession.PayAReceiveCashPickupReceiverDetails;
            }
            return vm;
        }

        public string SetMFCN(string MFCN)
        {
            Common.AgentSession.MFCN = MFCN;
            return MFCN;
        }

        public string GetMFCN()
        {
            var MFCN = "";
            if (Common.AgentSession.MFCN != null)
            {
                MFCN = Common.AgentSession.MFCN;
            }
            return MFCN;
        }


        public PayAReceiverKiiPayWalletViewModel GetUserDetailByKiipayWalletNumber(string kiipayWalletNo)
        {
            var businessKiipayData = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == kiipayWalletNo).FirstOrDefault();
            var agentId = Common.AgentSession.AgentInformation.Id;
            PayAReceiverKiiPayWalletViewModel vm = new PayAReceiverKiiPayWalletViewModel();

            if (businessKiipayData != null)
            {
                vm = new PayAReceiverKiiPayWalletViewModel()
                {
                    Id = businessKiipayData.Id,
                    AddressLine1 = businessKiipayData.AddressLine1,
                    Country = Common.Common.GetCountryCodeByName(businessKiipayData.Country),
                    AddressLine2 = businessKiipayData.AddressLine2,
                    CountryCode = businessKiipayData.Country,
                    Email = businessKiipayData.Email,
                    FirstName = businessKiipayData.FirstName,
                    MiddleName = businessKiipayData.MiddleName,
                    LastName = businessKiipayData.LastName,
                    MobileNo = businessKiipayData.MobileNo,
                    PhoneCode = Common.Common.GetCountryPhoneCode(businessKiipayData.Country),
                    DOB = businessKiipayData.DOB.Date,
                    ExpiryDate = businessKiipayData.IdExpiryDate,
                    IdType = GetIdCardType(businessKiipayData.IdCardType),
                    IssuingCountry = businessKiipayData.IdIssuingCountry,
                    IdNumber = businessKiipayData.IdCardNumber,
                    PostCode = businessKiipayData.PostalCode,
                    CashWithdrawalLimit = businessKiipayData.CashWithdrawalLimit,
                    WalletStatus = businessKiipayData.CardStatus,
                    WalletStatusName = Enum.GetName(typeof(CardStatus), businessKiipayData.CardStatus),
                    //CashLimitType = businessKiipayData.CashLimitType,
                    KiiPayWalletType = KiiPayWalletType.Business,
                    AgentId = agentId,
                    WalletNo = kiipayWalletNo
                };
                return vm;
            }
            else
            {
                var personalKiipayData = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == kiipayWalletNo).FirstOrDefault();
                if (personalKiipayData != null)
                {
                    var data = (from c in dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == kiipayWalletNo).ToList()
                                join d in dbContext.SenderKiiPayPersonalAccount on c.Id equals d.KiiPayPersonalWalletId
                                into joined
                                from d in joined.DefaultIfEmpty()
                                select new PayAReceiverKiiPayWalletViewModel()
                                {
                                    Id = c.Id,
                                    AddressLine1 = c.Address1,
                                    Country = Common.Common.GetCountryCodeByName(c.CardUserCountry),
                                    AddressLine2 = c.Address2,
                                    CountryCode = c.CardUserCountry,
                                    Email = c.CardUserEmail,
                                    FirstName = c.FirstName,
                                    MiddleName = c.MiddleName,
                                    LastName = c.LastName,
                                    MobileNo = c.MobileNo,
                                    PhoneCode = Common.Common.GetCountryPhoneCode(c.CardUserCountry),
                                    DOB = c.CardUserDOB.Date,
                                    WalletNo = c.MobileNo,
                                    WalletStatus = c.CardStatus,
                                    WalletStatusName = Enum.GetName(typeof(CardStatus), c.CardStatus),
                                    CashLimitType = d == null ? CardLimitType.NoLimitSet : d.CashLimitType,
                                    CashWithdrawalLimit = d == null ? 0 : d.CashWithdrawalLimitAmount,
                                    KiiPayWalletType = KiiPayWalletType.Personal,
                                    AgentId = Common.AgentSession.AgentInformation.Id,


                                }).FirstOrDefault();

                    vm = data;

                    //vm = new PayAReceiverKiiPayWalletViewModel()
                    //{
                    //    Id = personalKiipayData.Id,
                    //    AddressLine1 = personalKiipayData.Address1,
                    //    Country = Common.Common.GetCountryCodeByName(personalKiipayData.CardUserCountry),
                    //    AddressLine2 = personalKiipayData.Address2,
                    //    CountryCode = personalKiipayData.CardUserCountry,
                    //    Email = personalKiipayData.CardUserEmail,
                    //    FirstName = personalKiipayData.FirstName,
                    //    MiddleName = personalKiipayData.MiddleName,
                    //    LastName = personalKiipayData.LastName,
                    //    MobileNo = personalKiipayData.MobileNo,
                    //    PhoneCode = Common.Common.GetCountryPhoneCode(personalKiipayData.CardUserCountry),
                    //    DOB = personalKiipayData.CardUserDOB.Date,
                    //    WalletNo = personalKiipayData.MobileNo,
                    //    WalletStatus = personalKiipayData.CardStatus,
                    //    WalletStatusName = Enum.GetName(typeof(CardStatus), personalKiipayData.CardStatus),
                    //    CashLimitType = personalKiipayData.CashLimitType,
                    //    CashWithdrawalLimit = personalKiipayData.CashWithdrawalLimit,
                    //    KiiPayWalletType = KiiPayWalletType.Personal,
                    //    AgentId = Common.AgentSession.LoggedUser.PayingAgentStaffId,

                    //};

                    return vm;
                }

            }

            return null;
        }


        public int GetIdCardType(string IdCardName)
        {

            var IdentityCardTypeId = dbContext.IdentityCardType.Where(x => x.CardType == IdCardName).Select(x => x.Id).FirstOrDefault();
            return IdentityCardTypeId;
        }
        public List<KiiPayPersonalWalletInformation> GetKiiPayPersonalInformation()
        {
            var personalKiipayData = dbContext.KiiPayPersonalWalletInformation.ToList();
            return personalKiipayData;
        }


        public ServiceResult<List<KiiPayPersonalWalletWithdrawalFromAgent>> GetPersonalUserCardWithdrawl()
        {
            return new ServiceResult<List<KiiPayPersonalWalletWithdrawalFromAgent>>()
            {
                Data = dbContext.UserCardWithdrawl.ToList(),
                Message = "",
                Status = ResultStatus.OK

            };
        }
        public ServiceResult<KiiPayPersonalWalletInformation> UpdateKiiPayPersonalWalletInformation(KiiPayPersonalWalletInformation model)
        {
            dbContext.Entry<KiiPayPersonalWalletInformation>(model).State = EntityState.Modified;
            dbContext.SaveChanges();
            return new ServiceResult<KiiPayPersonalWalletInformation>()
            {
                Data = model,
                Message = "",
                Status = ResultStatus.OK
            };
        }

        public List<KiiPayBusinessWalletInformation> GetKiiPayBusinessInformation()
        {
            var businessKiipayData = dbContext.KiiPayBusinessWalletInformation.ToList();
            return businessKiipayData;
        }
        public PayAReceiverKiiPayWalletViewModel getFaxerInfo(string AccountNoORPHoneNo)
        {
            var data = (from c in dbContext.FaxerInformation.Where(x => x.Email == AccountNoORPHoneNo || x.PhoneNumber == AccountNoORPHoneNo).ToList()
                        select new Models.PayAReceiverKiiPayWalletViewModel()
                        {
                            Id = c.Id,
                            FirstName = c.FirstName,
                            MiddleName = c.MiddleName,
                            LastName = c.LastName,
                            AddressLine1 = c.Address1,
                            Country = c.Country,
                            ExpiryDate = c.IdCardExpiringDate,
                            IdNumber = c.IdCardNumber,
                            IssuingCountry = c.IssuingCountry,
                            IdType = Int32.Parse(c.IdCardType),
                            Email = c.Email,
                            MobileNo = c.PhoneNumber,
                            DOB = c.DateOfBirth,
                            WalletNo = c.PhoneNumber,
                            AddressLine2 = c.Address2,

                            CountryCode = Common.Common.GetCountryPhoneCode(c.Country),

                        }).FirstOrDefault();

            return data;
        }

        public void SetPayAReceiver(PayAReceiverKiiPayWalletViewModel model)
        {
            Common.AgentSession.PayAReceiverKiiPayWallet = model;
        }

        public PayAReceiverKiiPayWalletViewModel GetPayAReceiver()
        {
            PayAReceiverKiiPayWalletViewModel vm = new PayAReceiverKiiPayWalletViewModel();
            if (Common.AgentSession.PayAReceiverKiiPayWallet != null)
            {
                vm = Common.AgentSession.PayAReceiverKiiPayWallet;
            }

            return vm;
        }
        public void SetPayAReceiverCashPickupViewModel(PayAReceiverCashPickupViewModel model)
        {
            Common.AgentSession.PayAReceiverCashPickupViewModel = model;
        }

        public PayAReceiverCashPickupViewModel GetPayAReceiverCashPickupViewModel()
        {
            PayAReceiverCashPickupViewModel vm = new PayAReceiverCashPickupViewModel();
            if (Common.AgentSession.PayAReceiverCashPickupViewModel != null)
            {
                vm = Common.AgentSession.PayAReceiverCashPickupViewModel;
            }

            return vm;
        }


        public void SetPayAReceiverKiiPayWalletEnteramount(PayAReceiverKiiPayWalletEnteramountViewModel model)
        {
            Common.AgentSession.PayAReceiverKiiPayWalletEnteramount = model;
        }

        public PayAReceiverKiiPayWalletEnteramountViewModel GetPayAReceiverKiiPayWalletEnteramount()
        {
            PayAReceiverKiiPayWalletEnteramountViewModel vm = new PayAReceiverKiiPayWalletEnteramountViewModel();
            if (Common.AgentSession.PayAReceiverKiiPayWalletEnteramount != null)
            {
                vm = Common.AgentSession.PayAReceiverKiiPayWalletEnteramount;
            }

            return vm;
        }


        public void SetPayAReceiverKiipayWalletSuccess(PayAReceiverKiipayWalletSuccessViewModel model)
        {
            Common.AgentSession.PayAReceiverKiipayWalletSuccess = model;
        }

        public PayAReceiverKiipayWalletSuccessViewModel GetPayAReceiverKiipayWalletSuccess()
        {
            PayAReceiverKiipayWalletSuccessViewModel vm = new PayAReceiverKiipayWalletSuccessViewModel();
            if (Common.AgentSession.PayAReceiverKiiPayWalletEnteramount != null)
            {
                vm = Common.AgentSession.PayAReceiverKiipayWalletSuccess;
            }

            return vm;
        }


        public ServiceResult<KiiPayPersonalWalletWithdrawalFromAgent> AddPayAreceiverPersonalKiiPay(KiiPayPersonalWalletWithdrawalFromAgent model)
        {
            dbContext.UserCardWithdrawl.Add(model);
            dbContext.SaveChanges();


            #region Notification Section 

            var KiiPayPersonalWalletInfo = dbContext.KiiPayPersonalWalletInformation.Where(x => x.Id == model.KiiPayPersonalWalletInformationId).FirstOrDefault();
            DB.Notification notification = new DB.Notification()
            {
                SenderId = model.PayingAgentStaffId,
                ReceiverId = model.KiiPayPersonalWalletInformationId,
                Amount = Common.Common.GetCountryCurrency(KiiPayPersonalWalletInfo.CardUserCountry) + " " + model.TransactionAmount,
                CreationDate = DateTime.Now,
                Title = DB.Title.KiiPayWalletWithdrawal,
                Message = "Wallet No :" + KiiPayPersonalWalletInfo.MobileNo,
                NotificationReceiver = DB.NotificationFor.kiiPayPersonal,
                NotificationSender = DB.NotificationFor.Agent,
                Name = KiiPayPersonalWalletInfo.FirstName,
            };

            SenderCommonServices senderCommonServices = new SenderCommonServices();
            senderCommonServices.SendNotificationToSenderKiiPayPersonalAccount(notification);
            #endregion

            return new ServiceResult<KiiPayPersonalWalletWithdrawalFromAgent>()
            {
                Data = model,
                Message = "",
                Status = ResultStatus.OK
            };
        }






    }
}