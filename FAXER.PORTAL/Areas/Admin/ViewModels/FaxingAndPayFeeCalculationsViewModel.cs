using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class FaxingAndPayFeeCalculationsViewModel
    {
        public const string BindProperty = "Id , FaxingCountry ,ReceivingCountry , FaxingAmount, FaxingCurrency, FaxingCurrencySymbol,ReceivingCurrency ,ReceivingCurrencySymbol , FaxingFee" +
            ", TotalAmount,CurExchangeRate ,AmountToBeReceived , IncludeFaxingFee,TypeOfUser";

        public int Id { get; set; }
        public string FaxingCountry { get; set; }
        public string ReceivingCountry { get; set; }
        public decimal FaxingAmount { get; set; }
        public string FaxingCurrency { get; set; }
        public string FaxingCurrencySymbol { get; set; }
        public string ReceivingCurrency { get; set; }
        public string ReceivingCurrencySymbol { get; set; }
        public decimal FaxingFee { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CurExchangeRate { get; set; }
        public decimal AmountToBeReceived { get; set; }

        public bool IncludeFaxingFee { get; set; }

        public TypeOfUser TypeOfUser { get; set; }

    }

    public enum TypeOfUser
    {
        CardUser ,

        NonCardUser,

    }

}