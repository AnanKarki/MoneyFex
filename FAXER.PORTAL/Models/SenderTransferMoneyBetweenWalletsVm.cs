using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderTransferMoneyBetweenWalletsVm
    {

        public const string BindProperty = "Id,AvailableBalance,TransferringWalletId,ReceivingWalletId,Amount,Currency";
        [Range(0 , int.MaxValue)]
        public int Id { get; set; }

        [Range(0, int.MaxValue)]
        public decimal AvailableBalance { get; set; }

        [Range(0, int.MaxValue)]
        public int TransferringWalletId { get; set; }

        [Range(0, int.MaxValue)]
        public int ReceivingWalletId { get; set; }

        [Range(0, int.MaxValue)]
        public decimal Amount { get; set; }
        [MaxLength(200)]
        public string Currency { get; set; }
    }
}