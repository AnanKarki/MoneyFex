using FAXER.PORTAL.Areas.Agent.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Admin.Controllers
{
    public class AgentAMLTrainingRecordController : Controller
    {
        // GET: Admin/AgentAMLTrainingRecord
        public ActionResult Index()
        {
            List<AgentAMLTrainingRecordGridVM> vm = new List<AgentAMLTrainingRecordGridVM>();
            return View(vm);
        }

        public ActionResult SubmitAgentAMLTraningRecord()
        {
            AgentAMLTrainingRecordVM vm = new AgentAMLTrainingRecordVM();
            return View(vm);
        }
    }
}