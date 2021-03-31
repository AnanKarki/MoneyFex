using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Staff.Services
{
    public class StaffNoticeboardServices
    {
        FAXER.PORTAL.DB.FAXEREntities dbContext = null;
        public StaffNoticeboardServices() {

            dbContext = new DB.FAXEREntities(); 
        }

        public List<ViewModels.StaffNoticeBoardViewModel> GetNoticeList() {
            int staffId = Common.StaffSession.LoggedStaff.StaffId;
            var staffInformation = dbContext.StaffInformation.Where(x => x.Id == staffId).FirstOrDefault();
            var result = (from c in dbContext.StaffNotice.Where( x =>((x.StaffId == staffId 
                          || x.StaffId == 0) && x.IsDeleted == false) &&
                         (x.Country == staffInformation.Country || x.Country == null) &&
                         (x.City == staffInformation.City || x.City == null)).ToList()
                          select new ViewModels.StaffNoticeBoardViewModel()
                          {

                              NoticeTitle = c.Headline,
                              NoticeDescription = c.FullNotice,
                              NoticeDate = c.Date
                          }).ToList();

            return result;
        }
    }
}