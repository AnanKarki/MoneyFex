using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.AgentServices
{
    public class TransferMoneyServices
    {
        FAXEREntities db = null;
        CommonServices _commonServices = null;
        public TransferMoneyServices()
        {
            db = new FAXEREntities();
            _commonServices = new CommonServices();
        }

        public List<DropDownCardTypeViewModel> GetIDCardTypes()
        {
            var result = (from c in db.IdentityCardType
                          select new DropDownCardTypeViewModel()
                          {
                              Id = c.Id,
                              CardType = c.CardType
                          }).ToList();
            return result;
        }

        public bool checkVirtualAccountNumberExists(string accountNo)
        {
            if (!string.IsNullOrEmpty(accountNo))
            {
                accountNo.Trim();
                var data = db.KiiPayPersonalWalletInformation.Where(x => x.MobileNo != "" || x.MobileNo != null || x.IsDeleted == false).ToList();

                foreach (var item in data)
                {
                    var cardNo = item.MobileNo.Decrypt().GetVirtualAccountNo();
                    if (cardNo.Trim() == accountNo)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public bool checkBusinessAccountNumberExists(string accountNo)
        {
            if (!string.IsNullOrEmpty(accountNo))
            {
                accountNo = accountNo.Trim();
                var data = db.KiiPayBusinessInformation.Where(x => x.BusinessName.Trim().ToLower() == accountNo.ToLower() || x.BusinessMobileNo == accountNo).FirstOrDefault();
                if (data != null)
                {
                    return true;
                }
            }
            return false;
        }

        public bool checkSenderAccountNumberExists(string senderAccountNo)
        {
            var data = db.FaxerInformation.Where(x => x.AccountNo == senderAccountNo.Trim() || x.PhoneNumber == senderAccountNo.Trim()).FirstOrDefault();
            if (data != null)
            {
                return true;
            }
            return false;
        }

        public TransferMoneyViewModel getSendersInfo(string accountNo)
        {
            if (!string.IsNullOrEmpty(accountNo))
            {
                accountNo = accountNo.Trim();
                var data = db.FaxerInformation.Where(x => x.AccountNo == accountNo || x.PhoneNumber == accountNo).FirstOrDefault();
                if (data != null)
                {
                    TransferMoneyViewModel model = new TransferMoneyViewModel()
                    {
                        SenderFaxerId = data.Id,
                        SenderFirstName = data.FirstName,
                        SenderMiddleName = data.MiddleName,
                        SenderLastName = data.LastName,
                        SenderDOB = data.DateOfBirth == null ? "" : data.DateOfBirth.Value.ToString("yyyy-MM-dd"),
                        SenderGender = Enum.GetName(typeof(Gender), data.GGender),
                        SenderAddress = data.Address1,
                        SenderCity = data.City,
                        SenderCountry = _commonServices.getCountryNameFromCode(data.Country),
                        SenderCountryPhoneCode = _commonServices.getPhoneCodeFromCountry(data.Country),
                        SenderTelephone = data.PhoneNumber,
                        SenderEmail = data.Email,
                        SenderAccountStatus = "",
                        SenderDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        SenderTime = DateTime.Now.ToString("hh:mm"),
                        SenderCurrency = _commonServices.getCurrencyCodeFromCountry(data.Country),
                        SenderCurrencySymbol = _commonServices.getCurrencySymbol(data.Country)
                    };
                    return model;
                }
            }
            return null;
        }

        public AgentBusinessTransferMoneyViewModel getSendersInfo1(string accountNo)
        {
            if(!string.IsNullOrEmpty(accountNo))
            {
                accountNo = accountNo.Trim();
                var data = db.FaxerInformation.Where(x => x.AccountNo == accountNo || x.PhoneNumber == accountNo).FirstOrDefault();
                if (data != null)
                {
                    AgentBusinessTransferMoneyViewModel model = new AgentBusinessTransferMoneyViewModel()
                    {
                        SenderFaxerId = data.Id,
                        SendersAccountNo = accountNo,
                        SenderFirstName = data.FirstName,
                        SenderMiddleName = data.MiddleName,
                        SenderLastName = data.LastName,
                        SenderDOB = data.DateOfBirth == null ? "" : data.DateOfBirth.Value.ToString("yyyy-MM-dd"),
                        SenderGender = Enum.GetName(typeof(Gender), data.GGender),
                        SenderAddress = data.Address1,
                        SenderCity = data.City,
                        SenderCountry = _commonServices.getCountryNameFromCode(data.Country),
                        SenderCountryPhoneCode = _commonServices.getPhoneCodeFromCountry(data.Country),
                        SenderTelephone = data.PhoneNumber,
                        SenderEmail = data.Email,
                        SenderDate = DateTime.Now.ToString("yyyy-MM-dd"),
                        SenderTime = DateTime.Now.ToString("HH:mm"),
                        SenderCurrency = _commonServices.getCurrencyCodeFromCountry(data.Country),
                        SenderCurrencySymbol = _commonServices.getCurrencySymbol(data.Country)
                    };
                    return model;
                }
            }
            return null;
        }


        public VirtualAccountDetailsVm getCardUsersInfo(string accountNo)
        {
            if (!string.IsNullOrEmpty(accountNo))
            {
                accountNo = accountNo.Trim();
                var data = db.KiiPayPersonalWalletInformation.Where(x => x.MobileNo != "" || x.MobileNo != null || x.IsDeleted == false).ToList();
                foreach (var item in data)
                {
                    var cardNo = item.MobileNo.Decrypt().GetVirtualAccountNo();
                    if (cardNo.Trim() == accountNo)
                    {
                        VirtualAccountDetailsVm model = new VirtualAccountDetailsVm()
                        {
                            ReceiverMFTCId = item.Id,
                            AccountDetailsVirtualAccountNo = accountNo,
                            AccountDetailsName = item.FirstName + " " + item.MiddleName + " " + item.LastName,
                            AccountDetailsNumber = accountNo,
                            AccountDetailsBalance = _commonServices.getCurrencyCodeFromCountry(item.CardUserCountry) +" "+ item.CurrentBalance.ToString(),
                            AccountDetailsWithdrawalLimit = item.CashWithdrawalLimit,
                            AccountDetailsLimitType = item.CashLimitType.ToString(),
                            AccountUserDetailsFirstName = item.FirstName,
                            AccountUserDetailsMiddleName = item.MiddleName,
                            AccountUserDetailsLastName = item.LastName,
                            AccountUserDetailsAddress = item.Address1,
                            AccountUserDetailsCity = item.CardUserCity,
                            AccountUserDetailsCountry = _commonServices.getCountryNameFromCode(item.CardUserCountry),
                            AccountUserDetailsTelephone = _commonServices.getPhoneCodeFromCountry(item.CardUserCountry) + item.CardUserTel,
                            ReceiverCurrency = _commonServices.getCurrencyCodeFromCountry(item.CardUserCountry),
                            ReceiverCurrenySymbol = _commonServices.getCurrencySymbol(item.CardUserCountry),
                            SenderAccountStatus = item.CardStatus.ToString(),
                            AccountUserDetailsPhoto = item.UserPhoto
                        };
                        return model;
                    }
                }
            }
            return null;
        }

        public AgentBusinessTransferMoneyViewModel getBusinessInfo(string accountNo)
        {
            if (!string.IsNullOrEmpty(accountNo))
            {
                accountNo = accountNo.Trim();
                var data = db.KiiPayBusinessInformation.Where(x => x.BusinessName.ToLower() == accountNo || x.BusinessMobileNo == accountNo).FirstOrDefault();
                if (data != null)
                {
                    AgentBusinessTransferMoneyViewModel model = new AgentBusinessTransferMoneyViewModel()
                    {
                        BusinessAccountNo = accountNo,
                        BusinessName = data.BusinessName,
                        BusinessLicenseNumber = data.BusinessLicenseNumber,
                        BusinessAddress = data.BusinessOperationAddress1,
                        BusinessCity = data.BusinessOperationCity,
                        BusinessCountry = _commonServices.getCountryNameFromCode(data.BusinessOperationCountryCode),
                        BusinessCountryPhoneCode = _commonServices.getPhoneCodeFromCountry(data.BusinessOperationCountryCode),
                        BusinessTelephoneNumber = data.PhoneNumber,
                        ReceiverKiiPayBusinessInformationId = data.Id,
                        ReceiverCurrency = _commonServices.getCurrencyCodeFromCountry(data.BusinessOperationCountryCode),
                        ReceiverCurrenySymbol = _commonServices.getCurrencySymbol(data.BusinessOperationCountryCode)
                    };
                    var mfbcInfo = db.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == data.Id && x.CardStatus == CardStatus.Active).FirstOrDefault();
                    if (mfbcInfo != null)
                    {
                        model.BusinessAccountStatus = "Active";
                    }
                    else
                    {
                        model.BusinessAccountStatus = "Not Active";
                    }
                    return model;
                }
            }
            return null;
        }

        public bool saveVirtualAccountDepositByAgent(TransferMoneyViewModel model)
        {
            if (model != null)
            {
                VirtualAccountDepositByAgent data = new VirtualAccountDepositByAgent()
                {
                    AgentId = Common.AgentSession.AgentInformation.Id,
                    FaxerId = model.SenderFaxerId,
                    CardUserId = model.ReceiverMFTCId,
                    IDCardType = model.SenderIDType,
                    IDCardNumber = model.SenderIDNumber,
                    IDCardExpiryDate = model.SenderIDExpiringDate,
                    IDCardIssuingCountry = model.SenderIDIssuingCountry,
                    DepositAmount = model.DepositAmount,
                    DepositFees = model.DepositFees,
                    ExchangeRate = model.CurrentExchangeRate,
                    ReceivingAmount = model.ReceivingAmount,
                    PaymentReference = model.PaymentReference,
                    PayingStaffName = model.NameOfPayingStaff,
                    TransactionDate = DateTime.Now,
                    ReceiptNumber = model.ReceiptNumber,
                    PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId
                };
                db.VirtualAccountDepositByAgent.Add(data);
                db.SaveChanges();

                #region Bank Account Credit Update 
                BankAccountCreditUpdateServices bankAccountCreditUpdateServices = new BankAccountCreditUpdateServices();
                DB.BaankAccountCreditUpdateByAgent baankAccountCreditUpdateByAgent = new BaankAccountCreditUpdateByAgent()
                {
                    AgentId = Common.AgentSession.AgentInformation.Id,
                    CustomerDeposit = model.DepositAmount,
                    NameOfUpdater = model.NameOfPayingStaff,
                    NonCardTransactionId = 0,
                    CreatedDateTime = DateTime.Now,
                    CustomerDepositFees = model.DepositFees,
                    ReceiptNo = model.ReceiptNumber

                };
                db.BaankAccountCreditUpdateByAgent.Add(baankAccountCreditUpdateByAgent);
                db.SaveChanges();
                #endregion
                #region Virtual Account Credit Update
                var mftcUserData = db.KiiPayPersonalWalletInformation.Where(x => x.Id == model.ReceiverMFTCId).FirstOrDefault();
                mftcUserData.CurrentBalance = mftcUserData.CurrentBalance + model.ReceivingAmount;
                db.Entry(mftcUserData).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                #endregion

                return true;
            }
            return false;
        }

        public bool saveBusinessAccountDepositByAgent(AgentBusinessTransferMoneyViewModel model)
        {
            if (model != null)
            {
                KiiPayBusinessWalletDepositByAgent data = new KiiPayBusinessWalletDepositByAgent()
                {
                    AgentId = Common.AgentSession.AgentInformation.Id,
                    SenderId = model.SenderFaxerId,
                    KiiPayBusinessInformationId = model.ReceiverKiiPayBusinessInformationId,
                    IDCardType = model.SenderIDType,
                    IDCardNumber = model.SenderIDNumber,
                    IDCardExpiryDate = model.SenderIDExpiringDate,
                    IDCardIssuingCountry = model.SenderIDIssuingCountry,
                    DepositAmount = model.DepositAmount,
                    DepositFees = model.DepositFees,
                    ExchangeRate = model.CurrentExchangeRate,
                    ReceivingAmount = model.ReceivingAmount,
                    PaymentReference = model.PaymentReference,
                    PayingStaffName = model.NameOfPayingStaff,
                    TransactionDate = DateTime.Now,
                    ReceiptNumber = model.ReceiptNumber,
                    PayingAgentStaffId = Common.AgentSession.LoggedUser.PayingAgentStaffId
                };
                db.BusinessAccountDepositByAgent.Add(data);
                db.SaveChanges();

                #region Bank Account Credit Update 
                BankAccountCreditUpdateServices bankAccountCreditUpdateServices = new BankAccountCreditUpdateServices();
                DB.BaankAccountCreditUpdateByAgent baankAccountCreditUpdateByAgent = new BaankAccountCreditUpdateByAgent()
                {
                    AgentId = Common.AgentSession.AgentInformation.Id,
                    CustomerDeposit = model.DepositAmount,
                    NameOfUpdater = model.NameOfPayingStaff,
                    NonCardTransactionId = 0,
                    CreatedDateTime = DateTime.Now,
                    CustomerDepositFees = model.DepositFees,
                    ReceiptNo = model.ReceiptNumber

                };
                db.BaankAccountCreditUpdateByAgent.Add(baankAccountCreditUpdateByAgent);
                db.SaveChanges();
                #endregion

                #region business Account Credit Update
                var mfbccardData = db.KiiPayBusinessWalletInformation.Where(x => x.KiiPayBusinessInformationId == model.ReceiverKiiPayBusinessInformationId && x.CardStatus == CardStatus.Active).FirstOrDefault();
                mfbccardData.CurrentBalance = mfbccardData.CurrentBalance + model.ReceivingAmount;
                db.Entry(mfbccardData).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                #endregion
                return true;
            }
            return false;
        }

        public string getFaxerCountryCode(int faxerId)
        {
            if (faxerId != 0 )
            {
                var data = db.FaxerInformation.Where(x => x.Id == faxerId).FirstOrDefault();
                if (data != null)
                {
                    return data.Country;
                }
            }
            return "";
        }

        public string getMFTCCardUserCountrCode(int mftcId)
        {
            if (mftcId != 0)
            {
                var data = db.KiiPayPersonalWalletInformation.Where(x => x.Id == mftcId).FirstOrDefault();
                if (data != null)
                {
                    return data.CardUserCountry;
                }
            }
            return "";
        }

        public string getBusinessCountryCode(int KiiPayBusinessInformationId)
        {
            if (KiiPayBusinessInformationId != 0)
            {
                var data = db.KiiPayBusinessInformation.Where(x => x.Id == KiiPayBusinessInformationId).FirstOrDefault();
                if (data != null)
                {
                    return data.BusinessOperationCountryCode;
                }
            }
            return "";
                 
        }

        public List<ExchangeRate> getExchangeRateTable()
        {
            return db.ExchangeRate.ToList();
        }

        public string GetVirtualAccountDepositByAgentReceiptNumber()
        {
            //this code should be unique and random with 8 digit length
            var val = "Ad-Np-MF" + Common.Common.GenerateRandomDigit(5);

            while (db.VirtualAccountDepositByAgent.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Ad-Np-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }

        public string GetBusinessAccountDepositByAgentReceiptNumber()
        {
            //this code should be unique and random with 8 digit length
            var val = "Ad-Np-MF" + Common.Common.GenerateRandomDigit(5);

            while (db.VirtualAccountDepositByAgent.Where(x => x.ReceiptNumber == val).Count() > 0)
            {
                val = "Ad-Np-MF" + Common.Common.GenerateRandomDigit(5);
            }
            return val;
        }
    }
}