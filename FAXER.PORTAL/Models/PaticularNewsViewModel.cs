using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Models
{
    public class PaticularNewsViewModel
    {
        public int Id { get; set; }
        public string Headline { get; set; }
        [AllowHtml]
        public string FullNews { get; set; }
        public string NewsImage { get; set; }
        public int PublishedBy { get; set; }
        public string PublishedDate { get; set; }
        public string PublishedTime { get; set; }
        public int LastModifiedBy { get; set; }
        public string LastModifiedDate { get; set; }
        public string LastModifiedTime { get; set; }
        public List<ShowRecentNews> ShowRecentNews { get; set; }

    }
    public class ShowRecentNews
    {
        public int Id { get; set; }
        public string Headline { get; set; }
    }
}