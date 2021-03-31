using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class LogosUploadViewModel
    {
        public const string BindProperty = "Id , Country ,Service ,Title ,WebstieUrl ,Logo";

        public int Id { get; set; }
        [Required(ErrorMessage ="Select Country")]
        public string Country { get; set; }
        public TransactionTransferMethod Service { get; set; }
        [Required(ErrorMessage = "Enter Title")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Enter WebstieUrl")]
        public string WebstieUrl { get; set; }
        [Required(ErrorMessage = "Select Logo")]
        public string Logo { get; set; }
    }
}