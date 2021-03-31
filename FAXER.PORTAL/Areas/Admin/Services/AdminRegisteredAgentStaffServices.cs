using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AdminRegisteredAgentStaffServices
    {
        DB.FAXEREntities dbContext = null;
        AgentCommonServices _agentCommonServices = null;
        public AdminRegisteredAgentStaffServices()
        {
            dbContext = new DB.FAXEREntities();
            _agentCommonServices = new AgentCommonServices();
        }

        internal void Add(AdminRegisteredAgentStaffViewModel vm)
        {
            Common.Common.GenerateRandomDigit(10);
            AdminRegisteredAgentStaff model = new AdminRegisteredAgentStaff()
            {
                AccountNumber = GeneratedAccountNo(),
                Address = vm.Address,
                BranchName = vm.BranchName,
                EmailAddress = vm.EmailAddress,
                Password = Common.Common.Encrypt(Common.Common.GenerateRandomString(8)),
                StaffName = vm.StaffName,
                TelePhoneNumber = vm.TelePhoneNumber,

            };

            dbContext.AdminRegisteredAgentStaff.Add(model);
            dbContext.SaveChanges();

            // Agent Staff To Admin
            var agentStaffmodel = Common.AdminSession.AgentStaffInfoAndLoginViewModel;
            agentStaffmodel.AgenAccountNo = model.AccountNumber;
            AddStaffInformationAndLogin(agentStaffmodel);

        }


        internal void ActivateDeactivate(string accountNumber)
        {
            var agentstaffInfo = dbContext.AgentStaffInformation.Where(x => x.AgentMFSCode == accountNumber).Select(x => x.Id).FirstOrDefault();
            var model = dbContext.AgentStaffLogin.Where(x => x.AgentStaffId == agentstaffInfo).FirstOrDefault();
            if (model.IsActive)
            {
                model.IsActive = false;
            }
            else
            {
                model.IsActive = true;
            }
            dbContext.Entry<AgentStaffLogin>(model).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public List<AdminRegisteredAgentStaffViewModel> GetAgentAddedAgentStaffByAdmin()
        {

            var data = dbContext.AdminRegisteredAgentStaff.ToList();
            List<AdminRegisteredAgentStaffViewModel> model = (from c in data
                                                              select new AdminRegisteredAgentStaffViewModel()
                                                              {
                                                                  AccountNumber = c.AccountNumber,
                                                                  BranchName = c.BranchName,
                                                                  Address = c.Address,
                                                                  EmailAddress = c.EmailAddress,
                                                                  StaffName = c.StaffName,
                                                                  TelePhoneNumber = c.TelePhoneNumber,
                                                                  Id = c.Id
                                                              }).ToList();
            return model;

        }

        internal void DeleteAgentStaff(int AgentSatffId)
        {
            var agentstaffInfo = dbContext.AgentStaffInformation.Where(x => x.Id == AgentSatffId).FirstOrDefault();
            agentstaffInfo.IsDeleted = true;

            dbContext.Entry<AgentStaffInformation>(agentstaffInfo).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public List<AdminRegisteredAgentStaffInfoViewModel> GetAgentRegistered(string Country = "")
        {
            var data = dbContext.AdminRegisteredAgentStaff.ToList();
            var agentStaffInfo = dbContext.AgentStaffInformation.Where(x => x.IsDeleted == false).ToList();
            if (!string.IsNullOrEmpty(Country))
            {
                agentStaffInfo = agentStaffInfo.Where(x => x.Country == Country).ToList();
            }


            var result = (from c in agentStaffInfo
                          join e in dbContext.AgentStaffLogin on c.Id equals e.AgentStaffId
                          join f in dbContext.AgentInformation on c.AgentId equals f.Id
                          select new AdminRegisteredAgentStaffInfoViewModel()
                          {
                              Id = c.Id,
                              AgentCode = e.AgencyLoginCode,
                              StaffCode = e.StaffLoginCode,
                              AgentName = f.Name,
                              Country = Common.Common.GetCountryName(c.Country),
                              StaffName = e.AgentStaff.FirstName + " " + e.AgentStaff.MiddleName + " " + e.AgentStaff.LastName,
                              Status = e.IsActive == true ? "Active" : "InActive",
                              AccountNumber = c.AgentMFSCode,
                              CountryFlag = c.Country.ToLower(),
                              AgentId = c.AgentId

                          }).ToList();
            return result;
        }


        public AdminRegisteredAgentStaffInfoViewModel GetAgentDetails(string AccountNumber = "")
        {

            var agentStaffInfo = dbContext.AgentStaffInformation.Where(x => x.AgentMFSCode == AccountNumber).ToList();

            var result = (from c in agentStaffInfo
                          join e in dbContext.AgentStaffLogin on c.Id equals e.AgentStaffId
                          join f in dbContext.AgentInformation on c.AgentId equals f.Id
                          select new AdminRegisteredAgentStaffInfoViewModel()
                          {
                              Id = c.Id,
                              AgentCode = e.AgencyLoginCode,
                              StaffCode = e.StaffLoginCode,
                              AgentName = f.Name,
                              Country = Common.Common.GetCountryName(c.Country),
                              StaffName = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                              Status = e.IsActive == true ? "Active" : "InActive",
                              AccountNumber = c.AgentMFSCode,
                              Address = c.Address1,
                              Branch = " ",
                              DateOfBirth = c.DateOfBirth.Date,
                              PhoneNumber = c.PhoneNumber,
                              EmailAddress = c.EmailAddress,
                              IdCardNumber = c.IdCardNumber,
                              IdCardType = c.IdCardType,
                              IdIssuingCountry = Common.Common.GetCountryName(c.IssuingCountry),
                              Day = (int)c.IdCardExpiryDate.Day,
                              Month = (Month)c.IdCardExpiryDate.Month,
                              Year = (int)c.IdCardExpiryDate.Year,
                              AgentStaffId = c.Id,
                              AgentType = c.AgentStaffType.ToString()

                          }).FirstOrDefault();
            return result;
        }

        private string GeneratedAccountNo()
        {
            string AccountNo = Common.Common.GenerateRandomDigit(10);

            while (dbContext.AgentStaffInformation.Where(x => x.AgentMFSCode == AccountNo).Count() > 0)
            {
                AccountNo = Common.Common.GenerateRandomDigit(10);
            }

            return AccountNo;
        }

        internal void AddStaffInformationAndLogin(AgentStaffInfoAndLoginViewModel vm)
        {

            var adminRegisterd = dbContext.AdminRegisteredAgentStaff.Where(x => x.AccountNumber == vm.AgenAccountNo).FirstOrDefault();

            if (adminRegisterd != null)
            {

                var name = adminRegisterd.StaffName.Split(' ');
                string FirstName = name[0];
                string MiddleName = "";
                string LastName = "";
                if (name.Length > 2)
                {
                    MiddleName = name[1];
                    LastName = name[2];
                }
                else if (name.Length == 2)
                {
                    LastName = name[1];
                }

                //int agentStaffId = AgentStaffId(vm.AgentId);
                DB.AgentStaffInformation agentStaffInformation = new DB.AgentStaffInformation()
                {
                    FirstName = FirstName,
                    MiddleName = MiddleName,
                    LastName = LastName,
                    Country = vm.Country,
                    EmailAddress = adminRegisterd.EmailAddress,
                    AgentStaffType = StaffType.Transaction,
                    CreatedDate = DateTime.Now,
                    Address1 = vm.Address,
                    AgentId = vm.AgentId,
                    CreatedBy = vm.AgentId,
                    AgentMFSCode = vm.AgenAccountNo,
                    DateOfBirth = DateTime.Now,
                    IdCardExpiryDate = DateTime.Now


                };
                var agentStaffInfo = dbContext.AgentStaffInformation.Add(agentStaffInformation);
                dbContext.SaveChanges();


                AgentStaffLogin agentStaffLogin = new AgentStaffLogin()
                {
                    ActivationCode = Guid.NewGuid().ToString(),
                    AgencyLoginCode = _agentCommonServices.GetAgencyLoginCode(vm.AgentId),
                    AgentStaffId = agentStaffInfo.Id,
                    IsActive = false,
                    LoginFailedCount = 0,
                    Password = adminRegisterd.Password,
                    StaffLoginCode = _agentCommonServices.GetStaffLoginCode(),
                    Username = adminRegisterd.EmailAddress,
                    StartTime = "09:00",
                    EndTime = "17:00",
                    StartDay = DayOfWeek.Monday,
                    EndDay = DayOfWeek.Saturday,
                    IsFirstLogin = true

                };
                dbContext.AgentStaffLogin.Add(agentStaffLogin);
                dbContext.SaveChanges();

                #region send mail to admin as activation code

                MailCommon mail = new MailCommon();

                var baseUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);

                string body = Common.Common.GetTemplate(baseUrl + "/EmailTemplate/AgentActivationEmail?NameOfContactPerson=" + agentStaffInfo.FirstName
                                                  + " " + agentStaffInfo.MiddleName + " " + agentStaffInfo.LastName
                                                  + "&EmailAddress=" + agentStaffInfo.EmailAddress
                                                  + "&AgencyLoginCode=" + agentStaffLogin.AgencyLoginCode
                                                  + "&StaffLoginCode=" + agentStaffLogin.StaffLoginCode
                                                  + "&NameOfBusiness=" + " "
                                                  + "&Password=" + adminRegisterd.Password.Decrypt()
                                                  + "&IsRegisteredByAdmin=" + true);


                mail.SendMail(agentStaffInformation.EmailAddress, "Agent Registration Confirmation", body);

                #endregion


            }
        }

        internal void UpdateAgentStaffInfo(AdminRegisteredAgentStaffInfoViewModel vm)
        {
            var AgentStaffInfo = dbContext.AgentStaffInformation.Where(x => x.Id == vm.AgentStaffId).FirstOrDefault();


            var name = vm.AgentName.Split(' ');
            string FirstName = name[0];
            string MiddleName = "";
            string LastName = "";
            if (name.Length > 2)
            {
                MiddleName = name[1];
                LastName = name[2];
            }
            else if (name.Length == 2)
            {
                LastName = name[1];
            }

            AgentStaffInfo.FirstName = FirstName;
            AgentStaffInfo.MiddleName = MiddleName;
            AgentStaffInfo.LastName = LastName;
            AgentStaffInfo.IssuingCountry = vm.IdIssuingCountry;
            AgentStaffInfo.PhoneNumber = vm.PhoneNumber;
            AgentStaffInfo.Address1 = vm.Address;
            AgentStaffInfo.EmailAddress = vm.EmailAddress;
            AgentStaffInfo.UpdatedDate = DateTime.Now;
            AgentStaffInfo.IdCardNumber = vm.IdCardNumber;
            AgentStaffInfo.IdCardExpiryDate = vm.IdExpiryDate;
            AgentStaffInfo.IdCardType = vm.IdCardType;

            dbContext.Entry<AgentStaffInformation>(AgentStaffInfo).State = EntityState.Modified;
            dbContext.SaveChanges();

        }

        public bool CheckIfAccountNoIsvalid(string agenAccountNo)
        {
            var adminRegisterd = dbContext.AdminRegisteredAgentStaff.Where(x => x.AccountNumber == agenAccountNo).FirstOrDefault();
            if (adminRegisterd != null)
            {
                return true;
            }
            return false;
        }
        public bool CheckIfAgentStaffIsCreated(string agenAccountNo)
        {
            var AgentStaff = dbContext.AgentStaffInformation.Where(x => x.AgentMFSCode == agenAccountNo).FirstOrDefault();
            if (AgentStaff != null)
            {
                return true;
            }
            return false;
        }
        public bool CheckIfMobileNoExist(string MobileNo)
        {
            var AgentStaff = dbContext.AgentStaffInformation.Where(x => x.PhoneNumber == MobileNo).FirstOrDefault();
            if (AgentStaff != null)
            {
                return true;
            }
            return false;
        }
        public bool CheckIfEmailExist(string EmailAddress)
        {
            var AgentStaff = dbContext.AgentStaffInformation.Where(x => x.EmailAddress == EmailAddress).FirstOrDefault();
            if (AgentStaff != null)
            {
                return true;
            }
            return false;
        }

        public int AgentStaffId(int agentId)
        {
            int AgentStaffId = dbContext.AgentStaffInformation.Where(x => x.AgentId == agentId).Select(x => x.Id).FirstOrDefault();
            return AgentStaffId;
        }

        public string AgentStaffLoginCode(int AgentStaffId)
        {
            string LoginCode = dbContext.AgentStaffLogin.Where(x => x.AgentStaffId == AgentStaffId).Select(x => x.AgencyLoginCode).FirstOrDefault();
            return LoginCode;
        }

        public AgentStaffInformation AgentStaffInfo(int AgentStaffId)
        {
            var data = dbContext.AgentStaffInformation.Where(x => x.Id == AgentStaffId).FirstOrDefault();
            return data;
        }
    }
}