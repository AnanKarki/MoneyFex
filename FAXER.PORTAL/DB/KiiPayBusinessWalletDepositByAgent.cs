using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class KiiPayBusinessWalletDepositByAgent
    {
        public int Id { get; set; }
        #region Sender Details  ( MoneyFex Sender )

        public int SenderId { get; set; }
        #endregion

        #region Receiver Details (KiiPay Business Info )


        public int KiiPayBusinessInformationId { get; set; }


        /// <summary>
        /// KiiPay Business wallet Id (F.k)
        /// </summary>
        public int KiiPayBusinessWalletInformationId { get; set; }

        #endregion

        #region Sender Identification Details 
        public int IDCardType { get; set; }
        public string IDCardNumber { get; set; }
        public DateTime IDCardExpiryDate { get; set; }
        public string IDCardIssuingCountry { get; set; }
        #endregion

        #region Transaction Details 

        public decimal DepositAmount { get; set; }
        public decimal DepositFees { get; set; }
        public decimal ExchangeRate { get; set; }
        public decimal ReceivingAmount { get; set; }
        public string PaymentReference { get; set; }
        public string ReceiptNumber { get; set; }

        public DateTime TransactionDate { get; set; }
        #endregion

        #region Paying Agent Information 

        public string PayingStaffName { get; set; }
        public int AgentId { get; set; }
        public int PayingAgentStaffId { get; set; }

        #endregion
        public virtual FaxerInformation Sender { get; set; }
        public virtual AgentInformation Agent { get; set; }
        public virtual KiiPayBusinessWalletInformation KiiPayBusinessWalletInformation { get; set; }


    }
}