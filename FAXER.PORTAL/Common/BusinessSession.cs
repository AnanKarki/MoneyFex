using FAXER.PORTAL.Areas.Businesses.ViewModels;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels.SignUpViewModel;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public class BusinessSession
    {

        #region KiiPay Business 
        public static LoggedKiiPayBusinessUserInfo LoggedKiiPayBusinessUserInfo
        {
            get
            {
                return GetSession("LoggedKiiPayBusinessUserInfo") as LoggedKiiPayBusinessUserInfo;
            }
            set { SetSession("LoggedKiiPayBusinessUserInfo", value); }
        }
        public static BusinessLoginInformationVM KiiPayBusinessLoginInformation
        {
            get
            {
                return (GetSession("KiiPayBusinessLoginInformation") as BusinessLoginInformationVM);
            }
            set
            {
                SetSession("KiiPayBusinessLoginInformation", value);
            }
        }
        public static KiiPayBusinessEnterPaymentReferenceVM KiiPayBusinessEnterPaymentReference
        {
            get
            {
                return (GetSession("KiiPayBusinessEnterPaymentReference") as KiiPayBusinessEnterPaymentReferenceVM);
            }
            set
            {
                SetSession("KiiPayBusinessEnterPaymentReference", value);
            }
        }

        public static KiiPayBusinessSearchSuppliersVM KiiPayBusinessSearchSuppliers
        {
            get
            {
                return (GetSession("KiiPayBusinessSearchSuppliers") as KiiPayBusinessSearchSuppliersVM);
            }
            set
            {
                SetSession("KiiPayBusinessSearchSuppliers", value);
            }
        }public static KiiPayBusinessLocalTopUpSuccessVM KiiPayBusinessTopUpSuccess
        {
            get
            {
                return (GetSession("KiiPayBusinessTopUpSuccess") as KiiPayBusinessLocalTopUpSuccessVM);
            }
            set
            {
                SetSession("KiiPayBusinessTopUpSuccess", value);
            }
        }
        public static KiiPayBusinessLocalTopUpEnterAccountNoVM KiiPayBusinessTopUpEnterAccountNo
        {
            get
            {
                return (GetSession("KiiPayBusinessTopUpEnterAccountNo") as KiiPayBusinessLocalTopUpEnterAccountNoVM);
            }
            set
            {
                SetSession("KiiPayBusinessTopUpEnterAccountNo", value);
            }
        }
        public static KiiPayBusinessInternationalSearchCountryVM KiiPayBusinessInternationalSearchCountry
        {
            get
            {
                return (GetSession("KiiPayBusinessInternationalSearchCountry") as KiiPayBusinessInternationalSearchCountryVM);
            }
            set
            {
                SetSession("KiiPayBusinessInternationalSearchCountry", value);
            }
        }
        public static KiiPayBusinessPersonalInfoVM KiiPayBusinessPersonalInfo
        {
            get
            {
                return (GetSession("KiiPayBusinessPersonalInfo") as KiiPayBusinessPersonalInfoVM);
            }
            set
            {
                SetSession("KiiPayBusinessPersonalInfo", value);
            }
        }
        public static kiiPayBusinessInfoVM kiiPayBusinessInfo
        {
            get
            {
                return (GetSession("kiiPayBusinessInfo") as kiiPayBusinessInfoVM);
            }
            set
            {
                SetSession("kiiPayBusinessInfo", value);
            }
        }
        public static AddressVM KiiPayBusinessPersnalAddressInfo
        {
            get
            {
                return (GetSession("KiiPayBusinessPersnalAddressInfo") as AddressVM);
            }
            set
            {
                SetSession("KiiPayBusinessPersnalAddressInfo", value);
            }
        }
        public static AddressVM BusinessAddressInfo
        {
            get
            {
                return (GetSession("BusinessAddressInfo") as AddressVM);
            }
            set
            {
                SetSession("BusinessAddressInfo", value);
            }
        }

        public static AddressVM BusinessOpeationAddressInfo
        {
            get
            {
                return (GetSession("BusinessOpeationAddressInfo") as AddressVM);
            }
            set
            {
                SetSession("BusinessOpeationAddressInfo", value);
            }
        }
        #region RegisterECardUser
        public static KiiPayBusinessRegisterECardUserPersonalDetailVM KiiPayBusinessRegisterECardUserPersonalDetail
        {
            get
            {
                return (GetSession("KiiPayBusinessRegisterECardUserPersonalDetail") as KiiPayBusinessRegisterECardUserPersonalDetailVM);
            }
            set
            {
                SetSession("KiiPayBusinessRegisterECardUserPersonalDetail", value);
            }
        }

        public static KiiPayBusinessRegisterECardUserAddressInformationVM KiiPayBusinessRegisterECardUserAddressInformation
        {
            get
            {
                return (GetSession("KiiPayBusinessRegisterECardUserAddressInformation") as KiiPayBusinessRegisterECardUserAddressInformationVM);
            }
            set
            {
                SetSession("KiiPayBusinessRegisterECardUserAddressInformation", value);
            }
        }
        public static KiiPayBusinessRegisterECardUserIdentificationInformationVM KiiPayBusinessRegisterECardUserIdentificationInformation
        {
            get
            {
                return (GetSession("KiiPayBusinessRegisterECardUserIdentificationInformation") as KiiPayBusinessRegisterECardUserIdentificationInformationVM);
            }
            set
            {
                SetSession("KiiPayBusinessRegisterECardUserIdentificationInformation", value);
            }
        }

        #endregion


        #region AddMoneyToWallet
        public static AddMoneyToWalletEnterAmountVm KiiPayBusinessAddMoneyToWalletEnterAmount
        {
            get
            {
                return (GetSession("KiiPayBusinessAddMoneyToWalletEnterAmount") as AddMoneyToWalletEnterAmountVm);
            }
            set
            {
                SetSession("KiiPayBusinessAddMoneyToWalletEnterAmount", value);
            }
        }

        #endregion

        #region International Transaction 

        public static KiiPayBusinessInternationalTransferEnterAmountVM KiiPayBusinessInternationalTransferAmount {
            get
            {
                return (GetSession("KiiPayBusinessInternationalTransferAmount") as KiiPayBusinessInternationalTransferEnterAmountVM);
            }
            set
            {
                SetSession("KiiPayBusinessInternationalTransferAmount", value);
            }
        }
        public static KiiPayBusinessLocalTransferEnterAmountVM KiiPayBusinessLocalTransferAmount
        {
            get
            {
                return (GetSession("KiiPayBusinessLocalTransferAmount") as KiiPayBusinessLocalTransferEnterAmountVM);
            }
            set
            {
                SetSession("KiiPayBusinessLocalTransferAmount", value);
            }
        }
        public static PayingToKiiPayBusinessInfoVM PayingToKiiPayBusinessInfo
        {
            get
            {
                return (GetSession("PayingToKiiPayBusinessInfo") as PayingToKiiPayBusinessInfoVM);
            }
            set
            {
                SetSession("PayingToKiiPayBusinessInfo", value);
            }
        }


        #endregion

        public static bool SendSMSForLocalPayment { get; set; }
        #endregion


        #region Previous Version Business Session



        public static BusinessDetailsViewModel BusinessDetails
        {
            get
            {
                return GetSession("BusinessDetails") as BusinessDetailsViewModel;
            }
            set { SetSession("BusinessDetails", value); }
        }
        public static MerchantDetialsViewModel MerchantDetialsViewModel
        {
            get
            {
                return GetSession("MerchantDetialsViewModel") as MerchantDetialsViewModel;
            }
            set { SetSession("MerchantDetialsViewModel", value); }
        }


        public static BusinessContactDetailsViewModel BusinessContactDetails
        {
            get
            {
                return GetSession("BusinessContactDetails") as BusinessContactDetailsViewModel;
            }
            set { SetSession("BusinessContactDetails", value); }
        }
        public static MFBCCardUserDetailsViewModel BusinessCardUserDetails
        {
            get
            {
                return GetSession("BusinessCardUserDetails") as MFBCCardUserDetailsViewModel;
            }
            set { SetSession("BusinessCardUserDetails", value); }
        }
        public static MFBCCardUserContactDetailsViewModel BusinessCardUserContactDetails
        {
            get
            {
                return GetSession("BusinessCardUserContactDetails") as MFBCCardUserContactDetailsViewModel;
            }
            set { SetSession("BusinessCardUserContactDetails", value); }
        }
        public static LoggedBusinessMerchant LoggedBusinessMerchant
        {
            get
            {
                return GetSession("LoggedBusinessMerchant") as LoggedBusinessMerchant;
            }
            set { SetSession("LoggedBusinessMerchant", value); }
        }
        public static NonCardPaymentViewModel NonCardPaymentViewModel
        {
            get
            {
                return GetSession("NonCardPaymentViewModel") as NonCardPaymentViewModel;
            }
            set { SetSession("NonCardPaymentViewModel", value); }
        }
        public static ReceiverDetailsViewModel ReceiverDetailsViewModel
        {
            get
            {
                return GetSession("ReceiverDetailsViewModel") as ReceiverDetailsViewModel;
            }
            set { SetSession("ReceiverDetailsViewModel", value); }
        }
        public static MerchantNationalPaymentAmountViewModel MerchantNationalPaymentAmountViewModel
        {
            get
            {
                return GetSession("MerchantNationalPaymentAmountViewModel") as MerchantNationalPaymentAmountViewModel;
            }
            set { SetSession("MerchantNationalPaymentAmountViewModel", value); }
        }
        public static MerchantInternationalPaymentAmountViewModel MerchantInternationalPaymentAmountViewModel
        {
            get
            {
                return GetSession("MerchantInternationalPaymentAmountViewModel") as MerchantInternationalPaymentAmountViewModel;
            }
            set { SetSession("MerchantInternationalPaymentAmountViewModel", value); }
        }
        public static string FaxingCurrency
        {

            get
            {
                return (GetSession("FaxingCurrency") ?? "").ToString();
            }
            set { SetSession("FaxingCurrency", value); }
        }
        public static int NonCardReceiverId_merchantNonCard
        {
            get
            {
                return (int)(GetSession("NonCardReceiverId_merchantNonCard") ?? 0);
            }
            set
            {
                SetSession("NonCardReceiverId_merchantNonCard", value);
            }
        }
        public static string FaxingCurrencySymbol
        {

            get
            {
                return (GetSession("FaxingCurrencySymbol") ?? "").ToString();
            }
            set { SetSession("FaxingCurrencySymbol", value); }
        }
        public static string ReceivingCurrency
        {

            get
            {
                return (GetSession("ReceivingCurrency") ?? "").ToString();
            }
            set { SetSession("ReceivingCurrency", value); }
        }
        public static string BackButtonURL
        {

            get
            {
                return (GetSession("BackButtonURL") ?? "").ToString();
            }
            set { SetSession("BackButtonURL", value); }
        }
        public static string MerchantAccountNumber
        {

            get
            {
                return (GetSession("MerchantAccountNumber") ?? "").ToString();
            }
            set { SetSession("MerchantAccountNumber", value); }
        }
        public static string MFTCCardNumber
        {

            get
            {
                return (GetSession("MFTCCardNumber") ?? "").ToString();
            }
            set { SetSession("MFTCCardNumber", value); }
        }
        public static string ReceivingCurrencySymbol
        {

            get
            {
                return (GetSession("ReceivingCurrencySymbol") ?? "").ToString();
            }
            set { SetSession("ReceivingCurrencySymbol", value); }
        }
        public static string ReceivingCountry
        {

            get
            {
                return (GetSession("ReceivingCountry") ?? "").ToString();
            }
            set { SetSession("ReceivingCountry", value); }
        }
        public static string TransactionSummaryURL
        {

            get
            {
                return (GetSession("TransactionSummaryURL") ?? "").ToString();
            }
            set { SetSession("TransactionSummaryURL", value); }
        }
        public static EstimateFaxingFeeSummary FaxingAmountSummary
        {
            get
            {
                return (GetSession("FaxingAmountSummary") as EstimateFaxingFeeSummary);
            }
            set
            {
                SetSession("FaxingAmountSummary", value);
            }
        }
        public static TopUpAmountViewModel TopUpAmountViewModel
        {
            get
            {
                return (GetSession("TopUpAmountViewModel") as TopUpAmountViewModel);
            }
            set
            {
                SetSession("TopUpAmountViewModel", value);
            }
        }

        public static string FaxingCountry
        {

            get
            {
                return (GetSession("FaxingCountry") ?? "").ToString();
            }
            set { SetSession("FaxingCountry", value); }
        }
        public static string PasswordSecurityCode
        {
            get
            {
                return GetSession("PasswordSecurityCode").ToString();
            }
            set { SetSession("PasswordSecurityCode", value); }
        }
        public static string EmailAddress
        {
            get
            {
                return GetSession("EmailAddress").ToString();
            }
            set { SetSession("EmailAddress", value); }
        }

        public static string FirstLogin
        {
            get
            {
                return (GetSession("FirstLogin") ?? "").ToString();
            }
            set { SetSession("FirstLogin", value); }
        }
        public static string FormURL
        {
            get
            {
                return (GetSession("FormURL") ?? "").ToString();
            }
            set { SetSession("FormURL", value); }
        }

        public static MFTCCardLocalTopUpByMerchantViewModel MFTCCardLocalTopUpByMerchantViewModel
        {
            get
            {
                return (GetSession("MFTCCardLocalTopUpByMerchantViewModel") as MFTCCardLocalTopUpByMerchantViewModel);
            }
            set
            {
                SetSession("MFTCCardLocalTopUpByMerchantViewModel", value);
            }
        }
        #endregion


        public static bool MerchantHasMFBCCard { get; set; }
        public static KiiPayBusinessPaymentSummaryVM KiiPayBusinessPaymentSummary {

            get
            {
                return (GetSession("KiiPayBusinessPaymentSummary") as KiiPayBusinessPaymentSummaryVM);
            }
            set
            {
                SetSession("KiiPayBusinessPaymentSummary", value);
            }
        }

        private static object GetSession(string key)
        {
            return HttpContext.Current.Session[key];
        }
        private static void SetSession(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }
    }
}