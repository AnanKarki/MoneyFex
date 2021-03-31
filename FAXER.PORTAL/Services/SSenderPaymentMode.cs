using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SSenderPaymentMode
    {


        public bool PaymentUsingDebitCreditCard(CreditDebitCardViewModel vm) {


            return true;
        }


        public bool PaymentUsingKiiPayWallet()
        {

            return true;


        }
        /// <summary>
        /// The User will only receive money when the admin verifies the bank account
        /// deposit and confirm payment by themself
        /// </summary>
        /// <returns></returns>
        public bool PaymentUsingMoneyFexBankAccount() {

            return true;
        }
    }


}