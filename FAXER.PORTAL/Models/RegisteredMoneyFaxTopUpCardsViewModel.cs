using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    /// <summary>
    /// Registered Virtual Accounts ViewModel
    /// </summary>
    public class RegisteredMoneyFaxTopUpCardsViewModel
    {
        public int Id { get; set; }
        public string MoneyFaxTopUpCard { get; set; }
        public string CardUsersName { get; set; }
    }
}