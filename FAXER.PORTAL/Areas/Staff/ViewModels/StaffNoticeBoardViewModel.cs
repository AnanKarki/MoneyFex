using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.ViewModels
{
    public class StaffNoticeBoardViewModel
    {
        public int NoticeboardId { get; set; }
        public string NoticeTitle { get; set; }
        public string NoticeDescription { get; set; }
        public DateTime NoticeDate { get; set; }
    }
}