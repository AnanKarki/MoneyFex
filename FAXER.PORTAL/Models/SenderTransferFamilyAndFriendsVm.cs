using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderTransferFamilyAndFriendsVm
    {
        public int Id { get; set; }

        [Required(ErrorMessage ="Select Wallet")]
        public int WalletId { get; set; }
    }
}