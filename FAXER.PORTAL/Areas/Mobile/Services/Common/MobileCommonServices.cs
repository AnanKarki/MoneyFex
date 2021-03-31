using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Areas.Mobile.Common;
using FAXER.PORTAL.Areas.Mobile.Controllers.MoneyFex;
using FAXER.PORTAL.Areas.Mobile.Models.Common;
using FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.Controllers.Transfer;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Services.Description;

namespace FAXER.PORTAL.Areas.Mobile.Services.Common
{
    public class MobileCommonServices
    {

        DB.FAXEREntities dbContext = null;

        MobileRegistrationVerificationCodeService registrationVerificationCodeServices = new MobileRegistrationVerificationCodeService();
        public MobileCommonServices()
        {
            dbContext = new DB.FAXEREntities();
        }
        public ServiceResult<IQueryable<Country>> CountryList()
        {

            return new ServiceResult<IQueryable<Country>>
            {
                Data = dbContext.Country,
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<List<Country>> ServiceEnabledCountryList()
        {
            var enabledCountries = dbContext.TransferServiceMaster.GroupBy(x => x.ReceivingCountry).Select(x => x.FirstOrDefault()).ToList();
            var result = (from c in enabledCountries
                          join d in dbContext.Country on c.ReceivingCountry equals d.CountryCode
                          select d).GroupBy(x => x.CountryCode).Select(x => x.FirstOrDefault()).ToList();
            return new ServiceResult<List<Country>>
            {
                Data = result,
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<List<MobileCountryDropdownViewModel>> CountriesWithCurrency()
        {
            var enabledCountries = FAXER.PORTAL.Common.Common.GetReceivingCountries();
            var result = (from c in enabledCountries.ToList()
                          select new MobileCountryDropdownViewModel()
                          {
                              CountryCode = c.CountryCode,
                              CountryCurrency = c.Currency,
                              CountryName = c.CountryName,
                              CountryPhoneCode = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(c.CountryCode),
                              CurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(c.CountryCode),
                              FlagCode = c.FlagCountryCode,
                              CountryWithCurrency = c.CountryWithCurrency
                          }).ToList();

            return new ServiceResult<List<MobileCountryDropdownViewModel>>
            {
                Data = result,
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<bool> getIsMobilevalid(string CountryPhoneCode, string mobileNo)
        {
            SmsApi smsApi = new SmsApi();
            bool validMobile = smsApi.IsValidMobileNo(CountryPhoneCode + mobileNo);
            return new ServiceResult<bool>()
            {
                Data = validMobile,
                Message = "",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<IQueryable<Country>> CurrencyList()
        {
            return new ServiceResult<IQueryable<Country>>
            {
                Data = dbContext.Country,
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<IQueryable<Bank>> BankList()
        {
            return new ServiceResult<IQueryable<Bank>>
            {
                Data = dbContext.Bank,
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<IQueryable<MobileWalletOperator>> MobileOperatorsList()
        {
            return new ServiceResult<IQueryable<MobileWalletOperator>>
            {
                Data = dbContext.MobileWalletOperator,
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<IQueryable<Suppliers>> SuppliersList()
        {
            return new ServiceResult<IQueryable<Suppliers>>
            {
                Data = dbContext.Suppliers,
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<IQueryable<KiiPayBusinessBusinessStandingOrderInfo>> KiiPayBusinessBusinessStandingOrderInfoList()
        {
            return new ServiceResult<IQueryable<KiiPayBusinessBusinessStandingOrderInfo>>
            {
                Data = dbContext.KiiPayBusinessBusinessStandingOrderInfo,
                Status = ResultStatus.OK
            };
        }


        public ServiceResult<IQueryable<KiiPayBusinessWalletInformation>> WalletList()
        {
            return new ServiceResult<IQueryable<KiiPayBusinessWalletInformation>>
            {
                Data = dbContext.KiiPayBusinessWalletInformation,
                Status = ResultStatus.OK
            };
        }


        public string GetBankName(int bankId)
        {
            DB.FAXEREntities dbContext = new FAXEREntities();
            var name = dbContext.Bank.Where(x => x.Id == bankId).Select(x => x.Name).FirstOrDefault();
            return name;
        }
        public string GetMobileWalletname(int mobileWalletProvider)
        {
            string MobileWalletProvide = dbContext.MobileWalletOperator.Where(x => x.Id == mobileWalletProvider).Select(x => x.Name).FirstOrDefault();
            return MobileWalletProvide;
        }

        public ServiceResult<IQueryable<SenderBusinessDocumentation>> SenderDocumentation()
        {
            return new ServiceResult<IQueryable<SenderBusinessDocumentation>>
            {
                Data = dbContext.SenderBusinessDocumentation,
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<IQueryable<BankBranch>> BranchList()
        {
            return new ServiceResult<IQueryable<BankBranch>>
            {
                Data = dbContext.BankBranch,
                Status = ResultStatus.OK
            };
        }

        public bool IsMobileNoDuplicate(string MobileNo)
        {

            var result = dbContext.KiiPayBusinessInformation.Where(x => x.BusinessMobileNo == MobileNo).FirstOrDefault();
            if (result != null)
            {
                return true;
            }

            return false;


        }

        public bool IsMobileNoRegistered(string CountryCode, string MobileNo)
        {

            var result = dbContext.KiiPayBusinessLogin.Where(x => x.MobileNo == MobileNo && x.KiiPayBusinessInformation.BusinessCountry == CountryCode).FirstOrDefault();
            if (result != null)
            {
                return true;
            }

            return false;


        }

        public bool IsPassCodeCorrect(string MobileNo, string PassCode)
        {
            PassCode = PassCode.Encrypt();
            var result = dbContext.KiiPayBusinessLogin.Where(x => x.MobileNo == MobileNo && x.PinCode == PassCode).FirstOrDefault();
            if (result != null)
            {
                return true;
            }

            return false;



        }

        public bool IsEmailDuplicate(string email)
        {

            var result = dbContext.KiiPayBusinessInformation.Where(x => x.Email == email).FirstOrDefault();
            if (result != null)
            {
                return true;
            }
            return false;
        }

        public string GenerateVerificationCode(string CountryPhoneCode, string CountryCode, int KiiPayUserId, string PhoneNumber)
        {
            var phoneNumber = CountryPhoneCode + " " + PhoneNumber;
            var exist = registrationVerificationCodeServices.List().Data.Where(x => x.Country == CountryCode && x.PhoneNo == phoneNumber).FirstOrDefault();

            if (exist == null)
            {
                string verificationCode = FAXER.PORTAL.Common.Common.GenerateVerificationCode(6);

                MobileRegistrationCodeVerificationViewModel regverificationCodeVm = new MobileRegistrationCodeVerificationViewModel()
                {
                    Country = CountryCode,
                    UserId = KiiPayUserId,
                    //IsExpired = false,
                    PhoneNo = phoneNumber,
                    RegistrationOf = RegistrationOf.KiiPayBusiness,
                    VerificationCode = verificationCode,

                };
                AddRegistrationVerificationCode(regverificationCodeVm);
                SendVerificationCodeSMS(verificationCode, regverificationCodeVm.PhoneNo);

                return verificationCode;
            }
            else
            {
                string verificationCode = registrationVerificationCodeServices.List().Data.Where(x => x.Country == CountryCode && x.PhoneNo == phoneNumber).Select(x => x.VerificationCode).FirstOrDefault();
                SendVerificationCodeSMS(verificationCode, phoneNumber);
                return verificationCode;

            }
            // Send SMS 

        }
        public int GetKiiPayPersonalIdByMobileNo(string MobileNo, string Country)
        {
            var exist = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MobileNo).FirstOrDefault();

            if (exist != null)
            {
                int KiiPayPersonalWalletInformationId = exist.Id;

                return KiiPayPersonalWalletInformationId;
            }
            else
            {
                if (!string.IsNullOrEmpty(Country))
                {
                    var personalInfo = AddUnCompletedPersonalProfile(MobileNo, Country);


                    return personalInfo.Id;
                }
                return 0;
            }

        }

        public int GetKiiPayBusinessIdByMobileNo(string MobileNo)
        {
            var exist = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == MobileNo).FirstOrDefault();

            if (exist != null)
            {
                int KiiPayBusinessWalletInformationId = exist.KiiPayBusinessInformationId;

                return KiiPayBusinessWalletInformationId;
            }
            else
            {
                return 0;

            }

        }
        public string GetKiiPayPersonalNameByMobileNo(string MobileNo, string Country)
        {
            var exist = dbContext.KiiPayPersonalWalletInformation.Where(x => x.MobileNo == MobileNo).FirstOrDefault();

            if (exist != null)
            {

                if (string.IsNullOrEmpty(exist.FirstName))
                {

                    return exist.MobileNo;
                }
                string KiiPayPersonalWalletInformationName = exist.FirstName + " " + exist.MiddleName + " " + exist.LastName;

                return KiiPayPersonalWalletInformationName;
            }
            else
            {



                return "";

            }

        }

        public DB.KiiPayPersonalWalletInformation AddUnCompletedPersonalProfile(string MobileNo, string Country)
        {
            DB.KiiPayPersonalWalletInformation kiiPayPersonalWalletInformation = new KiiPayPersonalWalletInformation()
            {
                MobileNo = MobileNo,
                UnCompletedProfile = true,
                CardUserCountry = Country,
                CardUserDOB = DateTime.Now
            };
            dbContext.KiiPayPersonalWalletInformation.Add(kiiPayPersonalWalletInformation);
            dbContext.SaveChanges();
            return kiiPayPersonalWalletInformation;

        }
        public string GetKiiPayBusinessNameByMobileNo(string MobileNo)
        {
            var exist = dbContext.KiiPayBusinessWalletInformation.Where(x => x.MobileNo == MobileNo).FirstOrDefault();

            if (exist != null)
            {
                string KiiPayBusinessWalletInformationName = exist.FirstName + " " + exist.MiddleName + " " + exist.LastName;

                return KiiPayBusinessWalletInformationName;
            }
            else
            {
                return "";

            }

        }

        public DB.RegistrationVerificationCode AddRegistrationVerificationCode(MobileRegistrationCodeVerificationViewModel vm)
        {

            RegistrationVerificationCode model = new RegistrationVerificationCode()
            {
                UserId = vm.UserId,
                RegistrationOf = vm.RegistrationOf,
                VerificationCode = vm.VerificationCode,
                Country = vm.Country,
                PhoneNo = vm.PhoneNo
            };
            registrationVerificationCodeServices.Add(model);
            return model;
        }

        public bool RemoveCard(int cardId)
        {
            var result = dbContext.SavedCard.Where(x => x.Id == cardId).FirstOrDefault();
            if (result != null)
            {
                result.IsDeleted = true;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;

        }
        public bool RemoveBankAccount(int BankAccountId)
        {
            var result = dbContext.SavedBank.Where(x => x.Id == BankAccountId).FirstOrDefault();
            if (result != null)
            {
                result.isDeleted = true;
                dbContext.Entry(result).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;

        }

        public List<KiiPayBusinessSavedCreditDebitCardListVM> GetSavedCardsByKiiPayBusinessId(int kiiPayBusinessId)
        {
            var result = (from c in dbContext.SavedCard.Where(x => x.Module == DB.Module.KiiPayBusiness && x.UserId == kiiPayBusinessId && x.IsDeleted == false).ToList()
                          select new KiiPayBusinessSavedCreditDebitCardListVM()
                          {
                              CardId = c.Id,
                              CardNo = c.Num.Decrypt().FormatSavedCardNumber(),
                              CardStatus = getStatusByExpDate(c.EMonth, c.EYear) == false ? "Expired" : "Active",
                              ExpMonth = c.EMonth.Decrypt(),
                              ExpYear = c.EYear.Decrypt(),
                              CVVCode = c.ClientCode.Decrypt(),
                          }).ToList();

            return result;

        }
        public List<KiiPayBusinessSavedCreditDebitCardListVM> GetSavedCardsBySenderId(int senderId)
        {
            var result = (from c in dbContext.SavedCard.Where(x => x.Module == DB.Module.Faxer && x.UserId == senderId && x.IsDeleted == false).ToList()
                          select new KiiPayBusinessSavedCreditDebitCardListVM()
                          {
                              CardId = c.Id,
                              CardNo = c.Num.Decrypt().FormatSavedCardNumber(),
                              CardStatus = getStatusByExpDate(c.EMonth, c.EYear) == false ? "Expired" : "Active",
                              ExpMonth = c.EMonth.Decrypt(),
                              ExpYear = c.EYear.Decrypt(),
                              CVVCode = c.ClientCode.Decrypt(),
                              FullCardNo = c.Num.Decrypt()
                          }).ToList();

            return result;

        }
        public List<MobileSenderIdentityInformationViewModel> GetIdentityInformationListBySenderId(int senderId)
        {
            var result = (from c in dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == senderId).ToList()
                          join d in dbContext.IdentityCardType on c.IdentificationTypeId equals d.Id into gj
                          from e in gj.DefaultIfEmpty()
                          join f in dbContext.Country on c.IssuingCountry equals f.CountryCode
                          select new MobileSenderIdentityInformationViewModel()
                          {
                              Id = c.Id,
                              IdentityNumber = c.IdentityNumber,
                              IdentificationTypeName = c.IdentificationTypeId == 0 ? "" : e.CardType,
                              FormatedIdentityNumber = string.IsNullOrEmpty(c.IdentityNumber) ? "" : "***" + c.IdentityNumber.Substring(Math.Max(0, c.IdentityNumber.Length - 4)),
                              FormatedIdentityName = c.IdentificationTypeId == 0 ? "" : e.CardType.Substring(0, 1) + "***",
                              DocumentExpires = c.DocumentExpires,
                              Status = c.Status,
                              SenderId = c.SenderId,
                              AccountNo = c.AccountNo,
                              City = c.City,
                              Country = c.Country,
                              DocumentName = c.DocumentName,
                              DocumentPhotoUrl = c.DocumentPhotoUrl,
                              DocumentType = c.DocumentType,
                              ExpiryDate = c.ExpiryDate,
                              IdentificationTypeId = c.IdentificationTypeId,
                              IsUploadedFromSenderPortal = c.IsUploadedFromSenderPortal,
                              IssuingCountryCode = c.IssuingCountry,
                              ExpiryYearMonth = c.ExpiryDate == null ? "" : c.ExpiryDate.Value.Month + "/" + c.ExpiryDate.Value.Year,
                              ExpiryMonth = c.ExpiryDate == null ? "" : c.ExpiryDate.Value.Month.ToString(),
                              ExpiryYear = c.ExpiryDate == null ? "" : c.ExpiryDate.Value.Year.ToString(),
                              ExpiryDay = c.ExpiryDate == null ? "" : c.ExpiryDate.Value.Day.ToString(),
                              IssuingCountry = f.CountryName,


                          }).ToList();

            return result;

        }

        public MobileSenderIdentityInformationViewModel GetIdentityInformationBySenderId(int senderId)
        {
            var result = (from c in dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == senderId).ToList()
                          join d in dbContext.IdentityCardType on c.IdentificationTypeId equals d.Id into gj
                          from e in gj.DefaultIfEmpty()
                          join f in dbContext.Country on c.IssuingCountry equals f.CountryCode
                          select new MobileSenderIdentityInformationViewModel()
                          {
                              Id = c.Id,
                              IdentityNumber = c.IdentityNumber,
                              IdentificationTypeName = c.IdentificationTypeId == 0 ? "" : e.CardType,
                              FormatedIdentityNumber = string.IsNullOrEmpty(c.IdentityNumber) ? "" : "***" + c.IdentityNumber.Substring(Math.Max(0, c.IdentityNumber.Length - 4)),
                              FormatedIdentityName = c.IdentificationTypeId == 0 ? "" : e.CardType.Substring(0, 1) + "***",
                              DocumentExpires = c.DocumentExpires,
                              Status = c.Status,
                              SenderId = c.SenderId,
                              AccountNo = c.AccountNo,
                              City = c.City,
                              Country = c.Country,
                              DocumentName = c.DocumentName,
                              DocumentPhotoUrlTwo = c.DocumentPhotoUrlTwo,
                              DocumentPhotoUrl = c.DocumentPhotoUrl,
                              DocumentType = c.DocumentType,
                              ExpiryDate = c.ExpiryDate,
                              IdentificationTypeId = c.IdentificationTypeId,
                              IsUploadedFromSenderPortal = c.IsUploadedFromSenderPortal,
                              IssuingCountryCode = c.IssuingCountry,
                              ExpiryYearMonth = c.ExpiryDate == null ? "" : c.ExpiryDate.Value.Month + "/" + c.ExpiryDate.Value.Year,
                              ExpiryMonth = c.ExpiryDate == null ? "" : c.ExpiryDate.Value.Month.ToString(),
                              ExpiryYear = c.ExpiryDate == null ? "" : c.ExpiryDate.Value.Year.ToString(),
                              ExpiryDay = c.ExpiryDate == null ? "" : c.ExpiryDate.Value.Day.ToString(),
                              IssuingCountry = f.CountryName,


                          }).FirstOrDefault();

            return result;

        }


        internal MobilePaymentCreditDebitCardDetailsVm GetSavedDebitCreditCard(int senderId)
        {
            SenderCommonFunc _senderCommonFunc = new SenderCommonFunc();
            var savedCard = _senderCommonFunc.GetSavedDebitCreditCardDetails(senderId).FirstOrDefault();
            var senderInfo = FAXER.PORTAL.Common.Common.GetSenderInfo(senderId);
            if (savedCard != null)
            {
                MobilePaymentCreditDebitCardDetailsVm savedDebitCreditCard = new MobilePaymentCreditDebitCardDetailsVm()
                {
                    CardNumber = savedCard.CardNumber,
                    CardHolderName = savedCard.CardHolderName,
                    EndDate = savedCard.ExpiringDateYear,
                    EndMonth = savedCard.ExpiringDateMonth,
                    SecurityCode = savedCard.SecurityCode,
                    ExpiryDate = savedCard.ExpiringDateMonth + "/" + savedCard.ExpiringDateYear,
                    Address = senderInfo.Address1,
                    DecryptedCardNumber = savedCard.DecryptedCardNumber
                };
                return savedDebitCreditCard;
            }
            return new MobilePaymentCreditDebitCardDetailsVm();
        }

        public List<MobileDropdownViewModel> GetIdentityInformationTypeList()
        {
            var result = (from c in dbContext.IdentityCardType.Where(x => x.IsDeleted == false).ToList()
                          select new MobileDropdownViewModel()
                          {
                              Id = c.Id,
                              Name = c.CardType
                          }).ToList();

            return result;

        }

        internal CustomerPaymentFeeViewModel GetCreditDebitFee(string CountryCode)
        {
            var data = FAXER.PORTAL.Common.Common.CustomerPaymentFee(CountryCode);
            var result = new CustomerPaymentFeeViewModel();
            if (data != null)
            {
                result = new CustomerPaymentFeeViewModel()
                {
                    Id = data.Id,
                    BankTransfer = data.BankTransfer,
                    CreditCard = data.CreditCard,
                    DebitCard = data.DebitCard
                };
                return result;
            }
            result = new CustomerPaymentFeeViewModel()
            {
                BankTransfer = 0.79M
            };
            return result;
        }

        public List<SavedCard> GetSavedCards()
        {
            var result = (from c in dbContext.SavedCard.Where(x => x.IsDeleted == false).ToList()
                          select new SavedCard()
                          {
                              Id = c.Id,
                              Num = c.Num,
                              EMonth = c.EMonth.Decrypt(),
                              EYear = c.EYear.Decrypt(),
                              ClientCode = c.ClientCode.Decrypt(),
                              CreatedDate = c.CreatedDate,
                              CardName = c.CardName,
                              IsDeleted = c.IsDeleted,
                              Module = c.Module,
                              Remark = c.Remark,
                              Type = c.Type,
                              UserId = c.UserId
                          }).ToList();

            return result;

        }


        public List<KiiPayBusinessMobileSavedBankAccountViewModel> GetBankAccountByKiiPayBusinessId(int kiiPayBusinessId)
        {
            var result = (from c in dbContext.SavedBank.Where(x => x.UserId == kiiPayBusinessId && x.isDeleted == false && x.UserType == Module.KiiPayBusiness).ToList()
                          select new KiiPayBusinessMobileSavedBankAccountViewModel()
                          {
                              AccountNumber = c.AccountNumber,
                              CountryCode = c.Country,
                              BankId = c.BankId,
                              BankName = c.BankName,
                              BranchId = c.BranchId,
                              KiiPayBusinessId = c.UserId,
                              AccountOwnerName = c.OwnerName,
                              BranchCode = c.BranchCode,
                              BranchName = c.BranchName,
                              BankAccountId = c.Id,
                              CountryName = FAXER.PORTAL.Common.Common.GetCountryName(c.Country),
                              FormattedAccountNumber = c.AccountNumber.FormatSavedBankNumber()

                          }).ToList();

            return result;

        }

        public ServiceResult<MobileSenderIdentityInformationViewModel> AddIdentityInformation(MobileSenderIdentityInformationViewModel model)
        {
            /// AccountNo is not being sent from the mobile part 
            /// do check on this part my fellows 
            //var data = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == model.SenderId 
            //&& x.AccountNo == model.AccountNo && x.DocumentName == model.DocumentName).FirstOrDefault();

            var data = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == model.SenderId).FirstOrDefault();
            if (data == null)
            {
                var accountNo = dbContext.FaxerInformation.Where(x => x.Id == model.SenderId).FirstOrDefault().AccountNo;
                SenderBusinessDocumentation senderBusinessDocumentation = new SenderBusinessDocumentation()
                {
                    AccountNo = accountNo, //model.AccountNo,
                    DocumentType = (DocumentType)model.IdentificationTypeId,
                    IdentificationTypeId = model.IdentificationTypeId,
                    Country = model.Country,
                    CreatedDate = DateTime.Now,
                    DocumentExpires = DocumentExpires.Yes,
                    SenderId = model.SenderId,
                    DocumentName = model.IdentityNumber,
                    DocumentPhotoUrl = model.DocumentPhotoUrl,
                    DocumentPhotoUrlTwo = model.DocumentPhotoUrlTwo,
                    ExpiryDate = model.ExpiryDate,
                    IdentityNumber = model.IdentityNumber,
                    Status = DocumentApprovalStatus.InProgress,
                    IssuingCountry = model.IssuingCountryCode,
                    City = model.City,

                };
                dbContext.SenderBusinessDocumentation.Add(senderBusinessDocumentation);
                dbContext.SaveChanges();
                Log.Write("AccountNo :" + model.AccountNo + "ID Card Uploaded" + " " + model.SenderId);
                return new ServiceResult<MobileSenderIdentityInformationViewModel>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.OK

                };
            }
            else
            {
                model.Id = data.Id;
                var result = UpdateIdentityInformation(model);
                return new ServiceResult<MobileSenderIdentityInformationViewModel>()
                {
                    Data = result,
                    Message = "Already Exist",
                    Status = ResultStatus.OK

                };
            }

        }

        public MobileSenderIdentityInformationViewModel UpdateIdentityInformation(MobileSenderIdentityInformationViewModel model)
        {
            var data = dbContext.SenderBusinessDocumentation.Where(x => x.Id == model.Id).FirstOrDefault();
            if (data != null)
            {
                data.IdentificationTypeId = model.IdentificationTypeId;
                data.DocumentType = model.DocumentType;
                data.DocumentExpires = model.DocumentExpires;
                data.DocumentName = model.DocumentName;
                data.DocumentPhotoUrl = model.DocumentPhotoUrl;
                data.DocumentPhotoUrlTwo = model.DocumentPhotoUrlTwo;
                data.ExpiryDate = model.ExpiryDate;
                data.IdentityNumber = model.IdentityNumber;
                data.Status = DocumentApprovalStatus.InProgress;
                data.IssuingCountry = model.IssuingCountryCode;
                data.City = model.City;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
            }
            return model;
        }


        public bool DeleteIdentityInformation(int identityId)
        {
            var model = dbContext.SenderBusinessDocumentation.Where(x => x.Id == identityId).FirstOrDefault();
            if (model != null)
            {
                dbContext.SenderBusinessDocumentation.Remove(model);
                dbContext.SaveChanges();
            }
            return true;
        }
        private bool getStatusByExpDate(string eMonth, string eYear)
        {
            var currentDate = DateTime.Now;
            eYear = currentDate.Year.ToString().Substring(0, 2) + "" + eYear.Decrypt();
            if (int.Parse(eYear) > currentDate.Year)
            {
                return true;
            }
            else if (int.Parse(eYear) == currentDate.Year && int.Parse(eMonth.Decrypt()) > currentDate.Month)
            {

                return true;
            }
            else
            {
                return false;

            }

        }
        public void SendVerificationCodeSMS(string verificationCode, string PhoneNo)
        {

            // Sms Function Execute Here 
            SmsApi smsApiServices = new SmsApi();
            string message = smsApiServices.GetRegistrationMessage(verificationCode);

            smsApiServices.SendSMS(PhoneNo, message);
            //Email 
        }

        public List<RecentlyPaidKiiPayPersonalViewModel> GetAllRecenltyPaidKiiPayPersonalWalletInfo(int KiiPayBusinessInfoId)
        {

            var result = (from c in dbContext.KiiPayPersonalWalletPaymentByKiiPayBusiness.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessInfoId)
                          select new RecentlyPaidKiiPayPersonalViewModel()
                          {
                              WalletId = c.KiiPayPersonalWalletInformation.Id,
                              MobileNo = c.KiiPayPersonalWalletInformation.MobileNo,
                              ReceiverIsLocal = c.PaymentType == PaymentType.Local ? true : false,
                              Country = c.KiiPayPersonalWalletInformation.CardUserCountry,
                              FullName = c.KiiPayPersonalWalletInformation.FirstName + " " + c.KiiPayPersonalWalletInformation.MiddleName + " " + c.KiiPayPersonalWalletInformation.LastName
                          }).GroupBy(x => x.MobileNo).Select(x => x.FirstOrDefault()).ToList();
            return result;

        }
        public DB.SavedCard SaveCreditDebitCardInformation(DB.SavedCard model)
        {
            var savedCard = SavedCardSenderModule().Data.Where(x => x.Num == model.Num).FirstOrDefault();
            if (savedCard != null)
            {
                return null;

            }
            else
            {
                dbContext.SavedCard.Add(model);
                dbContext.SaveChanges();


                return model;

            }
        }
        public DB.SavedCard UpateCreditDebitCardInformation(DB.SavedCard model)
        {
            dbContext.Entry(model).State = EntityState.Modified;
            dbContext.SaveChanges();
            return model;

        }
        public bool DeleteCreditDebitCardInformation(DB.SavedCard model)
        {
            dbContext.SavedCard.Remove(model);
            dbContext.SaveChanges();

            // Send Email
            FAXER.PORTAL.Areas.Admin.Services.EmailServices _emailServices = new FAXER.PORTAL.Areas.Admin.Services.EmailServices();
            _emailServices.CardDeletionEmail(model);
            return true;
        }
        public DB.SenderBusinessDocumentation SaveSenderBusinessDocumentation(DB.SenderBusinessDocumentation model)
        {
            dbContext.SenderBusinessDocumentation.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        public bool DeleteSenderBusinessDocumentation(DB.SenderBusinessDocumentation model)
        {
            dbContext.SenderBusinessDocumentation.Remove(model);
            dbContext.SaveChanges();
            return true;
        }
        public DB.SenderBusinessDocumentation UpdateSenderBusinessDocumentation(DB.SenderBusinessDocumentation model)
        {
            dbContext.Entry<SenderBusinessDocumentation>(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return model;
        }

        public DB.SavedBank SaveBankAccountInformation(DB.SavedBank model)
        {
            dbContext.SavedBank.Add(model);
            dbContext.SaveChanges();
            return model;
        }
        public DB.SavedCard UpdateCreditDebitCardInformation(DB.SavedCard model)

        {
            var local = dbContext.Set<SavedCard>()
                         .Local
                         .FirstOrDefault(f => f.Id == model.Id);
            if (local != null)
            {
                dbContext.Entry(local).State = EntityState.Detached;
            }
            dbContext.Entry<SavedCard>(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return model;
        }
        public DB.SavedBank UpdateBankAccountInformation(DB.SavedBank model)
        {
            dbContext.Entry(model).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return model;
        }

        public ServiceResult<IQueryable<SavedCard>> SavedCardList()
        {
            return new ServiceResult<IQueryable<SavedCard>>
            {
                Data = dbContext.SavedCard.Where(x => x.Module == DB.Module.KiiPayBusiness && x.IsDeleted == false),
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<IQueryable<SavedCard>> SavedCardSenderModule()
        {
            return new ServiceResult<IQueryable<SavedCard>>
            {
                Data = dbContext.SavedCard.Where(x => x.Module == DB.Module.Faxer && x.IsDeleted == false),
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<bool> IsServiceAvailable(string SendingCountry, string ReceivingCountry, decimal SendingAmount, TransactionTransferMethod transferMethod)
        {

            var result = FAXER.PORTAL.Common.Common.GetApiservice(SendingCountry, ReceivingCountry, SendingAmount,  transferMethod);
            if (result == null)
            {
                return new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "Service Not Available",
                    Status = ResultStatus.Error,
                };
            }
            return new ServiceResult<bool>()
            {
                Data = true,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<IQueryable<SavedBank>> BankAccountList()
        {
            return new ServiceResult<IQueryable<SavedBank>>
            {
                Data = dbContext.SavedBank,
                Status = ResultStatus.OK
            };
        }

        public List<KiiPayBusinessMobileNotificationViewModel> GetNotification(int ReceiverId)
        {
            var data = dbContext.Notification.Where(x => x.NotificationReceiver == NotificationFor.KiiPayBusiness && x.ReceiverId == ReceiverId).OrderByDescending(x => x.CreationDate).ToList();
            var result = (from c in data.ToList()
                          select new KiiPayBusinessMobileNotificationViewModel
                          {
                              Amount = c.Amount,
                              CreationDate = c.CreationDate,
                              Id = c.Id,
                              IsSeen = c.IsSeen,
                              Message = c.Message,
                              Name = c.Name,
                              NotificationReceiver = Enum.GetName(typeof(NotificationFor), c.NotificationReceiver),
                              NotificationSender = Enum.GetName(typeof(NotificationFor), c.NotificationSender),
                              ReceiverId = c.ReceiverId,
                              SenderId = c.SenderId,
                              Title = Enum.GetName(typeof(Title), c.Title),

                          }).ToList();
            return result;
        }

        internal List<KiiPayBusinessInvoiceMaster> GetInvoiceDetails(int kiipayBusinessId)
        {

            var data = dbContext.KiiPayBusinessInvoiceMaster.Where(x => x.ReceiverId == kiipayBusinessId && x.InvoiceStatus != DB.InvoiceStatus.Deleted).ToList();
            return data;

        }

        public List<IdentificationTypeDropDownVm> GetIdentificationTypes()
        {

            var result = (from c in dbContext.IdentityCardType.ToList()
                          select new IdentificationTypeDropDownVm()
                          {
                              Id = c.Id,
                              Name = c.CardType
                          }).ToList();
            return result;
        }

        public ServiceResult<bool> IsValidMobileAccount(string sendingCountry, string receivingCountry, string mobileNo, decimal sendingAmount, string receivingCountryPhoneCode)
        {
            FAXER.PORTAL.Services.SSenderMobileMoneyTransfer _SSenderMobileMoneyTransfer = new FAXER.PORTAL.Services.SSenderMobileMoneyTransfer();

            FAXER.PORTAL.Models.SenderMobileMoneyTransferVM model = new FAXER.PORTAL.Models.SenderMobileMoneyTransferVM()
            {
                MobileNumber = mobileNo,
                CountryCode = receivingCountry,
                CountryPhoneCode = receivingCountryPhoneCode

            };
            var IsValidAccount = _SSenderMobileMoneyTransfer.IsValidMobileAccount(model, sendingAmount, sendingCountry).Data;

            return new ServiceResult<bool>()
            {
                Data = IsValidAccount,
                Message = "",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<SenderAddressVm> getSenderAddressLineOne(int senderId)
        {
            SenderAddressVm vm = new SenderAddressVm();
            string AddressLineOne = dbContext.FaxerInformation.Where(x => x.Id == senderId).Select(x => x.Address1).FirstOrDefault();
            vm.AddressLine = AddressLineOne;
            return new ServiceResult<SenderAddressVm>()
            {
                Data = vm,
                Message = "",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<bool> getIsMobileExist(string mobileNo)
        {
            bool mobileNoexist = false;
            if (dbContext.FaxerInformation.Where(x => x.PhoneNumber == mobileNo).Count() > 0)
            {
                mobileNoexist = true;
            }
            return new ServiceResult<bool>()
            {
                Data = mobileNoexist,
                Message = "",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<bool> getIsEmailExist(string email)
        {
            bool EmailExist = false;
            if (dbContext.FaxerInformation.Where(x => x.Email == email.Trim()).Count() > 0)
            {
                EmailExist = true;
            }
            return new ServiceResult<bool>()
            {
                Data = EmailExist,
                Message = "",
                Status = ResultStatus.OK
            };
        }
        public ServiceResult<bool> getIsEmailValid(string email)
        {
            bool EmialValid = false;
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                EmialValid = true;
            }
            catch
            {
            }
            return new ServiceResult<bool>()
            {
                Data = EmialValid,
                Message = "",
                Status = ResultStatus.OK
            };
        }

        public ServiceResult<string> GetCountryPhoneCode(string countryCode)
        {
            var PhoneCode = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(countryCode);
            return new ServiceResult<string>()
            {
                Data = PhoneCode,
                Message = "",
                Status = ResultStatus.OK

            };
        }

        public ServiceResult<IdentificationDetailvm> GetIdentityInformation(int senderId)
        {
            var senderDocumentation = dbContext.SenderBusinessDocumentation.Where(x => x.SenderId == senderId).FirstOrDefault();
            if (senderDocumentation == null)
            {
                return new ServiceResult<IdentificationDetailvm>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.OK
                };
            }
            else
            {
                bool IsIdentityDocumentExist = true;
                bool IsIdentityDocumentExpired = false;
                var currentDate = DateTime.Now;
                //if (senderDocumentation.DocumentExpires == DocumentExpires.Yes)
                //{
                //}
                if (senderDocumentation.ExpiryDate < currentDate)
                {
                    IsIdentityDocumentExpired = true;
                }
                else
                {
                    IsIdentityDocumentExpired = false;
                }

                IdentificationDetailvm identityDetails = new IdentificationDetailvm()
                {
                    Id = senderDocumentation.Id,
                    IdentificationTypeId = senderDocumentation.IdentificationTypeId,
                    IdentificationTypeName = FAXER.PORTAL.Common.Common.GetIDCardTypeName(senderDocumentation.IdentificationTypeId),
                    IdentityNumber = senderDocumentation.IdentityNumber,
                    ExpiryDate = senderDocumentation.ExpiryDate,
                    ExpiryDay = senderDocumentation.ExpiryDate == null ? "" : senderDocumentation.ExpiryDate.Value.Day.ToString(),
                    ExpiryYear = senderDocumentation.ExpiryDate == null ? "" : senderDocumentation.ExpiryDate.Value.Year.ToString(),
                    ExpiryMonth = senderDocumentation.ExpiryDate == null ? "" : senderDocumentation.ExpiryDate.Value.Month.ToString(),
                    IssuingCountryCode = senderDocumentation.IssuingCountry,
                    IssuingCountry = FAXER.PORTAL.Common.Common.GetCountryName(senderDocumentation.IssuingCountry),
                    IsIdentityDocumentExist = IsIdentityDocumentExist,
                    IsIdentityDocumentExpired = IsIdentityDocumentExpired,
                    DocumentPhotoUrl = senderDocumentation.DocumentPhotoUrl,
                    AccountNo = senderDocumentation.AccountNo,
                    City = senderDocumentation.City,
                    Country = senderDocumentation.Country,
                    Status = senderDocumentation.Status,
                    SenderId = senderDocumentation.SenderId,
                    DocumentExpires = senderDocumentation.DocumentExpires,
                    DocumentName = senderDocumentation.DocumentName,
                    DocumentPhotoUrlTwo = senderDocumentation.DocumentPhotoUrlTwo,
                    IsUploadedFromSenderPortal = senderDocumentation.IsUploadedFromSenderPortal,
                    DocumentType = senderDocumentation.DocumentType,

                };


                return new ServiceResult<IdentificationDetailvm>()
                {
                    Data = identityDetails,
                    Message = "",
                    Status = ResultStatus.OK
                };

            }

        }
        public ServiceResult<SenderInformationViewModel> GetSenderInformation(int senderId)
        {
            SenderCommonFunc senderCommonFunc = new SenderCommonFunc();
            var senderInfo = dbContext.FaxerInformation.Where(x => x.Id == senderId).FirstOrDefault();
            if (senderInfo == null)
            {
                return new ServiceResult<SenderInformationViewModel>()
                {
                    Data = new SenderInformationViewModel(),
                    Message = "",
                    Status = ResultStatus.Error
                };
            }
            else
            {
                SenderInformationViewModel senderInformation = new SenderInformationViewModel()
                {
                    Country = FAXER.PORTAL.Common.Common.GetCountryName(senderInfo.Country),
                    CountryCode = senderInfo.Country,
                    CountryPhoneCode = FAXER.PORTAL.Common.Common.GetCountryPhoneCode(senderInfo.Country),
                    CurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(senderInfo.Country),
                    CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(senderInfo.Country),
                    Email = senderInfo.Email,
                    MobileNo = senderInfo.PhoneNumber,
                    SenderId = senderInfo.Id,
                    SenderName = senderInfo.FirstName + " " + senderInfo.MiddleName + " " + senderInfo.LastName,
                    IsBusiness = senderInfo.IsBusiness,
                    MonthlyTransactionLimitAmount = senderCommonFunc.GetMonthlyTransactionMeter(senderId),
                };

                return new ServiceResult<SenderInformationViewModel>()
                {
                    Data = senderInformation,
                    Message = "",
                    Status = ResultStatus.OK
                };

            }

        }
    }
    public class SenderAddressVm
    {
        public string AddressLine { get; set; }
    }
    public class IdentificationTypeDropDownVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}