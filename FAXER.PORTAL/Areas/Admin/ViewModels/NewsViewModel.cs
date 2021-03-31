using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.ViewModels
{
    public class NewsViewModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string Headline { get; set; }
        public string FullNews { get; set; }

        public string PhotoURL { get; set; }
        
    }
   
    public class MasterNews {

        public IPagedList<NewsViewModel> NewsViewModel { get; set; }

        public int Title { get; set; }
        public string FullBody { get; set; }
        public string ImageUrl { get; set; }
    }
}