using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Models
{
    public class NewsHomeViewModel
    {
        public int Id { get; set; }
        public string Headline { get; set; }
        public string FullBody { get; set; }
        public string ImageUrl { get; set; }
        public int PublishedBy { get; set; }

    }
}