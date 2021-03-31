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
    
    public partial class SenderKiiPayBusinessPaymentTransaction
    {
        public int Id { get; set; }
        public int SenderKiiPayBusinessPaymentInformationId { get; set; }
        public int PaidToKiiPayBusinessWalletId { get; set; }

        public int? StaffId { get; set; }


        #region Transaction Details 
        public string PaymentMethod { get; set; }

        public string SendingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public decimal PaymentAmount { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ReceivingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentReference { get; set; }

        public string ReceiptNumber { get; set; }

        public SenderPaymentMode SenderPaymentMode { get; set; }

        public System.DateTime PaymentDate { get; set; }

        /// <summary>
        /// Payment Type (International , Local )
        /// </summary>
        public PaymentType PaymentType { get; set; }
        #endregion



        public bool IsAutoPaymentTransaction { get; set; }
        public virtual SenderKiiPayBusinessPaymentInformation SenderKiiPayBusinessPaymentInformation { get; set; }
    }
}