using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentFormsController : Controller
    {

        AgentServices.AgentFormServices _agentFormServices = null;
        public AgentFormsController()
        {
            _agentFormServices = new AgentServices.AgentFormServices();
        }
        // GET: Agent/AgentForms
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SARForms(string Year = "", int MonthId = 0)
        {


            var month = (Month)MonthId;

            Models.MySubmittedSARFormVm vm = new MySubmittedSARFormVm();
            vm.month = month;
            vm.Year = Year;
            vm.MySubmittedSARFormDetials = _agentFormServices.GetMySubmittedSARForms(Year, month);

            return View(vm);

        }

        public ActionResult ThirdPartyMoneyTransfer(string Year = "", int MonthId = 0)
        {


            var month = (Month)MonthId;

            Models.ThirdPartyMoneyTransferDetailVM vm = new ThirdPartyMoneyTransferDetailVM();
            vm.month = month;
            vm.Year = Year;
            vm.ThirdPartyMoneyTransferList = _agentFormServices.GetThirdPartyMoneyTransferDetails(Year, month);

            return View(vm);


        }

        public ActionResult LargeFundMoneyTransfer(string Year = "", int MonthId = 0)
        {

            var month = (Month)MonthId;

            Models.LargeFundMoneyTransferDetailVM vm = new LargeFundMoneyTransferDetailVM();
            vm.month = month;
            vm.Year = Year;
            vm.LargeFundMoneyTransferList = _agentFormServices.GetLargeFundMoneyTransferDetails(Year, month);

            return View(vm);

        }

        public ActionResult AgentAMLTrainingRecord(string Year = "", int MonthId = 0)
        {
            var month = (Month)MonthId;

            Models.AgentAMLTrainingRecordDetialVM vm = new AgentAMLTrainingRecordDetialVM();
            vm.month = month;
            vm.Year = Year;
            vm.AgentAMLTrainingRecordGridVM = _agentFormServices.GetAgentAMLTrainingRecordDetails(Year, month);

            return View(vm);

        }

        public ActionResult SourceOfFundDeclaration(string Year = "", int MonthId = 0)
        {

            var month = (Month)MonthId;

            Models.SourceOfFundDeclarationDetailVM vm = new SourceOfFundDeclarationDetailVM();
            vm.month = month;
            vm.Year = Year;
            vm.SourceOfFundDeclarationGridVM = _agentFormServices.GetSourceOfFundDeclarationDetails(Year, month);

            return View(vm);

        }
    }
}