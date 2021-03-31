using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.MoblieTransferApi.Models;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Common
{
    public class AgentSession
    {
        private static object GetSession(string key)
        {
            return HttpContext.Current.Session[key];
        }
        private static void SetSession(string key, object value)
        {
            HttpContext.Current.Session[key] = value;
        }
        public static bool IsTransferFromCalculateHowMuch
        {
            get
            {
                return (bool)(GetSession("IsTransferFromCalculateHowMuch") ?? false);
            }
            set
            {
                SetSession("IsTransferFromCalculateHowMuch", value);
            }
        }
        public static bool IsAuxAgent
        {
            get
            {
                return (bool)(GetSession("IsAuxAgent") ?? false);
            }
            set
            {
                SetSession("IsAuxAgent", value);
            }
        }
        public static AgentInformation AgentInformation
        {
            get
            {
                return GetSession("AgentInformation") as AgentInformation;
            }
            set { SetSession("AgentInformation", value); }
        }
        public static AgentInformtionViewModel AgentInformtionViewModel
        {
            get
            {
                return GetSession("AgentInformtionViewModel") as AgentInformtionViewModel;
            }
            set { SetSession("AgentInformtionViewModel", value); }
        }


        public static AgentUserDetailsViewModel AgentUserInformation
        {
            get
            {
                return GetSession("AgentUserInformation") as AgentUserDetailsViewModel;
            }
            set { SetSession("AgentUserInformation", value); }
        }

        public static AgentUserContactDetailsViewModel AgentUserContactDetails
        {
            get
            {
                return GetSession("AgentUserContactDetails") as AgentUserContactDetailsViewModel;
            }
            set { SetSession("AgentUserContactDetails", value); }
        }

        public static AgentLogin AgentLogin
        {
            get
            {
                return GetSession("AgentLogin") as AgentLogin;
            }
            set { SetSession("AgentLogin", value); }
        }
        public static AgentStaffLogin AgentStaffLogin
        {
            get
            {
                return GetSession("AgentStaffLogin") as AgentStaffLogin;
            }
            set { SetSession("AgentStaffLogin", value); }
        }


        public static LoggedUserVm LoggedUser
        {
            get
            {
                return GetSession("LoggedUser") as LoggedUserVm;
            }
            set { SetSession("LoggedUser", value); }
        }
        public static AgentFundAccountViewModel AgentFundAccount
        {
            get
            {
                return GetSession("AgentFundAccount") as AgentFundAccountViewModel;
            }
            set { SetSession("AgentFundAccount", value); }
        }

        public static string AgentTimeZone
        {
            get
            {
                return GetSession("AgentTimeZone").ToString();
            }
            set
            {
                SetSession("AgentTimeZone", value);
            }
        }

        public static StaffDetailsViewModel StaffDetailsViewModel
        {


            get
            {
                return GetSession("StaffDetailsViewModel") as StaffDetailsViewModel;
            }
            set { SetSession("StaffDetailsViewModel", value); }

        }

        public static StaffContaactDetailsViewModel StaffContaactDetailsViewModel
        {
            get
            {
                return GetSession("StaffContaactDetailsViewModel") as StaffContaactDetailsViewModel;
            }
            set { SetSession("StaffContaactDetailsViewModel", value); }
        }

        public static BecomeAnAgent BecomeAnAgent
        {
            get
            {
                return GetSession("BecomeAnAgent") as BecomeAnAgent;
            }
            set { SetSession("BecomeAnAgent", value); }
        }
        public static AgentsDetailsViewModel AgentDetails
        {
            get
            {
                return GetSession("AgentDetails") as AgentsDetailsViewModel;
            }
            set { SetSession("AgentDetails", value); }
        }
        public static string AgentValidEmailAddress
        {
            get
            {
                return GetSession("AgentValidEmailAddress").ToString();
            }
            set { SetSession("AgentValidEmailAddress", value); }
        }
        public static string MFTCCardAccessCode
        {
            get
            {
                return GetSession("MFTCCardAccessCode").ToString();
            }
            set { SetSession("MFTCCardAccessCode", value); }
        }
        public static string CashWithdrawalCode
        {
            get
            {
                return GetSession("CashWithdrawalCode").ToString();
            }
            set { SetSession("CashWithdrawalCode", value); }
        }

        public static string TempUrl
        {
            get
            {
                return GetSession("TempUrl").ToString();
            }
            set
            {
                SetSession("TempUrl", value);
            }
        }

        public static string VerificationCode
        {
            get
            {

                return (GetSession("VerificationCode") ?? "").ToString();

            }
            set
            {
                SetSession("VerificationCode", value);
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
        public static string BusinessCardAccessCode
        {
            get
            {
                return GetSession("BusinessCardAccessCode").ToString();
            }
            set { SetSession("BusinessCardAccessCode", value); }
        }



        public static string AgentPasswordRequestCode
        {
            get
            {
                return GetSession("AgentPasswordRequestCode").ToString();
            }
            set { SetSession("AgentPasswordRequestCode", value); }
        }
        public static string FormURL
        {
            get
            {
                return (GetSession("FormURL") ?? "").ToString();
            }
            set { SetSession("FormURL", value); }
        }
        public static string FirstLogin
        {
            get
            {
                return (GetSession("FirstLogin") ?? "").ToString();
            }
            set { SetSession("FirstLogin", value); }
        }

        public static StaffComplianceDocViewModel StaffComplianceDocViewModel
        {

            get
            {
                return GetSession("StaffComplianceDocViewModel") as StaffComplianceDocViewModel;
            }
            set { SetSession("StaffComplianceDocViewModel", value); }
        }

        public static string AgentLoginCode
        {
            get
            {
                return (GetSession("AgentLoginCode") ?? "").ToString();
            }
            set { SetSession("AgentLoginCode", value); }
        }
        public static PayAReceiveCashPickupReceiverDetailsViewModel PayAReceiveCashPickupReceiverDetails
        {

            get
            {
                return GetSession("PayAReceiveCashPickupReceiverDetails") as PayAReceiveCashPickupReceiverDetailsViewModel;
            }
            set
            {
                SetSession("PayAReceiveCashPickupReceiverDetails", value);
            }
        }

        public static string MFCN
        {
            get
            {

                return (GetSession("MFCN") ?? "").ToString();
            }
            set { SetSession("MFCN", value); }
        }
       
        public static string ReceiptNo
        {
            get
            {

                return (GetSession("ReceiptNo") ?? "").ToString();
            }
            set { SetSession("ReceiptNo", value); }
        }

        #region Transfer money

        public static int SenderId
        {
            get
            {
                return (int)(GetSession("SenderId") ?? 0);
            }
            set
            {
                SetSession("SenderId", value);
            }
        }


        public static CashPickUpEnterAmountViewModel CashPickUpEnterAmount
        {
            get
            {
                return (GetSession("CashPickUpEnterAmountViewModel") as CashPickUpEnterAmountViewModel);
            }
            set
            {
                SetSession("CashPickUpEnterAmountViewModel", value);
            }
        }

        public static ReceiverDetailsInformationViewModel ReceiverDetailsInformation
        {
            get
            {
                return (GetSession("ReceiverDetailsInformation") as ReceiverDetailsInformationViewModel);
            }
            set
            {
                SetSession("ReceiverDetailsInformation", value);
            }
        }

        public static MobileMoneyTransferEnterAmountViewModel MobileMoneyTransferEnterAmount
        {
            get
            {
                return (GetSession("MobileMoneyTransferEnterAmount") as MobileMoneyTransferEnterAmountViewModel);
            }
            set
            {
                SetSession("MobileMoneyTransferEnterAmount", value);
            }
        }


        public static SenderBankAccountDepositVm AgentBankAccountDeposit
        {
            get
            {
                return (GetSession("AgentBankAccountDeposit") as SenderBankAccountDepositVm);
            }
            set
            {
                SetSession("AgentBankAccountDeposit", value);
            }
        }
        public static BankDepositAbroadEnterAmountVM BankDepositAbroadEnterAmount
        {
            get
            {
                return (GetSession("BankDepositAbroadEnterAmount") as BankDepositAbroadEnterAmountVM);
            }
            set
            {
                SetSession("BankDepositAbroadEnterAmount", value);
            }
        }
        public static CreditDebitCardViewModel CreditDebitDetails
        {
            get
            {
                return GetSession("CreditDebitDetails") as CreditDebitCardViewModel;
            }
            set { SetSession("CreditDebitDetails", value); }
        }


        #endregion



        #region Agent Pay bills

        public static PayingSupplierReferenceViewModel AgentPayingSupplierReference
        {
            get
            {
                return GetSession("AgentPayingSupplierReference") as PayingSupplierReferenceViewModel;
            }
            set { SetSession("AgentPayingSupplierReference", value); }
        }
        public static PayMonthlyBillViewModel PayMonthlyBillViewModel
        {
            get
            {
                return GetSession("PayMonthlyBillViewModel") as PayMonthlyBillViewModel;
            }
            set { SetSession("PayMonthlyBillViewModel", value); }
        }
        public static TopUpSupplierEnterAmountVM AgentTopUpSupplierEnterAmount
        {
            get
            {
                return (GetSession("AgentTopUpSupplierEnterAmount") as TopUpSupplierEnterAmountVM);
            }
            set
            {
                SetSession("AgentTopUpSupplierEnterAmount", value);
            }
        }

        public static TopUpAnAccountViewModel TopUpAnAccountViewModel
        {
            get
            {
                return (GetSession("TopUpAnAccountViewModel") as TopUpAnAccountViewModel);
            }
            set
            {
                SetSession("TopUpAnAccountViewModel", value);
            }
        }


        public static PayAReceiverKiiPayWalletViewModel PayAReceiverKiiPayWallet
        {


            get
            {
                return (GetSession("PayAReceiverKiiPayWallet") as PayAReceiverKiiPayWalletViewModel);
            }
            set
            {
                SetSession("PayAReceiverKiiPayWallet", value);
            }
        }
        public static PayAReceiverCashPickupViewModel PayAReceiverCashPickupViewModel
        {


            get
            {
                return (GetSession("PayAReceiverCashPickupViewModel") as PayAReceiverCashPickupViewModel);
            }
            set
            {
                SetSession("PayAReceiverCashPickupViewModel", value);
            }
        }
        public static PayAReceiverKiiPayWalletEnteramountViewModel PayAReceiverKiiPayWalletEnteramount
        {

            get
            {
                return (GetSession("PayAReceiverKiiPayWalletEnteramount") as PayAReceiverKiiPayWalletEnteramountViewModel);
            }
            set
            {
                SetSession("PayAReceiverKiiPayWalletEnteramount", value);
            }
        }
        public static PayAReceiverKiipayWalletSuccessViewModel PayAReceiverKiipayWalletSuccess
        {

            get
            {
                return (GetSession("PayAReceiverKiipayWalletSuccess") as PayAReceiverKiipayWalletSuccessViewModel);
            }
            set
            {
                SetSession("PayAReceiverKiipayWalletSuccess", value);
            }
        }

        public static CashPickupInformationViewModel CashPickupInformationViewModel
        {

            get
            {
                return (GetSession("CashPickupInformationViewModel") as CashPickupInformationViewModel);
            }
            set
            {
                SetSession("CashPickupInformationViewModel", value);
            }
        }
        public static CashPickUpReceiverDetailsInformationViewModel CashPickUpReceiverDetailsInformationViewModel
        {


            get
            {
                return (GetSession("CashPickUpReceiverDetailsInformationViewModel") as CashPickUpReceiverDetailsInformationViewModel);
            }
            set
            {
                SetSession("CashPickUpReceiverDetailsInformationViewModel", value);
            }
        }

        public static SendMoneToKiiPayWalletViewModel SendMoneToKiiPayWalletViewModel
        {
            get
            {
                return (GetSession("SendMoneToKiiPayWalletViewModel") as SendMoneToKiiPayWalletViewModel);
            }
            set
            {
                SetSession("SendMoneToKiiPayWalletViewModel", value);
            }
        }

        public static KiiPayReceiverDetailsInformationViewModel KiiPayReceiverDetailsInformationViewModel
        {
            get
            {
                return (GetSession("KiiPayReceiverDetailsInformationViewModel") as KiiPayReceiverDetailsInformationViewModel);
            }
            set
            {
                SetSession("KiiPayReceiverDetailsInformationViewModel", value);
            }
        }

        public static SendMoneyToKiiPayEnterAmountViewModel SendMoneyToKiiPayEnterAmountViewModel
        {
            get
            {
                return (GetSession("SendMoneyToKiiPayEnterAmountViewModel") as SendMoneyToKiiPayEnterAmountViewModel);
            }
            set
            {
                SetSession("SendMoneyToKiiPayEnterAmountViewModel", value);
            }
        }
        #endregion


        public static MobileTransferAccessTokeneResponse MobileTransferAccessTokeneResponse
        {
            get
            {
                return GetSession("MobileTransferAccessTokeneResponse") as MobileTransferAccessTokeneResponse;
            }
            set { SetSession("MobileTransferAccessTokeneResponse", value); }
        }

        public static MTNCameroonResponseParamVm MTNCameroonResponseParamVm
        {
            get
            {
                return GetSession("MTNCameroonResponseParamVm") as MTNCameroonResponseParamVm;
            }
            set { SetSession("MTNCameroonResponseParamVm", value); }
        }
        public static string CardType
        {
            get
            {

                return (GetSession("CardType") ?? "").ToString();
            }
            set { SetSession("CardType", value); }
        }

        public static AgentTransactionSummaryVm AgentTransactionSummaryVm
        {
            get
            {
                return (GetSession("AgentTransactionSummaryVm") as AgentTransactionSummaryVm);
            }
            set
            {
                SetSession("AgentTransactionSummaryVm", value);
            }
        }
    }

}


