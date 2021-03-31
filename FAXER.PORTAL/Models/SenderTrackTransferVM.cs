using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class SenderTrackTransferVM
    {
        [Required(ErrorMessage ="Enter Sender Surname") ]
        public string SenderSurname { get; set; }
        [Required(ErrorMessage ="Enter MFCN")]
        public string MFCN { get; set; }

    }
}