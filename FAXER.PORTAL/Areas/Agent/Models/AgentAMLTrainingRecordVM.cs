using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.Areas.Agent.Models
{
    public class AgentAMLTrainingRecordVM
    {


        public const string BindProperty = "AgentStaffNameAndAddress , AgentStaffAccountNo ,AgentAMLTrainingRecordDetails";

        [Required(ErrorMessage ="Staff name and address is required")]
        public string AgentStaffNameAndAddress { get; set; }

        [Required(ErrorMessage = "Staff account no is required")]
        public string AgentStaffAccountNo { get; set; }
        
        public List<AgentAMLTrainingRecordDetailsVM> AgentAMLTrainingRecordDetails { get; set; }

    }

    public class AgentAMLTrainingRecordDetailsVM {

        public DateTime DateOfTraining { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeInitials { get; set; }

        //Type of Training 
        public bool IsInitial { get; set; }
        public bool IsOnGoing { get; set; }
        public string TrainingConductedBy { get; set; }

    }
    

}