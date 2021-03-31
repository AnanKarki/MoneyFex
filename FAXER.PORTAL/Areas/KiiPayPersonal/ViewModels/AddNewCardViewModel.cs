using FAXER.PORTAL.Areas.KiiPayBusiness.ViewModels;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.KiiPayPersonal.ViewModels
{
    public class AddNewCardViewModel
    {
        public const string BindProperty = "Id ,CardNumber ,ExpMonth , ExpYear, SecurityCode, NameOnCard ,CardType";
        public int Id { get; set; }
        [Required(ErrorMessage = "Invalid Card")]
        
        public string CardNumber { get; set; }
        [Required(ErrorMessage = "Invalid Month")]
        
        public int ExpMonth { get; set; }
        [Required(ErrorMessage = "Invalid Year")]
        
        public int ExpYear { get; set; }
        [Required(ErrorMessage = "Invalid SecurityCode")]
        

        public string SecurityCode { get; set; }
        [Required]
        public string NameOnCard { get; set; }
        public CreditDebitCardType CardType { get; set; }

    }
}