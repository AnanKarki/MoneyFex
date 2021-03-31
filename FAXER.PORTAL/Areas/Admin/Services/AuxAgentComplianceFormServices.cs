using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Twilio.Rest.Trunking.V1;

namespace FAXER.PORTAL.Areas.Admin.Services
{
    public class AuxAgentComplianceFormServices
    {

        DB.FAXEREntities db = null;
        CommonServices _commonServices = null;
        public AuxAgentComplianceFormServices()
        {
            db = new DB.FAXEREntities();
            _commonServices = new CommonServices();

        }
        public List<AgentInformation> GetAuxAgentInformation()
        {
            var list = db.AgentInformation.Where(x => x.IsAUXAgent == true).ToList();
            return list;
        }
        public AgentStaffLogin DeactivatedAgentStaff(int id, bool IsActive)
        {
            var list = db.AgentStaffLogin.Where(x => x.AgentStaffId == id).FirstOrDefault();
            if (IsActive == true)
            {

                list.IsActive = false;

            }
            else
            {
                list.IsActive = true;

            }
            var updatedlist = db.Entry<AgentStaffLogin>(list).State = EntityState.Modified;
            db.SaveChanges();
            return list;


        }

        public bool DeletedAgentInfo(int id)
        {
            var list = db.AgentStaffInformation.Where(x => x.Id == id).FirstOrDefault();
            if (list != null)
            {
                list.IsDeleted = true;
                var updatedlist = db.Entry<AgentStaffInformation>(list).State = EntityState.Modified;
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }



        }
        public AgentStaffInformationViewModel getAgentStaffInformation(string agentStaffId = "")
        {

            var list = db.AgentStaffInformation.Where(x => x.AgentMFSCode == agentStaffId).ToList();
            if (list.Count() > 0)
            {

                var result = (from c in list
                              join d in db.AgentStaffLogin on c.Id equals d.AgentStaffId

                              select new AgentStaffInformationViewModel
                              {
                                  Id = c.Id,
                                  DateOfBirth = c.DateOfBirth,
                                  IdCardExpiryDate = c.IdCardExpiryDate,
                                  Address1 = c.Address1,
                                  Country = _commonServices.getCountryNameFromCode(c.Country),
                                  CountryCode = c.Country,
                                  EmailAddress = c.EmailAddress,
                                  Id1 = c.Id1,
                                  Id2 = c.Id2,
                                  Id3 = c.Id3,
                                  IdCardNumber = c.IdCardNumber,
                                  IdCardType = c.IdCardType,
                                  IssuingCountry = _commonServices.getCountryNameFromCode(c.IssuingCountry),
                                  IssuingCountryCode = c.IssuingCountry,
                                  Name = c.FirstName + " " + c.MiddleName + " " + c.LastName,
                                  PhoneNumber = c.PhoneNumber,
                                  IsActive = d.IsActive,
                                  StaffLoginCode = d.StaffLoginCode,
                                  StaffId = c.AgentMFSCode,
                                  ExpiryDay = c.IdCardExpiryDate.Day,
                                  ExpiryMonth = (Month)c.IdCardExpiryDate.Month,
                                  ExpiryYear = c.IdCardExpiryDate.Year,
                              }).FirstOrDefault();
                return result;
            }

            else
            {
                return null;
            }

        }

        public AgentStaffInformation UpdateAgentStaffInformation(AgentStaffInformationViewModel model)
        {

            string[] name = model.Name.Split(' ');
            string firstname = "";
            string middleName = "";
            string lastName = "";
            if (name.Count() == 1)
            {
                firstname = name[0];

            }
            if (name.Count() == 2)
            {
                firstname = name[0];
                lastName = name[1];
            }
            if (name.Count() == 3)
            {
                firstname = name[0];
                middleName = name[1];
                lastName = name[2];
            }
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            var agentStaff = db.AgentStaffInformation.Where(x => x.Id == model.Id).FirstOrDefault();

           
            agentStaff.Id1 = model.Id1;
            agentStaff.DateOfBirth = model.DateOfBirth;
            agentStaff.IdCardExpiryDate = model.IdCardExpiryDate;
            agentStaff.Id2 = model.Id2;
            agentStaff.Id3 = model.Id3;
            agentStaff.Address1 = model.Address1;
            agentStaff.BirthCountry = model.CountryCode;
            agentStaff.IssuingCountry = model.IssuingCountryCode;
            agentStaff.EmailAddress = model.EmailAddress;
            agentStaff.FirstName = firstname;
            agentStaff.MiddleName = middleName;
            agentStaff.LastName = lastName;
            agentStaff.IdCardNumber = model.IdCardNumber;
            agentStaff.IdCardType = model.IdCardType;
            agentStaff.PhoneNumber = model.PhoneNumber;
            agentStaff.UpdatedDate = DateTime.Now;
            agentStaff.UpdatedBy = StaffId;

            db.Entry<AgentStaffInformation>(agentStaff).State = EntityState.Modified;
            db.SaveChanges();
            return agentStaff;
        }

