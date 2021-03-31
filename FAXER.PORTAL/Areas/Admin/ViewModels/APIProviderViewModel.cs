using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class APIProviderViewModel
    {
        public const string BindProperty = "Id ,Country, CountryFlag , TransferMethod , TransferMethodName , APIProviderName , ContactPerson,Telephone , Email , CreatedBY , CreatedDate";
        public int Id { get; set; }
        [Required(ErrorMessage ="select Country")]
        public string Country{ get; set; }
        public string CountryFlag{ get; set; }
        public TransactionTransferMethod TransferMethod { get; set; }
        public string TransferMethodName { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        public string APIProviderName { get; set; }
        [Required(ErrorMessage = "Enter Name")]
        public string ContactPerson { get; set; }
        [Required(ErrorMessage = "Enter Telephone")]
        public string Telephone { get; set; }
        [Required(ErrorMessage = "Enter Email")]
        public string Email { get; set; }
        public int CreatedBY { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}