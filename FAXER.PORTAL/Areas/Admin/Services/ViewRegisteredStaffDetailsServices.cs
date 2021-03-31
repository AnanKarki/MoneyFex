using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Staff.ViewModels;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Common;
using System.Data.Entity;
using FAXER.PORTAL.Services;
using System.Data.Entity.Infrastructure;
using TransferZero.Sdk.Model;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewRegisteredStaffDetailsServices
    {
        FAXEREntities dbContext = new FAXEREntities();
        CommonServices ComonnService = new CommonServices();
        public List<StaffInformation> List()
        {
            var data = dbContext.StaffInformation.ToList();
            return data;
        }


        public List<StaffHolidays> ListOfHolidayStaff()
        {
            var data = dbContext.StaffHolidays.ToList();
            return data;
        }

        public List<StaffLogin> ListOfInActiveStaff()
        {
            var data = dbContext.StaffLogin.Where(x => x.IsActive == false).ToList();
            return data;
        }

        public StaffDashboardViewModel getStaffDashBoard(int StaffId)
        {
            string StaffName = "";
            StaffDashboardViewModel model = new StaffDashboardViewModel();
            var Hoildays = dbContext.StaffHolidays.Where(x => x.StaffInformationId == StaffId).ToList();
            model.HoliDay = new List<StaffHolidaysViewModel>();
            if (Hoildays.Count > 0)
            {
                model.HoliDay = (from c in Hoildays
                                 select new StaffHolidaysViewModel()
                                 {
                                     StaffId = c.StaffInformationId,
                                     StartDate = c.HolidaysStartDate.ToString("MMM-dd-yyyy"),
                                     FinishDate = c.HolidaysEndDate.ToString("MMM-dd-yyyy"),
                                     NoOfDays = c.TotalNumberOfHolidaysRequeste,
                                     ApprovedByName = ComonnService.getStaffName(c.ApprovedBy),

                                 }).ToList();

            }
            var Documentations = dbContext.StaffDocumentation.Where(x => x.StaffId == StaffId).ToList();

            model.Documentation = new List<BusinessDocumentationViewModel>();
            if (Documentations.Count > 0)
            {
                model.Documentation = (from c in Documentations
                                       select new BusinessDocumentationViewModel()
                                       {
                                           StaffId = c.StaffId,
                                           DocumentName = c.DocumentName,
                                           CreationDate = c.CreatedDate.ToString("MMM-dd-yyyy"),
                                           ExpiryDateString = c.ExpiryDate.ToFormatedString(),

                                       }).ToList();
            }
            var PaySlips = dbContext.StaffPayslip.Where(x => x.StaffId == StaffId).ToList();
            model.Payslip = new List<ViewStaffPayslipViewModel>();
            if (PaySlips.Count > 0)
            {
                model.Payslip = (from c in PaySlips
                                 select new ViewStaffPayslipViewModel()
                                 {
                                     StaffId = c.StaffId,
                                     StaffStatus = "No Payslip",
                                     PayslipMonth = Enum.GetName(typeof(Month), c.Month),
                                     StaffMFSCode = c.Staff.StaffMFSCode,
                                     DueDate = c.CreatedDateTime.ToString("MMM-dd-yyyy")

                                 }).ToList();
            }
            var Tranings = dbContext.StaffTraining.Where(x => x.StaffInformationId == StaffId).ToList();
            model.Traning = new List<ViewStaffTrainingViewModel>();
            if (Tranings.Count > 0)
            {
                model.Traning = (from c in Tranings
                                 select new ViewStaffTrainingViewModel()
                                 {
                                     staffId = c.StaffInformationId,
                                     Title = c.Title,
                                     CreationDate = c.TrainingAddedDate.ToString("MMM-dd-yyyy"),
                                     EndDate = c.EndDate,
                                     Status = "no Status foud"
                                 }).ToList();
            }

            var staffDetails = dbContext.StaffInformation.Where(x => x.Id == StaffId).FirstOrDefault();
            model.Staff = new StaffInformationViewModel();
            if (staffDetails != null)
            {

                model.Staff = new StaffInformationViewModel()
                {
                    StaffId = staffDetails.Id,
                    FullName = staffDetails.FirstName + " " + staffDetails.MiddleName + " " + staffDetails.LastName,
                    DOB = staffDetails.DateOfBirth.ToString("MMM-dd-yyyy"),
                    Address1 = staffDetails.Address1,
                    Address2 = staffDetails.Address2,
                    TimeAtAddress = staffDetails.BeenLivingSince.GetEnumDescription<BeenLivingSince>(),
                    PostCode = staffDetails.PostalCode,
                    City = staffDetails.City,
                    Country = ComonnService.getCountryNameFromCode(staffDetails.Country),
                    CountryFlag = staffDetails.Country.ToLower(),
                    KinFullName = staffDetails.NOKFirstName + " " + staffDetails.NOKMiddleName + " " + staffDetails.NOKLastName,
                    KinRelation = staffDetails.NOKRelationship,
                    KinAddress = staffDetails.NOKAddress1,
                    KinTelephone = staffDetails.NOKPhoneNumber,
                    KinEmail = staffDetails.NOKPostalCode

                };
            }
            var employment = getStaffDetailsMore(StaffId, out StaffName);
            model.Employment = employment == null ? new ViewRegisteredStaffMoreViewModel() : employment;
            var previousAddress = getPreviousAddress(StaffId, out StaffName).FirstOrDefault();
            model.PreviousAddress = previousAddress == null ? new ViewPreviousAddressViewModel() : previousAddress;
            return model;


        }
        public List<ViewRegisteredStaffViewModel> getStaffInfoList(string CountryCode = "", string City = "")
        {
            var data = dbContext.StaffLogin.ToList();
            if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            {
                data = data.Where(x => (x.IsDeleted == false) && (x.Staff.Country == CountryCode)).ToList();
            }
            else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = data.Where(x => (x.IsDeleted == false) && (x.Staff.City.ToLower() == City.ToLower())).ToList();
            }
            else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            {
                data = data.Where(x => (x.IsDeleted == false) && (x.Staff.City.ToLower() == City.ToLower()) && (x.Staff.Country == CountryCode)).ToList();
            }

            var result = (from c in data
                          select new ViewRegisteredStaffViewModel()
                          {
                              StaffId = c.StaffId,
                              StaffFirstName = c.Staff.FirstName,
                              StaffMiddleName = c.Staff.MiddleName,
                              StaffLastName = c.Staff.LastName,
                              StaffDOB = c.Staff.DateOfBirth.ToString("dd-MM-yyyy"),
                              StaffGender = (Gender)c.Staff.Gender,
                              PrivateEmail = c.Staff.EmailAddress,
                              //MFSEmail = c.Staff.mfs
                              StaffAddress1 = c.Staff.Address1,
                              StaffCity = c.Staff.City,
                              StaffCountry = ComonnService.getCountryNameFromCode(c.Staff.Country),
                              StaffTelephone = ComonnService.getPhoneCodeFromCountry(c.Staff.Country) + c.Staff.PhoneNumber,
                              TimeAtCurrentAddress = c.Staff.BeenLivingSince.GetEnumDescription<BeenLivingSince>(),
                              StaffMFSCode = c.Staff.StaffMFSCode,
                              Status = c.IsActive ? "Active" : "Inactive",
                              FirstLetterOfStaff = c.Staff.FirstName == null ? "" : c.Staff.FirstName.Substring(0, 1).ToLower(),
                          }).ToList();
            return result;
        }

        public List<ViewPreviousAddressViewModel> getPreviousAddress(int staffId, out string StaffName)
        {
            var staff = dbContext.StaffInformation.Find(staffId);
            StaffName = staff.FirstName + " " + staff.MiddleName + " " + staff.LastName;
            var result = (from c in dbContext.StaffAddresses.Where(x => x.StaffId == staffId).ToList()
                          select new ViewPreviousAddressViewModel()
                          {
                              staffId = c.StaffId,
                              Address1 = c.StaffAddress1,
                              Address2 = c.StaffAddress2,
                              City = c.StaffCity,
                              State = c.StaffState,
                              PostalCode = c.StaffPostalCode,
                              Country = ComonnService.getCountryNameFromCode(c.StaffCountry),
                              CountryFlag = c.StaffCountry.ToLower(),
                              LivedFor = c.BeenLivingSince.GetEnumDescription<BeenLivingSince>(),
                              PhoneNumber = c.StaffPhoneNumber,
                          }).ToList();
            return result;
        }

        public ViewRegisteredStaffMoreViewModel getStaffDetailsMore(int staffId, out string staffName)
        {
            var staff = dbContext.StaffInformation.Find(staffId);
            staffName = staff.FirstName + " " + staff.MiddleName + " " + staff.LastName;

            var result = (from c in dbContext.StaffLogin.Where(x => x.StaffId == staffId).ToList()
                          join d in dbContext.StaffEmployment.Where(x => x.StaffId == staffId) on c.StaffId equals d.StaffId
                          into joined
                          from j in joined.DefaultIfEmpty()
                          select new ViewRegisteredStaffMoreViewModel()
                          {
                              StaffId = c.StaffId,
                              SystemLoginLevel = c.Staff.SytemLoginLevelOfStaff,
                              NOKFirstName = c.Staff.NOKFirstName,
                              NOKMiddleName = c.Staff.NOKMiddleName,
                              NOKLastName = c.Staff.NOKLastName,
                              NOKRelation = c.Staff.NOKRelationship,
                              NOKAddress1 = c.Staff.Address1,
                              NOKCity = c.Staff.NOKCity,
                              NOKCountry = ComonnService.getCountryNameFromCode(c.Staff.NOKCountry),
                              NOKTelephone = ComonnService.getPhoneCodeFromCountry(c.Staff.Country) + c.Staff.NOKPhoneNumber,

                              EmpPosition = j == null ? "" : j.Position,
                              EmpSalary = j == null ? 0 : j.Salary,
                              EmpMode = j == null ? 0 : j.Mode,
                              DateOfEmployment = j == null ? "" : j.EmploymentDate.ToString("dd/MM/yyyy"),
                              DateOfLeaving = j == null ? "" : (j.LeavingDate == null ? "" : Convert.ToDateTime(j.LeavingDate).ToString("dd/MM/yyyy")),

                              LogInStartTime = c.LoginStartTime,
                              LogInStartDay = c.LoginStartDay ?? 0,
                              LogInEndTime = c.LoginEndTIme,
                              LogInEndDay = c.LoginEndDay ?? DayOfWeek.Sunday,
                              HolidaysEntitlement = c.Staff.StaffHolidaysEntitlemant,
                              HolidaysNoOfDays = c.Staff.StaffNoOFHolidays,

                              StaffFirstName = c.Staff.FirstName,
                              StaffMiddleName = c.Staff.MiddleName,
                              StaffLastName = c.Staff.LastName,
                              ResidencePermitUrl = c.Staff.ResidencePermitUrl,
                              Type = c.Staff.ResidencePermitType,
                              ExpiryDate = c.Staff.ResidencePermitExpiryDate,
                              PassportSide1Url = c.Staff.PassportSide1Url,
                              PassportSide2Url = c.Staff.PassportSide2Url,
                              UtilityBillUrl = c.Staff.UtilityBillUrl,
                              CVUrl = c.Staff.CVUrl,
                              HighestQualUrl = c.Staff.HighestQualificationUrl

                          }).FirstOrDefault();
            return result;
        }

        public DB.StaffInformation SaveStaffInformation(DB.StaffInformation staffInfo)
        {
            dbContext.StaffInformation.Add(staffInfo);
            dbContext.SaveChanges();
            SCity.Save(staffInfo.City, staffInfo.Country, DB.Module.Staff);

            return staffInfo;



        }

        public DB.StaffAddresses SaveStaffAddresses(DB.StaffAddresses staffAddress)
        {
            dbContext.StaffAddresses.Add(staffAddress);
            dbContext.SaveChanges();
            SCity.Save(staffAddress.StaffCity, staffAddress.StaffCountry, DB.Module.Staff);
            return staffAddress;
        }

        public DB.StaffEmployment saveEmploymentDetails(DB.StaffEmployment employmentData)
        {
            dbContext.StaffEmployment.Add(employmentData);
            dbContext.SaveChanges();
            return employmentData;
        }

        public string GetNewLoginCode(int length)
        {
            var random = new Random();
            string s = string.Empty;
            for (int i = 0; i < length; i++)
                s = String.Concat(s, random.Next(5).ToString());
            //return s;
            while (dbContext.StaffLogin.Where(x => x.LoginCode == s).Count() > 0)
            {
                for (int i = 0; i < length; i++)
                    s = String.Concat(s, random.Next(5).ToString());
            }
            return s;
        }

        public DB.StaffLogin AddStaffLogin(DB.StaffLogin staffLogin)
        {

            dbContext.StaffLogin.Add(staffLogin);
            dbContext.SaveChanges();
            return staffLogin;
        }

        public bool ChangeStatus(int staffId)
        {
            var data = dbContext.StaffLogin.Where(x => x.StaffId == staffId).FirstOrDefault();
            if (data != null)
            {
                if (data.IsActive == true)
                {
                    data.IsActive = false;
                }
                else if (data.IsActive == false)
                {
                    data.IsActive = true;
                }
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool deleteStaff(int staffId)
        {
            var data = dbContext.StaffLogin.Where(x => x.StaffId == staffId).FirstOrDefault();
            if (data != null)
            {
                data.IsDeleted = true;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool changeSystemLoginLevel(int staffId, int sysLevel)
        {
            var data = dbContext.StaffInformation.Where(x => x.Id == staffId).FirstOrDefault();
            if (data != null)
            {
                data.SytemLoginLevelOfStaff = (SystemLoginLevel)sysLevel;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }





        public bool ChangeMode(int staffId, int mode)
        {
            var data = dbContext.StaffEmployment.Where(x => x.StaffId == staffId).FirstOrDefault();
            if (data != null)
            {
                data.Mode = (ModeOfJob)mode;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ChangeEmploymentDate(int staffId, DateTime date)
        {
            var data = dbContext.StaffEmployment.Where(x => x.StaffId == staffId).FirstOrDefault();
            if (data != null)
            {
                data.EmploymentDate = date;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                data = new StaffEmployment();
                data.StaffId = staffId;
                data.EmploymentDate = date;
                dbContext.StaffEmployment.Add(data);
                dbContext.SaveChanges();

            }
            return false;
        }

        public bool ChangeLeavingDate(int staffId, DateTime date)
        {
            var data = dbContext.StaffEmployment.Where(x => x.StaffId == staffId).FirstOrDefault();
            if (data != null)
            {
                data.LeavingDate = date;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            else
            {
                data = new StaffEmployment();
                data.StaffId = staffId;
                data.LeavingDate = date;
                dbContext.StaffEmployment.Add(data);
                dbContext.SaveChanges();
            }
            return false;
        }

        public bool ChangeStartTime(int staffId, DateTime time)
        {
            var data = dbContext.StaffLogin.Where(x => x.StaffId == staffId).FirstOrDefault();
            if (data != null)
            {
                data.LoginStartTime = time.ToString("HH:mm");
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ChangeEndTime(int staffId, DateTime time)
        {
            var data = dbContext.StaffLogin.Where(x => x.StaffId == staffId).FirstOrDefault();
            if (data != null)
            {
                data.LoginEndTIme = time.ToString("HH:mm");
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ChangeLoginStartDay(int staffId, DayOfWeek day)
        {
            var data = dbContext.StaffLogin.Where(x => x.StaffId == staffId).FirstOrDefault();
            if (data != null)
            {
                data.LoginStartDay = day;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ChangeLoginEndDay(int staffId, DayOfWeek day)
        {
            var data = dbContext.StaffLogin.Where(x => x.StaffId == staffId).FirstOrDefault();
            if (data != null)
            {
                data.LoginEndDay = day;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ChangeHolidaysEntitlement(int staffId, StaffHolidaysEntiltlement value)
        {
            var data = dbContext.StaffInformation.Where(x => x.Id == staffId).FirstOrDefault();
            if (data != null)
            {
                data.StaffHolidaysEntitlemant = value;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ChangeNoOfHolidays(int staffId, int noOfDays)
        {
            var data = dbContext.StaffInformation.Where(x => x.Id == staffId).FirstOrDefault();
            if (data != null)
            {
                data.StaffNoOFHolidays = noOfDays;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool changePermitType(int staffId, string permit)
        {
            var data = dbContext.StaffInformation.Where(x => x.Id == staffId).FirstOrDefault();
            if (data != null)
            {
                data.ResidencePermitType = permit;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool changeExpiryDate(int staffId, DateTime date)
        {
            var data = dbContext.StaffInformation.Where(x => x.Id == staffId).FirstOrDefault();
            if (data != null)
            {
                data.ResidencePermitExpiryDate = date;
                dbContext.Entry(data).State = EntityState.Modified;
                dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool checkExistingEmail(string email)
        {
            email = email.ToLower();
            var data = dbContext.StaffInformation.Where(x => x.EmailAddress.ToLower() == email).FirstOrDefault();
            if (data == null)
            {
                return true;
            }
            return false;
        }


    }


}