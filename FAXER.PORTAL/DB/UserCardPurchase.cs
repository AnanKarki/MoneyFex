//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FAXER.PORTAL.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserCardPurchase
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public decimal TransactionAmount { get; set; }
        public System.DateTime TransactionDate { get; set; }
        public int KiiPayBusinessInformationId { get; set; }
        public virtual KiiPayPersonalWalletInformation Card { get; set; }
        public virtual KiiPayBusinessInformation Business { get; set; }
    }
}
