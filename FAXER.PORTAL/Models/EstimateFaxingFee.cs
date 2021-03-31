using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Models
{

    public class EstimateFaxingFee
    {
        public const string BindProperty = "Faxing,Receiving";


        [Required(ErrorMessage = "Select Faxing Country")]
        [Display(Name = "Select Faxing Country")]
        public string Faxing { get; set; }

        [Required(ErrorMessage = "Select Receiving Country")]
        [Display(Name = "Select Receiving Country")]
        public string Receiving { get; set; }



    }
}