using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentUploadDocumentsController : Controller
    {
        SUploadNewDocmentServices _uploadNewDocument = null;

        public AgentUploadDocumentsController()
        {
            _uploadNewDocument = new SUploadNewDocmentServices();
        }
        // GET: Agent/AgentUploadDocuments
        public ActionResult Index()
        {
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            int agentId = agentInfo.Id;
            if (agentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
            int AgentStaffId = Common.AgentSession.AgentStaffLogin.AgentStaffId;
            List<AgentNewDocumentViewModel> model = _uploadNewDocument.GetAgentDocuments(AgentStaffId).Data;

            return View(model);
        }
        [HttpGet]
        public ActionResult UploadNewDocument()
        {
            AgentResult agentResult = new AgentResult();
            AgentNewDocument vm = new AgentNewDocument();
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }

        [HttpPost]
        public ActionResult UploadNewDocument(AgentNewDocument model)
        {
            AgentResult agentResult = new AgentResult();
            if (ModelState.IsValid)
            {
                var CurrentDate = DateTime.Now;
                if (model.DocumentExpires == DocumentExpires.Yes && model.ExpiryDate == null)
                {

                    //agentResult.Message = "Enter Expiry Date.";

                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("", "Enter Expiry Date. ");
                    ViewBag.AgentResult = agentResult;
                    return View(model);

                }

                if (model.ExpiryDate < CurrentDate)
                {
                    //agentResult.Message = "Document has expired.";
                    //agentResult.Status = ResultStatus.Warning;

                    ModelState.AddModelError("", "Document has expired. ");

                    ViewBag.AgentResult = agentResult;
                    return View(model);
                }

                if (Request.Files.Count < 1)
                {
                    var identificationdoc = Request.Files["DocumentPhotoUrl"];

                }
                string identificationDocPath = "";
                var IdentificationDoc = Request.Files["DocumentPhotoUrl"];
                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    var allowedExtensions = new string[] { "gif", "jpg", "png", "jpeg", "pdf" };
                    var extension = IdentificationDoc.FileName.Split('.')[1];
                    identificationDocPath = Guid.NewGuid() + "." + IdentificationDoc.FileName.Split('.')[1];


                    if (allowedExtensions.Contains(extension))
                    {

                        try
                        {


                            IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + identificationDocPath);
                        }
                        catch (Exception ex)
                        {


                        }

                        model.DocumentPhotoUrl = "/Documents/" + identificationDocPath;

                    }
                    else
                    {
                        agentResult.Message = "File type not allowed to upload.";
                        agentResult.Status = ResultStatus.Warning;
                        ViewBag.AgentResult = agentResult;
                        return View(model);
                    }

                }
                _uploadNewDocument.CreateNewDocument(model);

                return RedirectToAction("Index");

            }
            ViewBag.AgentResult = agentResult;
            return View(model);
        }
        [HttpGet]
        public ActionResult UpdateUploadDocument(int id)
        {
            AgentResult agentResult = new AgentResult();
            AgentNewDocument vm = _uploadNewDocument.GetAgentDocumentById(id);
            ViewBag.AgentResult = agentResult;
            return View(vm);

        }
        [HttpPost]
        public ActionResult UpdateUploadDocument(AgentNewDocument model)
        {
            AgentResult agentResult = new AgentResult();
            if (ModelState.IsValid)
            {
                var CurrentDate = DateTime.Now;
                if (model.DocumentExpires == DocumentExpires.Yes && model.ExpiryDate == null)
                {
                    //agentResult.Message = "Enter Expiry Date.";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("", "Enter Expiry Date. ");
                    ViewBag.AgentResult = agentResult;
                    return View(model);
                }
                if (model.ExpiryDate < CurrentDate)
                {
                    //agentResult.Message = "Document has expired.";
                    //agentResult.Status = ResultStatus.Warning;
                    ModelState.AddModelError("", "Document has expired. ");
                    ViewBag.AgentResult = agentResult;
                    return View(model);
                }
                if (Request.Files.Count < 1)
                {
                    var identificationdoc = Request.Files["DocumentPhotoUrl"];

                }
                string identificationDocPath = "";
                var IdentificationDoc = Request.Files["DocumentPhotoUrl"];
                if (IdentificationDoc != null && IdentificationDoc.ContentLength > 0)
                {
                    var allowedExtensions = new string[] { "gif", "jpg", "png", "jpeg", "pdf" };
                    var extension = IdentificationDoc.FileName.Split('.')[1];
                    identificationDocPath = Guid.NewGuid() + "." + IdentificationDoc.FileName.Split('.')[1];


                    if (allowedExtensions.Contains(extension))
                    {

                        IdentificationDoc.SaveAs(Server.MapPath("~/Documents") + "\\" + identificationDocPath);
                        model.DocumentPhotoUrl = "/Documents/" + identificationDocPath;

                    }
                    else
                    {
                        agentResult.Message = "File type not allowed to upload.";
                        agentResult.Status = ResultStatus.Warning;
                        ViewBag.AgentResult = agentResult;
                        return View(model);
                    }

                }
                _uploadNewDocument.UpdateNewDocument(model);

                return RedirectToAction("Index");

            }
            ViewBag.AgentResult = agentResult;
            return View(model);
        }

    }
}