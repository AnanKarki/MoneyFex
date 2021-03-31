using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static FAXER.PORTAL.Services.SSenderForAllTransfer;

namespace FAXER.PORTAL.Common
{
    public class AdminSession
    {
        public static RegisterAStaffViewModel StaffBasicDetails
        {
            get
            {
                return GetSession("StaffBasicDetails") as RegisterAStaffViewModel;
            }
            set
            {
                SetSession("StaffBasicDetails", value);
            }
        }

        public static StaffContactDetailsViewModel StaffContactDetails
        {
            get
            {
                return GetSession("StaffContactDetails") as StaffContactDetailsViewModel;
            }
            set
            {
                SetSession("StaffContactDetails", value);
            }
        }

        public static StaffContactDetails2ViewModel StaffContactDetails2
        {
            get
            {
                return GetSession("StaffContactDetails2") as StaffContactDetails2ViewModel;
            }
            set
            {
                SetSession("StaffContactDetails2", value);
            }
        }

        public static StaffContactDetails3ViewModel StaffContactDetails3
        {
            get
            {
                return GetSession("StaffContactDetails3") as StaffContactDetails3ViewModel;
            }
            set
            {
                SetSession("StaffContactDetails3", value);
            }
        }

        public static StaffNextOfKinViewModel StaffNOKDetails
        {
            get
            {
                return GetSession("StaffNOKDetails") as StaffNextOfKinViewModel;
            }
            set
            {
                SetSession("StaffNOKDetails", value);
            }
        }

        public static StaffComplianceDocumentationViewModel StaffDocumentsDetails
        {
            get
            {
                return GetSession("StaffDocumentsDetails") as StaffComplianceDocumentationViewModel;
            }
            set
            {
                SetSession("StaffDocumentsDetails", value);
            }
        }

        public static StaffEmploymentViewModel StaffEmploymentDetails
        {
            get
            {
                return GetSession("StaffEmploymentDetails") as StaffEmploymentViewModel;
            }
            set
            {
                SetSession("StaffEmploymentDetails", value);
            }
        }

        public static int StaffId
        {
            get
            {
                return Convert.ToInt32(GetSession("StaffId"));
            }
            set
            {
                SetSession("StaffId", value);
            }
        }

        public static int StaffLoginHistoryId
        {
            get
            {
                return Convert.ToInt32(GetSession("StaffLoginHistoryId"));
            }
            set
            {
                SetSession("StaffLoginHistoryId", value);
            }
        }
        public static SystemLoginLevel StaffLoginLevel
        {
            get
            {
                if (GetSession("StaffLoginLevel") == null)
                    return SystemLoginLevel.Level3;
                return (SystemLoginLevel)Convert.ToInt32(GetSession("StaffLoginLevel").ToString());
            }
            set
            {
                SetSession("StaffLoginLevel", (int)value);
            }
        }

        public static AgentStaffInfoAndLoginViewModel AgentStaffInfoAndLoginViewModel
        {

            get
            {
                return GetSession("AgentStaffInfoAndLoginViewModel") as AgentStaffInfoAndLoginViewModel;
            }
            set
            {
                SetSession("AgentStaffInfoAndLoginViewModel", value);
            }
        }
        #region Staff transfer Session
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
        public static CreditDebitCardViewModel CreditDebitDetails
        {
            get
            {
                return GetSession("CreditDebitDetails") as CreditDebitCardViewModel;
            }
            set { SetSession("CreditDebitDetails", value); }
        }

        public static string CardType
        {
            get
            {

                return (GetSession("CardType") ?? "").ToString();
            }
            set { SetSession("CardType", value); }
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
        public static TransactionServiceType TransactionServiceType
        {
            get
            {
                return (TransactionServiceType)(GetSession("TransactionServiceType") ?? TransactionServiceType.All);
            }
            set
            {
                SetSession("TransactionServiceType", value);
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
        public static SenderTransactionHistoryList SenderTransactionHistoryList
        {


            get
            {
                return (GetSession("SenderTransactionHistoryList") as SenderTransactionHistoryList);
            }
            set
            {
                SetSession("SenderTransactionHistoryList", value);
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
        #endregion

        public static void Clear()
        {
            HttpContext.Current.Session.Abandon();
            HttpContext.Current.Session.Clear();

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