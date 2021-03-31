using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers.Dashboard.LearnMore
{
    public class NewsHomeController : Controller
    {
        SNewsHomeServices Service = new SNewsHomeServices();
        // GET: NewsHome
        [HttpGet]
        public ActionResult Index(string search = "", int page = 0)
        {


            var vm = Service.getNewsList();
            if (!string.IsNullOrEmpty(search))
            {
                vm = Service.getNewsList(search);
            }
            var count = vm.Count();
            int PageSize = 6;
            var data = vm.Skip(page * PageSize).Take(PageSize).ToList();
            ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);
            ViewBag.Page = page;
            ViewBag.startCount = count == 0 ? page * PageSize : page * PageSize + 1;
            ViewBag.EndCount = page * PageSize + data.Count();
            ViewBag.TotalCount = vm.Count();

            return View(data);
        }
        [HttpGet]
        public ActionResult NewsDetails(int id = 0)
        {
            var vm = Service.getNews(id);
            return View(vm);
        }

        public ActionResult HomePage()
        {

            return View();
        }
    }
}