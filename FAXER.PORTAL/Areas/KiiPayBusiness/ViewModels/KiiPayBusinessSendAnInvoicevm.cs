using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels
{
    public class KiiPayBusinessSendAnInvoicevm
    {
        public const string BindProperty = "InvoiceMaster , InvoiceDetails";
        public InvoiceMastervm InvoiceMaster { get; set; }
        public List<InvoiceDetailsvm> InvoiceDetails { get; set; }
    }
    public class InvoiceMasterListvm
    {
       
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public string InvoiceDate { get; set; }

        public string ReciverName { get; set; }
        public string ReciverWalletNo { get; set; }
        public string SenderName { get; set; }
        public string StatusColor { get; set; }
        public string SenderWalletNo { get; set; }
        public string InvoiceStatus { get; set; }
        public InvoiceStatus InvoiceStatusEnum { get; set; }
        public string CurrencySymbol { get; set; }
        public decimal TotalAmount { get; set; }

        public DateTime TransactionDate { get; set; }


    }
    public class InvoiceMastervm
    {
        public const string BindProperty = "Id , InvoiceNo , InvoiceDate ,InvoiceDateToString ,AmountDue , FromBusinessName,FromInvoiceMobileNumber ,ToInvoiceMobileNumber " +
            ",ToCCInvoiceMobileNumber , NoteToReceipient, Discount,DiscountAmount , Subtotal , Shipping, TotalAmount, DiscountMethodId,CountryCode  ";
        public int Id { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string InvoiceDateToString { get; set; } 
        public decimal AmountDue { get; set; }
        public string FromBusinessName { get; set; }
        public string FromInvoiceMobileNumber { get; set; }
        public string ToInvoiceMobileNumber { get; set; }
        public string ToCCInvoiceMobileNumber { get; set; }
        public string NoteToReceipient { get; set; }
        public decimal Discount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Shipping { get; set; }
        public decimal TotalAmount { get; set; }
        public DiscountMethod DiscountMethodId { get; set; }
        public string CountryCode { get; set; }

    }
    public class InvoiceDetailsvm
    {
        public const string BindProperty = "ItemName , Quantity , Price ,Amount ,CurrencySymbol , InvoiceMasterId";

        public string ItemName { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
        public string CurrencySymbol { get; set; }
        public int InvoiceMasterId { get; set; }
    }
    public class KiiPayBusinessSendAnInvoiceSuccessvm
    {
        public decimal Amount { get; set; }
        public string CurrencySymbol { get; set; }
        public string ReceiverName { get; set; }
    }

}