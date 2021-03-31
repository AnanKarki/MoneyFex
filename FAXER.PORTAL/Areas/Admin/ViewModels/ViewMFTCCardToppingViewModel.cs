using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{


    public class ViewMFTCCardTopUpViewModel
    {
        public int Id { get; set; }
        public string FaxerFirstName { get; set; }
        public string FaxerMiddleName { get; set; }
        public string FaxerLastName { get; set; }
        public string CardUserFirstName { get; set; }
        public string CardUserMiddleName { get; set; }
        public string CardUserLastName { get; set; }
        public string MFTCCardNumber { get; set; }
        public decimal MFTCCardTopUPAmount { get; set; }
        public string Currency { get; set; }
        public string MFTCCardTopUPDate { get; set; }
        public string MFTCCardTopUPTime { get; set; }
        public decimal AmountOnMFTCCard { get; set; }
        public string MFTCCardTopUpMethod { get; set; }
        public string PaymentMethod { get; set; }
        public string MFTCCardTopUpper { get; set; }
        public decimal WithdrawalLimitAmount { get; set; }
        public string WithdrawalLimitType { get; set; }
        public decimal PurchaseLimitAmount { get; set; }
        public string PurchaseLimitType { get; set; }
        public DateTime DateTimeForOrdering { get; set; }

        public TopupedBy TopupedBy { get; set; }

        #region dr cr card informaiton
        public string CardName
        { get; set; }
        public string CardNumber { get; set; }
        public string CardSecurityNumber { get; set; }
        public string CardExpYear { get; set; }
        public string CardExpMonth { get; set; }
        public string IsCreditDebitSaved { get; set; }
        public string IsAutoRechargeActivated { get; set; }
        public int CreditDebitStatus { get; set; }
        #endregion

    }

    public enum TopupedBy
    {

        Sender, SomeoneElse, Admin
    }
    public class ViewDebitCreditCardInfoViewModel
    {
        public int Id { get; set; }




    }
    public class ViewMFTCCardToppingViewModel
    {
        public ViewMFTCCardToppingViewModel()
        {
            ViewMFTCCardTopUp = new List<ViewMFTCCardTopUpViewModel>();
            ViewDebitCreditCardInfo = new List<ViewDebitCreditCardInfoViewModel>();
        }

        public List<ViewMFTCCardTopUpViewModel> ViewMFTCCardTopUp { get; set; }
        public List<ViewDebitCreditCardInfoViewModel> ViewDebitCreditCardInfo { get; set; }
    }
}