        public List<AuxAgentComplianceFormViewModel> GetAuxAgentFormsList(int AgentID)
        {

            var SARFormList = (from c in db.SARForm.Where(x => x.AgentId == AgentID).ToList()
                               join d in db.AgentInformation on c.AgentId equals d.Id
                               join e in db.AgentStaffInformation on d.Id equals e.AgentId

                               select new AuxAgentComplianceFormViewModel()
                               {
                                   Id = c.Id,
                                   ActionDate = c.ApprovedDate.ToFormatedString(),
                                   SubDate = c.SubmittedDate.ToString("MMM-dd-yyyy"),
                                   AgentName = d.Name,
                                   AgentStaffId = e.AgentMFSCode,
                                   Country = _commonServices.getCountryNameFromCode(d.CountryCode),
                                   CountryCode = d.CountryCode,
                                   Form = "SAR",
                                   FormId = 1,
                                   FormAction = Enum.GetName(typeof(FormStatus), c.FormAction),
                                   StaffId = c.StaffAccountNo,
                                   AgentId = c.AgentId,
                               }).GroupBy(x => x.AgentId).Select(x => x.FirstOrDefault()).ToList();


            var ThirdPartyMoneyTransfer = (from c in db.ThirdPartyMoneyTransfer.Where(x => x.AgentId == AgentID).ToList()
                                           join d in db.AgentInformation on c.AgentId equals d.Id
                                           join e in db.AgentStaffInformation on d.Id equals e.AgentId
                                           select new AuxAgentComplianceFormViewModel()
                                           {
                                               Id = c.Id,
                                               ActionDate = c.ApprovedDate.ToFormatedString(),
                                               SubDate = c.SubmittedDate.ToString("MMM-dd-yyyy"),
                                               AgentName = d.Name,
                                               AgentStaffId = e.AgentMFSCode,
                                               Country = _commonServices.getCountryNameFromCode(d.CountryCode),
                                               CountryCode = d.CountryCode,
                                               Form = "Third Party Transfer",
                                               FormId = 5,
                                               FormAction = Enum.GetName(typeof(FormStatus), c.FormAction),
                                               StaffId = c.ApprovedStaffAccountNo,
                                               AgentId = c.AgentId,
                                           }).GroupBy(x => x.AgentId).Select(x => x.FirstOrDefault()).ToList();
            var LargeFundMoneyTransferFormList = (from c in db.LargeFundMoneyTransferFormData.Where(x => x.AgentId == AgentID).ToList()
                                                  join d in db.AgentInformation on c.AgentId equals d.Id
                                                  join e in db.AgentStaffInformation on d.Id equals e.AgentId
                                                  select new AuxAgentComplianceFormViewModel()
                                                  {
                                                      Id = c.Id,
                                                      ActionDate = c.ApprovedDate.ToFormatedString(),
                                                      SubDate = c.SubmittedDate.ToString("MMM-dd-yyyy"),
                                                      AgentName = d.Name,
                                                      AgentStaffId = e.AgentMFSCode,
                                                      Country = _commonServices.getCountryNameFromCode(d.CountryCode),
                                                      CountryCode = d.CountryCode,
                                                      Form = "Large Funds",
                                                      FormId = 2,
                                                      FormAction = Enum.GetName(typeof(FormStatus), c.FormAction),
                                                      StaffId = c.ApprovedStaffAccountNo,
                                                      AgentId = c.AgentId,
                                                  }).GroupBy(x => x.AgentId).Select(x => x.FirstOrDefault()).ToList();
            var AgentAMLTrainingRecordMasterList = (from c in db.AgentAMLTrainingRecordMaster.Where(x => x.AgentId == AgentID).ToList()
                                                    join d in db.AgentInformation on c.AgentId equals d.Id
                                                    join e in db.AgentStaffInformation on d.Id equals e.AgentId
                                                    select new AuxAgentComplianceFormViewModel()
                                                    {
                                                        Id = c.Id,
                                                        ActionDate = c.ApprovedDate.ToFormatedString(),
                                                        SubDate = c.SubmittedDate.ToString("MMM-dd-yyyy"),
                                                        AgentName = d.Name,
                                                        AgentStaffId = e.AgentMFSCode,
                                                        Country = _commonServices.getCountryNameFromCode(d.CountryCode),
                                                        CountryCode = d.CountryCode,
                                                        Form = "AML Trainig",
                                                        FormId = 4,
                                                        FormAction = Enum.GetName(typeof(FormStatus), c.FormAction),
                                                        StaffId = c.ApprovedStaffAccountNo,
                                                        AgentId = c.AgentId,
                                                    }).GroupBy(x => x.AgentId).Select(x => x.FirstOrDefault()).ToList();

            var SourceOfFundDeclarationFormList = (from c in db.SourceOfFundDeclarationFormData.Where(x => x.AgentId == AgentID).ToList()
                                                   join d in db.AgentInformation on c.AgentId equals d.Id
                                                   join e in db.AgentStaffInformation on d.Id equals e.AgentId
                                                   select new AuxAgentComplianceFormViewModel()
                                                   {
                                                       Id = c.Id,
                                                       ActionDate = c.ApprovedDate.ToFormatedString(),
                                                       SubDate = c.SubmittedDate.ToString("MMM-dd-yyyy"),
                                                       AgentName = d.Name,
                                                       AgentStaffId = e.AgentMFSCode,
                                                       Country = _commonServices.getCountryNameFromCode(d.CountryCode),
                                                       CountryCode = d.CountryCode,
                                                       Form = "Finding Source",
                                                       FormId = 3,
                                                       FormAction = Enum.GetName(typeof(FormStatus), c.FormAction),
                                                       StaffId = c.ApprovedStaffAccountNo,
                                                       AgentId = c.AgentId,
                                                   }).GroupBy(x => x.AgentId).Select(x=>x.FirstOrDefault()).ToList();




            var finalList = SARFormList.Concat(ThirdPartyMoneyTransfer).Concat(LargeFundMoneyTransferFormList)
                .Concat(AgentAMLTrainingRecordMasterList).Concat(SourceOfFundDeclarationFormList).ToList();


            return finalList;

        }
    }
}