using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.Services;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Areas.Mobile.Models.Common;
using FAXER.PORTAL.Areas.Mobile.Models.KiiPayBusiness;
using FAXER.PORTAL.Areas.Mobile.Models.MoneyFex;
using FAXER.PORTAL.Areas.Mobile.Services.Common;
using FAXER.PORTAL.Areas.Mobile.Services.KiiPayBusiness;
using FAXER.PORTAL.Areas.Mobile.Services.MoneyFex;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Mobile.Controllers.Common
{

    // GET: Mobile/Common
    public class CommonController : Controller
    {
        MobileCommonServices mobileCommonServices = null;
        MobileKiiPayBusinessInformationServices mobileKiiPayBusinessInformationServices = null;
        MobileKiiPayBusinessLoginServices mobileKiiPayBusinessLoginServices = null;
        MobileKiiPayBusinessWalletInformationServices mobileKiiPayBusinessWalletInformationServices = null;

        public CommonController()
        {
            mobileCommonServices = new MobileCommonServices();
            mobileKiiPayBusinessInformationServices = new MobileKiiPayBusinessInformationServices();
            mobileKiiPayBusinessLoginServices = new MobileKiiPayBusinessLoginServices();
            mobileKiiPayBusinessWalletInformationServices = new MobileKiiPayBusinessWalletInformationServices();

        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetCountry()
        {
            try
            {
                var result = mobileCommonServices.CountryList().Data.ToList();
                if (result.Count > 0)
                {
                    var data = (from c in result.ToList()
                                select new MobileCountryDropdownViewModel()
                                {
                                    CountryCode = c.CountryCode,
                                    CountryCurrency = c.Currency,
                                    CountryName = c.CountryName,
                                    CountryPhoneCode = c.CountryPhoneCode,
                                    CurrencySymbol = c.CurrencySymbol,
                                    FlagCode = c.FlagCode
                                }).ToList();
                    return Json(new ServiceResult<List<MobileCountryDropdownViewModel>>()
                    {
                        Data = data,
                        Message = "",
                        Status = ResultStatus.OK
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ServiceResult<List<MobileCountryDropdownViewModel>>()
                    {
                        Data = null,
                        Message = "",
                        Status = ResultStatus.Error
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new ServiceResult<List<MobileCountryDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }

        }

        public JsonResult GetDefaultReceivingCountryCodeAndCurrency()
        {
            try
            {
                MobileMoneyFexLoginServices mobileMoneyFexLoginServices = new MobileMoneyFexLoginServices();
                var result = mobileMoneyFexLoginServices.GetDefaultRecevingCountryAndCurrency();
                return Json(new ServiceResult<DefaultReceivingCountryViewModel>()
                {
                    Data = result.Data,
                    Message = result.Message,
                    Status = result.Status
                }, JsonRequestBehavior.AllowGet);
            }


            catch (Exception ex)
            {
                return Json(new ServiceResult<DefaultReceivingCountryViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }

        }


        [HttpGet]
        public JsonResult GetServicesAvailableCountries()
        {

            try
            {
                string token = FAXER.PORTAL.Common.Common.RequestToken;
                if (FAXER.PORTAL.Common.Common.ValidateToken(token))
                {
                    var result = mobileCommonServices.ServiceEnabledCountryList().Data.ToList();
                    if (result.Count > 0)
                    {
                        var data = (from c in result.ToList()
                                    select new MobileCountryDropdownViewModel()
                                    {
                                        CountryCode = c.CountryCode,
                                        CountryCurrency = c.Currency,
                                        CountryName = c.CountryName,
                                        CountryPhoneCode = c.CountryPhoneCode,
                                        CurrencySymbol = c.CurrencySymbol,
                                        FlagCode = c.FlagCode
                                    }).ToList();
                        return Json(new ServiceResult<List<MobileCountryDropdownViewModel>>()
                        {
                            Data = data,
                            Message = "",
                            Status = ResultStatus.OK
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new ServiceResult<List<MobileCountryDropdownViewModel>>()
                        {
                            Data = null,
                            Message = "",
                            Status = ResultStatus.Error
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new ServiceResult<List<MobileCountryDropdownViewModel>>()
                    {
                        Data = null,
                        Message = "",
                        Status = ResultStatus.Warning
                    }, JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                return Json(new ServiceResult<List<MobileCountryDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpGet]
        public JsonResult GetReceivingCountries()
        {
            var MobileCountryDropdownvm = new ServiceResult<List<MobileCountryDropdownViewModel>>();
            try
            {
                string token = FAXER.PORTAL.Common.Common.RequestToken;
                if (FAXER.PORTAL.Common.Common.ValidateToken(token))
                {
                    var result = mobileCommonServices.CountriesWithCurrency().Data.ToList();

                    if (result.Count > 0)
                    {
                        MobileCountryDropdownvm.Data = result;
                        MobileCountryDropdownvm.Status = ResultStatus.OK;
                    }
                    else
                    {
                        MobileCountryDropdownvm.Data = null;
                        MobileCountryDropdownvm.Status = ResultStatus.Error;
                    }
                }
                else
                {
                    MobileCountryDropdownvm.Data = null;
                    MobileCountryDropdownvm.Status = ResultStatus.Warning;

                }

            }
            catch (Exception ex)
            {
                MobileCountryDropdownvm.Data = null;
                MobileCountryDropdownvm.Status = ResultStatus.Warning;

            }
            return Json(MobileCountryDropdownvm, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult IsMobileNoValid(string CountryPhoneCode, string mobileNo)
        {
            try
            {
                var result = mobileCommonServices.getIsMobilevalid(CountryPhoneCode, mobileNo);
                return Json(result
                , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "",
                    Status = ResultStatus.Error

                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult IsMobileNoExistInSender(string mobileNo)
        {
            try
            {
                var result = mobileCommonServices.getIsMobileExist(mobileNo);

                return Json(result
                , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "",
                    Status = ResultStatus.Error

                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult IsEmailValid(string email)
        {
            try
            {
                var result = mobileCommonServices.getIsEmailValid(email);
                return Json(result
                , JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Status = ResultStatus.Warning,
                    Message = ""

                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult IsEmailExistInSender(string email)
        {
            try
            {
                var result = mobileCommonServices.getIsEmailExist(email);

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult GetAllBanks()
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = (from c in mobileCommonServices.BankList().Data.ToList()
                              select new MobileBankDropdownViewModel()
                              {
                                  Code = c.Code,
                                  CountryCode = c.CountryCode,
                                  Id = c.Id,
                                  Name = c.Name
                              }).ToList();
                return Json(new ServiceResult<List<MobileBankDropdownViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileBankDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpGet]
        public JsonResult GetAllBranches()
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = (from c in mobileCommonServices.BranchList().Data.ToList()
                              select new MobileBranchDropdownViewModel()
                              {
                                  BankId = c.BankId,
                                  Code = c.BranchCode,
                                  Id = c.Id,
                                  Name = c.BranchName,

                              }).ToList();
                return Json(new ServiceResult<List<MobileBranchDropdownViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileBranchDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetWalletOperators()
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = (from c in mobileCommonServices.MobileOperatorsList().Data.ToList()
                              select new MobileBankDropdownViewModel()
                              {
                                  Code = c.Code,
                                  CountryCode = c.Country,
                                  Id = c.Id,
                                  Name = c.Name
                              }).ToList();
                return Json(new ServiceResult<List<MobileBankDropdownViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileBankDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetMobileWalletOperators()
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = (from c in mobileCommonServices.MobileOperatorsList().Data.ToList()
                              select new MobileWalletDropDownVm()
                              {

                                  CountryCode = c.Country,
                                  WalletId = c.Id,
                                  WalletName = c.Name
                              }).ToList();
                return Json(new ServiceResult<List<MobileWalletDropDownVm>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileWalletDropDownVm>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetSuppliers()
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = (from c in mobileCommonServices.SuppliersList().Data.Where(x => x.IsActive == true).ToList()
                              select new MobileSuppliersDropdownViewModel()
                              {
                                  CountryCode = c.Country,
                                  Id = c.Id,
                                  KiiPayBusinessId = c.KiiPayBusinessInformationId,
                                  RefCode = c.RefCode,
                                  KiiPayBusinessName = c.KiiPayBusinessInformation.BusinessName
                              }).ToList();
                return Json(new ServiceResult<List<MobileSuppliersDropdownViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileSuppliersDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }

        }
        [HttpGet]
        public JsonResult GetWalletInformationList(int KiiPayBusinessId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = (from c in mobileCommonServices.WalletList().Data.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessId).ToList()
                              select new MobileWalletDropdownViewModel()
                              {
                                  AddressLine1 = c.AddressLine1,
                                  AddressLine2 = c.AddressLine2,
                                  City = c.City,
                                  Country = c.Country,
                                  DOB = c.DOB,
                                  Email = c.Email,
                                  FullName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                                  Id = c.Id,
                                  IdCardNumber = c.IdCardNumber,
                                  IdCardType = c.IdCardType,
                                  IdExpiryDate = c.IdExpiryDate,
                                  IdIssuingCountry = c.IdIssuingCountry,
                                  KiiPayBusinessInformationId = c.KiiPayBusinessInformationId,
                                  MobileNo = c.MobileNo,
                                  PostalCode = c.PostalCode,
                                  State = c.State
                              }).ToList();
                return Json(new ServiceResult<List<MobileWalletDropdownViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileWalletDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }

        }



        [HttpGet]
        public JsonResult GetBankbyCountryCode(string CountryCode)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = (from c in mobileCommonServices.BankList().Data.Where(x => x.CountryCode == CountryCode).ToList()
                              select new MobileBankDropdownViewModel()
                              {
                                  CountryCode = c.CountryCode,
                                  Code = c.Code,
                                  Id = c.Id,
                                  Name = c.Name
                              }).ToList();
                return Json(new ServiceResult<List<MobileBankDropdownViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileBankDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetAddressLineOneBySenderId(int SenderId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var addressLineOne = mobileCommonServices.getSenderAddressLineOne(SenderId);
                return Json(addressLineOne, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<SenderAddressVm>()
                {
                    Data = null,
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }


        [HttpGet]
        public JsonResult GetBranchbyBankId(int BankId)
        {

            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = (from c in mobileCommonServices.BranchList().Data.Where(x => x.BankId == BankId).ToList()
                              select new MobileBranchDropdownViewModel()
                              {
                                  BankId = c.BankId,
                                  Name = c.BranchName,
                                  Id = c.Id,
                                  Code = c.BranchCode
                              }).ToList();
                return Json(new ServiceResult<List<MobileBranchDropdownViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileBranchDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetAllRecenltyPaidKiiPayPersonalWalletInfo(int KiiPayBusinessId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = (from c in mobileCommonServices.GetAllRecenltyPaidKiiPayPersonalWalletInfo(KiiPayBusinessId).Where(x => x.ReceiverIsLocal == true).ToList()
                              select new RecentlyPaidKiiPayPersonalViewModel()
                              {
                                  Country = c.Country,
                                  FullName = c.FullName,
                                  MobileNo = c.MobileNo,
                                  ReceiverIsLocal = c.ReceiverIsLocal,
                                  WalletId = c.WalletId
                              }).ToList();
                return Json(new ServiceResult<List<RecentlyPaidKiiPayPersonalViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<RecentlyPaidKiiPayPersonalViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }


        public JsonResult GetBusinessStandingOrderList(int BusinessId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                MobileKiiPayBusinessInformationServices _mobileKiiPayBusinessInformationServices = new MobileKiiPayBusinessInformationServices();
                var kiipayinfo = _mobileKiiPayBusinessInformationServices.List().Data.Where(x => x.Id == BusinessId).ToList();
                var data = mobileCommonServices.KiiPayBusinessBusinessStandingOrderInfoList().Data.Where(x => x.SenderId == BusinessId).ToList();
                var result = (from c in data
                              join d in kiipayinfo on c.SenderId equals d.Id
                              select new KiiPayBusinessStandingOrderPaymentListVM()
                              {
                                  TransactionId = c.Id,
                                  Amount = c.Amount,
                                  City = d.BusinessOperationCity,
                                  Country = c.ReceivingCountry,
                                  CurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(c.ReceivingCountry),
                                  PaymentFrequency = c.Frequency,
                                  FrequencyDetail = FAXER.PORTAL.Common.Common.GetPaymentFrequncyDetail(c.Frequency, c.FrequencyDetail),
                                  MobileNo = d.BusinessMobileNo,
                                  Name = d.BusinessName.Substring(0, d.BusinessName.Length > 10 ? 10 : d.BusinessName.Length),
                                  ReceiverId = c.Id,
                                  IsEnabled = true
                              }).ToList();
                return Json(new ServiceResult<List<KiiPayBusinessStandingOrderPaymentListVM>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<KiiPayBusinessStandingOrderPaymentListVM>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpGet]
        public JsonResult GetSavedCardsByKiiPayBusinessId(int kiiPayBusinessId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = mobileCommonServices.GetSavedCardsByKiiPayBusinessId(kiiPayBusinessId).ToList();
                return Json(new ServiceResult<List<KiiPayBusinessSavedCreditDebitCardListVM>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<KiiPayBusinessSavedCreditDebitCardListVM>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetSavedCardsBySenderId(int senderId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = mobileCommonServices.GetSavedCardsBySenderId(senderId).ToList();
                return Json(new ServiceResult<List<KiiPayBusinessSavedCreditDebitCardListVM>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {


                return Json(new ServiceResult<List<KiiPayBusinessSavedCreditDebitCardListVM>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetIdentityInformationListBySenderId(int senderId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = mobileCommonServices.GetIdentityInformationListBySenderId(senderId);
                return Json(new ServiceResult<List<MobileSenderIdentityInformationViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileSenderIdentityInformationViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }
        [HttpGet]
        public JsonResult GetIdentityInformationTypeList()
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = mobileCommonServices.GetIdentityInformationTypeList().ToList();
                return Json(new ServiceResult<List<MobileDropdownViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<MobileDropdownViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult AddIdentity(MobileSenderIdentityInformationViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = mobileCommonServices.AddIdentityInformation(model);
                return Json(new ServiceResult<MobileSenderIdentityInformationViewModel>()
                {
                    Data = result.Data,
                    Message = result.Message,
                    Status = result.Status
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<MobileSenderIdentityInformationViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Error
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UpdateIdentity(MobileSenderIdentityInformationViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = mobileCommonServices.UpdateIdentityInformation(model);
                return Json(new ServiceResult<bool>()
                {
                    Data = true,
                    Message = "",
                    Status = ResultStatus.OK

                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "Invalid Token",
                    Status = ResultStatus.Warning,
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult DeleteIdentityInformation(int identityInformationId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var data = mobileCommonServices.DeleteIdentityInformation(identityInformationId);
                return Json(new ServiceResult<bool>()
                {
                    Data = data,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }


        [HttpGet]
        public JsonResult GetBankAccountByKiiPayBusinessId(int kiiPayBusinessId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = mobileCommonServices.GetBankAccountByKiiPayBusinessId(kiiPayBusinessId).ToList();
                return Json(new ServiceResult<List<KiiPayBusinessMobileSavedBankAccountViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<KiiPayBusinessMobileSavedBankAccountViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult RemoveCard(int cardId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = mobileCommonServices.RemoveCard(cardId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public JsonResult RemoveBankAccount(int BankAccountId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = mobileCommonServices.RemoveBankAccount(BankAccountId);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetAllRecenltyPaidInternationalKiiPayPersonalWalletInfo(int KiiPayBusinessId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                var result = (from c in mobileCommonServices.GetAllRecenltyPaidKiiPayPersonalWalletInfo(KiiPayBusinessId).Where(x => x.ReceiverIsLocal == false).ToList()
                              select new RecentlyPaidKiiPayPersonalViewModel()
                              {
                                  Country = c.Country,
                                  FullName = c.FullName,
                                  MobileNo = c.MobileNo,
                                  ReceiverIsLocal = c.ReceiverIsLocal,
                                  WalletId = c.WalletId
                              }).ToList();
                return Json(new ServiceResult<List<RecentlyPaidKiiPayPersonalViewModel>>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<RecentlyPaidKiiPayPersonalViewModel>>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpGet]
        public JsonResult IsMobileNoDuplicate(string MobileNo)
        {
            bool result = mobileCommonServices.IsMobileNoDuplicate(MobileNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsEmailDuplicate(string Email)
        {
            bool result = mobileCommonServices.IsEmailDuplicate(Email);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRegistrationCode(string CountryPhoneCode, string CountryCode, string MobileNo)
        {
            string result = mobileCommonServices.GenerateVerificationCode(CountryPhoneCode, CountryCode, 0, MobileNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetKiiPayPersonalId(string MobileNo, string Country)
        {
            int result = mobileCommonServices.GetKiiPayPersonalIdByMobileNo(MobileNo, Country);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetKiiPayBusinessId(string MobileNo)
        {
            int result = mobileCommonServices.GetKiiPayBusinessIdByMobileNo(MobileNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetKiiPayPersonalName(string MobileNo, string Country)
        {
            string result = mobileCommonServices.GetKiiPayPersonalNameByMobileNo(MobileNo, Country);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetKiiPayBusinessName(string MobileNo)
        {
            string result = mobileCommonServices.GetKiiPayBusinessNameByMobileNo(MobileNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsMobileNoRegistered(string CountryCode, string MobileNo)
        {
            bool result = mobileCommonServices.IsMobileNoRegistered(CountryCode, MobileNo);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult IsPassCodeCorrect(string MobileNo, string PassCode)
        {
            bool result = mobileCommonServices.IsPassCodeCorrect(MobileNo, PassCode);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult SendVerificationCodeSMS(string verificationCode, string CountryPhoneCode, string PhoneNo)
        {
            string PhoneNumber = CountryPhoneCode + " " + PhoneNo;


            mobileCommonServices.SendVerificationCodeSMS(verificationCode, PhoneNumber);


            return Json(true, JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetUserInformation(string MobileNo, string Passcode)
        {
            string EncryptPasscode = Passcode.Encrypt();

            int KiiPayBusinessId = mobileKiiPayBusinessLoginServices.List().Data.Where(x => x.IsActive == true && x.MobileNo == MobileNo && x.PinCode == EncryptPasscode).Select(x => x.KiiPayBusinessInformationId).FirstOrDefault();

            var result = (from c in mobileKiiPayBusinessLoginServices.List().Data.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessId).ToList()
                          join d in mobileKiiPayBusinessWalletInformationServices.List().Data.Where(x => x.KiiPayBusinessInformationId == KiiPayBusinessId) on c.KiiPayBusinessInformationId equals d.KiiPayBusinessInformationId
                          select new MobileUserInformationViewModel()
                          {
                              CompanyName = c.KiiPayBusinessInformation.BusinessName,
                              MobileNo = c.MobileNo,
                              PassCode = c.PinCode.Decrypt(),
                              KiiPayBusinessId = c.KiiPayBusinessInformationId,
                              CountryCode = c.KiiPayBusinessInformation.BusinessCountry,
                              CurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(c.KiiPayBusinessInformation.BusinessCountry),
                              CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(c.KiiPayBusinessInformation.BusinessCountry),
                              CurrentBalance = d.CurrentBalance,
                              BusinessAddress = d.City + "," + d.AddressLine1,
                              CountryPhoneCode = getCountryPhoneCodeByCountryCode(c.KiiPayBusinessInformation.BusinessCountry)
                          }).FirstOrDefault();
            return Json(new ServiceResult<MobileUserInformationViewModel>()
            {
                Data = result,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetUserInformationbykiiPayBusinessId(int kiiPayBusinessId)
        {


            var result = (from c in mobileKiiPayBusinessLoginServices.List().Data.Where(x => x.KiiPayBusinessInformationId == kiiPayBusinessId).ToList()
                          join d in mobileKiiPayBusinessWalletInformationServices.List().Data.Where(x => x.KiiPayBusinessInformationId == kiiPayBusinessId) on c.KiiPayBusinessInformationId equals d.KiiPayBusinessInformationId
                          select new MobileUserInformationViewModel()
                          {
                              CompanyName = c.KiiPayBusinessInformation.BusinessName,
                              MobileNo = c.MobileNo,
                              PassCode = c.PinCode.Decrypt(),
                              KiiPayBusinessId = c.KiiPayBusinessInformationId,
                              CountryCode = c.KiiPayBusinessInformation.BusinessCountry,
                              CurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(c.KiiPayBusinessInformation.BusinessCountry),
                              CurrencyCode = FAXER.PORTAL.Common.Common.GetCurrencyCode(c.KiiPayBusinessInformation.BusinessCountry),
                              CurrentBalance = d.CurrentBalance,
                              BusinessAddress = d.City + "," + d.AddressLine1,
                              CountryPhoneCode = getCountryPhoneCodeByCountryCode(c.KiiPayBusinessInformation.BusinessCountry)
                          }).FirstOrDefault();
            return Json(new ServiceResult<MobileUserInformationViewModel>()
            {
                Data = result,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }

        private string getCountryPhoneCodeByCountryCode(string businessCountry)
        {
            string phoneCode = mobileCommonServices.CountryList().Data.Where(x => x.CountryCode == businessCountry).Select(x => x.CountryPhoneCode).FirstOrDefault();
            return phoneCode;
        }

        [HttpGet]
        public JsonResult GetAllCurrencyExchange(string SendingCountryCode)
        {

            var result = SExchangeRate.GetAllCurrencyExchange(SendingCountryCode);

            return Json(new ServiceResult<List<MobileCurrencyDropDownViewModel>>()
            {
                Data = result,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetCurrencies()
        {
            var result = (from c in mobileCommonServices.CurrencyList().Data.ToList()
                          select new MobileCountryDropdownViewModel()
                          {
                              CountryCode = c.CountryCode,
                              CountryCurrency = c.Currency,
                              CountryName = c.CountryName,
                              CountryPhoneCode = c.CountryPhoneCode,
                              CurrencySymbol = c.CurrencySymbol,
                              FlagCode = c.FlagCode
                          }).ToList();
            return Json(new ServiceResult<List<MobileCountryDropdownViewModel>>()
            {
                Data = result,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetNotification(int ReceiverId)
        {

            var result = mobileCommonServices.GetNotification(ReceiverId);

            return Json(new ServiceResult<List<KiiPayBusinessMobileNotificationViewModel>>()
            {
                Data = result,
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public JsonResult GetPaymentSummary(string SendingCountry, string ReceivingCountry, decimal Amount, string ChoosenCurrency)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                string choosenCountry = FAXER.PORTAL.Common.Common.GetCountryCodeByCurrency(ChoosenCurrency);

                bool IsReceivingAmount = false;
                if (SendingCountry.ToLower() != choosenCountry.ToLower())
                {

                    IsReceivingAmount = true;
                }

                var exchangeRate = SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry);
                var paymentSummary = SEstimateFee.CalculateFaxingFee(Amount, false, IsReceivingAmount, exchangeRate,
                    SEstimateFee.GetFaxingCommision(SendingCountry));

                MobilePaymentSummaryViewModel result = new MobilePaymentSummaryViewModel()
                {

                    ExchangeRate = paymentSummary.ExchangeRate,
                    TotalAmount = paymentSummary.TotalAmount,
                    Fee = paymentSummary.FaxingFee,
                    ReceivingAmount = paymentSummary.ReceivingAmount,
                    SendingAmount = paymentSummary.FaxingAmount

                };
                return Json(new ServiceResult<MobilePaymentSummaryViewModel>()
                {
                    Data = result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<MobilePaymentSummaryViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetExchangeRate(string SendingCountry, string ReceivingCountry)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                decimal exchangeRate = SExchangeRate.GetExchangeRateValue(SendingCountry, ReceivingCountry);


                return Json(exchangeRate, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0M, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult IsCardValid([Bind(Include = KiiPayBusinessMobileSavedCreditDebitCardViewModel.BindProperty)]KiiPayBusinessMobileSavedCreditDebitCardViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
                {
                    CardName = "",
                    ExpirationMonth = model.ExpMonth,
                    ExpiringYear = model.ExpYear,
                    Number = model.CardNo,
                    SecurityCode = model.CVVCode,

                };
                var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);
                if (StripeResult.IsValid == true)
                {
                    return Json(new ServiceResult<KiiPayBusinessMobileSavedCreditDebitCardViewModel>()
                    {
                        Data = model,
                        Message = StripeResult.Message,
                        Status = ResultStatus.OK
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new ServiceResult<KiiPayBusinessMobileSavedCreditDebitCardViewModel>()
                {
                    Data = model,
                    Message = StripeResult.Message,
                    Status = ResultStatus.Error
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<KiiPayBusinessMobileSavedCreditDebitCardViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }

        }

        [HttpPost]
        public JsonResult PostCreditDebitCard([Bind(Include = KiiPayBusinessMobileSavedCreditDebitCardViewModel.BindProperty)]KiiPayBusinessMobileSavedCreditDebitCardViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                DB.SavedCard savedCard = new DB.SavedCard()
                {
                    CardName = "",
                    ClientCode = model.CVVCode.Encrypt(),
                    CreatedDate = DateTime.Now,
                    EMonth = model.ExpMonth.Encrypt(),
                    EYear = model.ExpYear.Encrypt(),
                    IsDeleted = false,
                    Module = DB.Module.KiiPayBusiness,
                    Num = model.CardNo.Encrypt(),
                    Remark = "",
                    Type = model.CardType,
                    UserId = model.KiiPayBusinessId,
                };

                var saveCreditDebitCard_result = mobileCommonServices.SaveCreditDebitCardInformation(savedCard);
                return Json(new ServiceResult<KiiPayBusinessMobileSavedCreditDebitCardViewModel>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<KiiPayBusinessMobileSavedCreditDebitCardViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UpdateCreditDebitCard([Bind(Include = KiiPayBusinessMobileSavedCreditDebitCardViewModel.BindProperty)]KiiPayBusinessMobileSavedCreditDebitCardViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var data = mobileCommonServices.SavedCardList().Data.Where(x => x.Id == model.CardId).FirstOrDefault();
                data.ClientCode = model.CVVCode.Encrypt();
                data.EMonth = model.ExpMonth.Encrypt();
                data.EYear = model.ExpYear.Encrypt();
                data.Num = model.CardNo.Encrypt();
                data.Type = model.CardType;
                var updateCreditDebitCard_result = mobileCommonServices.UpdateCreditDebitCardInformation(data);
                return Json(new ServiceResult<KiiPayBusinessMobileSavedCreditDebitCardViewModel>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<KiiPayBusinessMobileSavedCreditDebitCardViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult IsCardValidSenderMoneyFex([Bind(Include = SenderSavedDebitCreditCardViewModel.BindProperty)]SenderSavedDebitCreditCardViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                StripeResultIsValidCardVm stripeResultIsValidCardVm = new StripeResultIsValidCardVm()
                {
                    CardName = "",
                    ExpirationMonth = model.ExpMonth,
                    ExpiringYear = model.ExpYear,
                    Number = model.CardNo,
                    SecurityCode = model.CVVCode,
                    CurrencyCode = model.CurrencyCode

                };
                var StripeResult = StripServices.IsValidCardNo(stripeResultIsValidCardVm);
                if (StripeResult.IsValid == true)
                {
                    return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                    {
                        Data = model,
                        Message = StripeResult.Message,
                        Status = ResultStatus.OK
                    }, JsonRequestBehavior.AllowGet);
                }
                return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                {
                    Data = model,
                    Message = StripeResult.Message,
                    Status = ResultStatus.Error
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpPost]
        public JsonResult PostCreditDebitCard([Bind(Include = SenderSavedDebitCreditCardViewModel.BindProperty)]SenderSavedDebitCreditCardViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                DB.SavedCard savedCard = new DB.SavedCard()
                {
                    CardName = "",
                    ClientCode = model.CVVCode.Encrypt(),
                    CreatedDate = DateTime.Now,
                    EMonth = model.ExpMonth.Encrypt(),
                    EYear = model.ExpYear.Encrypt(),
                    IsDeleted = false,
                    Module = DB.Module.KiiPayBusiness,
                    Num = model.CardNo.Encrypt(),
                    Remark = "",
                    Type = model.CardType,
                    UserId = model.SenderId,
                };

                var saveCreditDebitCard_result = mobileCommonServices.SaveCreditDebitCardInformation(savedCard);
                return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }

        [HttpPost]
        public JsonResult PostCreditDebitCardForSender([Bind(Include = SenderSavedDebitCreditCardViewModel.BindProperty)]SenderSavedDebitCreditCardViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {


                DB.SavedCard userCardExist = mobileCommonServices.SavedCardSenderModule().Data
                    .Where(x => x.UserId ==
                model.SenderId).FirstOrDefault();
                if (userCardExist == null)
                {
                    DB.SavedCard savedCard = new DB.SavedCard()
                    {
                        CardName = "",
                        ClientCode = model.CVVCode.Encrypt(),
                        CreatedDate = DateTime.Now,
                        EMonth = model.ExpMonth.Encrypt(),
                        EYear = model.ExpYear.Encrypt(),
                        IsDeleted = false,
                        Module = DB.Module.Faxer,
                        Num = model.CardNo.Encrypt(),
                        Remark = "",
                        Type = model.CardType,
                        UserId = model.SenderId,
                    };
                    var saveCreditDebitCard_result = mobileCommonServices.SaveCreditDebitCardInformation(savedCard);
                    if (saveCreditDebitCard_result != null)
                    {
                        return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                        {
                            Data = model,
                            Message = "",
                            Status = ResultStatus.OK
                        }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                        {
                            Data = model,
                            Message = "Card Number Already Exist",
                            Status = ResultStatus.Warning
                        }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {

                    userCardExist.CardName = "";
                    userCardExist.ClientCode = model.CVVCode.Encrypt();
                    userCardExist.CreatedDate = DateTime.Now;
                    userCardExist.EMonth = model.ExpMonth.Encrypt();
                    userCardExist.EYear = model.ExpYear.Encrypt();
                    userCardExist.IsDeleted = false;
                    userCardExist.Module = DB.Module.Faxer;
                    userCardExist.Num = model.CardNo.Encrypt();
                    userCardExist.Remark = "";
                    userCardExist.Type = model.CardType;
                    userCardExist.UserId = model.SenderId;

                    var saveCreditDebitCard_result = mobileCommonServices.UpateCreditDebitCardInformation(userCardExist);
                    return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                    {
                        Data = model,
                        Message = "",
                        Status = ResultStatus.OK
                    }, JsonRequestBehavior.AllowGet);
                }




            }
            else
            {
                return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UpdateCreditDebitCard([Bind(Include = SenderSavedDebitCreditCardViewModel.BindProperty)]SenderSavedDebitCreditCardViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                var data = mobileCommonServices.SavedCardList().Data.Where(x => x.Id == model.CardId).FirstOrDefault();
                data.ClientCode = model.CVVCode.Encrypt();
                data.EMonth = model.ExpMonth.Encrypt();
                data.EYear = model.ExpYear.Encrypt();
                data.Num = model.CardNo.Encrypt();
                data.Type = model.CardType;
                var updateCreditDebitCard_result = mobileCommonServices.UpdateCreditDebitCardInformation(data);
                return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UpdateCreditDebitCardForSender([Bind(Include = SenderSavedDebitCreditCardViewModel.BindProperty)]SenderSavedDebitCreditCardViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                var data = mobileCommonServices.SavedCardSenderModule().Data.Where(x => x.Id == model.CardId).FirstOrDefault();
                data.ClientCode = model.CVVCode.Encrypt();
                data.EMonth = model.ExpMonth.Encrypt();
                data.EYear = model.ExpYear.Encrypt();
                data.Num = model.CardNo.Encrypt();
                data.Type = model.CardType;
                var updateCreditDebitCard_result = mobileCommonServices.UpdateCreditDebitCardInformation(data);
                return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<SenderSavedDebitCreditCardViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult DeleteCreditDebitCard(int SavedCardId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {


                var data = mobileCommonServices.SavedCardSenderModule().Data.Where(x => x.Id == SavedCardId).FirstOrDefault();

                var deleteCreditDebitCard_result = mobileCommonServices.DeleteCreditDebitCardInformation(data);

                return Json(new ServiceResult<bool>()
                {
                    Data = deleteCreditDebitCard_result,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult IsServiceAvailable(string SendingCountry,string ReceivingCountry,decimal SendingAmount, 
            TransactionTransferMethod transferMethod = TransactionTransferMethod.All)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var data = mobileCommonServices.IsServiceAvailable(SendingCountry,ReceivingCountry,SendingAmount, transferMethod);
                return Json(new ServiceResult<bool>()
                {
                    Data = data.Data,
                    Message = data.Message,
                    Status = data.Status
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public JsonResult GetInvoiceList(int KiipayBusinessId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {

                List<KiiPayBusinessMobileInvoiceListvm> vm = new List<KiiPayBusinessMobileInvoiceListvm>();


                KiiPayBusinessCommonServices kiiPayBusinessCommonServices = new KiiPayBusinessCommonServices();
                var data = mobileCommonServices.GetInvoiceDetails(KiipayBusinessId);
                var detials = (from c in data.ToList()
                               select new InvoiceMasterListvm()
                               {
                                   Id = c.Id,
                                   InvoiceStatus = Enum.GetName(typeof(DB.InvoiceStatus), c.InvoiceStatus),
                                   InvoiceStatusEnum = c.InvoiceStatus,
                                   TotalAmount = c.TotalAmount,
                                   CurrencySymbol = FAXER.PORTAL.Common.Common.GetCurrencySymbol(c.SenderCountry),
                                   InvoiceDate = c.InvoiceDate.Date.Day + "-" + Enum.GetName(typeof(Month), c.InvoiceDate.Month).Substring(0, 3) + "-" + c.InvoiceDate.Year,
                                   //InvoiceDate = getmobiledateformatbyinvoicedate(c.InvoiceDate),
                                   InvoiceNo = c.InvoiceNo,
                                   SenderName = kiiPayBusinessCommonServices.GetBusinessFullName(c.SenderId),
                                   SenderWalletNo = c.SenderWalletNo,
                                   ReciverName = kiiPayBusinessCommonServices.GetBusinessFullName(c.ReceiverId),
                                   ReciverWalletNo = c.ReceiverWalletNo,
                                   TransactionDate = c.InvoiceDate.Date,
                                   StatusColor = c.InvoiceStatus == InvoiceStatus.UnPaid ? "Red" : "Green"
                               }).OrderByDescending(x => x.TransactionDate).ToList();

                //vm.ListGroupByVm = (from c in vm.PayingInvoiceListvm.ToList()
                //                    select new ListGroupByVm() {
                //                        InvoiceDate = c.TransactionDate.ToString("dddd, dd MMMM ")
                //                    }).ToList();
                foreach (var item in detials.GroupBy(x => x.TransactionDate).Select(x => x.FirstOrDefault()).ToList())
                {

                    KiiPayBusinessMobileInvoiceListvm model = new KiiPayBusinessMobileInvoiceListvm();
                    string DateToday = DateTime.Now.Date.ToString("dddd, dd MMMM ");
                    string Itemdate = item.TransactionDate.ToString("dddd, dd MMMM ");
                    if (DateToday == Itemdate)
                    {
                        model.InvoiceDate = "Today";
                    }
                    else
                    {
                        model.InvoiceDate = Itemdate;
                    }
                    model.PayingInvoiceListvm = detials.Where(x => x.TransactionDate == item.TransactionDate).ToList();
                    vm.Add(model);


                }


                return Json(new ServiceResult<List<KiiPayBusinessMobileInvoiceListvm>>()
                {

                    Data = vm,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<KiiPayBusinessMobileInvoiceListvm>>()
                {

                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }

        }

        [HttpGet]
        public JsonResult GetWalletStatementList(int KiipayBusinessId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                KiiPayBusinessWalletStatementServices _kiiPayBusinessWalletStatementServices = new KiiPayBusinessWalletStatementServices();
                List<WalletStatementVM> vm = new List<WalletStatementVM>();
                List<KiiPayBusinessMobileWalletStatementvm> List = new List<KiiPayBusinessMobileWalletStatementvm>();


                vm = _kiiPayBusinessWalletStatementServices.GetWalletStatement(WalletStatementFilterType.All, KiipayBusinessId).OrderByDescending(x => x.TransactionDate).ToList();
                foreach (var item in vm.GroupBy(x => x.TransactionDate).Select(x => x.FirstOrDefault()).ToList())
                {

                    KiiPayBusinessMobileWalletStatementvm model = new KiiPayBusinessMobileWalletStatementvm();
                    string DateToday = DateTime.Now.Date.ToString("dddd, dd MMMM ");
                    string Itemdate = item.TransactionDateTime.ToString("dddd, dd MMMM ");
                    if (DateToday == Itemdate)
                    {
                        model.InvoiceDate = "Today";
                    }
                    else
                    {
                        model.InvoiceDate = Itemdate;
                    }
                    model.WalletStatementListvm = vm.Where(x => x.TransactionDate == item.TransactionDate).ToList();
                    List.Add(model);


                }


                return Json(new ServiceResult<List<KiiPayBusinessMobileWalletStatementvm>>()
                {

                    Data = List,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<List<KiiPayBusinessMobileWalletStatementvm>>()
                {

                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }

        }



        [HttpGet]
        public JsonResult IsValidMobileAccount(string sendingCountry, string receivingCountry, string mobileNo, decimal sendingAmount, string receivingCountryPhoneCode)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = mobileCommonServices.IsValidMobileAccount(sendingCountry, receivingCountry, mobileNo, sendingAmount, receivingCountryPhoneCode);
                return Json(result
                , JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Status = ResultStatus.Warning
                }
                , JsonRequestBehavior.AllowGet);
            }


        }

        [HttpGet]
        public JsonResult GetCountryPhoneCode(string countryCode)
        {
            var result = mobileCommonServices.GetCountryPhoneCode(countryCode);
            return Json(result
            , JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        public JsonResult GetIdentityInformationBySenderId(int senderId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var result = mobileCommonServices.GetIdentityInformationBySenderId(senderId);
                if (result != null)
                {

                    return Json(new ServiceResult<MobileSenderIdentityInformationViewModel>()
                    {
                        Data = result,
                        Message = "",
                        Status = ResultStatus.OK

                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ServiceResult<MobileSenderIdentityInformationViewModel>()
                    {
                        Data = null,
                        Message = "",
                        Status = ResultStatus.Info

                    }, JsonRequestBehavior.AllowGet);
                }

            }
            else
            {
                return Json(new ServiceResult<MobileSenderIdentityInformationViewModel>()
                {
                    Data = null,
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult GetSenderProfile(int senderId)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                try
                {
                    var result = mobileCommonServices.GetSenderInformation(senderId);
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {

                    return Json(new ServiceResult<SenderInformationViewModel>()
                    {
                        Data = null,
                        Message = "",
                        Status = ResultStatus.Warning

                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new ServiceResult<SenderInformationViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning

                }, JsonRequestBehavior.AllowGet);
            }

        }

        private string getmobiledateformatbyinvoicedate(DateTime invoiceDate)
        {
            throw new NotImplementedException();
        }

        [HttpPost]
        public JsonResult PostBankAccount([Bind(Include = KiiPayBusinessMobileSavedBankAccountViewModel.BindProperty)]KiiPayBusinessMobileSavedBankAccountViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                DB.SavedBank bankAccount = new DB.SavedBank()
                {
                    AccountNumber = model.AccountNumber,
                    Country = model.CountryCode,
                    BankId = model.BankId,
                    BankName = model.BankName,
                    BranchId = model.BranchId,
                    UserId = model.KiiPayBusinessId,
                    UserType = Module.KiiPayBusiness,
                    OwnerName = model.AccountOwnerName,
                    CreatedDate = DateTime.Now,
                    BranchCode = model.BranchCode,
                    BranchName = model.BranchName
                };

                var saveBankAccount_result = mobileCommonServices.SaveBankAccountInformation(bankAccount);
                return Json(new ServiceResult<KiiPayBusinessMobileSavedBankAccountViewModel>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<KiiPayBusinessMobileSavedBankAccountViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UpdateBankAccount([Bind(Include = KiiPayBusinessMobileSavedBankAccountViewModel.BindProperty)]KiiPayBusinessMobileSavedBankAccountViewModel model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var data = mobileCommonServices.BankAccountList().Data.Where(x => x.Id == model.BankAccountId).FirstOrDefault();
                data.AccountNumber = model.AccountNumber;
                data.Country = model.CountryCode;
                data.BankId = model.BankId;
                data.BankName = model.BankName;
                data.BranchId = model.BranchId;
                data.UserId = model.KiiPayBusinessId;
                data.UserType = Module.KiiPayBusiness;
                data.OwnerName = model.AccountOwnerName;
                data.CreatedDate = DateTime.Now;
                data.BranchCode = model.BranchCode;
                data.BranchName = model.BranchName;

                var updateCreditDebitCard_result = mobileCommonServices.UpdateBankAccountInformation(data);
                return Json(new ServiceResult<KiiPayBusinessMobileSavedBankAccountViewModel>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<KiiPayBusinessMobileSavedBankAccountViewModel>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult PostSenderBusinessDocumentation([Bind(Include = IdentificationDetailvm.BindProperty)]IdentificationDetailvm model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                SenderBusinessDocumentation senderBusinessDocumentation = new SenderBusinessDocumentation()
                {
                    AccountNo = model.AccountNo,
                    City = model.City,
                    Country = model.Country,
                    CreatedDate = DateTime.Now,
                    DocumentExpires = model.DocumentExpires,
                    DocumentName = model.DocumentName,
                    DocumentPhotoUrl = model.DocumentPhotoUrl,
                    DocumentPhotoUrlTwo = model.DocumentPhotoUrlTwo,
                    ExpiryDate = model.ExpiryDate,
                    IdentificationTypeId = model.IdentificationTypeId,
                    IssuingCountry = model.IssuingCountry,
                    IdentityNumber = model.IdentityNumber,
                    SenderId = model.SenderId,
                    Status = DocumentApprovalStatus.InProgress,
                    DocumentType = model.DocumentType,
                };
                var data = mobileCommonServices.SenderDocumentation().Data.Where(x => x.DocumentName == model.DocumentName).ToList();
                if (data.Count < 0)
                {
                    var result = mobileCommonServices.SaveSenderBusinessDocumentation(senderBusinessDocumentation);
                    return Json(new ServiceResult<IdentificationDetailvm>()
                    {
                        Data = model,
                        Message = "",
                        Status = ResultStatus.OK
                    }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new ServiceResult<IdentificationDetailvm>()
                    {
                        Data = model,
                        Message = "Document already added",
                        Status = ResultStatus.OK
                    }, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return Json(new ServiceResult<IdentificationDetailvm>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpGet]
        public JsonResult DeleteSenderDocumentation([Bind(Include = IdentificationDetailvm.BindProperty)]IdentificationDetailvm model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var data = mobileCommonServices.SenderDocumentation().Data.Where(x => x.Id == model.Id).FirstOrDefault();

                var reuslt = mobileCommonServices.DeleteSenderBusinessDocumentation(data);

                return Json(new ServiceResult<bool>()
                {
                    Data = reuslt,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<bool>()
                {
                    Data = false,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public JsonResult UpdateSenderBusinessDocumentation([Bind(Include = IdentificationDetailvm.BindProperty)]IdentificationDetailvm model)
        {
            string token = FAXER.PORTAL.Common.Common.RequestToken;
            if (FAXER.PORTAL.Common.Common.ValidateToken(token))
            {
                var data = mobileCommonServices.SenderDocumentation().Data.Where(x => x.Id == model.Id).FirstOrDefault();
                data.AccountNo = model.AccountNo;
                data.City = model.City;
                data.Country = model.Country;
                data.CreatedDate = DateTime.Now;
                data.DocumentExpires = model.DocumentExpires;
                data.DocumentName = model.DocumentName;
                data.DocumentPhotoUrl = model.DocumentPhotoUrl;
                data.DocumentPhotoUrlTwo = model.DocumentPhotoUrlTwo;
                data.ExpiryDate = model.ExpiryDate;
                data.IdentificationTypeId = model.IdentificationTypeId;
                data.IssuingCountry = model.IssuingCountry;
                data.IdentityNumber = model.IdentityNumber;
                data.SenderId = model.SenderId;
                data.Status = DocumentApprovalStatus.InProgress;
                data.DocumentType = model.DocumentType;

                var updateCreditDebitCard_result = mobileCommonServices.UpdateSenderBusinessDocumentation(data);
                return Json(new ServiceResult<IdentificationDetailvm>()
                {
                    Data = model,
                    Message = "",
                    Status = ResultStatus.OK
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new ServiceResult<IdentificationDetailvm>()
                {
                    Data = null,
                    Message = "",
                    Status = ResultStatus.Warning
                }, JsonRequestBehavior.AllowGet);

            }
        }
    }
}