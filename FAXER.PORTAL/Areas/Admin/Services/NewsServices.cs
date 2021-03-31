using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class NewsServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        Services.CommonServices CommonService = new Services.CommonServices();

        public List<NewsViewModel> getNewsList()
        {
            var result = (from c in dbContext.News.Where(x => x.IsDeleted == false).OrderByDescending(x=>x.PublishedDateTime).ToList()
                           select new NewsViewModel()
                           {
                               Id = c.Id,
                               Date = c.PublishedDateTime.ToFormatedString(),
                               Time = c.PublishedDateTime.ToString("HH:mm"),
                               Headline = c.Headline
                           }).ToList();

            return result;
        }

        public bool saveNews(AddNewNewsViewModel model)
        {
            if (model != null)
            {
                News data = new News()
                {
                    Headline = model.Headline,
                    FullNews = model.FullNews,
                    NewsImage = model.Image,
                    PublishedBy = Common.StaffSession.LoggedStaff.StaffId,
                    PublishedDateTime = DateTime.Now
                };
                dbContext.News.Add(data);
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public AddNewNewsViewModel getEditInfo(int id)
        {
            if (id!=0)
            {
                var data = (from c in dbContext.News.Where(x => x.Id == id && x.IsDeleted == false)
                            select new AddNewNewsViewModel()
                            {
                                Id = c.Id,
                                Headline = c.Headline,
                                FullNews = c.FullNews,
                                Image = c.NewsImage,
                                OldImagePath = c.NewsImage
                            }).FirstOrDefault();
                return data;
            }
            return null;
        }

        public bool saveEditedNews(AddNewNewsViewModel model)
        {
            if (model!= null)
            {
                var data = dbContext.News.Where(x => x.Id == model.Id).FirstOrDefault();
                if (data != null)
                {
                    data.Headline = model.Headline;
                    data.FullNews = model.FullNews;
                    if(!string.IsNullOrEmpty(model.Image))
                    {
                        data.NewsImage = model.Image;
                    }
                    
                    data.LastModifiedBy = Common.StaffSession.LoggedStaff.StaffId;
                    data.LastModifiedDate = DateTime.Now;
                }
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool deleteNews(int id)
        {
            if (id!=0)
            {
                var data = dbContext.News.Where(x => x.Id == id).FirstOrDefault();
                if(data != null)
                {
                    data.IsDeleted = true;
                    data.DeletedBy = Common.StaffSession.LoggedStaff.StaffId;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();
                    return true; 
                }
            }
            return false;
        }
    }
}