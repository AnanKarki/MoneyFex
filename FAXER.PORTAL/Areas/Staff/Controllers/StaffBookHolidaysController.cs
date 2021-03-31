using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using FAXER.PORTAL.Areas.Staff.Services;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;

namespace FAXER.PORTAL.Areas.Staff.Controllers
{
    public class StaffBookHolidaysController : Controller
    {
        StaffMessageServices Service = new StaffMessageServices();
        // GET: Staff/StaffBookHolidays
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult BookHolidays()
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("StaffMainLogin", "StaffLogin");
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            StaffBookHolidaysServices holidaysServices = new StaffBookHolidaysServices();
            var model = holidaysServices.getHolidays();
            return View(model);
        }

        public ActionResult RequestHolidays(string startdate, string enddate, string reason)
        {
            var HolidaysStartDate = Convert.ToDateTime(startdate);
            var HolidaysEndDate = Convert.ToDateTime(enddate);

            //if (HolidaysEndDate.Date.Year != DateTime.Now.Year) {
            //    TempData["Validation"] = "You are not allowed to request holidays between different Years";
            //    return RedirectToAction("BookHolidays");

            //}
            StaffBookHolidaysServices holidaysServices = new StaffBookHolidaysServices();
            var result1 = holidaysServices.GetHolidaysEntitlement(Common.StaffSession.LoggedStaff.StaffId);
            if (result1 == StaffHolidaysEntiltlement.No)
            {
                TempData["Validation"] = "You are not entitled to request holidays";
                return RedirectToAction("BookHolidays");
            }
            if (string.IsNullOrEmpty(reason))
            {


                TempData["Validation"] = "Please enter the reasons for holiday";
                return RedirectToAction("BookHolidays");
            }
            var lastHolidayRequest = holidaysServices.GetLastHolidayRequest(Common.StaffSession.LoggedStaff.StaffId);
            if ((lastHolidayRequest != null) && lastHolidayRequest.HolidaysRequestStatus == HollidayRequestStatus.Requested)
            {

                TempData["Validation"] = "You can not request holidays because your previous request is on process";
                return RedirectToAction("BookHolidays");
            }


            var diff = HolidaysEndDate - HolidaysStartDate;
            var diffCount = diff.Days + 1;
            int regularHolidaysCount = 0;
            // Calculate total Requested days 
            var publicHolidays = holidaysServices.GetPublicHolidays(HolidaysStartDate, HolidaysEndDate);
            for (int i = 0; i < diffCount; i++)
            {

                var regularholidays = HolidaysStartDate.AddDays(i).DayOfWeek;
                if (regularholidays == DayOfWeek.Saturday || regularholidays == DayOfWeek.Sunday)
                {

                    regularHolidaysCount = regularHolidaysCount + 1;
                }

                for (int j = 0; j < publicHolidays.Count; j++)
                {
                    var StartToEndDate = HolidaysStartDate.AddDays(i).Date;

                    if (StartToEndDate >= publicHolidays[j].FromDate && StartToEndDate <= publicHolidays[j].ToDate)
                    {

                        regularHolidaysCount = regularHolidaysCount + 1;
                    }
                }

            }

            int totalNumberofHolidaysRequested = diffCount - regularHolidaysCount;


            var staffInformation = holidaysServices.GetStaffInformation(Common.StaffSession.LoggedStaff.StaffId);
            if (HolidaysStartDate.Year == HolidaysEndDate.Year)
            {
                if (totalNumberofHolidaysRequested > staffInformation.StaffNoOFHolidays)
                {
                    TempData["Validation"] = "You are Only allowed to Request " + " " + staffInformation.StaffNoOFHolidays + " " + "No of Holidays a year";
                    return RedirectToAction("BookHolidays");
                }
            }
            var HolidaysLeft = holidaysServices.SumHolidaysLeft(Common.StaffSession.LoggedStaff.StaffId, HolidaysStartDate);
            var HolidaysTaken = holidaysServices.SumHolidaysTaken(Common.StaffSession.LoggedStaff.StaffId, HolidaysStartDate);
            int holidayLeft = 0;
            if (HolidaysLeft != null)
            {
                holidayLeft = HolidaysLeft.HoidaysLeft;

                //either the given date is valid or not
                bool isvalid = holidaysServices.ValidateDate(Common.StaffSession.LoggedStaff.StaffId, HolidaysStartDate, HolidaysEndDate);

                if (isvalid == false)
                {
                    TempData["Validation"] = "You have Already request hollday on your choosen date";
                    return RedirectToAction("BookHolidays");
                }
                //end

            }
            else
            {
                holidayLeft = staffInformation.StaffNoOFHolidays;
            }

            if (HolidaysStartDate.Year == HolidaysEndDate.Year)
            {
                // when startdate an enddate is with in same year

                if (holidayLeft < totalNumberofHolidaysRequested)
                {
                    TempData["Validation"] = "You have not enough holidays left to Request";
                    return RedirectToAction("BookHolidays");
                }
                //end
            }
            else
            {
                // If Holidaystartdate and endDate is of different Years
                int regularHolidaysCountOldYear = 0;
                int regularHolidaysCountNewYear = 0;
                int olddiff = 0;
                int newdiff = 0;

                for (int i = 0; i < diffCount; i++)
                {
                    var date = HolidaysStartDate.AddDays(i).Date;
                    if (date.Year == HolidaysStartDate.Date.Year)
                    {
                        olddiff = olddiff + 1;
                        var regularholidays = HolidaysStartDate.AddDays(i).DayOfWeek;

                        if (regularholidays == DayOfWeek.Saturday || regularholidays == DayOfWeek.Sunday)
                        {


                            regularHolidaysCountOldYear = regularHolidaysCountOldYear + 1;
                        }
                        for (int j = 0; j < publicHolidays.Count; j++)
                        {

                            var StartToEndDate = HolidaysStartDate.AddDays(i).Date;

                            if (StartToEndDate >= publicHolidays[j].FromDate && StartToEndDate <= publicHolidays[j].ToDate)
                            {
                                if (publicHolidays[j].FromDate.Year == StartToEndDate.Year && publicHolidays[j].ToDate.Year == StartToEndDate.Year)
                                {

                                    regularHolidaysCount = regularHolidaysCount + 1;
                                }
                            }
                        }

                    }

                    else
                    {
                        newdiff = newdiff + 1;
                        var regularholidays = HolidaysStartDate.AddDays(i).DayOfWeek;

                        if (regularholidays == DayOfWeek.Saturday || regularholidays == DayOfWeek.Sunday)
                        {


                            regularHolidaysCountNewYear = regularHolidaysCountNewYear + 1;
                        }
                        for (int j = 0; j < publicHolidays.Count; j++)
                        {

                            var StartToEndDate = HolidaysStartDate.AddDays(i).Date;

                            if (StartToEndDate >= publicHolidays[j].FromDate && StartToEndDate <= publicHolidays[j].ToDate)
                            {
                                if (publicHolidays[j].FromDate.Year == StartToEndDate.Year && publicHolidays[j].ToDate.Year == StartToEndDate.Year)
                                {
                                    regularHolidaysCount = regularHolidaysCount + 1;
                                }
                            }
                        }

                    }

                }
                int TotalRequestedDaysOldYear = olddiff - regularHolidaysCountOldYear;
                int TotalRequestdaysnewYear = newdiff - regularHolidaysCountNewYear;

                if (holidayLeft < TotalRequestedDaysOldYear)
                {
                    TempData["Validation"] = "You have not enough holidays left of this year to Request";
                    return RedirectToAction("BookHolidays");
                }
                var daysLeftNewYear = holidaysServices.SumHolidaysLeft(Common.StaffSession.LoggedStaff.StaffId, HolidaysEndDate);

                if ((daysLeftNewYear != null) && TotalRequestdaysnewYear > daysLeftNewYear.HoidaysLeft)
                {
                    TempData["Validation"] = "You have not enough holidays to Request";
                    return RedirectToAction("BookHolidays");
                }
                if (TotalRequestdaysnewYear > staffInformation.StaffNoOFHolidays)
                {

                    TempData["Validation"] = "You have not enough holidays to Request";
                    return RedirectToAction("BookHolidays");
                }
            }

            DB.StaffHolidays staffHolidays = new DB.StaffHolidays()
            {
                StaffInformationId = Common.StaffSession.LoggedStaff.StaffId,
                HolidaysStartDate = HolidaysStartDate,
                HolidaysEndDate = HolidaysEndDate,
                TotalNumberOfHolidaysRequeste = totalNumberofHolidaysRequested,
                HolidaysTaken = 0,
                HoidaysLeft = holidayLeft,
                HolidaysRequestStatus = DB.HollidayRequestStatus.Requested,
                HolidaysReason = reason
            };

            var result = holidaysServices.SaveHolidays(staffHolidays);
            if (result != null)
            {

                var baseUrl = Request.Url.GetLeftPart(UriPartial.Authority);
                // Email Template
                string body = string.Empty;
                using (StreamReader reader = new StreamReader(Server.MapPath("~/Areas/Staff/Views/StaffBookHolidays/RequestHolidayEmailTemplate.html")))
                {

                    body = reader.ReadToEnd();
                }
                body = body.Replace("{StaffName}", staffInformation.FirstName + " " + staffInformation.LastName);
                body = body.Replace("{StaffId}", staffInformation.Id.ToString());
                body = body.Replace("{StaffCountry}", staffInformation.Country);
                body = body.Replace("{StaffCity}", staffInformation.City);
                body = body.Replace("{TotalHolidayRequested}", result.TotalNumberOfHolidaysRequeste.ToString());
                body = body.Replace("{TotalHolidaysAllowed}", staffInformation.StaffNoOFHolidays.ToString());
                body = body.Replace("{HolidaysTaken}", HolidaysTaken.ToString());
                body = body.Replace("{HolidaysLeft}", holidayLeft.ToString());
                body = body.Replace("{HolidayStartDate}", result.HolidaysStartDate.ToString("dd/MM/yyyy"));
                body = body.Replace("{HolidayEndDate}", result.HolidaysEndDate.ToString("dd/MM/yyyy"));
                body = body.Replace("{HolidayId}", result.Id.ToString());
                body = body.Replace("{HolidayReason}", result.HolidaysReason);
                body = body.Replace("{ApproveHoliday}", baseUrl + "/Staff/StaffBookHolidays/ApproveHolidaysRequest?holidayId" + result.Id);
                body = body.Replace("{RejectHoliday}", baseUrl + "/Staff/StaffBookHolidays/RejectHolidaysRequest?holidayId" + result.Id);

                // Email template End  
                MailCommon mail = new MailCommon();
                MailMessage msg = new MailMessage();
                msg.Subject = "Holidays Request";
                msg.Body = body;
                msg.IsBodyHtml = true;
                try
                {

                    mail.SendMail("anege1234@gmail.com", "Holiday Request", msg.Body);
                    mail.SendMail(staffInformation.EmailAddress, "Holiday Request", msg.Body);
                }
                catch (Exception)
                {

                    throw;
                }
                ViewBag.Count = Service.getInboxCount();
                return RedirectToAction("BookHolidays");
            }
            ViewBag.Count = Service.getInboxCount();
            ViewBag.DraftCount = Service.getDraftCount();
            return RedirectToAction("BookHolidays");
        }


