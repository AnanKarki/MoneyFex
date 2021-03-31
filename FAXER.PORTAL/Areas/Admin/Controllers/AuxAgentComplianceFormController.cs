using FAXER.PORTAL.Areas.Admin.Services;
using FAXER.PORTAL.Areas.Admin.ViewModels;
using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Areas.CardUsers.ViewModels;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Validation;
using System.Web.Mvc;
using Twilio.Rest.Trunking.V1;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AuxAgentComplianceFormController : Controller
    {
        // GET: Admin/AuxAgentComplianceForm
        CommonServices _commonServices = null;
        AgentStaffRegistrationServices _agentStaffInformation = null;
        AuxAgentComplianceFormServices _services = null;
        public AuxAgentComplianceFormController()
        {
            _commonServices = new CommonServices();
            _agentStaffInformation = new AgentStaffRegistrationServices();
            _services = new AuxAgentComplianceFormServices();
        }
     
        public ActionResult Index(string SendingCountry = "", int AgentId = 0, string Date = "", int FormId = 0, string AgentStaffId = "", string staffId = ""
            , int? page = null)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var sendingcountries = _commonServices.GetCountries();
            ViewBag.SendingCountries = new SelectList(sendingcountries, "Code", "Name");

            var agents = _commonServices.GetAuxAgents();
            ViewBag.Agents = new SelectList(agents, "AgentId", "AgentName");
            ViewBag.AgentStaffId = AgentStaffId;
            ViewBag.staffId = staffId;


            int pageSize = 10;
            int pageNumber = (page ?? 1);
            IPagedList<AuxAgentComplianceFormViewModel> result = _services.GetAuxAgentFormsList(AgentId).ToPagedList(pageNumber, pageSize);

            if (!string.IsNullOrEmpty(SendingCountry))
            {
                result = result.Where(x => x.CountryCode == SendingCountry).ToPagedList(pageNumber, pageSize);

            }
            if (!string.IsNullOrEmpty(Date))
            {
                string[] DateString = Date.Split('-');
                DateTime FromDate = Convert.ToDateTime(DateString[0]);
                DateTime ToDate = Convert.ToDateTime(DateString[1]);
                result = result.Where(x => x.SubDate.ToDateTime() >= FromDate && x.SubDate.ToDateTime() <= ToDate).ToPagedList(pageNumber, pageSize);
            }
            if (AgentId != 0)
            {
                result = result.Where(x => x.AgentId == AgentId).ToPagedList(pageNumber, pageSize);
            }
            if (FormId != 0)
            {
                result = result.Where(x => x.FormId == FormId).ToPagedList(pageNumber, pageSize);
            }
            if (!string.IsNullOrEmpty(AgentStaffId))
            {
                AgentStaffId = AgentStaffId.Trim();
                result = result.Where(x => x.AgentStaffId.ToLower().Contains(AgentStaffId.ToLower())).ToPagedList(pageNumber, pageSize);


            }
            if (!string.IsNullOrEmpty(staffId))
            {
                staffId = staffId.Trim();
                result = result.Where(x => x.StaffId.ToLower().Contains(staffId.ToLower())).ToPagedList(pageNumber, pageSize);

            }



            return View(result);
        }


        public ActionResult AgentStaffDetails(string agentStaffId = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            AgentStaffInformationViewModel vm = new AgentStaffInformationViewModel();
            vm = _services.getAgentStaffInformation(agentStaffId);
            return View(vm);
        }

        [HttpGet]
        public ActionResult UpdaeAgentStaffDetails(string AgentStaffId = "")
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");
            ViewBag.IssuingCountries = new SelectList(country, "Code", "Name");

            var cardTypes = Common.Common.GetIdCardType();
            ViewBag.IdCardType = new SelectList(cardTypes, "Name", "Name");

            AgentStaffInformationViewModel vm = new AgentStaffInformationViewModel();
            if (AgentStaffId != null)
            {
                vm = _services.getAgentStaffInformation(AgentStaffId);
                ViewBag.IdCardType = new SelectList(cardTypes, "Name", "Name", vm.IdCardType);

            }

            return View(vm);
        }
        [HttpPost]
        public ActionResult UpdaeAgentStaffDetails(AgentStaffInformationViewModel vm)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            var country = Common.Common.GetCountries();
            ViewBag.Country = new SelectList(country, "CountryCode", "CountryName");
            ViewBag.IssuingCountries = new SelectList(country, "Code", "Name");

            var cardTypes = Common.Common.GetIdCardType();
            ViewBag.IdCardType = new SelectList(cardTypes, "Name", "Name");

            bool modelIsValid = true;
            DateTime ExpiryDate = new DateTime();
            try
            {
                ExpiryDate = new DateTime(vm.ExpiryYear, (int)vm.ExpiryMonth, vm.ExpiryDay);

                if (ExpiryDate <= DateTime.Now)
                {


                    modelIsValid = false;
                    ModelState.AddModelError("ExpiryDay", "ID already expired");

                }
            }
            catch (Exception)
            {

                modelIsValid = false;
                ModelState.AddModelError("ExpiryDay", "ID already expired");
            }

            vm.IdCardExpiryDate = ExpiryDate;

            string Id1path = "";
            var Id1 = Request.Files["Id1"];
            if (Id1 != null && Id1.ContentLength > 0)
            {
                Id1path = Guid.NewGuid() + "." + Id1.FileName.Split('.')[1];
                Id1.SaveAs(Server.MapPath("~/Documents") + "\\" + Id1path);
                vm.Id1 = "/Documents/" + Id1path;

            }
            string Id2path = "";
            var Id2 = Request.Files["Id2"];
            if (Id2 != null && Id2.ContentLength > 0)
            {
                Id2path = Guid.NewGuid() + "." + Id2.FileName.Split('.')[1];
                Id2.SaveAs(Server.MapPath("~/Documents") + "\\" + Id2path);
                vm.Id2 = "/Documents/" + Id2path;

            }
            string Id3path = "";
            var Id3 = Request.Files["Id3"];
            if (Id3 != null && Id3.ContentLength > 0)
            {
                Id3path = Guid.NewGuid() + "." + Id3.FileName.Split('.')[1];
                Id3.SaveAs(Server.MapPath("~/Documents") + "\\" + Id3path);
                vm.Id3 = "/Documents/" + Id3path;

            }

            var data = _services.UpdateAgentStaffInformation(vm);
            if (data != null)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View(vm);
            }

        }
        public ActionResult Deactived(int Id = 0, bool IsActive = false)
        {
            _services.DeactivatedAgentStaff(Id, IsActive);
            return RedirectToAction("Index");
        }

        public ActionResult Deleted(int Id = 0)
        {
            _services.DeletedAgentInfo(Id);
            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult ComplianceSARForm(int Id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            SARFormServices _sarFormServices = new SARFormServices();
            var vm = _sarFormServices.List().Where(x => x.Id == Id).FirstOrDefault();
            vm.StaffAccountNo = _commonServices.getStaffMFSCode(StaffId);
            vm.StaffName = _commonServices.getStaffName(StaffId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ComplianceSARForm(SARForm model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            SARFormServices _sarFormServices = new SARFormServices();
            var vm = _sarFormServices.List().Where(x => x.Id == model.Id).FirstOrDefault();
            vm.IsSAR = model.IsSAR;
            vm.StaffName = model.StaffName;
            vm.StaffAccountNo = model.StaffAccountNo;
            vm.InvestigationDate = model.InvestigationDate;
            vm.ApprovedBy = StaffId;
            vm.ApprovedDate = DateTime.Now;
            vm.FormAction = FormStatus.Approved;
            _sarFormServices.Update(vm);

            return RedirectToAction("Index");
        }

        [HttpGet]

        public ActionResult ComplianceLargeFunds(int Id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            LargeFundMoneyTransferServices _LargeFundsSerives = new LargeFundMoneyTransferServices();
            var vm = _LargeFundsSerives.List().Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.AgentName = "";
            ViewBag.AgentAcountNo = "";
            ViewBag.AgentCountry = "";
            if (vm != null)
            {

                ViewBag.AgentName = _commonServices.getAgentName(vm.AgentId);
                var countryCode = _commonServices.GetAgents().Where(x => x.AgentId == vm.AgentId).Select(x => x.Country).FirstOrDefault();
                ViewBag.AgentCountry = _commonServices.getCountryNameFromCode(countryCode);
                ViewBag.AgentAcountNo = _commonServices.GetAgentAccNo(vm.AgentId);
                ViewBag.AgentStaffCode = _commonServices.GetAgentStaffMFSCode(vm.AgentStaffId);
            }
            return View(vm);

        }

        [HttpPost]

        public ActionResult ComplianceLargeFunds(LargeFundMoneyTransferFormData model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            LargeFundMoneyTransferServices _LargeFundsSerives = new LargeFundMoneyTransferServices();
            var vm = _LargeFundsSerives.List().Where(x => x.Id == model.Id).FirstOrDefault();
            vm.ApprovedStaffAccountNo = _commonServices.getStaffMFSCode(StaffId);
            vm.ApprovedStaffId = StaffId;
            vm.FormAction = FormStatus.Approved;
            vm.ApprovedDate = DateTime.Now;
            _LargeFundsSerives.Update(vm);
            return RedirectToAction("Index");
        }

        [HttpGet]

        public ActionResult ComplianceFundingSource(int Id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            SourceOfFundDeclarationServices _FundingSourcesServices = new SourceOfFundDeclarationServices();
            var vm = _FundingSourcesServices.List().Where(x => x.Id == Id).FirstOrDefault();
            ViewBag.AdminPhoneNo = "";
            ViewBag.AdminEmail = "";
            ViewBag.AgentAccountNo = "";
            if (vm != null)
            {

                ViewBag.AdminPhoneNo = _commonServices.getAdminStaffInformation().Where(x => x.Id == vm.AdminStaffId).Select(x => x.PhoneNumber).FirstOrDefault();
                ViewBag.AdminEmail = _commonServices.getAdminStaffInformation().Where(x => x.Id == vm.AdminStaffId).Select(x => x.EmailAddress).FirstOrDefault();
                ViewBag.AgentAccountNo = _commonServices.GetAgentAccNo(vm.AgentId);
                vm.ApprovedStaffAccountNo = _commonServices.getStaffMFSCode(StaffId);
                vm.AdminStaffName = _commonServices.getStaffName(StaffId);
            }
            return View(vm);

        }

        [HttpPost]

        public ActionResult ComplianceFundingSource(SourceOfFundDeclarationFormData model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            SourceOfFundDeclarationServices _FundingSourcesServices = new SourceOfFundDeclarationServices();
            var vm = _FundingSourcesServices.List().Where(x => x.Id == model.Id).FirstOrDefault();
            vm.ApprovedDate = DateTime.Now;
            vm.ApprovedStaffAccountNo = _commonServices.getStaffMFSCode(StaffId);
            vm.ApprovedStaffId = StaffId;
            vm.FormAction = FormStatus.Approved;
            _FundingSourcesServices.Update(vm);

            return RedirectToAction("Index");
        }


        [HttpGet]
        public ActionResult ComplianceAMLTraning(int Id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }

            AgentAMLTrainingRecordServices _amlTraningServices = new AgentAMLTrainingRecordServices();
            ViewBag.AgentStaffNameAndAddress = _amlTraningServices.List().Where(x => x.Id == Id).Select(x => x.AgentStaffNameAndAddress).FirstOrDefault();
            ViewBag.AgentStaffAccountNo = _amlTraningServices.List().Where(x => x.Id == Id).Select(x => x.AgentStaffAccountNo).FirstOrDefault();
            var Details = _amlTraningServices.ListDetails().Data.Where(x => x.AgentAMLTrainingRecordMasterId == Id).FirstOrDefault();

            return View(Details);
        }
        [HttpPost]
        public ActionResult ComplianceAMLTraning(AgentAMLTrainingRecordDetails model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            SARFormServices _sarFormServices = new SARFormServices();

            AgentAMLTrainingRecordServices _amlTraningServices = new AgentAMLTrainingRecordServices();
            var vm = _amlTraningServices.List().Where(x => x.Id == model.AgentAMLTrainingRecordMasterId).FirstOrDefault();
            vm.ApprovedDate = DateTime.Now;
            vm.FormAction = FormStatus.Approved;
            vm.ApprovedStaffAccountNo = _commonServices.getStaffMFSCode(StaffId);
            vm.ApprovedStaffId = StaffId;
            _amlTraningServices.Update(vm);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult ComplianceThirdPartyTransfer(int Id)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            ThirdPartyMoneyTransferServices _thirdPartyMoneyServices = new ThirdPartyMoneyTransferServices();
            var vm = _thirdPartyMoneyServices.List().Where(x => x.Id == Id).FirstOrDefault();
            vm.ApprovedDate = DateTime.Now;
            vm.ApprovedById = StaffId;
            vm.ApprovedByName = _commonServices.getStaffName(StaffId);
            return View(vm);
        }

        [HttpPost]
        public ActionResult ComplianceThirdPartyTransfer(ThirdPartyMoneyTransfer model)
        {
            if (Common.StaffSession.LoggedStaff == null)
            {
                return RedirectToAction("Index", "StaffHome", new { area = "staff" });
            }
            int StaffId = Common.StaffSession.LoggedStaff.StaffId;
            ThirdPartyMoneyTransferServices _thirdPartyMoneyServices = new ThirdPartyMoneyTransferServices();
            var vm = _thirdPartyMoneyServices.List().Where(x => x.Id == model.Id).FirstOrDefault();
            vm.ApprovedDate = DateTime.Now;
            vm.ApprovedById = StaffId;
            vm.ApprovedByName = _commonServices.getStaffName(StaffId);
            vm.AppovedTitle = model.AppovedTitle;
            vm.DeclinedByName = model.DeclinedByName;
            vm.DeclinedDate = model.DeclinedDate;
            vm.DeclinedTitle = model.DeclinedTitle;
            vm.ApprovedStaffAccountNo = _commonServices.getStaffMFSCode(StaffId);
            vm.FormAction = FormStatus.Approved;
            _thirdPartyMoneyServices.Update(vm);
            return RedirectToAction("Index");

        }

    }
}