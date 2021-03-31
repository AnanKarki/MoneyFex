using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using Microsoft.Ajax.Utilities;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PagedList;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class ViewRegisteredFaxersController : Controller
    {
        FAXEREntities dbContext = new FAXEREntities();
        Services.ViewRegisteredFaxersServices faxer = new Services.ViewRegisteredFaxersServices();
        Services.CommonServices CommonService = new Services.CommonServices();


        // GET: Admin/ViewRegisteredFaxers
        [HttpGet]

        public ActionResult Index(string CountryCode = "", string City = "",
            string message = "",
            string SenderName = "", string AccountNo = "",
            string Address = "", string TelphoneNo = "", string Email = "",
            bool IsFormTransactionStatement = false, int? page = null, int PageSize = 10, string SenderStatus = "", int CurrentpageCount = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            if (message == "mailSent")
            {
                ViewBag.Message = "Mails sent successfully !";
                message = "";
            }
            else if (message == "mailNotSent")
            {
                ViewBag.Message = "Mails not sent ! Please try again.";
                message = "";
            }
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            SetViewBagForSCities(CountryCode);
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            var result = faxer.GetFilteredFaxerList(CountryCode, City, SenderName,
                AccountNo, Address, TelphoneNo, Email, SenderStatus, pageNumber, pageSize);
            ViewBag.NumberOfPage = 0;
            ViewBag.CurrentpageCount = CurrentpageCount;
            ViewBag.ButtonCount = 0;
            if (result.Count != 0)
            {
                var TotalCount = result.FirstOrDefault().TotalCount;
                int NumberOfPage = Common.Common.GetNumberOfPage(TotalCount, PageSize);
                ViewBag.NumberOfPage = NumberOfPage;
                var numberofbuttonshown = NumberOfPage - CurrentpageCount;
                ViewBag.ButtonCount = numberofbuttonshown;
            }
            List<ViewModels.ViewRegisteredFaxersViewModel> vm = result;
            ViewBag.SenderStatus = SenderStatus;
            ViewBag.Country = CountryCode;
            ViewBag.City = City;
            ViewBag.SenderName = SenderName;
            ViewBag.AccountNo = AccountNo;
            ViewBag.Address = Address;
            ViewBag.TelphoneNo = TelphoneNo;
            ViewBag.Email = Email;
            ViewBag.TotalRegisteredSender = faxer.List().Count;
            ViewBag.TotalActiveSender = faxer.ListOfActiveSender().Count();
            ViewBag.TotalInActiveSender = faxer.ListOfInActiveSender().Count;
            ViewBag.IsFormTransactionStatement = IsFormTransactionStatement;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = page ?? 1;
            return View(vm);
        }


        public ActionResult NewTransactionStatement(int SenderId = 0, int year = 0, string ReceiptNo = "", int? page = null, int PageSize = 10)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            ViewBag.Year = new SelectList(Enumerable.Range(2018, 10));
            if (SenderId != 0)
            {
                var senderInfo = CommonService.GetSenderInfo(SenderId);
                ViewBag.SenderName = senderInfo.FirstName + " " + (!string.IsNullOrEmpty(senderInfo.MiddleName) == true ? senderInfo.MiddleName + " " : "") + senderInfo.LastName;
                ViewBag.SenderAccountNo = senderInfo.AccountNo;
                ViewBag.SenderCountry = Common.Common.GetCountryName(senderInfo.Country);
            }
            ViewBag.SenderId = SenderId;
            int pageSize = PageSize;
            int pageNumber = (page ?? 1);
            var result = faxer.GetNewTransactionStatement(SenderId, year, ReceiptNo);
            result.Monthly = faxer.GetMonthlyTransactionMeter(result.TransactionListDownload, SenderId);
            result.TransactionList = result.TransactionListDownload.ToPagedList(pageNumber, pageSize == 0 ? result.TransactionListDownload.Count : pageSize);
            ViewBag.PageSize = pageSize;
            return View(result);

        }

        public FileContentResult DownloadStatement(int SenderId = 0, int year = 0)
        {

            var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
            var statementURL = baseUrl + "/EmailTemplate/SenderTransactionStatement/Index?SenderId=" + SenderId + "&year=" + year;
            var statementPDF = Common.Common.GetPdf(statementURL);
            byte[] bytes = statementPDF.Save();
            string mimeType = "Application/pdf";
            return File(bytes, "application/pdf", DateTime.Now + " " + "MoneyFex Sender Transaction Statement.pdf");
        }


        public ActionResult SenderDashboard(int senderId)
        {
            return View();
        }

        public ActionResult ViewRegisteredFaxersMore(int id, bool isBusiness = false)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            ViewBag.IsBusiness = isBusiness;
            if (id != 0)
            {
                var viewmodel = faxer.getFaxerInformation(id);
                viewmodel.FaxerNoteList = faxer.GetFaxerNotes(id);
                return View(viewmodel);
            }
            return View();
        }

        public ActionResult AddNewFaxerNote(string note, int faxerId)
        {
            FaxerNote faxerNote = new FaxerNote()
            {

                Note = note,
                FaxerId = faxerId,
                CreatedByStaffId = Common.StaffSession.LoggedStaff.StaffId,
                CreatedByStaffName = Common.StaffSession.LoggedStaff.FirstName + " " + Common.StaffSession.LoggedStaff.MiddleName + " " + Common.StaffSession.LoggedStaff.LastName,
                CreatedDateTime = DateTime.Now
            };

            var result = faxer.AddNewNote(faxerNote);


            return RedirectToAction("ViewRegisteredFaxersMore", new { id = faxerId });


        }


        public ActionResult DeleteFaxerInformation(int id)
        {
            if (id != 0)
            {
                var result = faxer.DeleteFaxerInformation(id);
                return Json(new
                {
                    Data = true,
                    Message = "Deleted Sucessfully"
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new
                {
                    Data = false,
                    Message = "Something went wrong. Please try again!"
                }, JsonRequestBehavior.AllowGet);
            }
        }



        public ActionResult ActivateStatus(int id = 0, bool AccountStatus = false, bool isBusiness = false)
        {


            if (id != 0)
            {
                var faxerid = id;
                var accountstatus = AccountStatus;
                faxer.AccountStatus(faxerid, accountstatus);
            }
            if (isBusiness == true)
            {
                return RedirectToAction("Index", "RegisteredBusinessSender");

            }
            return RedirectToAction("Index");
        }



        [HttpGet]
        public ActionResult UpdateFaxerInformation(int id, bool isFromBusiness = false)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var CardType = CommonService.GetCardType();
            ViewBag.CardType = new SelectList(CardType, "CardType", "CardType");

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            ViewRegisteredFaxersViewModel viewModel = new ViewRegisteredFaxersViewModel();
            viewModel.IsFromBusiness = isFromBusiness;
            if (id != 0)
            {
                viewModel = faxer.getFaxerInformation(id);
                ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(viewModel.CountryCode);
                viewModel.IsFromBusiness = isFromBusiness;
            }
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult UpdateFaxerInformation([Bind(Include = ViewRegisteredFaxersViewModel.BindProperty)] ViewRegisteredFaxersViewModel updatevm)
        {

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            var CardType = CommonService.GetCardType();
            ViewBag.CardType = new SelectList(CardType, "Id", "CardType");

            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(updatevm.CountryCode);
            if (ModelState.IsValid)
            {
                bool IsAgeValid = DateUtilities.ValidateAge(updatevm.DateOfBirth);
                if (IsAgeValid == false)
                {
                    ModelState.AddModelError("DateOfBirth", "Sorry, The sender must be 18 years of age or above to sign up to our services!");
                    return View(updatevm);
                }
                var faxerInfo = faxer.getFaxerInformation(updatevm.Id);
                updatevm.UsernameEmail = updatevm.UsernameEmail.Trim();
                updatevm.Phone = updatevm.Phone.Trim();

                //if (faxerInfo.UsernameEmail != updatevm.UsernameEmail)
                //{
                //    bool isEmailExist = Common.OtherUtlities.IsEmailExist(updatevm.UsernameEmail);
                //    if (isEmailExist == false)
                //    {
                //        ModelState.AddModelError("UsernameEmail", "Email Already Exist");
                //        return View(updatevm);
                //    }
                //}
                //if (faxerInfo.Phone != updatevm.Phone)
                //{
                //    bool IsMobileNoExist = OtherUtlities.IsMobileNoExist(updatevm.Phone);
                //    if (IsMobileNoExist == false)
                //    {
                //        ModelState.AddModelError("Phone", "Phone number Already Exist");
                //        return View(updatevm);
                //    }
                //}
                var countryPhoneCode = Common.Common.GetCountryPhoneCode(updatevm.CountryCode).Split('+')[1];
                if (updatevm.Phone.StartsWith(countryPhoneCode))
                {
                    updatevm.Phone = updatevm.Phone.Substring(countryPhoneCode.Length, updatevm.Phone.Length - countryPhoneCode.Length);
                }
                SmsApi smsApi = new SmsApi();
                bool validMobile = smsApi.IsValidMobileNo(Common.Common.GetCountryPhoneCode(updatevm.CountryCode) + updatevm.Phone);
                if (!validMobile)
                {
                    ModelState.AddModelError("Telephone", "Invalid mobile number ");
                    return View(updatevm);
                }


                bool result = faxer.UpdateFaxerInformation(updatevm);
                if (result)
                {
                    if (updatevm.IsFromBusiness)
                    {
                        return RedirectToAction("Index", "RegisteredBusinessSender");
                    }
                    else
                    {
                        return RedirectToAction("Index", "ViewRegisteredFaxers");
                    }
                }
            }
            return View(updatevm);
        }


        [HttpGet]
        public ActionResult RegisterAFaxer()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var CardType = CommonService.GetCardType();
            ViewBag.CardType = new SelectList(CardType, "Id", "CardType");

            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult RegisterAFaxer([Bind(Include = ViewRegisteredFaxersViewModel.BindProperty)] ViewRegisteredFaxersViewModel model)
        {
            var CardType = CommonService.GetCardType();
            ViewBag.CardType = new SelectList(CardType, "Id", "CardType");
            var countries = CommonService.GetCountries();
            ViewBag.Countries = new SelectList(countries, "Code", "Name");
            ViewBag.CountryPhoneCode = Common.Common.GetCountryPhoneCode(model.Country);
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.IssuingCountry))
                {
                    ModelState.AddModelError("IssuingCountry", "Select Issuing Country");
                    return View(model);
                }
                if (string.IsNullOrEmpty(model.IDCardNumber))
                {
                    ModelState.AddModelError("IssuingCountry", "Select Issuing Country");
                    return View(model);
                }
                bool isvalidAge = DateUtilities.ValidateAge(model.DateOfBirth);
                bool isEmailExist = faxer.checkExistingEmail(model.UsernameEmail);
                bool isValidExpDate = DateUtilities.DateGreaterThanToday(model.IDCardExpDate);
                if (isEmailExist == false)
                {
                    ModelState.AddModelError("UsernameEmail", "A sender with this e-mail address is already registered !");
                }
                if (isvalidAge == false)
                {
                    ModelState.AddModelError("DateOfBirth", "You must be 18 years of age or above to sign up to our services.");
                }
                if (isValidExpDate == false)
                {
                    ModelState.AddModelError("IDCardExpDate", "Sorry, Expired cards are not valid !");
                }
                if (isvalidAge && isEmailExist && isValidExpDate)
                {
                    if (Request.Files.Count > 0)
                    {
                        string fileName = "";
                        string directory = Server.MapPath("/Documents");
                        var CardImage = Request.Files["CardImage"];
                        if (CardImage != null && CardImage.ContentLength > 0)
                        {
                            fileName = Guid.NewGuid() + "." + CardImage.FileName.Split('.')[1];
                            CardImage.SaveAs(Path.Combine(directory, fileName));
                        }
                        model.IDCardImage = "/Documents/" + fileName;
                    }
                    var result = faxer.RegisterFaxer(model);
                    if (result)
                    {
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult FaxersContactDetails([Bind(Include = ViewRegisteredFaxersViewModel.BindProperty)] ViewRegisteredFaxersViewModel model)
        {
            FaxerInformation data = new FaxerInformation();

            data.FirstName = FaxerSession.FaxerDetails.FirstName;
            data.MiddleName = FaxerSession.FaxerDetails.MiddleName;
            data.LastName = FaxerSession.FaxerDetails.LastName;
            data.DateOfBirth = FaxerSession.FaxerDetails.DateOfBirth;
            data.GGender = (int)FaxerSession.FaxerDetails.Gender;
            data.Email = FaxerSession.FaxerDetails.UsernameEmail;

            data.IdCardType = FaxerSession.FaxerIdenty.IDCardType;
            data.IdCardNumber = FaxerSession.FaxerIdenty.IDCardNumber;
            data.IdCardExpiringDate = FaxerSession.FaxerIdenty.IDCardExpDate;
            data.IssuingCountry = FaxerSession.FaxerIdenty.IssuingCountry;
            data.CheckAmount = FaxerSession.FaxerIdenty.TransactionOver1000;


            data.Address1 = model.Address1; ;
            data.Address2 = model.Address2;
            data.City = model.City;
            data.State = model.State;
            data.PostalCode = model.PostalCode;
            data.Country = model.Country;
            data.PhoneNumber = model.Phone;

            dbContext.FaxerInformation.Add(data);
            dbContext.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult getCountryPhoneCode(string countryCode)
        {
            var code = CommonService.getPhoneCodeFromCountry(countryCode);
            return Json(new
            {
                PhoneCode = code
            }, JsonRequestBehavior.AllowGet);
        }


        //Filter By Country 

        public void SetViewBagForSCities(string Country = "")
        {
            var cities = SCity.GetCities(DB.Module.Faxer, Country);

            ViewBag.SCities = new SelectList(cities, "Name", "Name");
        }

        public ActionResult sendEmails(string body = "", string subject = "", string emails = "")
        {
            if (!string.IsNullOrEmpty(body) && !string.IsNullOrEmpty(subject) && !(string.IsNullOrEmpty(emails)))
            {
                bool result = Common.Common.sendBulkMail(emails, subject, body);
                if (result)
                {
                    return RedirectToAction("Index", new { @message = "mailSent" });
                }

            }
            return RedirectToAction("Index", new { @message = "mailNotSent" });
        }
        [HttpGet]
        public JsonResult SendEMail(string emails = "", string EmailType = "")
        {
            EmailServices _emailServices = new EmailServices();
            _emailServices.sendAnouncementEmail(emails, EmailType);
            return Json(new { Data = true }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult GetEmailType(string AnnouncementType = "")
        {
            var data = new List<Admin.Services.DropDownViewModel>();

            switch (AnnouncementType)
            {
                case "generalannouncement":
                    data = new List<Admin.Services.DropDownViewModel>();
                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "closure",
                        Name = "Closure"
                    });
                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "reopening",
                        Name = "Re-Opening"
                    });
                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "sales",
                        Name = "Sales"
                    });
                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "nationaldays",
                        Name = "NationalDays"
                    });
                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "internationalwomenday",
                        Name = "International Women Day"
                    });
                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "christmas",
                        Name = "Christmas"
                    });
                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "newyear",
                        Name = "New year"
                    });
                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "eid",
                        Name = "Eid mubarak"
                    });

                    break;
                case "newcorridor":
                    data = new List<Admin.Services.DropDownViewModel>();

                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "southafrica",
                        Name = "South Africa"
                    });
                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "morocco",
                        Name = "Morocco"
                    });
                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "senegal",
                        Name = "Senegal"
                    });
                    break;
                case "others":
                    data = new List<Admin.Services.DropDownViewModel>();

                    data.Add(new Admin.Services.DropDownViewModel()
                    {
                        Code = "mobileapplaunch",
                        Name = "Mobile App Launch"
                    });
                    break;
            }
            return Json(new
            {
                Data = data
            }, JsonRequestBehavior.AllowGet);
        }

        public FileResult DownloadAllRegisteredSenders(string dateRange = "")
        {
            var registeredSenders = faxer.GetSenderDetailsForExcelFile(dateRange);
            Common.Utilities _utilities = new Utilities();
            var fsr = _utilities.CreateExcelWorkSheet(registeredSenders, "RegisteredSenders");
            return fsr;
        }
    }

}