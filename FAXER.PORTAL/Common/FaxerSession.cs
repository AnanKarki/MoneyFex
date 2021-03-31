using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.BankApi.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Models.SignUp;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Common
{
    public class FaxerSession
    {
        public static FaxerRegistrationSession FaxerRegistration
        {
            get
            {
                return GetSession("FaxerRegistrationSession") as FaxerRegistrationSession;
            }
            set { SetSession("FaxerRegistrationSession", value); }
        }
        public static FaxerInformation FaxerInformation
        {
            get
            {
                return GetSession("FaxerInformation") as FaxerInformation;
            }
            set { SetSession("FaxerInformation", value); }
        }
        public static RegisterViewModel RegisterViewModel
        {
            get
            {
                return GetSession("RegisterViewModel") as RegisterViewModel;
            }
            set { SetSession("RegisterViewModel", value); }
        }
        public static FaxerIdentification FaxerIdentification
        {
            get
            {
                return GetSession("FaxerIdentification") as FaxerIdentification;
            }
            set
            {
                SetSession("FaxerIdentification", value);
            }
        }
        public static FaxerContactDetails FaxerContactDetails
        {
            get
            {
                return GetSession("FaxerContactDetails") as FaxerContactDetails;
            }
            set { SetSession("FaxerContactDetails", value); }
        }
        public static CreditDebitCardViewModel CreditDebitDetails
        {
            get
            {
                return GetSession("CreditDebitDetails") as CreditDebitCardViewModel;
            }
            set { SetSession("CreditDebitDetails", value); }
        }
        public static PaymentUsingSavedCreditDebitCardVm SavedCreditDebitCardDetails
        {
            get
            {
                return GetSession("SavedCreditDebitCardDetails") as PaymentUsingSavedCreditDebitCardVm;
            }
            set { SetSession("SavedCreditDebitCardDetails", value); }
        }

        public static RegistrationCodeVerificationViewModel RegistrationCodeVerificationViewModel
        {
            get
            {
                return GetSession("RegistrationCodeVerificationViewModel") as RegistrationCodeVerificationViewModel;
            }
            set { SetSession("RegistrationCodeVerificationViewModel", value); }
        }

        public static string CardUrl
        {
            get
            {
                return (GetSession("CardUrl") ?? "").ToString();
            }
            set
            {
                SetSession("CardUrl", value);
            }
        }
        public static bool IsValidToResetPassword
        {
            get
            {
                return (bool)(GetSession("IsValidToResetPassword"));
            }
            set
            {
                SetSession("IsValidToResetPassword", value);
            }
        }


        public static string TransactionSummaryUrl
        {
            get
            {
                return (GetSession("TransactionSummaryUrl") ?? "").ToString();
            }
            set
            {
                SetSession("TransactionSummaryUrl", value);
            }
        }

        public static bool IsMobileView
        {

            get
            {
                return (bool)(GetSession("IsMobileView"));
            }
            set
            {
                SetSession("IsMobileView", value);
            }
        }
        public static bool IsRedirectedFromEmail
        {

            get
            {
                return (bool)(GetSession("IsRedirectedFromEmail") ?? false);
            }
            set
            {
                SetSession("IsRedirectedFromEmail", value);
            }
        }
        public static string LoggedUserName
        {
            get
            {
                return GetSession(("LoggedUserName") ?? "").ToString();
            }
            set
            {
                SetSession("LoggedUserName", value);
            }
        }

        public static LoggedUser LoggedUser
        {
            get
            {
                return GetSession("LoggedUser") as LoggedUser;
            }
            set
            {
                SetSession("LoggedUser", value);
            }
        }

        public static string ResetPassToken
        {

            get
            {
                return GetSession(("ResetPassToken") ?? "").ToString();
            }
            set
            {
                SetSession("ResetPassToken", value);
            }

        }



        public static string AreaName
        {
            get
            {
                return (GetSession("AreaName") ?? "").ToString();
            }
            set
            {
                SetSession("AreaName", value);
            }
        }
        public static string ResetEmail
        {
            get
            {
                return (GetSession("ResetEmail") ?? "").ToString();
            }
            set
            {
                SetSession("ResetEmail", value);
            }
        }
        public static string AccountVerificationCode
        {
            get
            {
                return (GetSession("AccountVerificationCode") ?? "").ToString();
            }
            set
            {
                SetSession("AccountVerificationCode", value);
            }
        }

        public static string UserEnterAccountVerficationCode
        {

            get
            {

                return (GetSession("UserEnterAccountVerficationCode") ?? "").ToString();
            }
            set
            {

                SetSession("UserEnterAccountVerficationCode", value);
            }
        }
        public static string TransferMethod
        {

            get
            {
                return (GetSession("TransferMethod") ?? "").ToString();
            }
            set
            {
                SetSession("TransferMethod", value);
            }
        }
        public static string FromUrl
        {
            get
            {
                return (GetSession("FromUrl") ?? "").ToString();
            }
            set
            {
                SetSession("FromUrl", value);
            }
        }
        public static string BackUrl
        {
            get
            {
                return (GetSession("BackUrl") ?? "").ToString();
            }
            set
            {
                SetSession("BackUrl", value);
            }
        }
        public static string ToUrl
        {
            get
            {
                return (GetSession("ToUrl") ?? "").ToString();
            }
            set
            {
                SetSession("ToUrl", value);
            }
        }
        public static string BusinessName
        {
            get
            {
                return (GetSession("BusinessName") ?? "").ToString();
            }
            set
            {
                SetSession("BusinessName", value);
            }
        }
        public static string FaxerCountry
        {
            get
            {
                return (GetSession("FaxerCountry") ?? "").ToString();
            }
            set
            {
                SetSession("FaxerCountry", value);
            }
        }

        public static ReceiversDetailsViewModel ReceiversDetailsViewModel { get; internal set; }
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
        public static BankToBankTransferViewModel BankToBankTransfer
        {
            get
            {
                return (GetSession("BankToBankTransfer") as BankToBankTransferViewModel);
            }
            set
            {
                SetSession("BankToBankTransfer", value);
            }
        }

        public static string FaxingCountry
        {
            get
            {
                return (GetSession("FaxingCountry") as string);
            }
            set
            {
                SetSession("FaxingCountry", value);
            }
        }
        public static string ReceivingCountry
        {
            get
            {
                return (GetSession("ReceivingCountry") as string);
            }
            set
            {
                SetSession("ReceivingCountry", value);
            }
        }

        public static string MerchantACNumber
        {
            get
            {
                return (GetSession("MerchantACNumber") as string);
            }
            set
            {
                SetSession("MerchantACNumber", value);
            }
        }

        public static string PaymentMethod
        {
            get
            {
                return (GetSession("PaymentMethod") as string);
            }
            set
            {
                SetSession("PaymentMethod", value);
            }
        }

        public static string TopUpCardId
        {
            get
            {
                return (GetSession("TopUpCardId").ToString());
            }
            set
            {
                SetSession("TopUpCardId", value);
            }
        }
        public static string PayGoodsAndServicesReceiptNumber
        {
            get
            {
                return (GetSession("PayGoodsAndServicesReceiptNumber") ?? "").ToString();
            }
            set
            {
                SetSession("PayGoodsAndServicesReceiptNumber", value);
            }
        }




        public static ReceiversDetailsViewModel ReceiversDetails
        {
            get
            {
                return (GetSession("ReceiversDetails") as ReceiversDetailsViewModel);
            }
            set
            {
                SetSession("ReceiversDetails", value);
            }
        }

        public static NonCardReceiversDetailsViewModel NonCardReceiversDetails
        {
            get
            {
                return (GetSession("NonCardReceiversDetails") as NonCardReceiversDetailsViewModel);
            }
            set
            {
                SetSession("NonCardReceiversDetails", value);
            }
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
        public static TrackFax TrackAFaxFaxDetails
        {
            get
            {
                return (GetSession("TrackAFaxFaxDetails") as TrackFax);
            }
            set
            {
                SetSession("TrackAFaxFaxDetails", value);
            }
        }

        public static int MFTCCardInformationId
        {
            get
            {
                return (int)(GetSession("MFTCCardInformationId"));
            }
            set
            {
                SetSession("MFTCCardInformationId", value);
            }
        }
        public static string MobileNo
        {
            get
            {
                return (string)(GetSession("MobileNo"));
            }
            set
            {
                SetSession("MobileNo", value);
            }
        }
        public static bool TrackATransfer
        {
            get
            {
                return (bool)(GetSession("TrackATransfer"));
            }
            set
            {
                SetSession("TrackATransfer", value);
            }
        }
        public static bool IsTransferFromHomePage
        {
            get
            {
                return (bool)(GetSession("IsTransferFromHomePage") ?? false);
            }
            set
            {
                SetSession("IsTransferFromHomePage", value);
            }
        }
        public static bool IsCommonEstimationPage
        {
            get
            {
                return (bool)(GetSession("IsCommonEstimationPage") ?? false);
            }
            set
            {
                SetSession("IsCommonEstimationPage", value);
            }
        }

        public static SenderAutoPaymentAddViewModel SenderAddKiiPayStandingOrder
        {
            get
            {
                return (GetSession("SenderAddKiiPayStandingOrder") as SenderAutoPaymentAddViewModel);
            }
            set
            {
                SetSession("SenderAddKiiPayStandingOrder", value);
            }
        }

        public static bool IDhasbeenExpired
        {
            get
            {
                return (bool)(GetSession("IDhasbeenExpired") ?? false);
            }
            set
            {
                SetSession("IDhasbeenExpired", value);
            }
        }


        public static bool IsTransactionOnpending
        {
            get
            {
                return (bool)(GetSession("IsTransactionOnpending") ?? false);
            }
            set
            {
                SetSession("IsTransactionOnpending", value);
            }
        }

        public static string PaymentRefrence
        {
            get
            {
                return (GetSession("PaymentRefrence") ?? "").ToString();
            }
            set
            {
                SetSession("PaymentRefrence", value);
            }
        }
        public static string BackButtonURL
        {
            get
            {
                return (GetSession("BackButtonURL") ?? "").ToString();
            }
            set
            {
                SetSession("BackButtonURL", value);
            }
        }
        public static string BackButtonURLMyMoneyFex
        {
            get
            {
                return (GetSession("BackButtonURLMyMoneyFex").ToString() ?? "");
            }
            set
            {
                SetSession("BackButtonURLMyMoneyFex", value);
            }
        }
        public static string PayGoodsAndServicesBackURL
        {
            get
            {
                return (GetSession("PayGoodsAndServicesBackURL").ToString() ?? "");
            }
            set
            {
                SetSession("PayGoodsAndServicesBackURL", value);
            }
        }


        public static int BusinessInformationId
        {
            get
            {
                return (int)(GetSession("BusinessInformationId") ?? 0);
            }
            set
            {
                SetSession("BusinessInformationId", value);
            }
        }


        public static string TransferringTopUpCardRegisteredCountry
        {
            get
            {
                return (GetSession("TransferringTopUpCardRegisteredCountry").ToString());
            }
            set
            {
                SetSession("TransferringTopUpCardRegisteredCountry", value);
            }
        }

        public static decimal AmountToBeTransferred
        {
            get
            {
                return (decimal)(GetSession("AmountToBeTransferred"));
            }
            set
            {
                SetSession("AmountToBeTransferred", value);
            }
        }

        public static int TransferCardId
        {
            get
            {
                return (int)(GetSession("TransferCardId"));
            }
            set
            {
                SetSession("TransferCardId", value);
            }
        }

        public static int MerchantPayInfoId
        {
            get
            {
                return (int)(GetSession("MerchantPayInfoId"));
            }
            set
            {
                SetSession("MerchantPayInfoId", value);
            }
        }
        public static int TransactionId
        {
            get
            {
                return (int)(GetSession("TransactionId") ?? 0);
            }
            set
            {
                SetSession("TransactionId", value);
            }
        }
        public static int RecipientId
        {
            get
            {
                return (int)(GetSession("RecipientId") ?? 0);
            }
            set
            {
                SetSession("RecipientId", value);
            }
        }

        public static string ErrorMessage
        {
            get
            {
                return (string)(GetSession("Errormessage") ?? null);
            }
            set
            {
                SetSession("Errormessage", value);
            }
        }


        public static string MFCN
        {
            get
            {
                return (GetSession("MFCN").ToString());
            }
            set
            {
                SetSession("MFCN", value);
            }
        }

        public static decimal AutoAmount
        {
            get
            {
                return (decimal)(GetSession("AutoAmount"));
            }
            set
            {
                SetSession("AutoAmount", value);
            }
        }

        public static ViewRegisteredFaxersViewModel FaxerDetails
        {
            get
            {
                return (GetSession("FaxerDetails") as ViewRegisteredFaxersViewModel);
            }
            set
            {
                SetSession("FaxerDetails", value);
            }
        }

        public static ViewRegisteredFaxersViewModel FaxerIdenty
        {
            get
            {
                return (GetSession("FaxerIdenty") as ViewRegisteredFaxersViewModel);
            }
            set
            {
                SetSession("FaxerIdenty", value);
            }
        }

        public static string MFTCCard
        {
            get
            {
                return (GetSession("MFTCCard") as string);
            }
            set
            {
                SetSession("MFTCCard", value);
            }
        }

        public static string Currency
        {
            get
            {
                return (GetSession("Currency").ToString());
            }
            set
            {
                SetSession("Currency", value);
            }
        }

        public static TransactionPendingViewModel TransactionPendingViewModel
        {
            get
            {
                return (GetSession("TransactionPendingViewModel") as TransactionPendingViewModel);
            }
            set
            {
                SetSession("TransactionPendingViewModel", value);
            }
        }
        public static NonCardMoneyFaxViewModel NonCardMoneyFaxViewModel
        {
            get
            {
                return (GetSession("NonCardMoneyFaxViewModel") as NonCardMoneyFaxViewModel);
            }
            set
            {
                SetSession("NonCardMoneyFaxViewModel", value);
            }
        }
        public static DemoLoginModel DemoLoginModel
        {

            get
            {
                return (GetSession("DemoLoginModel") as DemoLoginModel);
            }
            set
            {
                SetSession("DemoLoginModel", value);
            }
        }
        public static BankAccountPaymentConfirmationViewModel BankAccountPaymentConfirmationViewModel
        {

            get
            {
                return (GetSession("BankAccountPaymentConfirmationViewModel") as BankAccountPaymentConfirmationViewModel);
            }
            set
            {
                SetSession("BankAccountPaymentConfirmationViewModel", value);
            }
        }


        #region Sender Session

        // Sender Pay Bill Session


        public static string PayBillSupplierCountryCode
        {
            get
            {
                return (string)(GetSession("SupplierCountryCode"));
            }
            set
            {
                SetSession("SupplierCountryCode", value);
            }
        }


        public static string PaymentReference
        {
            get
            {
                return (string)(GetSession("PaymentReference"));
            }
            set
            {
                SetSession("PaymentReference", value);
            }
        }

        //Sender Bank Account Deposit

        public static SenderBankAccountDepositVm SenderBankAccountDeposit
        {
            get
            {
                return (GetSession("SenderBankAccountDeposit") as SenderBankAccountDepositVm);
            }
            set
            {
                SetSession("SenderBankAccountDeposit", value);
            }
        }


        public static SenderBankAccoutDepositEnterAmountVm SenderBankAccoutDepositEnterAmount
        {
            get
            {
                return (GetSession("SenderBankAccoutDepositEnterAmountVm") as SenderBankAccoutDepositEnterAmountVm);
            }
            set
            {
                SetSession("SenderBankAccoutDepositEnterAmountVm", value);
            }
        }

        public static SenderPersonalLoginInformationVM SenderPersonalLoginInformationVM
        {
            get
            {
                return (GetSession("SenderPersonalLoginInformationVM") as SenderPersonalLoginInformationVM);
            }
            set
            {
                SetSession("SenderPersonalLoginInformationVM", value);
            }
        }

        public static SenderPersonalDetailVM SenderPersonalDetialVm
        {

            get
            {
                return (GetSession("SenderPersonalDetialVm") as SenderPersonalDetailVM);
            }
            set
            {
                SetSession("SenderPersonalDetialVm", value);
            }
        }

        public static SenderPersonalAddressVM SenderPersonalAddressVM
        {

            get
            {
                return (GetSession("SenderPersonalAddressVM") as SenderPersonalAddressVM);
            }
            set
            {
                SetSession("SenderPersonalAddressVM", value);
            }
        }
        public static SenderBusinessDetailsViewModel SenderBusinessDetailsViewModel
        {

            get
            {
                return (GetSession("SenderBusinessDetailsViewModel") as SenderBusinessDetailsViewModel);
            }
            set
            {
                SetSession("SenderBusinessDetailsViewModel", value);
            }
        }
        public static SenderBusinessRegisteredViewModel SenderBusinessRegisteredViewModel
        {

            get
            {
                return (GetSession("SenderBusinessRegisteredViewModel") as SenderBusinessRegisteredViewModel);
            }
            set
            {
                SetSession("SenderBusinessRegisteredViewModel", value);
            }
        }
        public static SenderBusinessOperatingViewModel SenderBusinessOperatingViewModel
        {

            get
            {
                return (GetSession("SenderBusinessOperatingViewModel") as SenderBusinessOperatingViewModel);
            }
            set
            {
                SetSession("SenderBusinessOperatingViewModel", value);
            }
        }

        public static SearchKiiPayWalletVM SearchKiiPayWalletVM
        {


            get
            {
                return (GetSession("SearchKiiPayWalletVM") as SearchKiiPayWalletVM);
            }
            set
            {
                SetSession("SearchKiiPayWalletVM", value);
            }
        }

        public static CommonEnterAmountViewModel CommonEnterAmountViewModel
        {

            get
            {
                return (GetSession("CommonEnterAmountViewModel") as CommonEnterAmountViewModel);
            }
            set
            {
                SetSession("CommonEnterAmountViewModel", value);
            }
        }
        public static KiiPayTransferPaymentSummary KiiPayTransferPaymentSummary
        {

            get
            {
                return (GetSession("KiiPayTransferPaymentSummary") as KiiPayTransferPaymentSummary);
            }
            set
            {
                SetSession("KiiPayTransferPaymentSummary", value);
            }
        }


        public static PaymentMethodViewModel PaymentMethodViewModel
        {

            get
            {
                return (GetSession("PaymentMethodViewModel") as PaymentMethodViewModel);
            }
            set
            {
                SetSession("PaymentMethodViewModel", value);
            }
        }

        public static SenderAndReceiverDetialVM SenderAndReceiverDetials
        {


            get
            {
                return (GetSession("SenderAndReceiverDetials") as SenderAndReceiverDetialVM);
            }
            set
            {
                SetSession("SenderAndReceiverDetials", value);
            }
        }



        #endregion
        #region SenderAddMoneyToWallet
        public static SenderAddMoneyToWalletViewModel SenderAddMoneyToWallet
        {
            get
            {
                return (GetSession("SenderAddMoneyToWallet") as SenderAddMoneyToWalletViewModel);
            }
            set
            {
                SetSession("SenderAddMoneyToWallet", value);
            }
        }

        public static SenderAddMoneyToWalletSuccessViewModel SenderAddMoneySuccess
        {
            get
            {
                return (GetSession("SenderAddMoneySuccess") as SenderAddMoneyToWalletSuccessViewModel);
            }
            set
            {
                SetSession("SenderAddMoneySuccess", value);
            }
        }

        #endregion
        #region 
        //SenderPersonalKeyPayBankAccount
        public static string SentMobilePinCode
        {
            get
            {
                return (GetSession("PinCode") ?? "").ToString();
            }
            set
            {
                SetSession("PinCode", value);
            }
        }

        public static int BankId
        {
            get
            {
                return (int)(GetSession("BankId"));
            }
            set
            {
                SetSession("BankId", value);
            }

        }
        public static int CardId
        {
            get
            {
                return (int)(GetSession("CardId"));
            }
            set
            {
                SetSession("CardId", value);
            }


        }


        #endregion

        #region Sender Transfer => CashPickUp

        public static SenderCashPickUpVM SenderCashPickUp
        {
            get
            {
                return (GetSession("SenderCashPickUp") as SenderCashPickUpVM);
            }
            set
            {
                SetSession("SenderCashPickUp", value);
            }
        }
        public static SenderMobileEnrterAmountVm SenderMobileEnrterAmount
        {
            get
            {
                return (GetSession("SenderMobileEnrterAmount") as SenderMobileEnrterAmountVm);
            }
            set
            {
                SetSession("SenderMobileEnrterAmount", value);
            }
        }

        #endregion


        #region Sender  Mobile Transfer 
        public static SenderMobileMoneyTransferVM SenderMobileMoneyTransfer
        {
            get
            {
                return (GetSession("SenderMobileMoneyTransfer") as SenderMobileMoneyTransferVM);
            }
            set
            {
                SetSession("SenderMobileMoneyTransfer", value);
            }
        }


        public static SenderMoneyFexBankDepositVM SenderMoneyFexBankDeposit
        {
            get
            {
                return (GetSession("SenderMoneyFexBankDeposit") as SenderMoneyFexBankDepositVM);
            }
            set
            {
                SetSession("SenderMoneyFexBankDeposit", value);
            }
        }

        public static SenderLocalEnterAmountVM SenderLocalEnterAmount
        {
            get
            {
                return (GetSession("SenderLocalEnterAmount") as SenderLocalEnterAmountVM);
            }
            set
            {
                SetSession("SenderLocalEnterAmount", value);
            }
        }

        public static SenderAccountPaymentSummaryViewModel SenderAccountPaymentSummary
        {
            get
            {
                return (GetSession("SenderAccountPaymentSummary") as SenderAccountPaymentSummaryViewModel);
            }
            set
            {
                SetSession("SenderAccountPaymentSummary", value);
            }
        }

        public static SenderControlWalletUsageAmountVM SenderControlWalletUsageAmountVM
        {

            get
            {
                return (GetSession("SenderControlWalletUsageAmountVM") as SenderControlWalletUsageAmountVM);
            }
            set
            {
                SetSession("SenderControlWalletUsageAmountVM", value);
            }
        }


        #endregion



        #region Sender Bank Account Deposit



        #endregion

        #region Sender Transfer FAmily And Friends

        public static SenderTransferFamilyAndFriendsVm SenderTransferFamilyAndFriends
        {


            get
            {
                return (GetSession("SenderTransferFamilyAndFriends") as SenderTransferFamilyAndFriendsVm);
            }
            set
            {
                SetSession("SenderTransferFamilyAndFriends", value);
            }
        }

        #endregion


        #region Sender Pay For Services

        public static SenderPayForGoodsAndServicesVM SenderPayForGoodsAndServices
        {


            get
            {
                return (GetSession("SenderPayForGoodsAndServices") as SenderPayForGoodsAndServicesVM);
            }
            set
            {
                SetSession("SenderPayForGoodsAndServices", value);
            }
        }

        #endregion



        #region Sender Pay Bill

        public static SenderPayingSupplierAbroadReferenceOneVM SenderPayingSupplierAbroadReferenceOne
        {
            get
            {
                return (GetSession("SenderPayingSupplierAbroadReferenceOne") as SenderPayingSupplierAbroadReferenceOneVM);
            }
            set
            {
                SetSession("SenderPayingSupplierAbroadReferenceOne", value);
            }
        }

        public static SenderTopUpSupplierAbroadAbroadEnterAmontVM SenderTopUpSupplierAbroadAbroadEnterAmont
        {
            get
            {
                return (GetSession("SenderTopUpSupplierAbroadAbroadEnterAmont") as SenderTopUpSupplierAbroadAbroadEnterAmontVM);
            }
            set
            {
                SetSession("SenderTopUpSupplierAbroadAbroadEnterAmont", value);
            }
        }

        public static SenderTopUpAnAccountVM SenderTopUpAnAccount
        {
            get
            {
                return (GetSession("SenderTopUpAnAccount") as SenderTopUpAnAccountVM);
            }
            set
            {
                SetSession("SenderTopUpAnAccount", value);
            }
        }
        public static SenderTopUpSupplierAbroadVm SenderTopUpAnLocalAccount
        {
            get
            {
                return (GetSession("SenderTopUpAnLocalAccount") as SenderTopUpSupplierAbroadVm);
            }
            set
            {
                SetSession("SenderTopUpAnLocalAccount", value);
            }
        }
        public static string SenderTopUpAccountNumber
        {
            get
            {
                return (GetSession("SenderTopUpAccountNumber") ?? "").ToString();
            }
            set
            {
                SetSession("SenderTopUpAccountNumber", value);
            }
        }
        #endregion

        #region Sender Request a Pyment
        public static SenderSendARequestVM SenderSendARequest
        {


            get
            {
                return (GetSession("SenderSendARequest") as SenderSendARequestVM);
            }
            set
            {
                SetSession("SenderSendARequest", value);
            }
        }
        public static TransactionSummaryVM TransactionSummary
        {

            get
            {

                return (GetSession("TransactionSummary") as TransactionSummaryVM);
            }
            set
            {

                SetSession("TransactionSummary", value);
            }
        }

        public static SenderPayAPaymentRequestViewModel SenderPayARequest
        {

            get
            {
                return GetSession("SenderPayARequestSession") as SenderPayAPaymentRequestViewModel;
            }
            set
            {
                SetSession("SenderPayARequestSession", value);
            }
        }

        #endregion

        #region Bank Api
        public static AccessTokenVM BankAccessToken
        {
            get
            {
                return GetSession("BankAccessToken") as AccessTokenVM;
            }
            set
            {
                SetSession("BankAccessToken", value);
            }
        }

        public static SenderPayMonthlyBillVM SenderPayMonthlyBillVM
        {
            get
            {
                return (GetSession("SenderPayMonthlyBillVM") as SenderPayMonthlyBillVM);
            }
            set
            {
                SetSession("SenderPayMonthlyBillVM", value);
            }
        }

        public static SenderTopUpSupplierAbroadVm SenderTopUpSupplierAbroadVm
        {
            get
            {
                return (GetSession("SenderTopUpSupplierAbroadVm") as SenderTopUpSupplierAbroadVm);
            }
            set
            {
                SetSession("SenderTopUpSupplierAbroadVm", value);
            }
        }

        public static SenderBusinessLoginInfoViewModel SenderBusinessLoginInfoViewModel
        {
            get
            {
                return (GetSession("SenderBusinessLoginInfoViewModel") as SenderBusinessLoginInfoViewModel);
            }
            set
            {
                SetSession("SenderBusinessLoginInfoViewModel", value);
            }
        }



        public static string ReceiptNo
        {
            get
            {
                return (GetSession("ReceiptNo").ToString());
            }
            set
            {
                SetSession("ReceiptNo", value);
            }
        }

        public static int SenderId
        {

            get
            {
                return (GetSession("SenderId").ToInt());
            }
            set
            {
                SetSession("SenderId", value);
            }
        }
        #endregion


        public static TransactionEmailType TransactionEmailTypeSession
        {

            get
            {
                return (GetSession("TransactionEmailTypeSession") ==  null ? TransactionEmailType.CustomerSupport 
                    : (TransactionEmailType)GetSession("TransactionEmailTypeSession"));
            }
            set
            {
                SetSession("TransactionEmailTypeSession", value);
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

    public class FaxerRegistrationSession
    {
        public Models.RegisterViewModel FaxerDetail { get; set; }
        public Models.FaxerIdentification Identification { get; set; }
        public Models.FaxerContactDetails Contact { get; set; }

    }
    public class LoggedUser
    {
        public string UserName { get; set; }
        public string FullName { get; set; }
        /// <summary>
        /// this is faxer id
        /// </summary>
        public int Id { get; set; }
        public string CountryCode { get; internal set; }

        public string FaxerMFCode { get; set; }
        public string PhoneNo { get; set; }

        public string CountryPhoneCode { get; set; }
        public bool IsBusiness { get; set; }
        public string HouseNo { get; internal set; }
        public string PostCode { get; internal set; }
        public string FirstName { get; set; }
    }

    public class DemoLoginModel
    {

        public string UserName { get; set; }
        public string Password { get; set; }
    }



}