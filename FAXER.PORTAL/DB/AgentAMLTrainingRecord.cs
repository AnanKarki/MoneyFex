using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FAXER.PORTAL.DB
{
    public class AgentAMLTrainingRecordMaster
    {
        public int Id { get; set; }

        public string AgentStaffNameAndAddress { get; set; }
        public string AgentStaffAccountNo { get; set; }

        public int AgentStaffId { get; set; }
        public int AgentId { get; set; }
        public DateTime SubmittedDate { get; set; }
        public FormStatus FormAction { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public int ApprovedStaffId { get; set; }
        public string ApprovedStaffAccountNo { get; set; }

        public virtual AgentStaffInformation AgentStaff { get; set; }


    }
    public class AgentAMLTrainingRecordDetails
    {

        public int Id { get; set; }
        public DateTime DateOfTraining { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeInitials { get; set; }

        //Type of Training 
        public bool IsInitial { get; set; }
        public bool IsOnGoing { get; set; }
        public string    TrainingConductedBy { get; set; }

        public int AgentAMLTrainingRecordMasterId { get; set; }

        public virtual AgentAMLTrainingRecordMaster AgentAMLTrainingRecordMaster { get; set; }



    }

}