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
    using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
    using FAXER.PORTAL.Models;
    using System;
    using System.Collections.Generic;
    
    public partial class SavedCard
    {
        public int Id { get; set; }
        public CreditDebitCardType Type { get; set; }
        public string Num { get; set; }
        public string EYear { get; set; }
        public string EMonth { get; set; }
        public string Remark { get; set; }
        public string ClientCode { get; set; }
        public string CardName { get; set; }
        public bool IsDeleted { get; set; }
        public int UserId { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Module Module { get; set; }
    }
}