        public ActionResult CancelHolidays(int HolidayId)
        {

            Services.StaffBookHolidaysServices bookHolidaysServices = new Services.StaffBookHolidaysServices();
            var cancelled = bookHolidaysServices.CancelHolidays(HolidayId);
            if (cancelled == true)
            {
                ViewBag.Count = Service.getInboxCount();
                return RedirectToAction("BookHolidays");
            }
            else
            {
                ViewBag.Count = Service.getInboxCount();
                TempData["Validation"] = "Your Holidays has already been expired";
                return RedirectToAction("BookHolidays");

            }
        }

        public JsonResult ApproveHolidaysRequest(string holidayId)
        {
            Services.StaffBookHolidaysServices holidaysServices = new StaffBookHolidaysServices();

            var holidayInformation = holidaysServices.GetHolidaysById(Convert.ToInt32(holidayId));
            if (holidayInformation != null)
            {
                if (holidayInformation.HolidaysRequestStatus == HollidayRequestStatus.Requested)
                {
                    var result = holidaysServices.AprroveHolidayRequest(Convert.ToInt32(holidayId));
                    if (result == true)
                    {
                        var staffInformation = holidaysServices.GetStaffById(holidayInformation.StaffInformationId);

                        MailCommon mail = new MailCommon();
                        try
                        {
                            string msg = "Your Holidays Request of " + " " + holidayInformation.TotalNumberOfHolidaysRequeste + " " + "From" + " " + holidayInformation.HolidaysStartDate + " " + "to" + " " + holidayInformation.HolidaysEndDate + "has been approved.";
                            mail.SendMail(staffInformation.EmailAddress, "Holiday Request Approved", msg);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    return Json("Holiday Approved Email has been sent", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    string status = Enum.GetName(typeof(HollidayRequestStatus), holidayInformation.HolidaysRequestStatus);
                    return Json("Holiday Request has been Already "  + status, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Holiday Approved Email Fail to sent", JsonRequestBehavior.AllowGet);
        }

        public JsonResult RejectHolidaysRequest(string holidayId)
        {
            Services.StaffBookHolidaysServices holidaysServices = new StaffBookHolidaysServices();

            var holidayInformation = holidaysServices.GetHolidaysById(Convert.ToInt32(holidayId));
            if (holidayInformation != null)
            {
                if (holidayInformation.HolidaysRequestStatus == HollidayRequestStatus.Requested)
                {
                    var result = holidaysServices.RejectholidayRequest(Convert.ToInt32(holidayId));
                    var staffInformation = holidaysServices.GetStaffById(holidayInformation.StaffInformationId);

                    MailCommon mail = new MailCommon();
                    try
                    {
                        string msg = "Your Holidays Request Has been rejected of" + " " + holidayInformation.HolidaysStartDate + " " + "to" + " " + holidayInformation.HolidaysEndDate;
                        mail.SendMail(staffInformation.EmailAddress, "Holiday Request Rejected", msg);
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                    return Json("Holiday Reject Email has been sent", JsonRequestBehavior.AllowGet);

                }
                else
                {
                    string status = Enum.GetName(typeof(HollidayRequestStatus), holidayInformation.HolidaysRequestStatus);
                    return Json("Holiday Request has been Already "  + status, JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Holiday Reject Email fail to sent", JsonRequestBehavior.AllowGet);
        }

    }


}
