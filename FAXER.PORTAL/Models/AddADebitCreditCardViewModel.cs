using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class AddADebitCreditCardViewModel
    {

        public const string BindProperty = "Type,Num,EYear,EMonth,Remark,ClientCode,CardName,CreatedDate";
        [Required(ErrorMessage ="Enter Type") ]
        public string Type { get; set; }
        public string Num { get; set; }
        public string EYear { get; set; }
        public string EMonth { get; set; }
        public string Remark { get; set; }
        public string ClientCode { get; set; }
        public string CardName { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }
}