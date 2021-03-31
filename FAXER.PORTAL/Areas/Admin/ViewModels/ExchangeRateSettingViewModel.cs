using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class ExchangeRateSettingViewModel
    {
        public const string BindProperty = "Id , SourceCountryCode,SourceCountryName ,SourceCountryNameLower,SourceCurrencyCode ,SourceCurrencySymbol ,DestinationCountryCode,DestinationCountryName,DestinationCountryNameLower , DestinationCurrencyCode,DestinationCurrencySymbol" +
            " , ExchangeRate ,CreatedDate ,CreatedBy , TransferType,TransferMethod ,TransferMethodName , AgentId,Rate ,KiiPayWallet , CashPickUp, ServicePayment ,OtherWallet , BankDeposit, BillPayment , AgentName, Range, AgentAccountNO,TransferFeeByCurrencyId";
        public ExchangeRateSettingViewModel()
        {
            SourceCountryCode = "";
            SourceCountryName = "";
            SourceCurrencyCode = "";
            SourceCurrencySymbol = "";
            DestinationCountryCode = "";
            DestinationCountryName = "";
            DestinationCurrencyCode = "";
            DestinationCurrencySymbol = "";
        }
        public int Id { get; set; }
        [Required(ErrorMessage = "Select Country")]

        public string SourceCountryCode { get; set; }
        public string SourceCountryName { get; set; }
        public string SourceCountryNameLower { get; set; }
        public string SourceCurrencyCode { get; set; }
        public string SourceCurrencySymbol { get; set; }
        [Required(ErrorMessage = "Select Country")]
        public string DestinationCountryCode { get; set; }
        public string DestinationCountryName { get; set; }
        public string DestinationCountryNameLower { get; set; }
        public string DestinationCurrencyCode { get; set; }
        public string DestinationCurrencySymbol { get; set; }
        public decimal ExchangeRate { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public TransactionTransferType TransferType { get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public string TransferMethodName { get; set; }
        public int? AgentId { get; set; }
        public decimal Rate { get; set; }
        public decimal KiiPayWallet { get; set; }
        public decimal CashPickUp { get; set; }
        public decimal ServicePayment { get; set; }
        public decimal OtherWallet { get; set; }
        public decimal BankDeposit { get; set; }
        public decimal BillPayment { get; set; }
        public string AgentName { get; set; }
        public string AgentAccountNO { get; set; }
        public string Range { get; set; }
        public decimal FromRange { get; set; }
        public decimal ToRange { get; set; }
        public int TransferFeeByCurrencyId { get; set; }
        public List<string> RangeList { get; set; }


    }
}