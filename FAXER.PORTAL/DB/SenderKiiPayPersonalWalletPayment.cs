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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class SenderKiiPayPersonalWalletPayment
    {
        public int Id { get; set; }
       
        public int FaxerId { get; set; }
        public decimal RecievingAmount { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public decimal FaxingAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal ExchangeRate { get; set; }
        [ForeignKey("KiiPayPersonalWalletInformation")]
        public int KiiPayPersonalWalletInformationId { get; set; }

        public string ReceiptNumber { get; set; }
        /// <summary>
        /// Current User type
        /// Faxer, Admin, Agent , Merchant, staff, etc.
        /// </summary>
        public OperatingUserType OperatingUserType { get; set; }

        public string PaymentMethod { get; set; }
        public string OperatingUserName { get; set; }
        public int OperatingStaffId { get; set; }
        public bool IsAutoPaymentTransaction { get; set; }
        public virtual KiiPayPersonalWalletInformation KiiPayPersonalWalletInformation { get; set; }
       

        public FaxingStatus FaxingStatus { get; set; }
    }
}
