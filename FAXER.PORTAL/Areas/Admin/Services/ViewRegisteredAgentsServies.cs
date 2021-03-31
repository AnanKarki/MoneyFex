using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class ViewRegisteredAgentsServies
    {
        FAXEREntities dbContext = null;
        Services.CommonServices CommonService = null;
        public ViewRegisteredAgentsServies()
        {
            dbContext = new FAXEREntities();
            CommonService = new CommonServices();
        }

        // get agent information by agent registration number 
        public ViewModels.ViewRegisteredAgentsViewModel getAgentInformation(string RegistrationNumber)
        {
            var data = dbContext.AgentInformation.Where(x => x.RegistrationNumber == RegistrationNumber).FirstOrDefault();
            if (data != null)
            {
                int AgentId = 0;
                AgentId = data.Id;

                var data1 = dbContext.AgentLogin.Where(x => x.AgentId == AgentId).FirstOrDefault();

                ViewRegisteredAgentsViewModel result = new ViewRegisteredAgentsViewModel()
                {
                    Id = data.Id,
                    Name = data.Name,
                    RegistrationNumber = data.RegistrationNumber,
                    PhoneNumber = data.PhoneNumber,
                    Address1 = data.Address1,
                    Address2 = data.Address2,
                    City = data.City,
                    Email = data.Email,
                    CountryCode = data.CountryCode,
                    Country = CommonService.getCountryNameFromCode(data.CountryCode),
                    CountryPhoneCode = CommonService.getPhoneCodeFromCountry(data.CountryCode),
                    ContactPerson = data.ContactPerson,
                    FaxNumber = data.FaxNumber,
                    PostalCode = data.PostalCode,
                    State = data.State,
                    Website = data.Website,
                    AccountNo = data.AccountNo,
                    AgentBusinessLicenseNumber = data.LicenseNumber,
                    ActivationCode = data1.ActivationCode.ToString(),
                    //    AgentTypePrincipal = data.AgentType == AgentTypePAndL.Principal ? true : false,
                    //    AgentTypeLocal = data.AgentType == AgentTypePAndL.Local ? true : false,
                    //    BusinessTypeEnum = data.BusinessType,
                };
                return result;
            }
            return null;

        }
        public ViewModels.ViewRegisteredAgentsViewModel ViewMoreAgentInformation(String RegistrationNumber)
        {

            var data = dbContext.AgentInformation.Where(x => x.RegistrationNumber == RegistrationNumber).FirstOrDefault();
            if (data != null)
            {
                int AgentId = 0;
                AgentId = data.Id;

                //var data2 = dbContext.AgentLogin.Where(x => x.AgentId == AgentId).FirstOrDefault();

                #region Bank Account Statment Details 

                var BankAccountStatmentDetails = BankAccountStatementDetails(AgentId);

                // Agent Bank Account Deposited Statement  List 
                var AgentBankAccountReceipts = (from c in dbContext.BaankAccountCreditUpdateByAgent.Where(x => x.AgentId == AgentId).OrderBy(x => x.CreatedDateTime).ToList()
                                                select new AgentBankAccountReceipt()
                                                {
                                                    Date = c.CreatedDateTime.ToString("dd/MM/yyyy"),
                                                    TransactionId = c.Id
                                                }).ToList();

                decimal CardUserNonCardWithdrawalAmount = dbContext.CardUserNonCardWithdrawal.Where(x => x.AgentId == AgentId).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
                decimal MerchantNonCardwithdrawalAmount = dbContext.MerchantNonCardWithdrawal.Where(x => x.AgentId == AgentId).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;

                decimal NonCardwithdrawalAmount = dbContext.ReceiverNonCardWithdrawl.Where(x => x.AgentId == AgentId).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
                decimal MFBCCardWithdrawalAmount = dbContext.MFBCCardWithdrawls.Where(x => x.AgentInformationId == AgentId).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;
                decimal MFTCCardWithdrawalAmount = dbContext.UserCardWithdrawl.Where(x => x.AgentInformationId == AgentId).Select(x => (decimal?)x.TransactionAmount).Sum() ?? 0;


                var TotalAmountWithdrawalFromAgent = CardUserNonCardWithdrawalAmount + MerchantNonCardwithdrawalAmount + NonCardwithdrawalAmount + MFBCCardWithdrawalAmount + MFTCCardWithdrawalAmount;
                #endregion
                ViewRegisteredAgentsViewModel vm = new ViewRegisteredAgentsViewModel()
                {
                    Id = AgentId,
                    AgentBusinessLicenseNumber = data.RegistrationNumber,
                    RegistrationNumber = data.RegistrationNumber,
                    //LoginFailedCount = data2.LoginFailedCount,
                    Website = data.Website,
                    ContactPerson = data.ContactPerson,
                    CurrentBankDeposit = BankAccountStatmentDetails.Select(x => x.BankDeposit).Sum(),
                    CurrentCustomerDepositFees = BankAccountStatmentDetails.Select(x => x.CustomerDepositFees).Sum(),
                    CurrentCustomerDeposit = BankAccountStatmentDetails.Select(x => x.CustomerDeposit).Sum(),
                    TotalDeposit = (BankAccountStatmentDetails.Select(x => x.BankDeposit).Sum()
                                    + BankAccountStatmentDetails.Select(x => x.CustomerDeposit).Sum()
                                    + BankAccountStatmentDetails.Select(x => x.CustomerDepositFees).Sum())
                                    - TotalAmountWithdrawalFromAgent,
                    LatestDepositedDateTime = (BankAccountStatmentDetails.Select(x => x.CreatedDateTime).LastOrDefault()).ToString("dd/MM/yyyy"),
                    NameOfLatestDepositer = BankAccountStatmentDetails.Select(x => x.NameOfUpdater).LastOrDefault(),
                    AgentBankAccountReceipts = AgentBankAccountReceipts
                };
                return vm;
            }
            else
            {
                return null;
            }


        }

        public List<DB.BaankAccountCreditUpdateByAgent> BankAccountStatementDetails(int AgentId)
        {

            var result = dbContext.BaankAccountCreditUpdateByAgent.Where(x => x.AgentId == AgentId).ToList();

            return result;
        }
        /// <summary>
        /// Add or update agent Information  
        /// </summary>
        /// <param name="agent"></param>
        /// <returns>Boolean Value </returns>
        public bool AddOrUpdateAgentInformtaion(ViewRegisteredAgentsViewModel agent)
        {

            var agentType = AgentTypePAndL.Non;
            if (agent.AgentTypePrincipal == true)
            {

                agentType = AgentTypePAndL.Principal;
            }
            else if (agent.AgentTypeLocal == true)
            {


                agentType = AgentTypePAndL.Local;
            }
            // var data = dbContext.BecomeAnAgent.Where(x => x.Id == agent.Id).FirstOrDefault();
            var data = dbContext.AgentInformation.Where(x => x.Id == agent.Id).FirstOrDefault();
            if (data != null)
            {
                // set new agent information
                data.Name = agent.Name;
                //data.RegistrationNumber = agent.RegistrationNumber;
                data.PhoneNumber = agent.PhoneNumber;
                data.Address1 = agent.Address1;
                data.Address2 = agent.Address2;
                data.City = agent.City;
                data.Email = agent.Email;
                data.CountryCode = agent.CountryCode;
                data.ContactPerson = agent.ContactPerson;
                data.FaxNumber = agent.FaxNumber;
                data.PostalCode = agent.PostalCode;
                data.State = agent.State;
                data.Website = agent.Website;
                //data.AgentType = agentType;
                //data.BusinessType = agent.BusinessTypeEnum;
                // upate agent information by id
                dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
                int result = dbContext.SaveChanges();
                SCity.Save(data.City, data.CountryCode, DB.Module.Agent);

                if (result == 1)
                {
                    return true;
                }

            }


            return false;
        }
        public bool checkExistingEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                email = email.ToLower();
                //var data = dbContext.BecomeAnAgent.Where(x => x.BusinessEmailAddress.ToLower() == email).FirstOrDefault();
                var data = dbContext.AgentInformation.Where(x => x.Email.ToLower() == email).FirstOrDefault();

                if (data == null)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Get List Of Agent Information 
        /// </summary>
        /// <returns>Agent List</returns>
        public List<ViewModels.ViewRegisteredAgentsViewModel> getAgentInformationList()
        {
            // get list of agent information 
            var result = (from c in dbContext.AgentInformation.Where(x => x.IsDeleted == false).ToList()
                          join d in dbContext.AgentLogin on c.Id equals d.AgentId
                          into j
                          from joined in j.DefaultIfEmpty()
                          select new ViewRegisteredAgentsViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name,
                              AgentBusinessLicenseNumber = c.RegistrationNumber,
                              PhoneNumber = c.PhoneNumber,
                              Address1 = c.Address1,
                              Address2 = c.Address2,
                              City = c.City,
                              Email = c.Email,
                              ContactPerson = c.ContactPerson,
                              FaxNumber = c.FaxNumber,
                              PostalCode = c.PostalCode,
                              State = c.State,
                              Website = c.Website,
                              AccountNo = c.AccountNo,
                              RegistrationNumber = c.RegistrationNumber,
                              AgentStatus = joined == null ? true : joined.IsActive

                          }).ToList();

            return result;
        }
        // update Agent Status
        public bool AgentStaus(int agent)
        {
            var data = dbContext.AgentLogin.Where(x => x.AgentId == agent).FirstOrDefault();
            // If Activate
            if (data.IsActive == false)
            {

                data.IsActive = true;
            }
            // if deactivate
            else
            {
                data.IsActive = false;
            }
            // update agent status
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();
            return true;
        }
        //Delte Agent By id 
        public bool DeleteAgent(int agentid)
        {
            var data = dbContext.AgentInformation.Where(x => x.Id == agentid).FirstOrDefault();
            data.IsDeleted = true;
            dbContext.Entry(data).State = System.Data.Entity.EntityState.Modified;
            dbContext.SaveChanges();

            return true;
        }

        public bool UpdateAgentInformation(ViewRegisteredAgentsViewModel model)
        {
            if (model != null)
            {

                var agentType = AgentTypePAndL.Non;
                if (model.AgentTypePrincipal == true)
                {

                    agentType = AgentTypePAndL.Principal;
                }
                else if (model.AgentTypeLocal == true)
                {


                    agentType = AgentTypePAndL.Local;
                }
                // var data1 = dbContext.BecomeAnAgent.Where(x => x.AgentRegistrationCode == model.RegistrationNumber).FirstOrDefault();
                var data2 = dbContext.AgentInformation.Where(x => x.RegistrationNumber == model.RegistrationNumber).FirstOrDefault();
                if (/*data1 == null ||*/ data2 == null)
                {
                    return false;
                }
                //updating info in BecomeAnAgent
                //data1.ContactName = model.ContactPerson;
                //data1.Address1 = model.Address1;
                //data1.Address2 = model.Address2;
                //data1.City = model.City;
                //data1.StateProvince = model.State;
                //data1.PostZipCode = model.PostalCode;
                //data1.ContactPhone = model.PhoneNumber;
                //data1.FaxNo = model.FaxNumber;
                //data1.Website = model.Website;
                //data1.AgentType = agentType;
                //data1.BusinessType = model.BusinessTypeEnum;
                //dbContext.Entry(data1).State = System.Data.Entity.EntityState.Modified;
                //dbContext.SaveChanges();

                //updating in AgentInformation
                data2.ContactPerson = model.ContactPerson;
                data2.Address1 = model.Address1;
                data2.Address2 = model.Address2;
                data2.City = model.City;
                data2.State = model.State;
                data2.PostalCode = model.PostalCode;
                data2.PhoneNumber = model.PhoneNumber;
                data2.FaxNumber = model.FaxNumber;
                data2.Website = model.Website;
                dbContext.Entry(data2).State = System.Data.Entity.EntityState.Modified;
                dbContext.SaveChanges();

                SCity.Save(data2.City, data2.CountryCode, DB.Module.Agent);
                //  SCity.Save(data1.City, data1.CountryCode, DB.Module.Agent);

                //sending email to the agent about information update
                MailCommon mail = new MailCommon();
                string body = "Dear " + data2.Name + ", Your account information has been successfully updated !";
                mail.SendMail(data2.Email, "Account Information Updated !", body);

                return true;
            }
            return false;
        }


        public List<AgentInformation> List()
        {
            var data = dbContext.AgentInformation.ToList();
            return data;
        }
        public List<AgentLogin> ListOfActiveAgents()
        {
            var data = dbContext.AgentLogin.Where(x => x.IsActive == true).ToList();
            return data;
        }
        public List<AgentLogin> ListOfInActiveAgents()
        {
            var data = dbContext.AgentLogin.Where(x => x.IsActive == false).ToList();
            return data;
        }

        public List<ViewModels.ViewRegisteredAgentsViewModel> getFilterAgentList(string CountryCode = "", string City = "", string searchAgent = "")
        {
            #region old
            //var data = new List<DB.AgentInformation>();

            //if (!string.IsNullOrEmpty(CountryCode) && string.IsNullOrEmpty(City))
            //{
            //    data = dbContext.AgentInformation.Where(x => (x.CountryCode == CountryCode) && (x.IsDeleted == false)).ToList();
            //}
            //else if (string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            //{

            //    data = dbContext.AgentInformation.Where(x => (x.City.ToLower() == City.ToLower()) && (x.IsDeleted == false)).ToList();
            //}
            //else if (!string.IsNullOrEmpty(CountryCode) && !string.IsNullOrEmpty(City))
            //{
            //    data = dbContext.AgentInformation.Where(x => (x.City.ToLower() == City.ToLower()) && (x.IsDeleted == false) && (x.CountryCode == CountryCode)).ToList();
            //}
            //else
            //{

            //    data = dbContext.AgentInformation.Where(x => x.IsDeleted == false).ToList();
            //}
            #endregion

            var data = List().Where(x => x.IsDeleted == false);

            if (!string.IsNullOrEmpty(CountryCode))
            {
                data = data.Where(x => x.CountryCode == CountryCode).ToList();
            }

            if (!string.IsNullOrEmpty(City))
            {
                data = data.Where(x => x.City == City).ToList();
            }
            if (!string.IsNullOrEmpty(searchAgent))
            {
                data = data.Where(x => x.Name.ToLower().StartsWith(searchAgent.ToLower())).ToList();
            }
            // get list of agent information 
            var result = (from c in data
                          join d in dbContext.AgentLogin on c.Id equals d.AgentId
                          into j
                          from joined in j.DefaultIfEmpty()
                          select new ViewRegisteredAgentsViewModel()
                          {
                              Id = c.Id,
                              Name = c.Name,
                              AgentBusinessLicenseNumber = c.RegistrationNumber,
                              PhoneNumber = CommonService.getPhoneCodeFromCountry(c.CountryCode) + c.PhoneNumber,
                              Address1 = c.Address1,
                              Address2 = c.Address2,
                              City = c.City,
                              Country = CommonService.getCountryNameFromCode(c.CountryCode),
                              Email = c.Email,
                              ContactPerson = c.ContactPerson,
                              FaxNumber = CommonService.getPhoneCodeFromCountry(c.CountryCode) + c.FaxNumber,
                              PostalCode = c.PostalCode,
                              State = c.State,
                              Website = c.Website,
                              AccountNo = c.AccountNo,
                              RegistrationNumber = c.RegistrationNumber,
                              AgentStatus = joined == null ? true : joined.IsActive,
                              Logincode = joined == null ? "" : joined.LoginCode,
                              //AgentType = Enum.GetName(typeof(AgentTypePAndL), dbContext.BecomeAnAgent.Where(x => x.AgentRegistrationCode == c.RegistrationNumber).Select(x => x.AgentType).FirstOrDefault()),
                              //BusinessType = Common.Common.GetEnumDescription((BusinessType)dbContext.BecomeAnAgent.Where(x => x.AgentRegistrationCode == c.RegistrationNumber).Select(x => x.BusinessType).FirstOrDefault()),
                          }).ToList();

            return result;
        }

        public bool AddNewAgentNote(AgentNote model)
        {

            dbContext.AgentNote.Add(model);
            dbContext.SaveChanges();

            return true;

        }

        public List<AgentNoteViewModel> GetAgentNotes(int AgentId)
        {

            var result = (from c in dbContext.AgentNote.Where(x => x.AgentId == AgentId).OrderByDescending(x => x.CreatedDateTime).ToList()
                          select new AgentNoteViewModel()
                          {

                              Note = c.Note,
                              Date = c.CreatedDateTime.ToString("dd/MM/yyyy"),
                              Time = c.CreatedDateTime.ToString("HH:mm"),
                              StaffName = c.CreatedByStaffName

                          }).ToList();

            return result;
        }
    }
}