//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FAXER.PORTAL.DB
{
    using FAXER.PORTAL.Models;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    /// <summary>
    /// Virtual Account Information 
    /// </summary>
    public partial class KiiPayPersonalWalletInformation
    {
        public int Id { get; set; }

        #region Personal Details Info

        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public System.DateTime CardUserDOB { get; set; }

        public Gender Gender { get; set; }

        #endregion

        #region Personal Address

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string CardUserCountry { get; set; }
        public string CardUserCity { get; set; }
        public string CardUserState { get; set; }
        public string CardUserPostalCode { get; set; }

        #endregion
        /// <summary>
        /// Virtual Account no 
        /// but you have to call common function  to get
        /// only number For ex : Common.Common.MFTCCardNumber.Decrypt().GetVirtualAccountNo()
        /// </summary>
        public string MobileNo { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal CashWithdrawalLimit { get; set; }
        public CardLimitType CashLimitType { get; set; }
        public decimal GoodsPurchaseLimit { get; set; }
        public AutoPaymentFrequency GoodsLimitType { get; set; }
        
        public bool AutoTopUp { get; set; }

        public string CardUserTel { get; set; }
        public string CardUserEmail { get; set; }
        public CardStatus CardStatus { get; set; }
        public string UserPhoto { get; set; }
        public string MFTCardPhoto { get; set; }
        public decimal AutoTopUpAmount { get; set; }
        public bool IsDeleted { get; set; }


        public WalletIsOF walletIsOF { get; set; }
        public bool UnCompletedProfile { get;  set; }
    }

    public class SenderKiiPayPersonalAccount
    {

        public int Id { get; set; }

        [ForeignKey("KiiPayPersonalWalletInformation")]
        public int KiiPayPersonalWalletId { get; set; }

        [ForeignKey("SenderInformation")]
        public int SenderId { get; set; }
        public decimal CashWithdrawalLimitAmount { get; set; }

        public CardLimitType CashLimitType { get; set; }

        public decimal GoodPurchaseLimitAmount { get; set; }

        public AutoPaymentFrequency GoodFrequency { get; set; }


        public KiiPayAccountIsOF KiiPayAccountIsOF { get; set; }


        public virtual FaxerInformation SenderInformation { get; set; }

        public virtual KiiPayPersonalWalletInformation KiiPayPersonalWalletInformation { get; set; }

    }

    public enum KiiPayAccountIsOF
    {

        Sender,
        Family
    }
    public enum CardStatus
    {
        Active,
        InActive,
        IsDeleted,
        IsRefunded
    }
    public enum CardLimitType
    {

        [Display(Name = "No Limit Set")]
        NoLimitSet,
        Daily,
        Weekly,
        Monthly

    }

    public enum WalletIsOF
    {
        KiiPayIndividualUser,
        Sender

    }
}
