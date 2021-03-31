using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.EmailTemplate
{
    public class EmailTemplateAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "EmailTemplate";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "EmailTemplate_default",
                "EmailTemplate/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}