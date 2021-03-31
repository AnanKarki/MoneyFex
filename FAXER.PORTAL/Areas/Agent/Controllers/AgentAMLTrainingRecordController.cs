using FAXER.PORTAL.Areas.Agent.AgentServices;
using FAXER.PORTAL.Areas.Agent.Models;
using FAXER.PORTAL.Common;
using FAXER.PORTAL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent.Controllers
{
    public class AgentAMLTrainingRecordController : Controller
    {

        AgentAMLTrainingRecordServices _agentAMLTrainingRecordServices = null;
        public AgentAMLTrainingRecordController()
        {
            _agentAMLTrainingRecordServices = new AgentAMLTrainingRecordServices();
        }
        // GET: Agent/AgentAMLTrainingRecord

        [HttpGet]
        public ActionResult Index()
        {
            AgentInformation agentInfo = Common.AgentSession.AgentInformation ?? new AgentInformation();
            int AgentId = agentInfo.Id;
            if (AgentId == 0)
            {
                return RedirectToAction("Login", "AgentLogin", new { area = "agent" });
            }
           
            AgentAMLTrainingRecordVM vm = new AgentAMLTrainingRecordVM();
            vm.AgentStaffNameAndAddress = _agentAMLTrainingRecordServices.GetNameAndAddressOfAgent(AgentId);
            vm.AgentStaffAccountNo = _agentAMLTrainingRecordServices.GetAgenStaffAccNo(AgentId);
            AgentResult agentResult = new AgentResult();

            agentResult.Message = "";
            ViewBag.AgentResult = agentResult;

            return View(vm);
        }
        [HttpPost]
        public ActionResult Index([Bind(Include = AgentAMLTrainingRecordVM.BindProperty)] AgentAMLTrainingRecordVM vm) {

            AgentResult agentResult = new AgentResult();
            if (ModelState.IsValid) {

                bool result =  _agentAMLTrainingRecordServices.CreateAgentAMLTrainingRecord(vm);

                agentResult.Message = "Agent Training Record added successfully";
                agentResult.Status = ResultStatus.OK;
                ViewBag.AgentResult = agentResult;
                ModelState.Clear();
                return View(vm);
            }
            agentResult.Message = "";
            ViewBag.AgentResult = agentResult;
            return View(vm);
        }
   
    }
}