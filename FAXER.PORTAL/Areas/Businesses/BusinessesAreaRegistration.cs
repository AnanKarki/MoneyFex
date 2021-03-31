using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.Businesses
{
    public class BusinessesAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Businesses";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            
            context.MapRoute(
                "Businesses_default",
                "Businesses/{controller}/{action}/{id}",
                //new { action = "Index", id = UrlParameter.Optional }
                new { controller = "BusinessHome", action = "Home", id = UrlParameter.Optional }
                
            );
        }
    }
}