using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using PagedList;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class NewsController : Controller
    {
        NewsServices Service = new NewsServices();
        CommonServices CommonService = new CommonServices();
        // GET: Admin/News
        public ActionResult Index(string message = "", string Date = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if(message == "success")
            {
                ViewBag.Message = "News Published Successfully !";
                message = "";
            }
            else if (message == "successEdit")
            {
                ViewBag.Message = "News Updated Successfully !";
                message = "";
            }
            else if (message == "deleteSuccess")
            {
                ViewBag.Message = "Message Deleted Successfully !";
                message = "";
            }
            else if (message == "deleteFail")
            {
                ViewBag.Message = "Message Delete Failed. Please try again !";
                message = "";
            }
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            var model = new MasterNews();
            IPagedList<NewsViewModel> vm = Service.getNewsList().ToPagedList(pageNumber,pageSize);
            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                vm = vm.Where(x => Convert.ToDateTime(x.Date)>= FromDate && Convert.ToDateTime(x.Date) <= ToDate).ToPagedList(pageNumber, pageSize);
            }
            
            model.NewsViewModel = vm;
            return View(model);
        }

        [HttpGet]
        public ActionResult AddNewNews(int id=0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            if (id != 0)
            {
                var vm = Service.getEditInfo(id);
                return View(vm);
            }
            return View();
        }

        [HttpPost]
        public ActionResult AddNewNews([Bind(Include = AddNewNewsViewModel.BindProperty)]AddNewNewsViewModel model)
        {
            if (model != null)
            {
                bool valid = true;
                if (string.IsNullOrEmpty(model.Headline))
                {
                    ModelState.AddModelError("Headline", "This field can't be blank !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.FullNews))
                {
                    ModelState.AddModelError("FullNews", "This field can't be blank !");
                    valid = false;
                }


                if (valid == true)
                {
                    
                    if (Request.Files.Count >0)
                    {
                        string fileName = "";
                        string directory = Server.MapPath("/Documents");
                        var newsImage = Request.Files["newsImage"];

                        if (newsImage != null && newsImage.ContentLength >0)
                        {
                            fileName = Guid.NewGuid() + "." + newsImage.FileName.Split('.')[1];
                            newsImage.SaveAs(Path.Combine(directory, fileName));
                            model.Image = "/Documents/" + fileName;
                        }
                        
                    }
                    if(model.Id>0)
                    {
                        bool result = Service.saveEditedNews(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "successEdit" });
                        }
                    }
                    else
                    {
                        bool result = Service.saveNews(model);
                        if (result)
                        {
                            return RedirectToAction("Index", new { @message = "success" });
                        }
                    }
                    
                }
            }
            return View(model);
        }

     
        public ActionResult DeleteNews(int id)
        {
            if (id != 0)
            {
                bool result = Service.deleteNews(id);
                if (result)
                {
                    return RedirectToAction("Index", new { @message = "deleteSuccess" });
                }
            }
            return RedirectToAction("Index", new { @message="deleteFail"});
        }

        public ActionResult showNews(int id)
        {
            var data = Service.getEditInfo(id);
            return Json(new
            {
                Title = data.Headline,
                FullBody = data.FullNews,
                ImageUrl = data.Image,
                Id=data.Id,
            }, JsonRequestBehavior.AllowGet);
        }


    }
}