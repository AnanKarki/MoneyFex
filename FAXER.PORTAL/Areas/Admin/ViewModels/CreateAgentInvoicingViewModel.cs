using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class CreateAgentInvoicingViewModel
    {
        public const string BindProperty= "Id,InvoiceNo ,Country ,City , AgentId, AccountNo ,NoteToReceipient , SubTotal,Discount , ShippingAmount ,TotalAmount , DiscountPercentage, Item,InvoiceDate,Name,PartnerId,GeneralId";
        public int Id{ get; set; }
        public string InvoiceNo{ get; set; }
        public string Country{ get; set; }
        public string City{ get; set; }
        public int AgentId{ get; set; }
        public string AccountNo{ get; set; }
        public string NoteToReceipient{ get; set; }
        public  decimal SubTotal{ get; set; }
        public  decimal Discount{ get; set; }
        public  decimal ShippingAmount{ get; set; }
        public  decimal TotalAmount{ get; set; }
       
        public  string DiscountPercentage{ get; set; }
        public List<ItemListVm> Item { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime? InvoiceDate { get; set; }
        public string  CountrySymbol { get; set; }
        public string  Name { get; set; }
        public string Address { get; set; }

        #region Partner
        public int PartnerId{ get; set; }
        #endregion

        #region General
        
        public string Email{ get; set; }
        #endregion
    }

    public class ItemListVm
    {
        public int Id{ get; set; }
        public string Description{ get; set; }
        public int Quantity{ get; set; }
        public decimal Price{ get; set; }
        public decimal Amount{ get; set; }
    }

    public class AgentPayaAnInvoiceSuccessVm
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InvoiceNo { get; set; }
    }
}