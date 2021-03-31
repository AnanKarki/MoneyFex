using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio.Rest.Trunking.V1;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ReceiverController : Controller
    {
        ReceiverServices _services = null;

        CommonServices _CommonServices = null;
        public ReceiverController()
        {
            _services = new ReceiverServices();
            _CommonServices = new CommonServices();
        }
        // GET: Admin/Receiver
        public ActionResult Index(string Country = "", string City = "", string receiverName = "", bool IsFromTransactionStatement = false, int? page = null, int PageSize = 10)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");
            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");
            bool IsBanned = false;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            IPagedList<ViewModels.ReceiverDetailsInfoViewModel> vm = _services.GetRecipients(IsBanned, receiverName, Country).ToPagedList(pageNumber, pageSize);
            ViewBag.ReceiverName = receiverName;
            ViewBag.IsFromTransactionStatement = IsFromTransactionStatement;
            return View(vm);
        }

        public ActionResult ReciverDashboard()
        {
            return View();
        }

        public ActionResult ReceiverTransactionStatement(int ReceiverId = 0, int year = 0, int month = 0, int day = 0, int service = 4,
            string senderName = "", int? page = null)
        {

            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.Service = service;
            ViewBag.Month = month;
            ViewBag.Day = day;
            ViewBag.senderName = senderName;
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<ReceiverTransactionStatement> vm = _services.GetTransactionStatement(year, month, day, service, senderName).ToPagedList(pageNumber, pageSize);
            return View(vm);
        }

        public ActionResult ReceiverNewTransactionStatement(int ReceiverId = 0, int year = 0, string ReceiptNo = "", int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            ViewBag.ReceiverId = ReceiverId;
            ViewBag.ReceiptNo = ReceiptNo;

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (ReceiverId != 0)
            {
                var receiverInfo = _services.receiverInfoByReceiverId(ReceiverId);
                ViewBag.ReceiverName = receiverInfo.ReceiverName;
                ViewBag.ReceiverTelephoneNo = receiverInfo.MobileNo;
                ViewBag.ReceiverCountryName = Common.Common.GetCountryName(receiverInfo.Country);
            }
            var result = _services.GetNewReceiverTransactionStatement(ReceiverId, year, pageSize, pageNumber);


            if (!string.IsNullOrEmpty(ReceiptNo))
            {
                ReceiptNo = ReceiptNo.Trim();
                result.TransactionList = result.TransactionList.Where(x => x.Identifier.ToLower().Contains(ReceiptNo.ToLower())).ToPagedList(pageNumber, pageSize);

            }


            return View(result);
        }
        public ActionResult ReceiverTransactionDetails(int transactionId, Service service)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            ReceiverTransactionDetailsViewModel vm = new ReceiverTransactionDetailsViewModel();
            vm = _services.GetTransactionDetailsOfReceiver(transactionId, service);
            return View(vm);
        }
        public FileContentResult DownloadStatement(int ReceiverId = 0, int year = 0)
        {

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var statementURL = baseUrl + "/EmailTemplate/NewRecipientTransactionStatement/Index?ReceiverId=" + ReceiverId + "&year=" + year;
            var statementPDF = Common.Common.GetPdf(statementURL);
            byte[] bytes = statementPDF.Save();
            string mimeType = "Application/pdf";

            return File(bytes, "application/pdf", DateTime.Now + " " + "MoneyFex Receiver Transaction Statement.pdf");
        }

        [HttpGet]
        public ActionResult Block(int id)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var recepientDetails = _services.receiverInfoByReceiverId(id);
            if (recepientDetails.IsBanned)
            {
                recepientDetails.IsBanned = false;
            }
            else
            {
                _services.AddBlackListedReceiver(recepientDetails);
                recepientDetails.IsBanned = true;
            }
            _services.UpdateRecepient(recepientDetails);
            return RedirectToAction("Index", "Receiver");
        }

        public ActionResult ReceiverDocumentation(string Country = "", string City = "", string ReceiverName = "", string CustomerNo = "", string DocumentName = "",
            string staffUpload = "", string DateRange = "", int? page = null)
        {

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountriesCities();
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            ViewBag.ReceiverName = ReceiverName;
            ViewBag.CustomerNo = CustomerNo;
            ViewBag.DocumentName = DocumentName;
            ViewBag.staffUpload = staffUpload;
            IPagedList<ReceiverDocumentationViewModel> vm = _services.GetReceiverDocumentation(Country, City).ToPagedList(pageNumber, pageSize);
            if (!string.IsNullOrEmpty(DateRange))
            {

                var Date = DateRange.Split('-');
                var FromDate = DateTime.Parse(Date[0]);
                var ToDate = DateTime.Parse(Date[1]);
                vm = vm.Where(x => x.CreatedDate >= FromDate && x.CreatedDate <= ToDate).ToPagedList(pageNumber, pageSize);
            }

            if (!string.IsNullOrEmpty(ReceiverName))
            {
                ReceiverName = ReceiverName.Trim();
                vm = vm.Where(x => x.ReceiverName.ToLower().Contains(ReceiverName.ToLower())).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(CustomerNo))
            {
                CustomerNo = CustomerNo.Trim();
                vm = vm.Where(x => x.ReceiverNumber.Contains(CustomerNo)).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(DocumentName))
            {
                DocumentName = DocumentName.Trim();
                vm = vm.Where(x => x.DocumentName.Contains(DocumentName)).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(staffUpload))
            {
                staffUpload = staffUpload.Trim();
                vm = vm.Where(x => x.CreatedByName.Contains(staffUpload)).ToPagedList(pageNumber, pageSize);

            }

            return View(vm);
        }

        public ActionResult UploadReceiverDocument(int Id = 0, string Country = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            SetViewBagForCountries();
            SetViewBagForCities(Country);
            var recipients = _CommonServices.GetRecipients();
            if (!string.IsNullOrEmpty(Country))
            {
                recipients.Where(x => x.CountryCode == Country).ToList();

            }
            ViewBag.Receivers = new SelectList(recipients, "Id", "Name");
            ReceiverDocumentationViewModel vm = new ReceiverDocumentationViewModel();

            if (Id != 0)
            {
                vm = _services.GetReceiverDocumentatiobyId(Id);
                ViewBag.Receivers = new SelectList(recipients, "Id", "Name", Country);
            }
            if (Country != "")
            {
                vm.Country = Country;

            }
            if (City != "")
            {
                vm.City = City;
            }
            return View(vm);

        }
        [HttpPost]
        public ActionResult UploadReceiverDocument([Bind(Include = ReceiverDocumentationViewModel.BindProperty)] ReceiverDocumentationViewModel vm)
        {

            SetViewBagForCountries();
            SetViewBagForCities(vm.Country);

            var recipients = _CommonServices.GetRecipients();
            if (!string.IsNullOrEmpty(vm.Country))
            {
                recipients.Where(x => x.CountryCode == vm.Country).ToList();

            }
            ViewBag.Receivers = new SelectList(recipients, "Id", "Name");
            if (ModelState.IsValid)
            {
                var CurrentDate = DateTime.Now;

                if (vm.DocumentType == DocumentType.Select)
                {
                    ModelState.AddModelError("DocumentType", "Select Document Type.");
                    return View(vm);
                }
                if (vm.DocumentExpires == DocumentExpires.Select)
                {
                    ModelState.AddModelError("DocumentExpires", "Select Document Expires.");
                    return View(vm);
                }
                if (vm.DocumentExpires == DocumentExpires.Yes && vm.ExpiryDate == null)
                {
                    ModelState.AddModelError("ExpiryDate", "Enter Expiry Date. ");
                    return View(vm);
                }

                if (vm.ExpiryDate < CurrentDate)
                {
                    ModelState.AddModelError("ExpiryDate", "Document has expired. ");
                    return View(vm);
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
                    extension = extension.ToLower();
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

                        vm.DocumentPhotoUrl = "/Documents/" + identificationDocPath;

                    }
                    else
                    {
                        ModelState.AddModelError("DocumentPhotoUrl", "File type not allowed to upload. ");
                        return View(vm);
                    }

                }
                if (vm.Id == 0)
                {
                    _services.UploadDocument(vm);
                }
                else
                {
                    _services.UpdateDocument(vm);
                }
                return RedirectToAction("ReceiverDocumentation", "Receiver");
            }

            return View();
        }
        public ActionResult Delete(int Id)
        {
            _services.Delete(Id);

            return RedirectToAction("ReceiverDocumentation", "Receiver");

        }
        public void SetViewBagForCountriesCities()
        {
            var Countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(Countries, "Code", "Name");

            var Cities = _CommonServices.GetCitiesByName();
            ViewBag.Cities = new SelectList(Cities, "City", "City");

        }
        private void SetViewBagForCountries()
        {
            var countries = _CommonServices.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
        }


        public void SetViewBagForCities(string country = "")
        {
            StaffHolidaysServices Service = new StaffHolidaysServices();
            var cities = Service.GetCities();
            if (country != "")
            {
                cities = Service.GetCitiesFromCountry(country);
            }

            ViewBag.Cities = new SelectList(cities, "City", "City");
        }

        public JsonResult GetReceiverNumber(int receiverId)
        {
            var Accoutno = _CommonServices.GetRecipientsAccountNo(receiverId);


            return Json(new
            {
                Data = Accoutno
            }, JsonRequestBehavior.AllowGet);
        }



    }
}