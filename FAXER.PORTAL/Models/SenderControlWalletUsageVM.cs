using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderControlWalletUsageVM
    {

        public const string BindProperty = "Id, WalletId ,LimitTypeId,CurrencySymbol, Balance ";

        [Range(0 , int.MaxValue)]
        public int Id { get; set; }

        [Range(0, int.MaxValue)]
        public int WalletId { get; set; }

        [Range(0, int.MaxValue)]
        public int LimitTypeId { get; set; }
        [MaxLength(200)]
        public string CurrencySymbol { get; set; }

        [Range(0.0, double.MaxValue)]
        public decimal Balance { get; set; }
    }
}