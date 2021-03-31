using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Agent
{
    public class AgentAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Agent";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "Agent_default",
                "Agent/{controller}/{action}/{id}",
                new {controller= "AgentLogin", action = "Login", id = UrlParameter.Optional }
            );
        }
    }
}