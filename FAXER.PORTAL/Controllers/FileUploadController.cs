using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class FileUploadController : Controller
    {
        // GET: FileUpload
        [HttpPost]
        public JsonResult Index()
        {
            //TODO: Delete Raw images and files before save scan file before save for antivirus security
            var supportedTypes = new[] { "doc", "docx", "xls", "xlsx", "jpg", "png", "zip", "rar", "csv", "pdf", "mp4" };

            var File = Request.Files[0];
            var fileExt = System.IO.Path.GetExtension(File.FileName).Substring(1);
            if (supportedTypes.Contains(fileExt.ToLower()))
            {
                var id = Guid.NewGuid();
                var fileSpec = @"/Documents/" + id + "." + File.FileName.Split('.')[1];
                string filePath = Server.MapPath(@"/Documents/") + id + "." + fileExt;
                File.SaveAs(filePath);
                return new JsonResult() { Data = fileSpec };
            }
            return new JsonResult() { Data = null };

        }

    }
}