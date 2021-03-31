using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ComplianceDocumentController : Controller
    {

        // GET: Admin/ComplianceDocument
        FAXER.PORTAL.Areas.Admin.Services.CommonServices _commonSerivces = null;
        ComplianceDocumentServices _complianceDocumentServices = null;
        public ComplianceDocumentController()
        {
            _commonSerivces = new Services.CommonServices();
            _complianceDocumentServices = new ComplianceDocumentServices();
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetCountryFolder(int senderType = 0)
        {
            var iTreeViewData = _complianceDocumentServices.GetCountryFolder(senderType);
            return Json(new ServiceResult<List<ItemGroupKendoTreeViewModel>>()
            {
                Data = iTreeViewData.ToList(),
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public JsonResult GetUserByCountryId(int countryId, int senderType = 0)
        {
            var data = _complianceDocumentServices.GetSendersDocumentByCountryId(countryId, senderType);
            IEnumerable<ItemGroupKendoTreeViewModel> iTreeViewData = (from c in data.ToList()
                                                                      select new ItemGroupKendoTreeViewModel()
                                                                      {
                                                                          id = c.Id,
                                                                          text = c.SenderName,
                                                                          ImageUrl = c.Url
                                                                      }).ToList();
            if (iTreeViewData.Count() > 0)
            {
                iTreeViewData = _complianceDocumentServices.BuildTree(iTreeViewData);
            }
            return Json(new ServiceResult<List<ItemGroupKendoTreeViewModel>>()
            {
                Data = iTreeViewData.ToList(),
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetFilesBySenderDocumentationId(int senderDoumentationId)
        {

            var data = _commonSerivces.GetSenderBusinessDocumentations().Where(x => x.Id == senderDoumentationId).ToList();
            var senderInfo = _commonSerivces.GetAllSenderInfo();
            IEnumerable<FileViewModel> iTreeViewData = (from c in data
                                                        join d in senderInfo on c.SenderId equals d.Id
                                                        select new FileViewModel()
                                                        {
                                                            Id = c.Id,
                                                            Name = c.DocumentName,
                                                            SenderName = d.FirstName + " " + (string.IsNullOrEmpty(d.MiddleName) == true ? "" : d.MiddleName + " ") + d.LastName,
                                                            Url = c.DocumentPhotoUrl,
                                                            FileExtension = c.DocumentPhotoUrl.Split('.')[1]
                                                        }).ToList();

            return Json(new ServiceResult<List<FileViewModel>>()
            {
                Data = iTreeViewData.ToList(),
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);

        }
        [HttpGet]
        public JsonResult GetFilesBySenderId(int senderId)
        {

            var data = _commonSerivces.GetSenderBusinessDocumentations().Where(x => x.SenderId == senderId).ToList();
            IEnumerable<FileViewModel> iTreeViewData = (from c in data

                                                        select new FileViewModel()
                                                        {
                                                            Id = c.Id,
                                                            Name = c.DocumentName,
                                                            Url = c.DocumentPhotoUrl,
                                                            FileExtension = c.DocumentPhotoUrl.Split('.')[1]
                                                        }).ToList();

            return Json(new ServiceResult<List<FileViewModel>>()
            {
                Data = iTreeViewData.ToList(),
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);

        }

        public JsonResult GetFilesByFolder(int CountryId)
        {
            var foldersAndFiles = _complianceDocumentServices.GetFilesByFolder(CountryId);

            return Json(new ServiceResult<List<FileViewModel>>()
            {
                Data = foldersAndFiles.ToList(),
                Message = "",
                Status = ResultStatus.OK
            }, JsonRequestBehavior.AllowGet);



        }
    }
}