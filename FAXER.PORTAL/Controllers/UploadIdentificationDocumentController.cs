using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Controllers
{
    public class UploadIdentificationDocumentController : Controller
    {
        FAXER.PORTAL.DB.FAXEREntities dbContext = null;
        
        public UploadIdentificationDocumentController()
        {

            dbContext = new DB.FAXEREntities();

        }
        // GET: UploadIdentificationDocument
        public ActionResult Index(string ToURL)
        {

            Common.FaxerSession.ToUrl = ToURL;

            return View();
        }

        [HttpPost]
        public ActionResult Index()
        {

            var fileName = "";

            if (Request.Files.Count > 0)
            {
                //check model validation
                if (ModelState.IsValid)
                {
                    var upload = Request.Files[0];
                    if (upload == null)
                    {
                        ModelState.AddModelError("File", "Please Upload Identity Document");
                        return View();
                    }

                    string directory = Server.MapPath("/Documents");
                    fileName = "";
                    if (upload != null && upload.ContentLength > 0)
                    {
                        fileName = Guid.NewGuid() + "." + upload.FileName.Split('.')[1]; //Path.GetFileName(upload.FileName);
                        upload.SaveAs(Path.Combine(directory, fileName));
                    }

                    string CardUrl = "/Documents/" + fileName;

                    var data = dbContext.FaxerInformation.Where(x => x.Id == Common.FaxerSession.LoggedUser.Id).FirstOrDefault();
                    data.CardUrl = CardUrl;
                    dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                    dbContext.SaveChanges();


                    return Redirect(Common.FaxerSession.ToUrl);


                }
            }
            return View();
        }
    }
}