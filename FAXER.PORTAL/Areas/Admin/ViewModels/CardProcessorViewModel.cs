using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class CardProcessorViewModel
    {
        public const string BindProperty = "Id,Country ,CountryName , TransferType , TransferTypeName ,TransferMethod ," +
            "TransferMethodName ,CardProcessorName, ContactName,TelephoneNo,Email ,CreatedDate ,CreatedBy ,CreatedByName, CardProcessorApi ,CardProcessorApiName ";
        public int? Id { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string Country { get; set; }
        public string CountryName { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public string TransferTypeName { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public string TransferMethodName { get; set; }
        [Required(ErrorMessage = "Enter Processor Name")]
        public string CardProcessorName { get; set; }
        [Required(ErrorMessage = "Enter Contact Person Name")]
        public string ContactName { get; set; }
        [Required(ErrorMessage = "Enter Telephone")]
        public string TelephoneNo { get; set; }
        [Required(ErrorMessage = "Enter Email")]
        public string Email { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedBy { get; set; }
        public string CreatedByName { get; set; }
        public CardProcessorApi CardProcessorApi { get; set; }
        public string CardProcessorApiName { get; set; }
    }
}