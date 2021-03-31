using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Staff.Services;
using FAXER.PORTAL.Areas.Staff.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Staff.Controllers
{
    public class StaffMessageController : Controller
    {
        StaffMessageServices Service = new StaffMessageServices();


        // GET: Staff/StaffMessage
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Inbox(string status = "", int page = 0, string message = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            if (message == "allRead")
            {
                ViewBag.Message = "All messages are marked as read !";
                message = "";
            }
            else if (message == "markedRead")
            {
                ViewBag.Message = "Messages are marked as read !";
                message = "";
            }
            else if (message == "null")
            {
                ViewBag.Message = "Please select the messages that you want to archive !";
                message = "";
            }

            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            var vm = Service.getInboxList(status);

            var count = vm.Count();
            int PageSize = 3;
            var data = vm.Skip(page * PageSize).Take(PageSize).ToList();
            ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);
            ViewBag.Page = page;
            ViewBag.startCount = count == 0 ? page * PageSize : page * PageSize + 1;
            ViewBag.EndCount = page * PageSize + data.Count();
            ViewBag.TotalCount = vm.Count();

            return View(data);
        }

        public ActionResult Sent(int page = 0, string message = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            if (message == "success")
            {
                ViewBag.Message = "Message Sent Successfully !";
                message = "";
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            var vm = Service.getSentMessagesList();


            int PageSize = 3;
            var count = vm.Count();
            var data = vm.Skip(page * PageSize).Take(PageSize).ToList();
            ViewBag.Page = page;
            ViewBag.startCount = count == 0 ? page * PageSize : page * PageSize + 1;
            ViewBag.EndCount = page * PageSize + data.Count();
            ViewBag.TotalCount = vm.Count();

            return View(data);
        }

        public ActionResult Drafts(int page = 0, string message = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            if (message == "success")
            {
                ViewBag.Message = "Message Saved to Drafts !";
                message = "";
            }
            else if (message == "deleted")
            {
                ViewBag.Message = "Message Deleted Successfully !";
                message = "";
            }
            else if (message == "null")
            {
                ViewBag.Message = "Please select the messages that you want to delete !";
                message = "";
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            var vm = Service.getDraftMessagesList();

            int PageSize = 3;
            var count = vm.Count();
            var data = vm.Skip(page * PageSize).Take(PageSize).ToList();
            ViewBag.Page = page;
            ViewBag.startCount = count == 0 ? page * PageSize : page * PageSize + 1;
            ViewBag.EndCount = page * PageSize + data.Count();
            ViewBag.TotalCount = vm.Count();

            return View(data);
        }

        public ActionResult Archive(int page = 0, string message = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            if (message == "success")
            {
                ViewBag.Message = "Message Archived Successfully !";
                message = "";
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            var vm = Service.getArchivedList();
            int PageSize = 3;
            var count = vm.Count();
            var data = vm.Skip(page * PageSize).Take(PageSize).ToList();
            ViewBag.Page = page;
            ViewBag.startCount = count == 0 ? page * PageSize : page * PageSize + 1;
            ViewBag.EndCount = page * PageSize + data.Count();
            ViewBag.TotalCount = vm.Count();

            return View(data);
        }

        public ActionResult Junk(int page = 0, string message = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            if (message == "deleted")
            {
                ViewBag.Message = "Message Deleted Successfully !";
                message = "";
            }
            else if (message == "null")
            {
                ViewBag.Message = "Please select the messages that you want to delete !";
                message = "";

            }
            else if (message == "permanent")
            {
                ViewBag.Message = "Message Deleted Permanently";
                message = "";
            }
            else if (message == "notDeleted")
            {
                ViewBag.Message = "Message Not Deleted. Please try again !";
                message = "";
            }
            else if (message == "restored")
            {
                ViewBag.Message = "Message Restored successfully !";
                message = "";
            }
            else if (message == "notRestored")
            {
                ViewBag.Message = "Message Not Restored ! Please try again !";
                message = "";
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            var vm = Service.getJunkList();

            var count = vm.Count();
            int PageSize = 3;
            var data = vm.Skip(page * PageSize).Take(PageSize).ToList();
            ViewBag.MaxPage = (count / PageSize) - (count % PageSize == 0 ? 1 : 0);
            ViewBag.Page = page;
            ViewBag.startCount = count == 0 ? page * PageSize : page * PageSize + 1;
            ViewBag.EndCount = page * PageSize + data.Count();
            ViewBag.TotalCount = vm.Count();

            return View(data);
        }

        [HttpGet]
        public ActionResult Compose(string Country = "", string City = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }

            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            StaffEmailComposeViewModel vm = new StaffEmailComposeViewModel();
            vm.FromEmailAddress = Service.getStaffEmail(Common.StaffSession.LoggedStaff.StaffId);
            vm.EmailList = Service.getAllStaffEmails(Country , City);

            Admin.Services.CommonServices adminCommonServices = new Admin.Services.CommonServices();
            var countries = adminCommonServices.GetCountries();

            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var cities = SCity.GetCities(DB.Module.Staff, Country);
            ViewBag.SCities = new SelectList(cities, "Name", "Name");

            if (Country != "" || City != "") {

                ViewBag.SearchByCountryorCity = true;
            }

            return View(vm);
        }

        [HttpPost]
        public ActionResult Compose([Bind(Include = StaffEmailComposeViewModel.BindProperty)]StaffEmailComposeViewModel model)
        {
            Admin.Services.CommonServices adminCommonServices = new Admin.Services.CommonServices();
            var countries = adminCommonServices.GetCountries();
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();

           

            ViewBag.Countries = new SelectList(countries, "Code", "Name");

            var cities = SCity.GetCities(DB.Module.Staff, "");
            ViewBag.SCities = new SelectList(cities, "Name", "Name");

            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            if (model != null)
            {
                int staffId = Service.getStaffIdFromEmail(model.ToEmailAddress);
                bool valid = true;
                if (staffId == 0)
                {
                    ModelState.AddModelError("ToEmailAddress", "Sorry! Staff with this Email Address not found !");
                    valid = false;
                }
                if (staffId == Common.StaffSession.LoggedStaff.StaffId)
                {
                    ModelState.AddModelError("ToEmailAddress", "You aren't allowed to send Email to yourself ! ");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.ToEmailAddress))
                {
                    ModelState.AddModelError("ToEmailAddress", "Receiver Staff Email Address can't be null !");
                    valid = false;
                }
                if (model.Draft == false)
                {
                    if (string.IsNullOrEmpty(model.Subject))
                    {
                        ModelState.AddModelError("Subject", "Subject Can't be empty !");
                        valid = false;
                    }
                    if (string.IsNullOrEmpty(model.BankPaymentReference))
                    {
                        ModelState.AddModelError("BankPaymentReference", "Sorry ! This field can't be empty !");
                        valid = false;
                    }
                }

                if (valid == true)
                {
                    if (Request.Files.Count > 0)
                    {
                        string fileName = "";
                        string directory = Server.MapPath("/Documents");
                        var attachment = Request.Files[0];

                        if (attachment != null && attachment.ContentLength > 0)
                        {
                            fileName = Guid.NewGuid() + "." + attachment.FileName.Split('.')[1];
                            attachment.SaveAs(Path.Combine(directory, fileName));
                            model.AttachmentURL = "/Documents/" + fileName;
                        }
                    }

                    model.ReceivingStaffId = staffId;
                    if (model.Draft == true)
                    {
                        bool result = Service.saveDraftMessage(model);
                        if (result)
                        {
                            return RedirectToAction("Drafts", new { @message = "success" });
                        }
                    }
                    else
                    {
                        bool result = Service.saveComposeMessage(model);
                        if (result)
                        {
                            return RedirectToAction("Sent", new { @message = "success" });
                        }
                    }
                }
            }
            model.EmailList = Service.getAllStaffEmails();
            return View(model);
        }


        public ActionResult ViewMessage(int id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            if (id != 0)
            {
                Service.MarkMessageDelivered(id);
                var vm = Service.viewMessage(id);
                if (vm.ReceivingStaffId == Common.StaffSession.LoggedStaff.StaffId)
                {
                    ViewBag.FromTo = "From";
                }
                else ViewBag.FromTo = "To";
                ViewBag.Count = Service.getInboxCount();
                ViewBag.DraftCount = Service.getDraftCount();
                return View(vm);
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            return RedirectToAction("Inbox");
        }


        [HttpGet]
        public ActionResult SentDraftMessage(int DraftMessageId = 0)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }

            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            StaffEmailComposeViewModel vm = new StaffEmailComposeViewModel();
            vm.FromEmailAddress = Service.getStaffEmail(Common.StaffSession.LoggedStaff.StaffId);

            if (DraftMessageId > 0)
            {
                var data = Service.viewMessage(DraftMessageId);
                vm.AttachmentURL = data.AttachmentURL;
                vm.Subject = data.Subject;
                vm.ToEmailAddress = data.ToEmailAddress;
                vm.BankPaymentReference = data.BankPaymentReference;
                vm.Id = DraftMessageId;

                if (data.AttachmentURL != null)
                {
                    var filename = data.AttachmentURL.Split('/');
                    ViewBag.FileName = filename[2];
                }
            }
            return View(vm);
        }

        [HttpPost]
        public ActionResult SentDraftMessage([Bind(Include = StaffEmailComposeViewModel.BindProperty)]StaffEmailComposeViewModel model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            if (model != null)
            {
                int staffId = Service.getStaffIdFromEmail(model.ToEmailAddress);
                bool valid = true;
                if (staffId == 0)
                {
                    ModelState.AddModelError("ToEmailAddress", "Sorry! Staff with this Email Address not found !");
                    valid = false;
                }
                if (string.IsNullOrEmpty(model.ToEmailAddress))
                {
                    ModelState.AddModelError("ToEmailAddress", "Receiver Staff Email Address can't be null !");
                    valid = false;
                }
                if (model.Draft == false)
                {
                    if (string.IsNullOrEmpty(model.Subject))
                    {
                        ModelState.AddModelError("Subject", "Subject Can't be empty !");
                        valid = false;
                    }
                    if (string.IsNullOrEmpty(model.BankPaymentReference))
                    {
                        ModelState.AddModelError("BankPaymentReference", "Sorry ! This field can't be empty !");
                        valid = false;
                    }
                }

                if (valid == true)
                {
                    if (Request.Files.Count > 0)
                    {
                        string fileName = "";
                        string directory = Server.MapPath("/Documents");
                        var attachment = Request.Files[0];

                        if (attachment != null && attachment.ContentLength > 0)
                        {
                            fileName = Guid.NewGuid() + "." + attachment.FileName.Split('.')[1];
                            attachment.SaveAs(Path.Combine(directory, fileName));
                            model.AttachmentURL = "/Documents/" + fileName;
                        }

                    }
                    model.ReceivingStaffId = staffId;

                    bool result = Service.UpdateMessage(model);
                    if (model.Draft == true)
                    {
                        return RedirectToAction("Drafts", new { @message = "success" });
                    }
                    else
                    {
                        return RedirectToAction("Sent", new { @message = "success" });
                    }
                }
            }
            return View(model);
        }


        public ActionResult MarkAsRead(string id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            if (string.IsNullOrEmpty(id))
            {
                Service.MarkAllMessagesDelivered();
                return RedirectToAction("Inbox", new { @message = "allRead" });
            }
            var ids = (from c in id.Split(',')
                       select c);
            foreach (var item in ids)
            {
                int adb = int.Parse(item);
                Service.MarkMessageDelivered(adb);
            }
            return RedirectToAction("Inbox", new { @message = "markedRead" });
        }

        public ActionResult MarkAsArchived(string id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            if (string.IsNullOrEmpty(id))
            {
                return RedirectToAction("Inbox", new { @message = "null" });
            }
            var ids = (from c in id.Split(',')
                       select c);
            foreach (var item in ids)
            {
                int adb = int.Parse(item);
                Service.MarkMessageArchived(adb);
            }
            return RedirectToAction("Archive", new { @message = "success" });
        }

        public ActionResult MarkAsDeleted(string id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            if (!string.IsNullOrEmpty(id))
            {
                var ids = (from c in id.Split(',')
                           select c);
                foreach (var item in ids)
                {
                    int adb = int.Parse(item);
                    Service.MarkMessageAsDeleted(adb);
                }
                return RedirectToAction("Junk", new { @message = "deleted" });
            }
            return RedirectToAction("Junk", new { @message = "null" });
        }
        public ActionResult DeleteDraftMessage(string id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            if (!string.IsNullOrEmpty(id))
            {
                var ids = (from c in id.Split(',')
                           select c);
                foreach (var item in ids)
                {
                    int adb = int.Parse(item);
                    Service.MarkMessageAsDeleted(adb);
                }
                return RedirectToAction("Drafts", new { @message = "deleted" });
            }
            return RedirectToAction("Drafts", new { @message = "null" });

        }

        public ActionResult DeleteMessagePermanently(string id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            if (!string.IsNullOrEmpty(id))
            {
                var ids = (from c in id.Split(',')
                           select c);

                foreach (var item in ids)
                {
                    int adb = int.Parse(item);
                    Service.deleteMessagePermanently(adb);
                }
                return RedirectToAction("Junk", new { @message = "permanent" });
            }

            return RedirectToAction("Junk", new { @message = "notDeleted" });
        }

        public ActionResult RestoreJunkMessage(string id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            if (!string.IsNullOrEmpty(id))
            {
                var ids = (from c in id.Split(',')
                           select c);

                foreach (var item in ids)
                {
                    int adb = int.Parse(item);
                    Service.restoreMessage(adb);
                }
                return RedirectToAction("Junk", new { @message = "restored" });
            }
            return RedirectToAction("Junk", new { @message = "notRestored" });
        }
    }
}