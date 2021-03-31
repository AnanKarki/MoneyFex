using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class SocialMedaiTrackingServices
    {
        DB.FAXEREntities db = null;

        public SocialMedaiTrackingServices()
        {
            db = new DB.FAXEREntities();
        }

        public bool Add(SocialMediaTrackingViewModel vm)
        {
            var staffId = Common.StaffSession.LoggedStaff.StaffId;
            SocialMediaTracking model = new SocialMediaTracking()
            {
                ApplicationType = vm.ApplicationType,
                Services = vm.Services,
                TrackingCode = vm.TrackingCode,
                TrackingPage = vm.TrackingPage,
                CreatedBy = staffId,
                CreatedDate = DateTime.Now,

            };

            db.SocialMediaTracking.Add(model);
            db.SaveChanges();
            return true;
        }

        public List<SocialMediaTrackingViewModel> GetTrackingPageInfo(string Services = "", int ApplicationType = 0, string TrackingPage = "")
        {
            var data = db.SocialMediaTracking.ToList();

            if (!string.IsNullOrEmpty(Services))
            {
                data = data.Where(x => x.Services == Services).ToList();
            }
            if (ApplicationType != 0)
            {
                data = data.Where(x => x.ApplicationType == (ApplicationType)ApplicationType).ToList();
            }
            if (!string.IsNullOrEmpty(TrackingPage))
            {
                data = data.Where(x => x.TrackingPage == TrackingPage).ToList();
            }

            var result = (from c in data.ToList()
                          select new SocialMediaTrackingViewModel()
                          {
                              Id = c.Id,
                              ApplicationType = c.ApplicationType,
                              CreatedBy = c.CreatedBy,
                              Services = c.Services,
                              CreatedDate = c.CreatedDate,
                              TrackingCode = c.TrackingCode,
                              TrackingPage = c.TrackingPage
                          }).ToList();
            return result;

        }

        public bool Remove(int id)
        {
            var data = db.SocialMediaTracking.Where(x => x.Id == id).FirstOrDefault();
            db.SocialMediaTracking.Remove(data);
            db.SaveChanges();
            return true;
        }

        public bool Update(SocialMediaTrackingViewModel vm)
        {
            var model = db.SocialMediaTracking.Where(x => x.Id == vm.Id).FirstOrDefault();
            model.Services = vm.Services;
            model.TrackingCode = vm.TrackingCode;
            model.TrackingPage = vm.TrackingPage;
            model.ApplicationType = vm.ApplicationType;
            db.Entry<SocialMediaTracking>(model).State = EntityState.Modified;
            db.SaveChanges();
            return true;
        }

        public IQueryable<SocialMediaTracking> GetSocialMediaTrackings()
        {

            return db.SocialMediaTracking;
        }
    }
}