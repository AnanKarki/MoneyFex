using System.Web.Mvc;

namespace FAXER.PORTAL.Areas.KiiPayBusiness
{
    public class KiiPayBusinessAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "KiiPayBusiness";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "KiiPayBusiness_default",
                "KiiPayBusiness/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}