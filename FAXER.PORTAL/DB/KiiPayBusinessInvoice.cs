using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    /// <summary>
    /// sender is  invoice requester  
    /// receiver is invoice payer
    /// </summary>
    public class KiiPayBusinessInvoiceMaster
    {
        public int Id { get; set; }
        /// <summary>
        /// KiiPay Business Info Id (sender )
        /// </summary>
        public int SenderId { get; set; }
        /// <summary>
        /// KiiPay Business Info Id (Receiver )
        /// </summary>
        public int ReceiverId { get; set; }

        /// <summary>
        /// KiiPay Business Info Id (CC)
        /// </summary>
        public int CCWalletId { get; set; }
        public int SenderWalletId { get; set; }

        public int ReceiverWalletId { get; set; }

        /// <summary>
        /// Business Mobile No
        /// </summary>
        public string SenderWalletNo { get; set; }

        /// <summary>
        /// Business Mobile No
        /// </summary>
        public string ReceiverWalletNo { get; set; }

        /// <summary>
        /// Business Mobile No
        /// </summary>
        public string CCWalletNo { get; set; }


        public DiscountMethod DiscountMethod { get; set; }
        public decimal Discount { get; set; }

        public decimal DiscountAmount { get; set; }
        public decimal SubTotal { get; set; }
        public decimal ShippingCost { get; set; }

        #region Transaction Details 

        public decimal TotalAmount { get; set; }

        public decimal PayerExchangeRate { get; set; }
        public decimal AmountToBePaidByPayer { get; set; }
        public decimal PayerFee { get; set; }

        public decimal TotalAmountIncludingFee { get; set; }


        #endregion


        public string NoteToReceipent { get; set; }



        public string SenderCountry { get; set; }
        public string ReceiverCountry { get; set; }

        public InvoiceStatus InvoiceStatus { get; set; }


        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }

        public DateTime? PaymentDate { get; set; }


        public DateTime CreationDateTime { get; set; }

    }
    public class KiiPayBusinessInvoiceDetail
    {

        public int Id { get; set; }
        public string ItemName { get; set; }

        public int KiiPayBusinessInvoiceMasterId { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        
    }

    public enum InvoiceStatus {

        Paid,
        UnPaid,
        Cancelled,
        Deleted
    }
    public enum DiscountMethod
    {

        Perc,
        Amount
    }
}