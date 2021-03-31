using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Services
{
    public class SNewsHomeServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        public List<NewsHomeViewModel> getNewsList(string searchText = "")
        {
            
            var data = dbContext.News.Where(x => x.IsDeleted == false).OrderByDescending(x => x.PublishedDateTime).ToList();
            if (!string.IsNullOrEmpty(searchText))
            {
                data = dbContext.News.Where(x => x.IsDeleted == false && x.Headline.ToLower().Contains(searchText.ToLower())).OrderByDescending(x => x.PublishedDateTime).ToList();
            }
            var result = (from c in data
                          select new NewsHomeViewModel()
                          {
                              Id = c.Id,
                              Headline = c.Headline,
                              FullBody = c.FullNews.Length > 95 ? c.FullNews.Substring(0, 95) + "..." : c.FullNews+"...",
                              ImageUrl = c.NewsImage,
                              PublishedBy = c.PublishedBy
                          }).ToList();
            return result;
        }

        public PaticularNewsViewModel getNews(int id)
        {
            var recent = (from c in dbContext.News.Where(x => x.IsDeleted == false).OrderByDescending(x => x.PublishedDateTime).Take(3).ToList()
                          select new ShowRecentNews()
                          {
                              Id = c.Id,
                              Headline = c.Headline
                          }).ToList();

            var result = (from c in dbContext.News.Where(x => x.Id == id).OrderByDescending(x => x.PublishedDateTime).ToList()
                          select new PaticularNewsViewModel()
                          {
                              Id = c.Id,
                              Headline = c.Headline,
                              FullNews = c.FullNews,
                              NewsImage = c.NewsImage,
                              PublishedBy = c.PublishedBy,
                              PublishedDate = c.PublishedDateTime.ToFormatedString(),
                              PublishedTime = c.PublishedDateTime.ToString("HH:mm"),
                              LastModifiedBy = c.LastModifiedBy,
                              ShowRecentNews = recent
                          }).FirstOrDefault();
            return result;
        }
    }
}