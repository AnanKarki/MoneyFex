using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.DB
{
    public class News
    {
        public int Id { get; set; }
        public string Headline { get; set; }
        [AllowHtml]
        public string FullNews { get; set; }
        public string NewsImage { get; set; }
        public int PublishedBy { get; set; }
        public DateTime PublishedDateTime { get; set; }
        public int LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
        public int DeletedBy { get; set; }
    }
}