using FAXER.PORTAL.Areas.CardUsers.ViewModels;
using FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public class CardUserSession
    {
        
        public static LoggedKiiPayUserViewModel LoggedCardUserViewModel
        {

            get
            {
                return GetSession("LoggedCardUserViewModel") as LoggedKiiPayUserViewModel;
            }
            set { SetSession("LoggedCardUserViewModel", value); }
        }

        public static KiiPayPersonalSignUpSessionViewModel KiiPayPersonalSignUpSessionViewModel
        {
            get
            {
                return GetSession("KiiPayPersonalSignUpSessionViewModel") as KiiPayPersonalSignUpSessionViewModel;
            }
            set
            {
                SetSession("KiiPayPersonalSignUpSessionViewModel", value);
            }
        }
        public static AddMoneyToWalletEnterAmountVM KiiPayPersonalAddMoneyToWalletEnterAmount
        {
            get
            {
                return GetSession("KiiPayPersonalAddMoneyToWalletEnterAmount") as AddMoneyToWalletEnterAmountVM;
            }
            set
            {
                SetSession("KiiPayPersonalAddMoneyToWalletEnterAmount", value);
            }
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

        

        public static string MerchantAccountNumber
        {
            get
            {
                return (GetSession("MerchantAccountNumber") ?? "").ToString();
            }
            set { SetSession("MerchantAccountNumber", value); }
        }
        public static string MerchantName
        {
            get
            {
                return (GetSession("MerchantName") ?? "").ToString();
            }
            set { SetSession("MerchantName", value); }
        }

        public static MerchantDetailsViewModel_CardUserViewModel MerchantDetailsViewModel_CardUserViewModel
        {
            get
            {
                return GetSession("MerchantDetailsViewModel_CardUserViewModel") as MerchantDetailsViewModel_CardUserViewModel;
            }
            set { SetSession("MerchantDetailsViewModel_CardUserViewModel", value); }
        }

        public static string BackButtonURL
        {
            get
            {
                return (GetSession("BackButtonURL") ?? "").ToString();
            }
            set { SetSession("BackButtonURL", value); }
        }

        public static string TransactionSummaryURL
        {
            get
            {
                return (GetSession("TransactionSummaryURL") ?? "").ToString();
            }
            set { SetSession("TransactionSummaryURL", value); }
        }
        public static string FaxingCurrency
        {
            get
            {
                return (GetSession("FaxingCurrency") ?? "").ToString();
            }
            set { SetSession("FaxingCurrency", value); }
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
        public static string ReceivingCurrencySymbol
        {
            get
            {
                return (GetSession("ReceivingCurrencySymbol") ?? "").ToString();
            }
            set { SetSession("ReceivingCurrencySymbol", value); }
        }

        public static MerchantNationalPayingAmount_CardUserViewModel MerchantNationalPayingAmount_CardUserViewModel
        {
            get
            {
                return GetSession("MerchantNationalPayingAmount_CardUserViewModel") as MerchantNationalPayingAmount_CardUserViewModel;
            }
            set { SetSession("MerchantNationalPayingAmount_CardUserViewModel", value); }
        }

        public static MerchantInternationalPayingAmount_CardUserViewModel MerchantInternationalPayingAmount_CardUserViewModel
        {
            get
            {
                return GetSession("MerchantInternationalPayingAmount_CardUserViewModel") as MerchantInternationalPayingAmount_CardUserViewModel;
            }
            set { SetSession("MerchantInternationalPayingAmount_CardUserViewModel", value); }
        }

        public static string FaxingCountry
        {
            get
            {
                return (GetSession("FaxingCountry") ?? "").ToString();
            }
            set { SetSession("FaxingCountry", value); }
        }
        public static string ReceivingCountry
        {
            get
            {
                return (GetSession("ReceivingCountry") ?? "").ToString();
            }
            set { SetSession("ReceivingCountry", value); }
        }

        public static EstimateFaxingFeeSummary FaxingAmountSummary
        {
            get
            {
                return GetSession("EstimateFaxingFeeSummary") as EstimateFaxingFeeSummary;
            }
            set { SetSession("EstimateFaxingFeeSummary", value); }
        }

        public static MFTCCardHolderDetailsViewModel MFTCCardHolderDetailsViewModel
        {
            get
            {
                return GetSession("MFTCCardHolderDetailsViewModel") as MFTCCardHolderDetailsViewModel;
            }
            set { SetSession("MFTCCardHolderDetailsViewModel", value); }
        }

        public static bool MFTCCardPaymentByCardUserSuccessful { get; set; }

        public static string MFTCCardNo
        {
            get
            {
                return (GetSession("MFTCCardNo") ?? "").ToString();
            }
            set { SetSession("MFTCCardNo", value); }
        }



        public static CardUser_MFTCCardPaymentPayingAmountViewModel CardUser_MFTCCardPaymentPayingAmountViewModel
        {
            get
            {
                return GetSession("CardUser_MFTCCardPaymentPayingAmountViewModel") as CardUser_MFTCCardPaymentPayingAmountViewModel;
            }
            set { SetSession("CardUser_MFTCCardPaymentPayingAmountViewModel", value); }
        }

        public static CardUser_NonCardPayingAmountViewModel CardUser_NonCardPayingAmountViewModel
        {
            get
            {
                return GetSession("CardUser_NonCardPayingAmountViewModel") as CardUser_NonCardPayingAmountViewModel;
            }
            set { SetSession("CardUser_NonCardPayingAmountViewModel", value); }
        }

        public static CardUser_ReceiverDetailsViewModel CardUser_ReceiverDetailsViewModel
        {
            get
            {
                return GetSession("CardUser_ReceiverDetailsViewModel") as CardUser_ReceiverDetailsViewModel;
            }
            set { SetSession("CardUser_ReceiverDetailsViewModel", value); }
        }

        public static int NonCardReceiverId
        {
            get
            {
                return (int)(GetSession("NonCardReceiverId") ?? 0);
            }
            set
            {
                SetSession("NonCardReceiverId", value);
            }
        }

        public static MFTCCardLocalTopupByCardUserViewModel MFTCCardLocalTopupByCardUserViewModel
        {
            get
            {
                return GetSession("MFTCCardLocalTopupByCardUserViewModel") as MFTCCardLocalTopupByCardUserViewModel;
            }
            set { SetSession("MFTCCardLocalTopupByCardUserViewModel", value); }
        }
        public static PayIntoAnotherWalletViewModel PayIntoAnotherWalletSession
        {

            get
            {
                return GetSession("PayIntoAnotherWalletSession") as PayIntoAnotherWalletViewModel;
            }
            set
            {
                SetSession("PayIntoAnotherWalletSession", value);
            }
        }

        public static KiiPayRequestAPaymentSessionViewModel RequestAPaymentSession
        {

            get
            {
                return GetSession("RequestAPaymentSession") as KiiPayRequestAPaymentSessionViewModel;
            }
            set
            {
                SetSession("RequestAPaymentSession", value);
            }
        }

        public static PayAPaymentRequestViewModel PayARequestSession
        {

            get
            {
                return GetSession("PayARequestSession") as PayAPaymentRequestViewModel;
            }
            set
            {
                SetSession("PayARequestSession", value);
            }
        }

        public static PayToBankAccountSessionViewModel PayToBankAccountSession
        {

            get
            {
                return GetSession("PayToBankAccountSession") as PayToBankAccountSessionViewModel;
            }
            set
            {
                SetSession("PayToBankAccountSession", value);
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