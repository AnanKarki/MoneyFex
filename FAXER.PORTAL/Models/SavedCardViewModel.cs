using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SavedCardViewModel
    {
        public int Id { get; set; }
        public string Type { get; set; }
        [Required(ErrorMessage ="Enter Num")]
        public string Num { get; set; }
        [Required(ErrorMessage ="Enter Year")]
        public string EYear { get; set; }
        [Required(ErrorMessage ="Enter Month") ]
        public string EMonth { get; set; }
        public string Remark { get; set; }
        [Required(ErrorMessage ="Enter Client Code") ]
        public string ClientCode { get; set; }
        [Required(ErrorMessage ="Enter Card Name")]
        public string CardName { get; set; }
        public bool IsDeleted { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string UserID { get; set; }
        [Required(ErrorMessage ="Enter Address Line one")]
        public string AddressLineOne { get; set; }
        [Required(ErrorMessage ="Enter City")]
        public string City { get; set; }
        [Required(ErrorMessage ="Enter ZipCode")]
        public string ZipCode { get; set; }
        [Required(ErrorMessage ="Select Country")]
        public string County { get; set; }
        public string AddressLineTwo { get; set; }
        public bool Confirmation { get; set; }
    }
}